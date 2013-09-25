<%@ Page Language="VB" AutoEventWireup="false" CodeFile="home.aspx.vb" Inherits="home" Debug="true" %>
<%@ Register TagPrefix="UserControl" TagName="SourceFiles" Src="~/userControls/SourceFiles.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="PageHeader" Src="~/userControls/PageHeader.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="SiteNavigation" Src="~/userControls/SiteNavigation.ascx" %>

<!DOCTYPE html>
<!--https://wbhill03.hill.afmc.ds.af.mil/PWCS/-->

<html>
<head id="Head1" runat="server">
    <title><%=CustomFunctions.PageTabText("Home")%></title>

    <UserControl:SourceFiles ID="SourceFiles1" runat="server" />

    <script type="text/javascript" src="js/Default_Page.js"></script>
    
    <style type="text/css">
        
        label { text-align:right; padding-right:10px; }
        
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            //Here we match the height of the trunk id/serial number div to the height of the account list div.
            //This helps the page to look a little nicer.
            var Height = $("#AccountsListDiv").height();
            $("#TrunkIdAndSerialNumberDiv").height(Height);

            //This works...
            $("[id^='Organization_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "text" });

            //GetAccountList();
            BuildRankSelectList("#NewRank");
            //$("#AccountInfoDiv").hide();
            //$("#AccountCommentsDiv").hide();
            //$("#ManagersInformationDiv").hide();

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

    <UserControl:PageHeader ID="PageHeader1" runat="server" />
    <UserControl:SiteNavigation ID="SiteNavigation1" runat="server" />

    <div id="DivTopMessage" style="display:none;" runat="server"></div>
    
    <div style="text-align:center; margin-bottom:5px;">
        Search:
        <input type="text" id="SerialNumberSearch" class="k-textbox" style="" runat="server" />
        <span class="Comment">(Serial Number or Trunk ID)</span>
    </div>
    
    <div id="SerialNumberSearchDiv" style=""></div>
    <br />
    <div id="AccountListContainer" class="k-block" style="float:left; margin-right:5px;">
        <div class="k-header" style="text-align:center; font-weight:bold;">
            Accounts
        </div>
        <div id="AccountsListDiv" runat="server">
            <%=WebServiceFunctions.AccountListDisplay()%>
        </div>
    </div>
    
    <div id="TrunkIdAndSerialNumberContainer" class="k-block" style="float:left; margin-right:5px; display:none;" runat="server">
        <div class="k-header" style="text-align:center;">
            Account Assets
        </div>
        <div id="TrunkIdAndSerialNumberDiv" style="overflow-y:scroll;" runat="server">
            before...
        </div>
    </div>

    <div id="AccountHeading" class="O-Auto" style="text-align:center;background-color:#2E83FF;color:White; margin-bottom:5px;" runat="server">
        <div style="display:inline-block; float:left;">
            Account Actions...
        </div>
        <div>
            <h1 class="MinimumHeading">
                <span id="AccountCodePlaceHolder" runat="server">0000...</span> - 
                <span id="AccountOrgPlaceHolder" runat="server">ORG...</span>
            </h1>
        </div>
    </div>
    
    <div class="O-Auto">
        
        <div id="ManagersInformationDiv" class="lightGrayBorder" style="margin-bottom:5px;">
            <h4 style="line-height:0px; text-align:center;">Account Managers</h4>
            <div id="ManagersInformationPlaceHolder" runat="server"></div>
        </div>

        <div class="O-Auto">

            <div id="AccountInfoDiv" class="lightGrayBorder" style="float:left; height:300px; margin:0px 5px 5px 0px;">
                <h4 style="line-height:0px; text-align:center;">Annual Requirements</h4>
                <div id="AccountInfoPlaceHolder" runat="server"></div>
            </div>

            <div id="AccountCommentsDiv" class="lightGrayBorder blankContainer O-Auto" style="min-width:300px; max-height:300px;">
                <h4 style="line-height:0px; text-align:center;">Account Comments</h4>
                <div id="AccountCommentsPlaceHolder" runat="server"></div>
            </div>

        </div>

        <div id="AssetInformationDiv" class="lightGrayBorder" style="margin-bottom:5px;">
            <h4 style="line-height:0px; text-align:center;">Asset Information</h4>
            <div id="AssetInformationPlaceHolder" runat="server"></div>
        </div>
        
        <div id="AssetMaintenanceHistoryDiv" class="lightGrayBorder" style="margin-bottom:5px;">
            <h4 style="line-height:0px; text-align:center;">Maintenance History</h4>
            <div id="AssetMaintenanceHistoryPlaceHolder" runat="server"></div>
        </div>
        
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