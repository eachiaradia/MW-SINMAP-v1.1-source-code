Imports MapWindow.Interfaces
Imports MapWinGIS
Imports MapWinGeoProc
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Text.RegularExpressions
Imports System.io

Module Functions
    Public Sub AddField(ByRef shpFile As MapWinGIS.Shapefile, ByVal fldName As String, ByVal fldType As MapWinGIS.FieldType)
        shpFile.StartEditingTable()
        Dim numField As Integer = shpFile.NumFields
        Dim newFld As New MapWinGIS.Field
        newFld.Name = fldName
        newFld.Type = fldType
        If Not fldType = FieldType.STRING_FIELD Then
            newFld.Width = 10
        Else
            newFld.Width = 255
        End If

        If fldType = FieldType.DOUBLE_FIELD Then newFld.Precision = 3
        If fldType = FieldType.INTEGER_FIELD Then newFld.Precision = 0

        shpFile.EditInsertField(newFld, numField)
        shpFile.StopEditingTable()
    End Sub
    Public Function existField(ByRef shpFile As MapWinGIS.Shapefile, ByVal fldName As String) As Integer
        existField = -1 'it returns -1 if field doesn't exist
        Dim j As Integer
        For j = 0 To shpFile.NumFields - 1
            If shpFile.Field(j).Name.ToLower() = fldName.ToLower Then
                existField = j
                Exit For
            End If
        Next j
    End Function
    Public Function ManageField(ByRef sf As MapWinGIS.Shapefile, ByVal fldnm As String, ByRef fldId As Integer) As Boolean
        ManageField = False
        fldId = existField(sf, fldnm)
        If fldId < 0 Then
            AddField(sf, fldnm, FieldType.DOUBLE_FIELD)
            fldId = sf.NumFields - 1
            ManageField = True
        End If
    End Function

    Public Function CreateSingleCalShpFile(ByVal grdName$, ByVal shpName$, ByRef store As MainClass) As Boolean
        CreateSingleCalShpFile = False
        Dim flg As Boolean
        Dim grid As New MapWinGIS.Grid
        grid.Open(grdName)
        Dim gridHd As MapWinGIS.GridHeader = grid.Header
        'grid extension
        Dim xmin, xmax, ymin, ymax As Double
        xmin = gridHd.XllCenter - gridHd.dX / 2
        xmax = xmin + (gridHd.NumberCols * gridHd.dX)
        ymin = gridHd.YllCenter - gridHd.dY / 2
        ymax = ymin + (gridHd.NumberRows * gridHd.dY)
        'create a new shapefile
        Dim newShpFile As New MapWinGIS.Shapefile
        flg = newShpFile.CreateNew(shpName, MapWinGIS.ShpfileType.SHP_POLYGON)
        If flg = False Then
            MsgBox("Can't create " & shpName & System.Environment.NewLine & _
                   "It is possible that the shapefile already exists")
            Exit Function
        End If


        'add fields and set default values
        AddField(newShpFile, "Region", FieldType.INTEGER_FIELD)
        AddField(newShpFile, "TRmin", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "TRmax", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "Cmin", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "Cmax", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "PHImin", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "PHImax", FieldType.DOUBLE_FIELD)
        AddField(newShpFile, "SoilDens", FieldType.DOUBLE_FIELD)

        flg = newShpFile.StopEditingShapes(True, True) 'it's necessary to work but I don't know why!!!
        'it always returns FALSE
        'If flg = False Then MsgBox("Can't stop editing FIRST" & shpName)
        flg = newShpFile.StartEditingShapes(True)
        If flg = False Then MsgBox("Can't start editing shapefile")

        'add only a shape (rectangle)
        Dim shp As New MapWinGIS.Shape
        flg = shp.Create(ShpfileType.SHP_POLYGON)
        If flg = False Then MsgBox("Can't create shape")
        addPoint2shape(shp, xmin, ymin, 0)
        addPoint2shape(shp, xmax, ymin, 1)
        addPoint2shape(shp, xmax, ymax, 2)
        addPoint2shape(shp, xmin, ymax, 3)
        flg = newShpFile.EditInsertShape(shp, 0)
        Dim errorcode$ = newShpFile.LastErrorCode
        If flg = False Then MsgBox("Can't insert shape " & errorcode)

        'populate table value
        flg = newShpFile.EditCellValue(0, 0, 1)
        If flg = False Then MsgBox("Can't populate table")
        newShpFile.EditCellValue(1, 0, store.TRlb)
        newShpFile.EditCellValue(2, 0, store.TRub)
        newShpFile.EditCellValue(3, 0, store.Clb)
        newShpFile.EditCellValue(4, 0, store.Cub)
        newShpFile.EditCellValue(5, 0, store.PHIlb)
        newShpFile.EditCellValue(6, 0, store.PHIub)
        newShpFile.EditCellValue(7, 0, store.soild)

        'stop, save and close
        flg = newShpFile.StopEditingShapes(True, True)
        If flg = False Then MsgBox("Can't stop editing " & shpName)
        flg = newShpFile.Close()
        If flg = False Then MsgBox("Can't close " & shpName)

        CreateSingleCalShpFile = True
    End Function
    Private Function SetDefaultCalValue(ByRef newShpFile As MapWinGIS.Shapefile, ByRef store As MainClass) As Boolean
        SetDefaultCalValue = False
        MsgBox("This function is not implemented")
    End Function

    Public Function CreateSingleCalGrid(ByVal newGrdName$, ByRef oldGrdName$) As Boolean
        Dim oldGrd As New MapWinGIS.Grid
        oldGrd.Open(oldGrdName, GridDataType.FloatDataType, True, GridFileType.UseExtension, Nothing)
        Dim newgrd As New MapWinGIS.Grid
        newgrd.CreateNew(newGrdName, oldGrd.Header, GridDataType.FloatDataType, oldGrd.Header.NodataValue)
        Dim nd As Single = oldGrd.Header.NodataValue
        'newgrd = CreateGrd(newGrdName, oldGrd)

        Dim data() As Single
        ReDim data((oldGrd.Header.NumberCols * oldGrd.Header.NumberRows) - 1)
        oldGrd.GetFloatWindow(0, oldGrd.Header.NumberRows - 1, 0, oldGrd.Header.NumberCols - 1, data(0))
        Dim temp As Single
        Dim i As Integer = 0
        
        For Each temp In data
            If temp <> nd Then
                data(i) = 1
            End If
            i = i + 1
        Next
        newgrd.PutFloatWindow(0, oldGrd.Header.NumberRows - 1, 0, oldGrd.Header.NumberCols - 1, data(0))

        'OLD LOOP
        'Dim r, c As Integer
        'For c = 0 To oldGrd.Header.NumberCols
        'For r = 0 To oldGrd.Header.NumberRows
        'If oldGrd.Value(c, r) <> nd Then
        'newgrd.Value(c, r) = 1
        ''Else
        ''newgrd.Value(c, r) = newgrd.Header.NodataValue
        'End If
        'Next
        'Next

        oldGrd.Close()
        newgrd.Save()
        newgrd.Close()
        CreateSingleCalGrid = True
    End Function
    Private Function CreateGrd(ByVal newGrdName$, ByRef grd As MapWinGIS.Grid) As MapWinGIS.Grid
        CreateGrd = New MapWinGIS.Grid
        'CreateGrd.Header.CopyFrom(grd.Header)
        CreateGrd.CreateNew(newGrdName, grd.Header, GridDataType.ShortDataType, grd.Header.NodataValue)

    End Function
    Private Sub addPoint2shape(ByRef shape As MapWinGIS.Shape, ByVal x As Double, ByVal y As Double, ByVal index As Integer)
        Dim flg As Boolean
        Dim point As New MapWinGIS.Point
        point.x = x
        point.y = y
        flg = shape.InsertPoint(point, index)
        If flg = False Then MsgBox("Can't insert point " & index)
    End Sub
    Public Function addLabel(ByVal origFileName$, ByVal label$) As String
        If origFileName <> "" Then
            Dim lastPtPos As Integer = origFileName.LastIndexOf(".")
            Dim ext$ = origFileName.Substring(lastPtPos)
            'MsgBox("last pos index: " & lastPtPos)
            'MsgBox("extension: " & ext)
            Dim newString$ = label & ext
            addLabel = Replace(origFileName, ext, newString)
        Else
            addLabel = label
        End If

    End Function
    Public Function addLabel(ByVal origFileName$, ByVal label$, ByVal newExt$) As String
        If origFileName <> "" Then
            Dim lastPtPos As Integer = origFileName.LastIndexOf(".")
            Dim ext$ = origFileName.Substring(lastPtPos)
            Dim newString$ = label & newExt
            addLabel = Replace(origFileName, ext, newString)
        Else
            addLabel = label
        End If

    End Function

    Public Function getSfColorScheme(ByVal FileType As Integer, ByVal mwLayerHandle As Integer, ByVal mwFieldIndex As Integer) As MapWinGIS.ShapefileColorScheme
        Dim CS As New MapWinGIS.ShapefileColorScheme
        Dim BR As New MapWinGIS.ShapefileColorBreak
        Dim i, nOfBrakes As Integer
        Try
            Select Case FileType
                Case 1 'Region shapefile
                    CS.LayerHandle = mwLayerHandle
                    CS.FieldIndex = mwFieldIndex

                    For i = 0 To nOfBrakes
                        BR = New MapWinGIS.ShapefileColorBreak
                        BR.Caption = "Region " & i
                        BR.StartValue = i
                        BR.EndValue = i
                        BR.StartColor = Convert.ToUInt32(RGB(255, 0, 0))
                        BR.EndColor = Convert.ToUInt32(RGB(0, 0, 255))
                        CS.Add(BR)
                    Next i
                    Return CS
                Case 2 'landslide
                    Return CS
            End Select
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return Nothing

    End Function

    Public Function GetColorScheme(ByVal FileType As Integer, ByVal min As Double, ByVal max As Double) As MapWinGIS.GridColorScheme
        Dim CS As New MapWinGIS.GridColorScheme
        Dim BR As New MapWinGIS.GridColorBreak
        Dim R1 As Integer, R2 As Integer, G1 As Integer, G2 As Integer, B1 As Integer, B2 As Integer
        Try
            Select Case FileType
                Case 1 'Base DEM and filled DEM
                    CS.UsePredefined(min, max, MapWinGIS.PredefinedColorScheme.SummerMountains)
                    Return CS
                Case 2 'Dinf slope
                    BR = New MapWinGIS.GridColorBreak ' white to green
                    BR.LowValue = 0 : BR.HighValue = 1
                    R1 = 255 : G1 = 255 : B1 = 255 : R2 = 0 : G2 = 255 : B2 = 0
                    BR.LowColor = System.Convert.ToUInt32(RGB(R1, G1, B1))
                    BR.HighColor = System.Convert.ToUInt32(RGB(R2, G2, B2))
                    BR.ColoringType = MapWinGIS.ColoringType.Gradient
                    CS.InsertBreak(BR)

                    BR = New MapWinGIS.GridColorBreak 'green to dark green
                    BR.LowValue = 1 : BR.HighValue = max
                    R1 = 0 : G1 = 255 : B1 = 0 : R2 = 0 : G2 = 100 : B2 = 0
                    BR.LowColor = System.Convert.ToUInt32(RGB(R1, G1, B1))
                    BR.HighColor = System.Convert.ToUInt32(RGB(R2, G2, B2))
                    BR.ColoringType = MapWinGIS.ColoringType.Gradient
                    CS.InsertBreak(BR)
                    Return CS
                Case 3 'Dinf flow dir - white to brown
                    BR.LowValue = min
                    BR.HighValue = max
                    R1 = 255 : G1 = 255 : B1 = 255 : R2 = 139 : G2 = 69 : B2 = 19
                    BR.LowColor = System.Convert.ToUInt32(RGB(R1, G1, B1))
                    BR.HighColor = System.Convert.ToUInt32(RGB(R2, G2, B2))
                    BR.ColoringType = MapWinGIS.ColoringType.Gradient
                    CS.InsertBreak(BR)
                    Return CS
                Case 4 'Dinf area - shading from white to red
                    BR = New MapWinGIS.GridColorBreak
                    BR.LowValue = min : BR.HighValue = max
                    BR.LowColor = System.Convert.ToUInt32(RGB(255, 255, 255))
                    BR.HighColor = System.Convert.ToUInt32(RGB(255, 0, 0))
                    BR.ColoringType = MapWinGIS.ColoringType.Gradient
                    BR.GradientModel = MapWinGIS.GradientModel.Logorithmic
                    CS.InsertBreak(BR)
                    Return CS
                Case 5 'Stability Index Map
                    CS.NoDataColor = System.Convert.ToUInt32(RGB(255, 255, 255))
                    Dim values() As Double = {0, 0, 0.5, 1, 1.25, 1.5, 10}
                    Dim captions() As String = {"Defended", "Upper threshold", "Lower threshold", "Quasi stable", "Moderately Stable", "Stable"}
                    Dim red() As Integer = {203, 255, 254, 254, 203, 203}
                    Dim green() As Integer = {203, 0, 152, 254, 203, 254}
                    Dim blue() As Integer = {152, 0, 152, 153, 254, 203}
                    Dim j As Integer
                    For j = 0 To values.Length - 2
                        BR = New MapWinGIS.GridColorBreak
                        BR.LowValue = values(j) : BR.HighValue = values(j + 1)
                        BR.LowColor = System.Convert.ToUInt32(RGB(red(j), green(j), blue(j)))
                        BR.HighColor = BR.LowColor
                        BR.ColoringType = MapWinGIS.ColoringType.Gradient
                        BR.Caption = captions(j)
                        CS.InsertBreak(BR)
                    Next
                    Return CS
                Case 6 'Saturation Map
                    CS.NoDataColor = System.Convert.ToUInt32(RGB(255, 255, 255))
                    Dim values() As Double = {0, 0.1, 1, 2, 3}
                    Dim captions() As String = {"Low moisture", "Partially wetted", "Threshold saturation", "Saturation zone"}
                    Dim red() As Integer = {254, 0, 0, 0}
                    Dim green() As Integer = {254, 203, 127, 0}
                    Dim blue() As Integer = {153, 0, 0, 254}
                    Dim j As Integer
                    For j = 0 To values.Length - 2
                        BR = New MapWinGIS.GridColorBreak
                        BR.LowValue = values(j) : BR.HighValue = values(j + 1)
                        BR.LowColor = System.Convert.ToUInt32(RGB(red(j), green(j), blue(j)))
                        BR.HighColor = BR.LowColor
                        BR.ColoringType = MapWinGIS.ColoringType.Gradient
                        BR.Caption = captions(j)
                        CS.InsertBreak(BR)
                    Next
                    Return CS
                Case Else
                    BR.LowValue = min
                    BR.HighValue = max
                    R1 = Int(Rnd() * 255) : R2 = Int(Rnd() * 255) : G1 = Int(Rnd() * 255) : G2 = Int(Rnd() * 255) : B1 = Int(Rnd() * 255) : B2 = Int(Rnd() * 255)
                    BR.LowColor = System.Convert.ToUInt32(RGB(R1, G1, B1))
                    BR.HighColor = System.Convert.ToUInt32(RGB(R2, G2, B2))
                    BR.ColoringType = MapWinGIS.ColoringType.Gradient
                    CS.InsertBreak(BR)
                    Return CS
            End Select
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return Nothing
    End Function

    Function Split2RGB(ByVal intValue As Integer, ByRef R As Integer, ByRef G As Integer, ByRef B As Integer) As Boolean
        Split2RGB = False
        R = intValue Mod 256
        G = (intValue / 256) Mod 256
        B = (intValue / (256 * 256))
        Split2RGB = True
    End Function

    Public Function MakeMultiCalRegGrd(ByVal sfName As String, ByVal grdName As String, ByVal demName As String, ByRef store As MainClass) As Boolean
        MakeMultiCalRegGrd = False
        Dim flg As Boolean
        'open shapefile
        Dim calSf As New MapWinGIS.Shapefile
        calSf.Open(sfName)
        'look for "Region" field (if not then exit sub)
        Dim fldId As Integer
        fldId = Functions.existField(calSf, "Region")

        If fldId = -1 Then
            MsgBox("Cannot find Region field")
            Exit Function
        End If
        Dim fldName As String = calSf.Field(fldId).Name

        flg = calSf.StartEditingShapes(True)
        If flg = False Then

            MsgBox("Can't start editing shapefile" & System.Environment.NewLine & _
                   "It is possible that the shapefile is already loaded by the program")
            Exit Function
        End If


        'add field if they don't exist
        'set temporary index of each field
        Dim f1, f2, f3, f4, f5, f6, f7 As Integer

        ManageField(calSf, "TRmin", f1)
        ManageField(calSf, "TRmax", f2)
        ManageField(calSf, "Cmin", f3)
        ManageField(calSf, "Cmax", f4)
        ManageField(calSf, "PHImin", f5)
        ManageField(calSf, "PHImax", f6)
        ManageField(calSf, "SoilDens", f7)

        flg = calSf.StopEditingShapes(True, True) 'it's necessary to work but I don't know why!!!
        'it always returns FALSE
        'If flg = False Then MsgBox("Can't stop editing FIRST" & shpName)
        flg = calSf.StartEditingShapes(True)
        If flg = False Then
            MsgBox("Can't start editing shapefile")

        End If


        'populate table value
        Dim s As Integer
        For s = 0 To calSf.NumShapes - 1
            calSf.EditCellValue(f1, s, store.TRlb)
            calSf.EditCellValue(f2, s, store.TRub)
            calSf.EditCellValue(f3, s, store.Clb)
            calSf.EditCellValue(f4, s, store.Cub)
            calSf.EditCellValue(f5, s, store.PHIlb)
            calSf.EditCellValue(f6, s, store.PHIub)
            calSf.EditCellValue(f7, s, store.soild)
        Next
        'stop, save and close
        flg = calSf.StopEditingShapes(True, True)
        If flg = False Then MsgBox("Can't stop editing " & sfName)
        calSf.Close()

        Dim demGrd As New MapWinGIS.Grid
        demGrd.Open(demName)
        'convert shape to grid using Region value

        flg = MapWinGeoProc.Utils.ShapefileToGrid(sfName, grdName, GridFileType.UseExtension, GridDataType.FloatDataType, fldName, demGrd.Header, Nothing)
        demGrd.Close()
        If flg = False Then
            MsgBox("Cannot create region grid")
            Exit Function
        End If
        MakeMultiCalRegGrd = True

    End Function
    Public Function IsAlreadyLoaded(ByVal layerName As String, ByVal store As MainClass) As Integer
        IsAlreadyLoaded = -1
        Try
            Dim i As Integer
            For i = 0 To store.m_Map.Layers.NumLayers - 1
                If store.m_Map.Layers(i).FileName = layerName Then
                    IsAlreadyLoaded = i
                End If
            Next
        Catch ex As Exception
            IsAlreadyLoaded = -1
        End Try

    End Function

    Public Function AddSpace(ByVal txt As String, ByVal space As Integer, ByVal sep As String) As String
        AddSpace = txt
        Dim numChar As Integer = txt.Length
        Dim nSpace, i As Integer
        Dim temp As String = ""
        If numChar < space Then
            nSpace = space - numChar
            nSpace = Math.Ceiling(nSpace / 2)
            For i = 0 To nSpace
                temp = temp & sep
            Next
            temp = temp & txt
            For i = (nSpace + numChar) To space
                temp = temp & sep
            Next
            AddSpace = temp
        End If
    End Function
    Public Function Addspace(ByVal txt As String, ByVal space As Integer) As String
        Addspace = txt
        Dim numChar As Integer = txt.Length
        Dim nSpace As Integer
        If numChar < space Then
            nSpace = space - numChar
            nSpace = Math.Ceiling(nSpace / 2)
            Addspace = vbTab & txt & vbTab

        End If
    End Function

    Public Function sum(ByRef data() As Single) As Single
        sum = 0
        Dim i As Integer
        For i = 0 To UBound(data)
            sum = sum + data(i)
        Next
        Return sum
    End Function

    Public Function runCmd(ByVal pluginPath As String, ByVal processCommand As String, ByVal processParam As String) As Integer
        Dim sinmapInfo As New ProcessStartInfo
        Dim sinmapProcess As New Process

        ' create a new process
        processCommand = processCommand & " " & processParam 'not shure but probably you have to build the string before pass it


        sinmapProcess.StartInfo.FileName = pluginPath & "sinmap.exe"
        sinmapProcess.StartInfo.Arguments = processCommand
        
        sinmapProcess.Start()

        ' wait until it exits
        sinmapProcess.WaitForExit()
        Dim err As Integer = sinmapProcess.ExitCode


        sinmapProcess.Close()

        Return err

    End Function
End Module
