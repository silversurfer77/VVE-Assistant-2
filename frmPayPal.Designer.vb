<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPayPal
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPayPal))
        Me.lblThankYou = New System.Windows.Forms.Label()
        Me.lnkCopyToClipboard = New System.Windows.Forms.LinkLabel()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblThankYou
        '
        Me.lblThankYou.AutoSize = True
        Me.lblThankYou.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblThankYou.Location = New System.Drawing.Point(188, 92)
        Me.lblThankYou.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblThankYou.Name = "lblThankYou"
        Me.lblThankYou.Size = New System.Drawing.Size(91, 13)
        Me.lblThankYou.TabIndex = 7
        Me.lblThankYou.Text = "THANK YOU!!!"
        '
        'lnkCopyToClipboard
        '
        Me.lnkCopyToClipboard.AutoSize = True
        Me.lnkCopyToClipboard.Location = New System.Drawing.Point(387, 43)
        Me.lnkCopyToClipboard.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lnkCopyToClipboard.Name = "lnkCopyToClipboard"
        Me.lnkCopyToClipboard.Size = New System.Drawing.Size(134, 20)
        Me.lnkCopyToClipboard.TabIndex = 6
        Me.lnkCopyToClipboard.TabStop = True
        Me.lnkCopyToClipboard.Text = "Copy to Clipboard"
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(22, 38)
        Me.txtEmail.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.ReadOnly = True
        Me.txtEmail.Size = New System.Drawing.Size(354, 26)
        Me.txtEmail.TabIndex = 5
        Me.txtEmail.Text = "dankunkel@gmail.com"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 14)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(492, 20)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "I appreciate any and all support you want to throw my way via PayPal!"
        '
        'frmPayPal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(525, 121)
        Me.Controls.Add(Me.lblThankYou)
        Me.Controls.Add(Me.lnkCopyToClipboard)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmPayPal"
        Me.Text = "Donate to the Cause"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblThankYou As Label
    Friend WithEvents lnkCopyToClipboard As LinkLabel
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents Label1 As Label
End Class
