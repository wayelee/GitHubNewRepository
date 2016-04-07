
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.NetworkAnalysis


'----------------Use case--------------------------------------
' ''ClsPathFinder   m_ipPathFinder;

' ''if(m_ipPathFinder==null)//打开网络空间
' ''    {
' ''     m_ipPathFinder = new ClsPathFinder();
' ''     ipMap = this.m_ActiveView.FocusMap;
' ''     ipLayer = ipMap.get_Layer(0);
' ''     ipFeatureLayer = ipLayer as IFeatureLayer;
' ''     ipFDB = ipFeatureLayer.FeatureClass.FeatureDataset;
' ''     m_ipPathFinder.SetOrGetMap = ipMap;
' ''     m_ipPathFinder.OpenFeatureDatasetNetwork(ipFDB);
' ''    }

' ''private void ViewMap_OnMouseDown(object sender, ESRI.ArcGIS.MapControl.IMapControlEvents2_OnMouseDownEvent e)//获取鼠标输入的点
' ''   {
' ''    IPoint ipNew ;  
' ''    if( m_ipPoints==null)
' ''    {
' ''     m_ipPoints = new MultipointClass();
' ''     m_ipPathFinder.StopPoints = m_ipPoints;
' ''    }
' ''    ipNew = ViewMap.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x,e.y);
' ''    object o = Type.Missing;
' ''    m_ipPoints.AddPoint(ipNew,ref o,ref o);    
' ''   }

' ''m_ipPathFinder.SolvePath("Weight");//解析路径

' ''IPolyline ipPolyResult = m_ipPathFinder.PathPolyLine();//返回最短路径



Public Class PathFinder

    Private m_ipGeometricNetwork As IGeometricNetwork
    Private m_ipMap As IMap
    Private m_ipPoints As IPointCollection
    Private m_ipPointToEID As IPointToEID
    Private m_dblPathCost As Double = 0
    Private m_ipEnumNetEID_Junctions As IEnumNetEID
    Private m_ipEnumNetEID_Edges As IEnumNetEID
    Private m_ipPolyline As IPolyline

#Region "Public Function"
    '返回和设置当前地图
    Public Property SetOrGetMap() As IMap
        Get
            Return m_ipMap
        End Get
        Set(ByVal value As IMap)
            m_ipMap = value
        End Set
    End Property
    '打开网络
    Public Sub OpenFeatureDatasetNetwork(ByVal FeatureDataset As IFeatureDataset)
        CloseWorkspace()
        If Not InitializeNetworkAndMap(FeatureDataset) Then
            Console.WriteLine("打开出错")
        End If
    End Sub
    '输入点的集合
    Public Property StopPoints() As IPointCollection
        Get
            Return m_ipPoints
        End Get
        Set(ByVal value As IPointCollection)
            m_ipPoints = value
        End Set
    End Property

    '路径成本
    Public ReadOnly Property PathCost() As Double
        Get
            Return m_dblPathCost
        End Get
    End Property

    '返回路径
    Public Function PathPolyLine() As IPolyline
        Dim ipEIDInfo As IEIDInfo
        Dim ipGeometry As IGeometry
        If m_ipPolyline IsNot Nothing Then
            Return m_ipPolyline
        End If

        m_ipPolyline = New PolylineClass()
        Dim ipNewGeometryColl As IGeometryCollection = TryCast(m_ipPolyline, IGeometryCollection)

        Dim ipSpatialReference As ISpatialReference = m_ipMap.SpatialReference
        Dim ipEIDHelper As IEIDHelper = New EIDHelperClass()
        ipEIDHelper.GeometricNetwork = m_ipGeometricNetwork
        ipEIDHelper.OutputSpatialReference = ipSpatialReference
        ipEIDHelper.ReturnGeometries = True

        Dim ipEnumEIDInfo As IEnumEIDInfo = ipEIDHelper.CreateEnumEIDInfo(m_ipEnumNetEID_Edges)
        Dim count As Integer = ipEnumEIDInfo.Count
        ipEnumEIDInfo.Reset()
        For i As Integer = 0 To count - 1
            ipEIDInfo = ipEnumEIDInfo.[Next]()
            ipGeometry = ipEIDInfo.Geometry
            ipNewGeometryColl.AddGeometryCollection(TryCast(ipGeometry, IGeometryCollection))
        Next
        Return m_ipPolyline
    End Function

    '解决路径
    Public Sub SolvePath(ByVal WeightName As String)
        Try
            Dim intEdgeUserClassID As Integer
            Dim intEdgeUserID As Integer
            Dim intEdgeUserSubID As Integer
            Dim intEdgeID As Integer
            Dim ipFoundEdgePoint As IPoint = New Point()
            Dim dblEdgePercent As Double
            'C#中使用
            '      *ITraceFlowSolverGEN替代ITraceFlowSolver
            '      

            Dim ipTraceFlowSolver As ITraceFlowSolverGEN = TryCast(New TraceFlowSolverClass(), ITraceFlowSolverGEN)
            Dim ipNetSolver As INetSolver = TryCast(ipTraceFlowSolver, INetSolver)
            Dim ipNetwork As INetwork = m_ipGeometricNetwork.Network
            ipNetSolver.SourceNetwork = ipNetwork
            Dim ipNetElements As INetElements = TryCast(ipNetwork, INetElements)
            Dim intCount As Integer = m_ipPoints.PointCount
            '定义一个边线旗数组
            Dim pEdgeFlagList As IEdgeFlag() = New EdgeFlagClass(intCount - 1) {}
            For i As Integer = 0 To intCount - 1

                Dim ipNetFlag As INetFlag = TryCast(New EdgeFlagClass(), INetFlag)
                Dim ipEdgePoint As IPoint = m_ipPoints.Point(i)
                '查找输入点的最近的边线
                m_ipPointToEID.GetNearestEdge(ipEdgePoint, intEdgeID, ipFoundEdgePoint, dblEdgePercent)
                ipNetElements.QueryIDs(intEdgeID, esriElementType.esriETEdge, intEdgeUserClassID, intEdgeUserID, intEdgeUserSubID)
                ipNetFlag.UserClassID = intEdgeUserClassID
                ipNetFlag.UserID = intEdgeUserID
                ipNetFlag.UserSubID = intEdgeUserSubID
                Dim pTemp As IEdgeFlag = DirectCast(TryCast(ipNetFlag, IEdgeFlag), IEdgeFlag)
                pEdgeFlagList(0) = pTemp
            Next
            ipTraceFlowSolver.PutEdgeOrigins(pEdgeFlagList)
            Dim ipNetSchema As INetSchema = TryCast(ipNetwork, INetSchema)
            Dim ipNetWeight As INetWeight = ipNetSchema.get_WeightByName(WeightName)

            Dim ipNetSolverWeights As INetSolverWeights = TryCast(ipTraceFlowSolver, INetSolverWeights)
            ipNetSolverWeights.FromToEdgeWeight = ipNetWeight
            '开始边线的权重
            ipNetSolverWeights.ToFromEdgeWeight = ipNetWeight
            '终止边线的权重
            Dim vaRes As Object() = New Object(intCount - 2) {}
            '通过findpath得到边线和交汇点的集合
            ipTraceFlowSolver.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum, m_ipEnumNetEID_Junctions, m_ipEnumNetEID_Edges, intCount - 1, vaRes)
            '计算成本
            m_dblPathCost = 0
            For i As Integer = 0 To vaRes.Length - 1
                Dim m_Va As Double = CDbl(vaRes(i))
                m_dblPathCost = m_dblPathCost + m_Va
            Next
            m_ipPolyline = Nothing
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub
#End Region

#Region "Private Function"
    '初始化
    Private Function InitializeNetworkAndMap(ByVal FeatureDataset As IFeatureDataset) As Boolean
        Dim ipFeatureClassContainer As IFeatureClassContainer
        Dim ipFeatureClass As IFeatureClass
        Dim ipGeoDataset As IGeoDataset
        Dim ipLayer As ILayer
        Dim ipFeatureLayer As IFeatureLayer
        Dim ipEnvelope As IEnvelope, ipMaxEnvelope As IEnvelope
        Dim dblSearchTol As Double

        Dim ipNetworkCollection As INetworkCollection = TryCast(FeatureDataset, INetworkCollection)
        Dim count As Integer = ipNetworkCollection.GeometricNetworkCount
        '获取几何网络工作空间
        m_ipGeometricNetwork = ipNetworkCollection.get_GeometricNetwork(0)
        Dim ipNetwork As INetwork = m_ipGeometricNetwork.Network

        If m_ipMap IsNot Nothing Then
            m_ipMap = New Map()
            ipFeatureClassContainer = TryCast(m_ipGeometricNetwork, IFeatureClassContainer)
            count = ipFeatureClassContainer.ClassCount
            For i As Integer = 0 To count - 1
                ipFeatureClass = ipFeatureClassContainer.get_Class(i)
                ipFeatureLayer = New FeatureLayer()
                ipFeatureLayer.FeatureClass = ipFeatureClass
                m_ipMap.AddLayer(ipFeatureLayer)
            Next
        End If
        count = m_ipMap.LayerCount
        ipMaxEnvelope = New EnvelopeClass()
        For i As Integer = 0 To count - 1
            ipLayer = m_ipMap.get_Layer(i)
            ipFeatureLayer = TryCast(ipLayer, IFeatureLayer)
            ipGeoDataset = TryCast(ipFeatureLayer, IGeoDataset)
            ipEnvelope = ipGeoDataset.Extent
            ipMaxEnvelope.Union(ipEnvelope)
        Next

        m_ipPointToEID = New PointToEIDClass()
        m_ipPointToEID.SourceMap = m_ipMap
        m_ipPointToEID.GeometricNetwork = m_ipGeometricNetwork

        Dim dblWidth As Double = ipMaxEnvelope.Width
        Dim dblHeight As Double = ipMaxEnvelope.Height

        If dblWidth > dblHeight Then
            dblSearchTol = dblWidth / 100
        Else
            dblSearchTol = dblHeight / 100
        End If
        m_ipPointToEID.SnapTolerance = dblSearchTol

        Return True

    End Function

    Private Sub CloseWorkspace()
        m_ipGeometricNetwork = Nothing
        m_ipPoints = Nothing
        m_ipPointToEID = Nothing
        m_ipEnumNetEID_Junctions = Nothing
        m_ipEnumNetEID_Edges = Nothing
        m_ipPolyline = Nothing
    End Sub

#End Region


End Class
