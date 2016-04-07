
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Carto


Namespace Carto


    Public Class MapDocHandler

        Private _MxDoc As IMxDocument
        Private _MapDoc As IMapDocument


        Public Sub New(ByVal mxDoc As IMxDocument)
            _MxDoc = mxDoc
            _MapDoc = Nothing
        End Sub

        Public Sub New(ByVal mapDoc As IMapDocument)
            _MxDoc = Nothing
            _MapDoc = mapDoc
        End Sub

        Public ReadOnly Property ActiveView() As IActiveView
            Get
                If _MxDoc IsNot Nothing Then
                    Return _MxDoc.ActiveView
                Else
                    Return _MapDoc.ActiveView
                End If

            End Get

        End Property

        Public ReadOnly Property FocusMap() As IMap
            Get
                If _MxDoc IsNot Nothing Then
                    Return _MxDoc.FocusMap
                Else
                    Return _MapDoc.Map(0)
                End If
            End Get
        End Property

        Public ReadOnly Property PageLayout() As IPageLayout
            Get
                If _MxDoc IsNot Nothing Then
                    Return _MxDoc.PageLayout
                Else
                    Return _MapDoc.PageLayout
                End If
            End Get
        End Property


        Private _App As Object
        Public Property App() As Object
            Get
                Return _App
            End Get
            Set(ByVal value As Object)
                _App = value
            End Set
        End Property

        Public Function GetMxdFilePath() As String

            If _MxDoc IsNot Nothing Then

                Dim arcMapApp As IApplication = _App        
                Return FindMxdTemplate(arcMapApp)
            Else
                Return _MapDoc.DocumentFilename
            End If

        End Function

        Private Function FindMxdTemplate(ByVal arcMapApp As IApplication) As String

            For index As Integer = 0 To arcMapApp.Templates.Count
                Dim temp As String = arcMapApp.Templates.Item(index)

                Dim fi As New IO.FileInfo(temp)
                If fi.Extension.ToUpper() = ".mxd".ToUpper() Then
                    Return temp
                End If

            Next

            Return String.Empty

        End Function

        Public Sub SaveAs(ByVal path As String)

            If _MxDoc IsNot Nothing Then
                Dim arcMapApp As IApplication = _App
                arcMapApp.SaveAsDocument(path, True)
            Else
                _MapDoc.SaveAs(path, False)
            End If
        End Sub


    End Class

End Namespace