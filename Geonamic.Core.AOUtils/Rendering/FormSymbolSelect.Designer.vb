Namespace Rendering



    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class FormSymbolSelect
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
            Me.Text1 = New System.Windows.Forms.NumericUpDown
            Me.Label4 = New System.Windows.Forms.Label
            Me.Text2 = New System.Windows.Forms.NumericUpDown
            Me.Picture3 = New System.Windows.Forms.PictureBox
            Me.Picture2 = New System.Windows.Forms.PictureBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.cbxCategory = New System.Windows.Forms.ComboBox
            Me.btOpen = New System.Windows.Forms.Button
            Me.txtStyle = New System.Windows.Forms.TextBox
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.Picture1 = New System.Windows.Forms.PictureBox
            Me.ColorDialog1 = New System.Windows.Forms.ColorDialog
            Me.btnCancel = New System.Windows.Forms.Button
            Me.btnOK = New System.Windows.Forms.Button
            Me.ListView1 = New System.Windows.Forms.ListView
            CType(Me.Text1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.Text2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.Picture3, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.Picture2, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            CType(Me.Picture1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'Text1
            '
            Me.Text1.DecimalPlaces = 1
            Me.Text1.Location = New System.Drawing.Point(86, 0)
            Me.Text1.Name = "Text1"
            Me.Text1.Size = New System.Drawing.Size(75, 20)
            Me.Text1.TabIndex = 105
            Me.Text1.Value = New Decimal(New Integer() {18, 0, 0, 0})
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(380, 19)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(52, 13)
            Me.Label4.TabIndex = 115
            Me.Label4.Text = "Category:"
            '
            'Text2
            '
            Me.Text2.Location = New System.Drawing.Point(86, 18)
            Me.Text2.Maximum = New Decimal(New Integer() {360, 0, 0, 0})
            Me.Text2.Minimum = New Decimal(New Integer() {360, 0, 0, -2147483648})
            Me.Text2.Name = "Text2"
            Me.Text2.Size = New System.Drawing.Size(75, 20)
            Me.Text2.TabIndex = 104
            '
            'Picture3
            '
            Me.Picture3.BackColor = System.Drawing.SystemColors.Control
            Me.Picture3.Location = New System.Drawing.Point(86, 43)
            Me.Picture3.Name = "Picture3"
            Me.Picture3.Size = New System.Drawing.Size(74, 31)
            Me.Picture3.TabIndex = 102
            Me.Picture3.TabStop = False
            '
            'Picture2
            '
            Me.Picture2.BackColor = System.Drawing.SystemColors.Control
            Me.Picture2.Location = New System.Drawing.Point(86, 80)
            Me.Picture2.Name = "Picture2"
            Me.Picture2.Size = New System.Drawing.Size(75, 30)
            Me.Picture2.TabIndex = 101
            Me.Picture2.TabStop = False
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(26, 48)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(25, 13)
            Me.Label3.TabIndex = 100
            Me.Label3.Text = "size"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Text1)
            Me.GroupBox2.Controls.Add(Me.Text2)
            Me.GroupBox2.Controls.Add(Me.Picture3)
            Me.GroupBox2.Controls.Add(Me.Picture2)
            Me.GroupBox2.Controls.Add(Me.Label3)
            Me.GroupBox2.Controls.Add(Me.Label2)
            Me.GroupBox2.Controls.Add(Me.Label1)
            Me.GroupBox2.Location = New System.Drawing.Point(381, 224)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(194, 128)
            Me.GroupBox2.TabIndex = 114
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Options"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(26, 20)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(25, 13)
            Me.Label2.TabIndex = 99
            Me.Label2.Text = "size"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(26, 89)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(25, 13)
            Me.Label1.TabIndex = 98
            Me.Label1.Text = "size"
            '
            'cbxCategory
            '
            Me.cbxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbxCategory.FormattingEnabled = True
            Me.cbxCategory.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.cbxCategory.Location = New System.Drawing.Point(438, 16)
            Me.cbxCategory.Name = "cbxCategory"
            Me.cbxCategory.Size = New System.Drawing.Size(137, 21)
            Me.cbxCategory.TabIndex = 113
            '
            'btOpen
            '
            Me.btOpen.Location = New System.Drawing.Point(288, 14)
            Me.btOpen.Name = "btOpen"
            Me.btOpen.Size = New System.Drawing.Size(69, 25)
            Me.btOpen.TabIndex = 112
            Me.btOpen.Text = "Style File..."
            Me.btOpen.UseVisualStyleBackColor = True
            '
            'txtStyle
            '
            Me.txtStyle.Location = New System.Drawing.Point(28, 17)
            Me.txtStyle.Name = "txtStyle"
            Me.txtStyle.Size = New System.Drawing.Size(254, 20)
            Me.txtStyle.TabIndex = 111
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Picture1)
            Me.GroupBox1.Location = New System.Drawing.Point(375, 52)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(200, 149)
            Me.GroupBox1.TabIndex = 110
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Preview"
            '
            'Picture1
            '
            Me.Picture1.BackColor = System.Drawing.SystemColors.ButtonHighlight
            Me.Picture1.Location = New System.Drawing.Point(6, 14)
            Me.Picture1.Name = "Picture1"
            Me.Picture1.Size = New System.Drawing.Size(188, 129)
            Me.Picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            Me.Picture1.TabIndex = 8
            Me.Picture1.TabStop = False
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(494, 454)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 25)
            Me.btnCancel.TabIndex = 109
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnOK
            '
            Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOK.Location = New System.Drawing.Point(393, 454)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(75, 25)
            Me.btnOK.TabIndex = 108
            Me.btnOK.Text = "OK"
            Me.btnOK.UseVisualStyleBackColor = True
            '
            'ListView1
            '
            Me.ListView1.Location = New System.Drawing.Point(28, 52)
            Me.ListView1.Name = "ListView1"
            Me.ListView1.Size = New System.Drawing.Size(329, 432)
            Me.ListView1.TabIndex = 107
            Me.ListView1.UseCompatibleStateImageBehavior = False
            '
            'FormSymbolSelect
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(602, 499)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.cbxCategory)
            Me.Controls.Add(Me.btOpen)
            Me.Controls.Add(Me.txtStyle)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.Controls.Add(Me.ListView1)
            Me.Name = "FormSymbolSelect"
            Me.Text = "FormSymbolSelect"
            CType(Me.Text1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.Text2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.Picture3, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.Picture2, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            CType(Me.Picture1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents Text1 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Text2 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Picture3 As System.Windows.Forms.PictureBox
        Friend WithEvents Picture2 As System.Windows.Forms.PictureBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents cbxCategory As System.Windows.Forms.ComboBox
        Friend WithEvents btOpen As System.Windows.Forms.Button
        Friend WithEvents txtStyle As System.Windows.Forms.TextBox
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents Picture1 As System.Windows.Forms.PictureBox
        Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnOK As System.Windows.Forms.Button
        Friend WithEvents ListView1 As System.Windows.Forms.ListView
    End Class


End Namespace

