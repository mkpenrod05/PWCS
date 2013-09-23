Imports Microsoft.VisualBasic

Public Class StyleChange
    
    Public Shared Function SetColor(ByVal DateData As Date) As String

        Dim str As String = ""

        If DateData <= Now.AddDays(-365) Then
            str = "lightRed" 'class defined in css/style.css
        ElseIf DateData <= Now.AddDays(-305) Then
            str = "lightYellow" 'class defined in css/style.css
        Else
            str = "lightGreen" 'class defined in css/style.css
        End If
        
        Return str

    End Function

End Class
