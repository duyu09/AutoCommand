'Copyright (c) 2022 Qilu University of Technology,School of Computer Science and Technology,Group of AutoCommand Software System(Duyu,Chen Yongquan,Liu Jia) All rights reserved.
'齐鲁工业大学 计算机科学与技术学院 AutoCommand软件开发小组(杜宇，陈勇全，刘佳)，保留所有权利。
'Source code of Main Process of AutoCommand Software System.
Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.FolderBrowserDialog1.ShowDialog()
        Dim temp_path As String
        temp_path = Me.FolderBrowserDialog1.SelectedPath
        If temp_path = "" Then
            Exit Sub
        End If
        If Not System.IO.Directory.Exists(temp_path) Then
            MsgBox("您输入的文件夹不存在"， 48)
            GoTo errhandle
            Exit Sub
        End If
        '--检查指令集--
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro2.StartInfo.Arguments = Chr(34) + temp_path + Chr(34) + " 3"
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
        If check_out.IndexOf("[CHECK]") >= 0 Then
            'MsgBox("指令集可能有错误，若能正常打开请忽略。"， 48)
            'GoTo errhandle
            'Exit Sub
        End If
        '写历史记录
        On Error Resume Next
        Dim filehi02 As New System.IO.StreamWriter(Application.StartupPath + "\AC_History.log", True)
        filehi02.WriteLine("[" + Now + "] 指令集文件夹：" + temp_path)
        filehi02.Close()
        CURRENT_PATH = temp_path
        Me.TextBox1.Text = CURRENT_PATH
        SET_TYPE = True                 '目录是true
        Me.ListBox1.Items.Clear()
        Me.Panel1.Enabled = True
        Me.Panel2.Enabled = True
        Me.Button14.Enabled = True
        Me.Button7.Enabled = True
        Me.Button2.Enabled = True
        On Error Resume Next
        System.IO.File.Delete(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp")
        Dim pro As New Process
        pro.StartInfo.FileName = "AutoCommand_Bridge.exe"
        pro.StartInfo.Arguments = Chr(34) + CURRENT_PATH + "\cmd.xls" + Chr(34) + " 0"
        pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        'pro.StartInfo.RedirectStandardOutput = True
        pro.StartInfo.UseShellExecute = False
        pro.StartInfo.CreateNoWindow = True
        pro.Start()
        While True
            Application.DoEvents()
            If pro.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(500)
        Application.DoEvents()
        Dim str As String
        If Not System.IO.File.Exists(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp") Then
            MsgBox("出现了未知错误，请检查程序运行权限。"， 48)
            Me.ListBox1.Items.Clear()
            Me.Panel1.Enabled = False
            Me.Panel2.Enabled = False
            Me.Button14.Enabled = False
            Me.Button7.Enabled = False
            Me.Button2.Enabled = False
            CURRENT_PATH = ""
            GoTo errhandle
            Exit Sub
        End If
        Dim line As String
        Dim qwer_tmp As String = Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp"
        On Error GoTo errhandle
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(qwer_tmp, System.Text.Encoding.Default)
        Do While sr.Peek() > 0
            line = sr.ReadLine()
            If Not line = "" Then
                Me.ListBox1.Items.Add(line)
            End If
        Loop
        sr.Close()
        sr = Nothing
        On Error GoTo errhandle
        Dim sr2 As System.IO.StreamReader = New System.IO.StreamReader(CURRENT_PATH + "\setup.config", System.Text.Encoding.Default)
        line = sr2.ReadLine()
        Me.NumericUpDown1.Value = CDbl(line) * 1000
        line = sr2.ReadLine()
        Me.NumericUpDown2.Value = CDbl(line) * 100
        line = sr2.ReadLine()
        Me.NumericUpDown3.Value = CDbl(line) * 1000
        sr2.Close()
        sr2 = Nothing
        If Me.ListBox1.Items.Count = 0 Then
            Button12_Click(Nothing, Nothing)
        End If
        Me.ListBox1.SelectedIndex = 0
        Exit Sub
errhandle:
        MsgBox("指令集有错误，有可能无法正常打开", 48)
        'CURRENT_PATH = ""
        IS_SAVED = True
        Exit Sub
    End Sub


    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        Me.OpenFileDialog1.ShowDialog()
        Dim temp_path As String
        temp_path = Me.OpenFileDialog1.FileName()
        If temp_path = "" Then
            Exit Sub
        End If
        If Not System.IO.File.Exists(temp_path) Then
            MsgBox("您输入的文件不存在"， 48)
            GoTo errh2
            Exit Sub
        End If
        '-------------
        '--检查指令集--
        '-------------
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro2.StartInfo.Arguments = Chr(34) + temp_path + Chr(34) + " 3"
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
        If check_out.IndexOf("[CHECK]") >= 0 Then
            'MsgBox("指令集可能有错误，若能正常打开请忽略。"， 48)
            'GoTo errhandle
            'Exit Sub
        End If
        Me.TextBox1.Text = temp_path
        temp_path = Microsoft.VisualBasic.Right(temp_path, Len(temp_path) - InStrRev(temp_path, "\"))
        temp_path = Microsoft.VisualBasic.Left(temp_path, InStr(temp_path, ".") - 1)
        Dim azxsd As String = Environment.GetEnvironmentVariable("temp") + "\" + temp_path
        If Not System.IO.Directory.Exists(azxsd) Then
            Me.TextBox1.Text = ""
            MsgBox("指令集有错误，有可能无法打开"， 48)
            GoTo errh2
            Exit Sub
        End If
        CURRENT_PATH = azxsd
        SET_TYPE = False '文件是false
        Me.ListBox1.Items.Clear()
        '写历史记录
        On Error Resume Next
        Dim filehi03 As New System.IO.StreamWriter(Application.StartupPath + "\AC_History.log", True)
        filehi03.WriteLine("[" + Now + "] " + "ACS文件：" + temp_path)
        filehi03.Close()
        Me.Panel1.Enabled = True
        Me.Panel2.Enabled = True
        Me.Button14.Enabled = True
        Me.Button7.Enabled = True
        Me.Button2.Enabled = True
        On Error Resume Next
        System.IO.File.Delete(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp")
        Dim pro As New Process
        pro.StartInfo.FileName = "AutoCommand_Bridge.exe"
        pro.StartInfo.Arguments = Chr(34) + CURRENT_PATH + "\cmd.xls" + Chr(34) + " 0"
        pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        'pro.StartInfo.RedirectStandardOutput = True
        pro.StartInfo.UseShellExecute = False
        pro.StartInfo.CreateNoWindow = True
        pro.Start()
        While True
            Application.DoEvents()
            If pro.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(500)
        Application.DoEvents()
        Dim str As String
        If Not System.IO.File.Exists(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp") Then
            MsgBox("出现了未知错误，请检查程序运行权限。"， 48)
            Me.ListBox1.Items.Clear()
            Me.Panel1.Enabled = False
            Me.Panel2.Enabled = False
            Me.Button14.Enabled = False
            Me.Button7.Enabled = False
            Me.Button2.Enabled = False
            CURRENT_PATH = ""
            Exit Sub
        End If
        Dim line As String
        Dim qwer_tmp As String = Environment.GetEnvironmentVariable("temp") + "\AC_BRI_OUT.tmp"
        On Error GoTo errh2
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(qwer_tmp, System.Text.Encoding.Default)
        Do While sr.Peek() > 0
            line = sr.ReadLine()
            If Not line = "" Then
                Me.ListBox1.Items.Add(line)
            End If
        Loop
        sr.Close()
        sr = Nothing
        On Error GoTo errh2
        Dim sr2 As System.IO.StreamReader = New System.IO.StreamReader(CURRENT_PATH + "\setup.config", System.Text.Encoding.Default)
        line = sr2.ReadLine()
        Me.NumericUpDown1.Value = CDbl(line) * 1000
        line = sr2.ReadLine()
        Me.NumericUpDown2.Value = CDbl(line) * 100
        line = sr2.ReadLine()
        Me.NumericUpDown3.Value = CDbl(line) * 1000
        sr2.Close()
        sr2 = Nothing
        If Me.ListBox1.Items.Count = 0 Then
            Button12_Click(Nothing, Nothing)
        End If
        Me.ListBox1.SelectedIndex = 0
        Exit Sub
errh2:
        MsgBox("指令集有错误，有可能无法正常打开", 48)
        'CURRENT_PATH = ""
        IS_SAVED = True
        Exit Sub
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        On Error GoTo qwe
        Dim str As String = ListBox1.Items().Item(ListBox1.SelectedIndex)
        If str = "" Then
            Exit Sub
        End If
        Dim str2 As String = Microsoft.VisualBasic.Mid(str, 1, 2)
        If str2 = "01" Or str2 = "02" Or str2 = "03" Then
            Me.ComboBox1.SelectedIndex = 0
            If str2 = "01" Then
                Me.RadioButton1.Checked = True
            ElseIf str2 = "02" Then
                Me.RadioButton2.Checked = True
            ElseIf str2 = "03" Then
                Me.RadioButton3.Checked = True
            End If
            Dim str3 As String = Mid(str, 4, 3)
            Me.NumericUpDown4.Value = CInt(str3)
            Dim str4 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            If Microsoft.VisualBasic.Left(str4, 3) = "[F]" Then
                Me.RadioButton4.Checked = True
                Me.TextBox3.Text = CURRENT_PATH + "\image\" + Microsoft.VisualBasic.Right(str4, Len(str4) - 3)
                Me.TextBox7.Text = Microsoft.VisualBasic.Right(str4, Len(str4) - 3)
                Me.PictureBox1.Image = Image.FromFile(Me.TextBox3.Text)
                RadioButton4_CheckedChanged(Nothing, Nothing)
            Else
                Me.RadioButton5.Checked = True
                Dim str5 As String = Microsoft.VisualBasic.Right(str4, Len(str4) - 3)
                Me.NumericUpDown5.Value = CInt(Microsoft.VisualBasic.Left(str5, InStr(str5, ",")))
                Me.NumericUpDown6.Value = CInt(Microsoft.VisualBasic.Right(str5, Len(str5) - InStr(str5, ",")))
                RadioButton4_CheckedChanged(Nothing, Nothing)
            End If
        ElseIf str2 = "04" Then
            Me.ComboBox1.SelectedIndex = 1
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str6 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            If Microsoft.VisualBasic.Left(str6, 3) = "[F]" Then
                Me.RadioButton7.Checked = True
                Me.TextBox4.Text = CURRENT_PATH + "\image\" + Microsoft.VisualBasic.Mid(str6, 4, InStr(str6, "[I]") - InStr(str6, "[F]") - 3)
                Me.TextBox8.Text = Microsoft.VisualBasic.Mid(str6, 4, InStr(str6, "[I]") - InStr(str6, "[F]") - 3)
                Me.PictureBox2.Image = Image.FromFile(Me.TextBox4.Text)
                Me.TextBox2.Text = Microsoft.VisualBasic.Right(str6, Len(str6) - InStr(str6, "[I]") - 2)
                RadioButton7_CheckedChanged(Nothing, Nothing)
            Else
                Me.RadioButton6.Checked = True
                Me.TextBox2.Text = Microsoft.VisualBasic.Right(str6, Len(str6) - InStr(str6, "[I]") - 2)
                Dim qw As String = Microsoft.VisualBasic.Mid(str6, 4, InStr(str6, "[I]") - InStr(str6, "[F]") - 4)
                Me.NumericUpDown8.Value = CInt(Microsoft.VisualBasic.Left(qw, InStr(qw, ",")))
                Me.NumericUpDown7.Value = CInt(Microsoft.VisualBasic.Right(qw, Len(qw) - InStr(qw, ",")))
                RadioButton7_CheckedChanged(Nothing, Nothing)
            End If


        ElseIf str2 = "05" Then
            Me.ComboBox1.SelectedIndex = 3
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str7 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            Me.NumericUpDown11.Value = CInt(str7)
        ElseIf str2 = "06" Then
            Me.ComboBox1.SelectedIndex = 2
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str7 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            Me.NumericUpDown9.Value = CInt(Microsoft.VisualBasic.Left(str7, InStr(str7, ",")))
            Me.NumericUpDown10.Value = CInt(Microsoft.VisualBasic.Right(str7, Len(str7) - InStr(str7, ",")) * 1000)
        ElseIf str2 = "07" Then
            Me.ComboBox1.SelectedIndex = 4
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str7 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            Me.TextBox5.Text = str7
        ElseIf str2 = "08" Or str2 = "09" Then
            Me.ComboBox1.SelectedIndex = 5
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str9 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            If str2 = "08" Then
                If str9 = "[Q]" Then
                    Me.RadioButton9.Checked = True
                Else
                    Me.RadioButton8.Checked = True
                End If
            Else
                Me.RadioButton10.Checked = True
            End If

        ElseIf str2 = "10" Then
            Me.ComboBox1.SelectedIndex = 6
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str9 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            Me.TrackBar1.Value = CInt(str9)
            Me.Label23.Text = Me.TrackBar1.Value
        ElseIf str2 = "11" Then
            Me.ComboBox1.SelectedIndex = 8
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
        ElseIf str2 = "12" Then
            Me.ComboBox1.SelectedIndex = 7
            Me.NumericUpDown4.Value = CInt(Mid(str, 4, 3))
            Dim str12 As String = Microsoft.VisualBasic.Right(str, Len(str) - 7)
            Me.TextBox6.Text = str12
        End If
qwe:
        Exit Sub
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim num As Integer = ListBox1.SelectedIndex
        If num > 0 Then
            Dim temp As String
            temp = ListBox1.Items.Item(num)
            ListBox1.Items.Item(num) = ListBox1.Items.Item(num - 1)
            ListBox1.Items.Item(num - 1) = temp
            ListBox1.SelectedIndex = num - 1
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Me.ComboBox1.SelectedIndex = 0 Then
            Me.TabPage1.Enabled = True
            Me.TabPage1.Show()
            Me.TabControl1.SelectedIndex = 0
            Me.TabPage1.Select()
            Me.TabPage2.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 1 Then
            Me.TabPage2.Enabled = True
            Me.TabPage2.Show()
            Me.TabControl1.SelectedIndex = 1
            Me.TabPage2.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 2 Then
            Me.TabPage3.Enabled = True
            Me.TabPage3.Show()
            Me.TabControl1.SelectedIndex = 2
            Me.TabPage3.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 3 Then
            Me.TabPage4.Enabled = True
            Me.TabPage4.Show()
            Me.TabControl1.SelectedIndex = 3
            Me.TabPage4.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 4 Then
            Me.TabPage5.Enabled = True
            Me.TabPage5.Show()
            Me.TabControl1.SelectedIndex = 4
            Me.TabPage5.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 5 Then
            Me.TabPage6.Enabled = True
            Me.TabPage6.Show()
            Me.TabControl1.SelectedIndex = 5
            Me.TabPage6.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 6 Then
            Me.TabPage7.Enabled = True
            Me.TabPage7.Show()
            Me.TabControl1.SelectedIndex = 6
            Me.TabPage7.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 7 Then
            Me.TabPage8.Enabled = True
            Me.TabPage8.Show()
            Me.TabControl1.SelectedIndex = 7
            Me.TabPage8.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage2.Enabled = False
            Me.TabPage9.Enabled = False
        ElseIf Me.ComboBox1.SelectedIndex = 8 Then
            Me.TabPage9.Enabled = True
            Me.TabPage9.Show()
            Me.TabControl1.SelectedIndex = 8
            Me.TabPage9.Select()
            Me.TabPage1.Enabled = False
            Me.TabPage3.Enabled = False
            Me.TabPage4.Enabled = False
            Me.TabPage5.Enabled = False
            Me.TabPage6.Enabled = False
            Me.TabPage7.Enabled = False
            Me.TabPage8.Enabled = False
            Me.TabPage2.Enabled = False
        End If
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Me.Label23.Text = Me.TrackBar1.Value
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        If Me.RadioButton4.Checked = True Then
            Me.NumericUpDown5.Enabled = False
            Me.NumericUpDown6.Enabled = False
            Me.TextBox3.Enabled = True
            Me.Button15.Enabled = True
            Me.Button16.Enabled = True
            On Error Resume Next
            Me.PictureBox1.Image = Image.FromFile(Me.TextBox3.Text)
            'Me.PictureBox1.Enabled = True
        Else
            Me.NumericUpDown5.Enabled = True
            Me.NumericUpDown6.Enabled = True
            Me.TextBox3.Enabled = False
            Me.Button15.Enabled = False
            Me.Button16.Enabled = False
            'Me.PictureBox1.Enabled = False
            Me.PictureBox1.Image = Nothing
        End If
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        IS_SAVED = False
        If Me.RadioButton1.Checked = False And Me.RadioButton2.Checked = False And Me.RadioButton3.Checked = False Then
            MsgBox("请选择鼠标操作模式", 48)
            Exit Sub
        End If
        If Me.RadioButton4.Checked = False And Me.RadioButton5.Checked = False Then
            MsgBox("请选择光标定位模式", 48)
            Exit Sub
        End If
        If Me.TextBox3.Text = "" Then
            MsgBox("当前图片文件位置不可以为空，请选择文件或截取图片，或输入一个有效的图片文件路径。", 48)
            Exit Sub
        End If
        If Me.TextBox7.Text = "" Then
            MsgBox("将要储存于指令集中的文件名不可为空，而且不可含有非ASCII字符。", 48)
            Exit Sub
        End If
        If Not System.IO.File.Exists(Me.TextBox3.Text) Then
            MsgBox("警告：当前图片文件的路径指向的文件不存在。", 48)
            Exit Sub
        End If
        MsgBox("提示：储存于指令集中的文件名中不可包含汉字等非ASCII字符，否则将导致指令集无法执行或异常执行", 48)
        Dim rst As Integer = MsgBox("重要提示：替换图片的操作将导致直接保存指令集，是否继续？", vbYesNo)
        If rst = vbNo Then
            Exit Sub
        End If
        Dim str As String = ""
        If Me.RadioButton1.Checked = True Then
            str = str + "01 "
        ElseIf Me.RadioButton2.Checked = True Then
            str = str + "02 "
        ElseIf Me.RadioButton3.Checked = True Then
            str = str + "03 "
        End If
        str = str + Format(Me.NumericUpDown4.Value, "000") + " "
        If Me.RadioButton4.Checked = True Then
            str = str + "[F]" + Me.TextBox7.Text
            '开始真正替换图片
            If Me.TextBox3.Text = CURRENT_PATH + "\image\" + Me.TextBox7.Text Then
                GoTo SS01
            End If
            On Error Resume Next
            System.IO.File.Delete(CURRENT_PATH + "\image\" + Me.TextBox7.Text)
            On Error Resume Next
            System.IO.File.Copy(Me.TextBox3.Text, CURRENT_PATH + "\image\" + Me.TextBox7.Text)
SS01:
            '保存指令集
            Dim aszv02 As Integer = Me.ListBox1.SelectedIndex
            Me.ListBox1.Items.Item(aszv02) = str
            SaveSet()
        Else
            str = str + "[C]" + CStr(Me.NumericUpDown5.Value) + "," + CStr(Me.NumericUpDown6.Value)
        End If
        Dim aszv As Integer = Me.ListBox1.SelectedIndex
        Me.ListBox1.Items.Item(aszv) = str
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        IS_SAVED = False
        If Me.RadioButton7.Checked = False And Me.RadioButton6.Checked = False Then
            MsgBox("请选择光标定位模式", 48)
            Exit Sub
        End If
        If Me.TextBox2.Text = "" Then
            MsgBox("警告：输入内容不可为空！", 48)
            Exit Sub
        End If
        If Me.TextBox4.Text = "" Then
            MsgBox("当前图片文件位置不可以为空，请选择文件或截取图片，或输入一个有效的图片文件路径。", 48)
            Exit Sub
        End If
        If Me.TextBox8.Text = "" Then
            MsgBox("将要储存于指令集中的文件名不可为空，而且不可含有非ASCII字符。", 48)
            Exit Sub
        End If
        If Not System.IO.File.Exists(Me.TextBox4.Text) Then
            MsgBox("警告：当前图片文件的路径指向的文件不存在。", 48)
            Exit Sub
        End If
        MsgBox("提示：储存于指令集中的文件名中不可包含汉字等非ASCII字符，否则将导致指令集无法执行或异常执行", 48)
        Dim rst As Integer = MsgBox("重要提示：替换图片的操作将导致直接保存指令集，是否继续？", vbYesNo)
        If rst = vbNo Then
            Exit Sub
        End If
        Dim str As String = "04 "
        str = str + Format(Me.NumericUpDown4.Value, "000") + " "
        If Me.RadioButton7.Checked = True Then
            str = str + "[F]" + Me.TextBox8.Text + "[I]" + Me.TextBox2.Text
            '开始真正替换图片
            If Me.TextBox4.Text = CURRENT_PATH + "\image\" + Me.TextBox8.Text Then
                GoTo SS02
            End If
            On Error Resume Next
            System.IO.File.Delete(CURRENT_PATH + "\image\" + Me.TextBox8.Text)
            On Error Resume Next
            System.IO.File.Copy(Me.TextBox4.Text, CURRENT_PATH + "\image\" + Me.TextBox8.Text)
SS02:
            '保存指令集
            Dim aszv02 As Integer = Me.ListBox1.SelectedIndex
            Me.ListBox1.Items.Item(aszv02) = str
            SaveSet()
            Exit Sub
        ElseIf Me.RadioButton6.Checked = True Then
            str = str + "[C]" + CStr(Me.NumericUpDown5.Value) + "," + CStr(Me.NumericUpDown6.Value) + "[I]" + Me.TextBox2.Text
        End If
        Dim aszv As Integer = Me.ListBox1.SelectedIndex
        Me.ListBox1.Items.Item(aszv) = str
    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        Dim stri = "06 " + Format(Me.NumericUpDown4.Value, "000") + " " + CStr(Me.NumericUpDown9.Value) + "," + CStr(Me.NumericUpDown10.Value / 1000)
        Me.ListBox1.Items.Item(a) = stri
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        Dim stri = "05 " + Format(Me.NumericUpDown4.Value, "000") + " " + CStr(Me.NumericUpDown11.Value)
        Me.ListBox1.Items.Item(a) = stri
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        If Not System.IO.File.Exists(Me.TextBox5.Text) Then
            MsgBox("提示：该文件在本地不存在。")
            '不终止过程。
        End If
        Dim stri = "07 " + Format(Me.NumericUpDown4.Value, "000") + " " + Me.TextBox5.Text
        Me.ListBox1.Items.Item(a) = stri

    End Sub

    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        Dim stri As String = "08 001 [P]"
        If Me.RadioButton8.Checked = True Then
            stri = "08 001 [P]"
        ElseIf Me.RadioButton9.Checked = True Then
            stri = "08 001 [Q]"
        ElseIf Me.RadioButton10.Checked = True Then
            stri = "09 001 null"
        End If
        Me.ListBox1.Items.Item(a) = stri
    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        Dim stri As String = "10 001 " + CStr(Me.TrackBar1.Value)
        Me.ListBox1.Items.Item(a) = stri
    End Sub

    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        IS_SAVED = False
        If Me.TextBox6.Text = "" Then
            MsgBox("错误：命令行不可以为空。")
            Exit Sub
        End If
        Dim stri As String
        stri = "12 " + Format(Me.NumericUpDown4.Value, "000") + " " + Me.TextBox6.Text
        Me.ListBox1.Items.Item(Me.ListBox1.SelectedIndex) = stri
    End Sub

    Private Sub RadioButton7_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton7.CheckedChanged
        If Me.RadioButton7.Checked = True Then
            Me.NumericUpDown8.Enabled = False
            Me.NumericUpDown7.Enabled = False
            Me.TextBox4.Enabled = True
            Me.Button18.Enabled = True
            Me.Button17.Enabled = True
            'Me.PictureBox1.Enabled = True
            On Error Resume Next
            Me.PictureBox2.Image = Image.FromFile(Me.TextBox4.Text)
        Else
            Me.NumericUpDown8.Enabled = True
            Me.NumericUpDown7.Enabled = True
            Me.TextBox4.Enabled = False
            Me.Button18.Enabled = False
            Me.Button17.Enabled = False
            'Me.PictureBox1.Enabled = False
            Me.PictureBox2.Image = Nothing
        End If
    End Sub

    Private Sub RadioButton6_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton6.CheckedChanged

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If AUTO_TIME < DateDiff(DateInterval.Second, Now, New Date(1970, 1, 1, 0, 0, 0)) And Me.CheckBox1.Checked = True Then
            Me.Timer1.Enabled = False
            Button14_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Me.OpenFileDialog2.Filter = "JPEG图像|*.jpg|PNG 图像|*.png"
        Me.OpenFileDialog2.ShowDialog()
        Dim picpath As String = Me.OpenFileDialog2.FileName
        If Not System.IO.File.Exists(picpath) Then
            MsgBox("文件不存在！", 48)
            Exit Sub
        End If
        Me.TextBox3.Text = picpath
        On Error Resume Next
        Me.PictureBox1.Image = Image.FromFile(Me.TextBox3.Text)
        If Err.Number > 0 Then
            MsgBox("警告：该文件有可能不受支持。"， 48)
        End If
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Me.OpenFileDialog2.Filter = "JPEG图像|*.jpg|PNG 图像|*.png"
        Me.OpenFileDialog2.ShowDialog()
        Dim picpath As String = Me.OpenFileDialog2.FileName
        If Not System.IO.File.Exists(picpath) Then
            MsgBox("文件不存在！", 48)
            Exit Sub
        End If
        Me.TextBox4.Text = picpath
        On Error Resume Next
        Me.PictureBox2.Image = Image.FromFile(Me.TextBox4.Text)
        If Err.Number > 0 Then
            MsgBox("警告：该文件有可能不受支持。"， 48)
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim num As Integer = ListBox1.SelectedIndex
        If num < Me.ListBox1.Items.Count - 1 Then
            Dim temp As String
            temp = ListBox1.Items.Item(num)
            ListBox1.Items.Item(num) = ListBox1.Items.Item(num + 1)
            ListBox1.Items.Item(num + 1) = temp
            ListBox1.SelectedIndex = num + 1
        End If
    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        Me.OpenFileDialog2.ShowDialog()
        Dim sqwez As String = Me.OpenFileDialog2.FileName
        Me.TextBox5.Text = sqwez
    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        Me.OpenFileDialog2.ShowDialog()
        Dim sqwez As String = Me.OpenFileDialog2.FileName
        Me.TextBox6.Text = Me.TextBox6.Text + " " + Chr(34) + sqwez + Chr(34)
    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        Me.FolderBrowserDialog1.ShowDialog()
        Dim sqwez As String = Me.FolderBrowserDialog1.SelectedPath
        Me.TextBox6.Text = Me.TextBox6.Text + " " + Chr(34) + sqwez + Chr(34)
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Dim num As Integer = Me.ListBox1.SelectedIndex
        If Not num = Me.ListBox1.Items.Count - 1 Then
            Me.ListBox1.SelectedIndex = num + 1
        Else
            On Error Resume Next
            Me.ListBox1.SelectedIndex = num - 1
        End If
        Me.ListBox1.Items.RemoveAt(num)
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Dim qwe As Integer = Me.ListBox1.SelectedIndex
        If qwe >= 0 Then
            Me.ListBox1.Items.Insert(qwe, "")
            Me.ListBox1.SelectedIndex = qwe
        Else
            Me.ListBox1.Items.Add("")
            'Me.ListBox1.SelectedIndex = qwe - 1
        End If
        If Me.ListBox1.Items.Count = 0 Then
            Me.ListBox1.Items.Add("")
            Me.ListBox1.SelectedIndex = 0
        End If
    End Sub

    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        IS_SAVED = False
        Dim a As Integer = Me.ListBox1.SelectedIndex
        Dim stri = "11 " + Format(Me.NumericUpDown4.Value, "000") + " null"
        Me.ListBox1.Items.Item(a) = stri
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form3.Show()
        'form3的代码中已经完成了对新建指令集的初始化。
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Panel1.Enabled = False
        Me.Panel2.Enabled = False
        Me.Button14.Enabled = False
        Me.Button7.Enabled = False
        Me.Button2.Enabled = False
        Me.DateTimePicker1.Value = Now()
        Me.Show()
        Me.NotifyIcon1.BalloonTipTitle = "AutoCommand软件系统提示"
        Me.NotifyIcon1.BalloonTipText = "AutoCommand自动执行助手正在运行。"
        Me.NotifyIcon1.ShowBalloonTip(18)
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Form2.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim q As Integer = MsgBox("您确定现在保存指令集？", vbYesNo)
        If q = vbNo Then
            Exit Sub
        End If
        IS_SAVED = True
        SaveSet()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        '强制停止的原理即杀掉可执行模块的AutoCommand_Executer_Core.exe进程。
        Dim Proname As String = "AutoCommand_Executer_Core.exe"
        For Each pro In Process.GetProcesses
            If pro.ProcessName = Proname Then
                pro.Kill()
            End If
        Next
    End Sub

    Private Sub Label27_Click(sender As Object, e As EventArgs) Handles Label27.Click
        Form5.Show()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Button4_Click(Nothing, Nothing)
    End Sub

    Private Sub 保存SToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 保存SToolStripMenuItem.Click
        If CURRENT_PATH = "" Then
            Exit Sub
        End If
        Button2_Click(Nothing, Nothing)
    End Sub

    Private Sub 检查CToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 检查CToolStripMenuItem.Click
        Button13_Click(Nothing, Nothing)
    End Sub

    Private Sub 打包PToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打包PToolStripMenuItem.Click
        Button32_Click(Nothing, Nothing)
    End Sub

    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        Form4.Show()
    End Sub

    Private Sub 执行EToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 执行EToolStripMenuItem.Click
        Button14_Click(Nothing, Nothing)
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        '------执行指令集------
        If IS_SAVED = False Then
            MsgBox("检测到您没有保存您的修改，请先保存指令集，再执行它。", 48)
            Exit Sub
        End If
        Me.WindowState = FormWindowState.Minimized '执行指令集时将窗体最小化，避免影响识图与点击。
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro2.StartInfo.Arguments = Chr(34) + CURRENT_PATH + Chr(34) + " 0"
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
        Me.WindowState = FormWindowState.Normal '执行完就要恢复窗体。
    End Sub

    Private Sub 关于AToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于AToolStripMenuItem.Click
        Label27_Click(Nothing, Nothing)
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        If Me.TextBox7.Text = "" Then
            MsgBox("存储于指令集中的文件名一项不可以为空。", 48)
            Exit Sub
        End If
        Me.WindowState = FormWindowState.Minimized
        Dim tempf01 As String = Environment.GetEnvironmentVariable("temp") + "\" + Me.TextBox7.Text
        On Error Resume Next
        System.IO.File.Delete(tempf01)
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AC_ScreenShot\AC_ScreenShot.exe"
        pro2.StartInfo.Arguments = Chr(34) + tempf01 + Chr(34) '仅以文件名为命令行参数，默认输出JPEG格式图片。对接截图子模块。
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
        Me.TextBox3.Text = tempf01
        Me.PictureBox1.Image = Image.FromFile(tempf01)
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        If Me.TextBox8.Text = "" Then
            MsgBox("存储于指令集中的文件名一项不可以为空。", 48)
            Exit Sub
        End If
        Me.WindowState = FormWindowState.Minimized
        Dim tempf01 As String = Environment.GetEnvironmentVariable("temp") + "\" + Me.TextBox8.Text
        On Error Resume Next
        System.IO.File.Delete(tempf01)
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AC_ScreenShot\AC_ScreenShot.exe"
        pro2.StartInfo.Arguments = Chr(34) + tempf01 + Chr(34) '仅以文件名为命令行参数，默认输出JPEG格式图片。对接截图子模块。
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
        Me.TextBox4.Text = tempf01
        Me.PictureBox2.Image = Image.FromFile(tempf01)
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Button14_Click(Nothing, Nothing)
    End Sub

    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click
        Me.Hide()
        Me.NotifyIcon1.BalloonTipText = "AutoCommand软件已切换到后台运行。"
        Me.NotifyIcon1.ShowBalloonTip(7)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Me.CheckBox1.Checked = True And CURRENT_PATH = "" Then
            Me.CheckBox1.Checked = False
            MsgBox("当前指令集目录为空，不可启用！", 48)
            Exit Sub
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        If DateDiff(DateInterval.Second, Now, Me.DateTimePicker1.Value) < 0 Then
            MsgBox("时间已过！", 48)
            Me.DateTimePicker1.Value = Now()
            Exit Sub
        End If
        AUTO_TIME = DateDiff(DateInterval.Second, Now, New Date(1970, 1, 1, 0, 0, 0))
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        On Error Resume Next
        System.IO.File.Delete(CURRENT_PATH + "\AC_BRI_IN.tmp")
        Dim file As New System.IO.StreamWriter(System.IO.File.Create(CURRENT_PATH + "\AC_BRI_IN.tmp"), System.Text.Encoding.UTF8)
        Dim str As String = Me.ListBox1.Items.Item(Me.ListBox1.SelectedIndex)
        file.WriteLine(str)
        file.Close()
        System.Threading.Thread.Sleep(500)
        Dim pro As New Process
        pro.StartInfo.FileName = "AutoCommand_Bridge.exe"
        pro.StartInfo.Arguments = Chr(34) + CURRENT_PATH + "\ac_temp.xls" + Chr(34) + " 1"
        pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        'pro.StartInfo.RedirectStandardOutput = True
        pro.StartInfo.UseShellExecute = False
        pro.StartInfo.CreateNoWindow = True
        pro.Start()
        While True
            Application.DoEvents()
            If pro.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(500)

        Me.WindowState = FormWindowState.Minimized '执行指令集时将窗体最小化，避免影响识图与点击。
        Dim pro21 As New Process
        pro21.StartInfo.FileName = Application.StartupPath + "\AutoCommand_Executer_Core\AutoCommand_Executer_Core.exe"
        pro21.StartInfo.Arguments = Chr(34) + CURRENT_PATH + Chr(34) + " 1" '执行一条指令，参数为1。
        pro21.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        pro21.StartInfo.RedirectStandardOutput = True
        pro21.StartInfo.UseShellExecute = False
        pro21.StartInfo.CreateNoWindow = True
        pro21.Start()
        While True
            Application.DoEvents()
            If pro21.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        Dim check_out As String = pro21.StandardOutput.ReadToEnd()
        Me.WindowState = FormWindowState.Normal '执行完就要恢复窗体。
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        On Error Resume Next
        End
        On Error Resume Next
        Dim p As Process = Process.GetProcessesByName("AutoCommand_VocalModel_Core")(0)
        On Error Resume Next
        p.Kill()
        On Error Resume Next
        Application.Exit()
    End Sub

    Private Sub Label12_Click(sender As Object, e As EventArgs) Handles Label12.Click
        Form6.Show()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Button5_Click(Nothing, Nothing)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        '--------------语音识别功能区--------------
        '先删残留的同名临时文件。
        On Error Resume Next
        System.IO.File.Delete(Environment.GetEnvironmentVariable("temp") + "\AutoCommand_Output.tmp")
        On Error Resume Next
        System.IO.File.Delete(Environment.GetEnvironmentVariable("temp") + "\AC_Command.tmp")
        Dim pro2 As New Process
        pro2.StartInfo.FileName = Application.StartupPath + "\AutoCommand_VocalModel_Core\AutoCommand_VocalModel_Core.exe"
        pro2.StartInfo.WorkingDirectory = Application.StartupPath + "\AutoCommand_VocalModel_Core"
        pro2.StartInfo.Arguments = Nothing '语音识别模块的调用不需要参数
        pro2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        'pro2.StartInfo.RedirectStandardOutput = True
        'pro2.StartInfo.UseShellExecute = False
        pro2.StartInfo.CreateNoWindow = True
        pro2.Start()
        While True
            Application.DoEvents()
            If pro2.HasExited = True Then Exit While
            Application.DoEvents()
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(400)
        '将两个文本文件的内容分别储存到STR01,STR02
        Dim STR01 As String = "" 'AC_Command.tmp
        Dim STR02 As String = ""
        If System.IO.File.Exists(Environment.GetEnvironmentVariable("temp") + "\AC_Command.tmp") Then
            Dim f01 As New IO.StreamReader(Environment.GetEnvironmentVariable("temp") + "\AC_Command.tmp")
            STR01 = f01.ReadToEnd()
            f01.Close()
        End If
        If System.IO.File.Exists(Environment.GetEnvironmentVariable("temp") + "\AutoCommand_Output.tmp") Then
            Dim f02 As New IO.StreamReader(Environment.GetEnvironmentVariable("temp") + "\AutoCommand_Output.tmp")
            STR01 = f02.ReadToEnd()
            f02.Close()
        End If
        Dim flag As Boolean = True 'true 代表立即执行。
        If STR01.Contains("立即执行") Or STR02.Contains("立即执行") Then
            flag = True
        Else
            flag = False
        End If
        Dim num01 As Integer = Val(STR01)
        Dim num02 As Integer = Val(STR02)
        Dim sn As Integer = Me.ListBox1.SelectedIndex
        Dim cmdarg As String = ""
        '可用的指令：开机关机，调音量，左右单击双击，输入内容，打开文件，执行当前指令或指令集，滚轮，休眠。
        If STR01.Contains("关机") Or STR01.Contains("关闭计算机") Then
            cmdarg = "08 001 [Q]"
        ElseIf STR01.Contains("调大音量") Then
            cmdarg = "10 001 90"
        ElseIf STR01.Contains("调小音量") Then
            cmdarg = "10 001 10"
        ElseIf STR01.Contains("音量") Or STR01.Contains("声音") Then
            Dim numtemp As Integer
            If (num01 > 100 Or num01 < 0) And (num02 <= 100 Or num02 > 0) Then
                numtemp = num02
            Else
                numtemp = num01
            End If
            cmdarg = "10 001 " + CStr(numtemp)
        ElseIf STR01.Contains("左键单击") Then
            Dim numtemp As Integer = num01
            If numtemp < 0 Or numtemp > 999 Then
                numtemp = 1
            End If
            cmdarg = "01 " + Format(numtemp, "000") + " [C]-1,-1"
        ElseIf STR01.Contains("左键") Then
            Dim numtemp As Integer = num01
            If numtemp < 0 Or numtemp > 999 Then
                numtemp = 1
            End If
            cmdarg = "02 " + Format(numtemp, "000") + " [C]-1,-1"
        ElseIf STR01.Contains("右键") Then
            Dim numtemp As Integer = num01
            If numtemp < 0 Or numtemp > 999 Then
                numtemp = 1
            End If
            cmdarg = "03 " + Format(numtemp, "000") + " [C]-1,-1"
        ElseIf STR01.Contains("输入") Then
            Dim con As String = Microsoft.VisualBasic.Right(STR01, Len(STR01) - 2)
            cmdarg = "04 001 [C]-1,-1[I]" + con
        ElseIf STR01.Contains("打开") Then
            If STR01.Contains("浏览器") Then
                Shell("explorer http:\\www.baidu.com", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("资源管理器") Then
                Shell("explorer.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("记事本") Then
                Shell("notepad.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("画图") Then
                Shell("mspaint.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("截图") Then
                Shell("snippingTool.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("计算器") Then
                Shell("calc.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("命令提示符") Then
                Shell("cmd.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("注册表") Then
                Shell("regedit.exe", AppWinStyle.NormalFocus)
            End If
            If STR01.Contains("任务管理器") Then
                Shell("taskmgr.exe", AppWinStyle.NormalFocus)
            End If
            Exit Sub '执行完就退出
        ElseIf STR01.Contains("执行指令集") Then
            SELENT = True
            SaveSet()
            SELENT = False
            Button14_Click(Nothing, Nothing)
        ElseIf STR01.Contains("执行指令") Then
            SELENT = True
            SaveSet()
            SELENT = False
            Button9_Click(Nothing, Nothing)
        ElseIf STR01.Contains("向上滚动") Then
            cmdarg = "06 001 100,100"
        ElseIf STR01.Contains("滚动") Then
            cmdarg = "06 001 -70,100"
        ElseIf STR01.Contains("休眠") Or STR01.Contains("息屏") Then
            cmdarg = "11 001 null"
        End If
        If cmdarg = "" And (Not STR01 = "") Then
            MsgBox("无效指令")
            Exit Sub
        End If
        Me.ListBox1.Items.Item(sn) = cmdarg
        SELENT = True
        SaveSet()
        SELENT = False
        Button9_Click(Nothing, Nothing)
    End Sub

    Private Sub 后台HToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 后台HToolStripMenuItem.Click
        Button33_Click(Nothing, Nothing)
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Me.Label36.Text = Format(Hour(Now), "00") + ":" + Format(Minute(Now), "00") + ":" + Format(Second(Now), "00")
        Me.Label35.Text = Format(Year(Now), "0000") + "/" + Format(Month(Now), "00") + "/" + Format(Microsoft.VisualBasic.Day(Now), "00")
    End Sub
End Class
