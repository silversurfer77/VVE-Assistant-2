Imports System.IO

Public Class clsZones

    Private READY As Boolean = True
    Private DT_RPM As DataTable = Nothing
    Private DT_MAP As DataTable = Nothing
    Private DT_HISTO As DataTable = Nothing
    Private ARR_HISTO_MAP(-1) As Double

    Public Sub New(ByVal RPM_Boundaries As DataTable, ByVal MAP_Boundaries As DataTable,
                   ByVal ROW_HEADERS() As String)
        If RPM_Boundaries IsNot Nothing Then
            DT_RPM = RPM_Boundaries
        Else
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries IsNot Nothing Then
            DT_MAP = MAP_Boundaries
        Else
            READY = False
            Exit Sub
        End If

        'If HISTO IsNot Nothing Then
        '    DT_HISTO = HISTO
        'Else
        '    READY = False
        '    Exit Sub
        'End If

        If ROW_HEADERS IsNot Nothing Then

            ReDim ARR_HISTO_MAP(ROW_HEADERS.Length - 1)
            For i As Integer = 0 To ROW_HEADERS.Length - 1
                ARR_HISTO_MAP(i) = CDbl(ROW_HEADERS(i))
            Next
        Else
            READY = False
            Exit Sub
        End If


        If RPM_Boundaries.Columns.Count <> 4 Then
            READY = False
            Exit Sub
        End If

        If RPM_Boundaries.Rows.Count = 0 Then
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries.Columns.Count <> 5 Then
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries.Rows.Count <> 5 Then
            READY = False
            Exit Sub
        End If

        'If HISTO.Columns.Count = 0 Then
        '    READY = False
        '    Exit Sub
        'End If

        'If HISTO.Rows.Count = 0 Then
        '    READY = False
        '    Exit Sub
        'End If

        If ROW_HEADERS.Length = 0 Then
            READY = False
            Exit Sub
        End If

    End Sub

    Public ReadOnly Property ReadyToRock As Boolean
        Get
            Return READY
        End Get
    End Property


    Public Sub GetZoneAxisDefinitions_Complete(ByVal ZONE As Integer, ByRef RPM() As Double, ByRef MAP() As Double)

    End Sub


    Public Function WhatZoneAmI(ByVal RPM As Double, ByVal MAP As Double) As Integer

        If Not READY Then
            Return -1
        End If


        Dim RPM_COL As Integer = -1
        Dim RPM_FOUND As Boolean = False

        'easy:   search for low rpm condition
        If RPM < CDbl(DT_RPM.Rows(0).Item(0)) Then
            'zone: 0-5
            RPM_FOUND = True
            RPM_COL = 0
        End If

        'easy:   search for high rpm condition
        If RPM >= CDbl(DT_RPM.Rows(0).Item(DT_RPM.Columns.Count - 1)) Then
            'zone: 24-29
            RPM_FOUND = True
            RPM_COL = DT_RPM.Columns.Count '4
        End If

        'harder: search for mid rpm condition
        'put our RPM od interest in an array with the RPM definitions sort the array to find where it sits
        If Not RPM_FOUND Then
            'zone: 6-23
            Dim RPM_LIST(4) As Double
            For i As Integer = 0 To DT_RPM.Columns.Count - 1

                If RPM = CDbl(DT_RPM.Rows(0).Item(i)) Then
                    'early exit: exact match on col break point
                    RPM_FOUND = True
                    RPM_COL = i + 1
                    Exit For
                Else
                    'RPM Is Not a breakpoint
                    RPM_LIST(i) = CDbl(DT_RPM.Rows(0).Item(i))
                End If
            Next

            If Not RPM_FOUND Then
                'RPM Is Not a breakpoint
                RPM_LIST(4) = RPM
                Array.Sort(RPM_LIST)
                RPM_COL = Array.IndexOf(RPM_LIST, RPM)
            End If
        End If

        Dim MAP_FOUND As Boolean = False
        Dim MAP_ROW As Integer = 0
        'easy:   search for low map condition
        If MAP < DT_MAP.Rows(0).Item(RPM_COL) Then
            MAP_FOUND = True
            MAP_ROW = 0
        End If
        'easy:   search for high map condition
        If MAP >= DT_MAP.Rows(DT_MAP.Rows.Count - 1).Item(RPM_COL) Then
            MAP_FOUND = True
            MAP_ROW = DT_MAP.Rows.Count
        End If


        If Not MAP_FOUND Then
            Dim MAP_LIST(4) As Double

            For i As Integer = 0 To DT_MAP.Rows.Count - 1

                If MAP = CDbl(DT_MAP.Rows(i).Item(RPM_COL)) Then
                    'early exit: exact match on row break point
                    MAP_FOUND = True
                    MAP_ROW = i + 1
                Else
                    'MAP Is Not a breakpoint
                    MAP_LIST(i) = CDbl(DT_MAP.Rows(i).Item(RPM_COL))
                End If
            Next

            If Not MAP_FOUND Then
                'MAP Is Not a breakpoint
                MAP_LIST(4) = MAP
                Array.Sort(MAP_LIST)
                MAP_ROW = Array.IndexOf(MAP_LIST, MAP)
            End If
        End If




        Return (MAP_ROW) + (RPM_COL * 6)
    End Function



    Public Sub GetZoneAxisDefinitions_MinMax(ByVal ZONE As Integer, ByRef RPM_MIN As Double, ByRef RPM_MAX As Double, ByRef MAP_MIN As Double, ByRef MAP_MAX As Double)

        RPM_MIN = 0.0
        RPM_MAX = 0.0
        MAP_MIN = 0.0
        MAP_MAX = 0.0

        Dim MAP_COL As Integer = 0

        ' ----------------------------------------------------------------------------------------------------
        ' RPM ZONES
        ' ----------------------------------------------------------------------------------------------------
        If 0 <= ZONE And ZONE <= 5 Then
            ' RPM 1st col
            MAP_COL = 0
            RPM_MIN = CDbl(DT_HISTO.Columns(0).ColumnName)
            RPM_MAX = CDbl(DT_RPM.Rows(0).Item(0).value) - 1
        End If

        If 6 <= ZONE And ZONE <= 11 Then
            ' RPM 2nd col
            MAP_COL = 1
            RPM_MIN = CDbl(DT_RPM.Rows(0).Item(0).value)
            RPM_MAX = CDbl(DT_RPM.Rows(0).Item(1).value) - 1
        End If

        If 12 <= ZONE And ZONE <= 17 Then
            ' RPM 3rd col
            MAP_COL = 2
            RPM_MIN = CDbl(DT_RPM.Rows(0).Item(1).value)
            RPM_MAX = CDbl(DT_RPM.Rows(0).Item(2).value) - 1
        End If

        If 18 <= ZONE And ZONE <= 23 Then
            ' RPM 4th col
            MAP_COL = 3
            RPM_MIN = CDbl(DT_RPM.Rows(0).Item(2).value)
            RPM_MAX = CDbl(DT_RPM.Rows(0).Item(3).value) - 1
        End If

        If 24 <= ZONE And ZONE <= 29 Then
            ' RPM 5th col
            MAP_COL = 4
            RPM_MIN = CDbl(DT_RPM.Rows(0).Item(3).value)
            RPM_MAX = CDbl(DT_HISTO.Columns(DT_HISTO.Columns.Count - 1).ColumnName)
        End If
        ' ----------------------------------------------------------------------------------------------------



        ' ----------------------------------------------------------------------------------------------------
        ' MAP ZONES
        ' ----------------------------------------------------------------------------------------------------


        ' we know what column we want from the logic in the RPM section above...
        ' but now we need to know what row...pare down 29 zones into rows 0-5 whith this while loop
        While ZONE > 5
            ZONE -= 5
        End While

        If ZONE = 0 Then
            MAP_MIN = 0.00 'need to look at the histo table to get this...should always be zero
        Else
            MAP_MIN = CDbl(DT_MAP.Rows(ZONE - 1).Item(MAP_COL).value)
        End If

        If ZONE = 29 Then
            MAP_MIN = ARR_HISTO_MAP(ARR_HISTO_MAP.Length - 1)
        Else
            MAP_MAX = CDbl(DT_MAP.Rows(ZONE).Item(MAP_COL).value) - 1
        End If
        ' ----------------------------------------------------------------------------------------------------

    End Sub






End Class
