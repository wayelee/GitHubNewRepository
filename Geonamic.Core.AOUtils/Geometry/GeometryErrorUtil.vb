
'Imports System.Drawing
'Imports System.Runtime.InteropServices
''Imports Geonamic.Core.AOUtils
'Imports ESRI.ArcGIS.Geometry
'Imports ESRI.ArcGIS.esriSystem
'Imports ESRI.ArcGIS.Geodatabase



'Namespace Geometry


'    Public Class GeometryException
'        Inherits Exception

'        Public Sub New(ByVal comEx As COMException)
'            MyBase.New(comEx.Message, comEx)

'            Dim ex As Exception = Me.InnerException


'        End Sub


'        Public Overrides ReadOnly Property Message() As String
'            Get

'                Dim baseMessage As String = MyBase.Message
'                Dim msg As String = baseMessage & GetDescription(33)
'                Return MyBase.Message
'            End Get
'        End Property





'        Public Shared Function GetDescription(ByVal valve As String) As String

'            Dim err As esriGeometryError = [Enum].Parse(GetType(esriGeometryError), valve)
'            Return err.ToString()

'        End Function


'    End Class




'End Namespace

