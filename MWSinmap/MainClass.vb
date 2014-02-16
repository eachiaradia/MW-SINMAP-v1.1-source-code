Imports MapWindow.Interfaces
Imports System.Windows.Forms
Imports System.IO

Public Class MainClass
    Implements MapWindow.Interfaces.IPlugin

#Region "MapWindow Variables"
    Public m_Map As MapWindow.Interfaces.IMapWin
    Public m_MenuItems As ArrayList
    Public m_MenusExist As Boolean
    Private sep$ = ";"
    Public regManager As CalRegManager
   'Private resMan As New System.Resources.ResourceManager("MyResource-IT.resx", Assembly.GetExecutingAssembly())
#End Region

#Region "MWSinmap constants and settings"
    'default values
    Public gc As Double = 9.81
    Public wd As Double = 1000
    Public numSAplot As Integer = 2000
    Public LAlowPerc As Double = 10
    Public Check4Contam As Boolean = True
    Public UseExistGrd As Boolean = True
    Public RecalculateGrd As Boolean = False
    Public zExt As Double = 20

    'calibration parameters input
    Public TRlb As Double = 2000
    Public TRub As Double = 3000
    Public Clb As Double = 0
    Public Cub As Double = 0.25
    Public PHIlb As Double = 30
    Public PHIub As Double = 45
    Public soild As Double = 2000

    'in/out file name
    Public demFileName As String = ""
    Public calFileName As String = ""
    Public slpFileName As String = ""
    Public calGrdFileName As String = "cal"
    Public fillDemFileName As String = "fel"
    Public flowDirFileName As String = "ang"
    Public slopeFileName As String = "slp"
    Public contrAreaFileName As String = "sca"
    Public SiFileName As String = "si"
    Public SatFileName As String = "sat"

#End Region

    Public ReadOnly Property Author() As String Implements MapWindow.Interfaces.IPlugin.Author
        Get
            Return "Enrico A. Chiaradia - Department of Agricultural Engineering - University of Milan"
        End Get
    End Property

    Public ReadOnly Property SerialNumber() As String Implements MapWindow.Interfaces.IPlugin.SerialNumber
        Get
            Return "" ' 3/22/2006 No longer needed.
        End Get
    End Property

    Public ReadOnly Property BuildDate() As String Implements MapWindow.Interfaces.IPlugin.BuildDate
        Get
            Return System.IO.File.GetLastWriteTime(Me.GetType.Assembly.Location).ToString
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements MapWindow.Interfaces.IPlugin.Description
        Get
            Return "Stability Index Mapping for MapwindowGIS"
        End Get
    End Property

    Public ReadOnly Property DllPath() As String
        Get
            Dim pluginPath As String = Me.GetType.Assembly.Location

            Dim iSlash As Integer = pluginpath.LastIndexOf("\")
            pluginpath = pluginpath.Substring(0, (iSlash + 1))
            iSlash = pluginpath.LastIndexOf("\")
            pluginpath = pluginpath.Substring(0, (iSlash + 1))
            Return pluginPath
        End Get
    End Property

    Public Sub Initialize(ByVal MapWin As MapWindow.Interfaces.IMapWin, ByVal ParentHandle As Integer) Implements MapWindow.Interfaces.IPlugin.Initialize
        'this function initializes values, creates buttons for the PhotoViewer plugin
        'Dim xtoolsmenu As MapWindow.Interfaces.Menus
        m_Map = MapWin

        Call LoadMenus()
    End Sub

    Public Sub ItemClicked(ByVal ItemName As String, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.ItemClicked
        'this function shows the viewer or the DBConnection, depending on settings when the PhotoViewer toolbar button is cliked
        HandleMenuEvent(ItemName, Handled)
    End Sub

    Public Sub LayerRemoved(ByVal Handle As Integer) Implements MapWindow.Interfaces.IPlugin.LayerRemoved
        'this function handles the problem when the shapefile associated with the Photoviewer is removed
    End Sub

    Public Sub LayersAdded(ByVal Layers() As MapWindow.Interfaces.Layer) Implements MapWindow.Interfaces.IPlugin.LayersAdded

    End Sub

    Public Sub LayersCleared() Implements MapWindow.Interfaces.IPlugin.LayersCleared

    End Sub

    Public Sub LayerSelected(ByVal Handle As Integer) Implements MapWindow.Interfaces.IPlugin.LayerSelected

    End Sub

    Public Sub LegendDoubleClick(ByVal Handle As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendDoubleClick

    End Sub

    Public Sub LegendMouseDown(ByVal Handle As Integer, ByVal Button As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendMouseDown

    End Sub

    Public Sub LegendMouseUp(ByVal Handle As Integer, ByVal Button As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendMouseUp

    End Sub

    Public Sub MapDragFinished(ByVal Bounds As System.Drawing.Rectangle, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapDragFinished

    End Sub

    Public Sub MapExtentsChanged() Implements MapWindow.Interfaces.IPlugin.MapExtentsChanged

    End Sub

    Public Sub MapMouseDown(ByVal Button As Integer, ByVal Shift As Integer, ByVal x As Integer, ByVal y As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseDown

    End Sub

    Public Sub MapMouseUp(ByVal Button As Integer, ByVal Shift As Integer, ByVal x As Integer, ByVal y As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseUp

    End Sub

    Public Sub MapMouseMove(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseMove

    End Sub

    Public Sub Message(ByVal msg As String, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.Message

    End Sub

    Public ReadOnly Property Name() As String Implements MapWindow.Interfaces.IPlugin.Name
        Get
            Return "MW-SINMAP"
        End Get
    End Property

    Public Sub ProjectLoading(ByVal ProjectFile As String, ByVal SettingsString As String) Implements MapWindow.Interfaces.IPlugin.ProjectLoading
        Dim settings() As String = SettingsString.Split(sep)
        'If settings.Length > 1 Then 'this prevents errors when loading an empty/new project
        Try 'eac 2009/05/22
            'default values
            gc = CType(settings(0), Double)
            wd = CType(settings(1), Double)
            numSAplot = CType(settings(2), Integer)
            LAlowPerc = CType(settings(3), Double)
            Check4Contam = CType(settings(4), Boolean)
            UseExistGrd = CType(settings(5), Boolean)
            RecalculateGrd = CType(settings(6), Boolean)

            'calibration parameters input
            TRlb = CType(settings(7), Double)
            TRub = CType(settings(8), Double)
            Clb = CType(settings(9), Double)
            Cub = CType(settings(10), Double)
            PHIlb = CType(settings(11), Double)
            PHIub = CType(settings(12), Double)
            soild = CType(settings(13), Double)

            'in/out file name
            demFileName = settings(14)
            calFileName = settings(15)
            slpFileName = settings(16)
            calGrdFileName = settings(17)
            fillDemFileName = settings(18)
            flowDirFileName = settings(19)
            slopeFileName = settings(20)
            contrAreaFileName = settings(21)
            SiFileName = settings(22)
            SatFileName = settings(23)

            'initialize calibration region manager
            regManager = New CalRegManager(Me)
            regManager.OpenCalibrationFiles()
        Catch
            MessageBox.Show("Error loading SINMAP project", "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        End Try

    End Sub

    Public Sub ProjectSaving(ByVal ProjectFile As String, ByRef SettingsString As String) Implements MapWindow.Interfaces.IPlugin.ProjectSaving
        SettingsString = ""
        SettingsString = Me.gc & sep & Me.wd & sep & Me.numSAplot & sep & Me.LAlowPerc
        SettingsString = SettingsString & sep & Me.Check4Contam.ToString & sep & Me.UseExistGrd.ToString & sep & Me.RecalculateGrd.ToString
        SettingsString = SettingsString & sep & Me.TRlb & sep & Me.TRub & sep & Me.Clb & sep & Me.Cub & sep & Me.PHIlb & sep & Me.PHIub & sep & Me.soild
        SettingsString = SettingsString & sep & Me.demFileName & sep & Me.calFileName & sep & Me.slpFileName & sep & Me.calGrdFileName & sep & Me.fillDemFileName & sep & Me.flowDirFileName & sep & Me.slopeFileName & sep & Me.contrAreaFileName & sep & Me.SiFileName & sep & Me.SatFileName
    End Sub


    Public Sub Terminate() Implements MapWindow.Interfaces.IPlugin.Terminate
        'this function closes viewers, releases resources, removes buttons from the toolbar,etc before the plugin is unloaded
        'Dim xtoolsmenu As MapWindow.Interfaces.Menus
        DestroyMenus()
        'Mapwin_g.Menus.Remove("xTools")
        'Mapwin_g.Menus.Remove("shp2grd")
        'Mapwin_g.Menus.Remove("Intersect2shp")

    End Sub

    Public ReadOnly Property Version() As String Implements MapWindow.Interfaces.IPlugin.Version
        Get
            Return System.Diagnostics.FileVersionInfo.GetVersionInfo(Me.GetType.Assembly.Location).FileVersion.ToString
        End Get
    End Property


    Public Sub ShapesSelected(ByVal Handle As Integer, ByVal SelectInfo As MapWindow.Interfaces.SelectInfo) Implements MapWindow.Interfaces.IPlugin.ShapesSelected

    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' TITLE: LoadMenus
    ' AUTHOR: Chris Michaelis
    ' DESCRIPTION: Method to load GIS Tool Menu items
    '
    ' INPUTS:   None
    '
    ' OUTPUTS: None
    '
    ' NOTES: None
    '
    ' Change Log: 
    ' Date          Changed By      Notes
    ' 11/03/2005    ARA             Added Header and added ClipMerge submenu
    ' 11/21/2005    Chris Michaelis Added Georeferencing Stuff
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub LoadMenus()
        If m_MenusExist Then Return
        If m_Map Is Nothing Then Return

        If (m_MenuItems Is Nothing) Then
            m_MenuItems = New ArrayList
        Else
            m_MenuItems.Clear()
        End If

        m_MenusExist = True

        ' Here, use a very unique phrase to avoid duplication: mwTools_MapWindowTools
        ' Each subitem will begin with "mwTools_" by convention
        Dim rootItem As MapWindow.Interfaces.MenuItem = m_Map.Menus.Item("MW-SINMAP")
        If (rootItem Is Nothing) Then rootItem = m_Map.Menus.AddMenu("MW-SINMAP", "", Nothing, "MW-SINMAP")
        m_MenuItems.Add(rootItem)

        Dim InitRootItem As MapWindow.Interfaces.MenuItem = m_Map.Menus.Item("Initialization")
        If (InitRootItem Is Nothing) Then InitRootItem = m_Map.Menus.AddMenu("Initialization", rootItem.Name, Nothing, "Initialization")
        m_MenuItems.Add(InitRootItem)

        Dim GridProcRootItem As MapWindow.Interfaces.MenuItem = m_Map.Menus.Item("Grid_processing")
        If (GridProcRootItem Is Nothing) Then GridProcRootItem = m_Map.Menus.AddMenu("Grid_processing", rootItem.Name, Nothing, "Grid processing")
        m_MenuItems.Add(GridProcRootItem)

        Dim SARootItem As MapWindow.Interfaces.MenuItem = m_Map.Menus.Item("Stability_analysis")
        If (SARootItem Is Nothing) Then SARootItem = m_Map.Menus.AddMenu("Stability_analysis", rootItem.Name, Nothing, "Stability Analysis")
        m_MenuItems.Add(GridProcRootItem)

        'Initialization
        m_MenuItems.Add(m_Map.Menus.AddMenu("SetDef", InitRootItem.Name, Nothing, "Set Defaults"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SetDefCalPar", InitRootItem.Name, Nothing, "Set Default Calibration Parameters"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line1", InitRootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SelDemGrd", InitRootItem.Name, Nothing, "Select Dem Grid For Analysis"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line2", InitRootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SingCal", InitRootItem.Name, Nothing, "Make Single Calibration Region Theme"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("MultiCal", InitRootItem.Name, Nothing, "Create Multi Calibration Region Theme"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("UseExCal", InitRootItem.Name, Nothing, "Use Existing Calibration Region Theme"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line3", InitRootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SelLS", InitRootItem.Name, Nothing, "Select Landslide Points Theme"))

        'Grid processing
        m_MenuItems.Add(m_Map.Menus.AddMenu("GrdProcAllSteps", GridProcRootItem.Name, Nothing, "Compute All Steps"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line4", GridProcRootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("PitFilDem", GridProcRootItem.Name, Nothing, "Pit Filled Dem"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("FlowDirSlp", GridProcRootItem.Name, Nothing, "Flow Direction and Slope"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SpeCatchArea", GridProcRootItem.Name, Nothing, "Specific Catchment Area"))

        'Stability analysis
        'm_MenuItems.Add(m_Map.Menus.AddMenu("SaAllSteps", SARootItem.Name, Nothing, "Compute All Steps"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SI&Sat", SARootItem.Name, Nothing, "Stability Index and Saturation"))
        'm_MenuItems.Add(m_Map.Menus.AddMenu("Sat", SARootItem.Name, Nothing, "Saturation"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line5", SARootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("SaPlots", SARootItem.Name, Nothing, "SA Plots"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line6", SARootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("CalParAdj", SARootItem.Name, Nothing, "Calibration Parameter Adjustment"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("line7", SARootItem.Name, Nothing, "-"))
        m_MenuItems.Add(m_Map.Menus.AddMenu("ExStat", SARootItem.Name, Nothing, "Export Statistics"))
        'm_MenuItems.Add(m_Map.Menus.AddMenu("dummy", SARootItem.Name, Nothing, "Dummy function!"))

    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' TITLE: DestroyMenus
    ' AUTHOR: Chris Michaelis
    ' DESCRIPTION: Method to destroy GIS Tool Menu items
    '
    ' INPUTS:   None
    '
    ' OUTPUTS: None
    '
    ' NOTES: None
    '
    ' Change Log: 
    ' Date          Changed By      Notes
    ' 11/03/2005    ARA             Added Header
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub DestroyMenus()
        If Not m_MenusExist Then Return
        If m_Map Is Nothing Then Return

        Dim i As IEnumerator = m_MenuItems.GetEnumerator()
        While (i.MoveNext())
            If CType(i.Current, MapWindow.Interfaces.MenuItem).Name = "" Then
                m_Map.Menus.Remove(CType(i.Current, MapWindow.Interfaces.MenuItem).Text)
            Else
                m_Map.Menus.Remove(CType(i.Current, MapWindow.Interfaces.MenuItem).Name)
            End If
        End While

        m_MenusExist = False
        m_MenuItems.Clear()
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' TITLE: HandleMenuEvent
    ' AUTHOR: Chris Michaelis
    ' DESCRIPTION: Method to handl GIS Tool Menu item events
    '
    ' INPUTS:   MenuText as menu item to handle
    '
    ' OUTPUTS: None
    '
    ' NOTES: None
    '
    ' Change Log: 
    ' Date          Changed By      Notes
    ' 11/03/2005    ARA             Added Header and handling for the clipmerge menu
    ' 04/05/2006    Angela Hillier  Added handling for Buffer
    ' 08/22/2006    JLK             Added use of Logger
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub HandleMenuEvent(ByVal MenuText As String, ByRef Handled As Boolean)
        If Not m_MenusExist Then Return

        Dim i As IEnumerator = m_MenuItems.GetEnumerator()
        Dim err As Integer

        While (i.MoveNext())
            If CType(i.Current, MapWindow.Interfaces.MenuItem).Name = MenuText Then
                ' Else it's not one of my menu items, as it wasn't in m_MenuItems

                Select Case MenuText
                    Case "SetDef"
                        Dim myFrm As New frmDefaultValue(Me)
                        myFrm.ShowDialog()
                    Case "SetDefCalPar"
                        Dim myFrm As New frmCalParam(Me)
                        myFrm.ShowDialog()
                    Case "SelDemGrd"
                        Dim myFrm As New frmSelectDemGrd(Me)
                        myFrm.ShowDialog()
                        SpecialAddLayer(Me.demFileName, "Base Dem Grid", 1)
                    Case "SingCal"
                        regManager = New CalRegManager(Me)
                        Dim flg As Boolean
                        Me.calFileName = Functions.addLabel(Me.demFileName, "cal", ".shp")
                        flg = Functions.CreateSingleCalShpFile(Me.demFileName, Me.calFileName, Me)
                        If flg = True Then m_Map.Layers.Add(Me.calFileName, "Single Calibration Area")
                        flg = Functions.CreateSingleCalGrid(Me.calGrdFileName, Me.demFileName)
                        If flg = True Then
                            m_Map.Layers.Add(Me.calGrdFileName, "Single Calibration Grid")
                            regManager.OpenCalibrationFiles()
                        End If
                    Case "MultiCal"

                        Dim myFrm As New frmSelectShape(Me, "Select a region map", "Region polygon shapefile:")
                        myFrm.ShowDialog()
                        Dim flg As Boolean = Functions.MakeMultiCalRegGrd(Me.calFileName, Me.calGrdFileName, Me.demFileName, Me)
                        If flg = True Then
                            m_Map.Layers.Add(Me.calFileName, "Multiple Calibration Area")
                            m_Map.Layers.Add(Me.calGrdFileName, "Multiple Calibration Grid")
                            regManager = New CalRegManager(Me)
                            regManager.OpenCalibrationFiles()
                        End If
                    Case "UseExCal"
                        regManager = New CalRegManager(Me)
                        Dim myFrm As New frmUseExCal(Me)
                        Dim res As System.Windows.Forms.DialogResult
                        res = myFrm.ShowDialog()
                        If res = Windows.Forms.DialogResult.Cancel Then Exit Select
                        m_Map.Layers.Add(Me.calFileName, "Existing Calibration Area")
                        m_Map.Layers.Add(Me.calGrdFileName, "Existing Calibration Grid")
                        regManager.OpenCalibrationFiles()
                    Case "SelLS"
                        Dim myFrm As New frmSelectShape(Me, "Select a soil slip map", "Soil slipe point shapefile:")
                        Dim res As System.Windows.Forms.DialogResult
                        res = myFrm.ShowDialog()
                        If res = Windows.Forms.DialogResult.Cancel Then Exit Select
                        m_Map.Layers.Add(Me.slpFileName, "Landslides")
                        'TODO: verify if it's a correct file
                    Case "GrdProcAllSteps"
                        '1) pit filling
                        If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.fillDemFileName) = True) Then
                            Me.SpecialAddLayer(Me.fillDemFileName, "Filled Dem Grid", 1)
                        Else
                            'Me.FillDem()
                            err = Me.FillDem()
                            If err = 0 Then Me.SpecialAddLayer(Me.fillDemFileName, "Filled Dem Grid", 1)
                        End If
                        '2) flow direction and slope
                        If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.flowDirFileName) = True) And (System.IO.File.Exists(Me.slopeFileName) = True) Then
                            Me.SpecialAddLayer(Me.flowDirFileName, "Multiple Flow Direction", 3)
                            Me.SpecialAddLayer(Me.slopeFileName, "Slope", 2)
                        Else
                            err = Me.FlowDirAndSlope()
                            If err = 0 Then
                                Me.SpecialAddLayer(Me.flowDirFileName, "Multiple Flow Direction", 3)
                                Me.SpecialAddLayer(Me.slopeFileName, "Slope", 2)
                            End If
                            End If
                            '3) contributing area
                            If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.contrAreaFileName) = True) Then
                                Me.SpecialAddLayer(Me.contrAreaFileName, "Contributing Area", 4)
                            Else
                                err = Me.DinfArea()
                                If err = 0 Then Me.SpecialAddLayer(Me.contrAreaFileName, "Contributing Area", 4)
                            End If
                    Case "PitFilDem"
                            If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.fillDemFileName) = True) Then
                                Me.SpecialAddLayer(Me.fillDemFileName, "Filled Dem Grid", 1)
                            Else
                            err = FillDem()
                            If err = 0 Then Me.SpecialAddLayer(Me.fillDemFileName, "Filled Dem Grid", 1)
                            End If
                    Case "FlowDirSlp"
                            If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.flowDirFileName) = True) And (System.IO.File.Exists(Me.slopeFileName) = True) Then
                                Me.SpecialAddLayer(Me.flowDirFileName, "Multiple Flow Direction", 3)
                                Me.SpecialAddLayer(Me.slopeFileName, "Slope", 2)
                            Else
                            err = FlowDirAndSlope()
                            If err = 0 Then
                                Me.SpecialAddLayer(Me.flowDirFileName, "Multiple Flow Direction", 3)
                                Me.SpecialAddLayer(Me.slopeFileName, "Slope", 2)
                            End If
                            End If
                    Case "SpeCatchArea"
                            If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.contrAreaFileName) = True) Then
                                Me.SpecialAddLayer(Me.contrAreaFileName, "Contributing Area", 4)
                            Else
                            err = Me.DinfArea()
                            If err = 0 Then Me.SpecialAddLayer(Me.contrAreaFileName, "Contributing Area", 4)
                            End If
                    Case "SaAllSteps"
                            'deprecated
                    Case "SI&Sat"
                            Dim grd As New MapWinGIS.Grid
                            If (Me.UseExistGrd = True) And (System.IO.File.Exists(Me.SiFileName) = True) And (System.IO.File.Exists(Me.SatFileName) = True) Then
                                Me.SpecialAddLayer(Me.SiFileName, "Stability Index", 5)
                                Me.SpecialAddLayer(Me.SatFileName, "Saturation", 6)
                            Else
                                If regManager.isLoaded Then
                                err = Me.StabIndex()
                                'err = Me.StabIndex2()
                                If err = 0 Then
                                    Me.SpecialAddLayer(Me.SiFileName, "Stability Index", 5)
                                    Me.SpecialAddLayer(Me.SatFileName, "Saturation", 6)
                                End If
                            Else
                                MsgBox("No region information are available")
                            End If
                            End If
                    Case "Sat"
                            'deprecated    
                    Case "SaPlots"
                            'lunch SA plot
                            regManager.PlotData()
                    Case "CalParAdj"
                            'lunch form to manage calibration data
                        regManager.ParamAdjust()
                    Case "ExStat"
                        'make statistics and save data
                        MakeReportStatistics()
                    Case "dummy"
                            'deprecated
                            Call dummy()

                    Case Else
                            'DO NOTHING!!!
                End Select
                Handled = True
            End If
        End While
    End Sub
    Public Function SpecialAddLayer(ByVal fileNm As String, ByVal layerNm As String, ByVal layerLeg As Integer) As Boolean
        SpecialAddLayer = False
        'if already loaded in the TOC, update it!
        'Dim i As Integer

        Dim flg As Boolean

        Try
            Dim isLoaded As Integer
            isLoaded = Functions.IsAlreadyLoaded(fileNm, Me)
            If isLoaded > -1 Then 'ok is just loaded
                If m_Map.Layers.Item(isLoaded).LayerType = eLayerType.Grid Then
                    Dim grd As New MapWinGIS.Grid
                    m_Map.Layers.Item(isLoaded).GetGridObject.Close()
                    grd.Open(fileNm)
                    flg = m_Map.Layers.RebuildGridLayer(isLoaded, grd, Functions.GetColorScheme(layerLeg, grd.Minimum, grd.Maximum)) 'this will rebuild the grid layer image so that it matches the values in grdEditGrid
                    grd.Close()
                    m_Map.Layers.Item(isLoaded).GetGridObject.Open(fileNm)
                Else
                    'do nothing for the moment!
                End If
            Else 'is not loaded
                'else load a new layer
                Dim lastSlashPos As Integer = fileNm.LastIndexOf("\")
                'Dim ext$ = fileNm.Substring(lastSlashPos)
                Dim ext$ = System.IO.Path.GetExtension(fileNm)
                If ext.ToLower = "shp" Then
                    Dim lay As MapWindow.Interfaces.Layer
                    lay = m_Map.Layers.Add(fileNm, layerNm)


                Else
                    'layerNm = layerNm + " (.." + ext + ")"
                    Dim grd As New MapWinGIS.Grid
                    grd.Open(fileNm)
                    m_Map.Layers.Add(grd, Functions.GetColorScheme(layerLeg, grd.Minimum, grd.Maximum), layerNm)
                End If
                
                SpecialAddLayer = True
            End If
            'For i = 0 To m_Map.Layers.NumLayers - 1
            'If m_Map.Layers.Item(i).FileName = fileNm Then
            'If m_Map.Layers.Item(i).LayerType = eLayerType.Grid Then
            'm_Map.Layers.Item(i).GetGridObject.Close()
            'grd.Open(fileNm)
            'flg = m_Map.Layers.RebuildGridLayer(i, grd, Functions.GetColorScheme(layerLeg, grd.Minimum, grd.Maximum)) 'this will rebuild the grid layer image so that it matches the values in grdEditGrid
            'grd.Close()
            'm_Map.Layers.Item(i).GetGridObject.Open(fileNm)
            ''m_Map.Layers.Remove(i)
            ''If flg = False Then MsgBox("Can't rebuild grid image ")
            ''m_Map.Layers.Item(i).Visible = True
            'Else
            ''do nothing for the moment!
            'End If

            SpecialAddLayer = True
            Exit Function
            'End If
            'Next

            ''else load a new layer
            'Dim lastSlashPos As Integer = fileNm.LastIndexOf("\")
            'Dim ext$ = fileNm.Substring(lastSlashPos)
            'layerNm = layerNm + " (.." + ext + ")"
            'grd.Open(fileNm)
            'm_Map.Layers.Add(grd, Functions.GetColorScheme(layerLeg, grd.Minimum, grd.Maximum), layerNm)
            'SpecialAddLayer = True

        Catch ex As Exception
            MsgBox("ERROR: unable to load the file")
        End Try

    End Function
    Public Sub UpdateGrids()
        If regManager.isLoaded Then
            Me.StabIndex()
            Me.SpecialAddLayer(Me.SiFileName, "Stability Index", 5)
            Me.SpecialAddLayer(Me.SatFileName, "Saturation", 6)
        Else
            MsgBox("No region information are available")
        End If
    End Sub

    Public Function FillDem() As Integer
        Dim param As String = Chr(34) + Me.demFileName + Chr(34) + " nofile " + Chr(34) + Me.fillDemFileName + Chr(34)
        Return Functions.runCmd(DllPath, "flood", param)
    End Function

    

    Public Function FlowDirAndSlope() As Integer
        Dim param As String = Chr(34) + Me.fillDemFileName + Chr(34) + " " + Chr(34) + Me.flowDirFileName + Chr(34) + " " + Chr(34) + Me.slopeFileName + Chr(34)
        Return Functions.runCmd(DllPath, "setdir", param)
    End Function

    
    Public Function DinfArea() As Integer
        'angfile, areafile, row, col, ccheck
        Dim param As String
        If Me.Check4Contam Then
            param = Chr(34) + Me.flowDirFileName + Chr(34) + " " + Chr(34) + Me.contrAreaFileName + Chr(34) + " 0 0 1"
        Else
            param = Chr(34) + Me.flowDirFileName + Chr(34) + " " + Chr(34) + Me.contrAreaFileName + Chr(34) + " 0 0 0"
        End If
        Return Functions.runCmd(DllPath, "area", param)
    End Function

   

    Public Function StabIndex() As Integer
        Dim calParFileName As String
        'create a new file that original sinmap can use
        Dim path As String = System.IO.Path.GetDirectoryName(calGrdFileName)
        calParFileName = path & "\tempsinmapfile.txt"
        'open the new file

        Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(calParFileName)

        Dim content As String = Replace(Me.gc.ToString, ",", ".") _
                                & " " & Replace(Me.wd.ToString, ",", ".") _
                                & " " & "1" & vbNewLine
        Dim i As Integer

        For i = 0 To Me.regManager.calRegs.GetLength(1) - 1
            content = content & Me.regManager.calRegs(0, i) & " " _
                    & Replace(Me.regManager.calRegs(1, i).ToString, ",", ".") _
                    & " " & Replace(Me.regManager.calRegs(2, i).ToString, ",", ".") _
                    & " " & Replace(Me.regManager.calRegs(3, i).ToString, ",", ".") _
                    & " " & Replace(Me.regManager.calRegs(4, i).ToString, ",", ".") _
                    & " " & Replace((Me.regManager.calRegs(5, i) * Math.PI / 180).ToString, ",", ".") _
                    & " " & Replace((Me.regManager.calRegs(6, i) * Math.PI / 180).ToString, ",", ".") _
                    & " " & Replace(Me.regManager.calRegs(7, i).ToString, ",", ".") _
                    & vbNewLine
        Next
        Replace(content, ",", ".")
        sw.Write(content)
        sw.Close()

        'slopefile,areafile,sindexfile,tergridfile,terparfile,satfile
        Dim param As String = Chr(34) + Me.slopeFileName + Chr(34) + " " _
                            + Chr(34) + Me.contrAreaFileName + Chr(34) + " " _
                            + Chr(34) + Me.SiFileName + Chr(34) + " " _
                            + Chr(34) + Me.calGrdFileName + Chr(34) + " " _
                            + Chr(34) + calParFileName + Chr(34) + " " _
                            + Chr(34) + Me.SatFileName + Chr(34)
        'Dim frm As New frmHydrMod(DllPath, "sindex", param)
        'Try 'sometime, the exe is faster then the dialog, so frm is killed before showing
        'frm.ShowDialog()
        'Catch ex As Exception
        'End Try
        'Return 0
        Return Functions.runCmd(DllPath, "sindex", param)
    End Function

    Public Function StabIndex2() As Integer
        'Dim model As New StabilityModel.Hydrology()
        'Dim res As Integer
        'res = model.sindex(Me.slopeFileName, Me.contrAreaFileName, Me.SiFileName, Me.calGrdFileName, Me.calFileName, Me.SatFileName, Me.gc, Me.wd, Me.soild)
        'Return res
    End Function

    Private Sub dummy()
        Dim stringa As String
        stringa = Chr(34) & Me.calGrdFileName & Chr(34)
        MsgBox("stringa: " & stringa)
    End Sub

    Private Sub MakeReportStatistics()
        
        Dim regNum As Integer = Me.regManager.regNum


        Dim data(,) As Single 'container for main statistic data
        ReDim data(regNum, 12)
        Dim area(6) As Single 'container for area transformation
        Dim percArea(6) As Single
        Dim numLs(6) As Single
        Dim densLs(6) As Single 'container for landslide density
        Dim percLs(6) As Single
        Dim totArea As Single 'total area of stability map
        Dim totLs As Integer 'total number of landslide
        Dim pxA As Single = Me.regManager.pxArea
        Dim i, r As Integer


        'define file
        Dim frm As SaveFileDialog = New SaveFileDialog()
        frm.Filter = "CSV|*.csv"
        frm.Title = "Save as CSV"
        frm.ShowDialog()

        Dim fileName As String = frm.FileName

        'load region ID
        For r = 0 To regNum
            data(r, 0) = Me.regManager.calRegs(0, r)
        Next

        MakeStatistics(data)

        Dim label() As String = {"Area [km^2]", "% of Region", "#Landslides", "% of slides", "LS Density [#/km^2]"}

        Dim txt As String = "MW-SINMAP Stability Index Software for Mapwindow GIS" & vbCrLf
        txt = txt & "Based on the original package version 1.0  3/30/98" & vbCrLf
        txt = txt & "See http://www.engineering.usu.edu/dtarb/sinmap.html for further informations" & vbCrLf
        txt = txt & "This version is provided by Enrico A. Chiaradia - University of Milan" & vbCrLf
        txt = txt & "e-mail:" & ";" & "enrico.chiaradia@unimi.it" & vbCrLf & vbCrLf

        For r = 0 To regNum
            'calculate area and percentage of area

            For i = 1 To 6
                area(i - 1) = data(r, i) * pxA / 1000000
            Next

            totArea = sum(area) - area(6)
            area(6) = totArea

            For i = 0 To 6
                percArea(i) = 100 * (area(i) / totArea)
            Next

            'calculate total landslide and percentage of it
            For i = 7 To 12
                numLs(i - 7) = data(r, i)
            Next
            'total landslide
            totLs = sum(numLs) - numLs(6)
            numLs(6) = totLs

            For i = 0 To 6
                percLs(i) = 100 * (numLs(i) / totLs)
                densLs(i) = numLs(i) / (area(i))
            Next

            txt = txt & ";" & "Stable" & ";" & "Moderately Stable" & ";" & _
                        "Quasi-stable" & ";" & "Lower Threshold" & ";" & _
                        "Upper Threshold" & ";" & "Defended" & ";" & "Total" & vbCrLf

            txt = txt & "Region " & data(r, 0).ToString & vbCrLf
            txt = txt & label(0)
            For i = 0 To 6
                txt = txt & ";" & Math.Round(area(i), 3)
            Next

            txt = txt & vbCrLf
            txt = txt & label(1)
            For i = 0 To 6
                txt = txt & ";" & Math.Round(percArea(i), 3)
            Next

            txt = txt & vbCrLf
            txt = txt & label(2)
            For i = 0 To 6
                txt = txt & ";" & Math.Round(numLs(i))
            Next

            txt = txt & vbCrLf
            txt = txt & label(3)
            For i = 0 To 6
                txt = txt & ";" & Math.Round(percLs(i), 3)
            Next

            txt = txt & vbCrLf
            txt = txt & label(4)
            For i = 0 To 6
                txt = txt & ";" & Math.Round(densLs(i), 3)
            Next


            txt = txt & vbCrLf & vbCrLf
        Next r

        'create and write file
        Dim fs As New FileStream(fileName, FileMode.Create, FileAccess.Write)
        Dim s As New StreamWriter(fs)
        s.Write(txt)
        s.Close()
        fs.Close()
        
    End Sub

    Private Sub MakeStatistics(ByRef stat(,) As Single)
        Dim calregdata(,) As Single
        Dim sidata(,) As Single
        Dim nodata As Single
        Dim ncols As Single
        Dim nrows As Single
        Dim l, c, r As Integer
        Dim grd As New MapWinGIS.Grid

        Dim numOfReg As Integer = stat.GetLength(0)
        'MsgBox("dim stat = " & numOfReg)

        'load calregdata
        l = Functions.IsAlreadyLoaded(Me.calGrdFileName, Me)
        If l > -1 Then
            grd = Me.m_Map.Layers(l).GetGridObject
        Else
            grd.Open(Me.calGrdFileName)
        End If

        ncols = grd.Header.NumberCols
        nrows = grd.Header.NumberRows
        nodata = grd.Header.NodataValue

        ReDim calregdata(ncols, nrows)
        For c = 0 To ncols
            For r = 0 To nrows
                calregdata(c, r) = grd.Value(c, r)
            Next
        Next
        '****************************************

        'load sidata
        l = Functions.IsAlreadyLoaded(Me.SiFileName, Me)
        If l > -1 Then
            grd = Me.m_Map.Layers(l).GetGridObject
        Else
            grd.Open(Me.SiFileName)
        End If
        Dim nodata2 As Double = grd.Header.NodataValue

        If grd.Header.NumberCols <> ncols Or grd.Header.NumberRows <> nrows Then
            MsgBox("Grids data must have the same header")
            Exit Sub
        End If

        ReDim sidata(ncols, nrows)

        For c = 0 To ncols
            For r = 0 To nrows
                sidata(c, r) = grd.Value(c, r)
            Next
        Next
        '****************************************

        'calculate area for each stability class in the region
        Dim i As Integer
        For c = 0 To ncols
            For r = 0 To nrows
                If (calregdata(c, r) <> nodata) Then
                    For i = 0 To numOfReg - 1
                        If (stat(i, 0) = calregdata(c, r)) Then
                            If sidata(c, r) <> nodata2 Then
                                If (sidata(c, r) > 1.5) Then stat(i, 1) = stat(i, 1) + 1
                                If (sidata(c, r) > 1.25 And sidata(c, r) <= 1.5) Then stat(i, 2) = stat(i, 2) + 1
                                If (sidata(c, r) > 1.0 And sidata(c, r) <= 1.25) Then stat(i, 3) = stat(i, 3) + 1
                                If (sidata(c, r) > 0.5 And sidata(c, r) <= 1.0) Then stat(i, 4) = stat(i, 4) + 1
                                If (sidata(c, r) > 0.0 And sidata(c, r) <= 0.5) Then stat(i, 5) = stat(i, 5) + 1
                                If (sidata(c, r) <= 0.0) Then stat(i, 6) = stat(i, 6) + 1

                            End If
                            Exit For 'go out, calReg id is unique
                        End If
                    Next
                End If
            Next
        Next

        'count how many landslide occour in each stability region
        'open shapefile
        Dim lsSf As New MapWinGIS.Shapefile
        lsSf.Open(Me.slpFileName)
        Dim nSf, s As Integer
        Dim x, y As Double
        Dim xcell, ycell As Integer

        nSf = lsSf.NumShapes

        For s = 0 To nSf - 1
            x = lsSf.Shape(s).Point(0).x
            y = lsSf.Shape(s).Point(0).y
            grd.ProjToCell(x, y, xcell, ycell)
            For i = 0 To numOfReg - 1
                If (stat(i, 0) = calregdata(xcell, ycell)) Then
                    If sidata(xcell, ycell) <> nodata2 Then
                        If (sidata(xcell, ycell) > 1.5) Then stat(i, 7) = stat(i, 7) + 1
                        If (sidata(xcell, ycell) > 1.25 And sidata(xcell, ycell) <= 1.5) Then stat(i, 8) = stat(i, 8) + 1
                        If (sidata(xcell, ycell) > 1.0 And sidata(xcell, ycell) <= 1.25) Then stat(i, 9) = stat(i, 9) + 1
                        If (sidata(xcell, ycell) > 0.5 And sidata(xcell, ycell) <= 1.0) Then stat(i, 10) = stat(i, 10) + 1
                        If (sidata(xcell, ycell) > 0.0 And sidata(xcell, ycell) <= 0.5) Then stat(i, 11) = stat(i, 11) + 1
                        If (sidata(xcell, ycell) <= 0.0) Then stat(i, 12) = stat(i, 12) + 1

                    End If
                End If
            Next i

        Next

        lsSf.Close()

    End Sub

End Class
