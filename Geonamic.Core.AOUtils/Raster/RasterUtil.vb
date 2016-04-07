
Imports System.Drawing

Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry

Namespace Raster

    Public Class RasterUtil

        Public Shared Function ConvertMapCoordToPoint(ByVal raster As IRaster, ByVal pt As IPoint) As System.Drawing.Point

            Dim col, row As Integer
            Dim pRaster As IRaster2 = raster
            pRaster.MapToPixel(pt.X, pt.Y, col, row)

            Return New System.Drawing.Point(col, row)

        End Function

        Public Shared Function ConvertPointToMapCoord(ByVal raster As IRaster, ByVal pt As System.Drawing.Point) As IPoint

            Dim mapX, mapY As Double
            Dim pRaster As IRaster2 = raster

            pRaster.PixelToMap(pt.X, pt.Y, mapX, mapY)
            Dim mapPt As IPoint = New ESRI.ArcGIS.Geometry.Point()
            mapPt.PutCoords(mapX, mapY)

            Return mapPt

        End Function


        Public Shared Function Cell2Polygon(ByVal raster As IRaster, ByVal cellPt As System.Drawing.Point) As IPolygon


            Dim mapPt As IPoint = ConvertPointToMapCoord(raster, cellPt)

            Dim cellSize As System.Drawing.PointF = GetCellSize(raster)

            cellSize.X = cellSize.X * 1
            cellSize.Y = cellSize.Y * 1

            Dim pt0, pt1, pt2, pt3, pt4 As IPoint
            pt0 = New ESRI.ArcGIS.Geometry.Point()
            pt0.PutCoords(mapPt.X - cellSize.X / 2, mapPt.Y - cellSize.Y / 2)
            pt1 = New ESRI.ArcGIS.Geometry.Point()
            pt1.PutCoords(mapPt.X - cellSize.X / 2, mapPt.Y + cellSize.Y / 2)
            pt2 = New ESRI.ArcGIS.Geometry.Point()
            pt2.PutCoords(mapPt.X + cellSize.X / 2, mapPt.Y + cellSize.Y / 2)
            pt3 = New ESRI.ArcGIS.Geometry.Point()
            pt3.PutCoords(mapPt.X + cellSize.X / 2, mapPt.Y - cellSize.Y / 2)
            pt4 = New ESRI.ArcGIS.Geometry.Point()
            pt4.PutCoords(mapPt.X - cellSize.X / 2, mapPt.Y - cellSize.Y / 2)

            Dim cellPoly As IPolygon = New Polygon()
            Dim ptCol As IPointCollection = cellPoly
            ptCol.AddPoint(pt0)
            ptCol.AddPoint(pt1)
            ptCol.AddPoint(pt2)
            ptCol.AddPoint(pt3)
            ptCol.AddPoint(pt4)

            Return cellPoly

        End Function

        'Public Shared Function CellArea2Polygon(ByVal raster As IRaster, ByVal cellArea As List(Of System.Drawing.Point)) As IPolygon

        '    Dim resultPolygon As IPolygon = Nothing

        '    For Each cellPt As System.Drawing.Point In cellArea

        '        Dim cellPolygon As IPolygon = Cell2Polygon(raster, cellPt)

        '        If resultPolygon Is Nothing Then
        '            resultPolygon = cellPolygon
        '        Else
        '            resultPolygon = EsriFeatureClassUtil.CombinePolygon(resultPolygon, cellPolygon)
        '        End If

        '    Next

        '    Return resultPolygon

        'End Function

        Public Shared Function CellArea2Polygon(ByVal raster As IRaster, ByVal cellArea As List(Of System.Drawing.Point)) As IPolygon

            Dim resultPolygon As IPolygon = Nothing

            Dim enumGeometry As IEnumGeometry = New GeometryBag()
            Dim geoCol As IGeometryCollection = enumGeometry

            For Each cellPt As System.Drawing.Point In cellArea

                Dim cellPolygon As IPolygon = Cell2Polygon(raster, cellPt)
                geoCol.AddGeometry(cellPolygon)

            Next

            resultPolygon = Geometry.GeometryUtil.CombinePolygons(enumGeometry)

            Return resultPolygon

        End Function


        Public Shared Function GetCellSize(ByVal raster As IRaster) As System.Drawing.PointF

            Dim pRasProp As IRasterProps
            pRasProp = raster

            '  Dim spatialRef As ISpatialReference = pRasProp.SpatialReference


            Dim pt As IPnt = pRasProp.MeanCellSize
            Return New PointF(pt.X, pt.Y)

        End Function


        Public Shared Function GetRasterExtent(ByVal raster As IRaster) As IEnvelope

            Dim pRasProp As IRasterProps
            pRasProp = raster

            Return pRasProp.Extent
        End Function

        Public Shared Function GetRasterSize(ByVal raster As IRaster) As System.Drawing.Size

            Dim pRasProp As IRasterProps
            pRasProp = raster

            Return New System.Drawing.Size(pRasProp.Width, pRasProp.Height)
        End Function


        Public Shared Function GetRasterSpatialRef(ByVal raster As IRaster) As ISpatialReference

            Dim pRasProp As IRasterProps
            pRasProp = raster

            Return pRasProp.SpatialReference


        End Function


        Public Shared Sub SetNoDataValue(ByVal raster As IRaster, ByVal v As Object)

            Try

                Dim pRasProp As IRasterProps
                pRasProp = raster

                pRasProp.NoDataValue = v

            Catch ex As Exception
                Throw ex
            End Try

        End Sub

        Public Shared Function GetNoDataValue(ByVal raster As IRaster) As Object

            Try

                Dim pRasProp As IRasterProps
                pRasProp = raster

                Return pRasProp.NoDataValue

            Catch ex As Exception
                Throw ex
            End Try

        End Function



        Public Shared Function GetArrayFromRaster(ByVal raster As IRaster, ByVal extent As System.Drawing.Rectangle) As System.Array

            Dim leftTop As System.Drawing.Point = New System.Drawing.Point(extent.Left, extent.Top)
            Dim size As System.Drawing.Size = extent.Size

            ' Dim pRLayer As IRasterLayer
            Dim pRaster As IRaster = raster
            Dim pRasterProps As IRasterProps
            Dim pRasterBandColl As IRasterBandCollection
            Dim pRasterBand As IRasterBand
            Dim pRawVals As IRawPixels
            Dim pPixBlock As IPixelBlock3
            Dim pSafeArray As System.Array = Nothing

            Try

                pRasterProps = CType(pRaster, IRasterProps)


                Dim pRaster2 As IRaster2
                pRaster2 = CType(pRaster, IRaster2)

                pRasterBandColl = CType(pRaster, IRasterBandCollection)
                pRasterBand = pRasterBandColl.Item(0)
                pRawVals = CType(pRasterBand, IRawPixels)

                Dim pSizePoint As IPnt
                pSizePoint = New Pnt()
                pSizePoint.SetCoords(size.Width, size.Height)

                ' pPixBlock = pRaster.CreatePixelBlock(pSizePoint)
                pPixBlock = pRaster.CreatePixelBlock(pSizePoint)

                Dim tlp As IPnt = New Pnt()
                tlp.SetCoords(leftTop.X, leftTop.Y)
                pRawVals.Read(tlp, pPixBlock)

                'pSafeArray = pPixBlock.SafeArray(0)
                pSafeArray = CType(pPixBlock.PixelData(0), System.Array)

            Catch ex As Exception

                MsgBox(ex.Message)

            End Try

            Return pSafeArray

        End Function


        Public Shared Function GetArrayFromRaster(ByVal raster As IRaster) As System.Array

            ' Dim pRLayer As IRasterLayer
            Dim pRaster As IRaster = raster
            Dim pRasterProps As IRasterProps
            Dim pRasterBandColl As IRasterBandCollection
            Dim pRasterBand As IRasterBand
            Dim pRawVals As IRawPixels
            Dim pPixBlock As IPixelBlock3
            Dim pSafeArray As System.Array = Nothing

            Try

                pRasterProps = CType(pRaster, IRasterProps)


                Dim pRaster2 As IRaster2
                pRaster2 = CType(pRaster, IRaster2)

                pRasterBandColl = CType(pRaster, IRasterBandCollection)
                pRasterBand = pRasterBandColl.Item(0)
                pRawVals = CType(pRasterBand, IRawPixels)

                Dim pSizePoint As IPnt
                pSizePoint = New Pnt()
                pSizePoint.SetCoords(pRasterProps.Width, pRasterProps.Height)

                ' pPixBlock = pRaster.CreatePixelBlock(pSizePoint)
                pPixBlock = pRaster.CreatePixelBlock(pSizePoint)

                '×óÉÏµã×ø±ê
                Dim tlp As IPnt = New Pnt()
                tlp.SetCoords(0, 0)
                pRawVals.Read(tlp, pPixBlock)

                'pSafeArray = pPixBlock.SafeArray(0)
                pSafeArray = CType(pPixBlock.PixelData(0), System.Array)

            Catch ex As Exception

                MsgBox(ex.Message)

            End Try

            Return pSafeArray

        End Function



        Public Shared Function Identify(ByVal raster As IRaster, ByVal pt As IPoint) As Double

            Try
                'Dim colRow As System.Drawing.Point = ConvertMapCoordToPoint(raster, pt)

                ''Dim r2 As IRaster2 = raster
                ''Return r2.GetPixelValue(0, colRow.X, colRow.Y)

                Dim pSizePoint As IPnt = New Pnt()
                pSizePoint.X = 1
                pSizePoint.Y = 1

                Dim tempPB As IPixelBlock = raster.CreatePixelBlock(pSizePoint)

                Dim tlp As IPnt = New Pnt()
                Dim colRow As System.Drawing.Point = ConvertMapCoordToPoint(raster, pt)
                tlp.SetCoords(colRow.X, colRow.Y)
                raster.Read(tlp, tempPB)

                Dim tempSA As Object = tempPB.SafeArray(0)
                Dim v As Object = tempSA(0, 0)

                'Dim r2 As IRaster2 = raster
                'Dim v2 = r2.GetPixelValue(0, colRow.X, colRow.Y)

                Return v

            Catch ex As Exception
                Throw ex
            End Try
        End Function


    End Class


End Namespace
