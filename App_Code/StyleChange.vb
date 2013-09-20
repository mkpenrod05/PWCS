Imports Microsoft.VisualBasic

Public Class StyleChange
    
    Public Shared Function SetColor(ByVal DateData As String) As String

        Dim str As String = ""

        If DateData <> "" Then
            If CDate(DateData) <= Now.AddDays(-365) Then
                str = "lightRed" 'class defined in css/style.css
            ElseIf CDate(DateData) <= Now.AddDays(-305) Then
                str = "lightYellow" 'class defined in css/style.css
            Else
                str = "lightGreen" 'class defined in css/style.css
            End If
        Else
            str = "lightRed" 'class defined in css/style.css
        End If

        Return str

    End Function

End Class
