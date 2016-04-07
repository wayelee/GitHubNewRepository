Imports ESRI.ArcGIS.Geometry

Namespace Geometry

    Public Class WKT_WKB_Util

        Public Shared Function ConvertGeometryToWKB(geometry As IGeometry) As Byte()
            Dim wkb As IWkb = TryCast(geometry, IWkb)
            Dim oper As ITopologicalOperator = TryCast(geometry, ITopologicalOperator)
            oper.Simplify()

            Dim factory As IGeometryFactory3 = TryCast(New GeometryEnvironment(), IGeometryFactory3)
            Dim b As Byte() = TryCast(factory.CreateWkbVariantFromGeometry(geometry), Byte())
            Return b
        End Function


        Public Shared Function ConvertWKBToGeometry(wkb As Byte()) As IGeometry
            Dim geom As IGeometry
            Dim countin As Integer = wkb.GetLength(0)
            Dim factory As IGeometryFactory3 = TryCast(New GeometryEnvironment(), IGeometryFactory3)
            factory.CreateGeometryFromWkbVariant(wkb, geom, countin)
            Return geom
        End Function


    End Class

End Namespace
