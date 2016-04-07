

Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry

Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms


Namespace Rendering



    Public Structure MarkerSymbolProperty

        Public Size As Integer
        Public Angle As Integer
        Public Color As System.Drawing.Color

    End Structure

    Public Class EsriCharacterSymbolProperty

        Public GDIColor As System.Drawing.Color
        Public Size As Integer
        Public Angle As Double
        Public XOffset As Double
        Public YOffset As Double

        Public CharacterIndex As Integer
        Public COMFont As stdole.IFontDisp


    End Class


    Public Class EsriSymbolHandler


        Private m_strServerStyleFile As String
        Private m_pStyleGallery As ServerStyleGallery

        Private m_strStyleClassName As String = "Marker Symbols"
        'Private m_pSymbol As ISymbol

        Private m_arrayItems As List(Of IStyleGalleryItem)


        Public ReadOnly Property DefaultStyleFileName() As String
            Get
                Return m_strServerStyleFile
            End Get
        End Property

        Public Sub New(ByVal styleFileName As String)

            m_pStyleGallery = New ServerStyleGallery()
            Dim pStyleGalleryStorage As ESRI.ArcGIS.Display.IStyleGalleryStorage = m_pStyleGallery
            If styleFileName = "" Then
                m_strServerStyleFile = pStyleGalleryStorage.DefaultStylePath & "ESRI.ServerStyle"
            Else
                m_strServerStyleFile = styleFileName
            End If

            pStyleGalleryStorage.TargetFile = m_strServerStyleFile

        End Sub

        Public Shared Function GetDefaultStylePath() As String
            Dim serverG As ServerStyleGallery = New ServerStyleGallery()
            Dim pStyleGalleryStorage As ESRI.ArcGIS.Display.IStyleGalleryStorage = serverG
            Return pStyleGalleryStorage.DefaultStylePath
        End Function


        Public Function GetCategories(Optional ByVal className As String = "Marker Symbols") As List(Of String)
            Dim nameList As List(Of String) = New List(Of String)

            Dim categories As IEnumBSTR = m_pStyleGallery.Categories(className)
            categories.Reset()

            Dim category As String = categories.Next()
            Do While Not category Is Nothing
                nameList.Add(category)
                category = categories.Next()
            Loop

            Return nameList

        End Function

        Public Sub PopulateSymbolsToListView(ByVal categoryName As String, ByVal lstView As ListView, Optional ByVal className As String = "Marker Symbols")

            Dim Largeimage As System.Windows.Forms.ImageList = New ImageList()
            Dim bmpB As System.Drawing.Bitmap
            Dim lvItem As System.Windows.Forms.ListViewItem


            lstView.Items.Clear()
            lstView.Columns.Clear()
            If Not m_arrayItems Is Nothing Then
                m_arrayItems.Clear()
            End If

            Largeimage.ImageSize = New Size(32, 32)
            lstView.LargeImageList = Largeimage

            lstView.Columns.Add("Name", 180, System.Windows.Forms.HorizontalAlignment.Left)
            lstView.Columns.Add("Index", 50, System.Windows.Forms.HorizontalAlignment.Left)
            lstView.Columns.Add("Category", 120, System.Windows.Forms.HorizontalAlignment.Left)


            Dim imageIndex As Integer = 0
            m_arrayItems = GetStyleGalleryItemList(categoryName, className)
            For Each pStyleItem As IStyleGalleryItem In m_arrayItems

                bmpB = CreateBitmap(pStyleItem.Item, m_strStyleClassName, 32)
                If Not bmpB Is Nothing Then

                    Largeimage.Images.Add(bmpB)
                    lvItem = New ListViewItem(New String() {pStyleItem.Name, pStyleItem.ID.ToString(), pStyleItem.Category}, imageIndex)
                    lstView.Items.Add(lvItem)

                End If

                imageIndex += 1
            Next


        End Sub


        Public Function GetStyleGalleryItemList(ByVal categoryName As String, Optional ByVal className As String = "Marker Symbols") As List(Of IStyleGalleryItem)

            Dim arrayItems As List(Of IStyleGalleryItem) = New List(Of IStyleGalleryItem)()

            Try


                'Dim pStyleGallery As IStyleGallery = m_pStyleGallery
                ''pStyleGallery.Clear()

                'm_pStyleGallery.Clear()

                Dim pEnumSGItem As IEnumStyleGalleryItem
                pEnumSGItem = m_pStyleGallery.Items(className, m_strServerStyleFile, "")
                'pEnumSGItem.Reset()

                Dim pStyleItem As IStyleGalleryItem = pEnumSGItem.Next()
                While Not pStyleItem Is Nothing

                    arrayItems.Add(pStyleItem)
                    pStyleItem = pEnumSGItem.Next()

                End While
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumSGItem)

                Return arrayItems
            Catch ex As Exception
                MsgBox(ex.Message)

            End Try
            Return arrayItems

        End Function



#Region "Get bitmap from StyeleGalleryItem by ISymbol"


        Public Function GetDefaultBitmapOfMarkerSymbol(ByVal selItemIndex As Integer, ByRef outSymbolProperty As MarkerSymbolProperty) As Bitmap
            Dim selStyleGalleryItem As IStyleGalleryItem = m_arrayItems(selItemIndex)

            Dim pSymbol As ISymbol
            Dim pMarkerSymbol As IMarkerSymbol
            Dim pColor As ESRI.ArcGIS.Display.IColor
            pMarkerSymbol = selStyleGalleryItem.Item

            outSymbolProperty.Size = pMarkerSymbol.Size
            outSymbolProperty.Angle = pMarkerSymbol.Angle
            pColor = pMarkerSymbol.Color
            outSymbolProperty.Color = System.Drawing.ColorTranslator.FromOle(pColor.RGB)

            pSymbol = pMarkerSymbol

            Dim imgSize As Integer = SymbolSizeToBitmapSize(-1)
            Dim img As Bitmap = CreateBitmap(pSymbol, m_strStyleClassName, imgSize)

            'For test of using Font
            'img = ConvertMarksymbol(pSymbol, m_strStyleClassName, imgSize)

            Return img
        End Function

        Public Function GetCustomBitmapOfMarkerSymbolItem(ByVal selItemIndex As Integer, ByVal inSymbolProperty As MarkerSymbolProperty) As Bitmap


            Dim selStyleGalleryItem As IStyleGalleryItem = m_arrayItems(selItemIndex)

            Dim pSymbol As ISymbol
            Dim pRealSymbol As ISymbol
            Dim pMarkerSymbol As IMarkerSymbol

            pRealSymbol = selStyleGalleryItem.Item
            pSymbol = Clone(pRealSymbol)
            pMarkerSymbol = pSymbol
            pMarkerSymbol.Size = inSymbolProperty.Size
            pMarkerSymbol.Angle = inSymbolProperty.Angle
            Dim pColor As IColor
            pColor = New RgbColor()
            pColor.RGB = System.Drawing.ColorTranslator.ToOle(inSymbolProperty.Color)
            pMarkerSymbol.Color = pColor
            pSymbol = pMarkerSymbol


            Dim imgSize As Integer = SymbolSizeToBitmapSize(inSymbolProperty.Size)
            Dim img As Bitmap = CreateBitmap(pSymbol, m_strStyleClassName, imgSize)


            ''For test convert symbol by Font
            'img = ConvertMarksymbol(pSymbol, m_strStyleClassName, imgSize)


            Return img

        End Function

        Private Function Clone(ByVal pClone As IClone) As IClone
            Clone = pClone.Clone
        End Function


        Private Function SymbolSizeToBitmapSize(ByVal symbolSize) As Integer
            If symbolSize > 0 Then
                Return symbolSize * 24 / 18
            Else
                Return 24
            End If

        End Function


        Public Function ConvertMarksymbol(ByVal pSymbol As ISymbol, ByVal className As String, ByVal BMPSize As Integer) As Bitmap

            Try

                Dim pMarkerSymbol As IMarkerSymbol = pSymbol
                Dim pMLMSym As IMultiLayerMarkerSymbol
                Dim pCMSym As ICharacterMarkerSymbol

                If TypeOf (pMarkerSymbol) Is IMultiLayerMarkerSymbol Then
                    pMLMSym = CType(pMarkerSymbol, IMultiLayerMarkerSymbol)

                    For i As Integer = 0 To pMLMSym.LayerCount - 1
                        If TypeOf pMLMSym.Layer(i) Is ICharacterMarkerSymbol Then

                            pCMSym = CType(pMLMSym.Layer(i), ICharacterMarkerSymbol)

                            Dim symbolProperty As EsriCharacterSymbolProperty = New EsriCharacterSymbolProperty()

                            symbolProperty.CharacterIndex = pCMSym.CharacterIndex

                            Dim rgbColor As IRgbColor = pCMSym.Color
                            Dim gdiColor As System.Drawing.Color = System.Drawing.Color.FromArgb(rgbColor.Red, rgbColor.Green, rgbColor.Blue)
                            symbolProperty.GDIColor = gdiColor

                            symbolProperty.Size = pCMSym.Size
                            symbolProperty.Angle = pCMSym.Angle
                            symbolProperty.XOffset = pCMSym.XOffset
                            symbolProperty.YOffset = pCMSym.YOffset
                            symbolProperty.COMFont = pCMSym.Font

                            Return DrawBitmapUsingFont(symbolProperty, BMPSize)
                        End If

                    Next


                End If

            Catch ex As Exception

            End Try
            Return Nothing

        End Function

        Public Function DrawBitmapUsingFont(ByVal symbolProp As EsriCharacterSymbolProperty, ByVal BMPSize As Integer) As Bitmap

            If symbolProp Is Nothing Then
                Return Nothing

            End If



            Dim gdiFont As Font = GetFontFromIFontDisp(symbolProp.COMFont)
            Dim img As Image = New Bitmap(BMPSize, BMPSize)

            Dim x, y As Integer
            x = 0
            y = 0

            Dim txt As String = ChrW(symbolProp.CharacterIndex)
            Dim b As System.Drawing.Brush = New System.Drawing.SolidBrush(symbolProp.GDIColor)

            Using gOfImg As Graphics = Graphics.FromImage(img)
                gOfImg.DrawString(txt, gdiFont, b, x, y)
            End Using

            Return img

        End Function

        ''' <summary>
        ''' 从IFontDisp转换到System.Drawing.Font
        ''' </summary>
        ''' <param name="pFontDisp">传入IFontDisp</param>
        ''' <returns>返回Font</returns>
        ''' <remarks></remarks>
        Shared Function GetFontFromIFontDisp(ByVal pFontDisp As stdole.IFontDisp) As System.Drawing.Font
            Dim pName As String
            Dim pSize As Single
            Dim pFontStyle As System.Drawing.FontStyle
            Dim pFont As System.Drawing.Font
            pName = pFontDisp.Name
            pSize = pFontDisp.Size
            If pFontDisp.Bold = True Then
                pFontStyle = pFontStyle + System.Drawing.FontStyle.Bold
            End If
            If pFontDisp.Italic = True Then
                pFontStyle = pFontStyle + System.Drawing.FontStyle.Italic
            End If
            If pFontDisp.Strikethrough = True Then
                pFontStyle = pFontStyle + System.Drawing.FontStyle.Strikeout
            End If
            If pFontDisp.Underline = True Then
                pFontStyle = pFontStyle + System.Drawing.FontStyle.Underline
            End If
            pFont = New System.Drawing.Font(pName, pSize, pFontStyle)
            Return pFont
        End Function



        Public Shared Function CreateBitmap(ByVal pSymbol As ISymbol, ByVal className As String, ByVal BMPSize As Integer) As System.Drawing.Bitmap

            Dim Bmp As System.Drawing.Bitmap = Nothing
            Dim Dpi As Double
            Dim pEnvelope As IEnvelope
            Dim pDeviceRect As tagRECT
            Dim pDisplayTransformation As IDisplayTransformation
            Dim pFromPoint As IPoint
            Dim pToPoint As IPoint
            Dim pGeometry As IGeometry = Nothing

            If Bmp Is Nothing Then
                Bmp = New System.Drawing.Bitmap(BMPSize, BMPSize)
            End If

            Try


                '把图片格式定义为位图
                Dim gImage As System.Drawing.Graphics
                gImage = System.Drawing.Graphics.FromImage(Bmp)
                gImage.Clear(System.Drawing.Color.Transparent)
                Dpi = gImage.DpiX
                pEnvelope = New Envelope()
                pEnvelope.PutCoords(0, 0, Bmp.Width, Bmp.Height)

                '设置位图的边界
                pDeviceRect.bottom = 0
                pDeviceRect.left = 0
                pDeviceRect.top = Bmp.Height
                pDeviceRect.right = Bmp.Width
                '设置位图的设备环境
                pDisplayTransformation = New DisplayTransformation()

                pDisplayTransformation.VisibleBounds = pEnvelope
                pDisplayTransformation.Bounds = pEnvelope        ' 边界
                pDisplayTransformation.DeviceFrame = pDeviceRect        '设备坐标的可见范围
                pDisplayTransformation.Resolution = Dpi         '每一个英寸有多少个像素点

                pFromPoint = New ESRI.ArcGIS.Geometry.Point()
                pToPoint = New ESRI.ArcGIS.Geometry.Point()


                '判断Symbol的类型,根据类型定义Symbol在位图中的大小和位置
                '注意屏幕坐标X为正值,Y坐标为负值
                Select Case className
                    Case "Marker Symbols"
                        pFromPoint.PutCoords(0.5 * Bmp.Width, -0.5 * Bmp.Height)
                        pGeometry = pFromPoint

                    Case "Line Symbols"
                        pFromPoint.PutCoords(0, -0.5 * Bmp.Height)
                        pToPoint.PutCoords(Bmp.Width, -0.5 * Bmp.Height)
                        Dim pPolyline As IPolyline
                        pPolyline = New Polyline
                        pPolyline.FromPoint = pFromPoint
                        pPolyline.ToPoint = pToPoint
                        pGeometry = pPolyline

                    Case "Fill Symbols"
                        Dim pEnvel As IEnvelope2
                        pEnvel = New Envelope
                        pEnvel.PutCoords(0, -0.8 * Bmp.Height, 0.8 * Bmp.Width, 0)
                        pEnvel.Expand(-1, -1, False)
                        Dim pSegColl As ISegmentCollection
                        pSegColl = New Polygon
                        pSegColl.SetRectangle(pEnvel)

                        pGeometry = pSegColl

                End Select

                '设置Symbol的属性,根据Symbol画位图
                Dim hdc As System.IntPtr
                hdc = New System.IntPtr
                hdc = gImage.GetHdc()
                Dim pSymbolBmp As ISymbol
                'Dim tmpSymbol As IFillProperties

                pSymbolBmp = pSymbol

                pSymbolBmp.SetupDC(hdc.ToInt32(), pDisplayTransformation)
                pSymbolBmp.Draw(pGeometry)

                pSymbolBmp.ResetDC()
                gImage.ReleaseHdc(hdc)
                gImage.Dispose()


            Catch ex As Exception

                MsgBox(ex.Message)

            End Try

            Return Bmp

        End Function



#End Region






    End Class


End Namespace

