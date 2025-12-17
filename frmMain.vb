
' icons
' https://icons8.com/icons/set/send-to


'Imports System.Windows.Forms.LinkLabel
'Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Runtime.InteropServices.ComTypes
Imports System.Security.AccessControl
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView
Imports Graph3D
Imports Graph3D.Plot3D

'Imports Microsoft.Win32

' To Do
'
' The tune grid needs to highlight cells that have histo hits
' The tune grid needs a zone focus/selector
' Undo changes
' Pick File Save Location
' Live rezone / autorezone
' Dark Theme







Public Class frmMain
    Public Enum enmFORM_STATE
        Loading
        Normal
        Update_VVE_RowHeaders
        Update_Base_Coeff_RowHeaders
        Update_Intake_Coeff_RowHeaders
        Update_Exhaust_Coeff_RowHeaders
        Update_Tune_RowHeaders
        GRID_SELECTION_IN_PROGRESS
        GRID_SELECTION_COMPLETE
    End Enum

    Private Enum enmWhichVVE
        Init
        Current
    End Enum

    Dim FORM_STATE As enmFORM_STATE = enmFORM_STATE.Loading
    Dim RESIZE_CONTROLS_WIDTH(5) As Integer
    Dim RESIZE_CONTROLS_HEIGHT(5) As Integer
    Dim ITERATIONS As Integer = 0

    Dim SMALL_ZONE_WARN As Boolean = True

    Dim DT_WORKING_VVE As clsMulti_Datatable

    Dim ARR_GRD_MIN_AND_MAX_VALUES(-1) As Double

    'Dim ARR_SCATTER_POINTS(,) As Double
    Dim ARR_SCATTER_POINTS As New List(Of Plot3D.Graph3D.cScatter)



    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        AddHandler Graph3D_Tune.PointValueChanged, AddressOf UpdateWorkingVVEFromChart
        AddHandler Graph3D_VVE.PointValueChanged, AddressOf UpdateVVEFromChart
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try


            FORM_STATE = enmFORM_STATE.Loading

            Me.Cursor = Cursors.WaitCursor
            'UpdateTitleBar()

            'Add3DGraphColorOptions()

            'GetUserSettings()
            'SetUpTabAndGridTagRelationship()

            'My.Settings.HistoLow = System.Drawing.Color.Green

            'grdVVE.SelectionMode = DataGridViewSelectionMode.CellSelect

            SetupWidgetRelationships()


            tsPrg.Minimum = 0
            tsPrg.Maximum = 10
            tsPrg.Value = 0


            FORM_STATE = enmFORM_STATE.Normal


        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            FORM_STATE = enmFORM_STATE.Normal
            Me.Cursor = Cursors.Default
        End Try
    End Sub



    Private Sub mnuFileExit_Click(sender As Object, e As EventArgs) Handles mnuFileExit.Click
        Try
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuZoom_Click(sender As Object, e As EventArgs) Handles mnuZoomExtraSmall.Click, mnuZoomSmall.Click, mnuZoomMedium.Click, mnuZoomLarge.Click, mnuZoomExtraLarge.Click
        Try

            Me.SuspendLayout()
            Dim sz As Size = Me.Size

            Dim mnu As ToolStripMenuItem
            For Each mnu In mnuZoom.DropDownItems
                mnu.Checked = False
            Next

            mnu = DirectCast(sender, ToolStripMenuItem)
            mnu.Checked = True

            Dim fnt As New Font(Me.Font.FontFamily, CInt(mnu.Tag), FontStyle.Regular)
            Me.Font = fnt


            Me.Size = sz


            For Each grd As DataGridView In GetAllGrids()
                grd.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
                grd.AutoResizeColumns()
            Next

            Me.ResumeLayout(True)
            Me.Refresh()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub




    Private Sub tabMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabMain.SelectedIndexChanged
        ' I believe the existence of this listener came from VVEA1 where there was a defect that caused
        ' grid row headers to dissapear and you had to keep displaying them. But VVEA2 is also using .NET 4.8
        ' but this no longer seems to be the case


        Try


            If tabMain.SelectedTab.Equals(tabTune) Then
                'GetGridManager(grdTune).ApplyRowHeaders()
                'Coeff_to_BaseVVE(GetGridManager(grdTune))

                ' will be called to refresh the tune tab incase the user manually edited the coeff
                If Not tsAutotune.Checked Then
                    ' we do this otherwise it will immediately autotune or warn you that it was already autotuned, avoid that
                    tsTuneMnuViewVVENew_Click(Nothing, Nothing)
                End If

                ' FORM_STATE = enmFORM_STATE.Normal
            End If






            'If tabMain.SelectedTab.Equals(tabVVE) Then
            '    ' send vve old to vve new was clicked
            '    GetGridManager(grdVVE).ApplyRowHeaders()

            '    FORM_STATE = enmFORM_STATE.Normal


            'ElseIf tabMain.SelectedTab.Equals(tabVVECoeff) Then
            '    GetGridManager(grdVVEConst).ApplyRowHeaders()
            '    GetGridManager(grdVVEMAP).ApplyRowHeaders()
            '    GetGridManager(grdVVEMAP2).ApplyRowHeaders()
            '    GetGridManager(grdVVERPM).ApplyRowHeaders()
            '    GetGridManager(grdVVERPM2).ApplyRowHeaders()
            '    GetGridManager(grdVVEMAPRPM).ApplyRowHeaders()


            '    FORM_STATE = enmFORM_STATE.Normal

            'ElseIf tabMain.SelectedTab.Equals(tabInCoeff) Then
            '    GetGridManager(grdVVEIntakeMAP).ApplyRowHeaders()
            '    GetGridManager(grdVVEIntakeRPM).ApplyRowHeaders()
            '    GetGridManager(grdVVEIntakeCam).ApplyRowHeaders()
            '    GetGridManager(grdVVEIntakeCam2).ApplyRowHeaders()

            '    FORM_STATE = enmFORM_STATE.Normal

            'ElseIf tabMain.SelectedTab.Equals(tabExCoeff) Then
            '    GetGridManager(grdVVEExhaustMAP).ApplyRowHeaders()
            '    GetGridManager(grdVVEExhaustRPM).ApplyRowHeaders()
            '    GetGridManager(grdVVEExhaustExIn).ApplyRowHeaders()
            '    GetGridManager(grdVVEExhaustCam).ApplyRowHeaders()
            '    GetGridManager(grdVVEExhaustCam2).ApplyRowHeaders()

            '    FORM_STATE = enmFORM_STATE.Normal

            'ElseIf tabMain.SelectedTab.Equals(tabTune) Then
            '    GetGridManager(grdTune).ApplyRowHeaders()
            '    'Coeff_to_BaseVVE(GetGridManager(grdTune))

            '    ' will be called to refresh the tune tab incase the user manually edited the coeff
            '    If Not tsAutotune.Checked Then
            '        ' we do this otherwise it will immediately autotune or warn you that it was already autotuned, avoid that
            '        tsTuneMnuViewVVENew_Click(Nothing, Nothing)
            '    End If

            '    FORM_STATE = enmFORM_STATE.Normal
            'End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            FORM_STATE = enmFORM_STATE.Normal
        End Try
    End Sub


    Private Sub grd_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles grdHisto.DataBindingComplete,
                                                                                                             grdZoneMAP.DataBindingComplete,
                                                                                                             grdZoneRPM.DataBindingComplete,
                                                                                                             grdVVE.DataBindingComplete,
                                                                                                             grdVVEConst.DataBindingComplete,
                                                                                                             grdVVEMAP.DataBindingComplete,
                                                                                                             grdVVEMAP2.DataBindingComplete,
                                                                                                             grdVVERPM.DataBindingComplete,
                                                                                                             grdVVERPM2.DataBindingComplete,
                                                                                                             grdVVEMAPRPM.DataBindingComplete,
                                                                                                             grdVVEIntakeMAP.DataBindingComplete,
                                                                                                             grdVVEIntakeRPM.DataBindingComplete,
                                                                                                             grdVVEIntakeCam.DataBindingComplete,
                                                                                                             grdVVEIntakeCam2.DataBindingComplete,
                                                                                                             grdVVEExhaustMAP.DataBindingComplete,
                                                                                                             grdVVEExhaustRPM.DataBindingComplete,
                                                                                                             grdVVEExhaustExIn.DataBindingComplete,
                                                                                                             grdVVEExhaustCam.DataBindingComplete,
                                                                                                             grdVVEExhaustCam2.DataBindingComplete,
                                                                                                             grdTune.DataBindingComplete
        Try



            ' force the row headers to paint
            'tabMain_SelectedIndexChanged(Nothing, Nothing)

            If FORM_STATE = enmFORM_STATE.Loading Then
                Exit Sub
            End If

            Paint_3D_Graphs()


        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub





    Private Sub grd_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles grdHisto.CellPainting,
                                                                                                               grdZoneMAP.CellPainting,
                                                                                                               grdZoneRPM.CellPainting,
                                                                                                               grdVVE.CellPainting,
                                                                                                               grdVVEConst.CellPainting,
                                                                                                               grdVVEMAP.CellPainting,
                                                                                                               grdVVEMAP2.CellPainting,
                                                                                                               grdVVERPM.CellPainting,
                                                                                                               grdVVERPM2.CellPainting,
                                                                                                               grdVVEMAPRPM.CellPainting,
                                                                                                               grdVVEIntakeMAP.CellPainting,
                                                                                                               grdVVEIntakeRPM.CellPainting,
                                                                                                               grdVVEIntakeCam.CellPainting,
                                                                                                               grdVVEIntakeCam2.CellPainting,
                                                                                                               grdVVEExhaustMAP.CellPainting,
                                                                                                               grdVVEExhaustRPM.CellPainting,
                                                                                                               grdVVEExhaustExIn.CellPainting,
                                                                                                               grdVVEExhaustCam.CellPainting,
                                                                                                               grdVVEExhaustCam2.CellPainting,
                                                                                                               grdTune.CellPainting


        Try
            If FORM_STATE = enmFORM_STATE.Loading Then
                Exit Sub
            End If

            Dim GRD As DataGridView = DirectCast(sender, DataGridView)

            ' ----------------------------------------------------------------------------------
            ' DRAW CELL COLORS
            If GRD.Equals(grdHisto) Or (GRD.Equals(grdTune) And tsTuneMnuViewError.Checked) Then
                clsLib.Paint_Histo_Scanner_Style2(GRD, e.RowIndex, e.ColumnIndex)
            Else
                ' old and slow
                'clsLib.Paint_VVE_Editor_Style(GRD, e)

                Dim MIN As Double = 0.0
                Dim MAX As Double = 0.0
                clsLib.FindMaxDataTableValue(GetGridManager(GRD).DataSource, MIN, MAX)

                clsLib.Paint_VVE_Editor_Style(GRD, e, MAX, MIN)
            End If
            ' ----------------------------------------------------------------------------------


            Dim PAINT_ZONES As Boolean = False
            If GRD.Equals(grdHisto) Or GRD.Equals(grdVVE) Or GRD.Equals(grdTune) Then
                PAINT_ZONES = True
            End If
            If Not PAINT_ZONES Then
                Exit Sub
            End If



            If e.RowIndex = -1 Or e.ColumnIndex = -1 Then
                Exit Sub
            End If


            Dim ARR_RPM(-1) As Double
            Dim ARR_MAP(-1) As Double

            If GRD.Equals(grdTune) Then
                ARR_RPM = GetGridManager(grdHisto).ColumnHeaders
                ARR_MAP = GetGridManager(grdHisto).RowHeaders_DBL
            Else
                ARR_RPM = GetGridManager(GRD).ColumnHeaders
                ARR_MAP = GetGridManager(GRD).RowHeaders_DBL
            End If




            'Exit Sub


            Dim gridlinePen As New Pen(System.Drawing.Color.Black)
            gridlinePen.Width = 4


            Dim objZones As New clsZones(GetGridManager(grdZoneRPM).DataSource,
                                         GetGridManager(grdZoneMAP).DataSource)

            Dim MAP As Double = ARR_MAP(e.RowIndex)
            Dim RPM As Double = ARR_RPM(e.ColumnIndex)

            Dim CURRENT_ZONE As Integer = objZones.WhatZoneAmI(RPM, MAP)

            Dim RIGHT_ZONE As Integer = CURRENT_ZONE
            Dim DOWN_ZONE As Integer = RIGHT_ZONE

            Dim h_topLeftPoint = New Point(e.CellBounds.Left, e.CellBounds.Top + 2)
            Dim h_topRightPoint = New Point(e.CellBounds.Right, e.CellBounds.Top + 2)
            Dim h_bottomRightPoint = New Point(e.CellBounds.Right, e.CellBounds.Bottom - 2)
            Dim h_bottomleftPoint = New Point(e.CellBounds.Left, e.CellBounds.Bottom - 2)

            Dim v_topLeftPoint = New Point(e.CellBounds.Left + 2, e.CellBounds.Top)
            Dim v_topRightPoint = New Point(e.CellBounds.Right - 2, e.CellBounds.Top)
            Dim v_bottomRightPoint = New Point(e.CellBounds.Right - 2, e.CellBounds.Bottom)
            Dim v_bottomleftPoint = New Point(e.CellBounds.Left + 2, e.CellBounds.Bottom)

            e.Paint(e.ClipBounds, DataGridViewPaintParts.All And Not DataGridViewPaintParts.Border)















            ' ----------------------------------------------------------------------------------
            ' DRAW BORDER AROUND THE GRID'S EXTERIOR
            If e.RowIndex = 0 Then
                ' always draw TOP border
                e.Graphics.DrawLine(gridlinePen, h_topLeftPoint, h_topRightPoint)
            End If

            If e.ColumnIndex = 0 Then
                ' always draw LEFT border
                e.Graphics.DrawLine(gridlinePen, v_topLeftPoint, v_bottomleftPoint)
            End If

            If e.RowIndex = GRD.Rows.Count - 1 Then
                ' always draw BOTTOM border
                e.Graphics.DrawLine(gridlinePen, h_bottomRightPoint, h_bottomleftPoint)
            End If

            If e.ColumnIndex = GRD.ColumnCount - 1 Then
                ' always draw RIGHT border
                e.Graphics.DrawLine(gridlinePen, v_bottomRightPoint, v_topRightPoint)
            End If
            ' ----------------------------------------------------------------------------------




            'Exit Sub




            ' ----------------------------------------------------------------------------------
            ' DRAW INTERIOR ZONE BOUNDARIES
            'check what zone is directly to the right...
            If 0 < e.ColumnIndex And e.ColumnIndex < GRD.Columns.Count - 1 Then
                RIGHT_ZONE = objZones.WhatZoneAmI(ARR_RPM(e.ColumnIndex + 1),
                                                  ARR_MAP(e.RowIndex))


                If CURRENT_ZONE <> RIGHT_ZONE Then
                    ' draw RIGHT border
                    e.Graphics.DrawLine(gridlinePen, v_topRightPoint, v_bottomRightPoint)
                End If
            End If

            'check what zone is directly below...
            If 0 < e.RowIndex And e.RowIndex < GRD.Rows.Count - 1 Then
                DOWN_ZONE = objZones.WhatZoneAmI(ARR_RPM(e.ColumnIndex),
                                                 ARR_MAP(e.RowIndex + 1))

                If CURRENT_ZONE <> DOWN_ZONE Then
                    ' draw BOTTOM border
                    e.Graphics.DrawLine(gridlinePen, h_bottomleftPoint, h_bottomRightPoint)
                End If
            End If
            ' ----------------------------------------------------------------------------------



            e.Handled = True
        Catch ex As Exception
            'MsgBox(ex.Message)
        Finally

        End Try





























































        'Try


        '    If FORM_STATE = enmFORM_STATE.Loading Then
        '        Exit Sub
        '    End If


        '    Dim GRD As DataGridView = DirectCast(sender, DataGridView)

        '    ' ----------------------------------------------------------------------------------
        '    ' DRAW CELL COLORS
        '    If GRD.Equals(grdHisto) Or (GRD.Equals(grdTune) And tsTuneMnuViewError.Checked) Then
        '        clsLib.Paint_Histo_Scanner_Style(GRD, True)
        '    Else
        '        clsLib.Paint_VVE_Editor_Style(GRD, e)
        '    End If
        '    ' ----------------------------------------------------------------------------------





        '    If GRD.Equals(grdZoneRPM) Or GRD.Equals(grdZoneMAP) Then
        '        Exit Sub
        '    End If

        '    If e.RowIndex = -1 Or e.ColumnIndex = -1 Then
        '        Exit Sub
        '    End If



        '    '' ----------------------------------------------------------------------------------
        '    '' GIVE A VISUAL INDICATOR ON THE NEW VVE TABLE IF WE HAVE HISTO DATA THERE
        '    'If grdHisto.DataSource IsNot Nothing And GRD.Equals(grdTune) Then


        '    '    Dim HISTO_VAL As Object = grdHisto.Item(e.ColumnIndex, e.RowIndex).Value

        '    '    If Not IsNumeric(HISTO_VAL) Then
        '    '        Exit Sub
        '    '    End If



        '    '    'Dim HISTO_PEN As Pen

        '    '    If HISTO_VAL < 0.0 Then
        '    '        'HISTO_PEN = New Pen(System.Drawing.Color.Green)
        '    '        'HISTO_PEN.Width = 2
        '    '        'HISTO_PEN.DashStyle = Drawing2D.DashStyle.Dot

        '    '        GRD.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = Color.FromArgb(213, 213, 255)
        '    '    End If

        '    '    If HISTO_VAL > 0.0 Then
        '    '        'HISTO_PEN = New Pen(System.Drawing.Color.Red)
        '    '        'HISTO_PEN.Width = 2
        '    '        'HISTO_PEN.DashStyle = Drawing2D.DashStyle.Dot

        '    '        GRD.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = Color.FromArgb(255, 155, 155)
        '    '    End If

        '    '    If HISTO_VAL = 0.0 Then
        '    '        'HISTO_PEN = New Pen(System.Drawing.Color.Blue)
        '    '        'HISTO_PEN.Width = 2
        '    '        'HISTO_PEN.DashStyle = Drawing2D.DashStyle.Dot

        '    '        GRD.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = System.Drawing.Color.White
        '    '    End If

        '    '    '' draw TOP border
        '    '    'e.Graphics.DrawLine(HISTO_PEN, h_topLeftPoint, h_topRightPoint)

        '    '    '' draw LEFT border
        '    '    'e.Graphics.DrawLine(HISTO_PEN, v_topLeftPoint, v_bottomleftPoint)

        '    '    '' draw BOTTOM border
        '    '    'e.Graphics.DrawLine(HISTO_PEN, h_bottomRightPoint, h_bottomleftPoint)

        '    '    '' draw RIGHT border
        '    '    'e.Graphics.DrawLine(HISTO_PEN, v_bottomRightPoint, v_topRightPoint)

        '    'End If
        '    ' ----------------------------------------------------------------------------------

























        '    ' for middle zones...see what zone you are in...
        '    Dim objZones As New clsZones(GetGridManager(grdZoneRPM).DataSource,
        '                                 GetGridManager(grdZoneMAP).DataSource,
        '                                 GetGridManager(GRD).RowHeaders_STR)

        '    If Not objZones.ReadyToRock Then
        '        Exit Sub
        '    End If

        '    Dim gridlinePen As New Pen(System.Drawing.Color.Black)
        '    gridlinePen.Width = 4




        '    Dim CURRENT_ZONE As Integer = objZones.WhatZoneAmI(CDbl(GRD.Columns(e.ColumnIndex).HeaderText),
        '                                                       CDbl(GetGridManager(GRD).RowHeaders_STR(e.RowIndex)))

        '    Dim RIGHT_ZONE As Integer = CURRENT_ZONE
        '    Dim DOWN_ZONE As Integer = RIGHT_ZONE

        '    Dim h_topLeftPoint = New Point(e.CellBounds.Left, e.CellBounds.Top + 2)
        '    Dim h_topRightPoint = New Point(e.CellBounds.Right, e.CellBounds.Top + 2)
        '    Dim h_bottomRightPoint = New Point(e.CellBounds.Right, e.CellBounds.Bottom - 2)
        '    Dim h_bottomleftPoint = New Point(e.CellBounds.Left, e.CellBounds.Bottom - 2)

        '    Dim v_topLeftPoint = New Point(e.CellBounds.Left + 2, e.CellBounds.Top)
        '    Dim v_topRightPoint = New Point(e.CellBounds.Right - 2, e.CellBounds.Top)
        '    Dim v_bottomRightPoint = New Point(e.CellBounds.Right - 2, e.CellBounds.Bottom)
        '    Dim v_bottomleftPoint = New Point(e.CellBounds.Left + 2, e.CellBounds.Bottom)

        '    e.Paint(e.ClipBounds, DataGridViewPaintParts.All And Not DataGridViewPaintParts.Border)


        '    ' ----------------------------------------------------------------------------------
        '    ' DRAW BORDER AROUND THE GRID'S EXTERIOR
        '    If e.RowIndex = 0 Then
        '        ' always draw TOP border
        '        e.Graphics.DrawLine(gridlinePen, h_topLeftPoint, h_topRightPoint)
        '    End If

        '    If e.ColumnIndex = 0 Then
        '        ' always draw LEFT border
        '        e.Graphics.DrawLine(gridlinePen, v_topLeftPoint, v_bottomleftPoint)
        '    End If

        '    If e.RowIndex = GRD.Rows.Count - 1 Then
        '        ' always draw BOTTOM border
        '        e.Graphics.DrawLine(gridlinePen, h_bottomRightPoint, h_bottomleftPoint)
        '    End If

        '    If e.ColumnIndex = GRD.ColumnCount - 1 Then
        '        ' always draw RIGHT border
        '        e.Graphics.DrawLine(gridlinePen, v_bottomRightPoint, v_topRightPoint)
        '    End If
        '    ' ----------------------------------------------------------------------------------


        '    ' ----------------------------------------------------------------------------------
        '    ' DRAW INTERIOR ZONE BOUNDARIES
        '    'check what zone is directly to the right...
        '    If 0 < e.ColumnIndex And e.ColumnIndex < GRD.Columns.Count - 1 Then
        '        RIGHT_ZONE = objZones.WhatZoneAmI(CDbl(GRD.Columns(e.ColumnIndex + 1).HeaderText),
        '                                          CDbl(GetGridManager(GRD).RowHeaders_STR(e.RowIndex)))
        '        If CURRENT_ZONE <> RIGHT_ZONE Then
        '            ' draw RIGHT border
        '            e.Graphics.DrawLine(gridlinePen, v_topRightPoint, v_bottomRightPoint)
        '        End If
        '    End If

        '    'check what zone is directly below...
        '    If 0 < e.RowIndex And e.RowIndex < GRD.Rows.Count - 1 Then
        '        DOWN_ZONE = objZones.WhatZoneAmI(CDbl(GRD.Columns(e.ColumnIndex).HeaderText),
        '                                         CDbl(GetGridManager(GRD).RowHeaders_STR(e.RowIndex + 1)))
        '        If CURRENT_ZONE <> DOWN_ZONE Then
        '            ' draw BOTTOM border
        '            e.Graphics.DrawLine(gridlinePen, h_bottomleftPoint, h_bottomRightPoint)
        '        End If
        '    End If
        '    ' ----------------------------------------------------------------------------------













        '    e.Handled = True






        'Catch ex As Exception
        '    'MsgBox(ex.Message)
        'Finally

        'End Try
    End Sub

    Private Sub grd_ColumnAdded(sender As Object, e As DataGridViewColumnEventArgs) Handles grdHisto.ColumnAdded,
                                                                                            grdZoneMAP.ColumnAdded,
                                                                                            grdZoneRPM.ColumnAdded,
                                                                                            grdVVE.ColumnAdded,
                                                                                            grdVVEConst.ColumnAdded,
                                                                                            grdVVEMAP.ColumnAdded,
                                                                                            grdVVEMAP2.ColumnAdded,
                                                                                            grdVVERPM.ColumnAdded,
                                                                                            grdVVERPM2.ColumnAdded,
                                                                                            grdVVEMAPRPM.ColumnAdded,
                                                                                            grdVVEIntakeMAP.ColumnAdded,
                                                                                            grdVVEIntakeRPM.ColumnAdded,
                                                                                            grdVVEIntakeCam.ColumnAdded,
                                                                                            grdVVEIntakeCam2.ColumnAdded,
                                                                                            grdVVEExhaustMAP.ColumnAdded,
                                                                                            grdVVEExhaustRPM.ColumnAdded,
                                                                                            grdVVEExhaustExIn.ColumnAdded,
                                                                                            grdVVEExhaustCam.ColumnAdded,
                                                                                            grdVVEExhaustCam2.ColumnAdded,
                                                                                            grdTune.ColumnAdded
        Try
            GetGridManager(DirectCast(sender, DataGridView)).PreventGridSort()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub grdVVEMAP_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles grdVVEConst.CellEndEdit,
                                                                                                grdVVEMAP.CellEndEdit,
                                                                                                grdVVEMAP2.CellEndEdit,
                                                                                                grdVVERPM.CellEndEdit,
                                                                                                grdVVERPM2.CellEndEdit,
                                                                                                grdVVEMAPRPM.CellEndEdit
        Try
            FORM_STATE = enmFORM_STATE.Update_Tune_RowHeaders
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Private Sub grdZone_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles grdZoneMAP.CellEndEdit, grdZoneRPM.CellEndEdit
        Try
            ' commit the edits to the background datatable before calling the populategrid code
            Dim GRD_MGR As clsGridManager = GetGridManager(DirectCast(sender, DataGridView))
            GRD_MGR.SetDatatable(DirectCast(GRD_MGR.Grid.DataSource, DataTable), GRD_MGR.RowHeaders_STR)

            PopulateGrid(GRD_MGR, False)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Private Sub btnSplitOrientationVVEOld_Click(sender As Object, e As EventArgs) Handles btnSplitOrientationVVE.Click, btnSplitOrientationTune.Click
        Try
            Dim split As SplitContainer = DirectCast(sender, Button).Tag

            If split.Orientation = Orientation.Horizontal Then
                split.Orientation = Orientation.Vertical
                split.SplitterDistance = CInt(Math.Floor(split.Width / 2))
            Else
                split.Orientation = Orientation.Horizontal
                split.SplitterDistance = CInt(Math.Floor(split.Height / 2))
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsBtnPaste_Click(sender As Object, e As EventArgs) Handles tsHistoBtnPaste.Click,
                                                                           tsZoneRPMBtnPaste.Click,
                                                                           tsZoneMAPBtnPaste.Click,
                                                                           tsVVEBtnPaste.Click,
                                                                           tsVVEConstBtnPaste.Click,
                                                                           tsVVEMAPBtnPaste.Click,
                                                                           tsVVEMAP2BtnPaste.Click,
                                                                           tsVVEMAPRPMBtnPaste.Click,
                                                                           tsVVERPMBtnPaste.Click,
                                                                           tsVVERPM2BtnPaste.Click,
                                                                           tsVVEIntakeMAPBtnPaste.Click,
                                                                           tsVVEIntakeRPMBtnPaste.Click,
                                                                           tsVVEIntakeCamBtnPaste.Click,
                                                                           tsVVEIntakeCam2BtnPaste.Click,
                                                                           tsVVEExhaustMAPBtnPaste.Click,
                                                                           tsVVEExhaustRPMBtnPaste.Click,
                                                                           tsVVEExhaustExInBtnPaste.Click,
                                                                           tsVVEExhaustCamBtnPaste.Click,
                                                                           tsVVEExhaustCam2BtnPaste.Click


        Try
            Dim TS As ToolStrip = DirectCast(sender, ToolStripButton).GetCurrentParent()
            PopulateGrid(TS.Tag, True)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Private Sub tsVVEOldBtnSendTo_Click(sender As Object, e As EventArgs)
    '    Try
    '        Dim DT_OLD As DataTable = DirectCast(grdVVE.DataSource, DataTable)
    '        If DT_OLD Is Nothing Then
    '            Exit Sub
    '        End If
    '        If DT_OLD.Columns.Count = 0 Then
    '            Exit Sub
    '        End If
    '        If DT_OLD.Rows.Count = 0 Then
    '            Exit Sub
    '        End If

    '        'FORM_STATE = enmFORM_STATE.FixVVENewRowHeaders

    '        Dim GRD_MGR_OLD As clsGridManager = GetGridManager(grdVVE)
    '        'Dim GRD_MGR_NEW As clsGridManager = GetGridManager(grdVVENew)

    '        'GRD_MGR_NEW.SetDatatable(GRD_MGR_OLD.DataSource, GRD_MGR_OLD.RowHeaders)

    '        Paint_3D_Graphs()

    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical)
    '    End Try
    'End Sub

    Private Sub mnuHelpSupportBuyMeACoffee_Click_1(sender As Object, e As EventArgs) Handles mnuHelpSupportBuyMeACoffee.Click
        Try
            'MsgBox("Paypal: dankunkel@gmail.com", MsgBoxStyle.Information, "Stay Golden Pony Boy")
            Dim objFrm As New frmPayPal
            objFrm.ShowDialog(Me)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuHelpSendMeAPM_Click(sender As Object, e As EventArgs) Handles mnuHelpSendMeAPM.Click
        Try
            Diagnostics.Process.Start("https://forum.hptuners.com/private.php?do=newpm&u=83494")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuHelpHowToYoutubeVideoDemo_Click(sender As Object, e As EventArgs) Handles mnuHelpHowToYoutubeVideoDemo.Click
        Try
            Diagnostics.Process.Start("https://youtu.be/_Eiyy460C_k")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuHelpManipulateVVE_Click(sender As Object, e As EventArgs) Handles mnuHelpManipulateVVE.Click
        Try
            Diagnostics.Process.Start("https://forum.hptuners.com/showthread.php?102595-Helpful-Info-on-Taming-the-VVE-Beast&p=721285")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuHelpManuallyCheckForNewVersion_Click(sender As Object, e As EventArgs) Handles mnuHelpManuallyCheckForNewVersion.Click
        Try
            Diagnostics.Process.Start("https://drive.google.com/open?id=1-Y4fORr6xiAfVSmZkSrXC1tN6y6XbMuR&authuser=dankunkel%40gmail.com&usp=drive_fs")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub mnuHelpAbout_Click(sender As Object, e As EventArgs) Handles mnuHelpAbout.Click
        Try
            'MsgBox("This is a pre-release version held together with duct tape and bailing wire. Good luck." & vbCrLf & vbCrLf &
            '       "EXE Version: " & clsLib.CURRENT_EXE_DATE & vbCrLf &
            '       "DLL Version: " & clsLib.CURRENT_DLL_DATE,
            '       MsgBoxStyle.Information,
            '       "Danger to the Manifold")


            MsgBox("VVE Assistant version 2.0" & vbCrLf & vbCrLf &
                   "EXE Version: " & clsLib.CURRENT_EXE_DATE & vbCrLf &
                   "Graph3D.dll Version: " & clsLib.CURRENT_CHART_DLL_DATE & vbCrLf &
                   "MathNet.Numerics.dll Version: " & clsLib.CURRENT_MATH_DLL_DATE,
                   MsgBoxStyle.Information,
                    "Version 2.0")


        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Private Sub SetupWidgetRelationships()

        btnSplitOrientationVVE.Tag = splitVVE
        btnSplitOrientationTune.Tag = splitTune


        ' -----------------------------------------------------------------------------
        ' HISTO
        ' -----------------------------------------------------------------------------
        Dim HISTO_META As New clsGridManager(grdHisto, 2)
        tsHisto.Tag = HISTO_META
        grdHisto.Tag = tsHisto
        ' -----------------------------------------------------------------------------


        ' -----------------------------------------------------------------------------
        ' ZONES
        Dim ZONE_MAP_META As New clsGridManager(grdZoneRPM, 0)
        tsZoneRPM.Tag = ZONE_MAP_META
        grdZoneRPM.Tag = tsZoneRPM

        Dim ZONE_RPM_META As New clsGridManager(grdZoneMAP, 0)
        tsZoneMAP.Tag = ZONE_RPM_META
        grdZoneMAP.Tag = tsZoneMAP
        ' -----------------------------------------------------------------------------


        ' -----------------------------------------------------------------------------
        ' VVE
        ' -----------------------------------------------------------------------------
        Dim VVE_META As New clsGridManager(grdVVE, 0)
        tsVVE.Tag = VVE_META
        grdVVE.Tag = tsVVE
        ' -----------------------------------------------------------------------------


        ' -----------------------------------------------------------------------------
        ' VVE BASE COEFFICIENTS
        ' -----------------------------------------------------------------------------
        Dim VVE_CONST_META As New clsGridManager(grdVVEConst, 5)
        tsVVEConst.Tag = VVE_CONST_META
        grdVVEConst.Tag = tsVVEConst

        Dim VVE_MAP_META As New clsGridManager(grdVVEMAP, 5)
        tsVVEMAP.Tag = VVE_MAP_META
        grdVVEMAP.Tag = tsVVEMAP

        Dim VVE_MAP2_META As New clsGridManager(grdVVEMAP2, 5)
        tsVVEMAP2.Tag = VVE_MAP2_META
        grdVVEMAP2.Tag = tsVVEMAP2

        Dim VVE_RPM_META As New clsGridManager(grdVVERPM, 5)
        tsVVERPM.Tag = VVE_RPM_META
        grdVVERPM.Tag = tsVVERPM

        Dim VVE_RPM2_META As New clsGridManager(grdVVERPM2, 5)
        tsVVERPM2.Tag = VVE_RPM2_META
        grdVVERPM2.Tag = tsVVERPM2

        Dim VVE_MAPRPM_META As New clsGridManager(grdVVEMAPRPM, 5)
        tsVVEMAPRPM.Tag = VVE_MAPRPM_META
        grdVVEMAPRPM.Tag = tsVVEMAPRPM
        ' -----------------------------------------------------------------------------


        ' -----------------------------------------------------------------------------
        ' VVE INTAKE COEFFICIENTS
        ' -----------------------------------------------------------------------------
        Dim VVE_INTAKE_MAP_META As New clsGridManager(grdVVEIntakeMAP, 5)
        tsVVEIntakeMAP.Tag = VVE_INTAKE_MAP_META
        grdVVEIntakeMAP.Tag = tsVVEIntakeMAP

        Dim VVE_INTAKE_RPM_META As New clsGridManager(grdVVEIntakeRPM, 5)
        tsVVEIntakeRPM.Tag = VVE_INTAKE_RPM_META
        grdVVEIntakeRPM.Tag = tsVVEIntakeRPM

        Dim VVE_INTAKE_CAM_META As New clsGridManager(grdVVEIntakeCam, 5)
        tsVVEIntakeCam.Tag = VVE_INTAKE_CAM_META
        grdVVEIntakeCam.Tag = tsVVEIntakeCam

        Dim VVE_INTAKE_CAM2_META As New clsGridManager(grdVVEIntakeCam2, 5)
        tsVVEIntakeCam2.Tag = VVE_INTAKE_CAM2_META
        grdVVEIntakeCam2.Tag = tsVVEIntakeCam2
        ' -----------------------------------------------------------------------------



        ' -----------------------------------------------------------------------------
        ' VVE EXHAUST COEFFICIENTS
        ' -----------------------------------------------------------------------------
        Dim VVE_EXHAUST_MAP_META As New clsGridManager(grdVVEExhaustMAP, 5)
        tsVVEExhaustMAP.Tag = VVE_EXHAUST_MAP_META
        grdVVEExhaustMAP.Tag = tsVVEExhaustMAP

        Dim VVE_EXHAUST_RPM_META As New clsGridManager(grdVVEExhaustRPM, 5)
        tsVVEExhaustRPM.Tag = VVE_EXHAUST_RPM_META
        grdVVEExhaustRPM.Tag = tsVVEExhaustRPM

        Dim VVE_EXHAUST_EXIN_META As New clsGridManager(grdVVEExhaustExIn, 5)
        tsVVEExhaustExIn.Tag = VVE_EXHAUST_EXIN_META
        grdVVEExhaustExIn.Tag = tsVVEExhaustExIn

        Dim VVE_EXHAUST_CAM_META As New clsGridManager(grdVVEExhaustCam, 5)
        tsVVEExhaustCam.Tag = VVE_EXHAUST_CAM_META
        grdVVEExhaustCam.Tag = tsVVEExhaustCam

        Dim VVE_EXHAUST_CAM2_META As New clsGridManager(grdVVEExhaustCam2, 5)
        tsVVEExhaustCam2.Tag = VVE_EXHAUST_CAM2_META
        grdVVEExhaustCam2.Tag = tsVVEExhaustCam2
        ' -----------------------------------------------------------------------------


        ' -----------------------------------------------------------------------------
        ' TUNE
        ' -----------------------------------------------------------------------------
        Dim TUNE As New clsGridManager(grdTune, 0)
        tsTune.Tag = TUNE
        grdTune.Tag = tsTuneBtnViewVVENew
        ' -----------------------------------------------------------------------------




    End Sub

    Private Function GetActiveGrid() As DataGridView()
        Dim tabRandom As TabPage = tabMain.SelectedTab

        If tabRandom.Equals(tabHisto) Then
            Return {grdHisto}
        ElseIf tabRandom.Equals(tabZone) Then
            Return {grdZoneMAP, grdZoneRPM}
        ElseIf tabRandom.Equals(grdVVE) Then
            Return {grdVVE}
            'ElseIf tabRandom.Equals(grdVVENew) Then
            '    Return {grdVVENew}
            'ElseIf tabRandom.Equals(tabInputVVENew) Then
            '    Return grdVVENew
            'ElseIf tabRandom.Equals(tabVVEDelta) Then
            '    Return grdVVEDelta
            'ElseIf tabRandom.Equals(tabVVECorrection) Then
            '    Return grdVVEPasteSpecial
        End If

        Return {}

    End Function

    Public Function GetAllGrids() As DataGridView()
        Return {grdHisto, grdZoneMAP, grdZoneRPM, grdVVE} ', grdVVENew}
    End Function

    Private Function GetGridManager(ByVal GRD As DataGridView) As clsGridManager

        ' new way works with toolstripbuttons and toolstripsplitbuttons
        If TypeOf GRD.Tag Is ToolStrip Then

            Return DirectCast(DirectCast(GRD.Tag, ToolStrip).Tag, clsGridManager)

        ElseIf TypeOf GRD.Tag Is ToolStripSplitButton Then

            Return DirectCast(GRD.Tag, ToolStripSplitButton).GetCurrentParent.Tag

        ElseIf TypeOf GRD.Tag Is ToolStripMenuItem Then

            Dim mnu As ToolStripDropDownMenu = DirectCast(GRD.Tag, ToolStripMenuItem).GetCurrentParent
            Dim tsi As ToolStripItem = DirectCast(mnu, ToolStripDropDownMenu).OwnerItem
            Return tsi.GetCurrentParent.Tag

        End If
    End Function

    Private Function DTs_HAVE_DATA(ByVal DT() As DataTable) As Boolean
        Dim RESULT As Boolean = IIf(DT.Length - 1 = 0, False, True)

        For i As Integer = 0 To DT.Length - 1
            If DT(i) Is Nothing Then
                RESULT = False
                Exit For
            End If
            If DT(i).Columns.Count = 0 Then
                RESULT = False
                Exit For
            End If
            If DT(i).Rows.Count = 0 Then
                RESULT = False
                Exit For
            End If
        Next

        Return RESULT
    End Function

    Private Sub chk_CheckedChanged(sender As Object, e As EventArgs) Handles chkSmoothing.CheckedChanged,
                                                                             chkSculpt.CheckedChanged,
                                                                             chkUseBrush.CheckedChanged
        'spinBrushRadius.ValueChanged,
        'spinBrushAgressiveness.ValueChanged
        Try
            'If FORM_STATE = enmFORM_STATE.Loading Then
            '    Exit Sub
            'End If


            'spinBrushRadius.Enabled = chkUseBrush.Checked
            'spinBrushAgressiveness.Enabled = chkUseBrush.Checked

            'Paint_3D_Graphs()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Paint_3D_Graphs()
        ' ---------------------------------------------------------------------------------------
        ' REFRESH ALL 3D GRAPHS (incase of color change request or datasource change)
        ' ---------------------------------------------------------------------------------------
        Dim HasData As Boolean = True

        ' -----------------
        ' VVE
        ' -----------------

        'Graph3D_VVE.TotalPoints> 4

        If GetGridManager(grdVVE).DataSource IsNot Nothing Then

            Graph3D_VVE.Raster = Graph3D.Plot3D.Graph3D.eRaster.MainAxis
            Graph3D_VVE.AllowuserEdit = True
            Graph3D_VVE.AxisX_Breakpoints = GetGridManager(grdVVE).ColumnHeaders
            Graph3D_VVE.AxisY_Breakpoints = GetGridManager(grdVVE).RowHeaders_DBL
            Graph3D_VVE.AxisZ_TickInterval = 500
            Graph3D_VVE.DrawAxisLabels = False
            Graph3D_VVE.DrawAxisLines = False
            Graph3D_VVE.SetHighlightedPolygons(GetSelectionFromGrid(grdVVE))

            'Graph3D_VVE.RPMZoneBoundaries = {1000, 2000, 2600, 3600}
            Graph3D_VVE.RPMZoneBoundaries = clsLib.DT_To_1D_Array_DBL(GetGridManager(grdZoneRPM).DataSource)


            'Graph3D_VVE.MAPZoneBoundaryMatrix = {{20, 40, 60, 80, 90}, ' Zone 0-1
            '                                     {15, 25, 35, 55, 80}, ' Zone 1-2
            '                                     {30, 40, 50, 75, 90}, ' Zone 2-3
            '                                     {25, 35, 45, 65, 75}  ' Zone 3-4
            '                                     }
            Graph3D_VVE.MAPZoneBoundaryMatrix = clsLib.DT_To_2D_Array_DBL(GetGridManager(grdZoneMAP).DataSource)


            'If Graph3D_VVE.TotalPoints < 4 Then
            Dim obj3D_Old As New cls3D(Graph3D_VVE)
            obj3D_Old.Plot3D({{0, 0}, {0, 0}})
            'End If


            Dim objDT_Old As DataTable = GetGridManager(grdVVE).DataSource
            If objDT_Old Is Nothing Then
                HasData = False
            ElseIf objDT_Old.Rows.Count = 0 Then
                HasData = False
            End If


            If HasData Then
                Dim VVE_Old(,) As Double
                VVE_Old = clsLib.DT_To_2D_Array(objDT_Old)
                obj3D_Old.Plot3D(VVE_Old)
            End If
        End If
        ' -----------------


        ' -----------------
        ' TUNE
        ' -----------------

        If GetGridManager(grdTune).DataSource IsNot Nothing Then
            HasData = True

            Graph3D_Tune.Raster = Graph3D.Plot3D.Graph3D.eRaster.MainAxis
            Graph3D_Tune.AllowuserEdit = True
            Graph3D_Tune.AxisX_Breakpoints = GetGridManager(grdHisto).ColumnHeaders
            Graph3D_Tune.AxisY_Breakpoints = GetGridManager(grdHisto).RowHeaders_DBL
            Graph3D_Tune.AxisZ_TickInterval = 500
            Graph3D_Tune.DrawAxisLabels = False
            Graph3D_Tune.DrawAxisLines = False
            Graph3D_Tune.RPMZoneBoundaries = Graph3D_VVE.RPMZoneBoundaries
            Graph3D_Tune.MAPZoneBoundaryMatrix = Graph3D_VVE.MAPZoneBoundaryMatrix

            Dim obj3D_New As New cls3D(Graph3D_Tune)
            obj3D_New.Plot3D({{0, 0}, {0, 0}})

            '-------------------------------------------------------------------------------------------------------------------------
            ' might want to rethink this as it sets the datatable, etc, might be a performance bummer
            'Dim objDT_New As DataTable = DisplayVVE() 'DirectCast(grdTune.DataSource, DataTable)
            Dim objDT_New As DataTable = GetGridManager(grdTune).DataSource
            '--------------------------------------------------------------------------------------------------------------------

            If objDT_New Is Nothing Then
                HasData = False
            ElseIf objDT_New.Rows.Count = 0 Then
                HasData = False
            End If
            If HasData Then
                Dim VVE_New(,) As Double
                VVE_New = clsLib.DT_To_2D_Array(objDT_New)
                obj3D_New.Plot3D(VVE_New)
                Graph3D_Tune.SetScatterOverlay(ARR_SCATTER_POINTS.ToArray)
            End If
        End If
        ' ---------------------------------------------------------------------------------------
    End Sub

    Private Function GetSelectionFromGrid(dgv As DataGridView) As List(Of Point)

        Dim result As New List(Of Point)

        If dgv.SelectedCells.Count = 0 Then
            Return result
        End If

        ' Collect unique column and row indices
        Dim selectedX As New HashSet(Of Integer)()
        Dim selectedY As New HashSet(Of Integer)()

        For Each cell As DataGridViewCell In dgv.SelectedCells
            selectedX.Add(cell.ColumnIndex)
            selectedY.Add(cell.RowIndex)
        Next

        ' Convert these into valid polygon coordinates
        ' Polygons exist for X = 0 → (XCount-2), Y = 0 → (YCount-2)
        Dim maxX As Integer = dgv.ColumnCount - 2
        Dim maxY As Integer = dgv.RowCount - 2

        For Each x In selectedX
            For Each y In selectedY

                ' Make sure polygon indices exist
                If x >= 0 AndAlso x <= maxX AndAlso
               y >= 0 AndAlso y <= maxY Then

                    result.Add(New Point(y, x))

                End If
            Next
        Next

        Return result

    End Function




    Private Sub grd_CellStateChanged(sender As Object, e As DataGridViewCellStateChangedEventArgs) Handles grdVVE.CellStateChanged, grdTune.CellStateChanged
        ' this code will fire logic to highlight the 3D grid polygons based on the cells the user has highlighted in the datagrid
        Try

            Dim GRD As DataGridView = CType(sender, DataGridView)

            If GRD Is grdVVE Then
                Graph3D_VVE.SetHighlightedPolygons(GetSelectionFromGrid(GRD))
                Graph3D_VVE.Invalidate()
            End If

            If GRD Is grdTune Then
                Graph3D_Tune.SetHighlightedPolygons(GetSelectionFromGrid(GRD))
                Graph3D_Tune.Invalidate()
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub



    'Private Sub grdVVE_MouseUp(sender As Object, e As MouseEventArgs) Handles grdVVE.MouseUp

    '    Try
    '        FORM_STATE = enmFORM_STATE.GRID_SELECTION_COMPLETE

    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical)
    '    End Try

    'End Sub

    'Private Sub grdVVE_MouseDown(sender As Object, e As MouseEventArgs) Handles grdVVE.MouseDown

    '    Try
    '        FORM_STATE = enmFORM_STATE.GRID_SELECTION_IN_PROGRESS
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical)
    '    End Try

    'End Sub




    Private Sub PopulateGrid(ByVal GRD_MGR As clsGridManager, ByVal FROM_CLIPBOARD As Boolean)

        If GRD_MGR Is Nothing Then
            Exit Sub
        End If

        Dim ROW_HEADERS(-1) As String
        Dim NUMERIC_COL_HEADERS As Boolean = True

        If GRD_MGR.Grid.Equals(grdZoneMAP) Or GRD_MGR.Grid.Equals(grdZoneRPM) Then
            ' allow for non-numeric column header names
            NUMERIC_COL_HEADERS = False

            ' ensure row headers display on coeff
            'FORM_STATE = enmFORM_STATE.Update_Base_Coeff_RowHeaders
        End If

        'Dim DT As DataTable = clsLib.ClipboardToDatatable(ROW_HEADERS, NUMERIC_COL_HEADERS)
        Dim DT As DataTable
        If FROM_CLIPBOARD Then
            DT = clsLib.ClipboardToDatatable(ROW_HEADERS, NUMERIC_COL_HEADERS)
        Else
            ROW_HEADERS = GRD_MGR.RowHeaders_STR
            DT = GRD_MGR.DataSource()
        End If


        If GRD_MGR.Grid.Equals(grdZoneMAP) Then
            Dim MIN As Double = 0.0
            Dim MAX As Double = 0.0
            clsLib.FindMaxDataTableValue(DT, MAX, MIN)

            If MAX < 5.0 Then 'technically should not exceed 3.0
                'Gen5 detected!
                GRD_MGR.DecimalPlaces = 2
            Else
                GRD_MGR.DecimalPlaces = 1
            End If
        End If

        'If FROM_CLIPBOARD Then
        GRD_MGR.SetDatatable(DT, ROW_HEADERS)
        'End If

        ' wrap both of these calls into Try/Catch blocks since they are calling 3rd party code. If this fails, at least allow the cell painting and zones to render
        'If GRD_MGR.Grid.Equals(grdVVE) Or GRD_MGR.Grid.Equals(grdZoneRPM) Or GRD_MGR.Grid.Equals(grdZoneMAP) Then
        '    Try
        '        'grdVVE.SelectionMode = DataGridViewSelectionMode.CellSelect
        '        Paint_3D_Graphs()
        '    Catch ex As Exception
        '        MsgBox("Error rendering 3D chart: " & ex.Message, MsgBoxStyle.Critical)
        '    End Try

        'End If

        If GRD_MGR.Grid.Equals(grdVVE) Or GRD_MGR.Grid.Equals(grdZoneRPM) Or GRD_MGR.Grid.Equals(grdZoneMAP) Then
            Try
                CalcCoeffFromBaseVVE(GetGridManager(grdVVE).DataSource,
                                     GetGridManager(grdVVE).RowHeaders_STR, ' it is critical to pass in the VVE headers (and not use GRD_MGR since it could also be from the grdZone*) from the base VVE table (NOT THE HISTO HEADERS since we cannot garuntee they match at this point)
                                     False)

                GRD_MGR.Grid.Invalidate()
            Catch ex As Exception
                MsgBox("Error calculating coefficients: " & ex.Message, MsgBoxStyle.Critical)
            End Try

        End If

    End Sub

    Private Sub CalcCoeffFromBaseVVE(ByVal DT_VVE As DataTable, ByVal ROW_HEADERS() As String, ByVal NEW_ROW_ONLY As Boolean)
        Dim objMath As New clsMath(SMALL_ZONE_WARN)

        Dim COEFF() As DataTable = objMath.FindAllVVECoefficients(DT_VVE,
                                                                  ROW_HEADERS,
                                                                  GetGridManager(grdZoneRPM).DataSource,
                                                                  GetGridManager(grdZoneMAP).DataSource,
                                                                  True,
                                                                  SMALL_ZONE_WARN,
                                                                  mnuOptionsVerlonModeSDPatch.Checked)

        If COEFF Is Nothing Then
            Exit Sub
        End If

        If COEFF.Length <> 6 Then
            Exit Sub
        End If

        FORM_STATE = enmFORM_STATE.Update_Base_Coeff_RowHeaders

        If NEW_ROW_ONLY Then

            GetGridManager(grdVVEConst).UpdateRow(1, COEFF(0).Rows(0))
            GetGridManager(grdVVEConst).SetDatatable(GetGridManager(grdVVEConst).DataSource, GetGridManager(grdVVEConst).RowHeaders_STR)

            GetGridManager(grdVVEMAP).UpdateRow(1, COEFF(1).Rows(0))
            GetGridManager(grdVVEMAP).SetDatatable(GetGridManager(grdVVEMAP).DataSource, GetGridManager(grdVVEMAP).RowHeaders_STR)

            GetGridManager(grdVVEMAP2).UpdateRow(1, COEFF(2).Rows(0))
            GetGridManager(grdVVEMAP2).SetDatatable(GetGridManager(grdVVEMAP2).DataSource, GetGridManager(grdVVEMAP2).RowHeaders_STR)

            GetGridManager(grdVVERPM).UpdateRow(1, COEFF(3).Rows(0))
            GetGridManager(grdVVERPM).SetDatatable(GetGridManager(grdVVERPM).DataSource, GetGridManager(grdVVERPM).RowHeaders_STR)

            GetGridManager(grdVVERPM2).UpdateRow(1, COEFF(4).Rows(0))
            GetGridManager(grdVVERPM2).SetDatatable(GetGridManager(grdVVERPM2).DataSource, GetGridManager(grdVVERPM2).RowHeaders_STR)

            GetGridManager(grdVVEMAPRPM).UpdateRow(1, COEFF(5).Rows(0))
            GetGridManager(grdVVEMAPRPM).SetDatatable(GetGridManager(grdVVEMAPRPM).DataSource, GetGridManager(grdVVEMAPRPM).RowHeaders_STR)


        Else
            Dim OLD_NEW_ROW_HEADERS() As String = {"Old", "New"}

            GetGridManager(grdVVEConst).SetDatatable(COEFF(0), OLD_NEW_ROW_HEADERS)
            GetGridManager(grdVVEMAP).SetDatatable(COEFF(1), OLD_NEW_ROW_HEADERS)
            GetGridManager(grdVVEMAP2).SetDatatable(COEFF(2), OLD_NEW_ROW_HEADERS)
            GetGridManager(grdVVERPM).SetDatatable(COEFF(3), OLD_NEW_ROW_HEADERS)
            GetGridManager(grdVVERPM2).SetDatatable(COEFF(4), OLD_NEW_ROW_HEADERS)
            GetGridManager(grdVVEMAPRPM).SetDatatable(COEFF(5), OLD_NEW_ROW_HEADERS)
        End If

    End Sub

    Private Sub tsVVEIntakeImport_Click(sender As Object, e As EventArgs) Handles tsVVEIntakeBtnImport.Click
        Try
            Dim objForm As New frmCamImport(Me.Font.Size, frmCamImport.enmWhichCam.Intake)
            objForm.ShowDialog(Me)

            If objForm.User_Canceled Then
                Exit Sub
            End If

            Dim DT_VVE As DataTable = GetGridManager(grdVVE).DataSource
            Dim DT_VVE_INTAKE As DataTable = objForm.CAM_VVE

            Dim objMath As New clsMath(SMALL_ZONE_WARN)
            Dim COEFF() As DataTable = objMath.FindAllIntakeCoefficients(DT_VVE, DT_VVE_INTAKE,
                                                                         GetGridManager(grdVVE).RowHeaders_STR,
                                                                         DirectCast(grdZoneRPM.DataSource, DataTable),
                                                                         DirectCast(grdZoneMAP.DataSource, DataTable),
                                                                         True,
                                                                         SMALL_ZONE_WARN)

            If COEFF Is Nothing Then
                Exit Sub
            End If

            If COEFF.Length <> 4 Then
                Exit Sub
            End If

            Dim ROW_HEADERS() As String = {"Old", "New"}

            FORM_STATE = enmFORM_STATE.Update_Intake_Coeff_RowHeaders

            GetGridManager(grdVVEIntakeMAP).SetDatatable(COEFF(0), ROW_HEADERS)
            GetGridManager(grdVVEIntakeRPM).SetDatatable(COEFF(1), ROW_HEADERS)
            GetGridManager(grdVVEIntakeCam).SetDatatable(COEFF(2), ROW_HEADERS)
            GetGridManager(grdVVEIntakeCam2).SetDatatable(COEFF(3), ROW_HEADERS)



        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsVVEExhaustBtnImport_Click(sender As Object, e As EventArgs) Handles tsVVEExhaustBtnImport.Click
        Try
            Dim objForm As New frmCamImport(Me.Font.Size, frmCamImport.enmWhichCam.Exhaust)
            objForm.ShowDialog(Me)

            If objForm.User_Canceled Then
                Exit Sub
            End If

            Dim DT_VVE As DataTable = GetGridManager(grdVVE).DataSource
            Dim DT_VVE_EXHAUST As DataTable = objForm.CAM_VVE

            Dim objMath As New clsMath(SMALL_ZONE_WARN)
            Dim COEFF() As DataTable = objMath.FindAllExhaustCoefficients(DT_VVE, DT_VVE_EXHAUST,
                                                                          GetGridManager(grdVVE).RowHeaders_STR,
                                                                          DirectCast(grdZoneRPM.DataSource, DataTable),
                                                                          DirectCast(grdZoneMAP.DataSource, DataTable),
                                                                          True,
                                                                          SMALL_ZONE_WARN)

            If COEFF Is Nothing Then
                Exit Sub
            End If

            If COEFF.Length <> 5 Then
                Exit Sub
            End If

            Dim ROW_HEADERS() As String = {"Old", "New"}

            FORM_STATE = enmFORM_STATE.Update_Exhaust_Coeff_RowHeaders

            GetGridManager(grdVVEExhaustMAP).SetDatatable(COEFF(0), ROW_HEADERS)
            GetGridManager(grdVVEExhaustRPM).SetDatatable(COEFF(1), ROW_HEADERS)
            GetGridManager(grdVVEExhaustExIn).SetDatatable(COEFF(2), ROW_HEADERS)
            GetGridManager(grdVVEExhaustCam).SetDatatable(COEFF(3), ROW_HEADERS)
            GetGridManager(grdVVEExhaustCam2).SetDatatable(COEFF(4), ROW_HEADERS)



        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsBtnMnuCopy_ButtonClick(sender As Object, e As EventArgs) Handles tsHistoBtnCopy.ButtonClick,
                                                                                   tsHistoMnuCopy.Click,
                                                                                   tsHistoMnuCopyWithAxis.Click,
                                                                                   tsZoneRPMBtnCopy.ButtonClick,
                                                                                   tsZoneRPMMnuCopy.Click,
                                                                                   tsZoneRPMMnuCopyWithAxis.Click,
                                                                                   tsZoneMAPBtnCopy.ButtonClick,
                                                                                   tsZoneMAPMnuCopy.Click,
                                                                                   tsZoneMAPMnuCopyWithAxis.Click,
                                                                                   tsVVEBtnCopy.ButtonClick,
                                                                                   tsVVEMnuCopy.Click,
                                                                                   tsVVEMnuCopyWithAxis.Click,
                                                                                   tsVVEConstBtnCopy.ButtonClick,
                                                                                   tsVVEConstMnuCopy.Click,
                                                                                   tsVVEConstMnuCopyWithAxis.Click,
                                                                                   tsVVEMAPBtnCopy.ButtonClick,
                                                                                   tsVVEMAPMnuCopy.Click,
                                                                                   tsVVEMAPMnuCopyWithAxis.Click,
                                                                                   tsVVEMAP2BtnCopy.ButtonClick,
                                                                                   tsVVEMAP2MnuCopy.Click,
                                                                                   tsVVEMAP2MnuCopyWithAxis.Click,
                                                                                   tsVVEMAPRPMBtnCopy.ButtonClick,
                                                                                   tsVVEMAPRPMMnuCopy.Click,
                                                                                   tsVVEMAPRPMMnuCopyWithAxis.Click,
                                                                                   tsVVERPMBtnCopy.ButtonClick,
                                                                                   tsVVERPMMnuCopy.Click,
                                                                                   tsVVERPMMnuCopyWithAxis.Click,
                                                                                   tsVVERPM2BtnCopy.ButtonClick,
                                                                                   tsVVERPM2MnuCopy.Click,
                                                                                   tsVVERPM2MnuCopyWithAxis.Click,
                                                                                   tsVVEIntakeMAPBtnCopy.ButtonClick,
                                                                                   tsVVEIntakeMAPMnuCopy.Click,
                                                                                   tsVVEIntakeMAPMnuCopyWithAxis.Click,
                                                                                   tsVVEIntakeRPMBtnCopy.ButtonClick,
                                                                                   tsVVEIntakeRPMMnuCopy.Click,
                                                                                   tsVVEIntakeRPMMnuCopyWithAxis.Click,
                                                                                   tsVVEIntakeCamBtnCopy.ButtonClick,
                                                                                   tsVVEIntakeCamMnuCopy.Click,
                                                                                   tsVVEIntakeCamMnuCopyWithAxis.Click,
                                                                                   tsVVEIntakeCam2BtnCopy.ButtonClick,
                                                                                   tsVVEIntakeCam2MnuCopy.Click,
                                                                                   tsVVEIntakeCam2MnuCopyWithAxis.Click,
                                                                                   tsVVEExhaustMAPBtnCopy.ButtonClick,
                                                                                   tsVVEExhaustMAPMnuCopy.Click,
                                                                                   tsVVEExhaustMAPMnuCopyWithAxis.Click,
                                                                                   tsVVEExhaustRPMBtnCopy.ButtonClick,
                                                                                   tsVVEExhaustRPMMnuCopy.Click,
                                                                                   tsVVEExhaustRPMMnuCopyWithAxis.Click,
                                                                                   tsVVEExhaustExInBtnCopy.ButtonClick,
                                                                                   tsVVEExhaustExInMnuCopy.Click,
                                                                                   tsVVEExhaustExInMnuCopyWithAxis.Click,
                                                                                   tsVVEExhaustCamBtnCopy.ButtonClick,
                                                                                   tsVVEExhaustCamMnuCopy.Click,
                                                                                   tsVVEExhaustCamMnuCopyWithAxis.Click,
                                                                                   tsVVEExhaustCam2BtnCopy.ButtonClick,
                                                                                   tsVVEExhaustCam2MnuCopy.Click,
                                                                                   tsVVEExhaustCam2MnuCopyWithAxis.Click
        'AxisNo
        'AxisYes
        Try

            Dim AXIS_YES_NO As String = ""
            Dim GRD_MGR As clsGridManager

            If TypeOf sender Is ToolStripSplitButton Then

                AXIS_YES_NO = DirectCast(sender, ToolStripSplitButton).Tag
                GRD_MGR = DirectCast(sender, ToolStripSplitButton).GetCurrentParent.Tag

            ElseIf TypeOf sender Is ToolStripMenuItem Then

                Dim mnu As ToolStripDropDownMenu = DirectCast(sender, ToolStripMenuItem).GetCurrentParent
                AXIS_YES_NO = DirectCast(sender, ToolStripMenuItem).Tag
                Dim tsi As ToolStripItem = DirectCast(mnu, ToolStripDropDownMenu).OwnerItem
                GRD_MGR = tsi.GetCurrentParent.Tag
            End If

            If AXIS_YES_NO = "AxisNo" Then
                GRD_MGR.CopyGridToClipboard(clsGridManager.enmCopyOptions.WithOutHeaders)
            ElseIf AXIS_YES_NO = "AxisYes" Then
                GRD_MGR.CopyGridToClipboard(clsGridManager.enmCopyOptions.WithHeaders)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsBtnMnuClear_Click(sender As Object, e As EventArgs) Handles tsHistoBtnClear.Click,
                                                                              tsZoneRPMBtnClear.Click,
                                                                              tsZoneMAPBtnClear.Click,
                                                                              tsVVEBtnClear.Click,
                                                                              tsVVEConstBtnClear.ButtonClick,
                                                                              tsVVEConstMnuClear.Click,
                                                                              tsVVEConstMnuClearRow.Click,
                                                                              tsVVEMAPBtnClear.ButtonClick,
                                                                              tsVVEMAPMnuClear.Click,
                                                                              tsVVEMAPMnuClearRow.Click,
                                                                              tsVVEMAP2BtnClear.ButtonClick,
                                                                              tsVVEMAP2MnuClear.Click,
                                                                              tsVVEMAP2MnuClearRow.Click,
                                                                              tsVVEMAPRPMBtnClear.ButtonClick,
                                                                              tsVVEMAPRPMMnuClear.Click,
                                                                              tsVVEMAPRPMMnuClearRow.Click,
                                                                              tsVVERPMBtnClear.ButtonClick,
                                                                              tsVVERPMMnuClear.Click,
                                                                              tsVVERPMMnuClearRow.Click,
                                                                              tsVVERPM2BtnClear.ButtonClick,
                                                                              tsVVERPM2MnuClear.Click,
                                                                              tsVVERPM2MnuClearRow.Click,
                                                                              tsVVEIntakeMAPBtnClear.ButtonClick,
                                                                              tsVVEIntakeMAPMnuClear.Click,
                                                                              tsVVEIntakeMAPMnuClearRow.Click,
                                                                              tsVVEIntakeRPMBtnClear.ButtonClick,
                                                                              tsVVEIntakeRPMMnuClear.Click,
                                                                              tsVVEIntakeRPMMnuClearRow.Click,
                                                                              tsVVEIntakeCamBtnClear.ButtonClick,
                                                                              tsVVEIntakeCamMnuClear.Click,
                                                                              tsVVEIntakeCamMnuClearRow.Click,
                                                                              tsVVEIntakeCam2BtnClear.ButtonClick,
                                                                              tsVVEIntakeCam2MnuClear.Click,
                                                                              tsVVEIntakeCam2BtnClearRow.Click,
                                                                              tsVVEExhaustMAPBtnClear.ButtonClick,
                                                                              tsVVEExhaustMAPMnuClear.Click,
                                                                              tsVVEExhaustMAPMnuClearRow.Click,
                                                                              tsVVEExhaustRPMBtnClear.ButtonClick,
                                                                              tsVVEExhaustRPMMnuClear.Click,
                                                                              tsVVEExhaustRPMMnuClearRow.Click,
                                                                              tsVVEExhaustExInBtnClear.ButtonClick,
                                                                              tsVVEExhaustExInMnuClear.Click,
                                                                              tsVVEExhaustExInMnuClearRow.Click,
                                                                              tsVVEExhaustCamBtnClear.ButtonClick,
                                                                              tsVVEExhaustCamMnuClear.Click,
                                                                              tsVVEExhaustCamMnuClearRow.Click,
                                                                              tsVVEExhaustCam2BtnClear.ButtonClick,
                                                                              tsVVEExhaustCam2MnuClear.Click,
                                                                              tsVVEExhaustCam2MnuClearRow.Click
        'Table
        'Row
        Try
            Dim TABLE_OR_ROW As String = ""
            Dim GRD_MGR As clsGridManager

            If TypeOf sender Is ToolStripSplitButton Then

                TABLE_OR_ROW = DirectCast(sender, ToolStripSplitButton).Tag
                GRD_MGR = DirectCast(sender, ToolStripSplitButton).GetCurrentParent.Tag

            ElseIf TypeOf sender Is ToolStripMenuItem Then

                Dim mnu As ToolStripDropDownMenu = DirectCast(sender, ToolStripMenuItem).GetCurrentParent
                TABLE_OR_ROW = DirectCast(sender, ToolStripMenuItem).Tag
                Dim tsi As ToolStripItem = DirectCast(mnu, ToolStripDropDownMenu).OwnerItem
                GRD_MGR = tsi.GetCurrentParent.Tag

            End If

            If TABLE_OR_ROW = "Table" Then
                GRD_MGR.SetDatatable(Nothing)
            ElseIf TABLE_OR_ROW = "Row" Then
                'GRD_MGR.DeleteRow(0)
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsTuneMnuViewVVENew_Click(sender As Object, e As EventArgs) Handles tsTuneBtnViewVVENew.ButtonClick,
                                                                                    tsTuneMnuViewError.Click,
                                                                                    tsTuneMnuViewVVENew.Click,
                                                                                    tsTuneMnuViewVVEOld.Click,
                                                                                    tsAutotune.Click

        'Dim COL_SCROLL As Integer = 0
        'Dim ROW_SCROLL As Integer = 0

        Try

            'COL_SCROLL = grdTune.FirstDisplayedScrollingColumnIndex
            'ROW_SCROLL = grdTune.FirstDisplayedScrollingRowIndex

            '----------------------------------------------------------------------------------------------------------
            ' this will execute when the user manually changes the coeff and then switches tabs back to the tune tab,
            ' we want the grid and 3D to auto-refresh
            If sender Is Nothing Then
                For Each mnu As ToolStripMenuItem In tsTuneBtnViewVVENew.DropDownItems
                    If mnu.Checked Then
                        sender = mnu
                        Exit For
                    End If
                Next
            End If
            '----------------------------------------------------------------------------------------------------------


            If sender.Equals(tsTuneMnuViewError) Then
                ' NEED TO POPUALTE NEW PASTE SPECIAL
                tsTuneMnuViewError.Checked = True
                tsTuneMnuViewVVENew.Checked = False
                tsTuneMnuViewVVEOld.Checked = False
                tsAutotune.Checked = False

                DisplayPasteSpecial()

            ElseIf sender.Equals(tsTuneBtnViewVVENew) Or sender.Equals(tsTuneMnuViewVVENew) Then
                tsTuneMnuViewError.Checked = False
                tsTuneMnuViewVVENew.Checked = True
                tsTuneMnuViewVVEOld.Checked = False
                tsAutotune.Checked = False

                DisplayVVE()

            ElseIf sender.Equals(tsTuneMnuViewVVEOld) Then
                tsTuneMnuViewError.Checked = False
                tsTuneMnuViewVVENew.Checked = False
                tsTuneMnuViewVVEOld.Checked = True
                tsAutotune.Checked = False

                DisplayVVE()

            ElseIf sender.Equals(tsAutotune) Then
                tsTuneMnuViewError.Checked = False
                tsTuneMnuViewVVENew.Checked = False
                tsTuneMnuViewVVEOld.Checked = False
                tsAutotune.Checked = True

                If DT_WORKING_VVE IsNot Nothing Then
                    If MsgBox("You have already begun VVE tuning, do you want to start over and lose all your work?" & vbCrLf & vbCrLf &
                                          "Yes = Start Over" & vbCrLf &
                                          "No = Cancel",
                                          MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                        Exit Sub
                    End If
                End If
                Autotune()
            End If

            'Paint_3D_Graphs()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            'grdTune.FirstDisplayedScrollingColumnIndex = COL_SCROLL
            'grdTune.FirstDisplayedScrollingRowIndex = ROW_SCROLL
        End Try
    End Sub






    Private Function DisplayVVE() As DataTable
        'If Not tsTuneMnuViewVVENew.Checked And Not tsTuneMnuViewVVEOld.Checked Then
        '    Exit Sub
        'End If

        'Dim COEFF_ROW As Integer = IIf(tsTuneMnuViewVVEOld.Checked, 0, 1) '0=OLD, 1=NEW
        'Dim DT_FINAL As New DataTable
        'Dim ROW_HEADERS(-1) As String

        '' ----------------------------------
        '' TO DO
        '' include IN and EX values
        '' don't forget about the Gen5 stuff
        '' ----------------------------------
        'CalcVVE(COEFF_ROW, DT_FINAL, ROW_HEADERS)

        'FORM_STATE = enmFORM_STATE.Update_Tune_RowHeaders
        'Dim GRD_MGR As clsGridManager = GetGridManager(grdTune)
        'GRD_MGR.DecimalPlaces = 0
        'GRD_MGR.SetDatatable(DT_FINAL, ROW_HEADERS)



        Dim COEFF_ROW As Integer = IIf(tsTuneMnuViewVVEOld.Checked, 0, 1) '0=OLD, 1=NEW
        Dim DT_FINAL As New DataTable
        Dim ROW_HEADERS(-1) As String

        ' ----------------------------------
        ' TO DO
        ' include IN and EX values
        ' don't forget about the Gen5 stuff
        ' ----------------------------------
        CalcVVE(COEFF_ROW, DT_FINAL, ROW_HEADERS)

        If tsTuneMnuViewVVENew.Checked Or tsTuneMnuViewVVEOld.Checked Then
            'user wants to see this in the tune grid
            FORM_STATE = enmFORM_STATE.Update_Tune_RowHeaders
            Dim GRD_MGR As clsGridManager = GetGridManager(grdTune)
            GRD_MGR.DecimalPlaces = 0
            GRD_MGR.SetDatatable(DT_FINAL, ROW_HEADERS)
        End If

        ' this will be used to paint the final VVE 3D chart
        Return DT_FINAL
    End Function

    Private Sub DisplayPasteSpecial()

        Dim DT_OLD As New DataTable
        Dim DT_NEW As New DataTable
        Dim DT_PASTE_SPECIAL As New DataTable
        Dim ROW_HEADERS() As String = GetGridManager(grdHisto).RowHeaders_STR

        CalcVVE(0, DT_OLD, ROW_HEADERS)
        CalcVVE(1, DT_NEW, ROW_HEADERS)
        CalcPasteSpecial(GetGridManager(grdHisto).DataSource, DT_OLD, DT_NEW, DT_PASTE_SPECIAL, True)

        FORM_STATE = enmFORM_STATE.Update_Tune_RowHeaders

        Dim GRD_MGR As clsGridManager = GetGridManager(grdTune)
        GRD_MGR.DecimalPlaces = 2
        GRD_MGR.SetDatatable(DT_PASTE_SPECIAL, ROW_HEADERS)

    End Sub

    Private Sub CalcVVE(ByVal COEFF_ROW As Integer, ByRef DT_FINAL As DataTable, ByRef ROW_HEADERS() As String)
        ' COEFF_ROW: 0 = OLD
        '            1 = NEW


        Dim DT_HISTO As DataTable = GetGridManager(grdHisto).DataSource
        If DT_HISTO Is Nothing Then
            Exit Sub
        End If
        'Dim DT_FINAL As DataTable = DT_HISTO.Clone
        DT_FINAL = DT_HISTO.Clone
        'Dim ROW_HEADERS() As String = GetGridManager(grdHisto).RowHeaders
        ROW_HEADERS = GetGridManager(grdHisto).RowHeaders_STR
        Dim objZones As New clsZones(GetGridManager(grdZoneRPM).DataSource, GetGridManager(grdZoneMAP).DataSource)
        Dim objMath As New clsMath(SMALL_ZONE_WARN)

        Dim DT_CONSTCoeff As DataTable = GetGridManager(grdVVEConst).DataSource
        Dim DT_MAPCoeff As DataTable = GetGridManager(grdVVEMAP).DataSource
        Dim DT_MAP2Coeff As DataTable = GetGridManager(grdVVEMAP2).DataSource
        Dim DT_RPMCoeff As DataTable = GetGridManager(grdVVERPM).DataSource
        Dim DT_RPM2Coeff As DataTable = GetGridManager(grdVVERPM2).DataSource
        Dim DT_MAPRPMCoeff As DataTable = GetGridManager(grdVVEMAPRPM).DataSource

        If Not DTs_HAVE_DATA({DT_HISTO, DT_CONSTCoeff, DT_MAPCoeff, DT_MAP2Coeff, DT_RPMCoeff, DT_RPM2Coeff, DT_MAPRPMCoeff}) Then
            Exit Sub
        End If

        Dim RPM As Double = 0.0
        Dim MAP As Double = 0.0
        Dim NEW_ROW As DataRow
        Dim ZONE As Integer = 0

        Dim ConstCoeff As Double = 0.0
        Dim MAPCoeff As Double = 0.0
        Dim MAP2Coeff As Double = 0.0
        Dim RPMCoeff As Double = 0.0
        Dim RPM2Coeff As Double = 0.0
        Dim MAPRPMCoeff As Double = 0.0

        For i As Integer = 0 To DT_HISTO.Rows.Count - 1
            MAP = CDbl(ROW_HEADERS(i))
            NEW_ROW = DT_FINAL.NewRow()

            For j As Integer = 0 To DT_HISTO.Columns.Count - 1
                RPM = CDbl(DT_HISTO.Columns(j).ColumnName)

                ZONE = objZones.WhatZoneAmI(RPM, MAP)
                ConstCoeff = CDbl(DT_CONSTCoeff.Rows(COEFF_ROW).Item(ZONE))
                MAPCoeff = CDbl(DT_MAPCoeff.Rows(COEFF_ROW).Item(ZONE))
                MAP2Coeff = CDbl(DT_MAP2Coeff.Rows(COEFF_ROW).Item(ZONE))
                RPMCoeff = CDbl(DT_RPMCoeff.Rows(COEFF_ROW).Item(ZONE))
                RPM2Coeff = CDbl(DT_RPM2Coeff.Rows(COEFF_ROW).Item(ZONE))
                MAPRPMCoeff = CDbl(DT_MAPRPMCoeff.Rows(COEFF_ROW).Item(ZONE))

                NEW_ROW(j) = objMath.CalcVVEAirmass(ConstCoeff,
                                                    MAPCoeff,
                                                    MAP2Coeff,
                                                    RPMCoeff,
                                                    RPM2Coeff,
                                                    MAPRPMCoeff,
                                                    MAP,
                                                    RPM,
                                                    mnuOptionsVerlonModeSDPatch.Checked)
            Next

            DT_FINAL.Rows.Add(NEW_ROW)
        Next



    End Sub

    Private Sub CalcPasteSpecial(ByVal DT_HISTO As DataTable,
                                 ByVal DT_VVE_OLD As DataTable,
                                 ByVal DT_VVE_NEW As DataTable,
                                 ByRef DT_PASTE_SPECIAL As DataTable,
                                 ByVal RESULT_AS_PERCENT As Boolean)

        If Not DTs_HAVE_DATA({DT_HISTO, DT_VVE_OLD, DT_VVE_NEW}) Then
            Exit Sub
        End If


        'Dim DT_PASTE_SPECIAL As DataTable = DT_HISTO.Clone
        DT_PASTE_SPECIAL = DT_HISTO.Clone
        For i As Integer = 0 To DT_PASTE_SPECIAL.Columns.Count - 1
            DT_PASTE_SPECIAL.Columns(i).AllowDBNull = True
        Next

        Dim ROW As DataRow

        Dim EXPECTED_VAL As Double = 0.0
        Dim CURRENT_VAL As Double = 0.0
        Dim CALC_ERR As Double = 0.0


        For i As Integer = 0 To DT_VVE_OLD.Rows.Count - 1
            ' prevent there is no row at position...
            If i > DT_HISTO.Rows.Count - 1 Then
                Exit For
            End If


            ROW = DT_PASTE_SPECIAL.NewRow
            For j As Integer = 0 To DT_VVE_OLD.Columns.Count - 1

                ' prevent there is no column at position...
                If j > DT_HISTO.Columns.Count - 1 Then
                    Exit For
                End If

                EXPECTED_VAL = 0.0
                CURRENT_VAL = 0.0
                CALC_ERR = 0.0

                If IsNumeric(DT_HISTO.Rows(i).Item(j)) Then
                    EXPECTED_VAL = GetExpectedValue(DT_VVE_OLD.Rows(i).Item(j), DT_HISTO, i, j)
                    CURRENT_VAL = DT_VVE_NEW.Rows(i).Item(j)

                    If CURRENT_VAL = 0.0 Then
                        CURRENT_VAL = 0.000001
                    End If

                    If RESULT_AS_PERCENT Then
                        ' allows for paste spectial in HPT for Half or Full Percent
                        CALC_ERR = ((EXPECTED_VAL - CURRENT_VAL) / CURRENT_VAL) * 100.0
                    Else
                        ' returns the literal value
                        CALC_ERR = CURRENT_VAL * (EXPECTED_VAL / CURRENT_VAL)
                    End If

                    ROW(j) = CALC_ERR
                Else
                    ROW(j) = DBNull.Value
                End If
            Next
            DT_PASTE_SPECIAL.Rows.Add(ROW)
        Next



    End Sub


    Public Function Autotune() As DataTable
        ' this is needed to create the VVE target (raw VVE values, not with percents)
        ' there will be nulls and we will interpolate those gaps
        ' we will also raise/lower the edges of the data to match.
        ' the problem is that we cannot count on the histo breakpoints
        ' to match the raw VVE breakpoints from teh scanner, so first
        ' we have to use the coefficients to re-create the VVE table...

        Try
            Me.Cursor = Cursors.WaitCursor

            If Not DTs_HAVE_DATA({GetGridManager(grdHisto).DataSource, GetGridManager(grdVVE).DataSource}) Then
                Return New DataTable
            End If

            'Dim MSG_RES As MsgBoxResult = MsgBoxResult.Yes
            'If DT_WORKING_VVE IsNot Nothing Then
            '    MSG_RES = MsgBox("You have already begun VVE tuning, do you want to start over and lose all your work?" & vbCrLf & vbCrLf &
            '              "Yes = Start Over" & vbCrLf &
            '              "No = Cancel",
            '              MsgBoxStyle.Question Or MsgBoxStyle.YesNo)


            '    If MSG_RES = MsgBoxResult.No Then
            '        Return New DataTable
            '    End If
            'End If


            Dim AUTOTUNE_DIALOG As frmAutotune
            Dim OPEN_AUTOTUNE_DIALOG As Boolean = True
            For Each frm As Form In Application.OpenForms
                If TypeOf frm Is frmAutotune Then
                    OPEN_AUTOTUNE_DIALOG = False

                    AUTOTUNE_DIALOG = frm
                    If AUTOTUNE_DIALOG.FORMLOADING Then
                        Return Nothing
                    End If
                End If
            Next

            If OPEN_AUTOTUNE_DIALOG Then
                AUTOTUNE_DIALOG = New frmAutotune()
                AUTOTUNE_DIALOG.Owner = Me
                AUTOTUNE_DIALOG.Show(Me)
            End If






            ' if starting over, start from a clean slate
            DT_WORKING_VVE = Nothing

            Dim MATH As New clsMath(SMALL_ZONE_WARN)
            Dim DT_TARGET_WITH_INTERPOLATION As DataTable

            'If MSG_RES = MsgBoxResult.Yes Then
            Dim DT_VVE_ADJUSTED_BREAKPOINTS As DataTable = MATH.CreateVVEFromCoeffAndBreakpoints(GetGridManager(grdVVEConst).DataSource,
                                                                                                 GetGridManager(grdVVEMAP).DataSource,
                                                                                                 GetGridManager(grdVVEMAP2).DataSource,
                                                                                                 GetGridManager(grdVVERPM).DataSource,
                                                                                                 GetGridManager(grdVVERPM2).DataSource,
                                                                                                 GetGridManager(grdVVEMAPRPM).DataSource,
                                                                                                 GetGridManager(grdZoneRPM).DataSource,
                                                                                                 GetGridManager(grdZoneMAP).DataSource,
                                                                                                 GetGridManager(grdHisto).RowHeaders_DBL,
                                                                                                 GetGridManager(grdHisto).ColumnHeaders,
                                                                                                 mnuOptionsVerlonModeSDPatch.Checked)

            Dim DT_TARGET_WITH_NULL As DataTable = clsLib.PasteSpecialMultiplyPercent_Datatables(DT_VVE_ADJUSTED_BREAKPOINTS,
                                                                                                 GetGridManager(grdHisto).DataSource,
                                                                                                 True)

            ARR_SCATTER_POINTS = clsLib.DT_TO_SCATTER_PTS(DT_TARGET_WITH_NULL)

            DT_TARGET_WITH_INTERPOLATION = DT_TARGET_WITH_NULL
            'End If

            'If MSG_RES = MsgBoxResult.No Then
            '    DT_TARGET_WITH_INTERPOLATION = DT_WORKING_VVE.GetCurrentDT
            'End If



            If AUTOTUNE_DIALOG.InterpolateMAP Then
                ' Interpolate MAP values as much as possible
                DT_TARGET_WITH_INTERPOLATION = MATH.InterpolateInteriorMAP(DT_TARGET_WITH_INTERPOLATION)
            End If


            If AUTOTUNE_DIALOG.InterpolateRPM Then
                ' Interpolate one and only one RPM gap
                DT_TARGET_WITH_INTERPOLATION = MATH.InterpolateOneRPMValue(DT_TARGET_WITH_INTERPOLATION)
            End If


            If AUTOTUNE_DIALOG.SmoothMAP Then
                ' Smooth with the grain
                DT_TARGET_WITH_INTERPOLATION = MATH.GaussianSmoothMAP(DT_TARGET_WITH_INTERPOLATION,
                                                                  AUTOTUNE_DIALOG.SmoothMAPSigma,
                                                                  AUTOTUNE_DIALOG.SmoothMAPWindow)
            End If


            If AUTOTUNE_DIALOG.SmoothRPM Then
                ' Smooth against the grain
                DT_TARGET_WITH_INTERPOLATION = MATH.GaussianSmoothRPM(DT_TARGET_WITH_INTERPOLATION,
                                                                  AUTOTUNE_DIALOG.SmoothRPMSigma,
                                                                  AUTOTUNE_DIALOG.SmoothRPMWindow)
            End If


            If AUTOTUNE_DIALOG.ExtrapolateMAPLinear And Not AUTOTUNE_DIALOG.ExtrapolateMAPQuadratic Then
                ' Fill in the missing MAP data for the upper and lower MAP values
                ' we will use the liner and quadratic methods to predict, and then
                ' we will average them for a final result


                DT_TARGET_WITH_INTERPOLATION = MATH.ExtrapolateMAPEdges(DT_TARGET_WITH_INTERPOLATION,
                                                                        AUTOTUNE_DIALOG.ExtrapolateMAPLinearWindow)

            ElseIf Not AUTOTUNE_DIALOG.ExtrapolateMAPLinear And AUTOTUNE_DIALOG.ExtrapolateMAPQuadratic Then
                'DT_TARGET_WITH_INTERPOLATION = MATH.ExtrapolateMAPEdgesQuadratic(DT_TARGET_WITH_INTERPOLATION,
                '                                                                 AUTOTUNE_DIALOG.ExtrapolateMAPQuadraticWindow)
                Dim DT_Quadratic As DataTable = MATH.ExtrapolateMAPEdgesQuadratic(DT_TARGET_WITH_INTERPOLATION)




            ElseIf AUTOTUNE_DIALOG.ExtrapolateMAPLinear And AUTOTUNE_DIALOG.ExtrapolateMAPQuadratic Then

                Dim DT_Linear As DataTable = MATH.ExtrapolateMAPEdges(DT_TARGET_WITH_INTERPOLATION.Copy,
                                                                      AUTOTUNE_DIALOG.ExtrapolateMAPLinearWindow)

                'Dim DT_Quadratic As DataTable = MATH.ExtrapolateMAPEdgesQuadratic(DT_TARGET_WITH_INTERPOLATION,
                '                                                              AUTOTUNE_DIALOG.ExtrapolateMAPQuadraticWindow)
                Dim DT_Quadratic As DataTable = MATH.ExtrapolateMAPEdgesQuadratic(DT_TARGET_WITH_INTERPOLATION.Copy)

                Dim DT_AVE As DataTable = clsLib.Compute_Average_DT({DT_Linear, DT_Quadratic})

                DT_TARGET_WITH_INTERPOLATION = DT_AVE

            End If


            If AUTOTUNE_DIALOG.FillRPMviaVVE Then
                ' default in the rest of the datatable where we don't have fuel trim data and cannot predict the results
                DT_TARGET_WITH_INTERPOLATION = clsLib.DT_ReplaceZeroValuesWithDefaultValues(DT_VVE_ADJUSTED_BREAKPOINTS, DT_TARGET_WITH_INTERPOLATION)
            End If









            If AUTOTUNE_DIALOG.SmoothRPM Then
                ' Smooth against the grain
                DT_TARGET_WITH_INTERPOLATION = MATH.GaussianSmoothRPM(DT_TARGET_WITH_INTERPOLATION,
                                                                  AUTOTUNE_DIALOG.SmoothRPMSigma,
                                                                  AUTOTUNE_DIALOG.SmoothRPMWindow)
            End If


            If AUTOTUNE_DIALOG.InterpolateMAP Then
                ' Interpolate MAP values as much as possible
                DT_TARGET_WITH_INTERPOLATION = MATH.InterpolateInteriorMAP(DT_TARGET_WITH_INTERPOLATION)
            End If





            Dim GRD_MGR As clsGridManager = GetGridManager(grdTune)
            GRD_MGR.DecimalPlaces = 0
            GRD_MGR.SetDatatable(DT_TARGET_WITH_INTERPOLATION, GetGridManager(grdTune).RowHeaders_STR)



            If DT_WORKING_VVE Is Nothing Then
                DT_WORKING_VVE = New clsMulti_Datatable(DT_TARGET_WITH_INTERPOLATION)
            Else
                DT_WORKING_VVE.AppendNewDT(DT_TARGET_WITH_INTERPOLATION)
            End If

            'If FORCE_PAINT_3D_CHART Then
            '    Paint_3D_Graphs()
            'End If

            Return DT_TARGET_WITH_INTERPOLATION

        Catch ex As Exception
            Throw ex
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Function



    Private Function GetExpectedValue(ByVal OLD_VAL As Double, ByVal DT_HISTO As DataTable, ByVal ROW As Integer, ByVal COL As Integer) As Double
        If ROW > DT_HISTO.Rows.Count - 1 Then
            Return OLD_VAL
        End If

        If COL > DT_HISTO.Columns.Count - 1 Then
            Return OLD_VAL
        End If

        If Not IsNumeric(DT_HISTO.Rows(ROW).Item(COL)) Then
            Return OLD_VAL
        End If

        Dim CALC_ERR As Double = CDbl(DT_HISTO.Rows(ROW).Item(COL)) / 100.0
        Return OLD_VAL + (OLD_VAL * CALC_ERR)
    End Function

    Private Sub tsTuneBtnPasteSpecial_Click(sender As Object, e As EventArgs) Handles tsTuneBtnPasteSpecial.ButtonClick,
                                                                                      tsTuneMnuPasteSpecial5.Click,
                                                                                      tsTuneMnuPasteSpecial10.Click,
                                                                                      tsTuneMnuPasteSpecial25.Click,
                                                                                      tsTuneMnuPasteSpecialCustom.Click
        Try


            If sender.Equals(tsTuneMnuPasteSpecialCustom) Then
                Dim s As String = tsTuneMnuPasteSpecialCustom.Tag.ToString

                s = InputBox("How many iterations: ", "Define Custom Iterations", s.ToString)

                If IsNumeric(s) Then
                    tsTuneMnuPasteSpecialCustom.Tag = s.Trim
                Else
                    Exit Sub
                End If
            End If


            If TypeOf sender Is ToolStripMenuItem Then
                ITERATIONS = CInt(DirectCast(sender, ToolStripMenuItem).Tag)
            Else
                ITERATIONS = CInt(DirectCast(sender, ToolStripSplitButton).Tag)
            End If

            'grdVVE.SuspendLayout()
            'grdTune.SuspendLayout()
            'Graph3D_Tune.SuspendLayout()

            PasteSpecialCalcCoeffIterations()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            'grdVVE.ResumeLayout()
            'grdTune.ResumeLayout()
            'Graph3D_Tune.ResumeLayout()

            tsPrg.Value = 0
        End Try
    End Sub


    Private Sub tsTuneBtnInterpolateVertical_Click(sender As Object, e As EventArgs) Handles tsTuneBtnInterpolateVertical.Click,
                                                                                             tsTuneBtnInterpolateHorizontal.Click
        Try
            Dim DIR As clsMath.INTERPOLATE_DIRECTION = clsMath.INTERPOLATE_DIRECTION.Vertical
            If sender.Equals(tsTuneBtnInterpolateHorizontal) Then
                DIR = clsMath.INTERPOLATE_DIRECTION.Horizontal
            End If

            Dim ROW_HEADERS() As String = GetGridManager(grdHisto).RowHeaders_STR

            Dim objMath As New clsMath(SMALL_ZONE_WARN)
            GetGridManager(grdTune).SetDatatable(objMath.InterpolateSelectedCells(grdTune.SelectedCells,
                                                                                  GetGridManager(grdTune).DataSource(False),
                                                                                  DIR),
                                                 ROW_HEADERS)

            CalcCoeffFromBaseVVE(GetGridManager(grdTune).DataSource, ROW_HEADERS, True)
            'Paint_3D_Graphs()


        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsTuneBtnSmooth_Click(sender As Object, e As EventArgs) Handles tsTuneBtnSmooth.Click
        Try
            Dim objMath As New clsMath(SMALL_ZONE_WARN)
            'Dim ROW_HEADERS() As String = GetGridManager(grdHisto).RowHeaders_STR

            'GetGridManager(grdTune).SetDatatable(objMath.SmoothSelectedCells(grdTune.SelectedCells,
            '                                     GetGridManager(grdTune).DataSource(False)),
            '                                     ROW_HEADERS)
            'CalcCoeffFromBaseVVE(GetGridManager(grdTune).DataSource, ROW_HEADERS, True)

            FORM_STATE = enmFORM_STATE.Loading

            'objMath.GaussianSmoothMAP_FromGrid(grdTune)
            objMath.GaussianSmoothMAP_FromGridFast(grdTune)

            GetGridManager(grdTune).ApplyRowHeaders()

            'GetGridManager(grdTune).SetDatatable(GetGridManager(grdTune).DataSource,
            '                                     GetGridManager(grdHisto).RowHeaders_STR)

            'Paint_3D_Graphs()



        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            FORM_STATE = enmFORM_STATE.Normal
            grd_DataBindingComplete(Nothing, Nothing)
        End Try
    End Sub



    Private Sub PasteSpecialCalcCoeffIterations()



        tsPrg.Minimum = 0
        tsPrg.Maximum = ITERATIONS
        tsPrg.Value = 0



        ' --------------------------------------------------------------------------------------------------
        ' Generate the raw VVE new shape
        ' --------------------------------------------------------------------------------------------------
        Dim DT_OLD As New DataTable
        Dim DT_NEW As New DataTable
        Dim DT_PASTE_SPECIAL As New DataTable
        Dim ROW_HEADERS(-1) As String

        For i As Integer = 1 To ITERATIONS

            CalcVVE(0, DT_OLD, ROW_HEADERS)
            CalcVVE(1, DT_NEW, ROW_HEADERS)

            CalcPasteSpecial(GetGridManager(grdHisto).DataSource, DT_OLD, DT_NEW, DT_PASTE_SPECIAL, True)
            Dim DT_VVE_NEW_RAW As DataTable = clsLib.PasteSpecialMultiplyPercent_Datatables(DT_NEW, DT_PASTE_SPECIAL)


            CalcCoeffFromBaseVVE(DT_VVE_NEW_RAW, GetGridManager(grdHisto).RowHeaders_STR, True)

            ' refresh the current view
            tsTuneMnuViewVVENew_Click(Nothing, Nothing)

            If tsPrg.Value < tsPrg.Maximum Then
                tsPrg.Value += 1
                'tsPrg.Invalidate()
            End If

        Next


    End Sub

    Private Sub tsTuneBtnPopOutEditor_Click(sender As Object, e As EventArgs) Handles tsTuneBtnPopOutEditor.Click
        Try
            'Dim objPopOut As New frmPopOutEditor(GetGridManager(grdZoneRPM), GetGridManager(grdZoneMAP), grdZoneRPM)
            Dim objPopOut As New frmPopOutEditor(GetGridManager(grdZoneRPM).DataSource(True), GetGridManager(grdZoneMAP).DataSource(True))
            objPopOut.Show()


            tabMain_SelectedIndexChanged(Nothing, Nothing)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Public Sub UpdateZonesFromChildForm(ByVal DT_RPM As DataTable, ByVal DT_MAP As DataTable)

        Dim GRD_MGR_RPM As clsGridManager = GetGridManager(grdZoneRPM)
        Dim ROW_HEADERS_RPM() As String = GRD_MGR_RPM.RowHeaders_STR
        GRD_MGR_RPM.SetDatatable(DT_RPM, ROW_HEADERS_RPM)

        Dim GRD_MGR_MAP As clsGridManager = GetGridManager(grdZoneMAP)
        Dim ROW_HEADERS_MAP() As String = GRD_MGR_MAP.RowHeaders_STR
        GRD_MGR_MAP.SetDatatable(DT_MAP, ROW_HEADERS_MAP)

        CalcCoeffFromBaseVVE(GetGridManager(grdVVE).DataSource,
                     GetGridManager(grdVVE).RowHeaders_STR, ' it is critical to pass in the VVE headers (and not use GRD_MGR since it could also be from the grdZone*) from the base VVE table (NOT THE HISTO HEADERS since we cannot garuntee they match at this point)
                     True)

        grdZoneRPM.Invalidate()
        grdZoneMAP.Invalidate()
        grdHisto.Invalidate()
        grdVVE.Invalidate()
        grdTune.Invalidate()


        tabMain_SelectedIndexChanged(Nothing, Nothing)

    End Sub

    Private Sub VerlonModeSDPatchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles mnuOptionsVerlonModeSDPatch.Click
        Try
            mnuOptionsVerlonModeSDPatch.Checked = Not mnuOptionsVerlonModeSDPatch.Checked

            ' 1) Re-calc coefficients
            Try
                CalcCoeffFromBaseVVE(GetGridManager(grdVVE).DataSource,
                                     GetGridManager(grdVVE).RowHeaders_STR, ' it is critical to pass in the VVE headers (and not use GRD_MGR since it could also be from the grdZone*) from the base VVE table (NOT THE HISTO HEADERS since we cannot garuntee they match at this point)
                                     False)
            Catch ex As Exception
                If mnuOptionsVerlonModeSDPatch.Checked Then
                    MsgBox("[Verlon Mode] Error calculating coefficients: " & ex.Message, MsgBoxStyle.Critical)
                Else
                    MsgBox("[Standard Mode] Error calculating coefficients: " & ex.Message, MsgBoxStyle.Critical)
                End If
            End Try

            ' 2) Update 3D Graph
            'Paint_3D_Graphs()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Public Sub UpdateWorkingVVEFromChart(ByVal X As Integer, ByVal Y As Integer, ByVal NewZ As Double)


        If Graph3D_Tune.IsCurrentlyEditing Then
            Exit Sub
        End If

        Dim DT As DataTable = GetGridManager(grdTune).DataSource
        DT.Rows(X).Item(Y) = NewZ

        Dim ROW_HEADERS() As String = GetGridManager(grdTune).RowHeaders_STR
        GetGridManager(grdTune).SetDatatable(DT, ROW_HEADERS)

    End Sub


    Private Sub UpdateVVEFromChart(ByVal X As Integer, ByVal Y As Integer, ByVal NewZ As Double)

        If Graph3D_VVE.IsCurrentlyEditing Then
            Exit Sub
        End If

        Dim DT As DataTable = GetGridManager(grdVVE).DataSource
        DT.Rows(X).Item(Y) = NewZ

        Dim ROW_HEADERS() As String = GetGridManager(grdVVE).RowHeaders_STR
        GetGridManager(grdVVE).SetDatatable(DT, ROW_HEADERS)

    End Sub



End Class