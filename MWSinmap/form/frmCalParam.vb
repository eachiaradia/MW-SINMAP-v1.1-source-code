Imports System.Windows.Forms

Public Class frmCalParam
    Dim store As MainClass

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

        store.TRlb = Me.TextBox1.Text
        store.TRub = Me.TextBox2.Text
        store.Clb = Me.TextBox3.Text
        store.Cub = Me.TextBox4.Text
        store.PHIlb = Me.TextBox5.Text
        store.PHIub = Me.TextBox6.Text
        store.soild = Me.TextBox7.Text

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByRef pluginClass As MainClass)
        store = pluginClass

        ' Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()

        Me.TextBox1.Text = store.TRlb
        Me.TextBox2.Text = store.TRub
        Me.TextBox3.Text = store.Clb
        Me.TextBox4.Text = store.Cub
        Me.TextBox5.Text = store.PHIlb
        Me.TextBox6.Text = store.PHIub
        Me.TextBox7.Text = store.soild


        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent().

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.TextBox1.Text = 2000
        Me.TextBox2.Text = 3000
        Me.TextBox3.Text = 0
        Me.TextBox4.Text = 0.25
        Me.TextBox5.Text = 35
        Me.TextBox6.Text = 45
        Me.TextBox7.Text = 2000

    End Sub
End Class
