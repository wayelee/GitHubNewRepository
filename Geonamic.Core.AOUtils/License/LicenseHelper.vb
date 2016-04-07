Imports ESRI.ArcGIS.esriSystem


Namespace License



    Public Enum ESRILicenseProductEnum

        ArcView
        ArcEditor
        ArcEngine
        ArcInfo

        Any

    End Enum


    Public Class LicenseHelper
        Implements IDisposable


        Private m_pAoInitialize As AoInitialize

        Public Sub New()

            '#If CONFIG = "Release" Then 'For arcgis 10
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop)
'#End If
            m_pAoInitialize = New AoInitialize()
        End Sub

        Private Function ConvertToLicenseProductCode(licProd As ESRILicenseProductEnum) As esriLicenseProductCode

            Select Case licProd
                Case ESRILicenseProductEnum.ArcView
					Return esriLicenseProductCode.esriLicenseProductCodeBasic
                Case ESRILicenseProductEnum.ArcEditor
					Return esriLicenseProductCode.esriLicenseProductCodeStandard
                Case ESRILicenseProductEnum.ArcEngine
                    Return esriLicenseProductCode.esriLicenseProductCodeEngine
                Case ESRILicenseProductEnum.ArcInfo
					Return esriLicenseProductCode.esriLicenseProductCodeAdvanced
            End Select

        End Function

        Public Sub TryInitialize(Optional product As ESRILicenseProductEnum = ESRILicenseProductEnum.ArcView)

            If product = ESRILicenseProductEnum.Any Then

				If m_pAoInitialize.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeAdvanced) = esriLicenseStatus.esriLicenseAvailable Then
					m_pAoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced)
				ElseIf m_pAoInitialize.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeStandard) = esriLicenseStatus.esriLicenseAvailable Then
					m_pAoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard)
				ElseIf m_pAoInitialize.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeBasic) = esriLicenseStatus.esriLicenseAvailable Then
					m_pAoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeBasic)
				Else
					Throw New Exception("License not available. Check your network connection")
				End If

            Else

                Dim esriProd As esriLicenseProductCode = ConvertToLicenseProductCode(product)

                If m_pAoInitialize.IsProductCodeAvailable(esriProd) = esriLicenseStatus.esriLicenseAvailable Then
                    m_pAoInitialize.Initialize(esriProd)
                Else
                    Throw New Exception(esriProd.ToString() & " license not available. Check your network connection")
                End If


            End If


        End Sub



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.

                m_pAoInitialize = Nothing

            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class


End Namespace
