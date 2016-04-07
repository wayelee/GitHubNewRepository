
Imports System.Drawing
Imports System.Windows.Forms


Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Geometry

Namespace Rendering

    Public Class ISymbolUtil

        Public Shared Function CreateColor(ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer) As IColor
            Dim color As IRgbColor = New RgbColorClass()
            color.Red = red
            color.Green = green
            color.Blue = blue

            Return color
        End Function

        Public Shared Function CreateColor(ByVal c As System.Drawing.Color) As IColor
            Dim color As IRgbColor = New RgbColorClass()
            color.Red = c.R
            color.Green = c.G
            color.Blue = c.B
            color.Transparency = c.A

            Return color
        End Function


    End Class

End Namespace
