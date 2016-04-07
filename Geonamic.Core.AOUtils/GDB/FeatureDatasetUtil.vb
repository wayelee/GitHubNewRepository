Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase




Namespace GDB

    Public Class FeatureDatasetUtil


        Public Shared Function FindFeatureClass(ByVal featDS As IFeatureDataset, ByVal fcName As String) As IFeatureClass

            Dim eds As IEnumDataset = featDS.Subsets()

            Dim ds As IDataset = eds.Next()
            While ds IsNot Nothing

                If TypeOf (ds) Is IFeatureClass Then

                    Dim fc As FeatureClass = ds
                    If fc.Name = fcName Then
                        Return fc
                    End If

                End If

                ds = eds.Next

            End While

            Return Nothing

        End Function



    End Class


End Namespace
