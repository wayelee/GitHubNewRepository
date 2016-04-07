
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.NetworkAnalysis
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.EditorExt


Namespace Network


    Public Class FlowTracer
        Inherits GeometricNetworkTool


        Private _FlowMethod As esriFlowMethod = esriFlowMethod.esriFMDownstream
        Public Property FlowMethod() As esriFlowMethod
            Get
                Return _FlowMethod
            End Get
            Set(ByVal value As esriFlowMethod)
                _FlowMethod = value
            End Set
        End Property


        Public Sub New(ByVal map As IMap, ByVal featureDS As IFeatureDataset)

            MyBase.New(map, featureDS)

        End Sub



#Region "Public Functions"

        Public Function FindSourceJunctions(ByVal inputPtList As List(Of IPoint), ByVal map As IMap, ByVal dRWSearchTolerance As Double) As List(Of IFeature)


            Dim traceFlowSolver As ITraceFlowSolverGEN = New TraceFlowSolver()
            Dim netSolver As INetSolver = CType(traceFlowSolver, INetSolver)
            netSolver.SourceNetwork = _GeometricNetwork

            Dim junctionFlagArray As IJunctionFlag() = GetNearestJunctionList(inputPtList).ToArray()
            traceFlowSolver.PutJunctionOrigins(junctionFlagArray)

            traceFlowSolver.TraceIndeterminateFlow = True

            Dim junctionEIDs As IEnumNetEID = Nothing
            Dim edgeEIDs As IEnumNetEID = Nothing
            Dim vaRes As Object() = New Object(junctionFlagArray.Count - 1) {}
            traceFlowSolver.FindSource(_FlowMethod, esriShortestPathObjFn.esriSPObjFnMinMax, junctionEIDs, edgeEIDs, junctionFlagArray.Count - 1, vaRes)


            Return GetFeaturesFromEIDNetEnum(junctionEIDs)

        End Function


        Public Function FindSourceEdges(ByVal inputPtList As List(Of IPoint)) As List(Of IFeature)



            Dim traceFlowSolver As ITraceFlowSolverGEN = New TraceFlowSolver()
            Dim netSolver As INetSolver = CType(traceFlowSolver, INetSolver)
            netSolver.SourceNetwork = _GeometricNetwork.Network

            Dim edgeFlagArray As IEdgeFlag() = GetNearestEdgeList(inputPtList).ToArray()
            If edgeFlagArray.Count = 0 Then
                Return New List(Of IFeature)()
            End If

            traceFlowSolver.PutEdgeOrigins(edgeFlagArray)

            'traceFlowSolver.TraceIndeterminateFlow = True

            Dim junctionEIDs As IEnumNetEID = Nothing
            Dim edgeEIDs As IEnumNetEID = Nothing
            Dim vaRes As Object() = New Object(edgeFlagArray.Count - 1) {}
            traceFlowSolver.FindSource(_FlowMethod, esriShortestPathObjFn.esriSPObjFnMinSum, junctionEIDs, edgeEIDs, edgeFlagArray.Count, vaRes)
            '   traceFlowSolver.FindFlowElements(_FlowMethod, esriFlowElements.esriFEEdges, junctionEIDs, edgeEIDs)


            Return GetFeaturesFromEIDNetEnum(edgeEIDs)

        End Function

#End Region


#Region "Private Functions"


        Private Function GetFeaturesFromEIDNetEnum(ByVal EIDLst As IEnumNetEID) As List(Of IFeature)

            Dim result As New List(Of IFeature)()

            Dim ipEIDHelper As IEIDHelper = New EIDHelperClass()
            ipEIDHelper.GeometricNetwork = _GeometricNetwork
            ipEIDHelper.OutputSpatialReference = GetNetworkDSSpatialRef() 'ipSpatialReference
            ipEIDHelper.ReturnFeatures = True
            ipEIDHelper.ReturnGeometries = False

            Dim ipEnumEIDInfo As IEnumEIDInfo = ipEIDHelper.CreateEnumEIDInfo(EIDLst)
            Dim count As Integer = ipEnumEIDInfo.Count
            ' ipEnumEIDInfo.Reset()

            Dim ipEIDInfo As IEIDInfo = ipEnumEIDInfo.Next()
            While ipEIDInfo IsNot Nothing

                result.Add(ipEIDInfo.Feature)
                ipEIDInfo = ipEnumEIDInfo.Next()

            End While

            Return result

        End Function



        Private Function GetNearestJunctionList(ByVal inputPtList As List(Of IPoint)) As List(Of IJunctionFlag)

            Dim result As New List(Of IJunctionFlag)()

            Dim netElements As INetElements = TryCast(_GeometricNetwork, INetElements)
            For Each inputPt As IPoint In inputPtList

                Dim edgeID As Integer = GetNearestJunctionID(inputPt)

                Dim edgeUserClassID As Integer
                Dim edgeUserID As Integer
                Dim edgeUserSubID As Integer
                netElements.QueryIDs(edgeID, esriElementType.esriETJunction, edgeUserClassID, edgeUserID, edgeUserSubID)

                Dim ipNetFlag As IEdgeFlag = New EdgeFlag()
                ipNetFlag.UserClassID = edgeUserClassID
                ipNetFlag.UserID = edgeUserID
                ipNetFlag.UserSubID = edgeUserSubID

                result.Add(ipNetFlag)

            Next

            Return result

        End Function



        Private Function GetNearestEdgeList(ByVal inputPtList As List(Of IPoint)) As List(Of IEdgeFlag)

                Dim result As New List(Of IEdgeFlag)()

                Dim netElements As INetElements = TryCast(_GeometricNetwork.Network, INetElements)
                For Each inputPt As IPoint In inputPtList

                    Dim edgeID As Integer = GetNearestEdgeID(inputPt)

                    If netElements.IsValidElement(edgeID, esriElementType.esriETEdge) Then

                        Dim edgeUserClassID As Integer
                        Dim edgeUserID As Integer
                        Dim edgeUserSubID As Integer
                        netElements.QueryIDs(edgeID, esriElementType.esriETEdge, edgeUserClassID, edgeUserID, edgeUserSubID)

                        Dim ipNetFlag As INetFlag = New EdgeFlag()
                        ipNetFlag.UserClassID = edgeUserClassID
                        ipNetFlag.UserID = edgeUserID
                        ipNetFlag.UserSubID = -1 ' edgeUserSubID

                        result.Add(ipNetFlag)

                    End If



                Next

                Return result

        End Function



      

        Private Function GetNearestJunctionID(ByVal inputPt As IPoint) As Integer

            Dim outputJunctionEID As Integer
            Dim snappedPt As IPoint = New Point()

            'Dim pPointToEid As IPointToEID = New PointToEID
            'pPointToEid.GeometricNetwork = _Network
            'pPointToEid.SourceMap = map
            'pPointToEid.SnapTolerance = dRWSearchTolerance
            _PointToEID.GetNearestJunction(inputPt, outputJunctionEID, snappedPt)

            Return outputJunctionEID

        End Function


        Private Function GetNearestEdgeID(ByVal inputPt As IPoint) As Integer

            Dim outputEdgeEID As Integer
            Dim snappedPt As IPoint = New Point()
            Dim percent As Double

            'Dim pPointToEid As IPointToEID = New PointToEID
            'pPointToEid.GeometricNetwork = _Network
            'pPointToEid.SourceMap = map
            '_PointToEID.SnapTolerance = 100

           
            _PointToEID.GetNearestEdge(inputPt, outputEdgeEID, snappedPt, percent)
           
            Return outputEdgeEID

        End Function

        Private Function GetNetworkDSSpatialRef() As ISpatialReference

            Dim ds As IFeatureDataset = _GeometricNetwork.FeatureDataset
            Dim geoDS As IGeoDataset = ds
            Return geoDS.SpatialReference

            'Return _CurMap.SpatialReference

        End Function




#End Region


    End Class

End Namespace
