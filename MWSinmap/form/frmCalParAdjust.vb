Imports System.Windows.Forms

Public Class frmCalParAdjust

    Dim store As CalRegManager
    Dim regList() As Integer

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Me.Button1.Enabled = True Then
            MsgBox("Please press Apply button in order to accept changes")
            Exit Sub
        End If
        Dim flg As Object = MsgBox("Calibration parameters will be permanently saved and grids updated. Do you want to continue?", MsgBoxStyle.YesNo)
        If flg = System.Windows.Forms.DialogResult.Yes Then
            Me.store.SaveCalibrationParam()
            Me.store.store.UpdateGrids()
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        Else
            'DO nothing!
        End If
        
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub UpLoadParameter(ByRef calRegMan As CalRegManager, ByVal reg As Integer)
        store = calRegMan
        'get selected item number

        Dim i As Integer
        If reg = -1 Then
            ReDim regList(store.regNum)
            For i = 0 To store.regNum
                Me.ListBox1.Items.Add("Region " & store.calRegs(0, i))
                regList(i) = store.calRegs(0, i)
            Next
        Else
            ReDim regList(0)
            For i = 0 To store.regNum
                If store.calRegs(0, i) = reg Then
                    regList(0) = reg
                    Me.ListBox1.Items.Add("Region " & store.calRegs(0, i))
                    Me.ListBox1.SetSelected(0, True)
                End If
            Next
        End If

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        'get selected item number
        Dim selReg As Integer = Me.ListBox1.SelectedIndex
        If selReg < 0 Then Exit Sub
        selReg = regList(selReg) - 1

        Me.TextBox1.Text = store.calRegs(1, selReg)
        Me.TextBox2.Text = store.calRegs(2, selReg)
        Me.TextBox3.Text = store.calRegs(3, selReg)
        Me.TextBox4.Text = store.calRegs(4, selReg)
        Me.TextBox5.Text = store.calRegs(5, selReg)
        Me.TextBox6.Text = store.calRegs(6, selReg)
        Me.TextBox7.Text = store.calRegs(7, selReg)
        Me.Button1.Enabled = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        'bug 
        Dim selRegName As String = Me.ListBox1.SelectedItem
        Dim sep() As Char = {" "}
        Dim subString() As String = selRegName.Split(sep, 2)
        Dim selReg As Integer
        'get index of region
        Dim i As Integer
        For i = 0 To store.regNum
            If store.calRegs(0, i) = CType(subString(1), Integer) Then
                selReg = i
            End If
        Next


         If selReg < 0 Then Exit Sub

        'store data in memory

        store.calRegs(1, selReg) = Me.TextBox1.Text
        store.calRegs(2, selReg) = Me.TextBox2.Text
        store.calRegs(3, selReg) = Me.TextBox3.Text
        store.calRegs(4, selReg) = Me.TextBox4.Text
        store.calRegs(5, selReg) = Me.TextBox5.Text
        store.calRegs(6, selReg) = Me.TextBox6.Text
        store.calRegs(7, selReg) = Me.TextBox7.Text

        Me.Button1.Enabled = False
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox5.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        Me.Button1.Enabled = True
    End Sub

    Private Sub TextBox7_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox7.TextChanged
        Me.Button1.Enabled = True
    End Sub
End Class
