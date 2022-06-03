Public Class Form1
    Dim entities = New LinkedList.LinkedList()
    Public Class Entity
        ' Public variables, can be set and read outside of the Entity class
        Public x As Double, y As Double, health As Object, color As Brush = Brushes.Red, isBot As Boolean = True, radius = 8, velocity = New Vector()
        ' Private variables, cannot be read or set outside of the Entity class
        Dim acceleration = New Vector(), speed = 5, life As Integer = 100 + Int(Rnd() * 50)
        Public Sub New(xPos As Double, yPos As Double, maxHealth As Integer)
            x = xPos
            y = yPos
            health = New Util.Health(maxHealth)
            Form1.entities.push(Me)
        End Sub
        Public Sub setVelocity(x As Double, y As Double)
            velocity.x = x
            velocity.y = y
        End Sub
        Public Sub computeToPlayer()
            life -= 1
            If life <= 0 Then
                If isBot Then
                    WorldWide.playerPoints += 1
                End If
                Form1.entities.removeValue(Me)
            End If
            If TypeOf Form1.player Is Entity Then
                Dim angle As Double = Math.Atan2(Form1.player.y - y, Form1.player.x - x)
                setVelocity(Math.Cos(angle), Math.Sin(angle))
                If Util.circleCollision(Me, Form1.player) Then
                    Dim collisionAngle As Double = Math.Atan2(Form1.player.y - y, Form1.player.x - x)
                    Me.velocity.x -= Math.Cos(collisionAngle) * 10
                    Me.velocity.y -= Math.Sin(collisionAngle) * 10
                    Form1.player.velocity.x += Math.Cos(collisionAngle) * 10
                    Form1.player.velocity.y += Math.Sin(collisionAngle) * 10
                    Form1.player.health.damage(Int(Rnd() * 100))
                End If
            End If
        End Sub
        Public Sub physics()
            If health.isDead() Then
                color = Brushes.DarkRed
            End If
            acceleration.x = Util.lerp(acceleration.x, velocity.x, 0.05)
            acceleration.y = Util.lerp(acceleration.y, velocity.y, 0.05)
            If (x + velocity.x * speed) < 0 Or (x + velocity.x * speed) > Form1.Size.Width Then
                velocity.x *= -1
                acceleration.x *= -2
            End If
            If (y + velocity.y * speed) < 0 Or (y + velocity.y * speed) > Form1.Size.Height Then
                velocity.y *= -1
                acceleration.y *= -2
            End If
            x += acceleration.x * speed
            y += acceleration.y * speed
        End Sub
        Public Sub draw()
            Dim pen As New Pen(color, radius)
            Form1.myGraphics.DrawEllipse(pen, Convert.ToSingle(x - radius / 2), Convert.ToSingle(y - radius / 2), radius * 2, radius * 2)
            pen.Dispose()
        End Sub
        Public Sub drawAsEnemy()
            Dim pen As New Pen(color, radius)
            Util.drawShape(Form1.myGraphics, pen, x, y, 3, radius, velocity.getDirection())
            pen.Dispose()
        End Sub
    End Class
    Dim myGraphics As Graphics = Me.CreateGraphics()
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        myGraphics.FillRectangle(New SolidBrush(Color.FromArgb(25, 0, 0, 0)), 0, 0, Convert.ToSingle(Me.Size.Width), Convert.ToSingle(Me.Size.Height))
        myGraphics.Dispose()
        myGraphics = Me.CreateGraphics()
        Dim angle As Double = Math.Atan2(mouseY - player.y, mouseX - player.x)
        player.setVelocity(Math.Cos(angle), Math.Sin(angle))
        Dim current As Object = entities
        While TypeOf current.after Is LinkedList.Node
            current = current.after
            If current.value.isBot Then
                current.value.computeToPlayer()
                current.value.drawAsEnemy()
            Else
                current.value.draw()
            End If
            current.value.physics()
        End While
        If entities.size < 10 And Rnd() > 0.85 Then
            Randomize()
            Dim ent As New Entity(Rnd() * Me.Size.Width, Rnd() * Me.Size.Height, 100)
            ent.setVelocity(Rnd() * 2 - 1, Rnd() * 2 - 1)
        End If
        WorldWide.playerLevel = Math.Floor(Math.Sqrt(WorldWide.playerPoints))
        myGraphics.DrawString("Score: " & WorldWide.playerPoints, New Font("Ubuntu", 16), Brushes.White, New PointF(10, 10))
        myGraphics.DrawString("Level: " & WorldWide.playerLevel, New Font("Ubuntu", 16), Brushes.White, New PointF(10, 30))
        myGraphics.DrawString("Health: " & player.health.getHealth() * 100 & "%", New Font("Ubuntu", 16), Brushes.White, New PointF(10, 50))
    End Sub
    Dim player As Entity, mouseX As Double, mouseY As Double
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        player = New Entity(250, 250, 1000)
        player.isBot = False
        player.color = Brushes.Green
        'Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        mouseX = e.X
        mouseY = e.Y
    End Sub
End Class
