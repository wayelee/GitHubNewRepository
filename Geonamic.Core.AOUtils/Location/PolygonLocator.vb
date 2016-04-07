
Imports System.Runtime.InteropServices

Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Carto


Namespace Location


    Public Class MeasurePair

        Private _BegMeasure As Double = Double.NaN
        Public Property BegMeasure() As Double
            Get
                Return _BegMeasure
            End Get
            Set(ByVal value As Double)
                _BegMeasure = value
            End Set
        End Property


        Private _EndMeasure As Double = Double.NaN
        Public Property EndMeasure() As Double
            Get
                Return _EndMeasure
            End Get
            Set(ByVal value As Double)
                _EndMeasure = value
            End Set
        End Property

        Public Sub New(ByVal begM As Double, ByVal endM As Double)
            _BegMeasure = begM
            _EndMeasure = endM
        End Sub


    End Class



    Public Class PolygonLocator


        Private _routeLine As IPolyline


        Public Sub New(ByVal routeLine As IPolyline)

            Dim mA As IMAware = routeLine
            If mA.MAware = False Then
                Throw New Exception("ZAware route need input.")
            End If

            _routeLine = routeLine

        End Sub

        'Private Function ConvertPolygonToPolyline(ByVal objPolygon As IPolygon) As IGeometry

        '    Dim result As IPolyline = New Polyline()
        '    Dim ptResultCol As IPointCollection = result

        '    Dim orgPtCol As IPointCollection = objPolygon
        '    For i As Integer = 0 To orgPtCol.PointCount - 2

        '        Dim pt As IPoint = orgPtCol.Point(i)
        '        Dim newPt As IPoint = Geometry.GeometryUtil.CloneGeometry(pt)
        '        ptResultCol.AddPoint(pt)

        '    Next

        '    Return result

        'End Function

        Public Function Locate(ByVal objPolygon As IPolygon) As List(Of MeasurePair)

            Dim result As New List(Of MeasurePair)()
            Dim topo As ITopologicalOperator5 = _routeLine


            Dim copyObj As IPolygon = Geometry.GeometryUtil.CloneGeometry(objPolygon)
            copyObj.Project(_routeLine.SpatialReference)
            ' Dim objLine As IGeometry = ConvertPolygonToPolyline(copyObj)

            Dim topo2 As ITopologicalOperator = copyObj
            topo2.Simplify()

            Dim intersectResult As IGeometry = topo.IntersectMultidimension(copyObj)

            If intersectResult.GeometryType = esriGeometryType.esriGeometryBag Then

                Dim isTheEnd As Boolean = False

                Dim resultEnum As IEnumGeometry = intersectResult
                resultEnum.Reset()
                Dim curResult As IGeometry = resultEnum.Next
                While curResult IsNot Nothing

                    If curResult.GeometryType = esriGeometryType.esriGeometryMultipoint Then

                        'Note: CreateImpactEvent function can handle EndPoint condition now, so donot need deal with it seperately. 
                        Dim ptCol As IPointCollection = curResult
                        result.AddRange(CreateMeasurePair(_routeLine, ptCol, copyObj))

                    ElseIf curResult.GeometryType = esriGeometryType.esriGeometryPolyline Then

                        'If isTheEnd Then
                        '    Dim line As IPolyline = curResult
                        '    Dim rE As ImpactResultEvent = CreateImpactEvent(lineID, line, objID, impactType)
                        '    result.Add(rE)
                        'End If

                    End If


                    curResult = resultEnum.Next

                End While

            Else
                Throw New Exception("Locate Polygon Error. Intersect Result Type:" & intersectResult.GeometryType.ToString())
            End If

            Return result

        End Function


        Private Function CreateMeasurePair(ByVal centerLine As IPolyline, _
                                                  ByVal ptCol As IPointCollection, _
                                                  ByVal obj As IPolygon) As List(Of MeasurePair)

            Try

                Dim result As New List(Of MeasurePair)()

                Dim ptLst As List(Of IPointMeasureWrapper) = CreateSortPoints(centerLine, ptCol)

                Dim count As Integer = ptLst.Count
                If count < 2 Then
                    Return Nothing
                ElseIf count Mod 2 = 1 Then
                    count = count - 1
                End If

                For index As Integer = 0 To ptLst.Count - 2 Step 1

                    Dim line As IPolyline = New Polyline()
                    line.FromPoint = ptLst(index).PT
                    line.ToPoint = ptLst(index + 1).PT
                    line.SpatialReference = centerLine.SpatialReference


                    If IsWithin(line, obj) Then
                        Dim rE As MeasurePair = CreateMeasurePair(line)
                        result.Add(rE)
                    End If

                Next

                Return result


            Catch ex As Exception
                Throw ex
            End Try

        End Function

        Private Function IsWithin(ByVal segment As IPolyline, ByVal obj As IPolygon) As Boolean

            Dim relOp As IRelationalOperator = obj
            Return relOp.Contains(segment)

        End Function



        Private Function CreateMeasurePair(ByVal segment As IPolyline) As MeasurePair

            Dim ptCol As IPointCollection = segment
            Dim count As Integer = ptCol.PointCount
            Dim firstPt, lastPt As IPoint
            firstPt = ptCol.Point(0)
            lastPt = ptCol.Point(count - 1)

            Dim mPair As New MeasurePair(firstPt.M, lastPt.M)
            Return mPair

        End Function


        Private Function CreateSortPoints(ByVal centerline As IPolyline, ByVal ptCol As IPointCollection) As List(Of IPointMeasureWrapper)

            Dim lst As New List(Of IPointMeasureWrapper)

            'Add centerline's From and To points  
            lst.Add(New IPointMeasureWrapper(centerline.FromPoint))
            lst.Add(New IPointMeasureWrapper(centerline.ToPoint))

            Dim count As Integer = ptCol.PointCount
            For index As Integer = 0 To count - 1
                Dim pt As IPoint = ptCol.Point(index)
                pt.SpatialReference = centerline.SpatialReference
                lst.Add(New IPointMeasureWrapper(pt))
            Next
            lst.Sort()
            Return lst

        End Function

    End Class



End Namespace


