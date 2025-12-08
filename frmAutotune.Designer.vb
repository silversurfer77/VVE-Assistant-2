<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAutotune
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.chkExtrapolateMAPLinear = New System.Windows.Forms.CheckBox()
        Me.chkSmoothMAP = New System.Windows.Forms.CheckBox()
        Me.chkSmoothRPM = New System.Windows.Forms.CheckBox()
        Me.chkIntMAP = New System.Windows.Forms.CheckBox()
        Me.chkIntRPM = New System.Windows.Forms.CheckBox()
        Me.chkFillRPMviaVVE = New System.Windows.Forms.CheckBox()
        Me.spinSigma_MAP_Smooth = New System.Windows.Forms.NumericUpDown()
        Me.spinWindow_MAP_Smooth = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.spinWindow_RPM_Smooth = New System.Windows.Forms.NumericUpDown()
        Me.spinSigma_RPM_Smooth = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.spinWindow_Linear_Extrapolate = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.spinWindow_Quadratic_Extrapolate = New System.Windows.Forms.NumericUpDown()
        Me.chkAverage = New System.Windows.Forms.CheckBox()
        Me.chkExtrapolateMAPQuadratic = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        CType(Me.spinSigma_MAP_Smooth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.spinWindow_MAP_Smooth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.spinWindow_RPM_Smooth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.spinSigma_RPM_Smooth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.spinWindow_Linear_Extrapolate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.spinWindow_Quadratic_Extrapolate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkExtrapolateMAPLinear
        '
        Me.chkExtrapolateMAPLinear.AutoSize = True
        Me.chkExtrapolateMAPLinear.Checked = True
        Me.chkExtrapolateMAPLinear.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkExtrapolateMAPLinear.Location = New System.Drawing.Point(6, 19)
        Me.chkExtrapolateMAPLinear.Name = "chkExtrapolateMAPLinear"
        Me.chkExtrapolateMAPLinear.Size = New System.Drawing.Size(105, 17)
        Me.chkExtrapolateMAPLinear.TabIndex = 57
        Me.chkExtrapolateMAPLinear.Text = "Linear Prediction"
        Me.chkExtrapolateMAPLinear.UseVisualStyleBackColor = True
        '
        'chkSmoothMAP
        '
        Me.chkSmoothMAP.AutoSize = True
        Me.chkSmoothMAP.Checked = True
        Me.chkSmoothMAP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSmoothMAP.Location = New System.Drawing.Point(8, 19)
        Me.chkSmoothMAP.Name = "chkSmoothMAP"
        Me.chkSmoothMAP.Size = New System.Drawing.Size(49, 17)
        Me.chkSmoothMAP.TabIndex = 55
        Me.chkSmoothMAP.Text = "MAP"
        Me.chkSmoothMAP.UseVisualStyleBackColor = True
        '
        'chkSmoothRPM
        '
        Me.chkSmoothRPM.AutoSize = True
        Me.chkSmoothRPM.Checked = True
        Me.chkSmoothRPM.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSmoothRPM.Location = New System.Drawing.Point(8, 49)
        Me.chkSmoothRPM.Name = "chkSmoothRPM"
        Me.chkSmoothRPM.Size = New System.Drawing.Size(50, 17)
        Me.chkSmoothRPM.TabIndex = 56
        Me.chkSmoothRPM.Text = "RPM"
        Me.chkSmoothRPM.UseVisualStyleBackColor = True
        '
        'chkIntMAP
        '
        Me.chkIntMAP.AutoSize = True
        Me.chkIntMAP.Checked = True
        Me.chkIntMAP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIntMAP.Location = New System.Drawing.Point(6, 19)
        Me.chkIntMAP.Name = "chkIntMAP"
        Me.chkIntMAP.Size = New System.Drawing.Size(49, 17)
        Me.chkIntMAP.TabIndex = 53
        Me.chkIntMAP.Text = "MAP"
        Me.chkIntMAP.UseVisualStyleBackColor = True
        '
        'chkIntRPM
        '
        Me.chkIntRPM.AutoSize = True
        Me.chkIntRPM.Checked = True
        Me.chkIntRPM.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIntRPM.Location = New System.Drawing.Point(6, 49)
        Me.chkIntRPM.Name = "chkIntRPM"
        Me.chkIntRPM.Size = New System.Drawing.Size(50, 17)
        Me.chkIntRPM.TabIndex = 54
        Me.chkIntRPM.Text = "RPM"
        Me.chkIntRPM.UseVisualStyleBackColor = True
        '
        'chkFillRPMviaVVE
        '
        Me.chkFillRPMviaVVE.AutoSize = True
        Me.chkFillRPMviaVVE.Checked = True
        Me.chkFillRPMviaVVE.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFillRPMviaVVE.Location = New System.Drawing.Point(8, 19)
        Me.chkFillRPMviaVVE.Name = "chkFillRPMviaVVE"
        Me.chkFillRPMviaVVE.Size = New System.Drawing.Size(198, 17)
        Me.chkFillRPMviaVVE.TabIndex = 58
        Me.chkFillRPMviaVVE.Text = "Fill Missing Data (from previous VVE)"
        Me.chkFillRPMviaVVE.UseVisualStyleBackColor = True
        '
        'spinSigma_MAP_Smooth
        '
        Me.spinSigma_MAP_Smooth.DecimalPlaces = 1
        Me.spinSigma_MAP_Smooth.Location = New System.Drawing.Point(147, 16)
        Me.spinSigma_MAP_Smooth.Name = "spinSigma_MAP_Smooth"
        Me.spinSigma_MAP_Smooth.Size = New System.Drawing.Size(52, 20)
        Me.spinSigma_MAP_Smooth.TabIndex = 59
        Me.spinSigma_MAP_Smooth.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'spinWindow_MAP_Smooth
        '
        Me.spinWindow_MAP_Smooth.Location = New System.Drawing.Point(283, 18)
        Me.spinWindow_MAP_Smooth.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.spinWindow_MAP_Smooth.Name = "spinWindow_MAP_Smooth"
        Me.spinWindow_MAP_Smooth.Size = New System.Drawing.Size(59, 20)
        Me.spinWindow_MAP_Smooth.TabIndex = 60
        Me.spinWindow_MAP_Smooth.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(105, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 13)
        Me.Label1.TabIndex = 61
        Me.Label1.Text = "Sigma"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(231, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 13)
        Me.Label2.TabIndex = 62
        Me.Label2.Text = "Window"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(231, 49)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 66
        Me.Label3.Text = "Window"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(105, 50)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(36, 13)
        Me.Label4.TabIndex = 65
        Me.Label4.Text = "Sigma"
        '
        'spinWindow_RPM_Smooth
        '
        Me.spinWindow_RPM_Smooth.Location = New System.Drawing.Point(283, 47)
        Me.spinWindow_RPM_Smooth.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.spinWindow_RPM_Smooth.Name = "spinWindow_RPM_Smooth"
        Me.spinWindow_RPM_Smooth.Size = New System.Drawing.Size(59, 20)
        Me.spinWindow_RPM_Smooth.TabIndex = 64
        Me.spinWindow_RPM_Smooth.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'spinSigma_RPM_Smooth
        '
        Me.spinSigma_RPM_Smooth.DecimalPlaces = 1
        Me.spinSigma_RPM_Smooth.Location = New System.Drawing.Point(147, 43)
        Me.spinSigma_RPM_Smooth.Name = "spinSigma_RPM_Smooth"
        Me.spinSigma_RPM_Smooth.Size = New System.Drawing.Size(52, 20)
        Me.spinSigma_RPM_Smooth.TabIndex = 63
        Me.spinSigma_RPM_Smooth.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(231, 18)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(46, 13)
        Me.Label5.TabIndex = 68
        Me.Label5.Text = "Window"
        '
        'spinWindow_Linear_Extrapolate
        '
        Me.spinWindow_Linear_Extrapolate.Location = New System.Drawing.Point(283, 16)
        Me.spinWindow_Linear_Extrapolate.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.spinWindow_Linear_Extrapolate.Name = "spinWindow_Linear_Extrapolate"
        Me.spinWindow_Linear_Extrapolate.Size = New System.Drawing.Size(59, 20)
        Me.spinWindow_Linear_Extrapolate.TabIndex = 67
        Me.spinWindow_Linear_Extrapolate.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.spinWindow_Quadratic_Extrapolate)
        Me.GroupBox1.Controls.Add(Me.chkAverage)
        Me.GroupBox1.Controls.Add(Me.chkExtrapolateMAPQuadratic)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.chkExtrapolateMAPLinear)
        Me.GroupBox1.Controls.Add(Me.spinWindow_Linear_Extrapolate)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 175)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(363, 89)
        Me.GroupBox1.TabIndex = 69
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Extrapolate Data"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(231, 44)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(46, 13)
        Me.Label6.TabIndex = 71
        Me.Label6.Text = "Window"
        '
        'spinWindow_Quadratic_Extrapolate
        '
        Me.spinWindow_Quadratic_Extrapolate.Location = New System.Drawing.Point(283, 42)
        Me.spinWindow_Quadratic_Extrapolate.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.spinWindow_Quadratic_Extrapolate.Name = "spinWindow_Quadratic_Extrapolate"
        Me.spinWindow_Quadratic_Extrapolate.Size = New System.Drawing.Size(59, 20)
        Me.spinWindow_Quadratic_Extrapolate.TabIndex = 70
        Me.spinWindow_Quadratic_Extrapolate.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'chkAverage
        '
        Me.chkAverage.AutoSize = True
        Me.chkAverage.Checked = True
        Me.chkAverage.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAverage.Enabled = False
        Me.chkAverage.Location = New System.Drawing.Point(6, 65)
        Me.chkAverage.Name = "chkAverage"
        Me.chkAverage.Size = New System.Drawing.Size(66, 17)
        Me.chkAverage.TabIndex = 72
        Me.chkAverage.Text = "Average"
        Me.chkAverage.UseVisualStyleBackColor = True
        '
        'chkExtrapolateMAPQuadratic
        '
        Me.chkExtrapolateMAPQuadratic.AutoSize = True
        Me.chkExtrapolateMAPQuadratic.Checked = True
        Me.chkExtrapolateMAPQuadratic.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkExtrapolateMAPQuadratic.Location = New System.Drawing.Point(6, 42)
        Me.chkExtrapolateMAPQuadratic.Name = "chkExtrapolateMAPQuadratic"
        Me.chkExtrapolateMAPQuadratic.Size = New System.Drawing.Size(122, 17)
        Me.chkExtrapolateMAPQuadratic.TabIndex = 69
        Me.chkExtrapolateMAPQuadratic.Text = "Quadratic Prediction"
        Me.chkExtrapolateMAPQuadratic.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.chkIntMAP)
        Me.GroupBox2.Controls.Add(Me.chkIntRPM)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(363, 74)
        Me.GroupBox2.TabIndex = 73
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Interpolate Data"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.chkSmoothMAP)
        Me.GroupBox3.Controls.Add(Me.chkSmoothRPM)
        Me.GroupBox3.Controls.Add(Me.spinSigma_MAP_Smooth)
        Me.GroupBox3.Controls.Add(Me.spinWindow_MAP_Smooth)
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Controls.Add(Me.spinWindow_RPM_Smooth)
        Me.GroupBox3.Controls.Add(Me.spinSigma_RPM_Smooth)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 92)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(363, 77)
        Me.GroupBox3.TabIndex = 74
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Smooth Data"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.chkFillRPMviaVVE)
        Me.GroupBox4.Location = New System.Drawing.Point(12, 270)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(363, 47)
        Me.GroupBox4.TabIndex = 75
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Default Data"
        '
        'frmAutotune
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(383, 323)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmAutotune"
        Me.Text = "Autotune Settings"
        CType(Me.spinSigma_MAP_Smooth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.spinWindow_MAP_Smooth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.spinWindow_RPM_Smooth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.spinSigma_RPM_Smooth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.spinWindow_Linear_Extrapolate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.spinWindow_Quadratic_Extrapolate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents chkExtrapolateMAPLinear As CheckBox
    Friend WithEvents chkSmoothMAP As CheckBox
    Friend WithEvents chkSmoothRPM As CheckBox
    Friend WithEvents chkIntMAP As CheckBox
    Friend WithEvents chkIntRPM As CheckBox
    Friend WithEvents chkFillRPMviaVVE As CheckBox
    Friend WithEvents spinSigma_MAP_Smooth As NumericUpDown
    Friend WithEvents spinWindow_MAP_Smooth As NumericUpDown
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents spinWindow_RPM_Smooth As NumericUpDown
    Friend WithEvents spinSigma_RPM_Smooth As NumericUpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents spinWindow_Linear_Extrapolate As NumericUpDown
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents spinWindow_Quadratic_Extrapolate As NumericUpDown
    Friend WithEvents chkExtrapolateMAPQuadratic As CheckBox
    Friend WithEvents chkAverage As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents GroupBox4 As GroupBox
End Class
