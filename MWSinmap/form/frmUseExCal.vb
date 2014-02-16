Imports System.Windows.Forms

Public Class frmUseExCal
    Dim store As MainClass

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
        store.calGrdFileName = Me.TextBox1.Text
        store.calFileName = Me.TextBox2.Text
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim sfd As New OpenFileDialog
        Dim filename As String

        With sfd
            .Filter = "GRID (*.asc) |*.asc|All other supported grid files (*.*)|*.*"
            .FilterIndex = 1
            .Title = "Select Calibration Grid ..."
            sfd.ShowDialog()
            filename = .FileName
        End With
        Me.TextBox1.Text = filename
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim sfd As New OpenFileDialog
        Dim filename As String

        With sfd
            .Filter = "SHAPEFILE (*.shp) |*.shp|All other supported vector files (*.*)|*.*"
            .FilterIndex = 1
            .Title = "Select Calibration Shapefile ..."
            sfd.ShowDialog()
            filename = .FileName
        End With
        Me.TextBox2.Text = filename
    End Sub

    Public Sub New(ByRef pluginClass As MainClass)
        store = pluginClass

        ' Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()

        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent().
        Me.TextBox1.Text = store.calGrdFileName
        Me.TextBox2.Text = store.calFileName

    End Sub

End Class
