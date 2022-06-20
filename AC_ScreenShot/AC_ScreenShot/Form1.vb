Imports System.Diagnostics.Process
Public Class FrmMain
    Public pid As Process() '存储所有进程
    Public listProcessIndex As New List(Of Integer) '泛型，存储pid变量进程下标
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Hide()
        Threading.Thread.Sleep(200)
        Dim p1 As New Point(0, 0)
        Dim p2 As New Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim pic As New Bitmap(p2.X, p2.Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(pic)
            g.CopyFromScreen(p1, p1, p2, System.Drawing.CopyPixelOperation.SourceCopy)
        End Using
        frmScreen.PictureBox1.Image = pic
        yuantu = pic
        frmScreen.Show()
        frmScreen.TopLevel = True
    End Sub
    Private Sub FrmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        JTyangshi = 1
        If System.Environment.GetCommandLineArgs.Length <= 1 Then
            End
        End If
        Button2_Click(vbNullString, Nothing)
    End Sub
    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged, RadioButton4.CheckedChanged
        If RadioButton1.Checked Then JTyangshi = 0
        If RadioButton2.Checked Then JTyangshi = 1
        If RadioButton3.Checked Then JTyangshi = 2
        If RadioButton4.Checked Then JTyangshi = 3
    End Sub

    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.DoubleClick
        Dim WindowCaption As String = ""
        Dim ClassName As String = ""
        Dim hWnd As Integer = 0
        If ListBox1.SelectedIndex <> -1 Then
            ClassName = pid(listProcessIndex.Item(ListBox1.SelectedIndex)).ProcessName
            WindowCaption = pid(listProcessIndex.Item(ListBox1.SelectedIndex)).MainWindowTitle
            hWnd = pid(listProcessIndex.Item(ListBox1.SelectedIndex)).MainWindowHandle
            '  MsgBox(ClassName & "|" & WindowCaption & "|" & hWnd.ToString)
            hWnd = pid(listProcessIndex.Item(ListBox1.SelectedIndex)).MainWindowHandle
            ' 如果找不到窗口，则 FindWindow 将窗口句柄设置为 0。如果该
            ' 句柄为 0，则会显示错误信息，否则会将该窗口置于前台。
            If hWnd = 0 Then
                MsgBox("Specified window is not running.", MsgBoxStyle.Exclamation, Me.Text)
            Else
                ' 将窗口设置为前台窗口。
                SetForegroundWindow(hWnd)
                ' 如果窗口是最小化的，则只需将其还原，否则应显示该窗口。注意，
                'IsIconic 的声明将返回值定义为 Boolean，
                ' 从而允许 .NET 将整数值封送到 Boolean。
                If IsIconic(hWnd) Then
                    On Error Resume Next
                    ShowWindow(hWnd, SW_RESTORE)
                Else
                    On Error Resume Next
                    ShowWindow(hWnd, SW_SHOW)
                End If

            End If
            Threading.Thread.Sleep(200)
            GetWndPic(hWnd)
            frmScreen.savetupian() '保存图片
        Else
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        pid = Process.GetProcesses '获得系统所有进程
        ListBox1.Items.Clear() '清零
        listProcessIndex.Clear() '清零
        For i As Integer = 0 To pid.Count - 1 '将有主窗口且窗口标题不为空的进程加进LISTBOX
            If pid(i).MainWindowHandle <> 0 And pid(i).MainWindowTitle <> "" Then
                ListBox1.Items.Add(pid(i).MainWindowTitle + " // 窗口ID  " + pid(i).MainWindowHandle.ToString + " //")
                listProcessIndex.Add(i) '记录pid进程下标
            End If
        Next
    End Sub
End Class
