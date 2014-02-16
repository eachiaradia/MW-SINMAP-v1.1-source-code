Imports System.Windows.Forms

Public Class frmSelectDemGrd
    Dim store As MainClass

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK

        store.demFileName = Me.TextBox1.Text
        store.calGrdFileName = Me.TextBox2.Text
        store.fillDemFileName = Me.TextBox3.Text
        store.flowDirFileName = Me.TextBox4.Text
        store.slopeFileName = Me.TextBox5.Text
        store.contrAreaFileName = Me.TextBox6.Text
        store.SiFileName = Me.TextBox7.Text
        store.SatFileName = Me.TextBox8.Text
        Me.Close()
        'add raster layer
        'store.SpecialAddLayer(store.demFileName, "Base Dem Grid", 1)
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByRef pluginClass As MainClass)
        store = pluginClass

        ' Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()

        Me.TextBox1.Text = store.demFileName
        Me.TextBox2.Text = store.calGrdFileName
        Me.TextBox3.Text = store.fillDemFileName
        Me.TextBox4.Text = store.flowDirFileName
        Me.TextBox5.Text = store.slopeFileName
        Me.TextBox6.Text = store.contrAreaFileName
        Me.TextBox7.Text = store.SiFileName
        Me.TextBox8.Text = store.SatFileName

        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent().

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.TextBox1.Text = ""
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim sfd As New OpenFileDialog
        Dim filename As String

        With sfd
            .Filter = "GRID (*.asc) |*.asc|All other supported grid files (*.*)|*.*"
            .FilterIndex = 1
            .Title = "Select DEM grid ..."
            sfd.ShowDialog()
            filename = .FileName
        End With
        Me.TextBox1.Text = filename
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Dim demname$ = Me.TextBox1.Text
        Me.TextBox2.Text = addLabel(demname, "cal")
        Me.TextBox3.Text = addLabel(demname, "fel")
        Me.TextBox4.Text = addLabel(demname, "ang")
        Me.TextBox5.Text = addLabel(demname, "slp")
        Me.TextBox6.Text = addLabel(demname, "sca")
        Me.TextBox7.Text = addLabel(demname, "si")
        Me.TextBox8.Text = addLabel(demname, "sat")
    End Sub

    Private Function addLabel(ByVal origFileName$, ByVal label$) As String
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
End Class
