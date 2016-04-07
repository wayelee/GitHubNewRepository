Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Location
Imports ESRI.ArcGIS.ArcMapUI
'Imports ESRI.ArcGIS.DataSourcesOleDB
'Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Display

Namespace Carto
    Public Class MapDisplay

        Public Shared Function SelectByFeature(ByVal pMap As IMap, _
              ByVal pLayer As IFeatureLayer, ByVal pF As IFeature) As Boolean
            Try
                pMap.SelectFeature(pLayer, pF)
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Shared Function ClearSelctedFeature(ByVal pMap As IMap) As Boolean
            Try
                pMap.ClearSelection()
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Shared Sub zoom2Selection(ByVal pMap As IMap, _
                          ByVal pipelineLayer As IFeatureLayer, _
                          ByVal pipelineIDField As String, _
                          ByVal pipelineIDValue As String)
            Try

                'Dim selCount As Integer
                'SetLayerSelection(pipelineLayer, pipelineIDField, pipelineIDValue, selCount)

                Dim pLayer As IFeatureLayer
                Dim pFSel As IFeatureSelection
                pLayer = pipelineLayer
                pFSel = pLayer

                'Get the selected features
                Dim pSelSet As ISelectionSet
                pSelSet = pFSel.SelectionSet

                Dim pEnumGeom As IEnumGeometry
                Dim pEnumGeomBind As IEnumGeometryBind

                pEnumGeom = New EnumFeatureGeometry
                pEnumGeomBind = pEnumGeom
                pEnumGeomBind.BindGeometrySource(Nothing, pSelSet)

                Dim pGeomFactory As IGeometryFactory
                pGeomFactory = New GeometryEnvironment

                Dim pGeom As IGeometry
                pGeom = pGeomFactory.CreateGeometryFromEnumerator(pEnumGeom)

                'pDoc.ActiveView.Extent = pGeom.Envelope
                'pDoc.ActiveView.Refresh()
            Catch ex As Exception

            End Try
        End Sub
        Public Shared Sub Zoom2Selection(ByVal pMap As IMap, ByVal pView As IActiveView, _
                ByVal pipelineLayer As IFeatureLayer, Optional ByVal isPan As Boolean = False)
            Try

                Dim pLayer As IFeatureLayer
                Dim pFSel As IFeatureSelection
                pLayer = pipelineLayer
                pFSel = pLayer

                'Get the selected features
                Dim pSelSet As ISelectionSet
                pSelSet = pFSel.SelectionSet

                Dim pEnumGeom As IEnumGeometry
                Dim pEnumGeomBind As IEnumGeometryBind

                pEnumGeom = New EnumFeatureGeometry
                pEnumGeomBind = pEnumGeom
                pEnumGeomBind.BindGeometrySource(Nothing, pSelSet)

                Dim pGeomFactory As IGeometryFactory
                pGeomFactory = New GeometryEnvironment

                Dim pGeom As IGeometry
                pGeom = pGeomFactory.CreateGeometryFromEnumerator(pEnumGeom)
                Dim prjnewEv As New Envelope
                prjnewEv = Geonamic.Core.AOUtils.Geometry.GeometryUtil.projectGeometry(pGeom.Envelope, pGeom.Envelope.SpatialReference, pMap.SpatialReference)
                If prjnewEv.Width = 0 Then isPan = True
                If isPan Then
                    Dim newPt As New Point
                    newPt.X = (prjnewEv.XMax + prjnewEv.XMin) / 2
                    newPt.Y = (prjnewEv.YMax + prjnewEv.YMin) / 2
                    'Dim prjPt As IPoint
                    'prjPt = Geonamic.Core.AOUtils.Geometry.GeometryUtil.projectGeometry(newPt, pGeom.SpatialReference, pMap.SpatialReference)
                    Dim newEv As New Envelope
                    newEv.XMax = pView.Extent.XMax
                    newEv.YMax = pView.Extent.YMax
                    newEv.XMin = pView.Extent.XMin
                    newEv.YMin = pView.Extent.YMin
                    newEv.CenterAt(newPt)
                    pView.Extent = newEv
                Else
                    pView.Extent = prjnewEv
                End If
                pView.Refresh()
            Catch ex As Exception
            End Try
        End Sub


        Public Shared Sub mapZoomOut(ByVal pMxDocument As ESRI.ArcGIS.ArcMapUI.IMxDocument, ByRef pInputEnvelope As IEnvelope)

            Dim pActiveView As IActiveView
            Dim pDisplayTransform As IDisplayTransformation
            Dim pEnvelope As IEnvelope
            Dim pCenterPoint As IPoint


            pActiveView = pMxDocument.FocusMap
            pDisplayTransform = pActiveView.ScreenDisplay.DisplayTransformation
            pEnvelope = pDisplayTransform.VisibleBounds
            'In this case, we could have set pEnvelope to IActiveView::Extent
            'Set pEnvelope = pActiveView.Extent
            pCenterPoint = New Point

            pCenterPoint.X = ((pEnvelope.XMax - pEnvelope.XMin) / 2) + pEnvelope.XMin
            pCenterPoint.Y = ((pEnvelope.YMax - pEnvelope.YMin) / 2) + pEnvelope.YMin
            pEnvelope.Width = pInputEnvelope.Width
            pEnvelope.Height = pInputEnvelope.Height
            pEnvelope.CenterAt(pCenterPoint)
            pDisplayTransform.VisibleBounds = pEnvelope
            pActiveView.Refresh()
        End Sub
        Public Shared Sub Pan2Selection(ByVal pMap As IMap, ByVal pView As IActiveView, _
               ByVal pipelineLayer As IFeatureLayer, Optional ByVal isPan As Boolean = True)
            Try

                Dim pLayer As IFeatureLayer
                Dim pFSel As IFeatureSelection
                pLayer = pipelineLayer
                pFSel = pLayer

                'Get the selected features
                Dim pSelSet As ISelectionSet
                pSelSet = pFSel.SelectionSet

                Dim pEnumGeom As IEnumGeometry
                Dim pEnumGeomBind As IEnumGeometryBind

                pEnumGeom = New EnumFeatureGeometry
                pEnumGeomBind = pEnumGeom
                pEnumGeomBind.BindGeometrySource(Nothing, pSelSet)

                Dim pGeomFactory As IGeometryFactory
                pGeomFactory = New GeometryEnvironment

                Dim pGeom As IGeometry
                pGeom = pGeomFactory.CreateGeometryFromEnumerator(pEnumGeom)
                Dim prjnewEv As New Envelope
                prjnewEv = Geonamic.Core.AOUtils.Geometry.GeometryUtil.projectGeometry(pGeom.Envelope, pGeom.Envelope.SpatialReference, pMap.SpatialReference)
                If prjnewEv.Width = 0 Then isPan = True
                If isPan Then
                    Dim newPt As New Point
                    newPt.X = (prjnewEv.XMax + prjnewEv.XMin) / 2
                    newPt.Y = (prjnewEv.YMax + prjnewEv.YMin) / 2
                    'Dim prjPt As IPoint
                    'prjPt = Geonamic.Core.AOUtils.Geometry.GeometryUtil.projectGeometry(newPt, pGeom.SpatialReference, pMap.SpatialReference)
                    Dim newEv As New Envelope
                    newEv.XMax = pView.Extent.XMax
                    newEv.YMax = pView.Extent.YMax
                    newEv.XMin = pView.Extent.XMin
                    newEv.YMin = pView.Extent.YMin
                    newEv.CenterAt(newPt)
                    pView.Extent = newEv

                End If
                pView.Refresh()
            Catch ex As Exception
            End Try
        End Sub
    End Class
End Namespace


