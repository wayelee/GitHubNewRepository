Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Carto
Imports System.Runtime.InteropServices


Namespace Location




    Public Class RouteUtil


        Private Shared Function CreateRouteLocatorName(ByVal routeFC As IFeatureClass, ByVal routeFC_RouteIDFieldName As String) As IRouteLocatorName

            '+++ Set up a RouteMeasureLocatorName (used by the RouteEventSourceName)
            Dim routeDS As IDataset = routeFC
            Dim routeDSName As IName = routeDS.FullName
            Dim pRMLocName As IRouteLocatorName = New RouteMeasureLocatorName()
            With pRMLocName
                .RouteFeatureClassName = routeDSName
                .RouteIDFieldName = routeFC_RouteIDFieldName '"Pipeline_ID"
                .RouteIDIsUnique = True
                .RouteMeasureUnit = esriUnits.esriFeet
            End With

            Return pRMLocName

        End Function

        Private Shared Function CreateINameFromITable(ByVal pEventTable As ITable) As IDatasetName
            Dim eventTblDS As IDataset = pEventTable
            Dim eventTblDSName As IName = eventTblDS.FullName
            Dim eventTblDataSetName As IDatasetName = eventTblDSName
            Return eventTblDataSetName
        End Function

        Private Shared Function CreateIRouteMeasureLineProperties(ByVal eventTbl_RouteIDFieldName As String, ByVal eventTbl_BegStationFieldName As String, _
                                                                 ByVal eventTbl_EndStationFieldName As String) As IRouteMeasureLineProperties
            '+++ Create some event properties.(used by the RouteEventSourceName)
            '++ For point events, you use RouteMeasurePointProperties. For line events, you would use RouteMeasureLineProperties
            Dim eventProp As IRouteEventProperties = New RouteMeasureLineProperties
            With eventProp
                .EventMeasureUnit = esriUnits.esriFeet
                .EventRouteIDFieldName = eventTbl_RouteIDFieldName
                '.LateralOffsetFieldName = "offset"
            End With

            Dim eventMLProp As IRouteMeasureLineProperties = eventProp
            eventMLProp.FromMeasureFieldName = eventTbl_BegStationFieldName
            eventMLProp.ToMeasureFieldName = eventTbl_EndStationFieldName '"EndCumSta"
            Return eventMLProp

        End Function

        Private Shared Function CreateOutputFCName(ByVal featWS As IFeatureWorkspace, ByVal outputFCName As String) As IFeatureClassName

            Dim ds As IDataset = featWS

            Dim srsWsName As IWorkspaceName = CType(ds.FullName, IWorkspaceName)

            '+++ Create the new output FeatureClassName object
            Dim pOutFCN As IFeatureClassName = New FeatureClassName()
            Dim pOutDSN2 As IDatasetName = pOutFCN
            pOutDSN2.WorkspaceName = srsWsName 'eventTblDS.FullName 'esp. necessary when pOutDSN is Nothing
            pOutDSN2.Name = outputFCName 'pTempDs.BrowseName & "_Events"

            Return pOutFCN

        End Function

        Private Shared Function CheckAndGetFields(ByVal eventSourceName As IRouteEventSourceName, ByVal featWS As IFeatureWorkspace) As IFields

            '+++ Open the eventsource to get a feature class. The fields of this event source
            '+++ will be run through the field checker.
            Dim tempName As IName = eventSourceName
            Dim pEvtSrc As IRouteEventSource = tempName.Open()
            Dim pFC As IFeatureClass = pEvtSrc
            '+++ Get the event source's fields and run them through the field checker
            Dim pFlds As IFields = pFC.Fields
            Dim pFldChk As IFieldChecker = New FieldChecker
            ' Dim wsName As IName = srsWsName
            Dim pTempWS As IWorkspace = featWS 'wsName.Open
            pFldChk.ValidateWorkspace = pTempWS
            Dim pOutFlds As IFields = Nothing
            pFldChk.Validate(pFlds, Nothing, pOutFlds)
            If Not pFlds.FieldCount = pOutFlds.FieldCount Then
                Dim msg As String = "The number of fields returned by the field checker is less than the input." _
                   & vbCrLf & "Cannot create output feature class"
                Throw New Exception(msg)
            End If

            Return pOutFlds

        End Function

        Private Shared Function CreateIQueryFilter(ByVal queryStr As String) As IQueryFilter
            '+++ Optional: Use a query filter
            Dim pQFilt As IQueryFilter = New QueryFilter()
            'pQFilt.WhereClause = "rkey > 200"
            If (Trim(Len(queryStr)) > 0) Then
                pQFilt = New QueryFilter
                pQFilt.WhereClause = queryStr
            Else
                pQFilt = Nothing
            End If
            Return pQFilt
        End Function


        Private Shared Function Create(ByVal routeFC As IFeatureClass, ByVal routeFC_RouteIDFieldName As String, _
                                       ByVal pEventTable As ITable, ByVal eventTbl_RouteIDFieldName As String, ByVal eventTbl_BegStationFieldName As String, ByVal eventTbl_EndStationFieldName As String)



            Dim pRMLocName As IRouteLocatorName = CreateRouteLocatorName(routeFC, routeFC_RouteIDFieldName)
            Dim eventTblDSName As IName = CreateINameFromITable(pEventTable)
            Dim eventMLProp As IRouteMeasureLineProperties = CreateIRouteMeasureLineProperties(eventTbl_RouteIDFieldName, eventTbl_BegStationFieldName, eventTbl_EndStationFieldName)


            '+++ Set up the RouteEventSourceName
            Dim eventSourceName As IRouteEventSourceName = New RouteEventSourceName()
            With eventSourceName
                .EventProperties = eventMLProp
                .EventTableName = eventTblDSName
                .RouteLocatorName = pRMLocName
            End With

            Return eventSourceName

        End Function

        Private Shared Function GetErrorInformation(ByVal pEnum As IEnumInvalidObject) As String

            Dim msg As String = String.Empty

            '+++ Code to make sure pEnumvalidObject does not reject any features
            pEnum.Reset()
            Dim pInvalidInfo As IInvalidObjectInfo = pEnum.Next
            While Not pInvalidInfo Is Nothing
                '    Debug.Print(pInvalidInfo.InvalidObjectID & ": " & pInvalidInfo.ErrorDescription)
                pInvalidInfo = pEnum.Next
                msg = msg & pInvalidInfo.InvalidObjectID & ": " & pInvalidInfo.ErrorDescription & ";" & vbNewLine
            End While

            Return msg

        End Function

        Public Shared Function ConvertLinearEventsToFC(ByVal featWS As IFeatureWorkspace, ByVal routeFC As IFeatureClass, ByVal eventTblName As String, _
                                                       ByVal queryStr As String, ByVal outputFCName As String, _
                                                 ByVal routeFC_RouteIDFieldName As String, ByVal eventTbl_BegStationFieldName As String, ByVal eventTbl_EndStationFieldName As String, _
                                                 Optional ByVal eventTbl_RouteIDFieldName As String = "") As String

            If String.IsNullOrEmpty(eventTbl_RouteIDFieldName) Then
                eventTbl_RouteIDFieldName = routeFC_RouteIDFieldName
            End If


            Dim pEventTable As ITable = featWS.OpenTable(eventTblName)

            Dim eventSourceName As IRouteEventSourceName = Create(routeFC, routeFC_RouteIDFieldName, pEventTable, eventTbl_RouteIDFieldName, _
                                                                 eventTbl_BegStationFieldName, eventTbl_EndStationFieldName)

            Dim pOutFCN As IFeatureClassName = CreateOutputFCName(featWS, outputFCName)
            Dim pOutFlds As IFields = CheckAndGetFields(eventSourceName, featWS)
            Dim pQFilt As IQueryFilter = CreateIQueryFilter(queryStr)



            Dim pOutFeatDSN As IFeatureDatasetName = Nothing '= pOutFCN
            '+++ Convert the RouteEventSourceName
            Dim pConv As IFeatureDataConverter2 = New FeatureDataConverter
            Dim pEnum As IEnumInvalidObject = pConv.ConvertFeatureClass(eventSourceName, pQFilt, Nothing, pOutFeatDSN, _
                                   pOutFCN, Nothing, pOutFlds, "", 1000, 0)


            Dim msg As String = GetErrorInformation(pEnum)
            Return msg

        End Function


        '***
        '--- Function to create featureclass from event table
        '***
        Public Shared Function ConvertEventsToFC(ByVal featWS As IFeatureWorkspace, _
                                    ByVal pRouteFC As IFeatureClass, _
                                    ByVal EventTableName As String, _
                                    ByVal QueryString As String, _
                                    ByVal OutputName As String, _
                                    ByVal routeIdFldName As String, _
                                    ByVal begStaFldName As String, _
                                    ByVal endStaFldName As String) As Boolean
            Try



                Dim pWS As IWorkspace
                Dim pWorkspaceFactory As IWorkspaceFactory
                Dim pFWS As IFeatureWorkspace
                Dim pEventTable As ITable
                Dim pTempWS As IWorkspace
                Dim pTempDSN As IDatasetName
                Dim pTempDs As IDataset
                Dim pTempGDS As IGeoDataset
                Dim pTempName As IName
                Dim pEnumDSN As IEnumDatasetName
                Dim pTempFC As IFeatureClass
                Dim pTempFWS As IFeatureWorkspace
                Dim pEventTableName As IDatasetName
                Dim pRtProp As IRouteEventProperties
                Dim pRMPtProp As IRouteMeasureLineProperties
                Dim pEventsourceName As IRouteEventSourceName
                Dim pOutDSN As IDatasetName
                Dim pOutWSN As IWorkspaceName
                Dim pOutFeatDSN As IFeatureDatasetName = Nothing
                Dim pOutFCN As IFeatureClassName
                Dim pOutDSN2 As IDatasetName
                Dim pFC As IFeatureClass
                Dim pEvtSrc As IRouteEventSource
                Dim pFlds As IFields
                Dim pOutFlds As IFields = Nothing
                Dim pFldChk As IFieldChecker
                Dim pQFilt As IQueryFilter
                Dim pEnum As IEnumInvalidObject
                Dim pConv As IFeatureDataConverter2
                Dim pInvalidInfo As IInvalidObjectInfo
                Dim pRMLocName As IRouteLocatorName


                'pWorkspaceFactory = New AccessWorkspaceFactory
                'pWS = pWorkspaceFactory.OpenFromFile(strMDBPath, 0)
                'pFWS = pWS
                pFWS = featWS
                pEventTable = pFWS.OpenTable(EventTableName)

                '+++ Set up a RouteMeasureLocatorName (to be passed to RouteEventSourceName)
                pTempDs = pRouteFC
                pTempName = pTempDs.FullName
                pRMLocName = New RouteMeasureLocatorName
                With pRMLocName
                    .RouteFeatureClassName = pTempName
                    .RouteIDFieldName = routeIdFldName '"Pipeline_ID"
                    .RouteIDIsUnique = True
                    .RouteMeasureUnit = esriUnits.esriFeet
                End With
                '+++ Create the input EventTableName (used by the RouteEventSourceName)
                pTempDs = pEventTable
                pTempName = pTempDs.FullName
                pEventTableName = pTempName
                '+++ Create some event properties. For point events,
                '+++ you use RouteMeasurePointProperties. For line events,
                '+++ you would use RouteMeasureLineProperties

                pRtProp = New RouteMeasureLineProperties
                With pRtProp
                    .EventMeasureUnit = esriUnits.esriFeet
                    .EventRouteIDFieldName = routeIdFldName
                    '.LateralOffsetFieldName = "offset"
                End With
                pRMPtProp = pRtProp
                pRMPtProp.FromMeasureFieldName = begStaFldName '"BegCumSta"
                pRMPtProp.ToMeasureFieldName = endStaFldName '"EndCumSta"
                '+++ Set up the RouteEventSourceName
                pEventsourceName = New RouteEventSourceName
                With pEventsourceName
                    .EventProperties = pRMPtProp
                    .EventTableName = pEventTableName
                    .RouteLocatorName = pRMLocName
                End With
                '+++ We'll write the results out to the same workspace as the event table
                pTempDs = pEventTable
                pTempWS = pTempDs.Workspace
                pOutWSN = New WorkspaceName
                pOutWSN.ConnectionProperties = pTempWS.ConnectionProperties
                If pTempWS.Type = esriWorkspaceType.esriRemoteDatabaseWorkspace Then
                    pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesGDB.SdeWorkspaceFactory.1"
                ElseIf pTempWS.Type = esriWorkspaceType.esriLocalDatabaseWorkspace Then
                    pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory.1"
                Else
                    pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory.1"
                End If
                '+++ Create the new output FeatureClassName object
                pOutFCN = New FeatureClassName
                pOutDSN2 = pOutFCN
                pOutDSN2.WorkspaceName = pOutWSN 'esp. necessary when pOutDSN is Nothing
                pTempDs = pEventTable
                pOutDSN2.Name = OutputName 'pTempDs.BrowseName & "_Events"
                '+++ Open the eventsource to get a feature class. The fields of this event source
                '+++ will be run through the field checker.
                pTempName = pEventsourceName
                pEvtSrc = pTempName.Open
                pFC = pEvtSrc
                '+++ Get the event source's fields and run them through the field checker
                pFlds = pFC.Fields
                pFldChk = New FieldChecker
                pTempName = pOutWSN
                pTempWS = pTempName.Open
                pFldChk.ValidateWorkspace = pTempWS
                pFldChk.Validate(pFlds, Nothing, pOutFlds)
                If Not pFlds.FieldCount = pOutFlds.FieldCount Then
                    MsgBox("The number of fields returned by the field checker is less than the input." _
                       & vbCrLf & "Cannot create output feature class", vbExclamation, "ConvertEvents")
                    Return False
                End If
                '+++ Optional: Use a query filter
                'Set pQFilt = New QueryFilter
                'pQFilt.WhereClause = "rkey > 200"
                If (Trim(Len(QueryString)) > 0) Then
                    pQFilt = New QueryFilter
                    pQFilt.WhereClause = QueryString
                Else : pQFilt = Nothing
                End If
                '+++ Convert the RouteEventSourceName
                pConv = New FeatureDataConverter
                pEnum = pConv.ConvertFeatureClass(pEventsourceName, pQFilt, Nothing, pOutFeatDSN, _
                                       pOutFCN, Nothing, pOutFlds, "", 1000, 0)
                '+++ Code to make sure pEnumvalidObject does not reject any features
                pEnum.Reset()
                pInvalidInfo = pEnum.Next
                While Not pInvalidInfo Is Nothing
                    'Debug.Print pInvalidInfo.InvalidObjectID & ": " & pInvalidInfo.ErrorDescription
                    pInvalidInfo = pEnum.Next
                End While
                '            GoTo endproc
                'err_H:
                '            Dim lNum As Long, sDesc As String, sSrc As String
                '            lNum = Err.Number
                '            sDesc = Err.Description

                'endproc:
                pWS = Nothing
                pFWS = Nothing
                pWorkspaceFactory = Nothing
                pTempWS = Nothing
                pTempDSN = Nothing
                pTempDs = Nothing
                pTempGDS = Nothing
                pTempName = Nothing
                pEnumDSN = Nothing
                pTempFC = Nothing
                pTempFWS = Nothing
                pRMLocName = Nothing
                pEventTableName = Nothing
                pEventsourceName = Nothing
                pOutDSN = Nothing
                pOutWSN = Nothing
                pOutFeatDSN = Nothing
                pOutFCN = Nothing
                pOutDSN2 = Nothing
                pFC = Nothing
                pEvtSrc = Nothing
                pFlds = Nothing
                pOutFlds = Nothing
                pQFilt = Nothing
                pFldChk = Nothing
                pEnum = Nothing
                pConv = Nothing


                Return True

            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function GetPointByMeasure(ByVal routeFC As IFeatureClass, ByVal routeID As Object, ByVal measure As Double, ByVal routeIDFieldName As String) As IPoint


            Dim routeLocation As IRouteLocation = New RouteMeasurePointLocation()
            routeLocation.MeasureUnit = esriUnits.esriFeet
            routeLocation.RouteID = routeID
            routeLocation.LateralOffset = 0


            Dim pRMPointLoc As IRouteMeasurePointLocation
            pRMPointLoc = routeLocation
            pRMPointLoc.Measure = measure


            Dim tempDS As IDataset = CType(routeFC, IDataset)
            Dim tempName As IName = CType(tempDS.FullName, IName)

            Dim routeLocName As IRouteLocatorName = New RouteMeasureLocatorName()
            routeLocName.RouteFeatureClassName = tempName
            routeLocName.RouteIDFieldName = routeIDFieldName
            routeLocName.RouteMeasureUnit = esriUnits.esriFeet

            tempName = CType(routeLocName, IName)
            Dim routeLoc As IRouteLocator2
            routeLoc = CType(tempName.Open(), IRouteLocator2)

            Dim geom As IGeometry = Nothing
            Dim err As esriLocatingError
            routeLoc.Locate(routeLocation, geom, err)

            If geom Is Nothing Then
                Return Nothing
            End If

            If TypeOf (geom) Is IMultipoint Then

                Dim multiPt As IMultipoint = CType(geom, IMultipoint)
                Dim ptCol As IPointCollection = CType(multiPt, IPointCollection)

                If ptCol.PointCount > 0 Then
                    Return ptCol.Point(0)
                End If

            ElseIf TypeOf (geom) Is IPoint Then
                Return geom
            End If

            Return Nothing

        End Function

        Public Shared Function GetSubPolylineByMeasure(ByVal routeFC As IFeatureClass, ByVal routeID As Object, _
                                                       ByVal begM As Double, ByVal endM As Double, ByVal routeIDFieldName As String) As IPolyline


            Dim routeLocation As IRouteLocation = New RouteMeasureLineLocation()
            routeLocation.MeasureUnit = esriUnits.esriFeet
            routeLocation.RouteID = routeID
            routeLocation.LateralOffset = 0


            Dim pRMLineLoc As IRouteMeasureLineLocation
            pRMLineLoc = routeLocation
            pRMLineLoc.FromMeasure = begM
            pRMLineLoc.ToMeasure = endM


            Dim tempDS As IDataset = CType(routeFC, IDataset)
            Dim tempName As IName = CType(tempDS.FullName, IName)

            Dim routeLocName As IRouteLocatorName = New RouteMeasureLocatorName()
            routeLocName.RouteFeatureClassName = tempName
            routeLocName.RouteIDFieldName = routeIDFieldName
            routeLocName.RouteMeasureUnit = esriUnits.esriFeet

            tempName = CType(routeLocName, IName)
            Dim routeLoc As IRouteLocator2
            routeLoc = CType(tempName.Open(), IRouteLocator2)

            Dim geom As IGeometry = Nothing
            Dim err As esriLocatingError
            routeLoc.Locate(routeLocation, geom, err)

            If geom Is Nothing Then
                Return Nothing
            End If

            If TypeOf (geom) Is IPolyline Then

                Return CType(geom, IPolyline)

            End If

            Return Nothing



        End Function


        Public Shared Function LocatePolygonsAlongRoutes_ArcInfo(ByVal pRouteFC As IFeatureClass, _
                                                    ByVal polygonFC As IFeatureClass, _
                                                    ByVal strEventTblName As String, _
                                                    ByVal pWS As IWorkspace, _
                                                    ByVal strRouteIDFld As String, _
                                                    ByVal strBegStaFld As String, _
                                                    ByVal strEndStaFld As String) As Boolean
            Dim pWSF As IWorkspaceFactory
            Dim pOutTable As ITable

            Try
                'Get the polygon feature class and the route feature class. We'll assume that they come from the same workspace.
                Dim pFWS As IFeatureWorkspace = pWS
                pFWS = pWS

                'Set up a RouteMeasureLocator object.
                Dim pTempName As IName
                Dim pTempDs As IDataset
                Dim pRMLocName As IRouteLocatorName
                Dim pRtLoc As IRouteLocator
                pTempDs = pRouteFC
                pTempName = pTempDs.FullName
                pRMLocName = New RouteMeasureLocatorName
                With pRMLocName
                    .RouteFeatureClassName = pTempName
                    .RouteIDFieldName = strRouteIDFld
                    .RouteIDIsUnique = True
                End With
                pTempName = pRMLocName
                pRtLoc = pTempName.Open

                'Create an output table name object. We'll write to the same workspace as the input routes and polygons
                Dim pOutDSN As IDatasetName
                Dim pOutWSN As IWorkspaceName
                pTempDs = pWS
                pOutWSN = pTempDs.FullName
                pOutDSN = New TableName
                pOutDSN.WorkspaceName = pOutWSN
                pOutDSN.Name = strEventTblName

                'Create RouteLocatorOperations object. Note that you can use a selection set of polygons. If you want to do this,
                'set the InputFeatureSelection property instead of the InputFeatureClass property.
                Dim pRouteLocOps As IRouteLocatorOperations
                pRouteLocOps = New RouteLocatorOperations
                With pRouteLocOps
                    .RouteLocator = pRtLoc
                    .InputFeatureClass = polygonFC
                End With

                'Set event properties for the output line event table. The field names specified will be written to the output table.
                Dim pEventProps As IRouteEventProperties
                Dim pRMLineProps As IRouteMeasureLineProperties
                pEventProps = New RouteMeasureLineProperties
                pEventProps.EventRouteIDFieldName = strRouteIDFld
                pRMLineProps = pEventProps
                pRMLineProps.FromMeasureFieldName = strBegStaFld
                pRMLineProps.ToMeasureFieldName = strEndStaFld

                'Locate the polygons along the routes
                Dim bKeepZero As Boolean
                Dim bKeepAllFields As Boolean
                bKeepZero = False 'do not keep events where FROM_M = TO_M
                bKeepAllFields = True 'keep all of the input polygon feature class's attributes
                pOutTable = pRouteLocOps.LocatePolygonFeatures(pEventProps, bKeepAllFields, bKeepZero, pOutDSN, "", Nothing)
                Return True
            Catch COMEx As COMException
                ''msgbox(COMEx.Message, "COM Error: " + COMEx.ErrorCode.ToString())
                Return False
            Catch SysEx As System.Exception
                ''msgbox(SysEx.Message, ".NET Error: ")
                Return False
            Finally
                pOutTable = Nothing
                pWSF = Nothing
            End Try
        End Function



        Public Shared Function ConvertPtEventsToFC(ByVal pFWS As IFeatureWorkspace, _
                                 ByVal pRouteFC As IFeatureClass, _
                                             ByVal EventTableName As String, _
                                             ByVal QueryString As String, _
                                             ByVal OutputName As String, _
                                             ByVal routeIdFldName As String, _
                                             ByVal begStaFldName As String, _
                                             Optional ByVal evRidFldName As String = "", _
                                             Optional ByVal onlyValidShape As Boolean = True) As Boolean



            Dim pEventTable As ITable = Nothing
            Dim pTempWS As IWorkspace = Nothing
            Dim pTempDSN As IDatasetName = Nothing
            Dim pTempDs As IDataset = Nothing
            Dim pTempGDS As IGeoDataset = Nothing
            Dim pTempName As IName = Nothing
            Dim pEnumDSN As IEnumDatasetName = Nothing
            Dim pTempFC As IFeatureClass = Nothing
            Dim pTempFWS As IFeatureWorkspace = Nothing
            Dim pEventTableName As IDatasetName = Nothing
            Dim pRtProp As IRouteEventProperties2 = Nothing
            Dim pRMPtProp As IRouteMeasurePointProperties2 = Nothing
            Dim pEventsourceName As IRouteEventSourceName = Nothing
            Dim pOutDSN As IDatasetName = Nothing
            Dim pOutWSN As IWorkspaceName = Nothing
            Dim pOutFeatDSN As IFeatureDatasetName = Nothing
            Dim pOutFCN As IFeatureClassName = Nothing
            Dim pOutDSN2 As IDatasetName = Nothing
            Dim pFC As IFeatureClass = Nothing
            Dim pEvtSrc As IRouteEventSource = Nothing
            Dim pFlds As IFields = Nothing
            Dim pOutFlds As IFields = Nothing
            Dim pFldChk As IFieldChecker = Nothing
            Dim pQFilt As IQueryFilter = Nothing
            Dim pEnum As IEnumInvalidObject = Nothing
            Dim pConv As IFeatureDataConverter2 = Nothing
            Dim pInvalidInfo As IInvalidObjectInfo = Nothing
            Dim pRMLocName As IRouteLocatorName = Nothing

            Try

                pEventTable = pFWS.OpenTable(EventTableName)

                '+++ Set up a RouteMeasureLocatorName (to be passed to RouteEventSourceName)
                pTempDs = pRouteFC
                pTempName = pTempDs.FullName
                pRMLocName = New RouteMeasureLocatorName
                With pRMLocName
                    .RouteFeatureClassName = pTempName
                    .RouteIDFieldName = routeIdFldName
                    .RouteIDIsUnique = True
                    .RouteMeasureUnit = esriUnits.esriFeet

                End With
                '+++ Create the input EventTableName (used by the RouteEventSourceName)
                pTempDs = pEventTable
                pTempName = pTempDs.FullName
                pEventTableName = pTempName
                '+++ Create some event properties. For point events,
                '+++ you use RouteMeasurePointProperties. For line events,
                '+++ you would use RouteMeasureLineProperties

                pRtProp = New RouteMeasurePointProperties
                With pRtProp
                    .EventMeasureUnit = esriUnits.esriFeet
                    .AddErrorField = True
                    .ErrorFieldName = "LOC_ERR"
                    If evRidFldName = "" Then
                        .EventRouteIDFieldName = routeIdFldName
                    Else
                        .EventRouteIDFieldName = evRidFldName
                    End If

                End With
                pRMPtProp = pRtProp
                pRMPtProp.MeasureFieldName = begStaFldName

                '+++ Set up the RouteEventSourceName
                pEventsourceName = New RouteEventSourceName
                With pEventsourceName
                    .EventProperties = pRMPtProp
                    .EventTableName = pEventTableName
                    .RouteLocatorName = pRMLocName
                End With
                '+++ We'll write the results out to the same workspace as the event table
                pTempDs = pEventTable
                pTempWS = pTempDs.Workspace
                pOutWSN = New WorkspaceName
                pOutWSN.ConnectionProperties = pTempWS.ConnectionProperties
                If pTempWS.Type = esriWorkspaceType.esriRemoteDatabaseWorkspace Then
                    pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesGDB.SdeWorkspaceFactory.1"
                ElseIf pTempWS.Type = esriWorkspaceType.esriLocalDatabaseWorkspace Then
                    Dim path As String = pTempWS.PathName
                    If path.ToUpper().EndsWith(".mdb".ToUpper) Then
                        pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory.1"
                    Else
                        pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesGDB.FileGDBWorkspaceFactory.1"
                    End If

                ElseIf pTempWS.Type = esriWorkspaceType.esriFileSystemWorkspace Then
                    pOutWSN.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory.1"
                End If
                '+++ Create the new output FeatureClassName object
                pOutFCN = New FeatureClassName
                pOutDSN2 = pOutFCN
                pOutDSN2.WorkspaceName = pOutWSN 'esp. necessary when pOutDSN is Nothing
                pTempDs = pEventTable
                pOutDSN2.Name = OutputName 'pTempDs.BrowseName & "_Events"
                '+++ Open the eventsource to get a feature class. The fields of this event source
                '+++ will be run through the field checker.
                pTempName = pEventsourceName
                pEvtSrc = pTempName.Open
                pFC = pEvtSrc
                '+++ Get the event source's fields and run them through the field checker
                pFlds = pFC.Fields
                pFldChk = New FieldChecker
                pTempName = pOutWSN
                pTempWS = pTempName.Open
                pFldChk.ValidateWorkspace = pTempWS
                pFldChk.Validate(pFlds, Nothing, pOutFlds)
                If Not pFlds.FieldCount = pOutFlds.FieldCount Then
                    MsgBox("The number of fields returned by the field checker is less than the input." _
                       & vbCrLf & "Cannot create output feature class", vbExclamation, "ConvertEvents")
                    Return False
                End If
                '+++ Optional: Use a query filter
                'Set pQFilt = New QueryFilter
                'pQFilt.WhereClause = "rkey > 200"
                If onlyValidShape Then
                    If (Trim(Len(QueryString)) > 0) Then
                        QueryString = QueryString & " and LOC_ERR ='NO ERROR'"
                    Else
                        QueryString = "LOC_ERR ='NO ERROR'"
                    End If
                End If


                If (Trim(Len(QueryString)) > 0) Then
                    pQFilt = New QueryFilter
                    pQFilt.WhereClause = QueryString
                Else : pQFilt = Nothing
                End If
                '+++ Convert the RouteEventSourceName
                pConv = New FeatureDataConverter


                'Dim pQFilt As IQueryFilter
                'pQFilt = New QueryFilter
                'pQFilt.WhereClause = "Shape_Area > 77000"    'use the query filter to select features
                'Dim pSelectionSet As ISelectionSet
                'pSelectionSet = pFeatcls.Select(pQFilt, esriSelectionTypeIDSet, _
                '                esriSelectionOptionNormal, pScratchWorkspace)




                pEnum = pConv.ConvertFeatureClass(pEventsourceName, pQFilt, Nothing, pOutFeatDSN, _
                                       pOutFCN, Nothing, pOutFlds, "", 1000, 0)
                '+++ Code to make sure pEnumvalidObject does not reject any features
                pEnum.Reset()
                pInvalidInfo = pEnum.Next
                While Not pInvalidInfo Is Nothing
                    'Debug.Print pInvalidInfo.InvalidObjectID & ": " & pInvalidInfo.ErrorDescription
                    pInvalidInfo = pEnum.Next
                End While

                Return True
                '            GoTo endproc
                'eh:
                '            Dim lNum As Long, sDesc As String, sSrc As String
                '            lNum = Err.Number
                '            sDesc = Err.Description
                '            sSrc = Err.Source

            Catch ex As Exception
                Return False
            Finally

                pTempWS = Nothing
                pTempDSN = Nothing
                pTempDs = Nothing
                pTempGDS = Nothing
                pTempName = Nothing
                pEnumDSN = Nothing
                pTempFC = Nothing
                pTempFWS = Nothing
                pRMLocName = Nothing
                pEventTableName = Nothing
                pEventsourceName = Nothing
                pOutDSN = Nothing
                pOutWSN = Nothing
                pOutFeatDSN = Nothing
                pOutFCN = Nothing
                pOutDSN2 = Nothing
                pFC = Nothing
                pEvtSrc = Nothing
                pFlds = Nothing
                pOutFlds = Nothing
                pQFilt = Nothing
                pFldChk = Nothing
                pEnum = Nothing
                pConv = Nothing

            End Try
        End Function




    End Class


End Namespace