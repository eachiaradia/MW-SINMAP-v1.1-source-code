Imports System.Windows.Forms

Public Class frmSelectShape
    Dim store As MainClass
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
        'not robust!!!
        If Me.Text = "Select a region map" Then store.calFileName = Me.TextBox1.Text
        If Me.Text = "Select a soil slip map" Then store.slpFileName = Me.TextBox1.Text

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim sfd As New OpenFileDialog
        Dim filename As String

        With sfd
            .Filter = "shape file (*.shp) |*.shp|All other supported vector files (*.*)|*.*"
            .FilterIndex = 1
            .Title = "Select a shape file ..."
            sfd.ShowDialog()
            filename = .FileName
        End With
        Me.TextBox1.Text = filename
    End Sub

    Public Sub New(ByRef pluginClass As MainClass, ByVal title$, ByVal label$)

        ' Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()
        Me.Label1.Text = label
        Me.Text = title

        store = pluginClass

        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent().

    End Sub

 
End Class
