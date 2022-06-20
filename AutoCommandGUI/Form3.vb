Public Class Form3
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.FolderBrowserDialog1.ShowDialog()
        Me.TextBox1.Text = Me.FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Me.TextBox1.Text = "" Then
            MsgBox("您必须要设置一个指令集保存的位置。", 48)
            Exit Sub
        End If
        If Not System.IO.Directory.Exists(Me.TextBox1.Text) Then
            MsgBox("您设置的位置不存在。", 48)
            Exit Sub
        End If
        If Me.TextBox2.Text = "" Then
            MsgBox("您必须要为指令集设置一个名称。", 48)
            Exit Sub
        End If
        '把基本指令集复制到目标目录
        My.Computer.FileSystem.CopyDirectory(Application.StartupPath + "\BaseSet", Me.TextBox1.Text + "\" + Me.TextBox2.Text)
        CURRENT_PATH = Me.TextBox1.Text + "\" + Me.TextBox2.Text
        IS_SAVED = True
        Me.TextBox1.Text = CURRENT_PATH
        SET_TYPE = True '目录是true
        Form1.ListBox1.Items.Clear()
        Form1.Panel1.Enabled = True
        Form1.Panel2.Enabled = True
        Form1.Button14.Enabled = True
        Form1.Button7.Enabled = True
        Form1.Button2.Enabled = True
        On Error Resume Next
        Form1.ListBox1.Items.Add("")
        Form1.ListBox1.SelectedIndex = 0
        Form1.NumericUpDown1.Value = 200
        Form1.NumericUpDown2.Value = 70
        Form1.NumericUpDown3.Value = 2000
        Me.Close()
    End Sub
End Class