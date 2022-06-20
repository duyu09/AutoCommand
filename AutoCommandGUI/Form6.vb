Public Class Form6
    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        On Error GoTo errh
        Dim file As New System.IO.StreamReader(Application.StartupPath + "\AC_History.log", System.Text.Encoding.UTF8)
        Dim strhi As String = file.ReadToEnd()
        Me.TextBox1.Text = strhi
        Me.TextBox1.ReadOnly = True
errh:
    End Sub
End Class