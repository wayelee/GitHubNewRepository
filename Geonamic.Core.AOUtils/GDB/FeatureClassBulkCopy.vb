Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry


Namespace GDB


    Public Class FeatureClassBulkCopy

        Private _DestinationWorkspace As IFeatureWorkspace

        Private _DestinationName As String
        Public Property DestinationName() As String
            Get
                Return _DestinationName
            End Get
            Set(ByVal value As String)
                _DestinationName = value
            End Set
        End Property


        Private _DestinationSpatialReference As ISpatialReference
        Public Property DestinationSpatialReference() As ISpatialReference
            Get
                Return _DestinationSpatialReference
            End Get
            Set(ByVal value As ISpatialReference)
                _DestinationSpatialReference = value
            End Set
        End Property



        Private _BulkFeatureCount As Integer = 10
        Public Property BulkFeatureCount() As Integer
            Get
                Return _BulkFeatureCount
            End Get
            Set(ByVal value As Integer)

                If value < 10 Then
                    _BulkFeatureCount = 10
                Else
                    _BulkFeatureCount = value
                End If

            End Set
        End Property



        Private _NeedCopyZ As Boolean

        Private _NeedProjectGeometry As Boolean = False


        Public Event OnFeaturesCopied(ByVal featuresCopied As Integer, ByVal sourceFeatCount As Integer)


        Public Sub New(ByVal destinationWorkspace As IFeatureWorkspace)
            _DestinationWorkspace = destinationWorkspace
        End Sub

        Public Sub WriteToWorkspace(ByVal sourceFeatureClass As IFeatureClass, Optional selelectSet As ISelectionSet = Nothing)

            If (String.IsNullOrEmpty(Me.DestinationName)) Then
                Me.DestinationName = sourceFeatureClass.AliasName
            End If

            If WorkspaceUtil.FeatureClassExists(_DestinationWorkspace, Me.DestinationName) Then
                Throw New Exception("FeatureClass:" & Me.DestinationName & " already exists.")
            End If

            ConfigSpatialReference(sourceFeatureClass)

            Dim editWS As IWorkspaceEdit = _DestinationWorkspace
            Try
                editWS.StartEditing(True)

                Dim desFC As IFeatureClass = CopySchema(sourceFeatureClass)
                CopyFeatures(sourceFeatureClass, desFC, selelectSet)

                editWS.StopEditing(True)
            Catch ex As Exception
                editWS.AbortEditOperation()
                Throw ex
            End Try
        End Sub

        Private Sub ConfigSpatialReference(ByVal sourceFeatureClass As IFeatureClass)

            Dim sourceSR As ISpatialReference = FeatureClassUtil.GetSpatialReference(sourceFeatureClass)
            If (Me.DestinationSpatialReference Is Nothing) Then
                Me.DestinationSpatialReference = sourceSR
                _NeedProjectGeometry = False
            ElseIf Me.DestinationSpatialReference.FactoryCode = sourceSR.FactoryCode Then
                _NeedProjectGeometry = False
            Else
                _NeedProjectGeometry = True
            End If


        End Sub

        Private Function CopySchema(ByVal sourceFeatureClass As IFeatureClass) As IFeatureClass

            Dim sourceFields As IFields = sourceFeatureClass.Fields

            Dim fieldsEdit As IFieldsEdit = New Fields()
            fieldsEdit.FieldCount_2 = sourceFields.FieldCount

            For index As Integer = 0 To sourceFields.FieldCount - 1

                Dim sourceFld As IField = sourceFields.Field(index)
                If sourceFld.Name = sourceFeatureClass.ShapeFieldName Then

                    Dim sourceGeomDef As IGeometryDef = sourceFld.GeometryDef

                    ' Shape
                    Dim geometryDef As IGeometryDef = New GeometryDefClass()
                    Dim geometryDefEdit As IGeometryDefEdit = CType(geometryDef, IGeometryDefEdit)
                    geometryDefEdit.GeometryType_2 = sourceGeomDef.GeometryType
                    'SetMForDefAndSR(geometryDef, sourceGeomDef, Me.DestinationSpatialReference)

                    geometryDefEdit.HasM_2 = sourceGeomDef.HasM
                    If sourceGeomDef.HasM Then
                        Me.DestinationSpatialReference.SetMDomain(0, Double.MaxValue)
                    End If

                    _NeedCopyZ = sourceGeomDef.HasZ AndAlso DestinationSpatialReferenceHasZ()
                    geometryDefEdit.HasZ_2 = _NeedCopyZ
                    geometryDefEdit.SpatialReference_2 = Me.DestinationSpatialReference
                    'geometryDefEdit.GridCount_2 = 1
                    'geometryDefEdit.GridSize_2(0) = CountGridSize(_DestinationSpatialReference)

                   
                    Dim fieldEditShape As IFieldEdit = New Field()
                    fieldEditShape.Name_2 = sourceFeatureClass.ShapeFieldName
                    fieldEditShape.Type_2 = esriFieldType.esriFieldTypeGeometry
                    fieldEditShape.GeometryDef_2 = geometryDef

                    fieldsEdit.Field_2(index) = fieldEditShape
                Else

                    Dim fieldEdit As IFieldEdit = New Field()
                    fieldEdit.Name_2 = sourceFld.Name
                    fieldEdit.Type_2 = sourceFld.Type
                    fieldsEdit.Field_2(index) = fieldEdit

                End If

            Next

            Dim result As IFeatureClass = _
             _DestinationWorkspace.CreateFeatureClass(Me.DestinationName, fieldsEdit, _
                                                      Nothing, Nothing, _
                                                      esriFeatureType.esriFTSimple, _
                                                      sourceFeatureClass.ShapeFieldName, "")

            Return result


        End Function

        Private Function CountGridSize(ByVal pSpaRef As ISpatialReference)

            Dim dGridSize As Integer
            If (pSpaRef.HasXYPrecision) Then
                Dim xmin As Double, ymin As Double, xmax As Double, ymax As Double
                pSpaRef.GetDomain(xmin, xmax, ymin, ymax)
                Dim dArea As Double
                dArea = Math.Abs((xmax - xmin) * (ymax - ymin))
                dGridSize = Math.Sqrt(dArea / 100)
            Else
                dGridSize = 190000 ' 2015-10 1000
            End If

            Return dGridSize
        End Function

        Private Sub SetMForDefAndSR(ByRef geometryDefEdit As IGeometryDef, _
                                    ByVal sourceGeomDef As IGeometryDef, ByRef sr As ISpatialReference)

            geometryDefEdit.HasM_2 = sourceGeomDef.HasM

            If sourceGeomDef.HasM Then
                sr.SetMDomain(0, Double.MaxValue)
            End If

        End Sub

        Private Sub CopyFeatures(ByVal sourceFeatureClass As IFeatureClass, ByVal desFC As IFeatureClass, Optional selelectSet As ISelectionSet = Nothing)

            Dim desFeatBuffer As IFeatureBuffer = desFC.CreateFeatureBuffer()
            Dim cursor As IFeatureCursor = desFC.Insert(True)


            Dim featIndex As Integer = 1

            Dim sourceCursor As IFeatureCursor
            If selelectSet Is Nothing Then
                sourceCursor = sourceFeatureClass.Search(Nothing, False)
            Else
                Dim csr As ICursor = Nothing
                selelectSet.Search(Nothing, False, csr)
                sourceCursor = csr
            End If

            Dim souceFeatCount As Integer = sourceFeatureClass.FeatureCount(Nothing)
            Dim sourceFeat As IFeature = sourceCursor.NextFeature()
            While sourceFeat IsNot Nothing

                CopyFeature(sourceFeat, desFeatBuffer, desFC.ShapeFieldName)
                cursor.InsertFeature(desFeatBuffer)

                RaiseEvent OnFeaturesCopied(featIndex, souceFeatCount)
                If featIndex Mod BulkFeatureCount Then
                    cursor.Flush()
                End If

                featIndex = featIndex + 1

                sourceFeat = sourceCursor.NextFeature()

            End While

            cursor.Flush()

        End Sub

        Private Function ProjectGeometry(ByVal sourceGeom As IGeometry) As IGeometry

            If _NeedProjectGeometry Then
                sourceGeom.Project(Me.DestinationSpatialReference)
            End If

            If Not _NeedCopyZ Then
                Dim zA As IZAware = sourceGeom
                zA.ZAware = False
            End If


            Return sourceGeom

        End Function


        Private Function DestinationSpatialReferenceHasZ() As Boolean

            If Me.DestinationSpatialReference.HasZPrecision AndAlso Me.DestinationSpatialReference.ZCoordinateUnit IsNot Nothing Then
                Return True
            End If

            Return False

        End Function


        Private Sub CopyFeature(ByVal sourceFeat As IFeature, ByVal desFeatBuffer As IFeatureBuffer, ByVal decFCShapeFieldName As String)

            desFeatBuffer.Shape = ProjectGeometry(sourceFeat.ShapeCopy())

            Dim sourceFields As IFields = sourceFeat.Fields
            For fldIndex As Integer = 0 To sourceFields.FieldCount - 1

                Dim sourceField As IField = sourceFields.Field(fldIndex)
                Dim sourceFldName As String = sourceField.Name

                If Not IsReadonlyField(sourceField) Then

                    Dim desFldIndex As Integer = desFeatBuffer.Fields.FindField(sourceFldName)

                    If desFldIndex >= 0 Then
                        desFeatBuffer.Value(desFldIndex) = sourceFeat.Value(fldIndex)
                    End If

                End If

            Next

        End Sub

        Private Function IsReadonlyField(ByVal fld As IField)
            If fld.Type = esriFieldType.esriFieldTypeOID Or _
                 fld.Type = esriFieldType.esriFieldTypeGeometry Or _
                fld.Name.ToUpper() = "SHAPE_LENGTH" Or fld.Name.ToUpper() = "SHAPE_AREA" Then
                Return True
            Else
                Return False
            End If

        End Function


    End Class






End Namespace