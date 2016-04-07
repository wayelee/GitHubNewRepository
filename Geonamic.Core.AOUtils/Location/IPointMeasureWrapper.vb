Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location




Namespace Location

    Public Class IPointMeasureWrapper
        Implements IComparable


        Private _Pt As IPoint
        Public Property PT() As IPoint
            Get
                Return _Pt
            End Get
            Set(ByVal value As IPoint)
                _Pt = value
            End Set
        End Property


        Private _Measure As Double
        Public Property Measure() As Double
            Get
                Return _Measure
            End Get
            Set(ByVal value As Double)
                _Measure = value
            End Set
        End Property



        Public Sub New(ByVal pt As IPoint)
            _Pt = pt
            _Measure = _Pt.M
        End Sub

        Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo

            Dim objPt As IPointMeasureWrapper = CType(obj, IPointMeasureWrapper)
            Return Me.Measure.CompareTo(objPt.Measure)

            'If CType(obj, IPointMeasureWrapper).Measure - Me.Measure < -0.0001 Then
            '    Return 1
            'ElseIf CType(obj, IPointMeasureWrapper).Measure - Me.Measure > 0.0001 Then
            '    Return -1
            'Else
            '    Return 0
            'End If

        End Function


    End Class

End Namespace
