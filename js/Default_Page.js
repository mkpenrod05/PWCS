/********************************************************************************
"WebStatusCodes" is defined in validation.js
********************************************************************************/

function GetAccountList() {
    jQuery.ajax({
        url: "WebService.asmx/AccountList",
        type: "POST",
        //data: "{Account:'" + Account + "'}",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        //async must be set to false here to allow the ajax call to finish before the rest of the jQuery executes
        async: false,
        beforeSend: function () {
            Processing("#AccountListDiv");
        },
        complete: function () {
            //Here we match the height of the trunk id/serial number div to the height of the account list div.
            //This helps the page to look a little cleaner
            var Height = $("#AccountListDiv").height();
            $("#TrunkIdAndSerialNumberDiv").height(Height);
        },
        success: function (data) {
            AccountListDiv.innerHTML = data.d

            //this function is defined towards the bottom of this page
            AccountListClick();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetAccountList() function

function GetTrunkIdAndSerialNumber(ID) {
    jQuery.ajax({
        url: "WebService.asmx/TrunkIdAndSerialNumber",
        type: "POST",
        data: "{Account:'" + ID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        beforeSend: function () {
            Processing("#TrunkIdAndSerialNumberDiv");
        },
        complete: function () {

        },
        success: function (data) {
            TrunkIdAndSerialNumberDiv.innerHTML = data.d
            //PrepSerialNumberInformation();
            //alert(data.d.html);

            $("#TrunkAndSerialNumberTable tr").click(function () {
                var SerialNumber = $(this).attr('id'); //Example output: TrunkSNRow_320CEG1644

                if (SerialNumber.indexOf("TrunkSNRow_") == 0) {
                    SerialNumber = SerialNumber.replace("TrunkSNRow_", ""); //Example output: 320CEG1644
                    GetSerialNumberInformation(SerialNumber);
                    GetSerialNumberMaintenanceHistory(SerialNumber);
                    $("[id^='TrunkSNRow_']").removeClass("selectedRow");
                    $("#TrunkSNRow_" + SerialNumber).addClass("selectedRow");
                }
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetTrunkIdAndSerialNumber() function

function GetManagersInformation(ID) {
    jQuery.ajax({
        url: "WebService.asmx/ManagersInformation",
        type: "POST",
        data: "{Account:'" + ID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        beforeSend: function () {
            Processing("#ManagersInformationDiv");
        },
        complete: function () {

        },
        success: function (data) {
            ManagersInformationDiv.innerHTML = data.d;
            //alert(data.d.html);

            $("[id^='position_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "select", select_options: { Primary: "Primary", Alternate: "Alternate"} });
            $("[id^='rank_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "select", select_options: RankOptions });
            $("[id^='fname_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "text" });
            $("[id^='lname_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "text" });
            $("[id^='org_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "text" });
            $("[id^='phone_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "text" });
            $("[id^='email_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "email" });

            //added the parameter "account_code" in the edit-in-place because we needed to refresh the managers information div
            //if the training date is changed.  The cell the date resides in is color coded based on the date entered.  We want
            //the user to see the color change immediately after saving so this provides a way to refresh the content. 
            $("[id^='trained_']").eip("WebService.asmx/ManagerInfoUpdate", { form_type: "date", account_code: ID });

            $("[id^='DeleteManager_']").hover(function () { $(this).toggleClass("ui-icon2") });
            $("[id^='DeleteManager_']").click(function () {
                var DeleteManagerLinkID = $(this).attr("id"); //e.g. "DeleteManager_158"
                DeleteManagerLinkID = DeleteManagerLinkID.replace("DeleteManager_", ""); //e.g. "158"

                var ManagerName = $("#fname_" + DeleteManagerLinkID).text() + " " + $("#lname_" + DeleteManagerLinkID).text();
                //alert(ManagerName);
                /*
                We call a function here that opens a dialog box asking the user if they are sure they want to delete this manager.
                */
                DeleteManagerDialog(DeleteManagerLinkID, ID, ManagerName);
            });

            $("#addManager").hover(function () { $(this).toggleClass("shade") });
            $("#addManager").button().click(function () {
                AddManagerDialog(ID);
                //$("#DialogFormDiv").dialog("open"); //This diaglog is defined below in the AddManagerDialog() function
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetManagersInformation() function

function GetAccountInfo(ID) {
    jQuery.ajax({
        url: "WebService.asmx/AccountInfo",
        type: "POST",
        data: "{Account:'" + ID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        beforeSend: function () {
            Processing("#AccountHeading");
            Processing("#AccountInfoDiv");
            Processing("#AccountCommentsDiv");
        },
        complete: function () {

        },
        success: function (data) {

            AccountHeading.innerHTML = "<div style='float:left;'><table id='AccountActionsTable' class='WhiteIconLayout'><tr>" +
                            "<td id='ReplacementPlanIcon'><img src='images/yellow_paper.png' /></td>" +
            //"<td id='InformationIcon'><img src='images/information.png' /></td>" +
            //"<td id='HelpIcon'><img src='images/help.png' /></td>" +
                            "</tr></table></div>" +
                            "<h1 class='MinimumHeading'>" + data.d.AccountCode + " - <span id='Organization_" + data.d.ID + "'>" + data.d.Organization + "</span>" +
                                "<a id='DeleteAccount_" + data.d.AccountCode + "' style='float:right; margin-right:10px;' class='ui-icon-WhiteXCircle' alt='Delete Account' title='Delete Account'></a>" +
                            "</h1>";
            AccountInfoDiv.innerHTML = "<p style='text-align:center;'><b>Annual Requirements</b><hr /><p>" +
                            "<table class='MainStyle' align='center' style='width:95%'><tr>" +
                            "<th>Annual Requirement</th>" +
                            "<th width=''>Current</th>" +
                            "<th width=''>1st Email</th>" +
                            "<th width=''>2nd Email</th>" +
                            "<th width=''>3rd Email</th></tr>" +
                            "<tr><td><b>Appointment Letter</b></td>" +
                            "<td><span id='AppointmentLetter_" + data.d.ID + "'>" + data.d.AppointmentLetter + "</span></td>" +
                            "<td><span id='EmailAppointmentLetter1_" + data.d.ID + "'>" + data.d.EmailAppointmentLetter1 + "</span></td>" +
                            "<td><span id='EmailAppointmentLetter2_" + data.d.ID + "'>" + data.d.EmailAppointmentLetter2 + "</span></td>" +
                            "<td><span id='EmailAppointmentLetter3_" + data.d.ID + "'>" + data.d.EmailAppointmentLetter3 + "</span></td>" +
                            "</tr>" +

                            "<tr><td><b>Inventory</b></td>" +
                            "<td><span id='Inventory_" + data.d.ID + "'>" + data.d.Inventory + "</span></td>" +
                            "<td><span id='EmailInventory1_" + data.d.ID + "'>" + data.d.EmailInventory1 + "</span></td>" +
                            "<td><span id='EmailInventory2_" + data.d.ID + "'>" + data.d.EmailInventory2 + "</span></td>" +
                            "<td><span id='EmailInventory3_" + data.d.ID + "'>" + data.d.EmailInventory3 + "</span></td>" +
                            "</tr>" +

                            "<tr><td><b>Account Validation</b></td>" +
                            "<td><span id='AccountValidation_" + data.d.ID + "'>" + data.d.AccountValidation + "</span></td>" +
                            "<td><span id='EmailAccountValidation1_" + data.d.ID + "'>" + data.d.EmailAccountValidation1 + "</span></td>" +
                            "<td><span id='EmailAccountValidation2_" + data.d.ID + "'>" + data.d.EmailAccountValidation2 + "</span></td>" +
                            "<td><span id='EmailAccountValidation3_" + data.d.ID + "'>" + data.d.EmailAccountValidation3 + "</span></td>" +
                            "</tr>" +
                            "</table>";

            //                        AccountCommentsDiv.innerHTML = "<table class='MainStyle' style='width:100%;'><tr>" +
            //                            "<th>Account Comments</th></tr>" +
            //                            //"<tr><td><span id='AccountComments_" + data.d.ID + "'>" + data.d.AccountComments + "</span></td></tr>" +
            //                            "<tr><td><span id='AccountComments_" + data.d.ID + "'>Add New Comment</span></td></tr>" +
            //                            "</table>" +
            //                            //"<span id='AccountComments_" + data.d.ID + "'>Add New Comment</span>" +
            //                            "<div id='AdditionalAccountComments' style='padding:2px 10px 2px 10px;'></div>"

            AccountCommentsDiv.innerHTML = "<p style='text-align:center;'><b>Account Comments</b><hr /><p>" +
                            "<div style='padding:2px 10px 2px 10px; verticle-align:center;'>" +
                                "<div class='newItem' style='float:left;'></div>" +
                                "<div style='position:relative; top:3px'><span id='AccountComments_" + data.d.ID + "' class='link'>Add New Account Comment</span></div>" +
                            "</div>" +
                            "<div id='AdditionalAccountComments' style='padding:2px 10px 2px 10px;'></div>"

            $("[id^='Organization_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "text" });

            $("[id^='AppointmentLetter_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date", account_code: ID });
            $("[id^='EmailAppointmentLetter1_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailAppointmentLetter2_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailAppointmentLetter3_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });

            $("[id^='Inventory_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date", account_code: ID });
            $("[id^='EmailInventory1_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailInventory2_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailInventory3_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });

            $("[id^='AccountValidation_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date", account_code: ID });
            $("[id^='EmailAccountValidation1_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailAccountValidation2_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });
            $("[id^='EmailAccountValidation3_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "date" });

            //$("[id^='AccountComments_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "textarea", textarea_text: "blank" });
            var CommentsWidth = $("#AccountCommentsDiv").width() / 11;
            //alert(CommentsWidth);
            $("[id^='AccountComments_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "textarea", after_save: false, textarea_text: "blank", rows: 5, cols: CommentsWidth });

            $("#ReplacementPlanIcon").click(function () {
                //window.location.href = "ReplacementPlan.aspx?account=" + data.d.AccountCode;
                window.open("ReplacementPlan.aspx?account=" + data.d.AccountCode, "_blank");
            });

            $("[id^='DeleteAccount_']").hover(function () { $(this).toggleClass("ui-icon2") });
            $("[id^='DeleteAccount_']").click(function () {
                $("#DivTopMessage").empty().removeClass();
                /*
                Need to check and make sure there are no managers still asssigned to the account before we allow the user to delete the account.
                Simply checking to see if a part of the managers table exists will tell us if any managers are still showing on the page.
                When a manager is added or deleted, an ajax call is used to reload the mangers table.  Deleting managers on the page will reload 
                the managers div and cause a null value here which is what we want so that the user can successfully delete the account.
                */
                if ($("[id^='position_']").attr("id") == null) {
                    /* 
                    No managers are currently assigned to the account.  Process account deletion... 
                    */
                    var DeleteAccountLinkID = $(this).attr("id");
                    DeleteAccountLinkID = DeleteAccountLinkID.replace("DeleteAccount_", "");
                    /*
                    We call a function here that opens a dialog box to ask the user if they really want to delete an account.
                    */
                    DeleteAccountConfirm(DeleteAccountLinkID);

                } else {
                    /*
                    else - There are still managers assigned to the account which are showing in the managers div.  Throw error...
                    */
                    ThrowError("#DivTopMessage", "There are still account managers assigned to this account!  Please delete these managers and try again...");
                }
            });

            //GetGrabActionType() function is located in js/RetrieveLogData.js and updates the content
            //in the account comments div with an id="AdditionalAccountComments"
            GetGrabActionType("Account Update", "", data.d.ID, "account_comments", "", "AdditionalAccountComments");
            //Reference: GetGrabActionType(ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue, HTMLElement)
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetAccountInfo() function

function GetSerialNumberInformation(SN) {
    jQuery.ajax({
        url: "WebService.asmx/SerialNumberInformation",
        type: "POST",
        //data: "{UserName:'" + $("#UserNameAdd").attr('value') + "', Reason:'" + $("#Reason").attr('value') + "', AccessType:'" + $("#AccessType").attr('value') + "', TargetSystem:'" + $("#TargetSystem").attr('value') + "', ClientSystem:'" + $("#ClientSystem").attr('value') + "', RemedyTicket:'" + $("#RemedyTicket").attr('value') + "', ReturnDate:'" + $("#ReturnDate").attr('value') + "'}",
        //data: "{Account:'" + $("#SerialNumberSearch").attr('value') + "'}",
        data: "{SerialNum:'" + SN + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        beforeSend: function () {
            Processing("#SerialNumberInformationDiv");
        },
        complete: function () {

        },
        success: function (data) {
            SerialNumberInformationDiv.innerHTML = data.d
            //alert(data.d.html);

            var RecordID = $("[id^='AssetComments_']").attr("id"); // - e.g. AssetComments_7465
            //if this webservice is called for a serial number that doesn't exist, this id is going to comeback as "AssetComments_False"
            //we can check this RecordID variable below to call another function or not
            RecordID = RecordID.replace("AssetComments_", "");
            //alert(RecordID); // - e.g. "7465" OR "False"


            $("[id^='ViewTrunkLog_']").hover(function () { $(this).toggleClass("clipboard") });
            $("[id^='ViewTrunkLog_']").click(function () {
                var TrunkID = $(this).attr("id");
                TrunkID = TrunkID.replace("ViewTrunkLog_", "");
                GetTrunkingSystemLogByID(TrunkID, "#TrunkingSystemLogByIDDiv");
            });

            $("[id^='serialNum_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "text" });
            $("[id^='modelNum_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "text" });
            $("[id^='modelDesc_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "text" });
            $("[id^='9600B_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: HardwareOptions });
            $("[id^='AES_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: HardwareOptions });
            $("[id^='OTAR_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: HardwareOptions });
            $("[id^='OTAP_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: HardwareOptions });

            //$("[id^='assetComments_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "textarea", cols: 50 });
            $("[id^='AssetComments_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "textarea", after_save: false, textarea_text: "blank", rows: 5, cols: 70 });

            $("[id^='assetDisabled_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: { No: "No", Yes: "Yes"} });
            $("[id^='assetDisabledDate_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "date" });
            //$("[id^='assetDisabledComments_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "textarea" });
            $("[id^='assetDisabledComments_']").eip("WebService.asmx/SerialNumberInfoUpdate", { form_type: "select", select_options: DisabledStateOptions });

            //RecordID is defined at the top of the function
            if (RecordID !== "False") {
                //GetGrabActionType() function is located in js/RetrieveLogData.js and updates the content
                //in the account comments div with an id="AdditionalAssetComments"
                GetGrabActionType("Asset Update", "", RecordID, "assetComments", "", "AdditionalAssetComments");
                //Reference: ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue, HTMLElement
            }
            
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetSerialNumberInformation() function

function GetSerialNumberMaintenanceHistory(SN) {
    jQuery.ajax({
        url: "WebService.asmx/SerialNumberMaintenanceHistory",
        type: "POST",
        //data: "{UserName:'" + $("#UserNameAdd").attr('value') + "', Reason:'" + $("#Reason").attr('value') + "', AccessType:'" + $("#AccessType").attr('value') + "', TargetSystem:'" + $("#TargetSystem").attr('value') + "', ClientSystem:'" + $("#ClientSystem").attr('value') + "', RemedyTicket:'" + $("#RemedyTicket").attr('value') + "', ReturnDate:'" + $("#ReturnDate").attr('value') + "'}",
        //data: "{Account:'" + $("#SerialNumberSearch").attr('value') + "'}",
        data: "{SerialNum:'" + SN + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        beforeSend: function () {
            Processing("#SerialNumberMaintenanceHistoryDiv");
        },
        complete: function () {

        },
        success: function (data) {
            SerialNumberMaintenanceHistoryDiv.innerHTML = data.d
            //alert(data.d.html);
            $("#AddMaintenanceActionLink").hover(function () { $(this).toggleClass("ui-icon3") });
            $("#AddMaintenanceActionLink").click(function () {
                window.open("maintenance.aspx?SerialNumber=" + SN, "_blank");
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetSerialNumberMaintenanceHistory() function

function GetSerialNumberSearch(SearchValue, SearchConstraint) {
    jQuery.ajax({
        url: "WebService.asmx/SerialNumberSearch",
        type: "POST",
        data: "{SearchValue:'" + SearchValue + "', SearchConstraint:'" + SearchConstraint + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            SerialNumberSearchDiv.innerHTML = data.d
            //alert(data.d.html);

            $("[id^='ViewTrunkLog_']").hover(function () { $(this).toggleClass("clipboard") });
            $("[id^='ViewTrunkLog_']").click(function () {
                var TrunkID = $(this).attr("id");
                TrunkID = TrunkID.replace("ViewTrunkLog_", "");
                GetTrunkingSystemLogByID(TrunkID, "#TrunkingSystemLogByIDDiv");
            });

            $("#SNID tr").click(function () {
                //alert($(this).attr("id")); //- This returns the account and serial number (i.e. 62WC_326ACG4587) for the selected row
                var account = $(this).attr("id").match(/^[^_]+(?=_)/); //This matches everything before the underscore

                //In the WebServiceFunctions.SerialNumberSearch function we give each row that is pulled from the Archive table
                //an ID of "FAKE_" to avoid the functions below from trying to execute when the user clicks on the row.  These
                //rows are given a red'ish background color to distinguish them from rows that have been pulled from the assets table.
                if (account != "FAKE") {

                    var serialNumber = $(this).attr("id").match(/_([^_]*)/); //This matches everything after the underscore
                    //alert(account + " : " + serialNumber[1]);

                    $("#SNID tr").removeClass("selectedRow");   //remove the selectedRow class when a new row is clicked on
                    $(this).addClass("selectedRow");            //add the selectedRow class to the row that is clicked

                    //alert(account);
                    if (($.trim(account) !== "NONE") && (account !== null) && (account !== "NULL")) {
                        GetTrunkIdAndSerialNumber(account);
                        GetAccountInfo(account);
                        GetManagersInformation(account);
                        GetSerialNumberMaintenanceHistory(serialNumber[1]);
                        GetSerialNumberInformation(serialNumber[1]);

                        $("#TrunkIdAndSerialNumberDiv").show();
                        $("#AccountHeading").show();
                        $("#AccountInfoDiv").show();
                        $("#AccountCommentsDiv").show();
                        $("#ManagersInformationDiv").show();

                    } else { //if the account field is null or equals "NONE", we don't want the account divs to show because 
                        //the edit-in-place will cause the page to crash
                        GetSerialNumberMaintenanceHistory(serialNumber[1]);
                        GetSerialNumberInformation(serialNumber[1]);
                        $("#TrunkIdAndSerialNumberDiv").hide();
                        $("#AccountHeading").hide();
                        $("#AccountInfoDiv").hide();
                        $("#AccountCommentsDiv").hide();
                        $("#ManagersInformationDiv").hide();
                    }
                }
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of GetSerialNumberSearch() function

function ChangeManagerStatus(Status, ManagerID, ManagerAccount) {
    //Status should be either "True" or "False"
    jQuery.ajax({
        url: "WebService.asmx/ChangeManagerStatus",
        type: "POST",
        data: "{Status:'" + Status + "', ManagerID:'" + ManagerID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.d.IsError == false) {
                ThrowSuccess("#DivTopMessage", data.d.str);
            } else {
                ThrowError("#DivTopMessage", data.d.ErrorMessage);
            }
            GetManagersInformation(ManagerAccount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of ChangeManagerStatus() function

function AddManager(Account) {
    jQuery.ajax({
        url: "WebService.asmx/AddManager",
        type: "POST",
        data: "{NewPosition:'" + $("#NewPosition").val() + "', NewRank:'" + $("#NewRank").val() + "', NewFirstName:'" + $("#NewFirstName").val() + "', NewLastName:'" + $("#NewLastName").val() + "', NewOrg:'" + $("#NewOrg").val() + "', NewPhone:'" + $("#NewPhone").val() + "', NewEmail:'" + $("#NewEmail").val() + "', NewTrainingDate:'" + $("#NewTrainingDate").val() + "', Account:'" + Account + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.d.IsError == true) {
                ThrowError("#DivTopMessage", data.d.ErrorMessage);
            } else {
                ThrowSuccess("#DivTopMessage", data.d.str);
            }
            GetManagersInformation(Account);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of AddManager() function

function CheckAccountStatus(Status, Account) {
    jQuery.ajax({
        url: "WebService.asmx/CheckAccountStatus",
        type: "POST",
        data: "{Status:'" + Status + "', Account:'" + Account + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.d.IsError == true) {
                //this error means that the account selected to be deleted still has assets assigned to it
                //or there was an exception/failure with this WebService.asmx or CustomFunctions.ChangeAccountStatus().
                ThrowError("#DivTopMessage", data.d.ErrorMessage); //ThrowError is defined in js/validation.js
                alert(data.d.str);
            } else {
                //account had no assets assigned to it and was successfully deleted.
                ThrowSuccess("#DivTopMessage", data.d.str); //ThrowSuccess is defined in js/validation.js
                GetAccountList();
                $("#TrunkIdAndSerialNumberDiv").hide();
                $("#AccountHeading").empty();
                $("#AccountCommentsDiv").empty();
                $("#AccountInfoDiv").empty();
                $("#AccountCommentsDiv").empty();
                $("#ManagersInformationDiv").empty();
                $("#SerialNumberInformationDiv").empty();
                $("#SerialNumberMaintenanceHistoryDiv").empty();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
} //end of CheckAccountStatus() function

function GetTrunkingSystemLogByID(TrunkID, HtmlElement) {

    $(HtmlElement).dialog({
        autoOpen: false,
        modal: true,
        height: 700,
        width: 500,
        title: "Trunking System Log By ID"
    });

    $(HtmlElement).dialog("open");

    jQuery.ajax({
        url: "WebService.asmx/TrunkingSystemLogByID",
        type: "POST",
        data: "{TrunkID:'" + TrunkID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        //async: false,
        beforeSend: function () {
            Processing(HtmlElement);
        },
        complete: function () {

        },
        success: function (data) {
            $(HtmlElement).html(data.d);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
        },
        statusCode: WebStatusCodes
    });
    
} //end of GetTrunkingSystemLogByID() function

function AccountListClick() {
    $("#AccountListTable tr").click(function () {
        /*
        Need to clear this div so that error/success messages aren't carried over to the next account that is clicked on.
        */
        $("#DivTopMessage").empty().removeClass();

        var ID = $(this).attr('id');
        //alert(ID); //e.g. 74LM
        var tempSN = "False";

        GetTrunkIdAndSerialNumber(ID);
        GetAccountInfo(ID);
        GetManagersInformation(ID);

        //force the following div's to load with no information
        GetSerialNumberInformation(tempSN);
        GetSerialNumberMaintenanceHistory(tempSN);

        $("#TrunkIdAndSerialNumberDiv").show();
        $("#AccountHeading").show();
        $("#AccountInfoDiv").show();
        $("#AccountCommentsDiv").show();
        $("#ManagersInformationDiv").show();

    });  //end "#AccountListTable tr" click function
} //end AccountListClick()

function AddManagerDialog(AccountSet) {

    var ValidateTips = $(".ValidateTips");

    function UpdateValidationTips(t) {
        ValidateTips.addClass("ui-state-error");
        ValidateTips.text(t).addClass("ui-state-highlight");
        setTimeout(function () { ValidateTips.removeClass("ui-state-error"); }, 1500);
    }

    function CheckLength(input, str, min, max) {
        if ($.trim(input.val()).length > max || $.trim(input.val()).length < min) {
            //alert("test");
            input.addClass("ui-state-error");
            //alert("after");
            UpdateValidationTips("Length of " + str + " must be between " +
					            min + " and " + max + " characters.");
            return false;
        } else {
            return true;
        }
    }

    $("#DialogFormDiv").dialog({
        autoOpen: false,
        //height: 500,
        width: 450,
        modal: true,
        title: "Add Manager to Account " + AccountSet,
        buttons: {
            "Add Manager": function () {
                //alert(ID);
                var AllFields = $([])
                    .add($("#NewPosition"))
                    .add($("#NewRank"))
                    .add($("#NewFirstName"))
                    .add($("#NewLastName"))
                    .add($("#NewOrg"))
                    .add($("#NewPhone"))
                    .add($("#NewEmail"));
                var bValid = true;
                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");
                //alert("before removeclass");
                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");
                //alert("after removeclass");
                bValid = bValid && CheckSelectList($("#NewPosition"), "Please select the manager's position.");
                bValid = bValid && CheckSelectList($("#NewRank"), "Please select the manager's rank.");
                bValid = bValid && CheckLength($("#NewFirstName"), "First Name", 1, 30);
                bValid = bValid && CheckLength($("#NewLastName"), "Last Name", 1, 30);
                bValid = bValid && CheckLength($("#NewOrg"), "Organization", 3, 50);
                bValid = bValid && CheckLength($("#NewPhone"), "Phone Number", 7, 25);
                bValid = bValid && CheckLength($("#NewEmail"), "Email", 6, 50);

                if (bValid) {
                    //alert(AccountSet); //e.g. "54FD"
                    AddManager(AccountSet);
                    $(this).dialog("close");
                }
            },
            Cancel: function () {
                //alert(AccountSet);
                $(this).dialog("close");
            }
        },
        close: function () {
            //AllFields.val("");
            $("#NewFirstName").val("");
            $("#NewLastName").val("");
            $("#NewOrg").val("");
            $("#NewPhone").val("");
            $("#NewEmail").val("");
            $("#NewTrainingDate").val("");
            $(this).dialog();
        }
    });
    $("#DialogFormDiv").dialog("open");

    $("#NewTrainingDate").datepicker({
        showButtonPanel: true,
        closeText: "Cancel"
    });

} //end of AddManagerDialog()

function DeleteManagerDialog(ManagerID, ManagerAccount, ManagerName) {
    $("#DeleteManagerConfirm").html(
        "<div class='BlackOnWhite'>" +
            "<div style='text-align:center;'>Are you sure you want to delete this manager?</div>" +
            "<div style='text-align:center;'><span class='HeaderText'>" + ManagerName + "<span></div>" +
        "</div>"
    );
    $("#DeleteManagerConfirm").dialog({
        autoOpen: false,
        height: 175,
        width: 325,
        modal: true,
        title: "Delete this manager?",
        buttons: {
            "Confirm": function () {
                ChangeManagerStatus("False", ManagerID, ManagerAccount)
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });
    $("#DeleteManagerConfirm").dialog("open");
} //end of DeleteManagerDialog() function

function DeleteAccountConfirm(Account) {
    $("#DeleteAccountConfirm").html(
        "<div class='BlackOnWhite'>" +
            "<div style='text-align:center;'>Are you sure you want to delete this account?</div>" +
            "<div style='text-align:center;'><span class='HeaderText'>" + Account + "<span></div>" +
        "</div>"
    );
    $("#DeleteAccountConfirm").dialog({
        autoOpen: false,
        modal: true,
        height: 175,
        width: 325,
        title: "Delete Account " + Account + "?",
        buttons: {
            "Confirm": function () {
                CheckAccountStatus("False", Account);
                $(this).dialog("close");
            },
            Cancel: function () {

                $(this).dialog("close");
            }
        }
    });
    $("#DeleteAccountConfirm").dialog("open");
}
