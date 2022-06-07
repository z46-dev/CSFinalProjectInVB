Module LinkedList
    Public Class Node
        Public value As Object, after As Object, parent As Object
        Public Sub New(val As Object, par As Object)
            value = val
            parent = par
        End Sub
    End Class
    Public Class LinkedList
        Public after As Node, last As Node, size As Integer
        Public Sub New()
            ' Do nothing
        End Sub
        Public Property length = size
        Public Function at(index As Integer)
            If index >= size Or index < 0 Then
                Throw New ArgumentOutOfRangeException()
                Return 0
            End If
            Dim current As Object = Me
            For i As Integer = 0 To index
                current = current.after
            Next
            Return current.value
        End Function
        Public Sub push(value)
            Dim current As Object = Me
            While TypeOf current.after Is Node
                current = current.after
            End While
            current.after = New Node(value, current)
            last = current.after
            size += 1
        End Sub
        Public Sub insertAt(index As Integer, value As Object)
            If index >= size Or index < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            Dim newNode As New Node(value, New Object), current As Object = Me, previous As Object = Me
            For i As Integer = 0 To index
                previous = current
                current = current.after
            Next
            newNode.parent = previous
            previous.after = newNode
            newNode.after = current
            size += 1
        End Sub
        Public Function removeAt(index As Integer)
            If index >= size Or index < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If index = 0 Then
                Return shift()
            ElseIf index = size - 1 Then
                Return pop()
            Else
                Dim current As Object = Me, previous As Object = Me
                For i As Integer = 0 To index
                    previous = current
                    current = current.after
                Next
                previous.after = current.after
                size -= 1
                Return 0
            End If
        End Function
        Public Function shift()
            If TypeOf after Is Node Then
                Dim returnValue = after.value
                If TypeOf after Is Node Then
                    after = after.after
                Else
                    after = New Object()
                End If
                size -= 1
                Return returnValue
            End If
            Return 0
        End Function
        Public Function pop()
            If TypeOf last Is Node Then
                Dim returnValue = last.value
                Dim current = after
                While current.after IsNot last
                    current = current.after
                End While
                current.after = vbNull
                last = current
                size -= 1
                Return returnValue
            End If
            Return 0
        End Function
        Public Function indexOf(value As Object)
            Dim index As Integer = 0, current As Object = Me
            While index < size
                index += 1
                current = current.after
                If current.value Is value Then
                    Return index - 1
                End If
            End While
            Return -1
        End Function
        Public Sub removeValue(value As Object)
            Dim index As Integer = indexOf(value)
            If index > -1 Then
                removeAt(index)
            End If
        End Sub
        Public Sub print()
            Dim current As Object = Me
            While TypeOf current.after Is Node
                current = current.after
                Console.WriteLine(current.value)
            End While
        End Sub
        Public Function toArray()
            Dim list(size - 1) As Object, current As Object = Me
            For i As Integer = 0 To size - 1
                current = current.after
                list(i) = current.value
            Next
            Return list
        End Function
        Public Sub writeToListBox(listBox As ListBox)
            listBox.Items.Clear()
            Dim current As Object = Me
            For i As Integer = 0 To size - 1
                current = current.after
                listBox.Items.Add(i & " - " & current.value)
            Next
        End Sub
    End Class
End Module
