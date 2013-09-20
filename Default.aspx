<%@ Page Language="VB" %>
<%@ Register TagPrefix="UserControl" TagName="SourceFiles" Src="~/userControls/SourceFiles.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="PageHeader" Src="~/userControls/PageHeader.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="SiteNavigation" Src="~/userControls/SiteNavigation.ascx" %>

<!DOCTYPE html>
<!--https://wbhill03.hill.afmc.ds.af.mil/PWCS/-->

<script runat="server">    
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        'If UserValidation.PageAccess(HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower()) = False Then
        '    Response.Write("Access Denied! - " & HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower())
        '    Response.End
        'End If
        
    End Sub
    
</script>

<html>
<head id="Head1" runat="server">
    <title><%=CustomFunctions.PageTabText("Home")%></title>

    <UserControl:SourceFiles runat="server" />

    <script type="text/javascript" src="js/Default_Page.js"></script>
    
    <style type="text/css">
        
        label { text-align:right; padding-right:10px; }
        
    </style>

    <script type="text/javascript">

        $(document).ready(function () {
            
            GetAccountList();
            BuildRankSelectList("#NewRank");
            $("#AccountInfoDiv").hide();
            $("#AccountCommentsDiv").hide();
            $("#ManagersInformationDiv").hide();

            if (sessionStorage.IsPostData == "True") {

                //localStorage.AccountCode is being set on the Dashboard.aspx page
                if (sessionStorage.AccountCode !== "") {

                    //alert(":" + sessionStorage.AccountCode + ":"); // - e.g. 00JJ

                    GetTrunkIdAndSerialNumber(sessionStorage.AccountCode);
                    GetAccountInfo(sessionStorage.AccountCode);
                    GetManagersInformation(sessionStorage.AccountCode);

                    //force the following div's to load with no information
                    var tempSN = "0000";
                    GetSerialNumberInformation(tempSN);
                    GetSerialNumberMaintenanceHistory(tempSN);

                    $("#TrunkIdAndSerialNumberDiv").show();
                    $("#AccountHeading").show();
                    $("#AccountInfoDiv").show();
                    $("#AccountCommentsDiv").show();
                    $("#ManagersInformationDiv").show();

                    //reset AccountCode to nothing so that returning to the default page by another means
                    //will not force a specific account to load
                    sessionStorage.AccountCode = "";

                } //end localStorage.AccountCode if statement

                //reset IsPostData to false so that returning to the default page by another means
                //will not force specific elements to load
                sessionStorage.IsPostData = "False";

            } //end localStorage.IsPostData if statement

            $("#SerialNumberSearchDiv").hide();

            $("#SerialNumberSearch").keyup(function () {
                var value = $(this).prop("value");
                if (value.length >= 1) {
                    //alert(value);
                    $("#SerialNumberSearchDiv").show();
                    GetSerialNumberSearch(value);
                } else {
                    $("#SerialNumberSearchDiv").hide();
                }
            }); //end "#SerialNumberSearch" keyup function

        });                                     
    
    </script>

</head>
<body>
    
    <form id="form1" runat="server">

    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />

    <div id="DivTopMessage" style="padding:5px;" runat="server"></div>
    <div style="text-align:center;">
        <p>
            <input type="text" id="SerialNumberSearch" style="" runat="server" />
        </p>
    </div>
    <div id="SerialNumberSearchDiv" style=""></div>
    <br />
    
    <div id="AccountListDiv" class="lightGrayBorder" style="Float:left;" runat="server"></div>
    <div id="TrunkIdAndSerialNumberDiv" class="lightGrayBorder blankContainer" style="float:left; overflow-y:scroll; height:700px; display:none;" runat="server"></div>

    <div id="AccountHeading" class="O-Auto" style="text-align:center;background-color:#2E83FF;color:White;"></div>
    
    <div class="O-Auto">
        
        <div id="ManagersInformationDiv" class="lightGrayBorder blankContainer" style="margin:3px;"></div>

        <div class="O-Auto">
            <div id="AccountInfoDiv" class="lightGrayBorder blankContainer" style="margin:3px; float:left; height:300px;"></div>
            <div id="AccountCommentsDiv" class="lightGrayBorder blankContainer O-Auto" style="margin:3px; min-width:300px; max-height:300px;"></div>
        </div>

        <div id="SerialNumberInformationDiv" class="lightGrayBorder blankContainer" style="margin:3px;"></div>
        
        <div id="SerialNumberMaintenanceHistoryDiv" class="lightGrayBorder blankContainer" style="margin:3px;"></div>
        
    </div>

<!--Dialog Form Div's START-->
<div id="DialogFormDiv" style="display:none;">
    <div id='DialogFormError' class='ValidateTips'>&nbsp;</div>
    <br />
    <fieldset>
        <p><label for='NewPosition'>*Position:</label><select id='NewPosition' class=''><option value=""></option><option value='Primary'>Primary</option><option value='Alternate'>Alternate</option></select><br /></p>
        <p><label for='NewRank'>*Rank:</label><select id='NewRank' class=''><!--Options built with function RankSelectList()--></select><br /></p>
        <p><label for='NewFirstName'>*First Name:</label><input type='text' id='NewFirstName' class='' /></p>
        <p><label for='NewLastName'>*Last Name:</label><input type='text' id='NewLastName' class='' /></p>
        <p><label for='NewOrg'>*Organization:</label><input type='text' id='NewOrg' class='' /></p>
        <p><label for='NewPhone'>*Phone Number:</label><input type='text' id='NewPhone' class='' /></p>
        <p><label for='NewEmail'>*Email:</label><input type='text' id='NewEmail' class='' size='30' /></p>
        <p><label for='NewTrainingDate'>Training Date:</label><input type='text' id='NewTrainingDate' class='' /></p>
    </fieldset>
    <br />
    <fieldset class="ui-state-highlight">
        <i>Reminder - Dont forget to add this manager to the <b>Hill Unit PWCS Managers</b> distro list in Outlook!</i>
    </fieldset>
</div>

<div id="DeleteManagerConfirm" style="display:none;"></div>

<div id="DeleteAccountConfirm" style="display:none;"></div>

<div id="TrunkingSystemLogByIDDiv" style="display:none; text-align:center;"></div>

<!--Dialog Form Div's END-->
    
</form>
</body>
</html>
