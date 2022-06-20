Public Class Form2
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Me.TextBox1.Text = "" Then
            MsgBox("待检测指令集不可为空！"， 48)
            Exit Sub
        End If
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro2.StartInfo.Arguments = Chr(34) + Me.TextBox1.Text + Chr(34) + " 3"
        pro2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        pro2.StartInfo.RedirectStandardOutput = True
        pro2.StartInfo.UseShellExecute = False
        pro2.StartInfo.CreateNoWindow = True
        pro2.Start()
        While True
            Application.DoEvents()
            If pro2.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        Dim check_out As String = pro2.StandardOutput.ReadToEnd()
        Me.TextBox2.Text = ""
        Me.TextBox2.Text = check_out
        If Len(Me.TextBox2.Text) > 0 Then
            MsgBox("经过初步检查，发现指令集中含有错误。"， 48)
        Else
            MsgBox("经过初步检查：指令集没有严重错误。")
        End If
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.TextBox1.Text = CURRENT_PATH
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.OpenFileDialog1.ShowDialog()
        Me.TextBox1.Text = Me.OpenFileDialog1.FileName
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.FolderBrowserDialog1.ShowDialog()
        Me.TextBox1.Text = Me.FolderBrowserDialog1.SelectedPath
    End Sub
End Class