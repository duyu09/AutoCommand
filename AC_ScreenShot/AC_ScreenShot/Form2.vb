Public Class frmScreen
    Private p1 As Point, p2 As Point
    Private penPath As New Drawing2D.GraphicsPath
    Private mye As Drawing.Graphics
    Private Sub frmScreen_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        Select Case JTyangshi
            Case 0 '全屏截图
                Me.Cursor = Cursors.Default
            Case 1 '矩形截图
                Me.Cursor = Cursors.Cross
            Case 2 '任意形状截图
                Me.Cursor = Cursors.Cross
            Case 3 '选择窗口
                Me.Cursor = Cursors.Hand
        End Select
    End Sub
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Select Case JTyangshi
                Case 0 '全屏截图
                Case 1 '矩形截图
                    p1 = New Point(e.X, e.Y)
                Case 2 '任意形状截图
                    p1 = New Point(e.X, e.Y)
                    penPath.Reset()
                    penPath.AddRectangle(New Rectangle(p1, New Size(1, 1)))
                Case 3 '选择窗口
                    Me.Hide()
                    Threading.Thread.Sleep(200)
                    '
                    Dim myhandle As IntPtr = getWindowsHDC()
                    GetWndPic(myhandle)
                    Me.Show()
            End Select
        End If
    End Sub
    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Select Case JTyangshi
                Case 0 '全屏截图
                Case 1 '矩形截图
                    p2 = New Point(e.X, e.Y)
                    drawJUXING()
                Case 2 '任意形状截图
                    p2 = New Point(e.X, e.Y)
                    penPath.AddLine(p1, p2)
                    p1 = p2
                    Dim pic As New Bitmap(PictureBox1.Width, PictureBox1.Height)
                    mye = Graphics.FromImage(pic)
                    mye.DrawImage(yuantu, 0, 0)
                    mye.DrawPath(Pens.Blue, penPath)
                    PictureBox1.Image = pic
                    mye.Dispose()
                    PictureBox1.Refresh()
                Case 3 '选择窗口
            End Select
        End If
    End Sub
    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Select Case JTyangshi
                Case 0 '全屏截图
                Case 1 '矩形截图
                    p2 = New Point(e.X, e.Y)
                    drawJUXING()
                    Dim jx As Rectangle = getjuxing(p1, p2)
                    Dim pic As New Bitmap(jx.Width, jx.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    mye = Graphics.FromImage(pic)
                    mye.DrawImage(yuantu, 0, 0, jx, GraphicsUnit.Pixel)
                    My.Computer.Clipboard.SetImage(pic)
                    mye.Dispose()
                    pic.Dispose()
                Case 2 '任意形状截图
                    penPath.CloseFigure()
                    Dim pic As New Bitmap(PictureBox1.Width, PictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    mye = Graphics.FromImage(pic)
                    mye.DrawImage(yuantu, 0, 0)
                    mye.DrawPath(Pens.Blue, penPath)
                    PictureBox1.Image = pic
                    mye.Dispose()
                    PictureBox1.Refresh()
                    Dim picJQB As New Bitmap(PictureBox1.Width, PictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    Dim eJQB As Graphics = Graphics.FromImage(picJQB)
                    eJQB.Clear(Color.White)
                    eJQB.FillPath(New TextureBrush(yuantu), penPath)
                    Dim picj As New Bitmap(CInt(penPath.GetBounds.Width), CInt(penPath.GetBounds.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    Dim ej As Graphics = Graphics.FromImage(picj)
                    ej.DrawImage(picJQB, 0, 0, New Rectangle(penPath.GetBounds.Left, penPath.GetBounds.Top, CInt(penPath.GetBounds.Width), CInt(penPath.GetBounds.Height)), GraphicsUnit.Pixel)
                    My.Computer.Clipboard.SetImage(picj)
                    eJQB.Dispose()
                    ej.Dispose()
                    picJQB.Dispose()
                    picj.Dispose()
                Case 3 '选择窗口

            End Select
        End If
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Select Case JTyangshi
                Case 0 '全屏截图
                    My.Computer.Clipboard.SetImage(yuantu)
                Case 1 '矩形截图
                Case 2 '任意形状截图
                Case 3 '选择窗口
            End Select
            savetupian()
            Me.Hide()
            FrmMain.Show()
        End If
    End Sub
    Private Sub drawJUXING()
        Dim pic As New Bitmap(PictureBox1.Width, PictureBox1.Height)
        mye = Graphics.FromImage(pic)
        mye.DrawImage(yuantu, 0, 0)
        mye.DrawRectangle(Pens.Red, getjuxing(p1, p2))
        PictureBox1.Image = pic
        mye.Dispose()
        PictureBox1.Refresh()
        Threading.Thread.Sleep(20)
    End Sub
    Private Function getjuxing(ByVal jp1 As Point, ByVal jp2 As Point) As Rectangle
        If jp1.X = jp2.X And jp1.Y = jp2.Y Then
            jp2.X = jp1.X + 1
            jp2.Y = jp1.Y + 1
        End If
        Dim jx1 As Point = New Point(Math.Min(jp1.X, jp2.X), Math.Min(jp1.Y, jp2.Y))
        Return New Rectangle(jx1.X, jx1.Y, Math.Abs(jp1.X - jp2.X), Math.Abs(jp1.Y - jp2.Y))
    End Function


    'Public Sub savetupian()
    '    Dim opic As Bitmap = My.Computer.Clipboard.GetImage
    '    My.Computer.Clipboard.Clear()
    '    If IsNothing(opic) Then
    '    Else
    '        Dim sdlag As New Windows.Forms.SaveFileDialog
    '        sdlag.Filter = "bmp文件|*.bmp|gif文件|*.gif|jpeg文件|*.jpeg|png文件|*.png"
    '        sdlag.InitialDirectory = My.Application.Info.DirectoryPath

    '        If sdlag.ShowDialog() = Windows.Forms.DialogResult.OK Then
    '            If sdlag.FileName <> "" Then
    '                Select Case sdlag.FilterIndex
    '                    Case 1
    '                        opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Bmp)
    '                    Case 2
    '                        opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Gif)
    '                    Case 3
    '                        opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Jpeg)
    '                    Case 4
    '                        opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Png)
    '                    Case 5
    '                        opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Tiff)
    '                    Case Else
    '                End Select
    '            End If
    '        Else

    '        End If

    '    End If
    'End Sub
    Public Sub savetupian()
        Dim opic As Bitmap = My.Computer.Clipboard.GetImage
        My.Computer.Clipboard.Clear()
        If IsNothing(opic) Then
        Else
            Dim args() As String = System.Environment.GetCommandLineArgs()
            Dim savePath As String
            savePath = args(1)
            If args.Length = 3 Then
                If "PNG" = args(2).ToUpper() Then
                    opic.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)
                ElseIf "JPG" = args(2).ToUpper() Then
                    opic.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg)
                End If
            ElseIf args.Length = 2 Then
                If savePath <> "" Then
                    opic.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg)
                End If
            ElseIf args.Length = 4 Then
                If args(3) = "showDialog" And savePath <> "" Then
                    My.Computer.Clipboard.Clear()
                    Dim sdlag As New Windows.Forms.SaveFileDialog
                    sdlag.Filter = "bmp文件|*.bmp|gif文件|*.gif|jpeg文件|*.jpeg|png文件|*.png"
                    sdlag.InitialDirectory = My.Application.Info.DirectoryPath
                    If sdlag.ShowDialog() = Windows.Forms.DialogResult.OK Then
                        If sdlag.FileName <> "" Then
                            Select Case sdlag.FilterIndex
                                Case 1
                                    opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Bmp)
                                Case 2
                                    opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Gif)
                                Case 3
                                    opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Jpeg)
                                Case 4
                                    opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Png)
                                Case 5
                                    opic.Save(sdlag.FileName, System.Drawing.Imaging.ImageFormat.Tiff)
                                Case Else
                            End Select
                        End If
                    End If
                End If
            End If
        End If
    End Sub

End Class