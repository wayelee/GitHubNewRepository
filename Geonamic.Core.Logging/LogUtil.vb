Imports System
Imports NLog
Imports NLog.Config
Imports System.IO
Imports NLog.Targets


Public Class LogUtil

    Private Shared m_log As Logger = CreateLogger()

#Region "CreateLogger"

    Private Shared Function CreateLogger() As Logger

        If LogManager.Configuration Is Nothing Then
            LogManager.Configuration = TryCreateConfiguration()
        End If

        LogManager.ThrowExceptions = True
        Return LogManager.GetCurrentClassLogger()

    End Function

    Private Shared Function TryCreateConfiguration() As LoggingConfiguration

        For Each configFile As String In GetCandidateFileNames()
            If (File.Exists(configFile)) Then
                Dim cfg As LoggingConfiguration = New XmlLoggingConfiguration(configFile)
                SetFilePathAsCurrentPath(cfg)
                Return cfg
            End If
        Next

        Return Nothing

    End Function

    Private Shared Function GetCandidateFileNames() As List(Of String)

        Dim result As New List(Of String)()
        Dim nlogFile As String = GetType(LogFactory).Assembly.Location & ".nlog"
        result.Add(nlogFile)

        Dim nlogConfigFile As String = GetCurrentDir() & "\NLog.config"
        result.Add(nlogConfigFile)

        Return result

    End Function

    Private Shared Function GetCurrentDir() As String

        Dim strConfigLoc As String = System.Reflection.Assembly.GetAssembly(GetType(LogFactory)).Location
        Dim fi As New IO.FileInfo(strConfigLoc)
        Return fi.DirectoryName

    End Function


    Private Shared Sub SetFilePathAsCurrentPath(cfg As LoggingConfiguration)

        Dim baseDirKeyWord As String = "${basedir}"
        Dim curDir As String = GetCurrentDir()

        For Each r In cfg.LoggingRules()
            For Each target In r.Targets
                If TypeOf (target) Is FileTarget Then

                    Dim fileTgt As FileTarget = CType(target, FileTarget)
                    'Dim fileName As String = fileTgt.FileName
                    If fileTgt.FileName.ToString().Contains(baseDirKeyWord) Then

                        Dim newFileName As String = fileTgt.FileName.ToString().Replace(baseDirKeyWord, curDir)
                        newFileName = newFileName.Replace("/", "\")
                        newFileName = newFileName.TrimStart("'")
                        newFileName = newFileName.TrimEnd("'")
                        fileTgt.FileName = newFileName

                    End If

                End If

            Next
        Next

    End Sub

#End Region

    'Info log
    Public Shared Sub Info(ByVal info As String)
        LogUtil.m_log.Info(info)
    End Sub
    Public Shared Sub InfoException(ByVal ex As Exception)
        LogUtil.m_log.InfoException(ex)
    End Sub
    'Debug log
    Public Shared Sub Debug(ByVal debug As String)
        LogUtil.m_log.Debug(debug)
    End Sub
    Public Shared Sub DebugException(ByVal ex As Exception)
        LogUtil.m_log.DebugException(ex)
    End Sub
    'Error log
    Public Shared Sub [Error](ByVal [error] As String)
        LogUtil.m_log.[Error]([error])
    End Sub
    Public Shared Sub ErrorException(ByVal ex As Exception)
        LogUtil.m_log.[Error](ex)
    End Sub
    'Fatal log
    Public Shared Sub Fatal(ByVal fatal As String)
        LogUtil.m_log.Fatal(fatal)
    End Sub
    Public Shared Sub FatalException(ByVal ex As Exception)
        LogUtil.m_log.FatalException(ex)
    End Sub

    Public Shared Sub Warn(ByVal warn As String)
        LogUtil.m_log.Warn(warn)
    End Sub
    Public Shared Sub WarnException(ByVal ex As Exception)
        LogUtil.m_log.WarnException(ex)
    End Sub

    Public Shared Sub LogObject(ByVal obj As Object, Optional ByVal depth As Integer = 1)
        LogUtil.m_log.LogObject(obj)
    End Sub

End Class



''' <summary>
''' extension nlog.dll
''' </summary>
Public Module NLogExtension

    'info extension
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub InfoException(ByVal log As Logger, ByVal ex As Exception)
        log.InfoException(ex.StackTrace, ex)
    End Sub
    'debug extension
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub DebugException(ByVal log As Logger, ByVal ex As Exception)
        log.Debug(ex.StackTrace, ex)
    End Sub
    'error extension
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub ErrorException(ByVal log As Logger, ByVal ex As Exception)
        log.Error(ex.StackTrace, ex)
    End Sub
    'fatal extension
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub FatalException(ByVal log As Logger, ByVal ex As Exception)

        log.Fatal(ex.StackTrace, ex)
    End Sub

    <System.Runtime.CompilerServices.Extension()> _
    Public Sub WarnException(ByVal log As Logger, ByVal ex As Exception)

        log.Warn(ex.StackTrace, ex)
    End Sub



    <System.Runtime.CompilerServices.Extension()> _
    Public Sub LogObject(ByVal log As Logger, ByVal obj As Object, Optional ByVal depth As Integer = 1)

        Dim writer As New StringBuilderWriter(New System.Text.StringBuilder())
        ObjectDumper.Write(obj, depth, writer)

        log.Info(writer.GetContent())

    End Sub



End Module
