Imports System.Windows.Forms

Imports ESRI.ArcGIS.Carto
Imports Geonamic.Core.AOUtils.Carto

Namespace Carto



    Public Class SetSelectableLayersDialog

        

        Private _Map As IMap

        Public Sub New(ByVal map As IMap)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _Map = map
            PopulateLyrs()
        End Sub

        Private Sub PopulateLyrs()

            Dim enumLayer As IEnumLayer = _Map.Layers
            Dim lyr As ILayer = enumLayer.Next

            Dim lyrIndex As Integer = -1

            While Not lyr Is Nothing

                lyrIndex = lyrIndex + 1


                If TypeOf lyr Is IFeatureLayer Then
                    Dim featLyr As IFeatureLayer = lyr
                    cklstLayers.Items.Add(CreateChkItem(lyr.Name, lyrIndex), featLyr.Selectable)
                End If

                lyr = enumLayer.Next
            End While

        End Sub

        Private Function CreateChkItem(ByVal lyrName As String, ByVal lyrIndex As Integer) As LyrInfo

            Return New LyrInfo(lyrName, lyrIndex)

        End Function




        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub



        Private Sub SetSelectableLayersDialog_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
        End Sub


        Private Sub cklstLayers_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles cklstLayers.ItemCheck

            Dim item As LyrInfo = cklstLayers.Items(e.Index)
            Dim featLyr As IFeatureLayer = FindLyrByIndex(item.Index) ' _Map.Layer(item.Index)
            featLyr.Selectable = (e.NewValue = CheckState.Checked)

        End Sub


        Private Function FindLyrByIndex(ByVal i As Integer) As ILayer

            Dim enumLayer As IEnumLayer = _Map.Layers
            Dim lyr As ILayer = enumLayer.Next

            Dim lyrIndex As Integer = 0
            While Not lyr Is Nothing

                If lyrIndex = i Then
                    Return lyr
                End If

                lyrIndex = lyrIndex + 1
                lyr = enumLayer.Next

            End While

            Return Nothing

        End Function


        Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click

            For i As Integer = 0 To cklstLayers.Items.Count - 1
                cklstLayers.SetItemChecked(i, True)
            Next


        End Sub

        Private Sub btnClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearAll.Click
            For i As Integer = 0 To cklstLayers.Items.Count - 1
                cklstLayers.SetItemChecked(i, False)
            Next
        End Sub

    End Class

End Namespace
