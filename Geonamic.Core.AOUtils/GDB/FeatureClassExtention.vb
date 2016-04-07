Imports System
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

Namespace GDBExtention

    Public Module FeatureClassExtention

        <System.Runtime.CompilerServices.Extension()> _
        Public Function GeometryHasZ(ByVal featClass As IFeatureClass) As Boolean

            Return GDB.FeatureClassUtil.HasZ(featClass)

        End Function


        <System.Runtime.CompilerServices.Extension()> _
        Public Function GeometryHasM(ByVal featClass As IFeatureClass) As Boolean

            Dim geoDef As IGeometryDef = GDB.FeatureClassUtil.GetGeometryDef(featClass)
            Return geoDef.HasM

        End Function


        <System.Runtime.CompilerServices.Extension()> _
        Public Function GeometrySpatialReference(ByVal featClass As IFeatureClass) As ISpatialReference

            Dim geoDef As IGeometryDef = GDB.FeatureClassUtil.GetGeometryDef(featClass)
            Return geoDef.SpatialReference

        End Function

    End Module



End Namespace