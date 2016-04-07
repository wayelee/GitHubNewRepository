Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesGDB


Namespace GDB

    Public Class ITableUtil

        Public Shared Function GetBlankDataTable(ByVal srcPTable As ITable) As System.Data.DataTable

            Dim fieldNamesToConvert As Dictionary(Of String, Integer) = GDB.ITableUtil.GetFieldNamesToQuery(srcPTable, Nothing)
            Dim dt As DataTable = GDB.ITableUtil.CreateBlankDataTableByITable(srcPTable, fieldNamesToConvert)
            Return dt

        End Function

        Private Shared Function CreateBlankDataTableByITable(ByVal srcPTable As ITable, fieldNamesToConvert As Dictionary(Of String, Integer)) As DataTable

            Dim resultTbl As New DataTable

            For Each fieldName As String In fieldNamesToConvert.Keys
                Dim fldIndex As Integer = fieldNamesToConvert.Item(fieldName)
                Dim srcField As IField = srcPTable.Fields.Field(fldIndex)

                Dim col As New DataColumn(fieldName.ToUpper, ConvertESRIFieldToType(srcField.Type))
                col.Caption = srcField.AliasName
                col.DefaultValue = srcField.DefaultValue
                ' col.AllowDBNull = srcField.IsNullable 'AllDBNull will trigger when editing as databinding mode..

                'If col.DataType Is GetType(String) Then 'MaxLength of String field will trigger exception when data in DB is not valid..
                '    col.MaxLength = srcField.Length
                'End If

                'col.ReadOnly = Not srcField.Editable '---Readonly property will trigger Readonly Exception when populate data

                resultTbl.Columns.Add(col)
            Next

            Return resultTbl

        End Function

        Public Shared Function ConvertESRIFieldToType(ByVal fldType As esriFieldType) As Type

            'need field type
            Select Case fldType

                Case esriFieldType.esriFieldTypeSmallInteger
                    Return GetType(Int32)

                Case esriFieldType.esriFieldTypeInteger
                    Return GetType(Int32)

                Case esriFieldType.esriFieldTypeSingle
                    Return GetType(Single)

                Case esriFieldType.esriFieldTypeDouble
                    Return GetType(Double)

                Case esriFieldType.esriFieldTypeString
                    Return GetType(String)

                Case esriFieldType.esriFieldTypeDate
                    Return GetType(DateTime)

                Case esriFieldType.esriFieldTypeOID
                    Return GetType(Int32)

                Case esriFieldType.esriFieldTypeGUID
                    Return GetType(Guid)

                Case esriFieldType.esriFieldTypeGlobalID
                    Return GetType(Guid)

                Case esriFieldType.esriFieldTypeXML
                    Return GetType(String)

                Case Else 'esriFieldType.esriFieldTypeGeometry,esriFieldType.Blob,esriFieldType.Raster
                    Return System.Type.GetType("System.String")

            End Select

        End Function


        Public Shared Function GetFieldNamesToQuery(pTable As ITable, pQFilter As IQueryFilter) As Dictionary(Of String, Integer)

            Dim result As New Dictionary(Of String, Integer)()

            Dim subFieldNames As List(Of String) = ParseSubFieldNames(pQFilter)

            If subFieldNames.Count = 0 Then
                'Add all Fields
                For index = 0 To pTable.Fields.FieldCount - 1
                    Dim fld As IField = pTable.Fields.Field(index)

                    If fld.Type = esriFieldType.esriFieldTypeBlob OrElse fld.Type = esriFieldType.esriFieldTypeGeometry _
                         OrElse fld.Type = esriFieldType.esriFieldTypeRaster Then
                        Continue For
                    End If

                    result.Add(fld.Name.ToUpper, index)
                Next

            Else
                'Just add SubFields
                For Each fldName As String In subFieldNames
                    Dim index As Integer = pTable.Fields.FindField(fldName)
                    Dim fld As IField = pTable.Fields.Field(index)
                    result.Add(fld.Name.ToUpper, index)
                Next

            End If

            Return result

        End Function

        Private Shared Function ParseSubFieldNames(pQFilter As IQueryFilter) As List(Of String)

            Dim result As New List(Of String)()

            If pQFilter IsNot Nothing AndAlso (Not String.IsNullOrEmpty(pQFilter.SubFields)) AndAlso (Not pQFilter.SubFields = "*") Then
                Dim words As String() = pQFilter.SubFields.Split(New Char() {","c})
                For Each w In words
                    Dim trimed As String = Trim(w)
                    If Not String.IsNullOrEmpty(trimed) Then
                        result.Add(w)
                    End If
                Next

            End If

            Return result

        End Function

        Public Shared Function GetDataTableFromITable(ByVal srcArcTbl As ITable, whereClause As String, Optional maxRowCount As Integer = 100000) As DataTable

            Dim arcFilter As IQueryFilter = New QueryFilter
            arcFilter.WhereClause = whereClause
            Return GetDataTableFromITable(srcArcTbl, arcFilter, maxRowCount)

        End Function


        Public Shared Sub PopulateDatarow(esriRow As IRow, dr As DataRow, fieldNamesToConvert As Dictionary(Of String, Integer))

            dr.BeginEdit()
            For Each col As DataColumn In dr.Table.Columns
                If Not fieldNamesToConvert.ContainsKey(col.ColumnName) Then
                    Continue For
                End If
                Dim fldIndex As Integer = fieldNamesToConvert(col.ColumnName)
                dr(col.ColumnName) = esriRow.Value(fldIndex)
            Next
            dr.EndEdit()

        End Sub

        Public Shared Function GetDataTableFromITable(ByVal srcArcTbl As ITable, arcFilter As IQueryFilter, Optional maxRowCount As Integer = 100000) As DataTable

            Dim fieldNamesToConvert As Dictionary(Of String, Integer) = GetFieldNamesToQuery(srcArcTbl, arcFilter)

            Dim dt As DataTable = CreateBlankDataTableByITable(srcArcTbl, fieldNamesToConvert)
            'Dim pFields As IFields
            'pFields = srcArcTbl.Fields

            Dim pCursor As ICursor = Nothing
            Dim pRowBuff As IRowBuffer
            Try


                pCursor = srcArcTbl.Search(arcFilter, True)

                pRowBuff = pCursor.NextRow
                Dim maxnum As Integer = maxRowCount

                dt.BeginLoadData()

                Dim count As Integer = 0
                Do While Not pRowBuff Is Nothing

                    Dim dr As DataRow = dt.Rows.Add()

                    PopulateDatarow(pRowBuff, dr, fieldNamesToConvert)

                    'dr.BeginEdit()
                    'For Each col As DataColumn In dr.Table.Columns
                    '    If Not fieldNamesToConvert.ContainsKey(col.ColumnName) Then
                    '        Continue For
                    '    End If
                    '    Dim fldIndex As Integer = fieldNamesToConvert(col.ColumnName)
                    '    dr(col.ColumnName) = pRowBuff.Value(fldIndex)
                    'Next
                    'dr.EndEdit()
                    'If dr.RowState = DataRowState.Added Then
                    '    dr.AcceptChanges()
                    'End If


                    pRowBuff = pCursor.NextRow
                    count += 1
                    'If count > maxnum Then
                    '    Throw New Exception("Maxnum records Over " & maxnum)
                    '    Exit Do
                    'End If
                Loop

                dt.EndLoadData()
                dt.AcceptChanges()

                Return dt
            Catch ex As Exception
                Return dt
            End Try
        End Function


    End Class


End Namespace
