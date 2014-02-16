Imports System.Windows.Forms

Public Class frmDefaultValue

    Dim store As MainClass

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

        store.gc = Me.TextBox1.Text
        store.wd = Me.TextBox2.Text
        store.numSAplot = Me.TextBox3.Text
        store.LAlowPerc = Me.TextBox4.Text
        store.Check4Contam = Me.CheckBox1.Checked
        store.UseExistGrd = Me.RadioButton1.Checked
        store.RecalculateGrd = Me.RadioButton2.Checked

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByRef pluginClass As MainClass)
        store = pluginClass

        ' Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()

        Me.TextBox1.Text = store.gc
        Me.TextBox2.Text = store.wd
        Me.TextBox3.Text = store.numSAplot
        Me.TextBox4.Text = store.LAlowPerc
        Me.CheckBox1.Checked = store.Check4Contam
        Me.RadioButton1.Checked = store.UseExistGrd
        Me.RadioButton2.Checked = store.RecalculateGrd

        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent().

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.TextBox1.Text = 9.81
        Me.TextBox2.Text = 1000
        Me.TextBox3.Text = 2000
        Me.TextBox4.Text = 10
        Me.CheckBox1.Checked = True
        Me.RadioButton1.Checked = True
        Me.RadioButton2.Checked = False

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            CheckBox1.Checked = False
            CheckBox1.Enabled = False
        End If
        If RadioButton1.Checked = False Then
            CheckBox1.Enabled = True
            CheckBox1.Checked = True
        End If
    End Sub

  
End Class
