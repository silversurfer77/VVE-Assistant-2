

Imports Graph3D
Imports VVE_Assistant_2.frmMain

Public Class frmCamImport

    Public Enum enmWhichCam
        Intake
        Exhaust
    End Enum

    Private CAM_SELECTION As enmWhichCam
    Private CANCELED As Boolean = True

    Public Sub New(ByVal FONT_SIZE As Integer, ByVal WHICH_CAM As enmWhichCam)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim fnt As New Font(Me.Font.FontFamily, FONT_SIZE, FontStyle.Regular)
        Me.Font = fnt
        'Me.Size = sz

        CAM_SELECTION = WHICH_CAM
    End Sub

    Public ReadOnly Property CAM_VVE As DataTable
        Get
            Return DirectCast(grdVVECam.DataSource, DataTable)
        End Get
    End Property

    Public ReadOnly Property User_Canceled As Boolean
        Get
            Return CANCELED
        End Get
    End Property


    Private Sub frmCamImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            grdVVECam.GetType.InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.SetProperty, Nothing, grdVVECam, New Object() {True})


            Dim INSTRUCTIONS As String = "HPT Editor > Edit > Virtual Volumetric Efficiency > Set "
            If CAM_SELECTION = enmWhichCam.Intake Then
                INSTRUCTIONS += "INTAKE"
            Else
                INSTRUCTIONS += "EXHAUST"
            End If
            INSTRUCTIONS += " Cam Angle to 20º > Copy with axis > Paste Here"
            lblInstructions.Text = INSTRUCTIONS

            Dim myToolTipText = INSTRUCTIONS
            ToolTip1.SetToolTip(lblInstructions, myToolTipText)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub tsBtnPaste_Click(sender As Object, e As EventArgs) Handles tsBtnPaste.Click
        Try
            Dim ROW_HEADERS(-1) As String
            Dim NUMERIC_COL_HEADERS As Boolean = True

            Dim DT As DataTable = clsLib.ClipboardToDatatable(ROW_HEADERS, True)
            Dim GRD_MGR As New clsGridManager(grdVVECam, 0)
            GRD_MGR.SetDatatable(DT, ROW_HEADERS)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub




    Private Sub grdVVECam_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles grdVVECam.CellPainting
        Try
            If grdVVECam.Rows.Count = 0 Then
                Exit Sub
            End If
            clsLib.Paint_VVE_Editor_Style(grdVVECam, e)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub grd_ColumnAdded(sender As Object, e As DataGridViewColumnEventArgs) Handles grdVVECam.ColumnAdded
        Try
            For Each COL As DataGridViewColumn In grdVVECam.Columns
                COL.SortMode = DataGridViewColumnSortMode.NotSortable
            Next
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsBtnSaveExit_Click(sender As Object, e As EventArgs) Handles tsBtnSaveExit.Click
        Try
            CANCELED = False
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub tsBtnCancel_Click(sender As Object, e As EventArgs) Handles tsBtnCancel.Click
        Try
            CANCELED = True
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
End Class