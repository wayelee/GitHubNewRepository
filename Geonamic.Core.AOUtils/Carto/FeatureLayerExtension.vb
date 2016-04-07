
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Carto


Namespace CartoExtention


    Public Module FeatureLayerExtension

        <System.Runtime.CompilerServices.Extension()> _
       Public Function IsSelectionLayer(ByVal featLyr As IFeatureLayer) As Boolean

            Dim lyrDef As IFeatureLayerDefinition = featLyr
            Dim exp As String = lyrDef.DefinitionExpression
            Dim selSet As ISelectionSet = lyrDef.DefinitionSelectionSet

            If String.IsNullOrEmpty(exp) And selSet Is Nothing Then
                Return False
            Else
                Return True
            End If

        End Function

        <System.Runtime.CompilerServices.Extension()> _
      Public Function CreateSelectionLayer(ByVal featLyr As IFeatureLayer, ByVal selectionWhereClause As String, ByVal selectionLyrName As String) As IFeatureLayer

            Dim pQueryFilter As IQueryFilter = New QueryFilter()
            pQueryFilter.WhereClause = selectionWhereClause

            Dim pFSel As IFeatureSelection = featLyr
            pFSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, False)

            Dim pFLDef As IFeatureLayerDefinition = featLyr
            Dim pSelFLayer As IFeatureLayer = pFLDef.CreateSelectionLayer(selectionLyrName, True, vbNullString, vbNullString)
            pFSel.Clear()
            ' clear selection on original layer  
            Return pSelFLayer

        End Function




    End Module


End Namespace
