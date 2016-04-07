
Namespace Carto


    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class SetSelectableLayersDialog
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
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.cklstLayers = New System.Windows.Forms.CheckedListBox
            Me.btnSelectAll = New System.Windows.Forms.Button
            Me.btnClearAll = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.ColumnCount = 2
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.17219!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.82781!))
            Me.TableLayoutPanel1.Controls.Add(Me.cklstLayers, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.btnSelectAll, 1, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.btnClearAll, 1, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 5)
            Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(12, 12)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 7
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(411, 190)
            Me.TableLayoutPanel1.TabIndex = 0
            '
            'cklstLayers
            '
            Me.cklstLayers.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cklstLayers.CheckOnClick = True
            Me.cklstLayers.FormattingEnabled = True
            Me.cklstLayers.Location = New System.Drawing.Point(3, 33)
            Me.cklstLayers.Name = "cklstLayers"
            Me.TableLayoutPanel1.SetRowSpan(Me.cklstLayers, 5)
            Me.cklstLayers.Size = New System.Drawing.Size(298, 139)
            Me.cklstLayers.TabIndex = 2
            '
            'btnSelectAll
            '
            Me.btnSelectAll.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.btnSelectAll.Location = New System.Drawing.Point(320, 33)
            Me.btnSelectAll.Name = "btnSelectAll"
            Me.btnSelectAll.Size = New System.Drawing.Size(75, 23)
            Me.btnSelectAll.TabIndex = 3
            Me.btnSelectAll.Text = "Select All"
            Me.btnSelectAll.UseVisualStyleBackColor = True
            '
            'btnClearAll
            '
            Me.btnClearAll.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.btnClearAll.Location = New System.Drawing.Point(320, 63)
            Me.btnClearAll.Name = "btnClearAll"
            Me.btnClearAll.Size = New System.Drawing.Size(75, 23)
            Me.btnClearAll.TabIndex = 4
            Me.btnClearAll.Text = "Clear All"
            Me.btnClearAll.UseVisualStyleBackColor = True
            '
            'Cancel_Button
            '
            Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Location = New System.Drawing.Point(320, 153)
            Me.Cancel_Button.Name = "Cancel_Button"
            Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
            Me.Cancel_Button.TabIndex = 1
            Me.Cancel_Button.Text = "Close"
            '
            'Label1
            '
            Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 2)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(298, 26)
            Me.Label1.TabIndex = 5
            Me.Label1.Text = "Choose which layers can have their features selected interactively with the Selec" & _
                "t Feature tool, Edit tool, etc."
            '
            'SetSelectableLayersDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ClientSize = New System.Drawing.Size(433, 211)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SetSelectableLayersDialog"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "SetSelectableLayersDialog"
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Cancel_Button As System.Windows.Forms.Button
        Friend WithEvents cklstLayers As System.Windows.Forms.CheckedListBox
        Friend WithEvents btnSelectAll As System.Windows.Forms.Button
        Friend WithEvents btnClearAll As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label

    End Class
End Namespace
