Public Class frmAutotune

    Private FORM_LOADING As Boolean = True

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public ReadOnly Property FORMLOADING As Boolean
        Get
            Return FORM_LOADING
        End Get
    End Property


    Public ReadOnly Property InterpolateMAP As Boolean
        Get
            Return chkIntMAP.Checked
        End Get
    End Property
    Public ReadOnly Property InterpolateRPM As Boolean
        Get
            Return chkIntRPM.Checked
        End Get
    End Property


    Public ReadOnly Property SmoothMAP As Boolean
        Get
            Return chkSmoothMAP.Checked
        End Get
    End Property
    Public ReadOnly Property SmoothMAPSigma As Decimal
        Get
            Return spinSigma_MAP_Smooth.Value
        End Get
    End Property
    Public ReadOnly Property SmoothMAPWindow As Decimal
        Get
            Return spinWindow_MAP_Smooth.Value
        End Get
    End Property



    Public ReadOnly Property SmoothRPM As Boolean
        Get
            Return chkSmoothRPM.Checked
        End Get
    End Property
    Public ReadOnly Property SmoothRPMSigma As Decimal
        Get
            Return spinSigma_RPM_Smooth.Value
        End Get
    End Property
    Public ReadOnly Property SmoothRPMWindow As Decimal
        Get
            Return spinWindow_RPM_Smooth.Value
        End Get
    End Property



    Public ReadOnly Property ExtrapolateMAPLinear As Boolean
        Get
            Return chkExtrapolateMAPLinear.Checked
        End Get
    End Property
    Public ReadOnly Property ExtrapolateMAPLinearWindow As Decimal
        Get
            Return spinWindow_Linear_Extrapolate.Value
        End Get
    End Property



    Public ReadOnly Property ExtrapolateMAPQuadratic As Boolean
        Get
            Return chkExtrapolateMAPQuadratic.Checked
        End Get
    End Property
    Public ReadOnly Property ExtrapolateMAPQuadraticWindow As Decimal
        Get
            Return spinWindow_Quadratic_Extrapolate.Value
        End Get
    End Property




    Public ReadOnly Property FillRPMviaVVE As Boolean
        Get
            Return chkFillRPMviaVVE.Checked
        End Get
    End Property


    Private Sub frmAutotune_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            FORM_LOADING = True

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        Finally
            FORM_LOADING = False
        End Try
    End Sub


    Private Sub SettingsChanged(sender As Object, e As EventArgs) Handles chkIntMAP.CheckedChanged,
                                                                          chkIntRPM.CheckedChanged,
                                                                          chkSmoothMAP.CheckedChanged,
                                                                          chkSmoothRPM.CheckedChanged,
                                                                          chkExtrapolateMAPLinear.CheckedChanged,
                                                                          chkExtrapolateMAPQuadratic.CheckedChanged,
                                                                          chkFillRPMviaVVE.CheckedChanged,
                                                                          spinSigma_MAP_Smooth.ValueChanged,
                                                                          spinSigma_RPM_Smooth.ValueChanged,
                                                                          spinWindow_MAP_Smooth.ValueChanged,
                                                                          spinWindow_RPM_Smooth.ValueChanged,
                                                                          spinWindow_Linear_Extrapolate.ValueChanged,
                                                                          spinWindow_Quadratic_Extrapolate.ValueChanged
        Try
            If FORM_LOADING Then
                Exit Sub
            End If

            'chkSmoothMAP
            spinSigma_MAP_Smooth.Enabled = chkSmoothMAP.Checked
            spinWindow_MAP_Smooth.Enabled = chkSmoothMAP.Checked

            spinSigma_RPM_Smooth.Enabled = chkSmoothRPM.Checked
            spinWindow_RPM_Smooth.Enabled = chkSmoothRPM.Checked

            spinWindow_Linear_Extrapolate.Enabled = chkExtrapolateMAPLinear.Checked
            spinWindow_Quadratic_Extrapolate.Enabled = chkExtrapolateMAPQuadratic.Checked

            chkAverage.Checked = chkExtrapolateMAPLinear.Checked And chkExtrapolateMAPQuadratic.Checked

            frmMain.Autotune()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub


End Class