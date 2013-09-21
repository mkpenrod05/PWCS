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
        
        Dim SearchInput As String = Request.Form("SearchInput")
        Dim SearchType As String = Request.Form("SearchType")
        'If SearchInput = "" Then
        '    SearchResultsDiv.InnerHtml = "<p>Please enter a search term...</p>"
        'Else
        '    SearchResultsDiv.InnerHtml = CustomFunctions.CustomFieldSearch("serialNum", "dbo.assets", SearchType, SearchInput.Trim())
        'End If
        'SearchResultsDiv.InnerHtml = CustomFunctions.CustomFieldSearch("serialNum", "dbo.assets", "serialNum", "320CEQ")
        'SearchResultsDiv.InnerHtml = SearchInput
        
    End Sub
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Archive")%></title>

    <UserControl:SourceFiles runat="server" />

    <style type="text/css">

        span { cursor:default; }
        img { border:1px solid #555; }
        
        /*
        #VerifyPastedData { color:Blue; cursor:pointer; }
        #VerifyPastedData:hover { color:Orange; }
        */

        #PastedDataTable { /*width:660px;*/ }
        #PastedDataTable th { color:#777; text-align:center; }
        #PastedDataTable td { padding-right:2px; }
        #PastedDataHeading td.cc { text-align:center; width:22%; }
        #PastedDataHeading td.mn { text-align:center; width:25%; }
        #PastedDataHeading td.sn { text-align:center; width:25%; }
        #PastedDataHeading td.md { text-align:center; width:25%; }
        #ArchiveInstructions p { font-weight:bold; }
        #DivTopMessage { padding:5px; }
        
        .InputTitle { font-weight:bold; }
        .Link { color:Blue; }
        .Link:hover { color:Red; cursor:pointer; }
        
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            //disable the submit button on page load
            $("#SubmitButton").attr({
                "disabled": "disabled",
                "alt": "You must verify the data before submission",
                "title": "You must verify the data before submission"
            });

            //dialog options - http://docs.jquery.com/UI/API/1.8/Dialog
            $("#ArchiveInstructions").dialog({
                autoOpen: false,
                height: 800,
                width: 850,
                //modal: true,
                //hide: "slide",
                position: "right",
                title: "Archive Instructions"
            });

            $("#Instruction_AIMInventoryAdjustedData").click(function () {
                $("#ArchiveInstructions").dialog("open");
            });

            $("#VerifyPastedData").click(function () {
                var bValid = true;
                var ValidateTips = $(".ValidateTips");

                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                bValid = bValid && CheckLength($("#PastedData"), 1, 50000, "Please enter valid AIM Adjustment Inventory data copied from AIM or the excel spreadsheet!");

                if (bValid) {

                    //LoaderGraphic() is defined in validation.js
                    LoaderGraphic("blankContainer");

                    setTimeout(function () {
                        var text = $("#PastedData").text();
                        var eachLine = text.split(/\t[1]{2}|\s{1,}[1]{2}/);
                        //alert("Number of lines is: " + eachLine.length);
                        $("#PastedDataDiv").empty();

                        $("#PastedDataDiv").append(
                            "<table id='PastedDataTable'><tr><th>Cage Code</th><th>Model Number</th><th>Serial Number</th><th>Model Description</th></tr></table>"
                        );

                        for (var i = 0; i < eachLine.length; i++) {
                            //alert("Line " + (i + 1) + " : " + $.trim(eachLine[i]));
                            if ($.trim(eachLine[i]).length > 1) {
                                var eachValue = $.trim(eachLine[i]).split(/\t|\s{2}|[\s]*,[\s]*/);
                                //$("#DivTopMessage").append("Serial Number: " + eachValue[2] + "<br />"); - This works

                                //$("#PastedDataDiv").append("<input type='text' id='CageCode_Row" + i + "_" + eachValue[0] + "' value='" + eachValue[0] + "' class='PastedDataClass' \> - ");
                                //$("#PastedDataDiv").append("<input type='text' id='ModelNumber_Row" + i + "_" + eachValue[1] + "' value='" + eachValue[1] + "' class='PastedDataClass' \> - ");
                                //$("#PastedDataDiv").append("<input type='text' id='SerialNumber_Row" + i + "_" + eachValue[2] + "' value='" + eachValue[2] + "' class='PastedDataClass' \> - ");
                                //$("#PastedDataDiv").append("<input type='text' id='ModelDescription_Row" + i + "_" + eachValue[3] + "' value='" + eachValue[3] + "' class='PastedDataClass' \>");
                                //$("#PastedDataDiv").append("<div id='Status_" + eachValue[2] + "' class='prepCheck' style='float:right; border:0px solid #ccc;'></div><br />");

                                $("#PastedDataTable tbody").append(
                                    "<tr>" +
                                    "<td><input type='text' id='CageCode_Row" + i + "_" + eachValue[0] + "' value='" + eachValue[0] + "' class='PastedDataClass' \></td>" +
                                    "<td><input type='text' id='ModelNumber_Row" + i + "_" + eachValue[1] + "' value='" + eachValue[1] + "' class='PastedDataClass' \></td>" +
                                    "<td><input type='text' id='SerialNumber_Row" + i + "_" + eachValue[2] + "' value='" + eachValue[2] + "' class='PastedDataClass' \></td>" +
                                    "<td><input type='text' id='ModelDescription_Row" + i + "_" + eachValue[3] + "' value='" + eachValue[3] + "' class='PastedDataClass' \></td>" +
                                    "<td><div style='float:right; border:0px solid #ccc;'>" +
                                            "<div id='Status_" + i + "' style='float:left;' class='prepCheck'></div>" +
                                            "<div id='Status_Message_" + i + "' class='' style='position:relative; top:2px;'></div>" +
                                        "</div></td>" +
                                    "</tr>"
                                );

                            } //end if

                        } //end for

                        $("#SubmitButton").removeAttr("disabled").removeAttr("alt").removeAttr("title");
                    }, 2000); //end setTimeout()
                }
            }); //end of #VerifyPastedData .click() function

            $("#SubmitButton").click(function () {
                SumbitForm();
            }); //end of #SubmitButton .click() function

            function SumbitForm() {

                var ValidateTips = $(".ValidateTips");
                var AIMTransactionNumber2 = $("#AIMTransactionNumber2");
                var ArchiveReason2 = $("#ArchiveReason2");
                var bValid = true;

                var AllFields = $([]).add(AIMTransactionNumber2).add(ArchiveReason2);

                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");

                bValid = bValid && CheckLength(AIMTransactionNumber2, 3, 50, "Please enter a valid AIM transaction number! - example: FU202920120919130712000444");
                bValid = bValid && CheckLength(ArchiveReason2, 1, 3000, "Archive Reason required!  Maximum of 3000 characters allowed. - example: DRMO 17 Nov 2012 (DTID: FB20270321Z3202)");

                if (bValid) {
                    $("[id^='SerialNumber_']").each(function (x) {
                        if (bValid) {
                            var ValidateTips = $(".ValidateTips");
                            var bValid2 = true;
                            var bad = false;
                            var CageCode = $("[id^='CageCode_Row" + x + "']");
                            var ModelNumber = $("[id^='ModelNumber_Row" + x + "']");
                            var SerialNumber = $("[id^='SerialNumber_Row" + x + "']");
                            var ModelDescription = $("[id^='ModelDescription_Row" + x + "']");
                            var AllFields = $([]).add(CageCode).add(ModelNumber).add(SerialNumber).add(ModelDescription);

                            ValidateTips.removeClass("ui-state-error");
                            ValidateTips.removeClass("ui-state-highlight");
                            ValidateTips.text("");

                            AllFields.removeClass("ui-state-highlight");
                            AllFields.removeClass("ui-state-error");

                            bValid2 = bValid2 && CheckLength(CageCode, 5, 50, "Cage Code value must be at least 5 characters in length! - example: 7H907");
                            bValid2 = bValid2 && CheckForUndefinedValues(CageCode, "Cage Code cannot be undefined! - example: 7H907");

                            bValid2 = bValid2 && CheckLength(ModelNumber, 3, 50, "Model Number must be at least 3 characters in length! - example: M20QSS9PW1AN");
                            bValid2 = bValid2 && CheckForUndefinedValues(ModelNumber, "Model Number cannot be undefined! - example: M20QSS9PW1AN");

                            bValid2 = bValid2 && CheckLength(SerialNumber, 2, 50, "Serial Number must be at least 3 characters in length! - example: 320CEG1545");
                            bValid2 = bValid2 && CheckForUndefinedValues(SerialNumber, "Serial Number cannot be undefined! - example: 320CEG1545");

                            bValid2 = bValid2 && CheckLength(ModelDescription, 3, 50, "Model Description must be at least 3 characters in length! - example: XTS 5000 III");
                            bValid2 = bValid2 && CheckForUndefinedValues(ModelDescription, "Model Description cannot be undefined! - example: XTS 5000 III");

                            if (bValid2) {
                                //alert("Row " + (x + 1) + " passed!");
                                bValid = bValid && true;
                            } else {
                                //alert("Row " + (x + 1) + " failed!");
                                bValid = bValid && false;
                            }
                        } //end of bValid2 check which checks each input field in the pasted data section after the verify data check has been clicked
                    }); //end of pasted data .each()

                    if (bValid) {

                        var Elements = $("#PastedDataTable").find($("[id^='SerialNumber_']"));
                        var Length = Elements.length;
                        //alert(Length);
                        var Index = 0;
                        var Timer = setInterval(EachRow, 750);

                        function EachRow() {
                            var RowID = $("#Status_" + Index).attr("id");
                            RowID = RowID.replace("Status_", "");

                            GetArchiveAsset(
                                $("[id^='SerialNumber_Row" + RowID + "']").val().toUpperCase(),
                                $("#ArchiveReason2").val(),
                                $("#AIMTransactionNumber2").val().toUpperCase(),
                                $("[id^='CageCode_Row" + RowID + "']").val().toUpperCase(),
                                $("[id^='ModelNumber_Row" + RowID + "']").val().toUpperCase(),
                                $("[id^='ModelDescription_Row" + RowID + "']").val().toUpperCase(),
                                RowID
                            );

                            Index++;

                            if (Index >= Length) {
                                clearInterval(Timer);
                                ThrowSuccess("#DivTopMessage", "All data has been processed!");
                            }
                        }

                        $("#SubmitButton").attr({ "disabled": "disabled", "alt": "Data has been processed!", "title": "Data has been processed!" });

                    } //end of final bValid check which will submit the data to the database
                } //end of 1st bValid check which checks to make sure a value has been entered into all 3 input fields
            } //end SubmitForm()

            ///////////////////////////////////////////////AJAX CALLS START///////////////////////////////////////////////

            function GetArchiveAsset(SerialNumber, ArchiveReason, AIMTransactionNumber, AIMCageCode, AIMModelNumber, AIMModelDescription, RowID) {
                //alert(SerialNumber + " : " + ArchiveReason + " : " + AIMTransactionNumber + " : " + AIMCageCode + " : " + AIMModelNumber + " : " + AIMModelDescription + " : " + RowID);

                /*Added on 08 Mar 2013 - MCP*/
                SerialNumber = ReplaceSpecialCharacters(SerialNumber);
                ArchiveReason = ReplaceSpecialCharacters(ArchiveReason);
                AIMTransactionNumber = ReplaceSpecialCharacters(AIMTransactionNumber);
                AIMCageCode = ReplaceSpecialCharacters(AIMCageCode);
                AIMModelNumber = ReplaceSpecialCharacters(AIMModelNumber);
                AIMModelDescription = ReplaceSpecialCharacters(AIMModelDescription);

                jQuery.ajax({
                    url: "WebService.asmx/ArchiveAsset",
                    type: "POST",
                    data: "{SerialNumber:'" + SerialNumber + "', ArchiveReason:'" + ArchiveReason + "', AIMTransactionNumber:'" + AIMTransactionNumber + "', AIMCageCode:'" + AIMCageCode + "', AIMModelNumber:'" + AIMModelNumber + "', AIMModelDescription:'" + AIMModelDescription + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: false, //stops other processes until this completes
                    success: function (data) {

                        $("#DivTopMessage").removeClass();
                        $("#DivTopMessage").html("");

                        if (data.d.IsError == false) {
                            DivTopMessage.innerHTML = data.d.str;
                            $("#DivTopMessage").addClass("ui-state-validated");

                            $("#Status_" + RowID).removeClass("prepCheck");
                            $("#Status_" + RowID).addClass("validCheck");
                            //$("#Status_Message_" + RowID).append(" - " + data.d.str);
                            $("#Status_Message_" + RowID).append(" - Successfully Archived");
                        } else {
                            DivTopMessage.innerHTML = data.d.ErrorMessage;
                            $("#DivTopMessage").addClass("ui-state-error");

                            $("#Status_" + RowID).removeClass("prepCheck");
                            $("#Status_" + RowID).addClass("invalidCheck");
                            //$("#StatusMessage tbody").append("<tr><td>" + SerialNumber + "</td><td>" + data.d.ErrorMessage + "</td></tr>");
                            $("#Status_Message_" + RowID).append(" - " + data.d.ErrorMessage);
                        }

                        //alert(SerialNumber + " - " + RowID);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        //alert('Error: ' + xhr.status + ' ' + thrownError);
                        $("#DivTopMessage").empty();
                        $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
                        $("#Status_" + RowID).removeClass("prepCheck");
                        $("#Status_" + RowID).addClass("invalidCheck");
                        //$("#StatusMessageTable tbody").append("<tr><td>" + SerialNumber + "</td><td>" + xhr.status + ' ' + thrownError + "</td></tr>");
                        $("#Status_Message_" + RowID).append(" - " + xhr.status + ' ' + thrownError);
                    },
                    statusCode: {
                        404: function () {
                            alert('WebService Not Found!');
                        }
                    }
                });
            } //end of GetArchiveAsset() ajax call

            ///////////////////////////////////////////////AJAX CALLS END///////////////////////////////////////////////

        });                                                                    //end document.ready

    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />

    <div id="DivTopMessage" class="ValidateTips"></div>

    <div style="margin-left:10px;">
        <p class="InputTitle">
            *AIM Transaction Number:
            &nbsp;
            <span class="SmallText">(e.g. FU202920120919130712000444 or use "Unknown")</span>
        </p>
        <div><input type='text' id='AIMTransactionNumber2' class='' size="30" /></div>
        <br />
        <p class="InputTitle">
            *AIM Inventory Adjusted Data:
            &nbsp;
            <span class="SmallText Link" style="">
                <a id="Instruction_AIMInventoryAdjustedData">What's this</a>
            </span>
            <br />
            <span class="SmallText">Reference for pasted data: Cage Code, Model Number, Serial Number, Model Description, 11</span>
        </p>
        <%--<div id="PastedDataDiv" class="blankContainer" style="width:690px; float:left;">--%>
        <div id="PastedDataDiv" class="blankContainer" style="float:left;">
            <textarea id="PastedData" rows="10" cols="83"></textarea>
            <br />
            <input type="button" id="VerifyPastedData" class="load" value="Verify Data" />
        </div>
        
        <div style="clear:both;"></div>
        <br />
        <p class="InputTitle">*Archive Reason:</p>
        <div><textarea id="ArchiveReason2" rows="5" cols="50"></textarea></div>
        <br />
        <div class="Seperator"></div>
        <br />
        <div><input type="button" id="SubmitButton" value="Archive" class="SubmitButton"/></div>
    </div>

    <div id="ArchiveInstructions">
        <p style="text-align:center; font-size:20px;">Archive Instructions</p>
        <div class="Seperator"></div>
        <br />
        <p>
            The "AIM Inventory Adjusted Data" field was set up to allow you to copy and paste AIM adjusted data which will then 
            be archived in the PWCS website database.  When performing an "Inventory Adjust" in AIM, you will be presented with a
            table of asset data that was just removed.  STOP AT THIS POINT!  Do not close the AIM browser window, this data needs to 
            be saved.   
        </p>
        <p>
            This is what the AIM Inventory Adjust page looks like:<br />
            <img src="images/instructions/AIMInventoryAdjustData.JPG" alt="" />
        </p>
        <p>
            The data above may be copied and put directly into the "AIM Inventory Adjusted Data" field OR copied and pasted into an 
            Excel spreadsheet.  Here is an example of the copied AIM Inventory Adjust data:<br />
            <img src="images/instructions/AIMInventoryAdjustData_Selected.JPG" alt="" />
        </p>
        <p>
            Here is the data pasted into the "AIM Inventory Adjusted Data" field:<br />
            <img src="images/instructions/AIMInventoryAdjustData_PastedIn.JPG" alt="" />
        </p>
        <p>
            The "Verify Data" button must be clicked before you are allowed to submit this archive request.  This is done to make sure
            the data being archived is accurate.  At this point you may click the "Verify Data" button:<br />
            <img src="images/instructions/VerifyData_Clicked.JPG" alt="" />
        </p>
        <p>
            You should see a result similiar to this:<br />
            <img src="images/instructions/VerifyData_Complete.JPG" alt="" />
        </p>
        <p>
            As stated above, the AIM Inventory Adjust data may be copied into an Excel spreadsheet for additional tracking:<br />
            <img src="images/instructions/ExcelData.JPG" alt="" />
        </p>
        <p>
            Copy the data from the Excel spreadsheet:<br />
            <img src="images/instructions/ExcelDataCopied.JPG" alt="" />
        </p>
        <p>
            Paste it into the "AIM Inventory Adjusted Data" field and click "Verify Data":<br />
            <img src="images/instructions/ExcelData_Pasted.JPG" alt="" />
        </p>
        <p>
            As a third and final option, the AIM Inventory Adjust data may be manually entered into the "AIM Inventory Adjusted Data" 
            field with each value being separated by a comma:<br />
            <img src="images/instructions/ManuallyEnteredData.JPG" alt="" />
        </p>
        <p>
            After you have selected one of the three options listed above, you are ready to archive the data:<br />
            <img src="images/instructions/ReadyToArchive.JPG" alt="" />
        </p>
    </div>
<!--Dialog Form Div's END-->


    </form>
</body>
</html>
