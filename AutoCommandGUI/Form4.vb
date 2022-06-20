Public Class Form4
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.FolderBrowserDialog1.ShowDialog()
        Me.TextBox1.Text = Me.FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If CURRENT_PATH = "" Then
            MsgBox("当前指令集工作目录为空，不能完成打包操作", 48)
            Exit Sub
        End If
        If Not System.IO.Directory.Exists(Me.TextBox1.Text) Then
            MsgBox("您输入的目录路径不存在。", 48)
            Exit Sub
        End If
        If Me.TextBox2.Text = "" Then
            MsgBox("文件名不可为空。", 48)
            Exit Sub
        End If
        Dim outf As String = Me.TextBox1.Text + "\" + Me.TextBox2.Text + ".acs"
        If System.IO.File.Exists(outf) Then
            Dim rst As Integer = MsgBox(outf + "已存在，是否替换它？", vbYesNo)
            If rst = vbNo Then
                Exit Sub
            End If
        End If
        '----打包-----
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro2.StartInfo.Arguments = Chr(34) + CURRENT_PATH + Chr(34) + " 2" '打包的第二个参数是2。
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
        Me.TextBox3.Text = ""
        Me.TextBox3.Text = check_out
        '复制
        On Error Resume Next
        System.IO.File.Delete(outf)
        On Error Resume Next
        System.IO.File.Copy(CURRENT_PATH + ".acs", outf)
        On Error Resume Next
        System.IO.File.Delete(CURRENT_PATH + ".acs")
        If System.IO.File.Exists(outf) Then
            MsgBox("打包成功！")
        Else
            MsgBox("打包失败，请检查目录权限或您的输入是否合法。", 48)
        End If
    End Sub

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class