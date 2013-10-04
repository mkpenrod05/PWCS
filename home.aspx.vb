
Partial Class home
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'If UserValidation.PageAccess(HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower()) = False Then
        '    Response.Write("Access Denied! - " & HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower())
        '    Response.End
        'End If

        If Request.QueryString("AccountCode") <> "" Then
            Dim AccountCode As String = Request.QueryString("AccountCode")
            Dim Account As New ClassModels.Account(AccountCode)

            TrunkIdAndSerialNumberDiv.InnerHtml = WebServiceFunctions.TrunkIdAndSerialNumberDisplay(Account.AccountCode)
            TrunkIdAndSerialNumberContainer.Style.Remove("display")

            'AccountHeading.InnerHtml = Account.AccountCode & " - " & Account.Organization

            AccountCodePlaceHolder.InnerText = Account.AccountCode
            AccountOrgPlaceHolder.InnerHtml = "<span id='Organization_" & Account.ID & "'>" & Account.Organization & "</span>"

            ManagersInformationPlaceHolder.InnerHtml = WebServiceFunctions.ManagerInformationDisplay(AccountCode)
            AccountInfoPlaceHolder.InnerHtml = WebServiceFunctions.AnnualRequirementsDisplay(Account)
            AccountCommentsPlaceHolder.InnerHtml = WebServiceFunctions.AccountCommentsDisplay(1, 5, Account.ID)

        End If

    End Sub

End Class
