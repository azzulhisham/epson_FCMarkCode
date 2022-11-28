<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_Support
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_Support))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lbl_MsgBox = New System.Windows.Forms.Label
        Me.txt_KeyIn = New System.Windows.Forms.TextBox
        Me.lbl_Progress = New System.Windows.Forms.Label
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Location = New System.Drawing.Point(-8, -2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(100, 106)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'lbl_MsgBox
        '
        Me.lbl_MsgBox.AutoSize = True
        Me.lbl_MsgBox.Location = New System.Drawing.Point(94, 32)
        Me.lbl_MsgBox.Name = "lbl_MsgBox"
        Me.lbl_MsgBox.Size = New System.Drawing.Size(48, 14)
        Me.lbl_MsgBox.TabIndex = 1
        Me.lbl_MsgBox.Text = "MsgBox"
        '
        'txt_KeyIn
        '
        Me.txt_KeyIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txt_KeyIn.Location = New System.Drawing.Point(95, 49)
        Me.txt_KeyIn.Name = "txt_KeyIn"
        Me.txt_KeyIn.Size = New System.Drawing.Size(222, 22)
        Me.txt_KeyIn.TabIndex = 2
        '
        'lbl_Progress
        '
        Me.lbl_Progress.AutoSize = True
        Me.lbl_Progress.Location = New System.Drawing.Point(94, 52)
        Me.lbl_Progress.Name = "lbl_Progress"
        Me.lbl_Progress.Size = New System.Drawing.Size(48, 14)
        Me.lbl_Progress.TabIndex = 3
        Me.lbl_Progress.Text = "MsgBox"
        Me.lbl_Progress.Visible = False
        '
        'frm_Support
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(349, 92)
        Me.ControlBox = False
        Me.Controls.Add(Me.lbl_Progress)
        Me.Controls.Add(Me.txt_KeyIn)
        Me.Controls.Add(Me.lbl_MsgBox)
        Me.Controls.Add(Me.PictureBox1)
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frm_Support"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lbl_MsgBox As System.Windows.Forms.Label
    Friend WithEvents txt_KeyIn As System.Windows.Forms.TextBox
    Friend WithEvents lbl_Progress As System.Windows.Forms.Label
End Class
