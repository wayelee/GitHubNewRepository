
Imports System.Drawing
Imports System.Windows.Forms


Namespace Rendering



    Public Class FormSymbolSelect

        Private m_strServerStyleFile As String

        Private m_pSymbolHandle As EsriSymbolHandler
        Private m_pCurSelSymbolIndex As Integer = -1
        Private m_pCurSelBitmapSymbol As Bitmap

        Public ReadOnly Property CurrentSymbol() As Bitmap
            Get
                Return m_pCurSelBitmapSymbol
            End Get

        End Property

        Public Sub New(Optional ByVal styleFilePath As String = "")

            InitializeComponent()


            m_pSymbolHandle = New EsriSymbolHandler(styleFilePath)
            Initialization()

            If styleFilePath = "" Then
                m_strServerStyleFile = m_pSymbolHandle.DefaultStyleFileName
            Else
                m_strServerStyleFile = styleFilePath
            End If
            txtStyle.Text = m_strServerStyleFile

        End Sub


        Private Sub Initialization()

            LayeroutControls()

            PopulateCategories(cbxCategory)

            If cbxCategory.Items.Count > 0 Then
                cbxCategory.SelectedIndex = 0
            End If

        End Sub


        Private Sub PopulateCategories(ByVal comBox As ComboBox, Optional ByVal className As String = "Marker Symbols")

            comBox.Items.Clear()
            Dim lstCategories As List(Of String) = m_pSymbolHandle.GetCategories(className)
            For Each Name As String In lstCategories
                comBox.Items.Add(Name)
            Next

        End Sub

        Private Sub LayeroutControls()

            Label1.Text = "Color"
            Label2.Text = "Size"
            Label3.Text = "Angle"
            'Picture2.Top = Label1.Top

            Picture2.Visible = True
            Picture3.Visible = False

            Text1.Top = Label2.Top
            Text2.Top = Label3.Top
            Text1.Visible = True
            Text2.Visible = True
        End Sub




        Private Sub cbxCategory_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbxCategory.SelectedIndexChanged

            Dim selIndex As Integer = cbxCategory.SelectedIndex
            Dim selName As String = cbxCategory.SelectedText

            m_pSymbolHandle.PopulateSymbolsToListView(selName, ListView1)
            InitialSelectListView()

        End Sub


        Private Sub InitialSelectListView()

            ListView1.Select()
            If ListView1.Items.Count > 0 Then

                ListView1.Items(0).Selected = True
                HandleListviewClick(0)
                m_pCurSelSymbolIndex = 0

            End If

        End Sub

        Private Sub HandleListviewClick(ByVal indexSelected As Integer)

            Dim inBitmapProperty As MarkerSymbolProperty = New MarkerSymbolProperty()
            inBitmapProperty.Size = 18
            Dim outBitmapProperty As MarkerSymbolProperty = New MarkerSymbolProperty()
            Dim imgSymbol As Bitmap = m_pSymbolHandle.GetDefaultBitmapOfMarkerSymbol(indexSelected, outBitmapProperty)

            PreviewSymbol(imgSymbol)
            PopulateSymbolProperty(outBitmapProperty)
            RefreshPreview()

        End Sub

        Private Sub PreviewSymbol(ByVal imgSymbol As Bitmap)

            Picture1.Image = imgSymbol
            m_pCurSelBitmapSymbol = imgSymbol

        End Sub


        Private Sub PopulateSymbolProperty(ByVal bitmapProperty As MarkerSymbolProperty)

            Text1.Text = bitmapProperty.Size
            Text2.Text = bitmapProperty.Angle
            Picture2.BackColor = bitmapProperty.Color

        End Sub


        Private Function ReadSymbolProperty() As MarkerSymbolProperty

            Dim bitmapProperty As MarkerSymbolProperty = New MarkerSymbolProperty()
            bitmapProperty.Size = Text1.Value
            bitmapProperty.Angle = Text2.Value
            bitmapProperty.Color = Picture2.BackColor
            Return bitmapProperty

        End Function

        Private Sub Text1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Text1.ValueChanged
            If m_pCurSelSymbolIndex > -1 Then
                RefreshPreview()
            End If
        End Sub


        Private Sub Text2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Text2.ValueChanged
            If m_pCurSelSymbolIndex > -1 Then
                RefreshPreview()
            End If
        End Sub


        Private Sub RefreshPreview()

            Dim inBitmapProperty As MarkerSymbolProperty = ReadSymbolProperty()
            Dim outBitmapProperty As MarkerSymbolProperty = New MarkerSymbolProperty()
            Dim imgSymbol As Bitmap = m_pSymbolHandle.GetCustomBitmapOfMarkerSymbolItem(m_pCurSelSymbolIndex, inBitmapProperty)

            PreviewSymbol(imgSymbol)

        End Sub


        Private Sub ListView1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseClick
            m_pCurSelSymbolIndex = ListView1.FocusedItem.Index
            HandleListviewClick(m_pCurSelSymbolIndex)
        End Sub


        Private Sub Picture2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Picture2.Click
            ColorDialog1.ShowDialog()
            Picture2.BackColor = ColorDialog1.Color
            If m_pCurSelSymbolIndex > -1 Then
                RefreshPreview()
            End If
        End Sub

        Private Sub btOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btOpen.Click
            Dim oDlg As System.Windows.Forms.OpenFileDialog = New OpenFileDialog()
            oDlg.Filter = "ESRI ServerStyle(*.ServerStyle)|*.ServerStyle"
            If oDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                If Not m_strServerStyleFile = oDlg.FileName Then
                    m_strServerStyleFile = oDlg.FileName
                    m_pSymbolHandle = New EsriSymbolHandler(m_strServerStyleFile)
                    txtStyle.Text = m_strServerStyleFile
                    Initialization()

                Else
                    Exit Sub
                End If

            Else
                Exit Sub
            End If

        End Sub
    End Class

End Namespace