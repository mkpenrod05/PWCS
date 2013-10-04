Imports Microsoft.VisualBasic

Public Class StyleChange
    
    Public Shared Function SetColor(ByVal DateData As String) As String
        Dim str As String = ""
        Dim NewDate As Date
        Dim Pass As Boolean = Date.TryParse(DateData, NewDate)

        If Pass = True Then
            If NewDate <= Now.AddDays(-365) Then
                str = "lightRed" 'class defined in css/style.css
            ElseIf NewDate <= Now.AddDays(-305) Then
                str = "lightYellow" 'class defined in css/style.css
            Else
                str = "lightGreen" 'class defined in css/style.css
            End If
        Else
            str = "lightRed"
        End If

        Return str

    End Function

End Class
