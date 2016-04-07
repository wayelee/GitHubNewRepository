Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.EditorExt
Imports ESRI.ArcGIS.NetworkAnalysis

Namespace Network

    Public MustInherit Class GeometricNetworkTool

        Protected _GeometricNetwork As IGeometricNetwork
        Protected _PointToEID As IPointToEID
        Protected _CurMap As IMap


        Public ReadOnly Property FeatureDS() As IFeatureDataset
            Get
                Return _GeometricNetwork.FeatureDataset
            End Get
        End Property

        Public WriteOnly Property SnapTolerance() As Single    
            Set(ByVal value As Single)
                _PointToEID.SnapTolerance = value
            End Set
        End Property

        Public Sub New(ByVal map As IMap, ByVal featureDS As IFeatureDataset)

            _CurMap = map
            OpenFeatureDatasetNetwork(featureDS)






        End Sub



#Region "Private Functions"

        Private Sub OpenFeatureDatasetNetwork(ByVal FeatureDataset As IFeatureDataset)

            CloseWorkspace()
            InitializeNetworkAndMap(FeatureDataset)

        End Sub



        Private Function InitializeNetworkAndMap(ByVal FeatureDataset As IFeatureDataset) As Boolean

            Dim featureClassContainer As IFeatureClassContainer = Nothing
            Dim featClass As IFeatureClass = Nothing

            
            Dim ipNetworkCollection As INetworkCollection = TryCast(FeatureDataset, INetworkCollection)
            Dim count As Integer = ipNetworkCollection.GeometricNetworkCount
            '获取几何网络工作空间
            _GeometricNetwork = ipNetworkCollection.GeometricNetwork(0)
            Dim ipNetwork As INetwork = _GeometricNetwork.Network

            'If _CurMap IsNot Nothing Then
            '    _CurMap = New MapClass()
            '    featureClassContainer = TryCast(_GeometricNetwork, IFeatureClassContainer)
            '    count = featureClassContainer.ClassCount
            '    For i As Integer = 0 To count - 1
            '        featClass = featureClassContainer.Class(i)
            '        featLyr = New FeatureLayerClass()
            '        featLyr.FeatureClass = featClass
            '        _CurMap.AddLayer(featLyr)
            '    Next
            'End If
            'count = _CurMap.LayerCount

            Dim env As IEnvelope, maxEnv As IEnvelope
            maxEnv = New EnvelopeClass()
            featureClassContainer = TryCast(_GeometricNetwork, IFeatureClassContainer)
            count = featureClassContainer.ClassCount
            For i As Integer = 0 To count - 1
                featClass = featureClassContainer.Class(i)
                Dim featLyr As IFeatureLayer = New FeatureLayer()
                featLyr.FeatureClass = featClass

                Dim geoDS As IGeoDataset = TryCast(featLyr, IGeoDataset)
                env = geoDS.Extent
                maxEnv.Union(env)

            Next
            
            _PointToEID = New PointToEIDClass()
            _PointToEID.SourceMap = _CurMap
            _PointToEID.GeometricNetwork = _GeometricNetwork

            Dim dblSearchTol As Double
            Dim dblWidth As Double = maxEnv.Width
            Dim dblHeight As Double = maxEnv.Height

            If dblWidth > dblHeight Then
                dblSearchTol = dblWidth / 100
            Else
                dblSearchTol = dblHeight / 100
            End If
            _PointToEID.SnapTolerance = dblSearchTol

            Return True

        End Function

        Private Sub CloseWorkspace()

            _GeometricNetwork = Nothing
            _PointToEID = Nothing

        End Sub


#End Region






    End Class



    Public Class NetworkAnalysisExtUtil

        Private _App As IApplication

        Public Sub New(ByVal app As IApplication)
            _App = app
        End Sub


        Private Function FindExtension() As INetworkAnalysisExt

            Dim pUID As UID = New UIDClass()
            pUID.Value = "esriEditorExt.UtilityNetworkAnalysisExt"

            Dim pNetworkAnalysisExt As INetworkAnalysisExt = _App.FindExtensionByCLSID(pUID)
            Return pNetworkAnalysisExt

        End Function

        Public Function CreateINetworkAnalysisExtByDataset(ByVal ds As IFeatureDataset, ByVal geoNetworkName As String) As INetworkAnalysisExt

            Dim networkCol As INetworkCollection = ds
            Dim geoNetwork As IGeometricNetwork = networkCol.GeometricNetworkByName(geoNetworkName)

            Dim fcCol As IEnumFeatureClass = geoNetwork.ClassesByType(esriFeatureType.esriFTSimpleJunction)
            fcCol.Reset()

            Dim fc As IFeatureClass = fcCol.Next()

            Dim featLyr As IFeatureLayer = New FeatureLayer()
            featLyr.FeatureClass = fc


            Dim networkExt As INetworkAnalysisExt = FindExtension()
            networkExt.AddLayer(featLyr)

            'Dim networkCount As Integer = networkExt.NetworkCount


            Return networkExt

        End Function

        Public Sub RemoveAllNetwork()

            Dim networkExt As INetworkAnalysisExt = FindExtension()

            'For index As Integer = 0 To networkExt.FeatureLayerCount - 1

            '    Dim lyr As ILayer = networkExt.FeatureLayer(index)
            '    networkExt.DropLayer(lyr)

            'Next

            For index As Integer = 0 To networkExt.NetworkCount - 1

                Dim network As IGeometricNetwork = networkExt.CurrentNetwork
                networkExt.DeleteNetwork(network)

            Next

        End Sub




    End Class

End Namespace
