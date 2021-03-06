Imports ZedGraph
Imports System.Drawing
Imports system.Windows.Forms

Public Class frmChart
    Dim myPane As GraphPane
    Dim store As MainClass
    Dim selReg As Integer

    Dim randPt As ZedGraph.LineItem
    Dim lsPt As ZedGraph.LineItem
    Dim calLn As ZedGraph.LineItem

    Dim isGridsOpen As Boolean = False
    Public slopeGrd As MapWinGIS.Grid
    Public contrAreaGrd As MapWinGIS.Grid
    Public CalRegGrd As MapWinGIS.Grid
    Dim pxDim As Double

    Dim title As String
    Dim regList() As Integer
    Public Sub addRegion2List(ByRef id() As Integer)
        Dim i As Integer
        ReDim regList(UBound(id))
        For i = 0 To UBound(id)
            regList(i) = id(i)
            Me.ListBox1.Items.Add("Region " & id(i))
        Next i
    End Sub

    Public Sub SetTitles(ByVal mainTitle$, ByVal xTitle$, ByVal yTitle$)
        ' Set the titles and axis labels
        title = mainTitle   'so I can modify it every time I load a new region
        myPane = Me.ZedGraphControl1.GraphPane
        myPane.Title.Text = mainTitle
        myPane.XAxis.Title.Text = xTitle
        myPane.YAxis.Title.Text = yTitle
        'set Y axis as logaritmic
        myPane.YAxis.Type = AxisType.Log

        'set legend on the right
        myPane.Legend.Position = LegendPos.InsideTopRight

        'initialize raster
        OpenOutputGrid()
    End Sub

    Private Sub OpenOutputGrid()
        slopeGrd = New MapWinGIS.Grid
        contrAreaGrd = New MapWinGIS.Grid
        CalRegGrd = New MapWinGIS.Grid
        Try
            CalRegGrd.Open(store.calGrdFileName)
            slopeGrd.Open(store.slopeFileName)
            contrAreaGrd.Open(store.contrAreaFileName)
            isGridsOpen = True
        Catch ex As Exception
            MsgBox("Can't open output file. " & ex.ToString)
        End Try
    End Sub

    Private Sub CloseOutputGrid()
        CalRegGrd.Close()
        slopeGrd.Close()
        contrAreaGrd.Close()
        isGridsOpen = False
    End Sub

    Private Sub LoadRandomPoint(ByVal id As Integer)

        Try
            'prepare storing random data
            Dim list2 = New PointPairList()
            'MsgBox("loading data")

            Dim npoints As Integer = store.numSAplot
            Dim mval As Double = slopeGrd.Header.NodataValue
            Dim slpValue, caValue As Double

            Dim nx, ny, randomrow, randomcol, j As Integer
            nx = slopeGrd.Header.NumberCols
            ny = slopeGrd.Header.NumberRows
            'MsgBox("nx: " & nx)

            Dim rnd As Random = New Random(DateTime.Now.Millisecond)
            'MsgBox("before trying random")
            Do While (j < npoints)
                randomrow = CType((rnd.NextDouble() * ny), Integer)
                randomcol = CType((rnd.NextDouble() * nx), Integer)
                'MsgBox("randomrow =  " & randomrow)
                'MsgBox("randomcol =  " & randomcol)

                If ((randomcol > 1 And randomrow > 1) And (randomcol < nx And randomrow < ny)) Then
                    'MsgBox("id =  " & id)
                    If (id >= 0) Then

                        If (CalRegGrd.Value(randomcol, randomrow) = store.regManager.calRegs(0, id)) Then
                            'get slope and contrarea value
                            slpValue = slopeGrd.Value(randomcol, randomrow)
                            caValue = contrAreaGrd.Value(randomcol, randomrow)
                            'MsgBox("slpValue = " & slpValue)
                            'MsgBox("caValue = " & caValue)
                            If ((slpValue <> mval) And (caValue <> mval)) Then
                                slpValue = Math.Atan(slpValue)
                                slpValue = 180 * slpValue / Math.PI
                                list2.Add(Math.Round(slpValue, 2), Math.Round(caValue, 2))
                                j = j + 1
                            End If
                        End If
                    End If
                End If
            Loop

            'set chart series
            randPt = myPane.AddCurve("Random point", list2, Drawing.Color.Black, SymbolType.Star)
            randPt.Symbol.Fill = New Fill(Drawing.Color.White)
            randPt.Symbol.Size = 0.1
            randPt.Line.IsVisible = False
            'randPt.Symbol.IsVisible = False
            'myPane.Legend.IsVisible = False

        Catch ex As Exception

            MsgBox("Can't load random point " + ex.Message)

        End Try

    End Sub

    Private Sub LoadLandSlidePoint(ByVal id As Integer)
        Try

            'prepare storing random data
            Dim list = New PointPairList()

            'open landslides point shapefile
            Dim lsSf As New MapWinGIS.Shapefile
            lsSf.Open(store.slpFileName)

            Dim mval As Double = slopeGrd.Header.NodataValue
            Dim slpValue, caValue As Double

            'for each landslide, get region and slope and contrarea characteristics
            Dim s As Integer = 0
            Dim x, y As Double
            Dim c, r As Integer
            Do While s < lsSf.NumShapes
                x = lsSf.Shape(s).Point(0).x
                y = lsSf.Shape(s).Point(0).y
                CalRegGrd.ProjToCell(x, y, c, r)
                If (CalRegGrd.Value(c, r) = store.regManager.calRegs(0, id)) Then
                    'get slope and contrarea value
                    slpValue = slopeGrd.Value(c, r)
                    caValue = contrAreaGrd.Value(c, r)
                    If ((slpValue <> mval) And (caValue <> mval)) Then
                        slpValue = Math.Atan(slpValue)
                        slpValue = 180 * slpValue / Math.PI
                        'list.Add(slpValue, caValue)
                        list.Add(slpValue, caValue, s + 1)

                    End If
                End If
                s = s + 1
            Loop

            'close all file
            lsSf.Close()

            'set chart series
            lsPt = myPane.AddCurve("Landslide", list, Drawing.Color.Black, SymbolType.Circle)
            lsPt.Symbol.Fill = New Fill(Drawing.Color.Red)
            lsPt.Symbol.Size = 7
            'lsPt.Symbol.IsVisible = False
            lsPt.Line.IsVisible = False
            'myPane.Legend.IsVisible = False

        Catch ex As Exception

            MsgBox("Can't load landslide data " + ex.Message)

        End Try
    End Sub

#Region "original sinmap code"

    'According to the Authors, this part ("original sinmap code" region)of the code
    'is released under the MIT License

    'Copyright (c) <2009> <original SINMAP code Authors>

    'Permission is hereby granted, free of charge, to any person obtaining a copy
    'of this software and associated documentation files (the "Software"), to deal
    'in the Software without restriction, including without limitation the rights
    'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    'copies of the Software, and to permit persons to whom the Software is
    'furnished to do so, subject to the following conditions:

    'The above copyright notice and this permission notice shall be included in
    'all copies or substantial portions of the Software.

    'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    'THE SOFTWARE.

    Private Sub AddCalibrationLines(ByVal id As Integer)
        'Dim annotx(5) As Double
        Dim pi As Double = Math.PI
        Dim k As Single, a As Double, p As Integer
        Dim diff As Single
        Dim v As Double
        Dim tr1 As Double, tr2 As Double, t1 As Double, t2 As Double, c1 As Double, c2 As Double
        Dim t1a, t2a As Double
        Dim r As Double

        Dim s1 As Double, s2 As Double, ss As Double
        Dim first As Boolean
        Dim ymin As Integer, smin As Double, smax As Double
        Dim v1 As Single, ip As Integer
        Dim vtm As Double, pvv As Double, ii As Integer
        Dim s4 As Double, a4 As Double
        Dim sca As Double
        Dim wet As Double
        Dim wlp As Double = Me.store.LAlowPerc
        Dim maxContArea As Double = 500000
        If isGridsOpen Then
            maxContArea = contrAreaGrd.Maximum
            pxDim = contrAreaGrd.Header.dX
        End If

        'set common line characteristics

        tr1 = store.regManager.calRegs(1, id)
        tr2 = store.regManager.calRegs(2, id)
        't1 = Me.PHIlb
        't2 = Me.PHIub
        c1 = store.regManager.calRegs(3, id)
        c2 = store.regManager.calRegs(4, id)
        t1a = store.regManager.calRegs(5, id)
        t2a = store.regManager.calRegs(6, id)
        t1 = Math.Tan(t1a * pi / 180)
        t2 = Math.Tan(t2a * pi / 180)
        r = store.wd / store.regManager.calRegs(7, id)

        'Lower Unsaturated line **********************
        Dim lowUnsLine As New PointPairList()
        For p = 1 To 40
            k = (p / 40) * 2.5
            sca = (Math.Sin(Math.Atan(k)) * tr1)
            If sca >= pxDim Then lowUnsLine.Add(Math.Atan(k) * 180 / pi, sca)
        Next p

        calLn = myPane.AddCurve("Lower Unsaturated line", lowUnsLine, Drawing.Color.Black)
        calLn.Line.Width = 1
        calLn.Symbol.Type = SymbolType.None
        calLn.Label.IsVisible = False
        AddLabelToChart("Unsaturated", (Math.Atan(k) * 180 / pi) - 5, 0.6 * sca)

        '*********************************************
        'Upper Saturated line
        Dim UpSatLine As New PointPairList()
        For p = 1 To 40 '12-1
            k = (p / 40) * 2.5
            sca = (Math.Sin(Math.Atan(k)) * tr2)
            If sca >= pxDim Then UpSatLine.Add(Math.Atan(k) * 180 / pi, (Math.Sin(Math.Atan(k)) * tr2))
        Next p
        calLn = myPane.AddCurve("Upper Saturated line", UpSatLine, Drawing.Color.Black)
        calLn.Line.Width = 1
        calLn.Symbol.Type = SymbolType.None
        calLn.Label.IsVisible = False
        AddLabelToChart("Saturated", (Math.Atan(k) * 180 / pi) - 5, 0.6 * sca)

        '*********************************************
        '10 wettness line
        Dim wettnessLine As New PointPairList()
        For p = 1 To 40 '12-1
            k = (p / 40) * 2.5
            sca = (wlp / 100) * (Math.Sin(Math.Atan(k)) * tr2)
            If sca >= pxDim Then wettnessLine.Add(Math.Atan(k) * 180 / pi, sca)
        Next p

        calLn = myPane.AddCurve(wlp & " % wettness line", wettnessLine, Drawing.Color.Black)
        calLn.Line.Width = 1
        calLn.Symbol.Type = SymbolType.None
        calLn.Label.IsVisible = False

        AddLabelToChart("Wetness = " & wlp & " %", (Math.Atan(k) * 180 / pi) - 5, 0.6 * sca)

        '***************************************************
        'FS = 1.5
        Dim si15 As New PointPairList()
        'Vertical line FS=1.5
        s1 = csw(t1, c1, r, 1, 1.5)   'for sat
        s2 = csw(t1, c1, r, 0, 1.5)  'for dry
        si15.Add(s1 * 180 / pi, maxContArea)
        si15.Add(s1 * 180 / pi, Math.Sin(s1) * tr1)
        AddLabelToChart("FS=1.5", s1 * 180 / pi - 2, 1.1 * maxContArea)

        'Curve for FS=1.5
        p = 2

        For p = 2 To 68
            ss = s1 + ((s2 - s1) * ((p - 2) / 68))
            sca = af(ss, c1, t1, tr1, r, 1.5)
            If sca <= pxDim Then Exit For
            si15.Add(ss * 180 / pi, sca)
        Next p

        calLn = myPane.AddCurve("FS=1.5", si15, Drawing.Color.Black)
        calLn.Line.Width = 1
        calLn.Symbol.Type = SymbolType.None
        calLn.Label.IsVisible = False

        '*********************************************
        'FS = 1.25
        Dim si125 As New PointPairList()
        'Vertical line FS=1.25
        s1 = csw(t1, c1, r, 1, 1.25)   'for sat
        s2 = csw(t1, c1, r, 0, 1.25)  'for dry
        si125.Add(s1 * 180 / pi, maxContArea)
        si125.Add(s1 * 180 / pi, Math.Sin(s1) * tr1)
        AddLabelToChart("1.25", s1 * 180 / pi, 1.1 * maxContArea)

        'Curve for FS=1.25

        For p = 2 To 68
            ss = s1 + ((s2 - s1) * ((p - 2) / 68))
            sca = af(ss, c1, t1, tr1, r, 1.25)
            If sca <= pxDim Then Exit For
            si125.Add(ss * 180 / pi, sca)
        Next p

        calLn = myPane.AddCurve("FS=1.25", si125, Drawing.Color.Black)
        calLn.Line.Width = 1
        calLn.Symbol.Type = SymbolType.None
        calLn.Label.IsVisible = False
        '*********************************************

        'FS = 1.0
        Dim si10 As New PointPairList()
        'Vertical line FS=1.0
        s1 = csw(t1, c1, r, 1, 1.0)   'for sat
        s2 = csw(t1, c1, r, 0, 1.0)  'for dry
        si10.Add(s1 * 180 / pi, maxContArea)
        si10.Add(s1 * 180 / pi, Math.Sin(s1) * tr1)
        AddLabelToChart("1.0", s1 * 180 / pi, 1.1 * maxContArea)

        'Curve for FS=1.0

        For p = 2 To 68
            ss = s1 + ((s2 - s1) * ((p - 2) / 68))
            sca = af(ss, c1, t1, tr1, r, 1.0)
            If sca <= pxDim Then Exit For
            si10.Add(ss * 180 / pi, sca)
        Next p

        calLn = myPane.AddCurve("FS=1.0", si10, Drawing.Color.Black)
        calLn.Symbol.Type = SymbolType.None
        calLn.Line.Width = 1
        calLn.Label.IsVisible = False

        '*********************************************
        'FS = 0.0
        Dim si00 As New PointPairList()
        '*********************************************

        '      'Vertical line FS=0.00

        ymin = pxDim ' this is lower limit of area plotting - to be made more general
        'Curve for FS=0.0

        s4 = csw(t2, c2, r, 1, 1.0)   'for sat
        a4 = Math.Sin(s4) * tr2
        si00.Add(s4 * 180 / pi, maxContArea)
        si00.Add(s4 * 180 / pi, a4)
        AddLabelToChart("0.0", s4 * 180 / pi, 1.1 * maxContArea)

        smin = s4

        s1 = csw(t2, c2, r, 0, 1.0)
        smax = s1
        v1 = a4 - ymin
        For ip = 1 To 100
            ss = (smax + smin) * 0.5
            v = af(ss, c2, t2, tr2, r, 1.0) - ymin
            If (v1 * v > 0) Then
                smin = ss
                v1 = v
            Else
                smax = ss
            End If
            If (smax - smin < 0.0001) Then Exit For

        Next ip
        s1 = smax
        smin = csw(t1, c1, r, 1, 1)

        '***************************************************
        ' 0.5 line stuff here

        Dim si05 As New PointPairList()

        'Dim StabilityModel As New StabilityModel.SIndex

        first = True
        ' i don't know why 68 is to be used to get the curve. Else it screws up everything
        For p = 2 To 68
            ss = s1 - ((s1 - s4) * ((66 - (p - 2)) / 66))
            a = af(ss, c2, t2, tr2, r, 1)
            If a < ymin Then a = ymin
            If a > pxDim / 2 Then si00.Add(ss * 180 / pi, a) ' for 0.0 line
            pvv = 0.5
            smax = ss
            vtm = Me.sindexcell(Math.Tan(smin), a, c1, c2, t1a * pi / 180, t2a * pi / 180, 1 / tr2, 1 / tr1, r, wet)
            'vtm = StabilityModel.sindexcell(Math.Tan(smin), a, c1, c2, t1a, t2a, 1 / tr2, 1 / tr1, r, wet)

            v1 = vtm - pvv
            For ii = 1 To 100
                ss = (smax + smin) * 0.5
                vtm = Me.sindexcell(Math.Tan(ss), a, c1, c2, t1a * pi / 180, t2a * pi / 180, 1 / tr2, 1 / tr1, r, wet)
                'vtm = StabilityModel.sindexcell(Math.Tan(ss), a, c1, c2, t1a, t2a, 1 / tr2, 1 / tr1, r, wet)
                v = vtm - pvv
                If (v1 * v > 0) Then
                    smin = ss
                    v1 = v
                Else
                    smax = ss
                End If
                diff = Math.Abs(smax - smin)
                If (diff < 0.0001) Then Exit For
            Next ii

            If (first) Then
                si05.Add((smin + smax) * 0.5 * 180 / pi, maxContArea)
                si05.Add((smin + smax) * 0.5 * 180 / pi, a)
                AddLabelToChart("0.5", (smin + smax) * 0.5 * 180 / pi, 1.1 * maxContArea)
                first = False
            End If
            If a > pxDim / 2 Then si05.Add((smin + smax) * 0.5 * 180 / pi, a)


        Next p

        calLn = myPane.AddCurve("FS=0.5", si05, Drawing.Color.Black)
        calLn.Symbol.Type = SymbolType.None
        calLn.Line.Width = 1
        calLn.Label.IsVisible = False

        calLn = myPane.AddCurve("FS=0.0", si00, Drawing.Color.Black)
        calLn.Symbol.Type = SymbolType.None
        calLn.Line.Width = 1
        calLn.Label.IsVisible = False


    End Sub
    Private Sub AddLabelToChart(ByVal label As String, ByVal xpos As Double, ByVal ypos As Double)
        ' Add a priority stamp
        Dim text As New TextObj(label, xpos, ypos)
        text.Location.CoordinateFrame = CoordType.AxisXYScale
        text.FontSpec.Border.IsVisible = False
        text.FontSpec.FontColor = Color.Black
        text.FontSpec.Size = 10
        text.Location.AlignH = AlignH.Center
        text.Location.AlignV = AlignV.Bottom
        myPane.GraphObjList.Add(text)
    End Sub



    'Function to return angle of slope with given conditions and safety factor
    Private Function csw(ByVal t As Double, ByVal c As Double, ByVal r As Double, _
    ByVal w As Double, ByVal fs As Double) As Double
        Dim rwt As Double
        rwt = (1 - r * w) * t
        csw = 0
        If (c < 1) Then
            csw = (Math.Sqrt(fs * fs * (fs * fs + rwt * rwt - c * c)) - c * rwt) / (fs * fs + rwt * rwt)
        End If
        csw = Math.Acos(csw)
    End Function

    'Function to return specific catchment area that results in the given factor of safety for the given parameters
    Private Function af(ByVal angle As Double, ByVal c As Double, ByVal t As Double, ByVal x As Double, ByVal r As Double, ByVal fs As Double) As Double
        'in this case x = T/R (m)
        Dim ss As Double, cs As Double
        ss = Math.Sin(angle)
        cs = Math.Cos(angle)
        af = ss * (1 - (fs * ss - c) / (t * cs)) * x / (r)
    End Function

    'function to compute stability index with potentially 
    'uncertain parameters  for a specific grid cell
    Private Function sindexcell(ByVal s As Double, ByVal a As Double, ByVal c1 As Double, _
                                ByVal c2 As Double, ByVal t1 As Double, ByVal t2 As Double, _
                                ByVal x1 As Double, ByVal x2 As Double, ByVal r As Double, _
                                ByRef sat As Double) As Double
        ' c1, c2  bounds on dimensionless cohesion
        ' t1,t2 bounds on friction angle
        ' x1,x2 bounds on wetness parameter q/T
        ' r Density ratio
        ' s slope angle
        ' a specific catchment area */

        Dim cs, ss, w1, w2, fs2, fs1, cdf1, cdf2, y1, y2, _as, si As Double

        ' cng - added this to prevent division by zero
        If (s = 0.0) Then
            sat = 3.0
            Return 10
        End If
        ' DGT - The slope in the GIS grid file is the tangent of the angle  */
        s = Math.Atan(s)
        ' t1 and t2 input as angle must be converted to tan  */
        t1 = Math.Tan(t1)
        t2 = Math.Tan(t2)

        cs = Math.Cos(s)
        ss = Math.Sin(s)

        If (x1 > x2) Then
            'Error these are wrong way round - switch them  */
            w1 = x2   'Using w1 as a temp variable  */
            x2 = x1
            x1 = w1
        End If
        'Wetness is coded between 0 and 1.0 based on the lower T/q parameter 
        '(this is conservative).  If wetness based on lower T/q is > 1.0 and wetness based 
        'on upper T/q <1.0, then call it the "threshold saturation zone" and hard code it 
        'to 2.0.  Then if wetness based on upper T/q >1.0, then call it the 
        '"saturation zone" and hard code it to 3.0.  
        'Lower T/q correspounds to upper q/T = x = x2 ->  w2

        w2 = x2 * a / ss
        w1 = x1 * a / ss
        sat = w2
        If (w2 > 1.0) Then
            w2 = 1.0
            sat = 2.0
        End If
        If (w1 > 1.0) Then
            w1 = 1.0
            sat = 3.0
        End If


        fs2 = fs(s, w1, c2, t2, r)

        If (fs2 < 1) Then  'always unstable
            si = 0
        Else
            fs1 = fs(s, w2, c1, t1, r)

            If (fs1 >= 1) Then 'Always stable
                si = fs1
            Else
                If (w1 = 1) Then 'region 1
                    si = 1 - f2s(c1 / ss, c2 / ss, cs * (1 - r) / ss * t1, cs * (1 - r) / ss * t2, 1)
                Else
                    If (w2 = 1) Then 'region 2
                        _as = a / ss
                        y1 = cs / ss * (1 - r)
                        y2 = cs / ss * (1 - x1 * _as * r)
                        cdf1 = (x2 * _as - 1) / (x2 * _as - x1 * _as) * f2s(c1 / ss, c2 / ss, cs * (1 - r) / ss * t1, cs * (1 - r) / ss * t2, 1)
                        cdf2 = (1 - x1 * _as) / (x2 * _as - x1 * _as) * f3s(c1 / ss, c2 / ss, y1, y2, t1, t2, 1)
                        si = 1 - cdf1 - cdf2

                    Else  'region 3
                        _as = a / ss
                        y1 = cs / ss * (1 - x2 * _as * r)
                        y2 = cs / ss * (1 - x1 * _as * r)
                        si = 1 - f3s(c1 / ss, c2 / ss, y1, y2, t1, t2, 1)
                    End If
                End If
            End If
        End If

        'cng - need to limit the size spread for arcview since it has
        'difficulties with equal intervals over a large range of numbers
        If (si > 10.0) Then
            si = 10.0
        End If

        Return si
    End Function

    'Generic function to implement dimensionless form of infinite slope
    'stability model

    Private Function fs(ByVal s As Double, ByVal w As Double, ByVal c As Double, ByVal t As Double, ByVal r As Double) As Double
        Dim cs, ss As Double
        cs = Math.Cos(s)
        ss = Math.Sin(s)
        Return ((c + cs * (1 - w * r) * t) / ss)
    End Function

    Private Function f3s(ByVal x1 As Double, ByVal x2 As Double, ByVal y1 As Double, _
    ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal z As Double) As Double

        Dim p As Double

        If (x2 < x1) Or (y2 < y1) Or (b2 < b1) Then
            'cat("Bad input parameters, upper limits less than lower limits\n")
            'p = NA
            'cng - make a big number to be subtracted and yield ndv */
            'p = 1000.0
            p = 1000.0
        ElseIf (x1 < 0) Or (y1 < 0) Or (b1 < 0) Then
            'cat("Bad input parameters, Variables cannot be negative\n")
            'p = NA
            p = 1000.0
        Else

            If (y1 = y2) Or (b1 = b2) Then
                'degenerate on y or b - reverts to additive convolution
                p = f2s(x1, x2, y1 * b1, y2 * b2, z)
            Else
                If (x2 = x1) Then
                    p = fa(y1, y2, b1, b2, z - x1)
                Else
                    p = (fai(y1, y2, b1, b2, z - x1) - fai(y1, y2, b1, b2, z - x2)) / (x2 - x1)
                End If
            End If
        End If
        Return p
    End Function

    Private Function f2s(ByVal x1 As Double, ByVal x2 As Double, ByVal y1 As Double, ByVal y2 As Double, ByVal z As Double) As Double
        Dim p, mn, mx, d, d1, d2 As Double

        'p = rep(0, length(z))
        p = 0.0

        mn = Math.Min(x1 + y2, x2 + y1)
        mx = Math.Max(x1 + y2, x2 + y1)
        d1 = Math.Min(x2 - x1, y2 - y1)
        d = z - y1 - x1
        d2 = Math.Max(x2 - x1, y2 - y1)
        If (z > (x1 + y1)) And (z < mn) Then p = (d * d) / (2 * (x2 - x1) * (y2 - y1)) Else p = p
        If (mn <= z) And (z <= mx) Then p = (d - d1 / 2) / d2 Else p = p
        If (mx < z) And (z < (x2 + y2)) Then p = 1 - Math.Pow((z - y2 - x2), 2) / (2 * (x2 - x1) * (y2 - y1)) Else p = p
        If z >= (x2 + y2) Then p = 1 Else p = p

        Return p

    End Function

    Private Function fa(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double

        Dim p, adiv, t As Double

        'p = rep(0, length(a));
        p = 0.0

        If (y1 * b2) > (y2 * b1) Then
            'Invoke symmetry and switch so that y1*b2 < y2*b1
            t = y1
            y1 = b1
            b1 = t
            'was   y <- y2  which is wrong
            t = y2
            y2 = b2
            b2 = t
        End If

        adiv = (y2 - y1) * (b2 - b1)
        If (y1 * b1 < a) And (a <= y1 * b2) Then p = (a * Math.Log(a / (y1 * b1)) - a + y1 * b1) / adiv Else p = p
        If (y1 * b2 < a) And (a <= y2 * b1) Then p = (a * Math.Log(b2 / b1) - (b2 - b1) * y1) / adiv Else p = p
        If (y2 * b1 < a) And (a < y2 * b2) Then p = (a * Math.Log((b2 * y2) / a) + a + b1 * y1 - b1 * y2 - b2 * y1) / adiv Else p = p
        If (a >= (b2 * y2)) Then p = 1 Else p = p

        Return p
    End Function

    Private Function fai(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double

        Dim p, t, a1, a2, a3, a4, c2, c3, c4, c5 As Double

        'p = rep(0,length(a));
        p = 0.0

        If (y1 * b2) > (y2 * b1) Then
            'Invoke symmetry and switch so that y1*b2 < y2*b1
            t = y1
            y1 = b1
            b1 = t
            t = y2
            y2 = b2
            b2 = t
        End If
        'define limits
        a1 = y1 * b1
        a2 = y1 * b2
        a3 = y2 * b1
        a4 = y2 * b2

        'Integration constants
        c2 = -fai2(y1, y2, b1, b2, a1)

        'Need to account for possibility of 0 lower bounds
        If (a2 = 0) Then
            c3 = 0
        Else
            c3 = fai2(y1, y2, b1, b2, a2) + c2 - fai3(y1, y2, b1, b2, a2)
        End If

        If (a3 = 0) Then
            c4 = 0
        Else
            c4 = fai3(y1, y2, b1, b2, a3) + c3 - fai4(y1, y2, b1, b2, a3)
        End If
        c5 = fai4(y1, y2, b1, b2, a4) + c4 - fai5(y1, y2, b1, b2, a4)

        'evaluate
        If (a1 < a) And (a <= a2) Then p = fai2(y1, y2, b1, b2, a) + c2 Else p = p
        If (a2 < a) And (a <= a3) Then p = fai3(y1, y2, b1, b2, a) + c3 Else p = p
        If (a3 < a) And (a < a4) Then p = fai4(y1, y2, b1, b2, a) + c4 Else p = p
        If (a >= a4) Then p = fai5(y1, y2, b1, b2, a) + c5 Else p = p

        Return p
    End Function

    Private Function fai2(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double
        Dim adiv As Double

        adiv = (y2 - y1) * (b2 - b1)
        Return (a * a * Math.Log(a / (y1 * b1)) / 2 - 3 * a * a / 4 + y1 * b1 * a) / adiv
    End Function

    Private Function fai3(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double
        Dim adiv As Double

        adiv = (y2 - y1) * (b2 - b1)
        Return (a * a * Math.Log(b2 / b1) / 2 - (b2 - b1) * y1 * a) / adiv
    End Function

    Private Function fai4(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double
        Dim adiv As Double

        adiv = (y2 - y1) * (b2 - b1)
        Return (a * a * Math.Log(b2 * y2 / a) / 2 + 3 * a * a / 4 + b1 * y1 * a - b1 * y2 * a - b2 * y1 * a) / adiv
    End Function

    Private Function fai5(ByVal y1 As Double, ByVal y2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal a As Double) As Double
        Return a
    End Function

#End Region

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        UpdateChart()
    End Sub
    Private Sub UpdateChart()
        Dim regId As Integer

        selReg = Me.ListBox1.SelectedIndex
        If selReg < 0 Then Exit Sub

        regId = store.regManager.GetRealRegionId(selReg)
        'remove all curves
        Do Until myPane.CurveList.Count = 0
            myPane.CurveList.RemoveAt(myPane.CurveList.Count - 1)
        Loop

        'remove all tests
        Do Until myPane.GraphObjList.Count = 0
            myPane.GraphObjList.RemoveAt(myPane.GraphObjList.Count - 1)
        Loop

        myPane.Title.Text = title & " Region " & store.regManager.calRegs(0, selReg)
        myPane.YAxis.Scale.Min = slopeGrd.Header.dX
        Me.LoadRandomPoint(selReg)
        Me.LoadLandSlidePoint(selReg)
        pxDim = slopeGrd.Header.dX
        Me.AddCalibrationLines(selReg)

        Me.ZedGraphControl1.RestoreScale(Me.ZedGraphControl1.GraphPane)
    End Sub

    Private Sub ExportAsImageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportAsImageToolStripMenuItem.Click
        Me.ZedGraphControl1.SaveAs()
    End Sub




    Private Sub AdjustCalibrationParametersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdjustCalibrationParametersToolStripMenuItem.Click
        Dim formParAdj As New frmCalParAdjust

        Dim rr As Integer = Me.ListBox1.SelectedIndex
        If rr < 0 Then Exit Sub
        rr = Me.regList(rr)
        'MsgBox(rr)
        formParAdj.UpLoadParameter(Me.store.regManager, rr)
        formParAdj.ShowDialog()
        UpdateChart()
    End Sub

    

    Private Sub SomeStat(ByRef calReg(,) As Double, ByVal noData As Double, _
                         ByRef si(,) As Double, ByVal noData2 As Double, _
                         ByVal nCols As Double, ByVal nRows As Double, _
                         ByRef stat(,) As Double, ByVal numOfReg As Integer)
        Dim c, r, i As Integer
        For c = 0 To nCols
            For r = 0 To nRows
                If (calReg(c, r) <> noData) Then
                    For i = 0 To numOfReg - 1
                        If (stat(i, 0) = calReg(c, r)) Then
                            If si(c, r) <> noData2 Then
                                If (si(c, r) > 1.5) Then stat(i, 1) = stat(i, 1) + 1
                                If (si(c, r) > 1.25 And si(c, r) <= 1.5) Then stat(i, 2) = stat(i, 2) + 1
                                If (si(c, r) > 1.0 And si(c, r) <= 1.25) Then stat(i, 3) = stat(i, 3) + 1
                                If (si(c, r) > 0.5 And si(c, r) <= 1.0) Then stat(i, 4) = stat(i, 4) + 1
                                If (si(c, r) > 0.0 And si(c, r) <= 0.5) Then stat(i, 5) = stat(i, 5) + 1
                                If (si(c, r) <= 0.0) Then stat(i, 6) = stat(i, 6) + 1

                            End If
                            Exit For 'go out, calReg id is unique
                        End If
                    Next
                End If
            Next
        Next

    End Sub
    Private Function ZedGraphControl1_MouseDownEvent(ByVal zgControl As ZedGraphControl, _
                                                     ByVal e As MouseEventArgs) _
                                                     As Boolean Handles ZedGraphControl1.MouseDownEvent
        If ToolStripButton2.Checked = True Then
            Dim myPane As GraphPane = zgControl.GraphPane
            Dim mousePt As New PointF(e.X, e.Y)
            Dim lsIndex As Integer
            Dim lsId As Integer
            Dim flg As Boolean = myPane.FindNearestPoint(mousePt, lsPt, lsIndex)
            If flg Then
                lsId = lsPt.Points(lsIndex).Z - 1 '-1 is necessary in order to prevent 0 value
                If lsId > -1 Then
                    Dim x, y As Double
                    Dim lsSf As New MapWinGIS.Shapefile
                    lsSf.Open(store.slpFileName)
                    x = lsSf.Shape(lsId).Point(0).x
                    y = lsSf.Shape(lsId).Point(0).y
                    lsSf.Close()


                    Dim newExt As New MapWinGIS.Extents
                    Dim minX, minY, maxX, maxY As Double
                    Dim offSet As Double = 10 * store.zExt
                    minX = x - offSet
                    maxX = x + offSet
                    minY = y - offSet
                    maxY = y + offSet

                    newExt.SetBounds(minX, minY, 0, maxX, maxY, 0)
                    store.m_Map.View.Extents = newExt
                    'At the moment I cannot select the landslide, only zoom to the extend
                    MessageBox.Show("Selected Landslide ID: " & lsId)

                End If
            End If
            Return True
        End If
        Return False

    End Function


    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        Me.ZedGraphControl1.SaveAs()
    End Sub

    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        If ToolStripButton2.Checked = True Then
            ToolStripButton2.Checked = False
        Else
            ToolStripButton2.Checked = True
        End If
    End Sub

    Private Sub ToolStripButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton3.Click
        Dim formParAdj As New frmCalParAdjust

        Dim rr As Integer = Me.ListBox1.SelectedIndex
        If rr < 0 Then Exit Sub
        rr = Me.regList(rr)
        'MsgBox(rr)
        formParAdj.UpLoadParameter(Me.store.regManager, rr)
        formParAdj.ShowDialog()
        UpdateChart()
    End Sub
End Class