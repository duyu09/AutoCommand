Module JTModule
    Structure POINTAPI
        Dim x As Long
        Dim y As Long
    End Structure
    Public JTyangshi As Integer = 0 '截图样式
    Public yuantu As Image '原图，用于存图片
    Public baocun As Boolean = False '是否保存，初始不保存

    Public Declare Function ShowWindow Lib "user32" (ByVal hwnd As Long, ByVal nCmdShow As Long) As Long
    Public Const SW_RESTORE = 9
    Public Const SW_SHOW = 5
    Public Declare Function IsIconic Lib "user32.dll" Alias "IsIconic" (ByVal hwnd As Integer) As Boolean
    Public Declare Function SetForegroundWindow Lib "user32.dll" Alias "SetForegroundWindow" (ByVal hwnd As Integer) As Integer
    Public Declare Function GetParent Lib "user32" (ByVal hwnd As IntPtr) As Integer

    '获取鼠标指针的当前位置
    Public Declare Function GetCursorPos Lib "user32" Alias "GetCursorPos" (ByRef lpPoint As POINTAPI) As Integer

    '返回包含了指定点的窗口的句柄。忽略屏蔽、隐藏以及透明窗口
    Public Declare Function WindowFromPoint Lib "user32" Alias "WindowFromPoint" (ByVal xPoint As Integer, ByVal yPoint As Integer) As IntPtr

    '为指定的窗口取得类名
    Public Declare Function GetClassName Lib "user32" Alias "GetClassNameA" (ByVal hwnd As IntPtr, ByVal lpClassName As String, ByVal nMaxCount As Integer) As Long
    '取得一个窗体的标题（caption）文字，或者一个控件的内容（在vb里使用：使用vb窗体或控件的caption或text属性）
    Public Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As IntPtr, ByVal lpString As String, ByVal cch As Integer) As Integer

    Private Declare Function GetWindowRect Lib "user32" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer

    Public Sub GetWndPic(ByVal Wnd As IntPtr) '根据窗口句柄将窗口截图到PIC
        Dim R As RECT, DC As IntPtr
        Dim mSize As Size
        ' BringWindowToTop(Wnd) '目标窗口提到前面(非置顶)
        '  ShowWindow(Wnd, 1)
        '  SetWindowPos(Wnd, HWND_TOPMOST, 0&, 0&, 0&, 0&, &H1 Or &H2)
        ' DC = GetWindowDC(Wnd) '得到dc
        GetWindowRect(Wnd, R) '获取指定窗口bai的左上角du、右下角位置(以便获取其大小)
        mSize = New Size(R.Right - R.Left, R.Bottom - R.Top) '定义大小
        '  Dim a As Integer = BitBlt(, 0, 0, mSize.Width, mSize.Height, DC, 0, 0, SRCCOPY) '复制绘图
        Dim bmp As Image = New Bitmap(mSize.Width, mSize.Height, Imaging.PixelFormat.Format32bppArgb)
        Dim g As Graphics = Graphics.FromImage(bmp)
        Threading.Thread.Sleep(200)
        g.CopyFromScreen(R.Left, R.Top, 0, 0, mSize, Drawing.CopyPixelOperation.SourceCopy)
        My.Computer.Clipboard.SetImage(bmp) '存到剪贴板
        g.Dispose()
        ' ReleaseDC(Wnd, DC) '释放
    End Sub
    Public Function getWindowsHDC() As IntPtr '得到窗口句柄
        Dim msg As String = ""
        Dim shubiao As POINTAPI
        Dim Leiming As String, Biaoti As String
        Leiming = Space(255)
        Biaoti = Space(255)
        GetCursorPos(shubiao)
        Dim mm As Point = System.Windows.Forms.Cursor.Position
        msg = "坐标：" + shubiao.x.ToString() & " , " & shubiao.y.ToString() + vbCrLf
        Dim hdc As IntPtr = WindowFromPoint(shubiao.x, shubiao.y)
        msg += "句柄：" + hdc.ToString() + vbCrLf
        msg += "父窗体句柄为" + GetParent(hdc).ToString + vbCrLf
        GetWindowText(hdc, Biaoti, 255)
        msg += "标题：" + Biaoti + vbCrLf
        GetClassName(hdc, Leiming, 255)
        msg += "类名称：" + Leiming + vbCrLf
        MsgBox(msg)
        Return hdc
    End Function
    Public Structure RECT
        Dim Left As Integer
        Dim Top As Integer
        Dim Right As Integer
        Dim Bottom As Integer
    End Structure


End Module
