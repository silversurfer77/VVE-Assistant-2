Imports VVE_Assistant_2.frmMain

Public Class frmPopOutEditor
    'Private OLD_GRD_MGR_RPM As clsGridManager
    'Private OLD_GRD_MGR_MAP As clsGridManager

    'Private NEW_GRD_MGR_RPM As clsGridManager
    'Private NEW_GRD_MGR_MAP As clsGridManager

    Private _DT_RPM As DataTable
    Private _DT_MAP As DataTable

    Private _FORM_LOADING As Boolean = True



    'Public Sub New(ByRef GRD_MGR_RPM As clsGridManager, ByRef GRD_MGR_MAP As clsGridManager, ByVal GRD_RPM As DataGridView)

    '    ' This call is required by the designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    grdZoneRPM.Tag = GRD_RPM.Tag


    '    OLD_GRD_MGR_RPM = GRD_MGR_RPM
    '    OLD_GRD_MGR_MAP = GRD_MGR_MAP


    'End Sub

    Public Sub New(ByVal DT_RPM As DataTable, ByVal DT_MAP As DataTable)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _DT_RPM = DT_RPM
        _DT_MAP = DT_MAP
    End Sub





    Private Sub frmCamAngle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            FillGrids()
            FillCombo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            _FORM_LOADING = False
        End Try
    End Sub

    'Private Sub FillGrids()
    '    NEW_GRD_MGR_RPM = New clsGridManager(grdZoneRPM, 0)
    '    NEW_GRD_MGR_RPM.SetDatatable(OLD_GRD_MGR_RPM.DataSource, {})

    '    NEW_GRD_MGR_MAP = New clsGridManager(grdZoneMAP, 0)
    '    NEW_GRD_MGR_MAP.SetDatatable(OLD_GRD_MGR_MAP.DataSource, {})
    'End Sub

    Private Sub FillGrids()
        grdZoneRPM.Tag = New clsGridManager(grdZoneRPM, 0)
        grdZoneMAP.Tag = New clsGridManager(grdZoneMAP, 0)

        DirectCast(grdZoneRPM.Tag, clsGridManager).SetDatatable(_DT_RPM, {})
        DirectCast(grdZoneMAP.Tag, clsGridManager).SetDatatable(_DT_MAP, {})
    End Sub

    Private Sub FillCombo()
        Dim objDT_CamOrCrankAngle As New DataTable
        Dim objCol_CamOrCrankAngle As DataColumn
        Dim objRow_CamOrCrankAngle As DataRow

        objCol_CamOrCrankAngle = New DataColumn
        objCol_CamOrCrankAngle.DataType = System.Type.GetType("System.String")
        objCol_CamOrCrankAngle.ColumnName = "Display"
        objDT_CamOrCrankAngle.Columns.Add(objCol_CamOrCrankAngle)

        objCol_CamOrCrankAngle = New DataColumn
        objCol_CamOrCrankAngle.DataType = System.Type.GetType("System.Byte")
        objCol_CamOrCrankAngle.ColumnName = "Value"
        objDT_CamOrCrankAngle.Columns.Add(objCol_CamOrCrankAngle)

        objRow_CamOrCrankAngle = objDT_CamOrCrankAngle.NewRow
        objRow_CamOrCrankAngle("Value") = 2 'will be used to mupltiply the In/Ex cam angle
        objRow_CamOrCrankAngle("Display") = "Gen 4: Crankº Reference"
        objDT_CamOrCrankAngle.Rows.Add(objRow_CamOrCrankAngle)

        objRow_CamOrCrankAngle = objDT_CamOrCrankAngle.NewRow
        objRow_CamOrCrankAngle("Value") = 1 'will be used to mupltiply the In/Ex cam angle
        objRow_CamOrCrankAngle("Display") = "Gen 5: Camº Reference"
        objDT_CamOrCrankAngle.Rows.Add(objRow_CamOrCrankAngle)

        cmbCamOrCrankAngle.DataSource = objDT_CamOrCrankAngle
        cmbCamOrCrankAngle.DisplayMember = "Display"
        cmbCamOrCrankAngle.ValueMember = "Value"
        cmbCamOrCrankAngle.SelectedIndex = 0

        'If OLD_GRD_MGR_MAP IsNot Nothing Then
        '    Dim MIN As Double = 0.0
        '    Dim MAX As Double = 0.0
        '    clsLib.FindMaxDataTableValue(OLD_GRD_MGR_MAP.DataSource, MAX, MIN)

        '    If MAX < 5.0 Then 'technically should not exceed 3.0
        '        'Gen5 detected!
        '        cmbCamOrCrankAngle.SelectedValue = 1
        '        NEW_GRD_MGR_MAP.DecimalPlaces = 2
        '    End If
        'End If


        Dim MIN As Double = 0.0
        Dim MAX As Double = 0.0
        clsLib.FindMaxDataTableValue(_DT_MAP, MAX, MIN)

        If MAX < 5.0 Then 'technically should not exceed 3.0
            'Gen5 detected!
            cmbCamOrCrankAngle.SelectedValue = 1
            DirectCast(grdZoneMAP.Tag, clsGridManager).DecimalPlaces = 2
        End If






    End Sub



    Private Sub lnkCamOrCrankAngle_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkCamOrCrankAngle.LinkClicked
        Try
            Dim strMessage As String = "Some ECM's (or perhaps just the HPT Editor) reference the 'cam angle' as either cam degrees or crank degrees. " &
                                       "Since the cam rotates at half the rate of the crank, 2º crank rotation = 1º cam rotation. " &
                                       "So in order to calculate the VVE correctly, this descrepancy must be taken into account."

            MsgBox(strMessage, MsgBoxStyle.Information)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub grdZoneMAP_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles grdZoneMAP.CellPainting, grdZoneRPM.CellPainting
        Try
            If _FORM_LOADING Then
                Exit Sub
            End If

            Dim GRD As DataGridView = DirectCast(sender, DataGridView)
            clsLib.Paint_VVE_Editor_Style(GRD, e)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub grd_ColumnAdded(sender As Object, e As DataGridViewColumnEventArgs) Handles grdZoneMAP.ColumnAdded,
                                                                                            grdZoneRPM.ColumnAdded

        Try
            DirectCast(DirectCast(sender, DataGridView).Tag, clsGridManager).PreventGridSort()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Private Sub grdZone_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles grdZoneRPM.CellEndEdit, grdZoneMAP.CellEndEdit
        Try
            Dim GRD_MGR_RPM As clsGridManager = DirectCast(grdZoneRPM.Tag, clsGridManager)
            Dim GRD_MGR_MAP As clsGridManager = DirectCast(grdZoneMAP.Tag, clsGridManager)

            GRD_MGR_RPM.SetDatatable(grdZoneRPM.DataSource, GRD_MGR_RPM.RowHeaders_STR)
            GRD_MGR_MAP.SetDatatable(grdZoneMAP.DataSource, GRD_MGR_MAP.RowHeaders_STR)



            frmMain.UpdateZonesFromChildForm(GRD_MGR_RPM.DataSource(True),
                                             GRD_MGR_MAP.DataSource(True))

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub










End Class