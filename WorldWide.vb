Module WorldWide
    ' Just some stuff, used as semi-global variables for player statistics
    Public playerPoints As Integer
    Public playerLevel As Integer
    Public oldPlayerLevel As Integer
    ' Messages list
    Dim messages As New LinkedList.LinkedList()
    ' Simply a holder object for messages
    Class Message
        Public content As String, time As Integer
        Public Sub New(stuff As String)
            content = stuff
        End Sub
    End Class
    ' Render every message, and remove those that are old
    ' Precondition: myGraphics must be an instance of the Graphics object
    Public Sub drawMessages(myGraphics As Graphics)
        Dim x As Integer = 10, y As Integer = 70
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
    ' Add a new message
    ' Precondition: message must be a string you wish to display on the screen
    Public Sub addMessage(message As String)
        messages.push(New Message(message))
    End Sub
    ' List of powerups that are active
    Public powerups As New LinkedList.LinkedList()
    ' Again, a holder for a powerup object
    Class PowerupHolder
        Public type As Integer, time As Integer
        Public Sub New(t1, t2)
            type = t1
            time = t2
        End Sub
    End Class
    ' Add a new powerup
    ' Precondition: type is the powerup type as an integer ID. time is the time (in game ticks) that the powerup will be active for.
    Public Sub addPowerup(type As Integer, time As Integer)
        powerups.push(New PowerupHolder(type, time))
    End Sub
    ' This checks to see if powerups are too old, and if they are, remove them and deactivate them.
    ' Postcondition: Will return a LinkedList of type IDs for every powerup that has no time left.
    Public Function checkPowerups() As LinkedList.LinkedList
        Dim output = New LinkedList.LinkedList()
        Dim current As Object = powerups
        While TypeOf current.after Is LinkedList.Node
            current = current.after
            If current.value.time <= 0 Then
                output.push(current.value.type)
                powerups.removeValue(current.value)
                Continue While
            End If
            current.value.time -= 1
        End While
        Return output
    End Function
End Module
