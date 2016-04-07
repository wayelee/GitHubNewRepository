
Imports System.Drawing
Imports System.Windows.Forms


Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry

Namespace Rendering


    Public Enum SymbolTypeEnum

        Point
        Polyline
        Polygon

        Unknown

    End Enum




    Public Class BitmapSymbolInfo

        Private _BitmapSymbol As Bitmap
        Public Property BitmapSymbol() As Bitmap
            Get
                Return _BitmapSymbol
            End Get
            Set(ByVal value As Bitmap)
                _BitmapSymbol = value
            End Set
        End Property


        Private _Destription As String = ""
        Public Property Description() As String
            Get
                Return _Destription
            End Get
            Set(ByVal value As String)
                _Destription = value
            End Set
        End Property

        Private _SymbolType As SymbolTypeEnum
        Public Property SymbolType() As SymbolTypeEnum
            Get
                Return _SymbolType
            End Get
            Set(ByVal value As SymbolTypeEnum)
                _SymbolType = value
            End Set
        End Property


    End Class


    Public Class SymbolExporter

        Private _Size As Integer = 32
        Public Property BitMapSize() As Integer
            Get
                Return _Size
            End Get
            Set(ByVal value As Integer)
                _Size = value
            End Set
        End Property

        Public Sub New()


        End Sub


#Region "Export From Map"




        Public Function ExporterMap(ByVal map As IMap) As List(Of BitmapSymbolInfo)

            Dim result As List(Of BitmapSymbolInfo) = New List(Of BitmapSymbolInfo)()

            Dim featureLyrs As List(Of IFeatureLayer) = GetAllFeatureLayers(map)
            For Each featLyr As IFeatureLayer In featureLyrs

                Dim sym As ISymbol = GetSymbolFromLyr(featLyr)
                If sym IsNot Nothing Then

                    Dim symType As String = GetSymbolType(sym)
                    Dim img As Bitmap = EsriSymbolHandler.CreateBitmap(sym, symType, _Size)

                    Dim symInfo As BitmapSymbolInfo = New BitmapSymbolInfo()
                    symInfo.BitmapSymbol = img
                    symInfo.Description = featLyr.Name
                    symInfo.SymbolType = GetSymbolType(symType)

                    result.Add(symInfo)

                End If
            Next

            Return result
        End Function


        Private Function GetSymbolType(ByVal symStr As String) As SymbolTypeEnum

            Dim result As SymbolTypeEnum
            Select Case symStr

                Case "Marker Symbols"
                    result = SymbolTypeEnum.Point
                Case "Line Symbols"
                    result = SymbolTypeEnum.Polyline
                Case "Fill Symbols"
                    result = SymbolTypeEnum.Polygon
            End Select

            Return result

        End Function

        Private Function GetSymbolType(ByVal sym As ISymbol) As String

            Dim symType As String = "Marker Symbols"
            If TypeOf sym Is ArrowMarkerSymbol Or TypeOf (sym) Is CharacterMarkerSymbol Or TypeOf (sym) Is SimpleMarkerSymbol _
                Or TypeOf (sym) Is PictureMarkerSymbol Then
                symType = "Marker Symbols"

            ElseIf TypeOf sym Is HashLineSymbol Or TypeOf sym Is LineFillSymbol Or TypeOf sym Is MarkerLineSymbol _
                    Or TypeOf sym Is MultiLayerLineSymbol Or TypeOf sym Is SimpleLineSymbol Then

                symType = "Line Symbols"


            ElseIf TypeOf sym Is DotDensityFillSymbol Or TypeOf sym Is GradientFillSymbol Or TypeOf sym Is LineFillSymbol _
                    Or TypeOf sym Is MarkerFillSymbol Or TypeOf sym Is MultiLayerFillSymbol Or TypeOf sym Is PictureFillSymbol _
                    Or TypeOf sym Is SimpleFillSymbol Or TypeOf sym Is SimpleFillSymbol Then
                symType = "Fill Symbols"
            End If

            Return symType

        End Function

        Private Function GetSymbolFromLyr(ByVal featLyr As IFeatureLayer) As ISymbol

            Dim geoLyr As IGeoFeatureLayer = featLyr
            Dim featRender As IFeatureRenderer = geoLyr.Renderer

            If TypeOf (featRender) Is SimpleRenderer Then

                Dim simpleRender As ISimpleRenderer = CType(featRender, ISimpleRenderer)
                Return simpleRender.Symbol

            Else
                Return Nothing

            End If

        End Function


        Private Function GetAllFeatureLayers(ByVal map As IMap) As List(Of IFeatureLayer)

            Dim result As List(Of IFeatureLayer) = New List(Of IFeatureLayer)

            Dim lyr As ILayer
            For index As Integer = 0 To map.LayerCount - 1
                lyr = map.Layer(index)
                If TypeOf (lyr) Is IFeatureLayer Then
                    result.Add(lyr)
                ElseIf TypeOf (lyr) Is IGroupLayer Then
                    result.AddRange(GetFeatureLayers(lyr))
                End If

            Next

            Return result
        End Function

        Private Function GetFeatureLayers(ByVal groupLyr As IGroupLayer) As List(Of IFeatureLayer)

            Dim comLyr As ICompositeLayer = groupLyr
            Dim result As List(Of IFeatureLayer) = New List(Of IFeatureLayer)

            Dim lyr As ILayer
            For index As Integer = 0 To comLyr.Count - 1
                lyr = comLyr.Layer(index)
                If TypeOf (lyr) Is IFeatureLayer Then
                    result.Add(lyr)
                ElseIf TypeOf (lyr) Is IGroupLayer Then
                    result.AddRange(GetFeatureLayers(lyr))
                End If

            Next

            Return result

        End Function

#End Region




        '#Region "Export From ServerStyleFile"

        '        Public Function ExporterServerStyleFile(ByVal file As String) As List(Of BitmapSymbolInfo)


        '        End Function



        '#End Region



    End Class


End Namespace

