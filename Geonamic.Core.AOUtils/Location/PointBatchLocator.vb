Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Carto


Namespace Location

    Public Class PointBatchLocator
        Implements IDisposable

        Private _TempWorkspace As IFeatureWorkspace
        Private _RouteFC As IFeatureClass

        Private _RouteIDFieldName As String
        Private _RouteID As Object

        Private _EventTblName As String = "EventTblName"
        Private _OutputFCName As String = "OutputFC"
        Private _MeasureFieldName As String = "Measure"



        Public Sub New(ByVal routeFC As IFeatureClass, ByVal routeID As String, ByVal routeIDFieldName As Object)

            _RouteFC = routeFC
            _RouteID = routeID
            _RouteIDFieldName = routeIDFieldName

            CreateWS()
        End Sub


        Public Function LocateBatch(ByVal measureLst As List(Of Double)) As List(Of IPoint)

            Dim tbl As ITable = SaveToEventTable(measureLst)

            GDB.WorkspaceUtil.DeleteFCIfExists(_OutputFCName, _TempWorkspace)
            Location.RouteUtil.ConvertPtEventsToFC(_TempWorkspace, _RouteFC, _EventTblName, "", _OutputFCName, _RouteIDFieldName, _MeasureFieldName)

            Dim fc As IFeatureClass = GetOutputFC()
            Return ReadFromFeatureClass(fc)

        End Function


#Region "Private Functions"

        Private Sub CreateWS()

            _TempWorkspace = GDB.WorkspaceUtil.CreateFileGdbScratchWorkspace()

        End Sub

        Private Function SaveToEventTable(ByVal sortedMeasureLst As List(Of Double)) As ITable

            sortedMeasureLst.Sort()

            GDB.WorkspaceUtil.DeleteTableIfExists(_EventTblName, _TempWorkspace)
            Dim tbl As ITable = CreateEventTable(_TempWorkspace)

            Dim rowBuffer As IRowBuffer = tbl.CreateRowBuffer()
            Dim cursor As ICursor = tbl.Insert(True)

            Dim routeIDFldIndex As Integer = rowBuffer.Fields.FindField(_RouteIDFieldName)
            Dim measureFldIndex As Integer = rowBuffer.Fields.FindField(_MeasureFieldName)
            For Each m As Double In sortedMeasureLst

                rowBuffer.Value(routeIDFldIndex) = _RouteID
                rowBuffer.Value(measureFldIndex) = m
                cursor.InsertRow(rowBuffer)

            Next

            cursor.Flush()

            Return tbl
        End Function

        Private Function CreateEventTable(ByVal ws As IFeatureWorkspace) As ITable


            Dim featWs As IFeatureWorkspace = ws

            Dim tbl As ITable = New StandaloneTable()

            Dim CLSID As UID = New UID()
            CLSID.Value = "esriGeoDatabase.Object"


            Dim fieldsEdit As IFieldsEdit = New Fields()
            fieldsEdit.FieldCount_2 = 3

            Dim fieldEditObjID As IFieldEdit = New Field()
            fieldEditObjID.Name_2 = "OID"
            fieldEditObjID.Type_2 = esriFieldType.esriFieldTypeOID
            fieldsEdit.Field_2(0) = fieldEditObjID


            'RouteID
            Dim fieldEdit As IFieldEdit = New Field()
            fieldEdit.Name_2 = _RouteIDFieldName
            ' If _EnviConfig.RouteIDIsString Then
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString
            ' Else
            ' fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger
            ' End If
            fieldsEdit.Field_2(1) = fieldEdit


            'Measure
            Dim fieldEditMeasure As IFieldEdit = New Field()
            fieldEditMeasure.Name_2 = _MeasureFieldName
            fieldEditMeasure.Type_2 = esriFieldType.esriFieldTypeDouble
            fieldsEdit.Field_2(2) = fieldEditMeasure


            tbl = featWs.CreateTable(_EventTblName, fieldsEdit, CLSID, Nothing, "")
            Return tbl



        End Function

        Private Function GetOutputFC() As IFeatureClass

            Return _TempWorkspace.OpenFeatureClass(_OutputFCName)

        End Function

        Private Function ReadFromFeatureClass(ByVal fc As IFeatureClass) As List(Of IPoint)

            Dim result As New List(Of IPoint)()
            Dim sql As String = _RouteIDFieldName & "='" & _RouteID & "'"

            Dim filter As IQueryFilter = New QueryFilter()
            filter.WhereClause = sql

            Dim pTable As ITable
            Dim tblSorter As New TableSort()
            pTable = fc
            tblSorter.Table = pTable
            tblSorter.QueryFilter = filter
            tblSorter.Fields = _MeasureFieldName
            tblSorter.Ascending(_MeasureFieldName) = True
            tblSorter.Sort(Nothing)

            Dim row As IRow
            Dim tblCsr As ICursor = tblSorter.Rows
            row = tblCsr.NextRow
            While row IsNot Nothing

                Dim feat As IFeature = row
                result.Add(feat.ShapeCopy)

                row = tblCsr.NextRow
            End While

            Return result

        End Function

#End Region



        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free other state (managed objects).
                End If

                ' TODO: free your own state (unmanaged objects).
                ' TODO: set large fields to null.
                _TempWorkspace = Nothing


            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
