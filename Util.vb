' Utility functions and classes
Module Util
    ' Lerp, also known as Linear Interpolation
    ' Smoothly interpolates v0 (start) to v1 (end) at the 'speed' of t (time)
    ' Algorithm from https://github.com/mattdesl/lerp/blob/master/index.js
    Public Function lerp(v0 As Double, v1 As Double, t As Double) As Double
        Return v0 * (1 - t) + v1 * t
    End Function

    ' Circle Collision
    ' Checks if two circles (with the properties x, y and radius) are touching eachother
    Public Function circleCollision(p1 As Object, p2 As Object) As Boolean
        Return Math.Sqrt((p2.x - p1.x) ^ 2 + (p2.y - p1.y) ^ 2) < p1.radius + p2.radius
    End Function

    ' Health class
    ' Stores the health of an entity
    Public Class Health
        Dim max As Integer, amount As Integer
        Public Sub New(health As Integer)
            max = health
            amount = health
        End Sub
        ' Deals damage to the amount
        Public Sub damage(dmg As Integer)
            amount -= dmg
        End Sub
        Public Sub heal()
            amount = max
        End Sub
        ' Tells you if you have no health left
        Public Function isDead() As Boolean
            Return amount <= 0
        End Function
        Public Function getHealth() As Double
            Return amount / max
        End Function
    End Class

    ' Holder for the x and y of a vector, can also be added to have more methods soon
    Public Class Vector
        Public x As Double, y As Double
        Public Sub New()
            ' Do nothing
        End Sub
        ' Return direction of my X and Y movement
        Public Function getDirection()
            Return Math.Atan2(y, x)
        End Function
    End Class
    ' Draw a shape at X, Y with some sides and a radius facing any which way
    Public Sub drawShape(graphics As Graphics, pen As Pen, x As Single, y As Single, sides As Integer, radius As Single, angle As Single)
        Dim state = graphics.Save(), points(sides + 1) As PointF
        For i As Integer = 0 To sides + 1
            Dim pointAngle = Math.PI * 2 / sides * i + angle
            points(i) = New PointF(x + Math.Cos(pointAngle) * radius, y + Math.Sin(pointAngle) * radius)
        Next
        graphics.DrawLines(pen, points)
        graphics.Restore(state)
    End Sub
End Module
