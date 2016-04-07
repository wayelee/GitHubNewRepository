Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geodatabase


Namespace GDB




    Public Class FeatureClassUtil

        '''<summary>
        ''' Get Unique Values By IQueryDef: not support File GDB
        ''' </summary>
        ''' <param name="pFeatureClass">Object FC</param>
        ''' <param name="strField">Object Field Name</param>
        ''' <returns>Result</returns>
        ''' <remarks></remarks>
        Public Shared Function GetUVByQueryDef(ByVal pFeatureClass As IFeatureClass, ByVal strField As String, Optional ByVal WhereClause As String = "") As IList(Of String)

            Dim uvList As IList(Of String) = New List(Of String)

            Dim pQueryDef As IQueryDef
            Dim pRow As IRow
            Dim pCursor As ICursor
            Dim pFeatureWorkspace As IFeatureWorkspace
            Dim pDataset As IDataset

            pDataset = pFeatureClass
            pFeatureWorkspace = pDataset.Workspace
            pQueryDef = pFeatureWorkspace.CreateQueryDef
            With pQueryDef
                .Tables = pDataset.Name ' Fully qualified table name
                .SubFields = "DISTINCT(" & strField & ")"
                .WhereClause = WhereClause
                pCursor = .Evaluate
            End With

            pRow = pCursor.NextRow
            While Not pRow Is Nothing
                Dim pObj As Object = pRow.Value(0)
                uvList.Add(pObj.ToString)
                pRow = pCursor.NextRow
            End While
            Return uvList
        End Function


        Public Shared Function GetUVByQueryDef2(ByVal pTbl As ITable, ByVal strField As String) As IList(Of String)

            Dim uvList As IList(Of String) = New List(Of String)

            Dim csr As ICursor = pTbl.Search(Nothing, False)
            Dim dataStcs As IDataStatistics = New DataStatistics()
            dataStcs.Field = strField
            dataStcs.Cursor = csr

            Dim pEnumerator As IEnumerator = dataStcs.UniqueValues
            pEnumerator.Reset()

            While pEnumerator.MoveNext
                Dim value As Object = pEnumerator.Current
                uvList.Add(value.ToString())
            End While

            Return uvList

        End Function



        Public Shared Function GetSingleFeature(ByVal pFeatureClass As IFeatureClass, ByVal whereClause As String) As IFeature

            Dim pFilter As IQueryFilter = New QueryFilter()
            pFilter.WhereClause = whereClause
            Dim pFeatCsr As IFeatureCursor = pFeatureClass.Search(pFilter, True)

            Dim feat As IFeature = pFeatCsr.NextFeature()

            pFeatCsr = Nothing

            Return feat

        End Function

        Public Shared Function GetFieldNames(ByVal pFeatureClass As IFeatureClass, ByVal filterType As esriFieldType) As List(Of String)

            Dim lst As List(Of String) = New List(Of String)()

            Dim fields As IFields = pFeatureClass.Fields
            Dim count As Integer = fields.FieldCount

            For index As Integer = 0 To count - 1
                Dim f As IField = fields.Field(index)
                If f.Type = filterType Then
                    lst.Add(f.Name)
                End If
            Next

            Return lst


        End Function

        Public Shared Function GetFieldNames(ByVal pFeatureClass As IFeatureClass) As List(Of String)

            Dim lst As List(Of String) = New List(Of String)()

            Dim fields As IFields = pFeatureClass.Fields
            Dim count As Integer = fields.FieldCount

            For index As Integer = 0 To count - 1
                Dim f As IField = fields.Field(index)
                lst.Add(f.Name)
                'If f.Type = esriFieldType.esriFieldTypeString Then
                '    lst.Add(f.Name)
                'End If
            Next

            Return lst

        End Function


        Public Shared Function HasZ(ByVal featClass As IFeatureClass) As Boolean

            Dim geoDef As IGeometryDef = GetGeometryDef(featClass)
            Return geoDef.HasZ

        End Function

        Public Shared Function GetSpatialReference(ByVal featClass As IFeatureClass) As ESRI.ArcGIS.Geometry.ISpatialReference

            Dim geoDef As IGeometryDef = GetGeometryDef(featClass)
            Return geoDef.SpatialReference

        End Function


        Public Shared Function GetGeometryDef(ByVal featClass As IFeatureClass) As IGeometryDef

            Dim shpFieldName As String = featClass.ShapeFieldName
            Dim shpFieldIndex As Integer = featClass.Fields.FindField(shpFieldName)
            Dim fld As IField = featClass.Fields.Field(shpFieldIndex)

            If fld IsNot Nothing Then
                Return fld.GeometryDef
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' Get OrderBy Cursor. If "Too few parmeters.." exception thrown, check orderfields is CORRECT first!!
        ''' </summary>
        ''' <param name="fc"></param>
        ''' <param name="fiter"></param>
        ''' <param name="orderFields"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function OrderBy(ByVal fc As IFeatureClass, ByVal fiter As IQueryFilter, ByVal orderFields As List(Of String)) As ICursor

            'Dim pQueryFilterDefinition As IQueryFilterDefinition
            'pQueryFilterDefinition = fiter
            'pQueryFilterDefinition.PostfixClause = "ORDER BY " & BuildFieldsString(orderFields)

            'Dim ifeatCursor As IFeatureCursor = fc.Search(fiter, False)
            'Return ifeatCursor


            Dim tbl As ITable = fc

            Dim tblSort As ITableSort = New TableSort()
            tblSort.Table = tbl

            tblSort.QueryFilter = fiter
            tblSort.Fields = BuildFieldsString(orderFields)

            For Each fld As String In orderFields
                tblSort.Ascending(fld) = True
            Next

            tblSort.Sort(Nothing)


            Return tblSort.Rows

        End Function

        Private Shared Function BuildFieldsString(ByVal orderFields As List(Of String)) As String

            Dim result As String = String.Empty
            For index As Integer = 0 To orderFields.Count - 1

                If index = 0 Then
                    result = result & orderFields(index)
                Else
                    result = result & ", " & orderFields(index)
                End If

            Next

            Return result

        End Function

        Public Shared Function GetMaxIDValue(ByVal featClass As IFeatureClass, ByVal idFieldName As String) As Object

            Dim qF As New QueryFilter
            Dim ts As New TableSort
            Dim pTable As ITable
            pTable = featClass
            ts.Table = pTable
            ts.QueryFilter = qF
            ts.Fields = idFieldName
            ts.Ascending(idFieldName) = False
            ts.Sort(Nothing)
            Dim index As Integer = pTable.FindField(idFieldName)
            Dim row As IRow
            Dim sC As ICursor = ts.Rows
            row = sC.NextRow
            If row IsNot Nothing Then
                Return row.Value(index)
            Else
                Return Nothing
            End If

        End Function




    End Class


End Namespace