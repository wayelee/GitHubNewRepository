Imports System.Drawing

'Imports Geonamic.Core.AOUtils
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geodatabase

Namespace Geometry

    Public Class GeometryUtil



        Public Shared Function RotateRect(ByVal env As IEnvelope, ByVal rotate As Double) As IPolygon
            Dim polygon As IPolygon = Rect2Polygon(env)
            Dim area As IArea = polygon

            Dim trans As ITransform2D = polygon
            trans.Rotate(area.Centroid, (rotate * (Math.PI / 180)))

            Return polygon
        End Function


        Public Shared Function Rect2Polygon(ByVal env As IEnvelope) As IPolygon

            Dim polygon As IPolygon = New Polygon()
            Dim ptCol As IPointCollection = polygon
            ptCol.AddPoint(env.LowerLeft)
            ptCol.AddPoint(env.LowerRight)
            ptCol.AddPoint(env.UpperRight)
            ptCol.AddPoint(env.UpperLeft)
            ptCol.AddPoint(env.LowerLeft)
            polygon.SpatialReference = env.SpatialReference
            Return polygon

        End Function

        Public Shared Function GetRotatedClipBound(ByVal clipGraphicExtent As IPolygon, ByVal rotate As Double) As IEnvelope


            Dim clonedClipGraphicExtent As IPolygon = Others.CommonUtil.Clone(clipGraphicExtent)

            Dim area As IArea = clonedClipGraphicExtent
            Dim area2 As IArea = clipGraphicExtent

            Dim ptCol As IPointCollection = clonedClipGraphicExtent

            Dim center As IPoint
            If area.Area = 0 Then
                center = New ESRI.ArcGIS.Geometry.Point()
                center.X = (clonedClipGraphicExtent.Envelope.LowerLeft.X + clonedClipGraphicExtent.Envelope.UpperRight.X) / 2
                center.Y = (clonedClipGraphicExtent.Envelope.LowerLeft.Y + clonedClipGraphicExtent.Envelope.UpperRight.Y) / 2
            Else
                center = area.Centroid
            End If

            Dim trans As ITransform2D = clonedClipGraphicExtent
            trans.Rotate(center, (rotate * (Math.PI / 180)))

            Return clonedClipGraphicExtent.Envelope

        End Function


        Public Shared Function GetLength(ByVal pt1 As PointF, ByVal pt2 As PointF) As Double

            Return Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))

        End Function


        Public Shared Function GetLength(ByVal pt1 As IPoint, ByVal pt2 As IPoint) As Double

            Return Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))

        End Function

        Public Shared Function GetAngle(ByVal pt1 As IPoint, ByVal pt2 As IPoint) As Double

            Dim line As ILine = New Line()
            line.FromPoint = pt1
            line.ToPoint = pt2

            Return line.Angle

        End Function


        Public Shared Function CombinePolygons(ByVal polygons As IEnumGeometry) As IPolygon

            Dim topo As ITopologicalOperator = New Polygon()

            Try
                topo.ConstructUnion(polygons)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try


            Dim geom As IPolygon = CType(topo, IPolygon)
            Return geom

        End Function


        Public Shared Function CombinePolygon(ByRef polygon1 As IPolygon, ByRef polygon2 As IPolygon) As IPolygon

            'Build a new polygon by uniting two existing polygons.
            Try

                Dim topo As ITopologicalOperator = polygon1


                'Dim topologicalOperator2 As ITopologicalOperator5 = CType(polygon1, ITopologicalOperator5)
                ''Simplify.
                'topologicalOperator2.IsKnownSimple_2 = False
                'topologicalOperator2.Simplify()

                'Dim geometry As IGeometry = topologicalOperator2.Union(polygon2)

                topo.Simplify()
                Dim geometry As IGeometry = topo.Union(polygon2)
                Dim polygonCombined As IPolygon = CType(geometry, IPolygon)

                Return polygonCombined

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

            Return Nothing

        End Function

        Public Shared Function projectGeometry(ByVal pGeo As IGeometry, _
                ByVal fromProj As ISpatialReference, _
                ByVal toProj As ISpatialReference) As IGeometry

            Try


                Dim pGeometry As IGeometry
                pGeometry = CloneGeometry(pGeo)
                'pGeometry = pGeo
                pGeometry.SpatialReference = fromProj
                pGeometry.Project(toProj)
                Return pGeometry
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Shared Function CloneGeometry(ByVal pGeo As IGeometry) As IGeometry
            Try


                Dim pClone As IClone
                pClone = pGeo
                Return pClone.Clone
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

#Region "from old ShapeUtil"

        Public Shared Sub ProjectShape(ByVal pFeature As IFeature, _
               ByVal pFromSpatialReference As ISpatialReference, _
               ByVal pToSpatialReference As ISpatialReference)
            Try
                Dim pProjectedShape As IGeometry
                pProjectedShape = CloneGeometry(pFeature.ShapeCopy)



                projectGeometry(pProjectedShape, pFromSpatialReference, pToSpatialReference)


            Catch ex As Exception

            End Try

        End Sub

        Public Shared Sub SetGeoProjection(ByRef pSR As ISpatialReference, ByVal isNAD1927 As Boolean)
            Dim pSpRFc As SpatialReferenceEnvironment
            pSpRFc = New SpatialReferenceEnvironment
            Dim pGCS As IGeographicCoordinateSystem

            'Set pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCS_NAD1983)
            If isNAD1927 Then
                pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1927)

            Else
                pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1983)
            End If
            pSR = pGCS
            pSR.SetFalseOriginAndUnits(-180, -90, 1000000)
        End Sub
        Public Shared Function CreateGeoProjection(ByVal isNAD1927 As Boolean) As IGeographicCoordinateSystem
            Try
                Dim pSpRFc As SpatialReferenceEnvironment
                pSpRFc = New SpatialReferenceEnvironment
                Dim pGCS As IGeographicCoordinateSystem

                Dim pSR As ISpatialReference

                'Set pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCS_NAD1983)
                If isNAD1927 Then
                    pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1927)

                Else
                    pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_NAD1983)
                End If
                pSR = pGCS
                'pSR.SetFalseOriginAndUnits(-180, -90, 1000000)
                pSR.SetFalseOriginAndUnits(-180, -90, 10000000000000) '@@@@@2015-10
                Return pSR
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Shared Function CreateGeoProjection_WGS(ByVal isWGS1972 As Boolean) As IGeographicCoordinateSystem
            Try
                Dim pSpRFc As SpatialReferenceEnvironment
                pSpRFc = New SpatialReferenceEnvironment
                Dim pGCS As IGeographicCoordinateSystem

                Dim pSR As ISpatialReference

                If isWGS1972 Then
                    pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_WGS1972)
                Else
                    pGCS = pSpRFc.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_WGS1984)
                End If
                pSR = pGCS
                'pSR.SetFalseOriginAndUnits(-180, -90, 1000000)
                pSR.SetFalseOriginAndUnits(-180, -90, 10000000000000) '@@@@@2015-10
                Return pSR
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
#End Region


#Region "Do TopologicalOperator "


        Public Shared Function DoBuffer(ByVal geom As IGeometry, ByVal bufferDisFeet As Single, Optional ByVal isOutter As Boolean = False) As IPolygon

            Dim cloneGeom As IGeometry = Geonamic.Core.AOUtils.Others.CommonUtil.Clone(geom)
            Dim sp As ISpatialReference = geom.SpatialReference

            Dim geoBag As IGeometryCollection = New GeometryBag()
            Dim enumGeo As IEnumGeometry = geoBag
            geoBag.AddGeometry(cloneGeom)

            Dim output As IGeometryCollection = New GeometryBag()


            Dim bff As IBufferConstruction = New BufferConstruction()
            Dim prop As IBufferConstructionProperties = bff
            prop.OutsideOnly = isOutter

            bff.ConstructBuffers(geoBag, bufferDisFeet, output)

            If output.GeometryCount > 0 Then
                Dim result As IGeometry = output.Geometry(0)
                result.SpatialReference = sp 'geom.SpatialReference
                Return result
            Else
                Return Nothing
            End If

        End Function


        Public Shared Function DoOutterBuffer(ByVal geom As IGeometry, ByVal bufferDis As Single) As IPolygon
            Return DoBuffer(geom, bufferDis, True)
        End Function




#End Region











    End Class


End Namespace
