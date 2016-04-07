Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.Carto


Namespace GDB

    Public Class WorkspaceUtil


        Public Shared Function OpenMDBWS(ByVal strMDB As String) As IWorkspace


            Dim pPropset As IPropertySet
            pPropset = New PropertySet

            Dim pFact As IWorkspaceFactory
            Dim pWorkspace As IWorkspace

            pPropset.SetProperty("DATABASE", strMDB)

            pFact = New AccessWorkspaceFactory()
            pWorkspace = pFact.Open(pPropset, 0)

            Return pWorkspace

        End Function

        Public Shared Function OpenFileGDBWS(ByVal strPath As String) As IWorkspace

            Dim pPropset As IPropertySet
            pPropset = New PropertySet

            Dim pFact As IWorkspaceFactory
            Dim pWorkspace As IWorkspace

            pPropset.SetProperty("DATABASE", strPath)

            pFact = New FileGDBWorkspaceFactory()
            pWorkspace = pFact.Open(pPropset, 0)

            Return pWorkspace

        End Function

        Public Shared Function OpenSDEWS(ByVal propSet As IPropertySet) As IWorkspace

            Dim wsFact As IWorkspaceFactory = New SdeWorkspaceFactory()
            Return wsFact.Open(propSet, 0)

        End Function



        Public Shared Function GetShpFileFC(ByVal strFCName As String, ByVal parentFolder As String) As IFeatureClass

            Try

                Dim pFC As IFeatureClass
                Dim pWorkspaceFactory As IWorkspaceFactory
                Dim pTargetWorkspace As IFeatureWorkspace
                Dim pPropset As IPropertySet

                pWorkspaceFactory = New ShapefileWorkspaceFactory()
                pPropset = New PropertySet()
                pPropset.SetProperty("DATABASE", parentFolder)
                pTargetWorkspace = pWorkspaceFactory.Open(pPropset, 0)

                pFC = pTargetWorkspace.OpenFeatureClass(strFCName)

                Return pFC

            Catch ex As Exception
                Throw ex
            End Try

        End Function


        Public Shared Function GetDBFTable(ByVal strDBFName As String, ByVal parentFolder As String) As ITable

            Try

                Dim pTbl As ITable
                Dim pWorkspaceFactory As IWorkspaceFactory
                Dim pTargetWorkspace As IFeatureWorkspace
                Dim pPropset As IPropertySet

                pWorkspaceFactory = New ShapefileWorkspaceFactory()
                pPropset = New PropertySet()
                pPropset.SetProperty("DATABASE", parentFolder)
                pTargetWorkspace = pWorkspaceFactory.Open(pPropset, 0)

                pTbl = pTargetWorkspace.OpenTable(strDBFName)

                Return pTbl


                Return pTbl

            Catch ex As Exception
                Throw ex
            End Try

        End Function





        Public Shared Function GetAccessFC(ByVal strFCname As String, ByVal strMDBPath As String) As IFeatureClass
            Try

                Dim pFC As IFeatureClass
                Dim pWorkspaceFactory As IWorkspaceFactory
                Dim pTargetWorkspace As IFeatureWorkspace
                Dim pPropset As IPropertySet

                pWorkspaceFactory = New AccessWorkspaceFactoryClass()
                pPropset = New PropertySet()
                pPropset.SetProperty("DATABASE", strMDBPath)
                pTargetWorkspace = pWorkspaceFactory.Open(pPropset, 0)

                pFC = pTargetWorkspace.OpenFeatureClass(strFCname)

                Return pFC

            Catch ex As Exception
                Throw ex
            End Try

        End Function



        Public Shared Function FeatureClassExists(ByVal pWS As IWorkspace, ByVal name As String) As Boolean

            Dim ws2 As IWorkspace2 = pWS
            If ws2.NameExists(esriDatasetType.esriDTFeatureClass, name) Then
                Return True
            End If

            'If StandloneFeatureClassExists(pWS, name) Then
            '    Return True
            'End If

            'If includeDataset AndAlso FeatureClassOfDatasetExists(pWS, name) Then
            '    Return True
            'End If

            Return False

        End Function


        'Private Shared Function StandloneFeatureClassExists(ByVal pWS As IWorkspace, ByVal name As String) As Boolean

        '    Dim dsNameEnum As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTFeatureClass)
        '    Dim dsName As IDatasetName = dsNameEnum.Next
        '    While dsName IsNot Nothing
        '        If dsName.Name = name Then
        '            Return True
        '        End If

        '        dsName = dsNameEnum.Next
        '    End While


        '    Return False

        'End Function

        'Private Shared Function FeatureClassOfDatasetExists(ByVal pWS As IWorkspace, ByVal name As String) As Boolean

        '    Dim dsNameEnum As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTFeatureDataset)
        '    Dim dsName As IDatasetName = dsNameEnum.Next
        '    While dsName IsNot Nothing

        '        If FeatureClassExistsInDataset(name, dsName) Then
        '            Return True
        '        End If

        '        dsName = dsNameEnum.Next
        '    End While

        '    Return False

        'End Function

        'Private Shared Function FeatureClassExistsInDataset(ByVal fcName As String, dsName As IDatasetName) As Boolean

        '    Dim subDSNameEnum As IEnumDatasetName = dsName.SubsetNames
        '    Dim subDSName As IDatasetName = subDSNameEnum.Next
        '    While subDSName IsNot Nothing
        '        If subDSName.Name.ToUpper() = fcName.ToUpper() Then
        '            Return True
        '        End If
        '        subDSName = subDSNameEnum.Next
        '    End While

        '    Return False

        'End Function


        Public Shared Function GetAllFeatureClass(ByVal pWS As IWorkspace) As List(Of IFeatureClass)

            Dim result As New List(Of IFeatureClass)()

            Dim dsNameEnum As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTFeatureClass)
            Dim dsName As IDatasetName = dsNameEnum.Next
            While dsName IsNot Nothing

                Dim name As IName = dsName
                Dim fc As IFeatureClass = name.Open()
                result.Add(fc)

                dsName = dsNameEnum.Next
            End While

            Return result

        End Function



        Public Shared Function ITableExists(ByVal pWS As IWorkspace, ByVal name As String) As Boolean

            Dim ws2 As IWorkspace2 = pWS
            Return ws2.NameExists(esriDatasetType.esriDTTable, name)

            'Dim dsNameEnum As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTTable)
            'Dim dsName As IDatasetName = dsNameEnum.Next
            'While dsName IsNot Nothing
            '    If dsName.Name = name Then
            '        Return True
            '    End If

            '    dsName = dsNameEnum.Next
            'End While

            'Return False

        End Function

        Public Shared Function DeleteTableIfExists(ByVal sTable As String, ByVal pWS As IWorkspace) As Boolean

            Dim pFWS As IFeatureWorkspace
            Dim pTempTable As ITable
            Dim pDS As IDataset
            pFWS = pWS

            If ITableExists(pWS, sTable) Then
                pTempTable = pFWS.OpenTable(sTable)
                pDS = pTempTable
                If pDS.CanDelete Then
                    pDS.Delete()
                Else
                    Return False
                End If
            End If

            Return True

        End Function


        Public Shared Function DeleteFCIfExists(ByVal fcName As String, ByVal pWS As IWorkspace) As Boolean
            Dim pFWS As IFeatureWorkspace
            Dim pTempFC As IFeatureClass
            Dim pDS As IDataset
            pFWS = pWS

            If FeatureClassExists(pWS, fcName) Then

                pTempFC = pFWS.OpenFeatureClass(fcName)
                pDS = pTempFC

                If pDS.CanDelete Then
                    pDS.Delete()
                Else
                    Return False
                End If

            End If

            Return True

        End Function


        Public Shared Function CreateNewMDBWS(ByVal strMDB As String, ByVal deleteOld As Boolean) As IWorkspace

            Dim fi As New System.IO.FileInfo(strMDB)
            If fi.Exists Then

                If deleteOld Then
                    fi.Delete()
                Else
                    Return OpenMDBWS(strMDB)
                End If
            End If

            Return CreateNewMDBWS(strMDB)

        End Function


        Public Shared Function CreateNewMDBWS(ByVal strMDB As String) As IWorkspace

            Dim fi As New System.IO.FileInfo(strMDB)

            Dim wsFact As IWorkspaceFactory = New AccessWorkspaceFactory()
            Dim workspaceName As IWorkspaceName = wsFact.Create(fi.Directory.FullName, fi.Name, Nothing, 0)

            ' Cast for IName and open a reference to the in-memory workspace through the name object.
            Dim name As IName = DirectCast(workspaceName, IName)
            Dim workspace As IWorkspace = DirectCast(name.Open(), IWorkspace)

            Return workspace

        End Function

        Public Shared Function CreateInMemoryWorkspace() As IWorkspace
            ' Create an in-memory workspace factory.
            Dim factoryType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.InMemoryWorkspaceFactory")
            Dim workspaceFactory As IWorkspaceFactory = DirectCast(Activator.CreateInstance(factoryType), IWorkspaceFactory)

            ' Create an in-memory workspace.
            Dim workspaceName As IWorkspaceName = workspaceFactory.Create("", "MyWorkspace", Nothing, 0)

            ' Cast for IName and open a reference to the in-memory workspace through the name object.
            Dim name As IName = DirectCast(workspaceName, IName)
            Dim workspace As IWorkspace = DirectCast(name.Open(), IWorkspace)

            Return workspace
        End Function

        Public Shared Function CreateScratchWorkspace() As IWorkspace
            ' Create a scratch workspace factory.
            Dim factoryType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.ScratchWorkspaceFactory")
            Dim scratchWorkspaceFactory As IScratchWorkspaceFactory = CType(Activator.CreateInstance(factoryType), IScratchWorkspaceFactory)

            ' Get the default scratch workspace.
            Dim scratchWorkspace As IWorkspace = scratchWorkspaceFactory.DefaultScratchWorkspace
            Return scratchWorkspace
        End Function


        Public Shared Function CreateFileGdbScratchWorkspace() As IWorkspace
            ' Create a file scratch workspace factory.
            Dim factoryType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBScratchWorkspaceFactory")
            Dim scratchWorkspaceFactory As IScratchWorkspaceFactory = CType(Activator.CreateInstance(factoryType), IScratchWorkspaceFactory)

            ' Get the default scratch workspace.
            Dim scratchWorkspace As IWorkspace = scratchWorkspaceFactory.DefaultScratchWorkspace
            Return scratchWorkspace
        End Function




        Public Shared Function GetWorkspaceOfFeatureLyr(ByVal featLyr As IFeatureLayer) As IWorkspace

            Dim ds As ESRI.ArcGIS.Geodatabase.IDataset = CType(featLyr.FeatureClass, IDataset)
            Return ds.Workspace

        End Function





    End Class

End Namespace