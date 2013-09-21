<%@ Page Language="VB" %>
<%@ Register TagPrefix="UserControl" TagName="SourceFiles" Src="~/userControls/SourceFiles.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="PageHeader" Src="~/userControls/PageHeader.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="SiteNavigation" Src="~/userControls/SiteNavigation.ascx" %>

<!DOCTYPE html>
<!--https://wbhill03.hill.afmc.ds.af.mil/PWCS/-->

<script runat="server">
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        'If UserValidation.PageAccess(HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower()) = False Then
        '    Response.Write("Access Denied!")
        '    Response.End()
        'End If
        
        FromAccountDiv.InnerHtml = CustomFunctions.AccountList("FromAccount")
        ToAccountDiv.InnerHtml = CustomFunctions.AccountList("ToAccount")
        TransferHeading.InnerHtml = "<h3>Initiate Transfer Number " & CustomFunctions.TransferNumber() & "</h3>"

    End Sub
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Transfer")%></title>

    <UserControl:SourceFiles runat="server" />

    <style type="text/css">
        
        #InitiateTransfer { display:inline-block; vertical-align:top; }
        #ActiveTransfersDiv { display:inline-block; vertical-align:top; }
        #TransferItemsDiv { display:inline-block; vertical-align:top; }
        #TransferItemsDiv h3 { text-align:center; }
        #ButtonPlaceHolder { text-align:center; }
        #DivTopMessage { padding:5px; }
        
        .ViewTransferItems { text-align:center; color:Blue; }
        .ViewTransferItems:hover { cursor:pointer; color:Red; }
        .ActiveTransferHeading { font-weight:bold; font-size:16px; text-align:center; }
    
        /* Adding additional elements to these classes which are defined in css/style.css */
        .lightGrayBorder { margin:0px 20px 20px 0px; }
    
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            //Initiate the Active Transfers section of the page.
            GetActiveTransfers();

            //Hide the asset list div until the user clicks to view it.
            //This element is set to display:inline-block so we have to hide it
            //this way instead of using display:none.
            $("#TransferItemsDiv").hide()

            //Set the TransferNumber variable to the next available transfer number coming out of the database
            var TransferNumber = "<% =CustomFunctions.TransferNumber() %>";

            $("#AccountList_FromAccount").change(function () {
                GetSerialNumberSearchForTransfer($(this).val());
                $("#ResultsListDiv").show();
                if ($(this).val() == "") {
                    $("#ResultsListDiv").hide();
                } else {
                    //$("#ResultsListDiv").append("<div><input type='button' id='submit' value='Transfer' /></div>");
                }
            });

            $("#AccountList_ToAccount").change(function () {

            });

            $("#TransferDate").datepicker({
                showButtonPanel: true,
                closeText: "Close"
            });

            function PrepTransfer() {
                $("#submit").button().click(function () {
                    var ValidateTips = $(".ValidateTips");
                    var FromAccount = $("#AccountList_FromAccount");
                    var ToAccount = $("#AccountList_ToAccount");
                    var TransferDate = $("#TransferDate");
                    var ItemsChecked = "";
                    var bValid = true;

                    var AllFields = $([]).add(FromAccount).add(ToAccount).add(TransferDate);

                    ValidateTips.removeClass("ui-state-error");
                    ValidateTips.removeClass("ui-state-highlight");
                    ValidateTips.text("");

                    AllFields.removeClass("ui-state-highlight");
                    AllFields.removeClass("ui-state-error");

                    if (!$("input:checkbox").is(':checked')) {
                        ItemsChecked = "";
                    } else {
                        ItemsChecked = "Yes"
                    }

                    bValid = bValid && CheckLength(FromAccount, 1, 4, "From Account field must contain a value!");
                    bValid = bValid && CheckLength(ToAccount, 1, 4, "To Account field must contain a value!");
                    bValid = bValid && CheckLength(TransferDate, 1, 12, "The Date field must contain a value!");
                    bValid = bValid && CheckForEmptyString(ItemsChecked, "No items have been selected for transfer!");

                    if (bValid) {

                        var AssetList = "";

                        $("#InitiateTransfer input:checkbox:checked").each(function () {
                            //alert($(this).attr("id")); //e.g. - "320CEG1614"
                            AssetList = AssetList +
                                "<li>" + $(this).attr("id") + "</li>";
                        });

                        $("#ConfirmationDialog").html(
                            "<div class='BlackOnWhite'>" +
                                "<div style=''>Are you sure you want to initiate a transfer for the following list of assets from account " +
                                    "<span class='SpanStrong'>" + FromAccount.val() + "</span> to account <span class='SpanStrong'>" + ToAccount.val() + "</span>?</div>" +
                                "<div style=''><ul style=''>" + AssetList + "</ul></div>" +
                            "</div>"
                        );
                        $("#ConfirmationDialog").dialog({
                            autoOpen: false,
                            modal: true,
                            //height: 175,
                            width: 250,
                            title: "Initiate Transfer Number " + TransferNumber + "?",
                            buttons: {
                                "Confirm": function () {

                                    GetAddTransfer(TransferNumber, FromAccount.val(), ToAccount.val(), TransferDate.val());

                                    $("#InitiateTransfer input:checkbox:checked").each(function () {
                                        //alert($(this).attr("id")); //e.g. - "320CEG1614"
                                        GetAddTransfersList(TransferNumber, $(this).attr("id"));
                                    });

                                    ValidationPassed("Transfer number " + TransferNumber + " has been initiated!");
                                    $("#submit").attr({ "disabled": "disabled", "alt": "Refresh the page to initiate another transfer", "title": "Refresh the page to initiate another transfer" });
                                    FromAccount.attr({ "disabled": "disabled", "alt": "Refresh the page to initiate another transfer", "title": "Refresh the page to initiate another transfer" });
                                    ToAccount.attr({ "disabled": "disabled", "alt": "Refresh the page to initiate another transfer", "title": "Refresh the page to initiate another transfer" });
                                    TransferDate.attr({ "disabled": "disabled", "alt": "Refresh the page to initiate another transfer", "title": "Refresh the page to initiate another transfer" });

                                    $(this).dialog("close");
                                    $(document).scrollTop(0);
                                },
                                Cancel: function () {

                                    $(this).dialog("close");
                                    $(document).scrollTop(0);
                                }
                            }
                        });
                        $("#ConfirmationDialog").dialog("open");

                    } else {
                        $(document).scrollTop(0);
                    }

                }); //end of submit button click function
            } //end PrepTransfer()

            function GetSerialNumberSearchForTransfer(Account) {
                jQuery.ajax({
                    url: "WebService.asmx/SerialNumberSearchForTransfer",
                    type: "POST",
                    data: "{Account:'" + Account + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {

                        ResultsListDiv.innerHTML = data.d

                        //ButtonPlaceHolder is a div which is populated with the transfer button if this ajax call comes back successful
                        ButtonPlaceHolder.innerHTML = "<input type='button' id='submit' value='Transfer' />"

                        PrepTransfer();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetSerialNumberSearch() function

            function GetAddTransfer(TransferNumber, FromAccount, ToAccount, TransferDate) {
                jQuery.ajax({
                    url: "WebService.asmx/AddTransfer",
                    type: "POST",
                    data: "{TransferNumber:'" + TransferNumber + "', FromAccount:'" + FromAccount + "', ToAccount:'" + ToAccount + "', TransferDate:'" + TransferDate + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {

                        DivTopMessage.innerHTML = data.d;

                        GetActiveTransfers();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetAddTransfer() function

            function GetAddTransfersList(TransferNumber, SerialNumber) {
                jQuery.ajax({
                    url: "WebService.asmx/AddTransfersList",
                    type: "POST",
                    data: "{TransferNumber:'" + TransferNumber + "', SerialNumber:'" + SerialNumber + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {

                        DivTopMessage.innerHTML = data.d

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetAddTransfersList() function

            function GetConfirmTransfer(TransferNumber, Cancelled) {
                jQuery.ajax({
                    url: "WebService.asmx/ConfirmTransfer",
                    type: "POST",
                    data: "{TransferNumber:'" + TransferNumber + "', 'Cancelled':'" + Cancelled + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {

                        if (data.d.IsError == "True") {
                            //DivTopMessage.innerHTML = data.d.ErrorMessage;
                            ThrowError("#DivTopMessage", data.d.str + data.d.ErrorMessage);
                        } else {
                            //DivTopMessage.innerHTML = data.d.str;
                            ThrowSuccess("#DivTopMessage", data.d.str + data.d.ErrorMessage);
                        }

                        GetActiveTransfers();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetConfirmTransfer() function

            function GetTransferList(TransferNumber) {
                jQuery.ajax({
                    url: "WebService.asmx/TransferList",
                    type: "POST",
                    data: "{TransferNumber:'" + TransferNumber + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {

                        TransferItemsDiv.innerHTML = data.d

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetSerialNumberSearch() function

            //this function is called on page load and after various actions on the page are executed.
            function GetActiveTransfers() {
                jQuery.ajax({
                    url: "WebService.asmx/ActiveTransfers",
                    type: "POST",
                    data: "",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    //async has to be set to false in this ajax call so that the page won't finish loading before this content is received
                    //if the content doesn't post back before the page loads, the html elements are not accessable via jQuery
                    async: false,
                    success: function (data) {

                        if (data.d.IsError == true) {
                            ActiveTransfersDiv.innerHTML = data.d.ErrorMessage
                        } else {
                            ActiveTransfersDiv.innerHTML = data.d.str
                        }

                        $(".MainStyle .ViewTransferItems").click(function () {
                            //alert($(this).attr("id"));
                            GetTransferList($(this).attr("id"));
                            $("#TransferItemsDiv").show();
                        });

                        $("[id^='TransferedInAim']").click(function () {
                            var TransferValue;

                            //alert($(this).attr("id")); //e.g. - TransferedInAimYes_207 OR TransferedInAimNo_207
                            var TransferedInAim = $(this).attr("id");
                            var RecordID;
                            //Replace "TransferedInAim" with nothing to give a "Yes_207" or "No_207" type of value
                            //then we can call a webservice to update the value true/false accordingly

                            //alert($(this).attr("name")); //e.g. - TransferedInAim_46
                            //the "46" above represents the transfer number
                            var TransferNumber = $(this).attr("name");
                            TransferNumber = TransferNumber.replace("TransferedInAim_", "");
                            //alert(TransferNumber); //e.g. 46

                            var HoverMessage = "You must select the 'Yes' option before this transfer may be completed";

                            if (TransferedInAim.indexOf("Yes") >= 0) {
                                //alert("The Yes radio button was clicked");
                                RecordID = TransferedInAim.replace("TransferedInAimYes_", "");
                                TransferValue = "True"
                                $("#TransferCompleteButton_" + TransferNumber).removeAttr("disabled").removeAttr("alt").removeAttr("title");
                            } else {
                                //alert("The No radio button was clicked");
                                RecordID = TransferedInAim.replace("TransferedInAimNo_", "");
                                TransferValue = "False"
                                $("#TransferCompleteButton_" + TransferNumber).attr({ "disabled": "disabled", "alt": HoverMessage, "title": HoverMessage });
                            }

                            //alert("Record ID: " + RecordID + " - TransferNumber: " + TransferNumber + " - Transfer Value: " + TransferValue);
                            GetTransferedInAim(RecordID, TransferNumber, TransferValue);
                        });

                        $.each($("[id^='TransferedInAimNo_']"), function () {
                            //alert($(this).attr("id")); //e.g. - TransferedInAimNo_207
                            //the "207" above represents the record id

                            //alert($(this).attr("name")); //e.g. - TransferedInAim_46
                            //the "46" above represents the transfer number
                            var TransferNumber = $(this).attr("name");
                            TransferNumber = TransferNumber.replace("TransferedInAim_", "");
                            //alert(TransferNumber); //e.g. 46

                            //alert($(this).attr("checked")); //e.g. "checked" or "undefined"
                            //if the radio button is not selected when the page loads, it will not have
                            //a "checked" attribute and will be returned as undefined

                            if ($(this).prop("checked")) {
                                var HoverMessage = "You must select the 'Yes' option before this transfer may be completed";
                                //alert($("#TransferCompleteButton_" + TransferNumber).attr("id")); //e.g. - TransferCompleteButton_46
                                $("#TransferCompleteButton_" + TransferNumber).attr({ "disabled": "disabled", "alt": HoverMessage, "title": HoverMessage });
                            }
                        });

                        $("[id^='TransferCompleteButton_']").click(function () {
                            var TransferNumber = $(this).attr("id").replace("TransferCompleteButton_", "");
                            //alert(TransferNumber);
                            GetConfirmTransfer(TransferNumber, "False");
                            //Reference: TransferNumber, Cancelled
                        });

                        $("[id^='TransferCancelledButton_']").click(function () {
                            var TransferNumber = $(this).attr("id").replace("TransferCancelledButton_", "");
                            //alert(TransferNumber + " cancelled!");
                            GetConfirmTransfer(TransferNumber, "True");
                            //Reference: TransferNumber, Cancelled
                        });

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetActiveTransfers() function

            function GetTransferedInAim(RecordNumber, TransferNumber, TransferValue) {
                jQuery.ajax({
                    url: "WebService.asmx/TransferedInAim",
                    type: "POST",
                    data: "{RecordNumber:'" + RecordNumber + "', TransferNumber:'" + TransferNumber + "', TransferValue:'" + TransferValue + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: false,
                    beforeSend: function () {
                        LoaderGraphic("blankContainer");
                    },
                    success: function (data) {
                        if (data.d.IsError == true) {
                            ThrowError("#DivTopMessage", data.d.ErrorMessage);
                        } else {

                        }
                    },
                    complete: function () {
                        KillLoaderGraphic();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetTransferedInAim() function

        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />

    <div id="DivTopMessage" class="ValidateTips"></div>
    <br />

    <div id="InitiateTransfer" style="" class="lightGrayBorder">
        <table>
            <tr>
                <td colspan="2">
                    <div id="TransferHeading" class="" runat="server"></div>
                    <hr />
                </td>
            </tr>
            <tr>
                <td><b>*From Account:</b></td>
                <td><div id="FromAccountDiv" runat="server" style=""></div></td>
            </tr>
            <tr>
                <td><b>*To Account:</b></td>
                <td><div id="ToAccountDiv" runat="server" style=""></div></td>
            </tr>
            <tr>
                <td><b>*Date:</b></td>
                <td><input type="text" id="TransferDate" size="10" /></td>
            </tr>
        </table>
        <div id="ResultsListDiv" class="" style=""></div>
    </div>

    <div id="ActiveTransfersDiv" class="lightGrayBorder"></div>
    <div id="TransferItemsDiv" class="lightGrayBorder"></div>
    
    <div id="ClearBothDiv" style="clear:both;"></div>
    <div id="ConfirmationDialog" style="display:none;"></div>

    </form>

</body>
</html>
