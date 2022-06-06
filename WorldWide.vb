Module WorldWide
    Public playerPoints As Integer
    Public playerLevel As Integer
    Public oldPlayerLevel As Integer
    Dim messages As New LinkedList.LinkedList()
    Class Message
        Public content As String, time As Integer

        Public Sub New(stuff As String)
            content = stuff
        End Sub
    End Class
    Public Sub drawMessages(myGraphics As Graphics)
        Dim x As Integer = Math.Round(Form1.Size.Width / 2), y As Integer = 20
        Dim current As Object = messages
        While TypeOf current.after Is LinkedList.Node
            current = current.after
            If current.value.time > 100 Then
                messages.removeValue(current.value)
                Continue While
            End If
            current.value.time += 1
            myGraphics.DrawString(current.value.content, New Font("Ubuntu", 15), Brushes.White, New PointF(x, y))
            y += 20
        End While
    End Sub
    Public Sub addMessage(message As String)
        messages.push(New Message(message))
    End Sub
End Module
