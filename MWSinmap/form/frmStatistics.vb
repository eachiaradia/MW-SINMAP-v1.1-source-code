Imports System.Windows.Forms

Public Class frmStatistics

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub AddLineText(ByVal txt As String)
        Dim exTxt As String = Me.TextBox1.Text
        Me.TextBox1.Text = exTxt & vbCrLf & txt
    End Sub

End Class
