<%@ Page Language="VB" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data.CommandBehavior" %>
<%@ Register TagPrefix="UserControl" TagName="SourceFiles" Src="~/userControls/SourceFiles.ascx" %>

<!DOCTYPE html>
<!--https://wbhill03.hill.afmc.ds.af.mil/PWCS/-->

<script runat="server">
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        If UserValidation.PageAccess(HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower()) = False Then
            Response.Write("Access Denied!")
            Response.End()
        End If
        
        If IsPostBack Then
            Dim NewUpgradeCost As Integer = Request.Form("UpgradeCostInput")
            Dim NewUserInstructions As String = Request.Form("UserInstructionsInput")
            
            CustomFunctions.UpdateSettingsByPageAndName("ReplacementPlan", "UpgradeCost", NewUpgradeCost)
            CustomFunctions.UpdateSettingsByPageAndName("ReplacementPlan", "UserInstructions", NewUserInstructions)
            
            'Response.Write("PostBack = " & IsPostBack)
        End If
        
        Dim MyAccount As String = Request.QueryString("Account")
        Dim MyRecord As New ReturnObject
        Dim AccountInfo As New AccountInfoObject
        
        MyRecord = CustomFunctions.GetSettingsByPage("ReplacementPlan")
        AccountInfo = WebServiceFunctions.AccountInfo(MyAccount)
        
        For Each Setting As Dictionary(Of String, String) In MyRecord.Data
            For Each Pair In Setting
                
                'Response.Write("Key: " & Pair.Key & " - Value: " & Pair.Value & "<br />")
                
                If (Pair.Key = "UpgradeCost") Then
                    SettingUpgradeCost = System.Convert.ToDecimal(Pair.Value)
                ElseIf (Pair.Key = "UserInstructions") Then
                    SettingUserInstructions = Pair.Value
                End If
            
            Next
        Next
        
        If MyAccount = "" Then
            ReplacementPlanForm.Controls.Add(New LiteralControl("No account was specified!"))
        Else
            ReplacementPlanForm.Controls.Add(New LiteralControl(MakePlan(MyAccount, SettingUpgradeCost)))
        End If
        
        ReplacementPlanCostDiv.InnerHtml = String.Format(CostDisplayFormat, ReplacementPlanCost)
        AccountNumberSpan.InnerText = MyAccount
        AccountOrgSpan.InnerText = AccountInfo.Organization
        DateDiv.InnerHtml = "Date: <b>" & Now.ToShortDateString() & "</b>"
        UserInstructions.InnerHtml = "<b>" & SettingUserInstructions & "</b>"
        
        'Send data to input fields and form for modification
        UpgradeCostInput.Text = SettingUpgradeCost
        UserInstructionsInput.InnerText = SettingUserInstructions
        ReplacementPlanForm.Action = "ReplacementPlan.aspx?account=" + MyAccount
        
    End Sub
    
    'Protected Sub FormSubmit()
        
    '    Dim NewUpgradeCost As Integer = Request.Form("UpgradeCostInput")
    '    Dim NewUserInstructions As String = Request.Form("UserInstructionsInput")
    '    Dim returnObject As New ReturnObject
        
    '    returnObject = CustomFunctions.UpdateSettingsByPageAndName("ReplacementPlan", "UpgradeCost", NewUpgradeCost)
    '    CustomFunctions.UpdateSettingsByPageAndName("ReplacementPlan", "UserInstructions", NewUserInstructions)
        
    '    'Response.Write(returnObject.IsError & " - " & returnObject.ErrorMessage)
    '    'Response.End()
    '    'Ran out of time to throw an error message here to the user if one of these updates fail...
        
    'End Sub
    
    'Global variables
    Dim ReplacementPlanCost As Decimal = 0
    Dim CostDisplayFormat As String = "{0:$#,##0.00}"
    Dim SettingUpgradeCost As Decimal = 0
    Dim SettingUserInstructions As String = ""
    
    Public Function MakeHeaderRow(ByVal Title As String, ByVal TitleNote As String) As String
        Dim HeaderRowTH As String = "<tr>" & _
            "<td colspan='8' class='ReplacementPlan_" & Title & "'><b>" & Title & "</b> " & _
                "<br /><span style='font-size:12px;'>(" & TitleNote & ")</span></td>" & _
            "</tr>" & _
            "<tr>" & _
            "<th class='THSerialNum'>Serial Number</th>" & _
            "<th class='THModelNum'>Model Number</th>" & _
            "<th class='THModelDesc'>Model Description</th>" & _
            "<th class='THHW'>9600B</th>" & _
            "<th class='THHW'>AES</th>" & _
            "<th class='THHW'>OTAR</th>" & _
            "<th class='THHW'>OTAP</th>" & _
            "<th class='THCost'>Cost</th>" & _
            "</tr>"
        Return HeaderRowTH
    End Function
    
    'Public Function MakePlan(ByVal Account As String) As String
    ' Dim strHTML As String = "<p>" & GetUpgrade(Account) & "</p>"
    '    Return strHTML
    'End Function
  
    Public Function MakePlan(ByVal Account As String, ByVal SettingUpgradeCost As Decimal) As String
        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spReplacementPlan")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection
        
        Dim strHTML As String = ""
        
        Dim UpgradeCost As Decimal = 0
        Dim ReplaceCost As Decimal = 0
        Dim TotalCost As Decimal = 0
        
        Dim UpgradeData As String = ""
        Dim ReplaceData As String = ""
        Dim CompatibleData As String = ""
        Dim UnknownData As String = ""
        
        Try
            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)
            
            While objDataReader.Read()
                Dim Cost As Decimal = 0.0
                
                'If objDataReader("category") = "1-upgrade" Then Cost = 350.0 Else Cost = objDataReader("Cost")
                'If objDataReader("category") = "1-upgrade" Then Cost = objDataReader("UpgradeCost") Else Cost = objDataReader("Cost")
                If objDataReader("category") = "1-upgrade" Then Cost = SettingUpgradeCost Else Cost = objDataReader("Cost")
                If objDataReader("category") = "3-compatible" Then Cost = 0.0
                
                Dim Row As String = "<tr>" & _
                    "<td>" & objDataReader("SerialNum").ToString & "</td>" & _
                    "<td>" & objDataReader("ModelNum").ToString & "</td>" & _
                    "<td>" & objDataReader("ModelDesc").ToString & "</td>" & _
                    "<td class='TDHW'>" & objDataReader("9600B").ToString & "</td>" & _
                    "<td class='TDHW'>" & objDataReader("AES").ToString & "</td>" & _
                    "<td class='TDHW'>" & objDataReader("OTAR").ToString & "</td>" & _
                    "<td class='TDHW'>" & objDataReader("OTAP").ToString & "</td>" & _
                    "<td class='cost'>" & String.Format(CostDisplayFormat, Cost) & "</td>" & _
                    "</tr>"
                
                'This stored procedure adds a column to the result set called "category"
                'There are four values that category can equal: "1-upgrade", "2-replace", "3-compatible", "4-unknown" 
                If objDataReader("category") = "1-upgrade" Then
                    UpgradeData = UpgradeData & Row
                    UpgradeCost = UpgradeCost + Cost
                ElseIf objDataReader("category") = "2-replace" Then
                    ReplaceData = ReplaceData & Row
                    ReplaceCost = ReplaceCost + objDataReader("Cost")
                ElseIf objDataReader("category") = "3-compatible" Then
                    CompatibleData = CompatibleData & Row
                ElseIf objDataReader("category") = "4-unknown" Then
                    UnknownData = UnknownData & Row
                End If
                
            End While
            
            strHTML = strHTML & "<table class='ReplacementPlanTable'>"
            strHTML = strHTML & MakeHeaderRow("Upgrade", "missing required hardware modules") & UpgradeData
            strHTML = strHTML & "<tr style='background-color:gray;'><td colspan='7'></td>"
            strHTML = strHTML & "<td style='background-color:white;' class='cost'><b>" & String.Format(CostDisplayFormat, UpgradeCost) & "</b></td></tr>"
            strHTML = strHTML & "</table>"
            
            strHTML = strHTML & "<table class='ReplacementPlanTable'>"
            strHTML = strHTML & MakeHeaderRow("Replace", "not compatible with future trunking system requirements") & ReplaceData
            strHTML = strHTML & "<tr style='background-color:gray;'><td colspan='7'></td>"
            strHTML = strHTML & "<td style='background-color:white;' class='cost'><b>" & String.Format(CostDisplayFormat, ReplaceCost) & "</b></td></tr>"
            strHTML = strHTML & "</table>"
            
            strHTML = strHTML & "<table class='ReplacementPlanTable'>"
            strHTML = strHTML & MakeHeaderRow("Compatible", "assets have all required hardware modules") & CompatibleData
            strHTML = strHTML & "</table>"
            
            strHTML = strHTML & "<table class='ReplacementPlanTable'>"
            strHTML = strHTML & MakeHeaderRow("Unknown", "unknown if hardware modules are installed") & UnknownData
            strHTML = strHTML & "</table>"
            
            ReplacementPlanCost = UpgradeCost + ReplaceCost
            'Response.Write(ReplacementPlanCost)
        Catch ex As Exception
            strHTML = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try
        Return strHTML
    End Function
    
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Replacement Plan")%></title>

    <UserControl:SourceFiles runat="server" />

    <style type="text/css">
        div { border:0px; }
    </style>

    <script type="text/javascript">
        //jQuery script section...

        $(document).ready(function () {

            var Show = false;
            $("#SettingItems").hide();

            $("#EditSettingsLink").click(function () {
                if (Show == false) {
                    $("#SettingItems").show();
                    Show = true;
                } else {
                    $("#SettingItems").hide();
                    Show = false;
                }
            });

            $("#UpgradeCostInput").kendoNumericTextBox({
                min: 0,
                max: 9999
            });

        });
    </script>

</head>
<body>
    <form id="ReplacementPlanForm" runat="server">
        
        <div id="PageSettingsDiv">
            <p style="text-align:center;">
                <span id="EditSettingsLink" class="link">Edit Page Content</span>
            </p>
            <div id="SettingItems">
                <p>
                    <label>Upgrade Cost:</label>
                    <asp:TextBox id="UpgradeCostInput" name="UpgradeCostInput" runat="server"></asp:TextBox>
                </p>
                <p>
                    <label>User Instructions:</label>
                    <textarea id="UserInstructionsInput" name="UserInstructionsInput" cols="50" rows="5" runat="server"></textarea>
                </p>
                <p>
                    <label></label>
                    <%--<asp:Button id="FormSubmitButton" class="k-button" Text="Submit" OnClick="FormSubmit" runat="server" />--%>
                    <asp:Button id="FormSubmitButton" class="k-button" Text="Submit" runat="server" />
                </p>
            </div>
        </div>

        <table class="ReplacementPlanHeader">
            <tr>
                <th colspan="2">Personal Wireless Communication Systems (PWCS) <br /> Replacement Plan</th>
            </tr>
            <tr>
                <td class="ReplacementPlanHeader_Account">
                    <label>Account Code: </label>
                    <span id="AccountNumberSpan" runat="server"></span>
                    <br />
                    <label>Organization: </label>
                    <span id="AccountOrgSpan" runat="server"></span>
                </td>
                <td class="ReplacementPlanHeader_Date"><div id="DateDiv" runat="server"></div></td>
            </tr>
            <tr>
                <td class="ReplacementPlanHeader_Description" colspan="2">
                    <%--<div>Review and budget for upgrades and replacements.&nbsp;&nbsp;Please direct any questions to the Base PWCS Office at 777-2122 or send an email to Hill.PWCS@hill.af.mil.</div>--%>
                    <div id="UserInstructions" runat="server"></div>
                </td>
            </tr>
            <tr>
                <td class="ReplacementPlanHeader_Total">Estimated cost to become compatible: </td>
                <td class="ReplacementPlanHeader_TotalCost"><div id='ReplacementPlanCostDiv' runat="server"></div></td>
            </tr>
        </table>
        <br />
    
        

    </form>
</body>
</html>
