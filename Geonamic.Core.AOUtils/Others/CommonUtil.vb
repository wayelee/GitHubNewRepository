
Imports ESRI.ArcGIS.esriSystem

Namespace Others



    Public Class CommonUtil

        'Test...
        Public Shared Function Clone(ByVal obj As Object) As Object

            Dim c As IClone = CType(obj, IClone)
            Return c.Clone()

        End Function


        Public Shared Function CloneAOObject(ByVal obj As Object) As Object

            Dim objCopy As IObjectCopy = New ObjectCopy()
            Return objCopy.Copy(obj)

        End Function






    End Class

End Namespace

