Imports System.Security.Policy
Imports Graph3D
Imports MathNet
Imports MathNet.Numerics




' https://forum.hptuners.com/showthread.php?102906-VVE-Assistant-exe&p=723773&viewfull=1#post723773
'FYI here Is the formula For 4-6 cyl DOHC engines with variable exhaust cams

'[Constant]+([MAP] * MAP) + ([MAP^2]*MAP^2)+([MAP*RPM]*MAP*RPM)+([MAP_Intake]*Intake*MAP)+([MAP_Exhaust]*Exhaust*MAP)+([RPM]*RPM)+([RPM^2]*RPM^2)+([RPM_Intake]*Intake*RPM)+([RPM_Exhaust]*Exhaust*RPM)+([Intake]*Intake)+([Intake^2]*Intake^2)+([Exhaust]*Exhaust)+([Exhaust^2]*Exhaust^2)+([Intake_Exhaust]*Intake*Exhaust) = VVE cell value

'Where:
'Square brackets = value In that coefficient table
'MAP = MAP value (row axis) In VVE table
'RPM = RPM value (column axis) In VVE table
'Intake = intake cam angle selected In VVE table
'Exhaust = exhaust cam angle selected In VVE table



Public Class clsMath

    Dim SMALL_ZONE_WARN As Boolean = True

    Public Const CAM_MIN_ANGLE As Integer = 0
    Public Const CAM_MAX_ANGLE As Integer = 64 'https://forum.hptuners.com/showthread.php?102906-VVE-Assistant-exe&p=723459&viewfull=1#post723459

    Public Enum INTERPOLATE_DIRECTION
        Vertical
        Horizontal
    End Enum

    Public Sub New(ByVal MIN_ZONE_SIZE_WARN As Boolean)
        SMALL_ZONE_WARN = MIN_ZONE_SIZE_WARN
    End Sub

    Enum MERGE_OPERATION
        AVERAGE
        KEEP_OLD
        KEEP_NEW
    End Enum

    Public Function CalcVVEAirmass(ByVal ConstCoeff As Double,
                                   ByVal MAPCoeff As Double,
                                   ByVal MAP2Coeff As Double,
                                   ByVal RPMCoeff As Double,
                                   ByVal RPM2Coeff As Double,
                                   ByVal MAPRPMCoeff As Double,
                                   ByVal MAP As Double,
                                   ByVal RPM As Double,
                                   ByVal VERLON_MODE As Boolean) As Double
        ' VVE Airmass = (ConstCoeff)
        '             + (MAPCoeff * MAP)
        '             + (MAP2Coeff * MAP^2)
        '             + (RPMCoeff * RPM)
        '             + (RPM2Coeff * RPM^2)
        '             + (MAPRPMCoeff * MAP * RPM)

        ' Final Airmass = VVE Airmass + Intake Cam Airmass + Exhaust Cam Airmass

        If VERLON_MODE Then
            ' https://forum.hptuners.com/showthread.php?106946-Failed-MAF-Intermittent-Stumble&p=760034&viewfull=1#post760034
            Return (ConstCoeff) + (MAPCoeff * MAP) + (RPMCoeff * RPM) + (RPM2Coeff * RPM ^ 2) + (MAPRPMCoeff * MAP * RPM)
        Else
            Return (ConstCoeff) + (MAPCoeff * MAP) + (MAP2Coeff * MAP ^ 2) + (RPMCoeff * RPM) + (RPM2Coeff * RPM ^ 2) + (MAPRPMCoeff * MAP * RPM)
        End If

    End Function

    Public Function CalcIntakeCamAirmass(ByVal MAPInCamCoeff As Double,
                                         ByVal RPMInCamCoeff As Double,
                                         ByVal InCamCoeff As Double,
                                         ByVal InCam2Coeff As Double,
                                         ByVal InCamAngle As Double,
                                         ByVal MAP As Double,
                                         ByVal RPM As Double) As Double


        ' Intake Cam Airmass = (MAP.IntakeCam Coeff * MAP * Cam Degrees)
        '                    + (RPM.IntakeCam Coeff * RPM * Cam Degrees)
        '                    + (IntakeCam Coeff * Cam Degrees)
        '                    + (IntakeCam2 Coeff * Cam Degrees * Cam Degrees)

        ' Final Airmass = VVE Airmass + Intake Cam Airmass + Exhaust Cam Airmass

        If InCamAngle > CAM_MIN_ANGLE And InCamAngle <= CAM_MAX_ANGLE Then
            Return (MAPInCamCoeff * MAP * InCamAngle) + (RPMInCamCoeff * RPM * InCamAngle) + (InCamCoeff * InCamAngle) + (InCam2Coeff * InCamAngle * InCamAngle)
        Else
            Return 0.0
        End If
    End Function

    Public Function CalcExhaustCamAirmass(ByVal MAPExCamCoeff As Double,
                                          ByVal RPMExCamCoeff As Double,
                                          ByVal ExCamCoeff As Double,
                                          ByVal ExCam2Coeff As Double,
                                          ByVal ExCamInCamCoeff As Double,
                                          ByVal ExCamAngle As Double,
                                          ByVal InCamAngle As Double,
                                          ByVal MAP As Double,
                                          ByVal RPM As Double) As Double


        ' Exhaust Cam Airmass = (MAP.ExhaustCam Coeff * MAP * Cam Degrees)
        '                     + (RPM.ExhaustCam Coeff * RPM * Cam Degrees)
        '                     + (ExhaustCam Coeff * Cam Degrees)
        '                     + (ExhaustCam2 Coeff * Cam Degrees * Cam Degrees)
        '                     + (ExhaustCam.IntakeCam Coeff * Cam Degrees)  <- this last one is a GUESS! Need to test

        ' Final Airmass = VVE Airmass + Intake Cam Airmass + Exhaust Cam Airmass

        If ExCamAngle > CAM_MIN_ANGLE And ExCamAngle <= CAM_MAX_ANGLE Then
            Return (MAPExCamCoeff * MAP * ExCamAngle) + (RPMExCamCoeff * RPM * ExCamAngle) + (ExCamCoeff * ExCamAngle) + (ExCam2Coeff * ExCamAngle * ExCamAngle) + (ExCamInCamCoeff * ExCamAngle * InCamAngle)
        Else
            Return 0.0
        End If
    End Function





    Public Function FindAllVVECoefficients(ByVal DT_VVE As DataTable,
                                           ByVal ROW_HEADERS() As String,
                                           ByVal RPM_Boundaries As DataTable,
                                           ByVal MAP_Boundaries As DataTable,
                                           ByVal DUPLICATE_RESULT_ROWS As Boolean,
                                           ByRef DO_NOT_WARN_AGAIN As Boolean,
                                           ByVal VERLON_MODE As Boolean) As DataTable()


        If DT_VVE Is Nothing Then
            Return Nothing
        End If
        If DT_VVE.Rows.Count = 0 Then
            Return Nothing
        End If
        If DT_VVE.Columns.Count = 0 Then
            Return Nothing
        End If
        If ROW_HEADERS Is Nothing Then
            Return Nothing
        End If
        If ROW_HEADERS.Length = 0 Then
            Return Nothing
        End If
        If DT_VVE.Rows.Count <> ROW_HEADERS.Length Then
            Return Nothing
        End If
        If RPM_Boundaries Is Nothing Then
            Return Nothing
        End If
        If RPM_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If RPM_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries Is Nothing Then
            Return Nothing
        End If
        If MAP_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If

        Dim DT_CONST As New DataTable()
        Dim DT_MAP As New DataTable()
        Dim DT_MAP2 As New DataTable()
        Dim DT_RPM As New DataTable()
        Dim DT_RPM2 As New DataTable()
        Dim DT_MAPRPM As New DataTable()

        ' create a column for each zone (0-29)
        For i As Integer = 0 To 29
            DT_CONST.Columns.Add(i.ToString, GetType(Double))
        Next
        ' replicate the columns to all the tables
        DT_MAP = DT_CONST.Clone
        DT_MAP2 = DT_CONST.Clone
        DT_RPM = DT_CONST.Clone
        DT_RPM2 = DT_CONST.Clone
        DT_MAPRPM = DT_CONST.Clone

        Dim ROW_CONST As DataRow = DT_CONST.NewRow()
        Dim ROW_MAP As DataRow = DT_MAP.NewRow()
        Dim ROW_MAP2 As DataRow = DT_MAP2.NewRow()
        Dim ROW_RPM As DataRow = DT_RPM.NewRow()
        Dim ROW_RPM2 As DataRow = DT_RPM2.NewRow()
        Dim ROW_MAPRPM As DataRow = DT_MAPRPM.NewRow()

        Dim RPM() As Double
        Dim MAP() As Double
        Dim VVE() As Double
        Dim COEFF() As Double

        Dim objZones As New clsZones(RPM_Boundaries, MAP_Boundaries, ROW_HEADERS)
        Dim CURRENT_RPM As Double = 0.0
        Dim CURRENT_MAP As Double = 0.0
        Dim CURRENT_VVE As Double = 0.0
        Dim ZONE_LOOKUP As Integer = -1


        For CURRENT_ZONE = 0 To 29


            ReDim RPM(-1)
            ReDim MAP(-1)
            ReDim VVE(-1)
            ReDim COEFF(-1)




            For COL As Integer = 0 To DT_VVE.Columns.Count - 1
                For ROW As Integer = 0 To DT_VVE.Rows.Count - 1


                    If Not IsNumeric(DT_VVE.Rows(ROW).Item(COL)) Then
                        ' we are passing in blanks (a sparse dataset)
                        ' only add x,y,z data IF there is a real number
                        Continue For
                    End If



                    CURRENT_RPM = CDbl(DT_VVE.Columns(COL).ColumnName)
                    CURRENT_MAP = CDbl(ROW_HEADERS(ROW))
                    CURRENT_VVE = CDbl(DT_VVE.Rows(ROW).Item(COL))
                    ZONE_LOOKUP = objZones.WhatZoneAmI(CURRENT_RPM, CURRENT_MAP)

                    If ZONE_LOOKUP = CURRENT_ZONE Then

                        If Array.IndexOf(RPM, CURRENT_RPM) = -1 Then
                            ReDim Preserve RPM(RPM.Length)
                            RPM(RPM.Length - 1) = CURRENT_RPM
                        End If

                        If Array.IndexOf(MAP, CURRENT_MAP) = -1 Then
                            ReDim Preserve MAP(MAP.Length)
                            MAP(MAP.Length - 1) = CURRENT_MAP
                        End If

                        ReDim Preserve VVE(VVE.Length)
                        VVE(VVE.Length - 1) = CURRENT_VVE

                    ElseIf ZONE_LOOKUP > CURRENT_ZONE Then
                        ' break condition
                        ' this will save a little time here on the inner loop,
                        ' but we will still have to keeping looping on the outer loop
                        Continue For
                    End If
                Next

            Next

            ' calc the coefficients for this zone
            COEFF = FindVVECoefficients(RPM, MAP, VVE, VERLON_MODE)

            ' save the results
            If COEFF.Length = 6 Then
                ROW_CONST(CURRENT_ZONE) = COEFF(0)
                ROW_MAP(CURRENT_ZONE) = COEFF(1)
                ROW_MAP2(CURRENT_ZONE) = COEFF(2)
                ROW_RPM(CURRENT_ZONE) = COEFF(3)
                ROW_RPM2(CURRENT_ZONE) = COEFF(4)
                ROW_MAPRPM(CURRENT_ZONE) = COEFF(5)
            End If


            If COEFF.Length = 5 Then
                ' VERLON MODE
                ROW_CONST(CURRENT_ZONE) = COEFF(0)
                ROW_MAP(CURRENT_ZONE) = COEFF(1)
                ROW_MAP2(CURRENT_ZONE) = 0.0
                ROW_RPM(CURRENT_ZONE) = COEFF(2)
                ROW_RPM2(CURRENT_ZONE) = COEFF(3)
                ROW_MAPRPM(CURRENT_ZONE) = COEFF(4)
            End If



        Next

        ' add the results to the final datatables

        DT_CONST.Rows.Add(ROW_CONST)
        DT_MAP.Rows.Add(ROW_MAP)
        DT_MAP2.Rows.Add(ROW_MAP2)
        DT_RPM.Rows.Add(ROW_RPM)
        DT_RPM2.Rows.Add(ROW_RPM2)
        DT_MAPRPM.Rows.Add(ROW_MAPRPM)


        ' duplicate rows if desired (this will help user to establish the initial starting values before the tuning process)
        If DUPLICATE_RESULT_ROWS Then
            DT_CONST.ImportRow(ROW_CONST)
            DT_MAP.ImportRow(ROW_MAP)
            DT_MAP2.ImportRow(ROW_MAP2)
            DT_RPM.ImportRow(ROW_RPM)
            DT_RPM2.ImportRow(ROW_RPM2)
            DT_MAPRPM.ImportRow(ROW_MAPRPM)
        End If




        DO_NOT_WARN_AGAIN = SMALL_ZONE_WARN
        Return {DT_CONST, DT_MAP, DT_MAP2, DT_RPM, DT_RPM2, DT_MAPRPM}

    End Function

    Public Function FindVVECoefficients(ByVal RPM As Double(), ByVal MAP As Double(), ByVal VVE As Double(), ByVal VERLON_MODE As Boolean) As Double()
        'IN:
        ' RPM Array for a single zone (this is the X-axis)
        ' MAP Array for a single zone (this is the Y-axis)
        ' VVE Array for a single zone (this is the Z-axis)
        '
        ' Transform:
        ' A Jagged Array (array of arrays) will be created from the XY values. These will be the coordinate pairs...
        '   {x1, y1}
        '   {x2, y2}
        '   {x3, y3}
        '   ...
        '   {x100, y100}
        '
        'The VVE (or Z-axis) then will need to be in the same order, but it will just be a standard array...
        '   {z1}
        '   {z2}
        '   {z3}
        '   ...
        '   {z100}
        '
        '
        'OUT:
        ' An array of 6 coefficients will be returned


        SmallZoneWarn(RPM.Length, MAP.Length)
        'If SMALL_ZONE_WARN Then
        '    Dim MSG As String = "One of your RPM/MAP zones is too narrow. It needs to be at least 3 columns/rows wide (the wider the better). " &
        '                        "Do you want to ignore this and try to Calculate Coefficients anyway? This can produce very unstable results."
        '    If RPM.Length < 3 Or MAP.Length < 3 Then
        '        If MsgBox(MSG, MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Send It?") = MsgBoxResult.Yes Then
        '            SMALL_ZONE_WARN = False
        '        Else
        '            Throw New Exception("User Canceled.")
        '        End If
        '    End If
        'End If

        ' MathNet will error without at least 6 samples.
        ' I should probably ensure there is at least 3 in each X and Y coordinate though...
        If VVE.Length < 7 Then
            Return {}
        End If

        If VVE.Length <> RPM.Length * MAP.Length Then
            Return {}
        End If

        ' define array of arrays, lets anticipate the total size so we don't have to [redim] it and have a performance hit
        Dim xy = New Double((RPM.Length * MAP.Length) - 1)() {}
        Dim x As Integer = 0
        For i As Integer = 0 To RPM.Length - 1
            For j As Integer = 0 To MAP.Length - 1
                xy(x) = {RPM(i), MAP(j)}
                x += 1
            Next
        Next

        ' This is defining the model function
        ' you might be able to use the syntax below for: Dim f As Func(Of Double(), Double) = Function(d)
        ' but for now, chaining the function call with the model function definition seems to work...
        ' look at this post for clues: https://stackoverflow.com/questions/22673404/multiple-linear-regression-math-net-2-6-with-fit-linearmultidim


        'VVE = (ConstCoeff) + (MAPCoeff * MAP) + (MAP2Coeff * MAP ^ 2) + (RPMCoeff * RPM) + (RPM2Coeff * RPM ^ 2) + (MAPRPMCoeff * MAP * RPM)
        'd(0) = x = RPM
        'd(1) = y = MAP



        If VERLON_MODE Then
            ' Return 5 coefficients (sans MAP^2)
            Return Fit.LinearMultiDim(xy,
                                  VVE,
                                  Function(d) 1.0,'         ConstCoeff
                                  Function(d) d(1),'        MAPCoeff * MAP
                                  Function(d) d(0),'        RPMCoeff * RPM
                                  Function(d) d(0) ^ 2,'    RPM2Coeff * RPM ^ 2
                                  Function(d) d(1) * d(0)) 'MAPRPMCoeff * MAP * RPM
        Else
            ' Return all standard 6 coefficients
            Return Fit.LinearMultiDim(xy,
                                  VVE,
                                  Function(d) 1.0,'         ConstCoeff
                                  Function(d) d(1),'        MAPCoeff * MAP
                                  Function(d) d(1) ^ 2,'    MAP2Coeff * MAP ^ 2
                                  Function(d) d(0),'        RPMCoeff * RPM
                                  Function(d) d(0) ^ 2,'    RPM2Coeff * RPM ^ 2
                                  Function(d) d(1) * d(0)) 'MAPRPMCoeff * MAP * RPM
        End If



        'Dim f As Func(Of Double(), Double) = Function(d)
        '                                         'd(0) = x
        '                                         'd(1) = y
        '                                         Dim p0 = d(0)
        '                                         Dim p1 = Math.Tanh(d(0))
        '                                         Dim p2 = Math.Tanh(d(1))
        '                                         Dim p3 = d(0)
        '                                         Dim p4 = d(0) * d(1)
        '                                         'p0 + p1*tanh(x) + p2*tanh(y) + p3*x + p4*x*y

        '                                         Return p0 + p1 + p2 + p3 + p4

        '                                     End Function


    End Function




    Public Function FindAllIntakeCoefficients(ByVal DT_BASE_VVE As DataTable,
                                              ByVal DT_INTAKE_VVE As DataTable,
                                              ByVal ROW_HEADERS() As String,
                                              ByVal RPM_Boundaries As DataTable,
                                              ByVal MAP_Boundaries As DataTable,
                                              ByVal DUPLICATE_RESULT_ROWS As Boolean,
                                              ByRef DO_NOT_WARN_AGAIN As Boolean,
                                              Optional ByVal CAM_ANGLE As Integer = 20) As DataTable()


        If DT_BASE_VVE Is Nothing Then
            Return Nothing
        End If
        If DT_BASE_VVE.Rows.Count = 0 Then
            Return Nothing
        End If
        If DT_BASE_VVE.Columns.Count = 0 Then
            Return Nothing
        End If
        If DT_INTAKE_VVE Is Nothing Then
            Return Nothing
        End If
        If DT_INTAKE_VVE.Rows.Count = 0 Then
            Return Nothing
        End If
        If DT_INTAKE_VVE.Columns.Count = 0 Then
            Return Nothing
        End If
        If DT_INTAKE_VVE.Rows.Count <> DT_BASE_VVE.Rows.Count Then
            Return Nothing
        End If
        If DT_INTAKE_VVE.Columns.Count <> DT_BASE_VVE.Columns.Count Then
            Return Nothing
        End If
        If ROW_HEADERS Is Nothing Then
            Return Nothing
        End If
        If ROW_HEADERS.Length = 0 Then
            Return Nothing
        End If
        If DT_BASE_VVE.Rows.Count <> ROW_HEADERS.Length Then
            Return Nothing
        End If
        If RPM_Boundaries Is Nothing Then
            Return Nothing
        End If
        If RPM_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If RPM_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries Is Nothing Then
            Return Nothing
        End If
        If MAP_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If

        ' subtract the base VVE airmass from the CAM Airmass
        For i As Integer = 0 To DT_INTAKE_VVE.Rows.Count - 1
            For j As Integer = 0 To DT_INTAKE_VVE.Columns.Count - 1
                If DT_INTAKE_VVE.Columns(j).ColumnName <> DT_BASE_VVE.Columns(j).ColumnName Then
                    Return Nothing
                End If

                DT_INTAKE_VVE.Rows(i).Item(j) = DT_INTAKE_VVE.Rows(i).Item(j) - DT_BASE_VVE.Rows(i).Item(j)
            Next
        Next

        ' now that we have untangled the base and cam VVE's, we can calc coeff for just the cam airmass



        Dim DT_MAP As New DataTable()
        Dim DT_RPM As New DataTable()
        Dim DT_CAM As New DataTable()
        Dim DT_CAM2 As New DataTable()

        ' create a column for each zone (0-29)
        For i As Integer = 0 To 29
            DT_MAP.Columns.Add(i.ToString, GetType(Double))
        Next
        ' replicate the columns to all the tables
        DT_RPM = DT_MAP.Clone
        DT_CAM = DT_MAP.Clone
        DT_CAM2 = DT_MAP.Clone


        Dim ROW_MAP As DataRow = DT_MAP.NewRow()
        Dim ROW_RPM As DataRow = DT_RPM.NewRow()
        Dim ROW_CAM As DataRow = DT_CAM.NewRow()
        Dim ROW_CAM2 As DataRow = DT_CAM2.NewRow()


        Dim RPM() As Double
        Dim MAP() As Double
        Dim VVE() As Double
        Dim COEFF() As Double

        Dim objZones As New clsZones(RPM_Boundaries, MAP_Boundaries, ROW_HEADERS)
        Dim CURRENT_RPM As Double = 0.0
        Dim CURRENT_MAP As Double = 0.0
        Dim CURRENT_VVE As Double = 0.0
        Dim ZONE_LOOKUP As Integer = -1


        For CURRENT_ZONE = 0 To 29


            ReDim RPM(-1)
            ReDim MAP(-1)
            ReDim VVE(-1)
            ReDim COEFF(-1)




            For COL As Integer = 0 To DT_INTAKE_VVE.Columns.Count - 1
                For ROW As Integer = 0 To DT_INTAKE_VVE.Rows.Count - 1

                    CURRENT_RPM = CDbl(DT_INTAKE_VVE.Columns(COL).ColumnName)
                    CURRENT_MAP = CDbl(ROW_HEADERS(ROW))
                    CURRENT_VVE = CDbl(DT_INTAKE_VVE.Rows(ROW).Item(COL))
                    ZONE_LOOKUP = objZones.WhatZoneAmI(CURRENT_RPM, CURRENT_MAP)

                    If ZONE_LOOKUP = CURRENT_ZONE Then

                        If Array.IndexOf(RPM, CURRENT_RPM) = -1 Then
                            ReDim Preserve RPM(RPM.Length)
                            RPM(RPM.Length - 1) = CURRENT_RPM
                        End If

                        If Array.IndexOf(MAP, CURRENT_MAP) = -1 Then
                            ReDim Preserve MAP(MAP.Length)
                            MAP(MAP.Length - 1) = CURRENT_MAP
                        End If

                        ReDim Preserve VVE(VVE.Length)
                        VVE(VVE.Length - 1) = CURRENT_VVE

                    ElseIf ZONE_LOOKUP > CURRENT_ZONE Then
                        ' break condition
                        ' this will save a little time here on the inner loop,
                        ' but we will still have to keeping looping on the outer loop
                        Continue For
                    End If
                Next

            Next

            ' calc the coefficients for this zone
            COEFF = FindIntakeCoefficients(RPM, MAP, VVE, CAM_ANGLE)

            ' save the results
            ROW_MAP(CURRENT_ZONE) = COEFF(0)
            ROW_RPM(CURRENT_ZONE) = COEFF(1)
            ROW_CAM(CURRENT_ZONE) = COEFF(2)
            ROW_CAM2(CURRENT_ZONE) = COEFF(3)

        Next

        ' add the results to the final datatables
        DT_MAP.Rows.Add(ROW_MAP)
        DT_RPM.Rows.Add(ROW_RPM)
        DT_CAM.Rows.Add(ROW_CAM)
        DT_CAM2.Rows.Add(ROW_CAM2)



        ' duplicate rows if desired (this will help user to establish the initial starting values before the tuning process)
        If DUPLICATE_RESULT_ROWS Then
            DT_MAP.ImportRow(ROW_MAP)
            DT_RPM.ImportRow(ROW_RPM)
            DT_CAM.ImportRow(ROW_CAM)
            DT_CAM2.ImportRow(ROW_CAM2)
        End If




        DO_NOT_WARN_AGAIN = SMALL_ZONE_WARN
        Return {DT_MAP, DT_RPM, DT_CAM, DT_CAM2}

    End Function

    Public Function FindIntakeCoefficients(ByVal RPM As Double(), ByVal MAP As Double(), VVE As Double(), ByVal CAM_ANGLE As Integer) As Double()
        'IN:
        ' RPM Array for a single zone (this is the X-axis)
        ' MAP Array for a single zone (this is the Y-axis)
        ' VVE Array for a single zone (this is the Z-axis)
        '
        ' Transform:
        ' A Jagged Array (array of arrays) will be created from the XY values. These will be the coordinate pairs...
        '   {x1, y1}
        '   {x2, y2}
        '   {x3, y3}
        '   ...
        '   {x100, y100}
        '
        'The VVE (or Z-axis) then will need to be in the same order, but it will just be a standard array...
        '   {z1}
        '   {z2}
        '   {z3}
        '   ...
        '   {z100}
        '
        '
        'OUT:
        ' An array of 6 coefficients will be returned

        SmallZoneWarn(RPM.Length, MAP.Length)
        'If SMALL_ZONE_WARN Then
        '    Dim MSG As String = "One of your RPM/MAP zones is too narrow. It needs to be at least 3 columns/rows wide (the wider the better). " &
        '                        "Do you want to ignore this and try to Calculate Coefficients anyway? This can produce very unstable results."
        '    If RPM.Length < 3 Or MAP.Length < 3 Then
        '        If MsgBox(MSG, MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Send It?") = MsgBoxResult.Yes Then
        '            SMALL_ZONE_WARN = False
        '        Else
        '            Throw New Exception("User Canceled.")
        '        End If
        '    End If
        'End If

        If VVE.Length <> RPM.Length * MAP.Length Then
            Return {}
        End If

        ' define array of arrays, lets anticipate the total size so we don't have to [redim] it and have a performance hit
        Dim xy = New Double((RPM.Length * MAP.Length) - 1)() {}
        Dim x As Integer = 0
        For i As Integer = 0 To RPM.Length - 1
            For j As Integer = 0 To MAP.Length - 1
                xy(x) = {RPM(i), MAP(j)}
                x += 1
            Next
        Next

        ' This is defining the model function
        ' you might be able to use the syntax below for: Dim f As Func(Of Double(), Double) = Function(d)
        ' but for now, chaining the function call with the model function definition seems to work...
        ' look at this post for clues: https://stackoverflow.com/questions/22673404/multiple-linear-regression-math-net-2-6-with-fit-linearmultidim


        ' d(0) = x = RPM
        ' d(1) = y = MAP
        ' Intake Cam Airmass = (MAP.IntakeCam Coeff * MAP * Cam Degrees)
        '                    + (RPM.IntakeCam Coeff * RPM * Cam Degrees)
        '                    + (IntakeCam Coeff * Cam Degrees)
        '                    + (IntakeCam2 Coeff * Cam Degrees * Cam Degrees)


        ' Return the 4 coefficients
        Return Fit.LinearMultiDim(xy,
                                  VVE,
                                  Function(d) d(1) * CAM_ANGLE,   ' MAP.IntakeCam Coeff * MAP * Cam Degrees
                                  Function(d) d(0) * CAM_ANGLE,   ' RPM.IntakeCam Coeff * RPM * Cam Degrees
                                  Function(d) CAM_ANGLE,          ' IntakeCam Coeff * Cam Degrees
                                  Function(d) CAM_ANGLE ^ 2)      ' IntakeCam2 Coeff * Cam Degrees * Cam Degrees



        'Dim f As Func(Of Double(), Double) = Function(d)
        '                                         'd(0) = x
        '                                         'd(1) = y
        '                                         Dim p0 = d(0)
        '                                         Dim p1 = Math.Tanh(d(0))
        '                                         Dim p2 = Math.Tanh(d(1))
        '                                         Dim p3 = d(0)
        '                                         Dim p4 = d(0) * d(1)
        '                                         'p0 + p1*tanh(x) + p2*tanh(y) + p3*x + p4*x*y

        '                                         Return p0 + p1 + p2 + p3 + p4

        '                                     End Function


    End Function





    Public Function FindAllExhaustCoefficients(ByVal DT_BASE_VVE As DataTable,
                                               ByVal DT_EXHAUST_VVE As DataTable,
                                               ByVal ROW_HEADERS() As String,
                                               ByVal RPM_Boundaries As DataTable,
                                               ByVal MAP_Boundaries As DataTable,
                                               ByVal DUPLICATE_RESULT_ROWS As Boolean,
                                               ByRef DO_NOT_WARN_AGAIN As Boolean,
                                               Optional ByVal CAM_ANGLE As Integer = 20) As DataTable()


        If DT_BASE_VVE Is Nothing Then
            Return Nothing
        End If
        If DT_BASE_VVE.Rows.Count = 0 Then
            Return Nothing
        End If
        If DT_BASE_VVE.Columns.Count = 0 Then
            Return Nothing
        End If
        If DT_EXHAUST_VVE Is Nothing Then
            Return Nothing
        End If
        If DT_EXHAUST_VVE.Rows.Count = 0 Then
            Return Nothing
        End If
        If DT_EXHAUST_VVE.Columns.Count = 0 Then
            Return Nothing
        End If
        If DT_EXHAUST_VVE.Rows.Count <> DT_BASE_VVE.Rows.Count Then
            Return Nothing
        End If
        If DT_EXHAUST_VVE.Columns.Count <> DT_BASE_VVE.Columns.Count Then
            Return Nothing
        End If
        If ROW_HEADERS Is Nothing Then
            Return Nothing
        End If
        If ROW_HEADERS.Length = 0 Then
            Return Nothing
        End If
        If DT_BASE_VVE.Rows.Count <> ROW_HEADERS.Length Then
            Return Nothing
        End If
        If RPM_Boundaries Is Nothing Then
            Return Nothing
        End If
        If RPM_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If RPM_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries Is Nothing Then
            Return Nothing
        End If
        If MAP_Boundaries.Rows.Count = 0 Then
            Return Nothing
        End If
        If MAP_Boundaries.Columns.Count = 0 Then
            Return Nothing
        End If

        ' subtract the base VVE airmass from the CAM Airmass
        For i As Integer = 0 To DT_EXHAUST_VVE.Rows.Count - 1
            For j As Integer = 0 To DT_EXHAUST_VVE.Columns.Count - 1
                If DT_EXHAUST_VVE.Columns(j).ColumnName <> DT_BASE_VVE.Columns(j).ColumnName Then
                    Return Nothing
                End If

                DT_EXHAUST_VVE.Rows(i).Item(j) = DT_EXHAUST_VVE.Rows(i).Item(j) - DT_BASE_VVE.Rows(i).Item(j)
            Next
        Next


        Dim DT_MAP As New DataTable()
        Dim DT_RPM As New DataTable()
        Dim DT_INEX As New DataTable()
        Dim DT_CAM As New DataTable()
        Dim DT_CAM2 As New DataTable()

        ' create a column for each zone (0-29)
        For i As Integer = 0 To 29
            DT_MAP.Columns.Add(i.ToString, GetType(Double))
        Next
        ' replicate the columns to all the tables
        DT_RPM = DT_MAP.Clone
        DT_INEX = DT_MAP.Clone
        DT_CAM = DT_MAP.Clone
        DT_CAM2 = DT_MAP.Clone


        Dim ROW_MAP As DataRow = DT_MAP.NewRow()
        Dim ROW_RPM As DataRow = DT_RPM.NewRow()
        Dim ROW_INEX As DataRow = DT_INEX.NewRow()
        Dim ROW_CAM As DataRow = DT_CAM.NewRow()
        Dim ROW_CAM2 As DataRow = DT_CAM2.NewRow()


        Dim RPM() As Double
        Dim MAP() As Double
        Dim VVE() As Double
        Dim COEFF() As Double

        Dim objZones As New clsZones(RPM_Boundaries, MAP_Boundaries, ROW_HEADERS)
        Dim CURRENT_RPM As Double = 0.0
        Dim CURRENT_MAP As Double = 0.0
        Dim CURRENT_VVE As Double = 0.0
        Dim ZONE_LOOKUP As Integer = -1


        For CURRENT_ZONE = 0 To 29


            ReDim RPM(-1)
            ReDim MAP(-1)
            ReDim VVE(-1)
            ReDim COEFF(-1)




            For COL As Integer = 0 To DT_EXHAUST_VVE.Columns.Count - 1
                For ROW As Integer = 0 To DT_EXHAUST_VVE.Rows.Count - 1

                    CURRENT_RPM = CDbl(DT_EXHAUST_VVE.Columns(COL).ColumnName)
                    CURRENT_MAP = CDbl(ROW_HEADERS(ROW))
                    CURRENT_VVE = CDbl(DT_EXHAUST_VVE.Rows(ROW).Item(COL))
                    ZONE_LOOKUP = objZones.WhatZoneAmI(CURRENT_RPM, CURRENT_MAP)

                    If ZONE_LOOKUP = CURRENT_ZONE Then

                        If Array.IndexOf(RPM, CURRENT_RPM) = -1 Then
                            ReDim Preserve RPM(RPM.Length)
                            RPM(RPM.Length - 1) = CURRENT_RPM
                        End If

                        If Array.IndexOf(MAP, CURRENT_MAP) = -1 Then
                            ReDim Preserve MAP(MAP.Length)
                            MAP(MAP.Length - 1) = CURRENT_MAP
                        End If

                        ReDim Preserve VVE(VVE.Length)
                        VVE(VVE.Length - 1) = CURRENT_VVE

                    ElseIf ZONE_LOOKUP > CURRENT_ZONE Then
                        ' break condition
                        ' this will save a little time here on the inner loop,
                        ' but we will still have to keeping looping on the outer loop
                        Continue For
                    End If
                Next

            Next

            ' calc the coefficients for this zone
            COEFF = FindExhaustCoefficients(RPM, MAP, VVE, CAM_ANGLE)

            ' save the results
            ROW_MAP(CURRENT_ZONE) = COEFF(0)
            ROW_RPM(CURRENT_ZONE) = COEFF(1)
            ROW_INEX(CURRENT_ZONE) = COEFF(2)
            ROW_CAM(CURRENT_ZONE) = COEFF(3)
            ROW_CAM2(CURRENT_ZONE) = COEFF(4)

        Next

        ' add the results to the final datatables
        DT_MAP.Rows.Add(ROW_MAP)
        DT_RPM.Rows.Add(ROW_RPM)
        DT_INEX.Rows.Add(ROW_INEX)
        DT_CAM.Rows.Add(ROW_CAM)
        DT_CAM2.Rows.Add(ROW_CAM2)



        ' duplicate rows if desired (this will help user to establish the initial starting values before the tuning process)
        If DUPLICATE_RESULT_ROWS Then
            DT_MAP.ImportRow(ROW_MAP)
            DT_RPM.ImportRow(ROW_RPM)
            DT_INEX.ImportRow(ROW_INEX)
            DT_CAM.ImportRow(ROW_CAM)
            DT_CAM2.ImportRow(ROW_CAM2)
        End If




        DO_NOT_WARN_AGAIN = SMALL_ZONE_WARN
        Return {DT_MAP, DT_RPM, DT_INEX, DT_CAM, DT_CAM2}

    End Function

    Public Function FindExhaustCoefficients(ByVal RPM As Double(), ByVal MAP As Double(), VVE As Double(), ByVal CAM_ANGLE As Integer) As Double()
        'IN:
        ' RPM Array for a single zone (this is the X-axis)
        ' MAP Array for a single zone (this is the Y-axis)
        ' VVE Array for a single zone (this is the Z-axis)
        '
        ' Transform:
        ' A Jagged Array (array of arrays) will be created from the XY values. These will be the coordinate pairs...
        '   {x1, y1}
        '   {x2, y2}
        '   {x3, y3}
        '   ...
        '   {x100, y100}
        '
        'The VVE (or Z-axis) then will need to be in the same order, but it will just be a standard array...
        '   {z1}
        '   {z2}
        '   {z3}
        '   ...
        '   {z100}
        '
        '
        'OUT:
        ' An array of 6 coefficients will be returned

        SmallZoneWarn(RPM.Length, MAP.Length)
        'If SMALL_ZONE_WARN Then
        '    Dim MSG As String = "One of your RPM/MAP zones is too narrow. It needs to be at least 3 columns/rows wide (the wider the better). " &
        '                        "Do you want to ignore this and try to Calculate Coefficients anyway? This can produce very unstable results."
        '    If RPM.Length < 3 Or MAP.Length < 3 Then
        '        If MsgBox(MSG, MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Send It?") = MsgBoxResult.Yes Then
        '            SMALL_ZONE_WARN = False
        '        Else
        '            Throw New Exception("User Canceled.")
        '        End If
        '    End If
        'End If

        If VVE.Length <> RPM.Length * MAP.Length Then
            Return {}
        End If

        ' define array of arrays, lets anticipate the total size so we don't have to [redim] it and have a performance hit
        Dim xy = New Double((RPM.Length * MAP.Length) - 1)() {}
        Dim x As Integer = 0
        For i As Integer = 0 To RPM.Length - 1
            For j As Integer = 0 To MAP.Length - 1
                xy(x) = {RPM(i), MAP(j)}
                x += 1
            Next
        Next

        ' This is defining the model function
        ' you might be able to use the syntax below for: Dim f As Func(Of Double(), Double) = Function(d)
        ' but for now, chaining the function call with the model function definition seems to work...
        ' look at this post for clues: https://stackoverflow.com/questions/22673404/multiple-linear-regression-math-net-2-6-with-fit-linearmultidim



        ' d(0) = x = RPM
        ' d(1) = y = MAP
        ' Exhaust Cam Airmass = (MAP.ExhaustCam Coeff * MAP * Cam Degrees)
        '                     + (RPM.ExhaustCam Coeff * RPM * Cam Degrees)
        '                     + (ExhaustCam.IntakeCam Coeff * Cam Degrees)  <- this last one is a GUESS! Need to test
        '                     + (ExhaustCam Coeff * Cam Degrees)
        '                     + (ExhaustCam2 Coeff * Cam Degrees * Cam Degrees)



        ' Return the 5 coefficients
        Return Fit.LinearMultiDim(xy,
                                  VVE,
                                  Function(d) d(1) * CAM_ANGLE,   ' MAP.ExhaustCam Coeff * MAP * Cam Degrees
                                  Function(d) d(0) * CAM_ANGLE,   ' RPM.ExhaustCam Coeff * RPM * Cam Degrees
                                  Function(d) CAM_ANGLE,          ' ExhaustCam.IntakeCam Coeff * Cam Degrees
                                  Function(d) CAM_ANGLE,          ' ExhaustCam Coeff * Cam Degrees
                                  Function(d) CAM_ANGLE ^ 2)      ' ExhaustCam2 Coeff * Cam Degrees * Cam Degrees



        'Dim f As Func(Of Double(), Double) = Function(d)
        '                                         'd(0) = x
        '                                         'd(1) = y
        '                                         Dim p0 = d(0)
        '                                         Dim p1 = Math.Tanh(d(0))
        '                                         Dim p2 = Math.Tanh(d(1))
        '                                         Dim p3 = d(0)
        '                                         Dim p4 = d(0) * d(1)
        '                                         'p0 + p1*tanh(x) + p2*tanh(y) + p3*x + p4*x*y

        '                                         Return p0 + p1 + p2 + p3 + p4

        '                                     End Function


    End Function

    Public Function CurveFit2D() As String

        Dim x() As Double = {1, 3, 7, 9, 15}
        Dim y() As Double = {1, 3, 7, 9, 15}

        Dim o As Object = Fit.Line(x, x)



        Return ""
    End Function











    'Public Function FindAllVVECoefficients_Sparse(ByVal DT_VVE As DataTable, ByVal ROW_HEADERS() As String,
    '                                              ByVal RPM_Boundaries As DataTable, ByVal MAP_Boundaries As DataTable,
    '                                              ByVal DUPLICATE_RESULT_ROWS As Boolean) As DataTable()


    '    If DT_VVE Is Nothing Then
    '        Return Nothing
    '    End If
    '    If DT_VVE.Rows.Count = 0 Then
    '        Return Nothing
    '    End If
    '    If DT_VVE.Columns.Count = 0 Then
    '        Return Nothing
    '    End If
    '    If ROW_HEADERS Is Nothing Then
    '        Return Nothing
    '    End If
    '    If ROW_HEADERS.Length = 0 Then
    '        Return Nothing
    '    End If
    '    If DT_VVE.Rows.Count <> ROW_HEADERS.Length Then
    '        Return Nothing
    '    End If
    '    If RPM_Boundaries Is Nothing Then
    '        Return Nothing
    '    End If
    '    If RPM_Boundaries.Rows.Count = 0 Then
    '        Return Nothing
    '    End If
    '    If RPM_Boundaries.Columns.Count = 0 Then
    '        Return Nothing
    '    End If
    '    If MAP_Boundaries Is Nothing Then
    '        Return Nothing
    '    End If
    '    If MAP_Boundaries.Rows.Count = 0 Then
    '        Return Nothing
    '    End If
    '    If MAP_Boundaries.Columns.Count = 0 Then
    '        Return Nothing
    '    End If

    '    Dim DT_CONST As New DataTable()
    '    Dim DT_MAP As New DataTable()
    '    Dim DT_MAP2 As New DataTable()
    '    Dim DT_RPM As New DataTable()
    '    Dim DT_RPM2 As New DataTable()
    '    Dim DT_MAPRPM As New DataTable()

    '    ' create a column for each zone (0-29)
    '    For i As Integer = 0 To 29
    '        DT_CONST.Columns.Add(i.ToString, GetType(Double))
    '    Next
    '    ' replicate the columns to all the tables
    '    DT_MAP = DT_CONST.Clone
    '    DT_MAP2 = DT_CONST.Clone
    '    DT_RPM = DT_CONST.Clone
    '    DT_RPM2 = DT_CONST.Clone
    '    DT_MAPRPM = DT_CONST.Clone

    '    Dim ROW_CONST As DataRow = DT_CONST.NewRow()
    '    Dim ROW_MAP As DataRow = DT_MAP.NewRow()
    '    Dim ROW_MAP2 As DataRow = DT_MAP2.NewRow()
    '    Dim ROW_RPM As DataRow = DT_RPM.NewRow()
    '    Dim ROW_RPM2 As DataRow = DT_RPM2.NewRow()
    '    Dim ROW_MAPRPM As DataRow = DT_MAPRPM.NewRow()

    '    Dim RPM() As Double
    '    Dim MAP() As Double
    '    Dim VVE() As Double
    '    Dim COEFF() As Double

    '    Dim objZones As New clsZones(RPM_Boundaries, MAP_Boundaries, ROW_HEADERS)
    '    Dim CURRENT_RPM As Double = 0.0
    '    Dim CURRENT_MAP As Double = 0.0
    '    Dim CURRENT_VVE As Double = 0.0
    '    Dim ZONE_LOOKUP As Integer = -1


    '    For CURRENT_ZONE = 0 To 29


    '        ReDim RPM(-1)
    '        ReDim MAP(-1)
    '        ReDim VVE(-1)
    '        ReDim COEFF(-1)




    '        For COL As Integer = 0 To DT_VVE.Columns.Count - 1
    '            For ROW As Integer = 0 To DT_VVE.Rows.Count - 1

    '                If Not IsNumeric(DT_VVE.Rows(ROW).Item(COL)) Then
    '                    ' we are passing in blanks (a sparse dataset)
    '                    ' only add x,y,z data IF there is a real number
    '                    Continue For
    '                End If

    '                CURRENT_RPM = CDbl(DT_VVE.Columns(COL).ColumnName)
    '                CURRENT_MAP = CDbl(ROW_HEADERS(ROW))
    '                CURRENT_VVE = CDbl(DT_VVE.Rows(ROW).Item(COL))
    '                ZONE_LOOKUP = objZones.WhatZoneAmI(CURRENT_RPM, CURRENT_MAP)

    '                If ZONE_LOOKUP = CURRENT_ZONE Then

    '                    If Array.IndexOf(RPM, CURRENT_RPM) = -1 Then
    '                        ReDim Preserve RPM(RPM.Length)
    '                        RPM(RPM.Length - 1) = CURRENT_RPM
    '                    End If

    '                    If Array.IndexOf(MAP, CURRENT_MAP) = -1 Then
    '                        ReDim Preserve MAP(MAP.Length)
    '                        MAP(MAP.Length - 1) = CURRENT_MAP
    '                    End If

    '                    ReDim Preserve VVE(VVE.Length)
    '                    VVE(VVE.Length - 1) = CURRENT_VVE

    '                ElseIf ZONE_LOOKUP > CURRENT_ZONE Then
    '                    ' break condition
    '                    ' this will save a little time here on the inner loop,
    '                    ' but we will still have to keeping looping on the outer loop
    '                    Continue For
    '                End If
    '            Next

    '        Next

    '        ' calc the coefficients for this zone
    '        COEFF = FindVVECoefficients(RPM, MAP, VVE)

    '        ' save the results
    '        If COEFF.Length = 5 Then
    '            ROW_CONST(CURRENT_ZONE) = COEFF(0)
    '            ROW_MAP(CURRENT_ZONE) = COEFF(1)
    '            ROW_MAP2(CURRENT_ZONE) = COEFF(2)
    '            ROW_RPM(CURRENT_ZONE) = COEFF(3)
    '            ROW_RPM2(CURRENT_ZONE) = COEFF(4)
    '            ROW_MAPRPM(CURRENT_ZONE) = COEFF(5)
    '        End If

    '    Next

    '    ' add the results to the final datatables
    '    DT_CONST.Rows.Add(ROW_CONST)
    '    DT_MAP.Rows.Add(ROW_MAP)
    '    DT_MAP2.Rows.Add(ROW_MAP2)
    '    DT_RPM.Rows.Add(ROW_RPM)
    '    DT_RPM2.Rows.Add(ROW_RPM2)
    '    DT_MAPRPM.Rows.Add(ROW_MAPRPM)


    '    ' duplicate rows if desired (this will help user to establish the initial starting values before the tuning process)
    '    If DUPLICATE_RESULT_ROWS Then
    '        DT_CONST.ImportRow(ROW_CONST)
    '        DT_MAP.ImportRow(ROW_MAP)
    '        DT_MAP2.ImportRow(ROW_MAP2)
    '        DT_RPM.ImportRow(ROW_RPM)
    '        DT_RPM2.ImportRow(ROW_RPM2)
    '        DT_MAPRPM.ImportRow(ROW_MAPRPM)
    '    End If





    '    Return {DT_CONST, DT_MAP, DT_MAP2, DT_RPM, DT_RPM2, DT_MAPRPM}

    'End Function




    Public Function InterpolateSelectedCells(ByVal GridSelectedCells As DataGridViewSelectedCellCollection, ByRef DT As DataTable, ByVal DIRECTION As INTERPOLATE_DIRECTION) As DataTable

        ' -------------------------------------------------------------------------
        ' ITERATE THROUGH THE SELECTED CELLS AND GET THE UNIQUE ROWS AND COLUMNS
        ' -------------------------------------------------------------------------
        Dim COLS(-1) As Integer
        Dim ROWS(-1) As Integer
        For Each cell As DataGridViewCell In GridSelectedCells
            If Array.IndexOf(COLS, cell.OwningColumn.Index) = -1 Then
                ReDim Preserve COLS(COLS.Length)
                COLS(COLS.Length - 1) = cell.OwningColumn.Index
            End If
            If Array.IndexOf(ROWS, cell.OwningRow.Index) = -1 Then
                ReDim Preserve ROWS(ROWS.Length)
                ROWS(ROWS.Length - 1) = cell.OwningRow.Index
            End If
        Next
        Array.Sort(COLS)
        Array.Sort(ROWS)
        ' -------------------------------------------------------------------------


        ' -------------------------------------------------------------------------
        ' INTERPOLATE AND APPLY RESULTS TO SUPPLIED DATATABLE
        ' -------------------------------------------------------------------------
        Dim P0 As Object = 0.0
        Dim P1 As Object = 0.0
        Dim NUM_OF_PTS As Integer = 0
        Dim RESULT(-1) As Double


        If DIRECTION = INTERPOLATE_DIRECTION.Vertical Then
            For i As Integer = 0 To COLS.Length - 1
                P0 = DT.Rows(ROWS(0)).Item(COLS(i))
                P1 = DT.Rows(ROWS(ROWS.Length - 1)).Item(COLS(i))

                If Not IsNumeric(P0) And Not IsNumeric(P1) Then
                    Continue For
                End If

                RESULT = Interpolate(P0, P1, ROWS.Length)

                For j As Integer = 0 To ROWS.Length - 1
                    DT.Rows(ROWS(j)).Item(COLS(i)) = RESULT(j)
                Next
            Next
        End If


        If DIRECTION = INTERPOLATE_DIRECTION.Horizontal Then
            For i As Integer = 0 To ROWS.Length - 1
                P0 = DT.Rows(ROWS(i)).Item(COLS(0))
                P1 = DT.Rows(ROWS(i)).Item(COLS(COLS.Length - 1))

                If Not IsNumeric(P0) And Not IsNumeric(P1) Then
                    Continue For
                End If

                RESULT = Interpolate(P0, P1, COLS.Length)

                For j As Integer = 0 To COLS.Length - 1
                    DT.Rows(ROWS(i)).Item(COLS(j)) = RESULT(j)
                Next
            Next
        End If
        ' -------------------------------------------------------------------------

        Return DT

    End Function

    Public Function Interpolate(ByVal P0 As Double, ByVal P1 As Double, ByVal NUM_OF_POINTS As Double) As Double()
        ' NUM_OF_POINTS is inclusive of the starting point (P0) and ending point (P1)
        '   RETURN is also inclusive of the starting point (P0) and ending point (P1)

        Dim INTERVAL As Double = (P1 - P0) / (NUM_OF_POINTS - 1)
        Dim ALL_PTS(NUM_OF_POINTS - 1) As Double
        For i As Integer = 0 To NUM_OF_POINTS - 1
            ALL_PTS(i) = P0 + (INTERVAL * i)
        Next
        Return ALL_PTS
    End Function


    Public Function SmoothSelectedCells(ByVal GridSelectedCells As DataGridViewSelectedCellCollection, ByRef DT As DataTable) As DataTable

        ''''' NEED TO USE THE ALGORIRTHM BELOW...


        ' -------------------------------------------------------------------------
        ' ITERATE THROUGH THE SELECTED CELLS
        ' GET THE UNIQUE ROWS AND COLUMNS
        ' FIND THE MIN, MAX, AND AVERAGE VALUES CONTAINED
        ' -------------------------------------------------------------------------
        'Dim MIN As Double = System.Double.MaxValue
        'Dim MAX As Double = System.Double.MinValue
        Dim CELL_AVE As Double = 0.0


        Dim CELL_VAL As Double = 0.0
        Dim CELL_CNT As Integer = 0

        Dim COLS(-1) As Integer
        Dim ROWS(-1) As Integer

        For Each cell As DataGridViewCell In GridSelectedCells
            If Array.IndexOf(COLS, cell.OwningColumn.Index) = -1 Then
                ReDim Preserve COLS(COLS.Length)
                COLS(COLS.Length - 1) = cell.OwningColumn.Index
            End If
            If Array.IndexOf(ROWS, cell.OwningRow.Index) = -1 Then
                ReDim Preserve ROWS(ROWS.Length)
                ROWS(ROWS.Length - 1) = cell.OwningRow.Index
            End If

            If IsNumeric(cell.Value) Then
                CELL_CNT += 1

                CELL_VAL = CDbl(cell.Value)
                CELL_AVE += CELL_VAL

                'If CELL_VAL < MIN Then
                '    MIN = CELL_VAL
                'End If

                'If CELL_VAL > MAX Then
                '    MAX = CELL_VAL
                'End If
            End If
        Next

        CELL_AVE = CELL_AVE / CELL_CNT



        Array.Sort(COLS)
        Array.Sort(ROWS)
        ' -------------------------------------------------------------------------

        Dim SMOOTHING_FACTOR As Double = 0.2
        Dim DIFF As Double = 0.0



        For i As Integer = 0 To COLS.Length - 1
            For j As Integer = 0 To ROWS.Length - 1


                If IsNumeric(DT.Rows(ROWS(j)).Item(COLS(i))) Then
                    CELL_VAL = CDbl(DT.Rows(ROWS(j)).Item(COLS(i)))

                    If CELL_VAL < CELL_AVE Then
                        DIFF = CELL_AVE - CELL_VAL
                        DIFF = DIFF * SMOOTHING_FACTOR

                        CELL_VAL += DIFF
                        'CELL_VAL += CELL_VAL * SMOOTHING_FACTOR
                    End If

                    If CELL_VAL > CELL_AVE Then
                        DIFF = CELL_VAL - CELL_AVE
                        DIFF = DIFF * SMOOTHING_FACTOR

                        CELL_VAL -= DIFF
                        'CELL_VAL -= CELL_VAL * SMOOTHING_FACTOR
                    End If
                End If

                DT.Rows(ROWS(j)).Item(COLS(i)) = CELL_VAL
            Next
        Next

        Return DT

    End Function







    Public Shared Function Smooth2DTable(ByVal table As Double(,)) As Double(,)
        ' THIS IS SOURCE CODE FROM "UniversalPatcher"
        ' converted from C# to VB.Net for reference, not used...see below...

        Dim rows As Integer = table.GetLength(0)
        Dim cols As Integer = table.GetLength(1)
        Dim smoothedTable As Double(,) = New Double(rows - 1, cols - 1) {}

        For i As Integer = 0 To rows - 1

            For j As Integer = 0 To cols - 1
                Dim sum As Double = 0
                Dim count As Integer = 0

                For di As Integer = -1 To 1

                    For dj As Integer = -1 To 1
                        Dim ni As Integer = i + di
                        Dim nj As Integer = j + dj

                        If ni >= 0 AndAlso ni < rows AndAlso nj >= 0 AndAlso nj < cols Then
                            sum += table(ni, nj)
                            count += 1
                        End If
                    Next
                Next

                smoothedTable(i, j) = sum / count
            Next
        Next

        Return smoothedTable
    End Function


    Public Shared Function Smoother(ByVal DT_Old As System.Data.DataTable) As DataTable
        ' THIS IS SOURCE CODE FROM "UniversalPatcher"
        ' but converted over to using data tables

        If DT_Old Is Nothing Then
            Return Nothing
        End If

        Dim rows As Integer = DT_Old.Rows.Count
        Dim cols As Integer = DT_Old.Columns.Count

        If rows = 0 Or cols = 0 Then
            Return Nothing
        End If

        Dim DT_New As DataTable = DT_Old.Clone


        For i As Integer = 0 To rows - 1

            For j As Integer = 0 To cols - 1
                Dim sum As Double = 0
                Dim count As Integer = 0

                For di As Integer = -1 To 1

                    For dj As Integer = -1 To 1
                        Dim ni As Integer = i + di
                        Dim nj As Integer = j + dj

                        If ni >= 0 AndAlso ni < rows AndAlso nj >= 0 AndAlso nj < cols Then
                            sum += DT_New.Rows(ni).Item(nj)
                            count += 1
                        End If
                    Next
                Next

                DT_New.Rows(i).Item(j) = sum / count
            Next
        Next








        Return DT_New



    End Function


    Private Sub SmallZoneWarn(ByVal RPM_LEN As Integer, ByVal MAP_LEN As Integer)



        If SMALL_ZONE_WARN Then

            If RPM_LEN < 3 Or MAP_LEN < 3 Then

                Dim warning_msg As String = "One of your RPM/MAP zones is too narrow. Ideally it should be at least 3 columns/rows wide (the wider the better). " & vbCrLf &
                                            "   MAP rows: " & MAP_LEN & vbCrLf &
                                            "   RPM cols: " & RPM_LEN & vbCrLf &
                                            "Do you want to ignore this and try to Calculate Coefficients anyway?" & vbCrLf &
                                            "This can produce very unstable results!"


                Dim msgbx As New frmCustomMsgbox("Small Zone Size Warning", warning_msg)
                msgbx.ShowDialog()

                SMALL_ZONE_WARN = Not msgbx.DoNotShowAgain

                If Not msgbx.UserClickedYes Then
                    Throw New Exception("User Canceled.")
                End If



                'If MsgBox(msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Send It?") = MsgBoxResult.Yes Then
                '        SMALL_ZONE_WARN = False
                '    Else
                '        Throw New Exception("User Canceled.")
                '    End If
                'End If
            End If
        End If



    End Sub








    Public Function CreateVVEFromCoeffAndBreakpoints(ByVal COEFF_CONST As DataTable,
                                                     ByVal COEFF_MAP As DataTable,
                                                     ByVal COEFF_MAP2 As DataTable,
                                                     ByVal COEFF_RPM As DataTable,
                                                     ByVal COEFF_RPM2 As DataTable,
                                                     ByVal COEFF_MAPRPM As DataTable,
                                                     ByVal DT_RPM_ZONE_DEF As DataTable,
                                                     ByVal DT_MAP_ZONE_DEF As DataTable,
                                                     ByVal MAP_BREAKPOINTS() As String,
                                                     ByVal RPM_BREAKPOINTS() As String,
                                                     ByVal VVE_RAW_FROM_EDITOR As DataTable,
                                                     ByVal VERLON_MODE As Boolean) As DataTable

        ' since we do not know if the breakpoints from the SCANNER match the EDITOR
        ' we wil use the coefficients and the breakpoints to calc a VVE table with the proper breakpoints

        Dim DT_VVE_CORRECTED_BREAKPOINTS As New DataTable

        For i As Integer = 0 To RPM_BREAKPOINTS.Length - 1
            DT_VVE_CORRECTED_BREAKPOINTS.Columns.Add(RPM_BREAKPOINTS(i), GetType(Decimal))
        Next

        Dim objZones As New clsZones(DT_RPM_ZONE_DEF, DT_MAP_ZONE_DEF, MAP_BREAKPOINTS)
        Dim NEW_ROW As DataRow
        Dim CURRENT_ZONE As Integer = -1
        Dim ConstCoeff As Double = 0.0
        Dim MAPCoeff As Double = 0.0
        Dim MAP2Coeff As Double = 0.0
        Dim RPMCoeff As Double = 0.0
        Dim RPM2Coeff As Double = 0.0
        Dim MAPRPMCoeff As Double = 0.0

        Dim AirMass As Double = 0.0



        For i As Integer = 0 To MAP_BREAKPOINTS.Length - 1
            NEW_ROW = DT_VVE_CORRECTED_BREAKPOINTS.NewRow()

            For j As Integer = 0 To RPM_BREAKPOINTS.Length - 1

                CURRENT_ZONE = objZones.WhatZoneAmI(RPM_BREAKPOINTS(j), MAP_BREAKPOINTS(i))

                ' ROW 0 = VVE OLD
                ' ROW 1 = VVE NEW
                ConstCoeff = CDbl(COEFF_CONST.Rows(0).Item(CURRENT_ZONE))
                MAPCoeff = CDbl(COEFF_MAP.Rows(0).Item(CURRENT_ZONE))
                MAP2Coeff = CDbl(COEFF_MAP2.Rows(0).Item(CURRENT_ZONE))
                RPMCoeff = CDbl(COEFF_RPM.Rows(0).Item(CURRENT_ZONE))
                RPM2Coeff = CDbl(COEFF_RPM2.Rows(0).Item(CURRENT_ZONE))
                MAPRPMCoeff = CDbl(COEFF_MAPRPM.Rows(0).Item(CURRENT_ZONE))



                AirMass = CalcVVEAirmass(ConstCoeff,
                                            MAPCoeff,
                                            MAP2Coeff,
                                            RPMCoeff,
                                            RPM2Coeff,
                                            MAPRPMCoeff,
                                            CDbl(MAP_BREAKPOINTS(i)),
                                            CDbl(RPM_BREAKPOINTS(j)),
                                            VERLON_MODE)



                NEW_ROW(j) = AirMass

            Next
            DT_VVE_CORRECTED_BREAKPOINTS.Rows.Add(NEW_ROW)
        Next

        Return DT_VVE_CORRECTED_BREAKPOINTS
    End Function






    Public Function InterpolateInteriorMAP(ByVal DT As DataTable) As DataTable

        For c As Integer = 0 To DT.Columns.Count - 1

            Dim startVal As Double = 0
            Dim startRow As Integer = -1
            Dim haveStart As Boolean = False

            Dim r As Integer = 0
            While r < DT.Rows.Count

                Dim cur As Double = CDbl(DT.Rows(r)(c))

                If cur <> 0 Then
                    If Not haveStart Then
                        ' first non-zero in this column
                        startVal = cur
                        startRow = r
                        haveStart = True
                    Else
                        ' found end of a gap
                        Dim endVal As Double = cur
                        Dim endRow As Integer = r
                        Dim steps As Integer = endRow - startRow

                        If steps > 1 Then
                            For k As Integer = startRow + 1 To endRow - 1
                                Dim t As Double = (k - startRow) / steps
                                DT.Rows(k)(c) = startVal + (endVal - startVal) * t
                            Next
                        End If

                        startVal = endVal
                        startRow = r
                    End If
                End If

                r += 1
            End While

        Next

        Return DT
    End Function





    Public Function InterpolateOneRPMValue(ByVal DT As DataTable) As DataTable

        For r As Integer = 0 To DT.Rows.Count - 1

            Dim c As Integer = 0
            While c < DT.Columns.Count - 2

                Dim a As Double = CDbl(DT.Rows(r)(c))
                Dim b As Double = CDbl(DT.Rows(r)(c + 1))
                Dim d As Double = CDbl(DT.Rows(r)(c + 2))

                ' pattern: nonzero, zero, nonzero => exactly 1-gap
                If a <> 0 AndAlso b = 0 AndAlso d <> 0 Then
                    DT.Rows(r)(c + 1) = (a + d) / 2.0
                    c += 3 ' skip past interpolated region
                Else
                    c += 1
                End If

            End While

        Next

        Return DT
    End Function



    Public Function GaussianSmoothMAP(ByVal DT As DataTable,
                                    Optional sigma As Double = 1.0,
                                    Optional window As Integer = 3) As DataTable

        ' precompute gaussian weights
        Dim w(window * 2 + 1) As Double
        Dim sumW As Double = 0
        For i As Integer = -window To window
            Dim g As Double = Math.Exp(-(i * i) / (2 * sigma * sigma))
            w(i + window) = g
            sumW += g
        Next

        Dim rows As Integer = DT.Rows.Count
        Dim cols As Integer = dt.Columns.Count

        ' output clone
        Dim out = dt.Copy()

        For c As Integer = 0 To cols - 1
            For r As Integer = 0 To rows - 1

                Dim v As Double = CDbl(dt.Rows(r)(c))
                If v = 0 Then
                    ' leave as is
                    out.Rows(r)(c) = 0
                    Continue For
                End If

                Dim acc As Double = 0
                Dim wsum As Double = 0

                For k As Integer = -window To window
                    Dim rr As Integer = r + k
                    If rr < 0 OrElse rr >= rows Then Continue For

                    Dim vk As Double = CDbl(dt.Rows(rr)(c))
                    If vk = 0 Then Continue For

                    Dim wk As Double = w(k + window)
                    acc += vk * wk
                    wsum += wk
                Next

                If wsum > 0 Then
                    out.Rows(r)(c) = acc / wsum
                Else
                    out.Rows(r)(c) = v
                End If

            Next
        Next

        ' copy back
        For r As Integer = 0 To rows - 1
            For c As Integer = 0 To cols - 1
                dt.Rows(r)(c) = out.Rows(r)(c)
            Next
        Next

        Return DT
    End Function





End Class
