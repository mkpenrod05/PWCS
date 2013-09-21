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
        
        If Not Request.QueryString("SerialNumber") = "0000" Then
            serialNumber.Value = Request.QueryString("SerialNumber")
        End If

    End Sub        
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Maintenance")%></title>

    <UserControl:SourceFiles runat="server" />

    <style type="text/css">
        .FormLabel { text-align:right; padding-right:5px; font-weight:bold; font-size:1em; }
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            GetMaintenanceBadActors();

            $("#submit").button();

            function validateForm() {
                var ValidateTips = $(".ValidateTips");
                var invoiceNumber = $("#invoiceNumber");
                var serialNumber = $("#serialNumber");
                var description = $("#description");
                var dateOfAction = $("#dateOfAction");
                var cost = $("#cost");
                var bValid = true;
                //var count = 0
                //var errorMessage = "The following fields are missing a value "

                var AllFields = $([]).add(invoiceNumber)
                                     .add(serialNumber)
                                     .add(description)
                                     .add(dateOfAction)
                                     .add(cost);

                //ValidateTips is set in js/validation.js
                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");

                bValid = bValid && CheckLength(serialNumber, 1, 30, "The Serial Number field must contain a value!");
                bValid = bValid && CheckLength(invoiceNumber, 1, 20, "Invoice Number must be between 1 - 20 characters in length.");
                bValid = bValid && IsNumeric(invoiceNumber, "Invoice Number field must contain only numbers!");
                bValid = bValid && CheckLength(description, 3, 300, "Please enter a description of the maintenance actions performed on this asset!");
                bValid = bValid && CheckLength(dateOfAction, 1, 100, "Please enter a date for this maintenance action!");
                bValid = bValid && CheckLength(cost, 1, 50, "Please enter the cost associated with this maintenance action!");
                bValid = bValid && IsNumeric(cost, "The cost field may contain only numbers!");

                if (bValid) {
                    ConfirmFormSubmit($.trim(serialNumber.val()), $.trim(invoiceNumber.val()), $.trim(description.val()), $.trim(dateOfAction.val()), $.trim(cost.val()));

                }

            } //end of validateForm() 

            $(function () {
                $("#dateOfAction").datepicker({
                    showButtonPanel: true,
                    closeText: "Cancel"
                });
            });

            $("#submit").click(function () {
                validateForm();
            });

            function ConfirmFormSubmit(serialNumber, invoiceNumber, description, dateOfAction, cost) {
                $("#ConfirmFormSubmitDiv").html(
                    "<div class='BlackOnWhite' style='text-align:center;'>" +
                        "<p>Submit a maintenance action for: </p>" +
                        "<p><span class='SpanStrong'>" + serialNumber + "</span> ?</p>" + 
                    "</div>"
                );
                $("#ConfirmFormSubmitDiv").dialog({
                    autoOpen: false,
                    height: 200,
                    width: 300,
                    modal: true,
                    title: "Submit Maintenance Action?",
                    buttons: {
                        "Submit": function () {
                            addMaintenanceAction(serialNumber.toUpperCase(), invoiceNumber, description, dateOfAction, cost);
                            $("#serialNumber").val("");
                            $(this).dialog("close");
                        },
                        Cancel: function () {
                            $(this).dialog("close");
                        }
                    }
                });
                $("#ConfirmFormSubmitDiv").dialog("open");
            }

            function addMaintenanceAction(serialNumber, invoiceNumber, description, dateOfAction, cost) {
                jQuery.ajax({
                    url: "WebService.asmx/addMaintenanceAction",
                    type: "POST",
                    data: "{invoiceNumber:'" + invoiceNumber + "', serialNumber:'" + serialNumber + "', description:'" + description + "', dateOfAction:'" + dateOfAction + "', cost:'" + cost + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {
                        AddMaintenanceResults.innerHTML = data.d
                        // 
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetResults() function

            function GetMaintenanceBadActors() {
                jQuery.ajax({
                    url: "WebService.asmx/MaintenanceBadActors",
                    type: "POST",
                    data: "",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    success: function (data) {
                        MaintenanceHistory.innerHTML = data.d
                        // 
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        //$("#divtopMessage").append("<p>Error: " + UserName + " failed to update. </p>");
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetResults() function

        });    
    
    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />

    <div id="DivTopMessage"></div>
    <br />
    <div style="float:left;">
    <div style="padding:10px; margin-right:20px;" class="lightGrayBorder">
        <div style="text-align:center;"><h3>Add A Maintenance Action</h3><hr /></div>
        <div id="FormErrors" class="ValidateTips" style="padding:5px;"></div>        
        <br />
        <table>
            <tr>
                <td class="FormLabel">*Serial Number:</td>
                <td><input type="text" id="serialNumber" runat="server" /></td>
            </tr>
            <tr>
                <td class="FormLabel">*Invoice Number:</td>
                <td><input type="text" id="invoiceNumber" maxlength="20" runat="server" /></td>
            </tr>
            <tr>
                <td class="FormLabel">*Description:</td>
                <td><textarea id="description" rows="5" cols="50" runat="server"></textarea></td>
            </tr>
            <tr>
                <td class="FormLabel">*Date:</td>
                <td><input type="text" id="dateOfAction" runat="server" /></td>
            </tr>
            <tr>
                <td class="FormLabel">*Cost:</td>
                <td><input type="text" id="cost" runat="server" /></td>
            </tr>
            <tr>
                <td></td>
                <td><input type="button" id="submit" value="Submit" runat="server" /></td>
            </tr>
        </table>

    </div>
    <div id="AddMaintenanceResults" style="padding:20px 10px 20px 10px"></div>
    </div>
     
    <div id="MaintenanceHistory" class="lightGrayBorder" style="float:left; padding:10px;"></div>

    <div id="ClearBothDiv" style="clear:both;"></div>
    
    
    <!--Dialog Div's START-->
        <div id="ConfirmFormSubmitDiv" style="display:none;"></div>
    <!--Dialog Div's END-->
    </form>

</body>
</html>
