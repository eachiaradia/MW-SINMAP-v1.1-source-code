Imports MapWindow
Imports MapWinGIS
Imports ZedGraph
Public Class CalRegManager
    'manage opening shapefile, load calibration parameter, make statistics and lunch user interface

    Dim cr() As Integer

    Dim baseWetCrv, unsatCrv, satCrv As ZedGraph.LineItem
    Dim si0, si05, si1, si125, si15 As ZedGraph.LineItem

    Dim isGridsOpen As Boolean = False
    Public slopeGrd As MapWinGIS.Grid
    Public contrAreaGrd As MapWinGIS.Grid
    Public CalRegGrd As MapWinGIS.Grid

    Dim calSf As New MapWinGIS.Shapefile

    Public store As MainClass
    Public regNum As Integer
    Public calRegs As Double(,)
    Public isLoaded As Boolean = False
    Public pxArea As Double

    Dim formChart As frmChart
    Dim formParAdj As frmCalParAdjust

    Dim regFldId, TRminFldId, TRmaxFldId, CminFldId, CmaxFldId, PHIminFldid, PHImaxFldId, SoilDensFldId As Integer

  
    Public Sub OpenCalibrationFiles()
        'Try
        'it supposes that all file exist !!!
        'open calibration region grid file
        CalRegGrd = New MapWinGIS.Grid
        CalRegGrd.Open(store.calGrdFileName)

        'get max value of the grid
        regNum = CalRegGrd.Maximum
        Dim c, r, t, temp As Integer

        If regNum = 1 Then 'there is only one region of calibration (EAC 13-01-09)
            regNum = 0
            ReDim cr(regNum)
            cr(0) = 1
        Else
            'create an array of region id
            ReDim cr(regNum)

            'set all cr value to -1 (No region)
            Dim i As Integer
            For i = 0 To regNum
                cr(i) = -1
            Next
            'MsgBox("OK cr initialization: regnum = " & regNum)

            'loop throw the region grid and add region id
            temp = -1
            Dim itExists As Boolean
            Dim value As Double
            Dim nd As Double = CalRegGrd.Header.NodataValue

            For c = 0 To CalRegGrd.Header.NumberCols
                For r = 0 To CalRegGrd.Header.NumberRows
                    value = CalRegGrd.Value(c, r)
                    If Not value = nd Then
                        'if value already exists, skip else add to the array
                        itExists = False
                        For t = 0 To temp
                            If cr(t) = value Then itExists = True
                        Next
                        If itExists = False Then
                            temp = temp + 1
                            cr(temp) = value
                        End If
                    End If
                Next
            Next


            regNum = temp
            ReDim Preserve cr(regNum)
            Array.Sort(cr)

        End If

        pxArea = CalRegGrd.Header.dX * CalRegGrd.Header.dY
        CalRegGrd.Close()

        'MsgBox("OK cr reallocation: regnum = " & regNum & " temp = " & temp)
        'open calibration region shapefile
        calSf = New MapWinGIS.Shapefile
        calSf.Open(store.calFileName)

        'MsgBox("error: " + calSf.ErrorMsg(calSf.LastErrorCode))

        calSf.StartEditingTable()
        'controll if calibration fields exist
        'if no, add field and store field index
        Dim isNew As Boolean
        'look for region id column
        'EAC 28 nov 08
        ManageField(calSf, "region", regFldId)
        isNew = ManageField(calSf, "TRmin", TRminFldId)
        ManageField(calSf, "TRmax", TRmaxFldId)
        ManageField(calSf, "Cmin", CminFldId)
        ManageField(calSf, "Cmax", CmaxFldId)
        ManageField(calSf, "PHImin", PHIminFldid)
        ManageField(calSf, "PHImax", PHImaxFldId)
        ManageField(calSf, "SoilDens", SoilDensFldId)
        calSf.StopEditingTable()

        'create region array and initialize value with existing or default data
        ReDim calRegs(7, regNum)
        Dim s As Integer

        For r = 0 To regNum
            'inizialize parameter
            s = 0
            Do Until calSf.CellValue(regFldId, s) = (cr(r))
                s = s + 1
            Loop
            calRegs(0, r) = cr(r)
            calRegs(1, r) = calSf.CellValue(TRminFldId, s)
            calRegs(2, r) = calSf.CellValue(TRmaxFldId, s)
            calRegs(3, r) = calSf.CellValue(CminFldId, s)
            calRegs(4, r) = calSf.CellValue(CmaxFldId, s)
            calRegs(5, r) = calSf.CellValue(PHIminFldid, s)
            calRegs(6, r) = calSf.CellValue(PHImaxFldId, s)
            calRegs(7, r) = calSf.CellValue(SoilDensFldId, s)

        Next

        'save data if they are new else write the same (call general function)
        SaveCalibrationParam()
        MsgBox("Complete Calibration Region Loading!")
        isLoaded = True
        'Catch ex As Exception
        'isLoaded = False
        'DO NOTHING
        'End Try
    End Sub

    

    Public Sub SaveCalibrationParam()
        Try
            'start editing session
            calSf.StartEditingTable()
            Dim r, s As Integer
            For r = 0 To regNum
                'save parameter
                For s = 0 To calSf.NumShapes - 1
                    If calSf.CellValue(regFldId, s) = calRegs(0, r) Then
                        calSf.EditCellValue(TRminFldId, s, calRegs(1, r))
                        calSf.EditCellValue(TRmaxFldId, s, calRegs(2, r))
                        calSf.EditCellValue(CminFldId, s, calRegs(3, r))
                        calSf.EditCellValue(CmaxFldId, s, calRegs(4, r))
                        calSf.EditCellValue(PHIminFldid, s, calRegs(5, r))
                        calSf.EditCellValue(PHImaxFldId, s, calRegs(6, r))
                        calSf.EditCellValue(SoilDensFldId, s, calRegs(7, r))
                    End If
                Next
            Next
            calSf.StopEditingTable()
        Catch ex As Exception
            MsgBox("Can't save calibration region parameter in calibration region shapefile " + ex.Message)
        End Try
    End Sub

    Public Function GetRealRegionId(ByVal arrayPos As Integer)
        GetRealRegionId = -1
        GetRealRegionId = cr(arrayPos)

    End Function

    Public Sub PlotData()
        formChart = New frmChart(store)
        formChart.SetTitles("SA Plot", "Slope (degree)", "Contributing Area ")
        'update legend
        formChart.addRegion2List(cr)
        'For i = 0 To regNum
        ' formChart.ListBox1.Items.Add("Region " & cr(i))
        'Next

        formChart.Show()
    End Sub

    Public Sub ParamAdjust()
        formParAdj = New frmCalParAdjust
        formParAdj.UpLoadParameter(Me, -1)
        formParAdj.ShowDialog()
    End Sub

    Public Sub New(ByRef pluginObj As MainClass)
        store = pluginObj
    End Sub

End Class

Public Class CalRegion

#Region "Common variables"




    Public calName As String
    Public id As Integer
    Public chart As ZedGraph.GraphPane
    Public store As CalRegManager


#End Region

#Region "Calibration variables"
    Public TRlb As Double = 2000
    Public TRub As Double = 3000
    Public Clb As Double = 0
    Public Cub As Double = 0.25
    Public PHIlb As Double = 30
    Public PHIub As Double = 45
    Public soild As Double = 2000
#End Region

    Private Sub DrawCurves()
        'TODO:
        'implement drawing curves
    End Sub

    Public Sub InitializeParam(ByVal TRmin As Double, ByVal TRmax As Double, ByVal Cmin As Double, ByVal Cmax As Double, ByVal PHImin As Double, ByVal PHImax As Double, ByVal sd As Double)
        TRlb = TRmin
        TRub = TRmax
        Clb = Cmin
        Cub = Cmax
        PHIlb = PHImin
        PHIub = PHImax
        soild = sd
    End Sub

    Public Sub SetChart(ByRef gPane As ZedGraph.GraphPane)
        chart = gPane
    End Sub

    Public Sub New(ByVal regionID As Integer, ByRef regManager As CalRegManager, ByRef gPane As ZedGraph.GraphPane)
        Try
            store = regManager
            chart = gPane

            id = regionID
            calName = "Region " & id

        Catch ex As Exception
            MsgBox("Can't create region  " + ex.Message)
        End Try

    End Sub
End Class
