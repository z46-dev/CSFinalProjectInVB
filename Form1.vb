Public Class Form1
    ' This linked list will hold all players and enemies in this game
    Dim entities = New LinkedList.LinkedList()
    ' The entity class, Entities are players, both human and AI in this game
    Public Class Entity
        ' Public variables, can be set and read outside of the Entity class
        Public x As Double, y As Double, health As Object, color As Brush = Brushes.Red, isBot As Boolean = True, radius = 8, velocity = New Vector(), acceleration = New Vector(), speed = 5, life As Integer = 100 + Int(Rnd() * 50)
        ' The main constructor, initializes the entity and pushes it to the list
        Public Sub New(xPos As Double, yPos As Double, maxHealth As Integer)
            x = xPos
            y = yPos
            health = New Util.Health(maxHealth)
            Form1.entities.push(Me)
        End Sub
        ' This is basically the sub procedure that the bots use in order to think about the player and do collision with it
        Public Sub computeToPlayer()
            ' Remove the bot if it's too old
            life -= 1
            If life <= 0 Then
                WorldWide.playerPoints += 1
                Form1.entities.removeValue(Me)
            End If
            ' If the player exists
            If TypeOf Form1.player Is Entity Then
                ' Move to it
                Dim angle As Double = Math.Atan2(Form1.player.y - y, Form1.player.x - x)
                velocity.x = Math.Cos(angle)
                velocity.y = Math.Sin(angle)
                ' If we're too close
                If Util.circleCollision(Me, Form1.player) Then
                    ' Bounce
                    Dim collisionAngle As Double = Math.Atan2(Form1.player.y - y, Form1.player.x - x)
                    Me.velocity.x -= Math.Cos(collisionAngle) * 10
                    Me.velocity.y -= Math.Sin(collisionAngle) * 10
                    Form1.player.velocity.x += Math.Cos(collisionAngle) * 10
                    Form1.player.velocity.y += Math.Sin(collisionAngle) * 10
                    ' Hurt the player
                    Form1.player.health.damage(Int(Rnd() * 100))
                End If
            End If
        End Sub
        ' The main physics of all entities
        Public Sub physics()
            ' If we're dead, make us red!
            If health.isDead() Then
                color = Brushes.DarkRed
            End If
            ' Smoothly accelerate to the desired velocity
            acceleration.x = Util.lerp(acceleration.x, velocity.x, 0.05)
            acceleration.y = Util.lerp(acceleration.y, velocity.y, 0.05)
            ' Bounce off the edges of the form
            If (x + velocity.x * speed) < 0 Or (x + velocity.x * speed) > Form1.Size.Width Then
                velocity.x *= -1
                acceleration.x *= -2
            End If
            If (y + velocity.y * speed) < 0 Or (y + velocity.y * speed) > Form1.Size.Height Then
                velocity.y *= -1
                acceleration.y *= -2
            End If
            ' Move
            x += acceleration.x * speed
            y += acceleration.y * speed
        End Sub
        ' Draw me (but only for players)
        Public Sub draw()
            Dim pen As New Pen(color, radius)
            Form1.myGraphics.DrawEllipse(pen, Convert.ToSingle(x - radius / 2), Convert.ToSingle(y - radius / 2), radius * 2, radius * 2)
            pen.Dispose()
        End Sub
        ' Draw me (but only for enemies/bots)
        Public Sub drawAsEnemy()
            Dim pen As New Pen(color, radius)
            Util.drawShape(Form1.myGraphics, pen, x, y, 3, radius, velocity.getDirection())
            pen.Dispose()
        End Sub
    End Class
    ' The main graphics of the game, gets used, disposed and reset each frame
    Dim myGraphics As Graphics = Me.CreateGraphics()
    ' Draw this once we're dead lol
    Private Sub drawDeathScreen()
        myGraphics.FillRectangle(New SolidBrush(Color.FromArgb(255, 50, 50, 50)), 0, 0, Convert.ToSingle(Me.Size.Width), Convert.ToSingle(Me.Size.Height))
        Dim stringFormat = New StringFormat()
        stringFormat.Alignment = StringAlignment.Center
        stringFormat.LineAlignment = StringAlignment.Center
        myGraphics.DrawString("Final Score: " & WorldWide.playerPoints, New Font("Ubuntu", 24), Brushes.White, New PointF(Me.Size.Width / 2, Me.Size.Height / 3), stringFormat)
        myGraphics.DrawString("Final Level: " & WorldWide.playerLevel, New Font("Ubuntu", 24), Brushes.White, New PointF(Me.Size.Width / 2, Me.Size.Height / 3 + 50), stringFormat)
        myGraphics.DrawString("Restart the application to play again!", New Font("Ubuntu", 14), Brushes.White, New PointF(Me.Size.Width / 2, Me.Size.Height / 3 + 100), stringFormat)
    End Sub
    ' Game loop
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' If we're dead, transition to the death screen
        If player.health.isDead() Then
            Me.Timer1.Enabled = False
            Me.Timer1.Stop()
            Me.drawDeathScreen()
            Return
        End If
        ' Reset graphics + add background
        myGraphics.FillRectangle(New SolidBrush(Color.FromArgb(25, 0, 0, 0)), 0, 0, Convert.ToSingle(Me.Size.Width), Convert.ToSingle(Me.Size.Height))
        myGraphics.Dispose()
        myGraphics = Me.CreateGraphics()
        ' Move player to mouse
        Dim angle As Double = Math.Atan2(mouseY - player.y, mouseX - player.x)
        player.velocity.x = Math.Cos(angle)
        player.velocity.y = Math.Sin(angle)
        ' Update every entity
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
        ' If we need more enemies and we just feel like it, add them
        If entities.size < 10 And Rnd() > 0.95 Then
            Randomize()
            Dim ent As New Entity(Rnd() * Me.Size.Width, Rnd() * Me.Size.Height, 100)
            ent.velocity.x = Math.Cos(angle)
            ent.velocity.y = Math.Sin(angle)
        End If
        ' Update the level (more points needed for each level)
        WorldWide.playerLevel = Math.Floor(Math.Sqrt(WorldWide.playerPoints))
        ' if it's an odd level, give us a powerup
        If WorldWide.playerLevel > WorldWide.oldPlayerLevel And WorldWide.playerLevel Mod 2 Then
            Select Case Math.Floor(Rnd() * 3)
                Case Is = 0
                    player.speed *= 1.25
                    WorldWide.addMessage("Your speed has been increased by 25%!")
                    WorldWide.addPowerup(0, Math.Round(120 * Rnd() * 3))
                Case Is = 1
                    player.radius *= 0.75
                    WorldWide.addMessage("Your size has been decreased by 25%!")
                    WorldWide.addPowerup(1, Math.Round(120 * Rnd() * 3))
                Case Is = 2
                    player.health.heal()
                    WorldWide.addMessage("Your health has been restored!")
            End Select
        End If
        ' Remove old powerups
        Dim powerUpsDone As LinkedList.LinkedList = WorldWide.checkPowerups()
        current = powerUpsDone
        While TypeOf current.after Is LinkedList.Node
            current = current.after
            Select Case current.value
                Case Is = 0
                    player.speed /= 1.25
                    WorldWide.addMessage("Your speed buff has worn off!")
                Case Is = 1
                    player.radius /= 0.75
                    WorldWide.addMessage("Your size buff has worn off!")
            End Select
        End While
        WorldWide.oldPlayerLevel = WorldWide.playerLevel
        ' Display basic monitors
        myGraphics.DrawString("Score: " & WorldWide.playerPoints, New Font("Ubuntu", 16), Brushes.White, New PointF(10, 10))
        myGraphics.DrawString("Level: " & WorldWide.playerLevel, New Font("Ubuntu", 16), Brushes.White, New PointF(10, 30))
        myGraphics.DrawString("Health: " & Math.Round(player.health.getHealth() * 100) & "%", New Font("Ubuntu", 16), Brushes.White, New PointF(10, 50))
        WorldWide.drawMessages(myGraphics)
    End Sub
    Dim player As Entity, mouseX As Double, mouseY As Double
    ' Create the player
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        player = New Entity(Me.Size.Width / 2, Me.Size.Height / 2, 1000)
        player.isBot = False
        player.color = Brushes.Green
        'Me.WindowState = FormWindowState.Maximized
    End Sub
    ' Save mouse position so player can move to it
    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        mouseX = e.X
        mouseY = e.Y
    End Sub
End Class
