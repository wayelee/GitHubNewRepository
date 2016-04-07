

Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Carto
Imports System.Runtime.InteropServices


Namespace Location

    Public Class RouteLayerUtil

        Public Shared Function GetRouteFeatureLayers(ByVal map As IMap) As List(Of IFeatureLayer)

            Dim result As New List(Of IFeatureLayer)()
            Dim featLyrs As List(Of IFeatureLayer) = Carto.LayerUtil.GetFeatureLayers(map, esriGeometryType.esriGeometryPolyline)
            For Each lyr As IFeatureLayer In featLyrs

                If IsRouteLayer(lyr) Then
                    result.Add(lyr)
                End If

            Next

            Return result

        End Function


        Public Shared Function IsRouteLayer(ByVal lyr As IFeatureLayer) As Boolean

            If GetRouteLayerExtension(lyr) IsNot Nothing Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Shared Function GetRouteLayerExtension(ByVal lyr As IFeatureLayer) As IRouteLayerExtension

            Dim lyrExt As ILayerExtensions = lyr
            For index As Integer = 0 To lyrExt.ExtensionCount - 1


                If TypeOf lyrExt.Extension(index) Is ESRI.ArcGIS.Location.IRouteLayerExtension Then
                    Return lyrExt.Extension(index)
                End If

            Next

            Return Nothing

        End Function


        Public Shared Function GetRouteIDFieldName(ByVal routeLayer As IFeatureLayer) As String

            Dim lyrExt As IRouteLayerExtension = GetRouteLayerExtension(routeLayer)
            If lyrExt IsNot Nothing Then
                Return lyrExt.RouteIDFieldName
            Else
                Return String.Empty
            End If
         
        End Function


    End Class





End Namespace
