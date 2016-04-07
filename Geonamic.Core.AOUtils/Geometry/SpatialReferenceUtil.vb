
Imports ESRI.ArcGIS.Geometry


Namespace Geometry



    Public Class SpatialReferenceUtil


#Region "GeographicCoordinateSystem"

        Public Shared Function CreateGCS(ByVal gcsType As esriSRGeoCSType) As ISpatialReference

            Dim spRefEnvi As SpatialReferenceEnvironment = New SpatialReferenceEnvironment()
            Dim gcs As IGeographicCoordinateSystem = spRefEnvi.CreateGeographicCoordinateSystem(gcsType)

            Dim ctrlPrecision As IControlPrecision2 = gcs
            ctrlPrecision.IsHighPrecision = True

            Dim srResolution As ISpatialReferenceResolution = gcs
            srResolution.ConstructFromHorizon()
            srResolution.SetDefaultXYResolution()

            Return gcs

        End Function

        Public Shared Function Create_GCS_North_American_1983() As ISpatialReference

            Return CreateGCS(esriSRGeoCSType.esriSRGeoCS_NAD1983)

        End Function

        Public Shared Function Create_GCS_North_American_1927() As ISpatialReference

            Return CreateGCS(esriSRGeoCSType.esriSRGeoCS_NAD1927)

        End Function


#End Region



#Region "ProjectedCoordinateSystem"

        Public Shared Function CreatePCS(ByVal pcsType As esriSRProjCSType) As ISpatialReference

            Dim spRefEnvi As SpatialReferenceEnvironment = New SpatialReferenceEnvironment()
            Dim gcs As IProjectedCoordinateSystem = spRefEnvi.CreateProjectedCoordinateSystem(pcsType)

            Dim ctrlPrecision As IControlPrecision2 = gcs
            ctrlPrecision.IsHighPrecision = True

            Dim srResolution As ISpatialReferenceResolution = gcs
            srResolution.ConstructFromHorizon()
            srResolution.SetDefaultXYResolution()

            Return gcs

        End Function


        Public Shared Function Create_NAD1983N_AmericaAlbers() As ISpatialReference
            Return CreatePCS(esriSRProjCSType.esriSRProjCS_NAD1983N_AmericaAlbers)
        End Function


#End Region




    End Class

End Namespace
