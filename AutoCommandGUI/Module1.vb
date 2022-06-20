Module Module1
    Public CURRENT_PATH As String '目录路径
    Public SET_TYPE As Boolean 'true代表目录，false代表acs文件。
    Public IS_SAVED As Boolean = True
    Public TEMP_DATA_01 As String '临时公有变量一般不要擅自读写使用。
    Public AUTO_TIME As Long
    Public SELENT As Boolean = False
    Public Sub SaveSet()
        '保存更改到指令集
        'System.IO.File.Create(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_IN.tmp")
        If SET_TYPE = False And SELENT = False Then
            MsgBox("警告：被打包的指令集文件仅支持临时保存，请单击'打包指令集'按钮重新打包。"， 48)
        End If
        '写指令集属性setup.config文件：
        Using sink As New System.IO.StreamWriter(CURRENT_PATH + "\setup.config")
            sink.WriteLine(CStr(Form1.NumericUpDown1.Value / 1000))
            sink.WriteLine(CStr(Form1.NumericUpDown2.Value / 100))
            sink.WriteLine(CStr(Form1.NumericUpDown3.Value / 1000))
        End Using
        Dim file As New System.IO.StreamWriter(System.IO.File.Create(Environment.GetEnvironmentVariable("temp") + "\AC_BRI_IN.tmp"), System.Text.Encoding.UTF8)
        For i = 0 To Form1.ListBox1.Items.Count - 1
            Dim str As String = Form1.ListBox1.Items.Item(i)
            If Not str = "" Then
                file.WriteLine(str)
            End If
        Next i
        file.Close()
        System.Threading.Thread.Sleep(500)
        Dim pro As New Process
        pro.StartInfo.FileName = "AutoCommand_Bridge.exe"
        pro.StartInfo.Arguments = Chr(34) + CURRENT_PATH + "\cmd.xls" + Chr(34) + " 1"
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
        If SELENT = False Then
            MsgBox("AutoCommand系统提示：" + vbCrLf + "已保存")
        End If
    End Sub
End Module
