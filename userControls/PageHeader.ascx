<%@ Control Language="VB" ClassName="PageHeader" %>

<script runat="server">
    Protected Sub Page_Header(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        
        Dim Server As String = HttpContext.Current.Request.ServerVariables("SERVER_NAME")
        Dim TextStyle = ""
        
        If (Server.Contains("wbhill03")) Then
            'UserInformationText.InnerText = CustomFunctions.ReplaceDomain(User)
            'PageTitleText.InnerHtml = "<span style=''>PWCS Site</span>"
        ElseIf (Server.Contains("wbhill08dev")) Then
            'UserInformationText.InnerText = CustomFunctions.ReplaceDomain(User)
            PageTitleText.InnerHtml = "<span style='color:Red; font-size:30px;'>PWCS Server Development Site</span>"
        ElseIf (Server.Contains("localhost")) Then
            'UserInformationText.InnerText = CustomFunctions.ReplaceDomain(User)
            PageTitleText.InnerHtml = "<span style='color:Red; font-size:30px;'>PWCS Localhost Development Site</span>"
        End If
        
    End Sub
</script>

<div id="PageTitleText" style="text-align:center;" runat="server"></div>

<table class="" style="background-color: #304e80; width:100%; border:0px; padding:0px; margin:0px">
    <tbody>
        <tr>
            <td align="left">
                <a title="Hill Air Force Base" href="http://www.hill.af.mil/">
                    <img border="0" alt="Hill Air Force Base" src="images/hillafb2.jpg" width="900" height="60" />
                </a>
            </td>
            <td>
                &nbsp;
            </td>
            <td align="right" style="white-space:nowrap;">
                <a title="U.S. Air Force" href="http://www.af.mil">
                    <img border="0" alt="U.S Air Force" src="images/nav_03.jpg" width="187" height="60" />
                </a>
            </td>
        </tr>
</tbody>
</table>
