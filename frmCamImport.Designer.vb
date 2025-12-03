<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCamImport
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCamImport))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.ts = New System.Windows.Forms.ToolStrip()
        Me.tsBtnPaste = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsBtnSaveExit = New System.Windows.Forms.ToolStripButton()
        Me.tsBtnCancel = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsBtnClear = New System.Windows.Forms.ToolStripButton()
        Me.grdVVECam = New System.Windows.Forms.DataGridView()
        Me.lblInstructions = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tsBtnCopy = New System.Windows.Forms.ToolStripSplitButton()
        Me.tsMnuCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsMnuCopyWithAxis = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.ts.SuspendLayout()
        CType(Me.grdVVECam, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 1
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.ts, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.grdVVECam, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.lblInstructions, 0, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 3
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(800, 450)
        Me.TableLayoutPanel3.TabIndex = 40
        '
        'ts
        '
        Me.ts.AutoSize = False
        Me.ts.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnPaste, Me.tsBtnSaveExit, Me.tsBtnCancel, Me.ToolStripSeparator1, Me.tsBtnCopy, Me.ToolStripSeparator14, Me.tsBtnClear})
        Me.ts.Location = New System.Drawing.Point(0, 20)
        Me.ts.Name = "ts"
        Me.ts.Size = New System.Drawing.Size(800, 50)
        Me.ts.TabIndex = 40
        '
        'tsBtnPaste
        '
        Me.tsBtnPaste.AutoSize = False
        Me.tsBtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnPaste.Image = CType(resources.GetObject("tsBtnPaste.Image"), System.Drawing.Image)
        Me.tsBtnPaste.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnPaste.Name = "tsBtnPaste"
        Me.tsBtnPaste.Size = New System.Drawing.Size(36, 38)
        Me.tsBtnPaste.Text = "Paste"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 50)
        '
        'tsBtnSaveExit
        '
        Me.tsBtnSaveExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnSaveExit.Image = CType(resources.GetObject("tsBtnSaveExit.Image"), System.Drawing.Image)
        Me.tsBtnSaveExit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBtnSaveExit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnSaveExit.Name = "tsBtnSaveExit"
        Me.tsBtnSaveExit.Size = New System.Drawing.Size(40, 47)
        Me.tsBtnSaveExit.Text = "Save & Exit"
        Me.tsBtnSaveExit.ToolTipText = "Save & Exit"
        '
        'tsBtnCancel
        '
        Me.tsBtnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnCancel.Image = CType(resources.GetObject("tsBtnCancel.Image"), System.Drawing.Image)
        Me.tsBtnCancel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBtnCancel.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnCancel.Name = "tsBtnCancel"
        Me.tsBtnCancel.Size = New System.Drawing.Size(40, 47)
        Me.tsBtnCancel.Text = "Cancel"
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        Me.ToolStripSeparator14.Size = New System.Drawing.Size(6, 50)
        '
        'tsBtnClear
        '
        Me.tsBtnClear.AutoSize = False
        Me.tsBtnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnClear.Image = CType(resources.GetObject("tsBtnClear.Image"), System.Drawing.Image)
        Me.tsBtnClear.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBtnClear.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnClear.Name = "tsBtnClear"
        Me.tsBtnClear.Size = New System.Drawing.Size(50, 38)
        Me.tsBtnClear.Text = "Clear"
        '
        'grdVVECam
        '
        Me.grdVVECam.AllowUserToAddRows = False
        Me.grdVVECam.AllowUserToDeleteRows = False
        Me.grdVVECam.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdVVECam.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.grdVVECam.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaption
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.NullValue = Nothing
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.grdVVECam.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.grdVVECam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdVVECam.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grdVVECam.EnableHeadersVisualStyles = False
        Me.grdVVECam.Location = New System.Drawing.Point(4, 75)
        Me.grdVVECam.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.grdVVECam.Name = "grdVVECam"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ActiveCaption
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.NullValue = Nothing
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.grdVVECam.RowHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.grdVVECam.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        Me.grdVVECam.ShowEditingIcon = False
        Me.grdVVECam.Size = New System.Drawing.Size(792, 370)
        Me.grdVVECam.TabIndex = 39
        '
        'lblInstructions
        '
        Me.lblInstructions.AutoSize = True
        Me.lblInstructions.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblInstructions.Location = New System.Drawing.Point(3, 0)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Size = New System.Drawing.Size(794, 13)
        Me.lblInstructions.TabIndex = 32
        Me.lblInstructions.Text = "HPT Editor > Edit > Virtual Volumetric Efficiency > Set  Cam Angle to 20º > Copy " &
    "with axis > Paste Here"
        '
        'tsBtnCopy
        '
        Me.tsBtnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnCopy.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsMnuCopy, Me.tsMnuCopyWithAxis})
        Me.tsBtnCopy.Image = CType(resources.GetObject("tsBtnCopy.Image"), System.Drawing.Image)
        Me.tsBtnCopy.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnCopy.Name = "tsBtnCopy"
        Me.tsBtnCopy.Size = New System.Drawing.Size(52, 47)
        Me.tsBtnCopy.Text = "ToolStripSplitButton1"
        '
        'tsMnuCopy
        '
        Me.tsMnuCopy.Name = "tsMnuCopy"
        Me.tsMnuCopy.Size = New System.Drawing.Size(182, 26)
        Me.tsMnuCopy.Text = "Copy"
        '
        'tsMnuCopyWithAxis
        '
        Me.tsMnuCopyWithAxis.Name = "tsMnuCopyWithAxis"
        Me.tsMnuCopyWithAxis.Size = New System.Drawing.Size(182, 26)
        Me.tsMnuCopyWithAxis.Text = "Copy with Axis"
        '
        'frmCamImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.TableLayoutPanel3)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmCamImport"
        Me.Text = "Variable Cam Import Wizard"
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.ts.ResumeLayout(False)
        Me.ts.PerformLayout()
        CType(Me.grdVVECam, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel3 As TableLayoutPanel
    Friend WithEvents ts As ToolStrip
    Friend WithEvents tsBtnPaste As ToolStripButton
    Friend WithEvents ToolStripSeparator14 As ToolStripSeparator
    Friend WithEvents tsBtnClear As ToolStripButton
    Friend WithEvents grdVVECam As DataGridView
    Friend WithEvents lblInstructions As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents tsBtnSaveExit As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsBtnCancel As ToolStripButton
    Friend WithEvents tsBtnCopy As ToolStripSplitButton
    Friend WithEvents tsMnuCopy As ToolStripMenuItem
    Friend WithEvents tsMnuCopyWithAxis As ToolStripMenuItem
End Class
