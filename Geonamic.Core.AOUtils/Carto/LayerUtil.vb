Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesGDB



Namespace Carto


    Public Class LyrInfo
        Implements ICloneable


        Public Name As String
        Public Index As Integer

        Public Sub New(ByVal n As String, ByVal i As Integer)
            Name = n
            Index = i
        End Sub

        Public Sub New()

        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean

            If Not TypeOf (obj) Is LyrInfo Then
                Return False
            End If

            Dim other As LyrInfo = obj
            If other.Name = Me.Name And other.Index = Me.Index Then
                Return True
            Else
                Return False
            End If

        End Function


        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim newObj As New LyrInfo(Me.Name, Me.Index)
            Return newObj
        End Function
    End Class


    Public Class LayerWrapper

        Private _Layer As ILayer
        Public Property Layer() As ILayer
            Get
                Return _Layer
            End Get
            Set(ByVal value As ILayer)
                _Layer = value
            End Set
        End Property

        Public Sub New(ByVal lyr As ILayer)
            _Layer = lyr
        End Sub


        Public Overrides Function ToString() As String
            Return _Layer.Name
        End Function

    End Class


    Public Enum SimpleLayerTypeEnum

        PolygonLayer
        PolylineLayer
        PointLayer

    End Enum


    Public Class LayerUtil





        Public Shared Function GetFeatureLayersInfo(ByVal map As IMap, Optional ByVal filterShapeType As esriGeometryType = esriGeometryType.esriGeometryAny) As List(Of LyrInfo)

            Dim result As New List(Of LyrInfo)()

            Dim enumLayer As IEnumLayer = map.Layers
            Dim lyr As ILayer = enumLayer.Next

            Dim lyrIndex As Integer = -1

            While Not lyr Is Nothing

                lyrIndex = lyrIndex + 1

                If TypeOf lyr Is IFeatureLayer Then

                    Dim featLyr As IFeatureLayer = lyr

                    If featLyr.FeatureClass IsNot Nothing AndAlso featLyr.FeatureClass.ShapeType = filterShapeType Then
                        result.Add(New LyrInfo(featLyr.Name, lyrIndex))
                    End If

                End If

                lyr = enumLayer.Next
            End While

            Return result

        End Function

        Public Shared Function IndexLayerByLayerInfo(ByVal map As IMap, ByVal info As LyrInfo, Optional ByVal elseFindByName As Boolean = True) As ILayer

            Dim enumLayer As IEnumLayer = map.Layers
            Dim lyr As ILayer = enumLayer.Next

            Dim lyrIndex As Integer = 0
            While Not lyr Is Nothing

                If lyrIndex = info.Index And lyr.Name = info.Name Then
                    Return lyr
                End If

                lyrIndex = lyrIndex + 1
                lyr = enumLayer.Next

            End While

            If elseFindByName Then
                Return FindLayerByName(map, info.Name)
            Else
                Return Nothing
            End If

        End Function


        Public Shared Function GetFeatureLayers(ByVal doc As IMapDocument, Optional ByVal filterShapeType As esriGeometryType = esriGeometryType.esriGeometryAny) As IList(Of IFeatureLayer)

            Dim result As List(Of IFeatureLayer) = New List(Of IFeatureLayer)()
            Dim allLyr As List(Of ILayer) = GetLayers(doc, True)

            For Each lyr As ILayer In allLyr
                Dim featLyr As IFeatureLayer = CType(lyr, IFeatureLayer)
                If featLyr.FeatureClass Is Nothing Then
                    Continue For
                End If
                If featLyr.FeatureClass.ShapeType = esriGeometryType.esriGeometryAny Or featLyr.FeatureClass.ShapeType = filterShapeType Then
                    result.Add(featLyr)
                End If
            Next
            Return result
        End Function
        Public Shared Function GetFeatureLayers(ByVal map As IMap, Optional ByVal filterShapeType As esriGeometryType = esriGeometryType.esriGeometryAny) As IList(Of IFeatureLayer)

            Dim result As List(Of IFeatureLayer) = New List(Of IFeatureLayer)()
            Dim allLyr As List(Of ILayer) = GetLayers(map, True)

            For Each lyr As ILayer In allLyr
                Dim featLyr As IFeatureLayer = CType(lyr, IFeatureLayer)
                If featLyr.FeatureClass Is Nothing Then
                    Continue For
                End If


                If filterShapeType = esriGeometryType.esriGeometryAny Then
                    result.Add(featLyr)
                ElseIf featLyr.FeatureClass.ShapeType = filterShapeType Then
                    result.Add(featLyr)
                End If
            Next
            Return result
        End Function


        Public Shared Function GetRasterLayersInfo(ByVal map As IMap) As List(Of LyrInfo)

            Dim result As New List(Of LyrInfo)()

            Dim enumLayer As IEnumLayer = map.Layers
            Dim lyr As ILayer = enumLayer.Next

            Dim lyrIndex As Integer = -1

            While Not lyr Is Nothing

                lyrIndex = lyrIndex + 1

                If TypeOf lyr Is IRasterLayer Then

                    Dim rLyr As IRasterLayer = lyr
                    If rLyr.Raster IsNot Nothing Then
                        result.Add(New LyrInfo(rLyr.Name, lyrIndex))
                    End If

                End If


                lyr = enumLayer.Next
            End While

            Return result


        End Function

        Public Shared Function GetRasterLayers(ByVal map As IMap) As IList(Of IRasterLayer)


            Dim result As List(Of IRasterLayer) = New List(Of IRasterLayer)()
            Dim allLyr As List(Of ILayer) = GetLayers(map, False)

            For Each lyr As ILayer In allLyr
                If TypeOf lyr Is IRasterLayer Then
                    result.Add(lyr)
                End If
            Next
            Return result
        End Function


        Public Shared Function AddFeatureClassToMap(ByVal fc As IFeatureClass, ByVal map As IMap, Optional ByVal name As String = "") As IFeatureLayer

            Dim featLyr As IFeatureLayer = New FeatureLayer()
            featLyr.FeatureClass = fc

            If Not name = "" Then
                featLyr.Name = name
            End If
            map.AddLayer(featLyr)
            Dim activeView As IActiveView = map
            activeView.Refresh()

            Return featLyr

        End Function
        '***
        'Add Personal Geodatabase to the Map,must give a geo layer table
        '***
        Public Shared Function AddLayerToMap(ByVal pMap As IMap, _
                    ByVal strGDBName As String, _
                    ByVal strLayerName As String, _
                    Optional ByVal bRefresh As Boolean = False) As Boolean
            Try
                'Dim pWSF As IWorkspaceFactory
                'Dim pFWS As IFeatureWorkspace
                'Dim pWS As IWorkspace
                Dim pFclass As IFeatureClass

                Dim pFLayer As IFeatureLayer
                'pWSF = New AccessWorkspaceFactory
                'pWS = pWSF.OpenFromFile(strGDBName, 0)
                'pFWS = pWS

                pFclass = GDB.WorkspaceUtil.GetAccessFC(strLayerName, strGDBName)   'pFWS.OpenFeatureClass(strLayerName)
                pFLayer = New FeatureLayer
                pFLayer.FeatureClass = pFclass
                pFLayer.Name = pFclass.AliasName
                pMap.AddLayer(pFLayer)
                Return True

                If bRefresh Then
                    'pMxDoc.ActiveView.Refresh()
                    'pMxDoc.CurrentContentsView.Refresh(0)

                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function AddTableToMap(ByVal pMap As IMap, _
                          ByVal strMDBpath As String, _
                          ByVal strTableName As String, _
                          Optional ByRef strErr As String = Nothing) As Boolean
            Try
                Dim pStdTabColl As IStandaloneTableCollection
                Dim pTable As ITable
                Dim pFact As IWorkspaceFactory
                Dim pFeatWS As IFeatureWorkspace

                pStdTabColl = pMap

                pFact = New AccessWorkspaceFactory
                pFeatWS = pFact.OpenFromFile(strMDBpath, 0)
                pTable = pFeatWS.OpenTable(strTableName)

                Dim sTable As IStandaloneTable
                sTable = New StandaloneTable()
                sTable.Table = pTable

                'Set sTable = pTable
                pStdTabColl.AddStandaloneTable(sTable)
                Return True
            Catch ex As Exception
                strErr = ex.Message
                Return False
            End Try
        End Function

        'Public Shared Function FindITable(ByVal pMap As IMap, _
        '        ByVal tableName As String, Optional ByRef strErr As String = Nothing) As ITable
        '    Try
        '        Dim pStTabColl As IStandaloneTableCollection
        '        Dim i As Long
        '        Dim pStTab As IStandaloneTable
        '        Dim aName As String
        '        Dim pTable As ITable

        '        pStTabColl = pMap
        '        pTable = Nothing

        '        If (pStTabColl.StandaloneTableCount > 0) Then
        '            For i = 0 To pStTabColl.StandaloneTableCount - 1
        '                pStTab = pStTabColl.StandaloneTable(i)

        '                aName = pStTab.Name
        '                'Debug.Print(aName)
        '                'If aName = "TestStructure_Events" Then
        '                '    Debug.Print(aName)
        '                'End If
        '                If aName.ToUpper() = tableName.ToUpper() Then
        '                    pTable = pStTab.Table
        '                    Exit For
        '                End If
        '            Next
        '        End If
        '        Return pTable
        '    Catch ex As Exception
        '        strErr = ex.Message
        '        Return Nothing
        '    End Try
        'End Function
        Public Shared Function FindLayerByName(ByVal map As IMap, ByVal name As String) As ILayer
            Dim enumLayer As IEnumLayer = map.Layers
            Dim pLayer As ILayer = enumLayer.Next

            While Not pLayer Is Nothing
                If pLayer.Name.ToLower = name.ToLower Then
                    Return pLayer
                    Exit While
                End If
                pLayer = enumLayer.Next
            End While
            Return Nothing
        End Function
        Public Shared Function FindLayerIndex(ByVal mMap As IMap, ByVal mLayerName As String) As Integer
            Dim TLayer As ILayer
            Dim LayersEnum As IEnumLayer
            Dim Pos As Integer = 0

            FindLayerIndex = -1
            LayersEnum = mMap.Layers
            TLayer = LayersEnum.Next
            Do While Not TLayer Is Nothing
                If TLayer.Name.ToLower = mLayerName.ToLower Then
                    FindLayerIndex = Pos
                    Return FindLayerIndex
                End If
                Pos += 1
                TLayer = LayersEnum.Next
            Loop
            Return FindLayerIndex
        End Function
        Public Shared Function FindStandaloneTableByName(ByVal map As IMap, ByVal name As String) As IStandaloneTable
            Dim pStTblColl As IStandaloneTableCollection = map

            For i As Integer = 0 To pStTblColl.StandaloneTableCount - 1
                Dim stTbl As IStandaloneTable = pStTblColl.StandaloneTable(i)
                If stTbl.Name.ToLower = name.ToLower Then
                    Return stTbl
                    Exit For
                End If
            Next
            Return Nothing
        End Function
        Public Shared Function FindRasterLayer(ByVal map As IMap, ByVal name As String) As IRasterLayer
            Dim pLyr As ILayer = FindLayerByName(map, name)
            If pLyr Is Nothing Then
                Return Nothing
            End If
            Dim pResult As IRasterLayer = CType(pLyr, IRasterLayer)
            Return pResult
        End Function
        Public Shared Function GetDSNameOfFeaturelayer(ByVal pLayer As IFeatureLayer) As String
            Dim pDataLayer As IDataLayer
            Dim pDatasetName As IDatasetName

            pDataLayer = pLayer
            pDatasetName = pDataLayer.DataSourceName

            GetDSNameOfFeaturelayer = pDatasetName.WorkspaceName.PathName & "\" & pDatasetName.Name

        End Function
        Public Shared Function GetFeaturelayerByName(ByVal pMap As IMap, ByVal pStrname As String) As IFeatureLayer

            Dim lyrLst As List(Of ILayer) = GetLayers(pMap, True)
            For Each lyr As ILayer In lyrLst

                If TypeOf (lyr) Is IFeatureLayer Then
                    If lyr.Name.ToUpper() = pStrname.ToUpper() Then
                        Return lyr
                    End If

                End If

            Next

            Return Nothing

        End Function
        Public Shared Function GetFeatureLayerFromLayerIndexNumber(ByVal activeView As IActiveView, ByVal layerIndex As System.Int32) As IFeatureLayer
            If activeView Is Nothing OrElse layerIndex < 0 Then
                Return Nothing
            End If
            Dim map As IMap = activeView.FocusMap
            If layerIndex < map.LayerCount AndAlso TypeOf map.Layer(layerIndex) Is IFeatureLayer Then
                Return CType(activeView.FocusMap.Layer(layerIndex), IFeatureLayer) ' Explicit Cast
            Else
                Return Nothing
            End If
        End Function
        Public Shared Function GetLayers(ByVal doc As IMapDocument, Optional ByVal filterFeatureLayer As Boolean = True) As IList(Of ILayer)
            Dim allArray As List(Of ILayer) = New List(Of ILayer)()
            For index As Integer = 0 To doc.MapCount - 1
                Dim map As IMap = doc.Map(index)
                allArray.AddRange(GetLayers(map, filterFeatureLayer))
            Next
            Return allArray
        End Function

        Public Shared Function GetLayers(ByVal map As IMap, Optional ByVal filterFeatureLayer As Boolean = True) As IList(Of ILayer)
            Dim layerArray As List(Of ILayer) = New List(Of ILayer)()
            Dim enumLayer As IEnumLayer = map.Layers
            Dim pLayer As ILayer = enumLayer.Next
            While Not pLayer Is Nothing
                If filterFeatureLayer Then
                    If TypeOf pLayer Is IFeatureLayer Then

                        Dim featLyr As IFeatureLayer = CType(pLayer, IFeatureLayer)
                        If featLyr.FeatureClass IsNot Nothing Then
                            layerArray.Add(pLayer)
                        End If

                    End If
                Else
                    layerArray.Add(pLayer)
                End If
                pLayer = enumLayer.Next
            End While
            Return layerArray
        End Function

        Public Shared Function GetSelectedFeatures(ByVal featLyr As IFeatureLayer, Optional ByVal ifNoneGetAll As Boolean = True) As IList(Of IFeature)

            Dim lst As List(Of IFeature) = New List(Of IFeature)()
            Dim cursor As ICursor = Nothing
            Dim featureSelection As IFeatureSelection = CType(featLyr, IFeatureSelection)
            Dim selectionSet As ISelectionSet = featureSelection.SelectionSet
            If selectionSet.Count = 0 Then
                If ifNoneGetAll Then
                    cursor = featLyr.FeatureClass.Search(Nothing, False)
                End If
            Else
                selectionSet.Search(Nothing, False, cursor)
            End If

            If cursor IsNot Nothing Then
                Dim row As IRow = cursor.NextRow()
                While Not row Is Nothing
                    Dim feat As IFeature = CType(row, IFeature)
                    lst.Add(feat)
                    row = cursor.NextRow()
                End While
            End If
            Return lst
        End Function


        Public Shared Function GetSelectedFeaturesAsDataTable(ByVal featLyr As IFeatureLayer) As DataTable

            Dim desTbl As DataTable = GDB.ITableUtil.GetBlankDataTable(featLyr.FeatureClass)
            Dim fieldNamesToConvert As Dictionary(Of String, Integer) = GDB.ITableUtil.GetFieldNamesToQuery(featLyr.FeatureClass, Nothing)

            Dim selFeatLst As List(Of IFeature) = GetSelectedFeatures(featLyr, False)
            For Each feat As IFeature In selFeatLst

                Dim desRow As DataRow = desTbl.NewRow()
                GDB.ITableUtil.PopulateDatarow(feat, desRow, fieldNamesToConvert)
                desTbl.Rows.Add(desRow)

            Next

            Return desTbl

        End Function

        '***********
        'selected field value
        '***********
        Public Shared Function GetFieldValuesOfSelectedFeature(ByVal pLayer As IFeatureLayer, ByVal pFieldName As String) As List(Of String)
            Dim result As List(Of String) = New List(Of String)()
            Try
                Dim featureSelection As IFeatureSelection = CType(pLayer, IFeatureSelection)
                Dim selectionSet As ISelectionSet = featureSelection.SelectionSet
                Dim cursor As ICursor = Nothing

                If (pLayer.FeatureClass.FindField(pFieldName) = -1) Then Return Nothing

                selectionSet.Search(Nothing, False, cursor)
                Dim row As IRow
                Dim indexf As Integer
                row = cursor.NextRow()

                If row Is Nothing Then Return Nothing
                indexf = row.Fields.FindField(pFieldName)
                While Not row Is Nothing
                    Dim v As Object = row.Value(indexf)
                    If v IsNot Nothing Then
                        Dim str As String = Convert.ToString(v)
                        result.Add(str)
                    End If
                    row = cursor.NextRow()
                End While
            Catch ex As Exception
                Throw ex
            End Try
            Return result
        End Function
        Public Shared Sub TryDeleteLayer(ByVal map As IMap, ByVal lyrName As String)
            Dim lyr As ILayer = FindLayerByName(map, lyrName)
            If lyr IsNot Nothing Then
                map.DeleteLayer(lyr)
            End If
        End Sub

        Public Shared Function GetWorkspaceOfFeatureLyr(ByVal featLyr As IFeatureLayer) As IWorkspace
            Dim ds As ESRI.ArcGIS.Geodatabase.IDataset = CType(featLyr.FeatureClass, IDataset)
            Return ds.Workspace
        End Function

        Public Shared Function GetITableListName(ByRef pMap As IMap) As List(Of String) 'ArrayList

            Dim pStTabColl As IStandaloneTableCollection
            Dim i As Long
            'Dim pStTab As IStandaloneTable
            Dim aName As String
            'Dim pTable As ITable
            Dim tableNames As New List(Of String) 'ArrayList

            pStTabColl = pMap

            If (pStTabColl.StandaloneTableCount > 0) Then
                For i = 0 To pStTabColl.StandaloneTableCount - 1
                    aName = pStTabColl.StandaloneTable(i).Name
                    'aName = pStTab.Name
                    If Not tableNames.Contains(aName) Then
                        tableNames.Add(aName)
                    End If
                Next
            End If
            Return tableNames
        End Function
        Public Shared Function GetAllStandaloneTable(ByVal pMap As IMap) As IList(Of IStandaloneTable)

            Dim pStTabColl As IStandaloneTableCollection
            Dim i As Long
            Dim pStTab As IStandaloneTable
            'Dim pTable As ITable
            Dim tableList As New List(Of IStandaloneTable)
            pStTabColl = pMap

            If (pStTabColl.StandaloneTableCount > 0) Then
                For i = 0 To pStTabColl.StandaloneTableCount - 1
                    pStTab = pStTabColl.StandaloneTable(i)
                    tableList.Add(pStTab)
                Next
            End If
            Return tableList
        End Function

        Public Shared Function getLayerSelectedCount(ByVal pObj As Object, ByVal strLayerName As String) As Integer
            Try
                Dim pMap As IMap
                pMap = DirectCast(pObj, IMap)
                Dim pLayer As IFeatureLayer
                pLayer = GetFeaturelayerByName(pMap, strLayerName)
                If pLayer Is Nothing Then
                    Return 0
                Else
                    ' Dim pSelSet As ISelectionSet
                    Dim pFeatSelHouse As IFeatureSelection
                    pFeatSelHouse = pLayer
                    'pSelSet = pFeatSelHouse.SelectionSet
                    'Return pSelSet.Count
                    Return pFeatSelHouse.SelectionSet.Count
                End If

            Catch ex As Exception
                Return 0
            End Try
        End Function
        '**********
        'Report number of selected features according to the field value
        '**********
        Public Shared Function SetLayerSelection(ByVal pLayer As IFeatureLayer, _
                                  ByVal WhereClause As String, _
                                  ByRef selectionCount As Integer, Optional ByVal ResultAnd As Boolean = True) As Boolean
            'whereclause :"ROUTE_NAME = 'W151400_510TT' and ENDCUMSTA = 19.5"
            Try
                selectionCount = 0
                Dim pFeatSel As IFeatureSelection = pLayer
                If Not pFeatSel Is Nothing Then
                    Dim pFilter As IQueryFilter = New QueryFilter
                    pFilter.WhereClause = WhereClause
                    ' report number of selected features
                    If ResultAnd Then
                        pFeatSel.SelectFeatures(pFilter, esriSelectionResultEnum.esriSelectionResultAnd, False)
                    Else
                        pFeatSel.SelectFeatures(pFilter, esriSelectionResultEnum.esriSelectionResultNew, False)
                    End If
                    pFeatSel.SelectionChanged()
                    selectionCount = pFeatSel.SelectionSet.Count
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function
        '***
        '--- Function to Move layer in order
        '***
        Public Shared Sub MoveLayerToTheLast(ByVal pMap As IMap, ByVal strLyName As String)
            Try
                Dim pLayer As ILayer = Nothing
                Dim i As Integer
                For i = 0 To pMap.LayerCount - 1
                    pLayer = pMap.Layer(i)
                    If pLayer.Name.ToLower = strLyName.ToLower Then
                        Exit For
                    End If
                    pLayer = Nothing
                Next
                'Set pLayer = pMxDocument.SelectedLayer
                If pLayer IsNot Nothing Then
                    pMap.MoveLayer(pLayer, pMap.LayerCount - 1)
                End If
            Catch ex As Exception
            End Try
        End Sub
        Public Shared Sub zoom2Layer(ByVal pMap As IMapDocument, ByVal strLyName As String)
            Try
                Dim pLayer As ILayer = Nothing
                pLayer = FindLayerByName(pMap, strLyName)
                If Not pLayer Is Nothing Then
                    pMap.ActiveView.Extent = pLayer.AreaOfInterest
                    pMap.ActiveView.Refresh()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Shared Function GetLayerPath(ByVal pLayer As IFeatureLayer) As String
            Try

                Dim pFeatcls As IFeatureClass
                Dim pFeatLayer As IFeatureLayer
                Dim pWorkspace As IWorkspace

                pFeatLayer = pLayer

                pFeatcls = pFeatLayer.FeatureClass
                Dim pFeatDS As IDataset
                pFeatDS = pFeatcls
                pWorkspace = pFeatDS.Workspace
                If pWorkspace.PathName = "" Then
                    GetLayerPath = pWorkspace.ConnectionProperties.GetProperty("Database")
                Else
                    GetLayerPath = pWorkspace.PathName
                End If
                Exit Function
            Catch ex As Exception
                Return ""
            End Try
        End Function
#Region "from old maputil"
        'Get layer list or table list from IMap, return IFeature list of IStandAlone Table list
        Public Shared Function GetLayerByType(ByVal m_Map As IMap, ByVal fType As Integer) As ArrayList

            Dim myLayerOrTableList As ArrayList = New ArrayList()

            Try


                Dim pFL As IFeatureLayer
                Dim i As Integer


                Dim pL As ILayer
                For i = 0 To m_Map.LayerCount - 1
                    pL = m_Map.Layer(i)
                    If pL.Valid = False Then
                        Continue For
                    End If
                    If Not TypeOf pL Is IFeatureLayer Then
                        Continue For
                    End If


                    pFL = m_Map.Layer(i)
                    Select Case fType
                        Case 1
                            If pFL.FeatureClass.ShapeType = esriGeometryType.esriGeometryPoint Then
                                myLayerOrTableList.Add(pFL) 'pFL.Name)
                            End If
                        Case 2
                            If pFL.FeatureClass.ShapeType = esriGeometryType.esriGeometryPolyline Then
                                myLayerOrTableList.Add(pFL) 'pLayers.Add(pFL.Name)
                            End If
                        Case 3
                            If pFL.FeatureClass.ShapeType = esriGeometryType.esriGeometryPolygon Then
                                myLayerOrTableList.Add(pFL) 'pLayers.Add(pFL.Name)
                            End If
                    End Select
                Next

                If fType = 4 Then   'table
                    myLayerOrTableList = getTableList(m_Map)
                End If

            Catch ex As Exception

            End Try
            Return myLayerOrTableList
        End Function
        Public Shared Function GetTableList(ByVal pMap As IMap) As ArrayList

            Dim tableNames As New ArrayList

            Try
                Dim pStTabColl As IStandaloneTableCollection
                Dim i As Long
                Dim pStTab As IStandaloneTable
                Dim aName As String

                pStTabColl = pMap

                If (pStTabColl.StandaloneTableCount > 0) Then
                    For i = 0 To pStTabColl.StandaloneTableCount - 1
                        pStTab = pStTabColl.StandaloneTable(i)

                        If pStTab.Valid Then
                            aName = pStTab.Name
                            If Not tableNames.Contains(aName) Then
                                'tableNames.Add(aName)
                                tableNames.Add(pStTab)
                            End If
                        End If

                    Next
                End If
            Catch ex As Exception

            End Try
            Return tableNames
        End Function
        Public Shared Function GetLayerOrTableFromList(ByVal myList As ArrayList, ByVal myLayerName As String) As Object
            Try
                For Each o As Object In myList
                    If TypeOf o Is IFeatureLayer Then
                        If DirectCast(o, IFeatureLayer).Name.ToLower = myLayerName.ToLower Then
                            Return o
                        End If
                    End If
                    If TypeOf o Is IStandaloneTable Then
                        If DirectCast(o, IStandaloneTable).Name.ToLower = myLayerName.ToLower Then
                            Return o
                        End If
                    End If
                Next
                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Shared Function getFieldNameListByType(ByVal pLayerOrTable As Object, _
                      Optional ByVal fieldType As String = "") As ArrayList

            Dim i As Integer
            Dim fieldNames As New ArrayList
            Dim pFields As IFields
            Dim strFields As String = ""
            If TypeOf pLayerOrTable Is IFeatureLayer Then
                Dim pLayer As IFeatureLayer = DirectCast(pLayerOrTable, IFeatureLayer)
                pFields = pLayer.FeatureClass.Fields
            Else 'table
                Dim pTable As ITable = DirectCast(pLayerOrTable, ITable)
                pFields = pTable.Fields
            End If

            Dim pField As IField
            Dim theT As String = ""
            For i = 0 To pFields.FieldCount - 1
                pField = pFields.Field(i)
                theT = ""
                Select Case pField.Type
                    Case esriFieldType.esriFieldTypeBlob
                    Case esriFieldType.esriFieldTypeDate
                        theT = "D"
                    Case esriFieldType.esriFieldTypeDouble
                        theT = "N"
                    Case esriFieldType.esriFieldTypeGeometry
                    Case esriFieldType.esriFieldTypeGlobalID
                    Case esriFieldType.esriFieldTypeGUID
                    Case esriFieldType.esriFieldTypeInteger
                        theT = "N"
                    Case esriFieldType.esriFieldTypeOID
                    Case esriFieldType.esriFieldTypeRaster
                    Case esriFieldType.esriFieldTypeSingle
                        theT = "N"
                    Case esriFieldType.esriFieldTypeSmallInteger
                        theT = "N"
                    Case esriFieldType.esriFieldTypeString
                        theT = "S"
                End Select

                If fieldType = "" Then
                    If theT <> "" Then
                        fieldNames.Add(pFields.Field(i).Name)
                    End If
                Else
                    If theT = fieldType.ToUpper Then
                        fieldNames.Add(pFields.Field(i).Name)
                    End If
                End If


            Next

            fieldNames.Sort()

            Return fieldNames
        End Function
#End Region
    End Class

End Namespace





