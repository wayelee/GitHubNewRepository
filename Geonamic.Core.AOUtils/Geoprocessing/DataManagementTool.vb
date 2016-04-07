Imports System.IO
Imports System.Text



Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geoprocessor
'Imports ESRI.ArcGIS.Geoprocessing
Imports ESRI.ArcGIS.GeoprocessingUI
Imports ESRI.ArcGIS.DataManagementTools

Namespace Geoprocessing



    Public Class DataManagementTool


        'Public Shared Function CompressFileGDB() As Boolean

        '    Dim pGPMessages As IGPMessages

        '    'Try

        '    '    Dim compressHandler As CompressFileGeodatabaseData = New CompressFileGeodatabaseData()



        '    '    'Dim pGPTFact As IWorkspaceFactory = New ToolboxWorkspaceFactory()
        '    '    'Dim pToolboxWorkspace As IToolboxWorkspace = pGPTFact.OpenFromFile("C:\Program Files\ArcGIS\ArcToolBox\Toolboxes", 0)
        '    '    'Dim pGPToolbox As IGPToolbox = pToolboxWorkspace.OpenToolbox("Data Management Tools")
        '    '    'Dim pGPTool As IGPTool = pGPToolbox.OpenTool("Compact")


        '    '    Dim pGPTFact As IWorkspaceFactory = New ToolboxWorkspaceFactory()
        '    '    Dim pToolboxWorkspace As IToolboxWorkspace = pGPTFact.OpenFromFile("C:\Program Files\ArcGIS\ArcToolBox\Toolboxes", 0)
        '    '    Dim pGPToolbox As IGPToolbox = pToolboxWorkspace.OpenToolbox(compressHandler.ToolboxName)
        '    '    Dim pGPTool As IGPTool = pGPToolbox.OpenTool(compressHandler.ToolName)

        '    '    Dim pParams As IArray = pGPTool.ParameterInfo
        '    '    Dim pParameter As IGPParameter = Nothing
        '    '    Dim pParamEdit As IGPParameterEdit = Nothing
        '    '    Dim pDataType As IGPDataType = Nothing
        '    '    Dim sValue As String = String.Empty

        '    '    pParameter = pParams.Element(0)
        '    '    pParamEdit = pParameter
        '    '    pDataType = pParameter.DataType
        '    '    pParamEdit.Value = pDataType.CreateValue("D:\Temp\Test1.gdb")

        '    '    pParameter = pParams.Element(1)
        '    '    pParamEdit = pParameter
        '    '    pDataType = pParameter.DataType
        '    '    pParamEdit.Value = pDataType.CreateValue("Centerline")

        '    '    pGPMessages = pGPTool.Validate(pParams, True, Nothing)
        '    '    'Dim trkcan As ITrackCancel=New t


        '    '    Dim msgArray As IArray = pGPMessages.Messages
        '    '    For i As Integer = 0 To msgArray.Count - 1

        '    '        Dim msg As IGPMessage = msgArray.Element(i)
        '    '        Dim str As String = msg.Description

        '    '    Next

        '    '    Dim pEnvmgr As IGPEnvironmentManager = New GPEnvironmentManager()
        '    '    pGPTool.Execute(pParams, Nothing, pEnvmgr, pGPMessages)

        '    'Catch ex As Exception

        '    '    If pGPMessages IsNot Nothing Then
        '    '        Dim msgArray As IArray = pGPMessages.Messages
        '    '        For i As Integer = 0 To msgArray.Count - 1

        '    '            Dim msg As IGPMessage = msgArray.Element(i)
        '    '            Dim str As String = msg.Description

        '    '        Next
        '    '    End If



        '    '    Throw ex

        '    'Finally
        '    '    Dim pGPUtilities As IGPUtilities = New GPUtilities()
        '    '    pGPUtilities.ReleaseInternals()
        '    'End Try




        'End Function


        Public Shared Function UncompressFileGDB(ByVal gdpPath As String) As List(Of String)

            Try

                Dim compressHandler As UncompressFileGeodatabaseData = New UncompressFileGeodatabaseData()

                compressHandler.in_data = gdpPath
                compressHandler.out_data = gdpPath

                Dim GP As Geoprocessor = New Geoprocessor()
                GP.OverwriteOutput = True
                Return RunTool(GP, compressHandler, Nothing)


            Catch ex As Exception
                Throw ex
            End Try

        End Function

        Public Shared Function CompressFileGDB(ByVal gdpPath As String) As List(Of String)

            Try

                Dim compressHandler As CompressFileGeodatabaseData = New CompressFileGeodatabaseData()

                compressHandler.in_data = gdpPath
                compressHandler.out_data = gdpPath

                Dim GP As Geoprocessor = New Geoprocessor()
                GP.OverwriteOutput = True
                Return RunTool(GP, compressHandler, Nothing)


            Catch ex As Exception
                Throw ex
            End Try

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inputDatasets">format like: E:\NHD123.gdb\DatasetName\FeatureclassName</param>
        ''' <param name="outputDataset">format like: E:\OutputEleTable\HydroData.gdb\FCName_Merge</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function MergeDatasets(ByVal inputDatasets As List(Of String), ByVal outputDataset As String) As List(Of String)

            Dim mergeTool As New Merge()
            mergeTool.inputs = inputDatasets.Aggregate(Function(current, nextOne) current & ";" & nextOne)
            mergeTool.output = outputDataset

            Dim GP As Geoprocessor = New Geoprocessor()
            GP.OverwriteOutput = True
            Return RunTool(GP, mergeTool, Nothing)

        End Function

        Private Shared Function RunTool(ByVal geoprocessor As Geoprocessor, _
                              ByVal process As IGPProcess, ByVal TC As ITrackCancel) As List(Of String)

            ' Set the overwrite output option to true
            geoprocessor.OverwriteOutput = True

            Try
                geoprocessor.Execute(Process, Nothing)
                Return ReturnMessages(geoprocessor)

            Catch err As Exception
                Console.WriteLine(err.Message)
                Return ReturnMessages(geoprocessor)
            End Try

        End Function

        ' Function for returning the tool messages.
        Private Shared Function ReturnMessages(ByVal gp As Geoprocessor)

            Dim result As New List(Of String)
            ' Print out the messages from tool executions
            Dim Count As Integer
            If gp.MessageCount > 0 Then
                For Count = 0 To gp.MessageCount - 1
                    Debug.WriteLine(gp.GetMessage(Count))
                    result.Add(gp.GetMessage(Count))
                Next
            End If

            Return result
        End Function

        'Public Shared Function RunPythonScript() As Boolean

        '    Dim txtScript As StringBuilder = New StringBuilder()
        '    txtScript.Append("import arcgisscripting ").Append(vbNewLine).Append("from arcgisscripting import env")

        '    Try

        '        Dim setup As New ScriptRuntimeSetup()
        '        setup.LanguageSetups.Add(IronPython.Hosting.Python.CreateLanguageSetup(Nothing))
        '        Dim runtime As New ScriptRuntime(setup)
        '        runtime.IO.RedirectToConsole()

        '        Dim engine As ScriptEngine = runtime.GetEngine("IronPython")

        '        AddReferencePath(engine, "C:\Program Files\ArcGIS\Bin\arcgisscripting.pyd")

        '        Dim scope As ScriptScope = engine.CreateScope()
        '        Dim source As ScriptSource = engine.CreateScriptSourceFromString(txtScript.ToString(), SourceCodeKind.Statements)
        '        source.Execute(scope)
        '        Console.WriteLine(scope.GetVariable("a"))

        '    Catch ex As Exception
        '        Throw ex
        '    End Try


        'End Function


        'Private Shared Sub AddReferencePath(ByVal engine As ScriptEngine, ByVal scriptPath As String)

        '    Dim dir As String = Path.GetDirectoryName(scriptPath)
        '    Dim paths As ICollection(Of String) = engine.GetSearchPaths()

        '    If Not [String].IsNullOrEmpty(dir) Then
        '        paths.Add(dir)
        '    Else
        '        paths.Add(Environment.CurrentDirectory)
        '    End If
        '    engine.SetSearchPaths(paths)



        'End Sub



    End Class


End Namespace