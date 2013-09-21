Imports Microsoft.VisualBasic

Public Class UserValidation

    Public Shared Function PageAccess(ByVal user As String) As String

        'Dim test As Integer = WebcodeHelp.UserValidation.PageAccess(user, "PWCS")

        Dim UserClean As String = user.Trim().ToLower()
        Dim Authenticated As Boolean

        If UserClean = "hill-2k\michael.penrod" Or
            UserClean = "hill-2k\andrew.duncan" Or
            UserClean = "hill-2k\stanley.holmes" Or
            UserClean = "hill-2k\amber.urbano" Or
            UserClean = "hill-2k\joshua.dager" Or
            UserClean = "hill-2k\jodi.livesay" Then

            Authenticated = True
        Else
            Authenticated = False
        End If

        Return Authenticated

    End Function

    Public Shared Function AccessedByUser() As String

        Dim User As String = HttpContext.Current.Request.ServerVariables("AUTH_USER")
        Dim Server As String = HttpContext.Current.Request.ServerVariables("SERVER_NAME")

        If User.Length < 1 Then
            If (Server.Contains("localhost")) Then
                User = ConfigurationManager.AppSettings("Admin")
            Else
                User = ConfigurationManager.AppSettings("Guest")
            End If
        End If

        Return User

    End Function

End Class
