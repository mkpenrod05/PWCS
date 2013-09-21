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

    End Sub
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Modifications")%></title>

    <UserControl:SourceFiles runat="server" />

    <style type="text/css">
        /* SINGLE PAGE OVERRIDES */
        
        span { cursor:default; }
        
        #DivTopMessage { padding:5px; }
        
        /* END SINGLE PAGE OVERRIDES */
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            var Valid_TrunkID = false;
            var Valid_TrunkID_str = "";
            var Valid_SerialNumber = false;
            var Valid_SerialNumber_str = "";

            $("#AccountFormSubmit").button().click(function () {
                SubmitAccountForm();
            });

            $("#AssetFormSubmit").button().click(function () {
                SubmitAssetForm();
            });

            $(function () {
                $("#tabs").tabs();
            });

            $("#CopyAssetCheckbox").click(function () {
                if ($("#CopyAssetCheckbox").is(":checked")) {
                    $("#CopyAssetDialog").dialog("open");
                }
            });

            $("#CopyAssetDialog").dialog({
                autoOpen: false,
                modal: true,
                height: 200,
                width: 400,
                title: "Copy Existing Asset",
                buttons: {
                    "Search": function () {
                        var InputValue = $.trim($("#CopyAssetInput").val());
                        if (InputValue == "") {
                            ThrowError("#CopyAssetMessage", "Please enter a serial number!");
                        } else {
                            GetCopyAsset(InputValue);
                            $(this).dialog("close");
                        }
                    },
                    Cancel: function () {
                        $("#CopyAssetCheckbox").prop("checked", false);
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $("#CopyAssetCheckbox").attr("checked", false);
                }
            }); //end "#CopyAssetDialog" dialog

            $(function () {
                //this function changes the color of the text located above the hardware checkbox items on the "New Asset" tab
                $("[id^='Checkbox_']").hover(function () {
                    var ID = $(this).attr("id");
                    ID = ID.replace("Checkbox_", "");
                    $("#Label_" + ID).toggleClass("ColorHover");
                });

                //this function changes the color of the Hardware Module Name (e.g. "9600B") on the "New Asset" tab
                $("[id^='HM-Item_']").hover(function () {
                    var ID = $(this).attr("id");
                    //alert(ID); //e.g. "HM-Item_9600B"
                    ID = ID.replace("HM-Item_", "");
                    $("#Span_" + ID).toggleClass("ColorHover");
                });
            });

            //this function sets the Trunk ID input field to disabled if the user checks the 
            //"Unknown" checkbox to the right of the input. 
            $(function () {
                $("#TrunkID_Checkbox").change(function () {
                    if ($(this).is(":checked")) {
                        $("#TrunkID").attr("disabled", "disabled");
                    } else {
                        $("#TrunkID").removeAttr("disabled");
                    }
                });
            });

            //this function sets the Cost input fields to disabled if the user checks the 
            //"Unknown" checkbox to the right of the input.
            $(function () {
                $("#Cost_Checkbox").change(function () {
                    if ($(this).is(":checked")) {
                        $("#Cost").attr("disabled", "disabled");
                        $("#Cost_2").attr("disabled", "disabled");
                    } else {
                        $("#Cost").removeAttr("disabled");
                        $("#Cost_2").removeAttr("disabled");
                    }
                });
            });

            //this function makes an ajax call to check and see if the Trunk ID value entered by the user
            //exists in the database and is available for use
            $("#TrunkID").blur(function () {
                $("#TrunkID").removeClass();
                $("#TrunkID_Message").removeClass();
                $("#TrunkID_Message").text("");
                if ($(this).val().length >= 1) {
                    GetCheckTrunkIDAvailable($("#TrunkID").val(), "TrunkID");
                }
            }); //end TrunkID blur function

            //this function checks to see if the serial number entered already exists in the database
            //in this case it is bad if it is found
            $("#SerialNumber").blur(function () {
                $("#SerialNumber").removeClass();
                $("#SerialNumber_Message").removeClass();
                $("#SerialNumber_Message").text("");
                if ($(this).val().length >= 1) {
                    GetCheckSerialNumberAvailable($("#SerialNumber").val(), "SerialNumber", "Bad");
                }
            }); //end SerialNumber blur function

            function SubmitAccountForm() {
                var ValidateTips = $(".ValidateTips");
                var AccountCode = $("#AccountCode");
                var Unit = $("#Unit");
                var AccountComments = $("#AccountComments");
                var bValid = true;

                var AllFields = $([]).add(AccountCode).add(Unit).add(AccountComments);

                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");

                bValid = bValid && CheckLength(AccountCode, 4, 4, "Account Code must be 4 characters in length!"); //CheckLength() is a global function - js/validation.js
                bValid = bValid && CheckLength(Unit, 2, 50, "Please enter a unit for this account (2 characters minimum)!"); //CheckLength() is a global function - js/validation.js
                bValid = bValid && CheckLength(AccountComments, 0, 2000, "2000 character max length for the Account Comments field!"); //CheckLength() is a global function - js/validation.js

                if (bValid) {
                    //alert("Form submitted!");
                    AccountCode = $.trim(AccountCode.val());
                    Unit = $.trim(Unit.val());
                    AccountComments = $.trim(AccountComments.val());

                    GetAccountAddition(AccountCode, Unit, AccountComments);
                } //end if bValid
            } //end SubmitAccountForm

            function SubmitAssetForm() {
                //
                //alert("Valid_TrunkID: " + Valid_TrunkID);
                //
                var ValidateTips = $(".ValidateTips");
                var TrunkID = $("#TrunkID");
                var SerialNumber = $("#SerialNumber");
                var ModelNumber = $("#ModelNumber");
                var ModelDescription = $("#ModelDescription");
                var AssetComments = $("#AssetComments");
                var Cost = $("#Cost");
                var Cost2 = $("#Cost_2");
                //var AssetComments = $("#AssetComments");
                var Baud = $("#HM-Item_9600B");
                var AES = $("#HM-Item_AES");
                var OTAR = $("#HM-Item_OTAR");
                var OTAP = $("#HM-Item_OTAP");
                var QueryType = "";
                var bValid = true;

                var AllFields = $([]).add(TrunkID)
                                     .add(SerialNumber)
                                     .add(ModelNumber)
                                     .add(ModelDescription)
                                     .add(AssetComments)
                                     .add(Cost)
                                     .add(Cost2)
                                     .add(AssetComments)
                                     .add(Baud)
                                     .add(AES)
                                     .add(OTAR)
                                     .add(OTAP);

                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");

                //INTERNAL FUNCTIONS - START//
                function ProcessHardwareMods(HardwareObject, HardwareName) {
                    if (!$("[id^='" + HardwareName + "_']").is(":checked")) {
                        //error
                        if (bValid) {
                            bValid = bValid && false;
                            UpdateValidationTips("Please select the appropriate option for the " + HardwareName + " hardware module!");
                            HardwareObject.addClass("ui-state-error");
                        }
                        return false;
                    } else {
                        return true;
                    }
                }
                function SetHardwareValue(HardwareName) {
                    var HardwareValue;
                    $.each($("[id^='" + HardwareName + "_']"), function () {
                        if ($(this).is(":checked")) {
                            HardwareValue = $(this).val();
                        }
                    });
                    return HardwareValue;
                }
                //INTERNAL FUNCTIONS - END//

                if (!$("#TrunkID_Checkbox").is(":checked")) {
                    bValid = bValid && CheckLength(TrunkID, 6, 6, "Trunk ID must be 6 characters in length!"); //CheckLength() is a global function - js/validation.js
                    bValid = bValid && Valid_TrunkID;
                    TrunkID = TrunkID.val();
                    QueryType = "Update";
                } else {
                    TrunkID = "0";
                    QueryType = "Insert";
                }

                bValid = bValid && CheckLength(SerialNumber, 6, 50, "Serial Number must be at least 6 characters in length!"); //CheckLength() is a global function - js/validation.js
                bValid = bValid && CheckLength(ModelNumber, 6, 50, "Model Number must be at least 6 characters in length!"); //CheckLength() is a global function - js/validation.js
                bValid = bValid && CheckLength(ModelDescription, 6, 50, "Model Description must be at least 6 characters in length!"); //CheckLength() is a global function - js/validation.js
                bValid = bValid && CheckLength(AssetComments, 0, 2000, "Asset Comments must not exceed 2000 characters in length!"); //CheckLength() is a global function - js/validation.js

                if (!$("#Cost_Checkbox").is(":checked")) {
                    bValid = bValid && CheckLength(Cost, 1, 4, "Please enter a cost or select 'Unknown'!"); //CheckLength() is a global function - js/validation.js
                    bValid = bValid && IsNumeric(Cost, "Cost field must contain numbers only!"); //IsNumeric() is a global function - js/validation.js
                    bValid = bValid && CheckLength(Cost2, 2, 2, "Please enter a cost or select 'Unknown'!"); //CheckLength() is a global function - js/validation.js
                    bValid = bValid && IsNumeric(Cost2, "Cost field must contain numbers only!"); //IsNumeric() is a global function - js/validation.js
                    Cost = Cost.val() + "." + Cost2.val();
                } else {
                    Cost = 0;
                }

                bValid = bValid && ProcessHardwareMods(Baud, "Baud");
                bValid = bValid && ProcessHardwareMods(AES, "AES");
                bValid = bValid && ProcessHardwareMods(OTAR, "OTAR");
                bValid = bValid && ProcessHardwareMods(OTAP, "OTAP");
                bValid = bValid && Valid_SerialNumber;

                if (bValid) {
                    //alert("Baud: " + Baud);
                    //alert("AES: " + AES);
                    //alert("OTAR: " + OTAR);
                    //alert("OTAP: " + OTAP);
                    //alert("Form Submitted!!!");

                    TrunkID = $.trim(TrunkID);

                    SerialNumber = $.trim(SerialNumber.val());
                    SerialNumber = SerialNumber.toUpperCase();

                    ModelNumber = $.trim(ModelNumber.val());
                    ModelNumber = ModelNumber.toUpperCase();

                    ModelDescription = $.trim(ModelDescription.val());
                    ModelDescription = ModelDescription.toUpperCase();

                    AssetComments = $.trim(AssetComments.val());
                    Cost = $.trim(Cost);

                    Baud = SetHardwareValue("Baud");
                    AES = SetHardwareValue("AES");
                    OTAR = SetHardwareValue("OTAR");
                    OTAP = SetHardwareValue("OTAP");

                    $("#AssetAdditionDialog").html(
                        "<div class='BlackOnWhite' style=''>" +
                            "<h3 style='text-align:center;'>Add the following asset?</h3>" +
                            "<hr />" +
                            "<p><label class='DialogLabel'>Serial Number: </label>" + SerialNumber + "</p>" +
                            "<p><label class='DialogLabel'>Trunking System ID: </label>" + TrunkID + "</p>" +
                            "<p><label class='DialogLabel'>Model Number: </label>" + ModelNumber + "</p>" +
                            "<p><label class='DialogLabel'>Model Description: </label>" + ModelDescription + "</p>" +
                            "<p><label class='DialogLabel'>Comments: </label>" + AssetComments + "</p>" +
                        "</div>"
                    );
                    
                    $("#AssetAdditionDialog").dialog({
                        autoOpen: false,
                        modal: true,
                        //height: 500,
                        width: 500,
                        title: "Are You Sure You Want To Continue?",
                        buttons: {
                            "Add": function () {
                                GetAssetAddition(TrunkID, SerialNumber, ModelNumber, ModelDescription, AssetComments, Cost, Baud, AES, OTAR, OTAP, QueryType);
                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                //do nothing here...
                                $(this).dialog("close");
                            }
                        }, //end buttons
                        close: function () {

                        }
                    }); //end SwapTrunkIDValidDiv dialog
                    
                    $("#AssetAdditionDialog").dialog("open");

                } //end if bValid
            } //end SubmitAccountForm

            //////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////SWAP TRUNK ID PROCESSING START//////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////

            var Valid_SwapTrunkID = false;
            var Valid_SwapTrunkID_str = "";
            var Valid_SwapSerialNumber = false;
            var Valid_SwapSerialNumber_str = "";

            //OldRecordID is set in the GetCheckTrunkIDAvailable() webservice ajax call
            var OldRecordID = "";

            //NewRecordID is set in the GetCheckTrunkIDAvailable() webservice ajax call 
            var NewRecordID = "";

            //OldTrunkID is set in the GetCheckSerialNumberAvailable() webservice ajax call 
            var OldTrunkID = "";

            //this function makes an ajax call to check and see if the Trunk ID value entered by the user
            //exists in the database and is available for use
            $("#SwapTrunkID_TrunkID").blur(function () {
                $("#SwapTrunkID_TrunkID").removeClass();
                $("#SwapTrunkID_TrunkID_Message").removeClass();
                $("#SwapTrunkID_TrunkID_Message").text("");
                if ($(this).val().length >= 1) {
                    GetCheckTrunkIDAvailable($("#SwapTrunkID_TrunkID").val(), "SwapTrunkID_TrunkID");
                }
            }); //end TrunkID blur function

            //this function checks to see if the serial number entered already exists in the database
            //in this case it is good if it is found
            $("#SwapTrunkID_SerialNumber").blur(function () {
                $("#SwapTrunkID_SerialNumber").removeClass();
                $("#SwapTrunkID_SerialNumber_Message").removeClass();
                $("#SwapTrunkID_SerialNumber_Message").text("");
                if ($(this).val().length >= 1) {
                    GetCheckSerialNumberAvailable($("#SwapTrunkID_SerialNumber").val(), "SwapTrunkID_SerialNumber", "Good");
                }
            }); //end SerialNumber blur function

            $("#SwapTrunkIDFormSubmit").button().click(function () {
                SubmitSwapTrunkIDForm();
            });

            function SubmitSwapTrunkIDForm() {
                var ValidateTips = $(".ValidateTips");
                var TrunkID = $("#SwapTrunkID_TrunkID");
                var SerialNumber = $("#SwapTrunkID_SerialNumber");
                var bValid = true;

                var AllFields = $([]).add(TrunkID).add(SerialNumber);

                ValidateTips.removeClass("ui-state-error");
                ValidateTips.removeClass("ui-state-highlight");
                ValidateTips.text("");

                AllFields.removeClass("ui-state-highlight");
                AllFields.removeClass("ui-state-error");

                bValid = bValid && CheckLength(SerialNumber, 3, 25, "Please enter a serial number!");
                bValid = bValid && CheckLength(TrunkID, 6, 6, "Trunk ID must be exactly 6 characters!");
                bValid = bValid && IsNumeric(TrunkID, "Trunk ID field must contain only numbers!");

                bValid = bValid && Valid_SwapSerialNumber;
                bValid = bValid && Valid_SwapTrunkID;

                SerialNumber = CleanObjectValue(SerialNumber, "UpperCase"); //CleanObjectValue() is defined in js/validation.js
                TrunkID = CleanObjectValue(TrunkID, "Ignore"); //CleanObjectValue() is defined in js/validation.js

                if (bValid) {
                    
                    $("#SwapTrunkIDValidDiv").html(
                        "<div class='BlackOnWhite' style='text-align:center;'>Are you sure you want to swap the trunking system ID for serial number <br />" +
                        "<b>" + SerialNumber + "</b> to <b>" + TrunkID + "</b>"
                    );

                    $("#SwapTrunkIDValidDiv").dialog({
                        autoOpen: false,
                        modal: true,
                        height: 200,
                        width: 300,
                        title: "Are You Sure You Want To Continue?",
                        buttons: {
                            "Swap": function () {
                                //NewRecordID is a global variable set in the ajax call from the onBlur new trunk id function
                                //OldRecordID is a global variable set in the ajax call from the onBlur serial number function

                                //need to check the global variables to make sure they were loaded with a record id
                                //both record id's will be needed in order to run the query in the webservice below
                                if (OldRecordID == "" || NewRecordID == "" || OldTrunkID == "" || TrunkID == "" || SerialNumber == "") {
                                    alert("A fatal error has occured!  Please refresh the page and try again.");
                                } else {
                                    GetSwapTrunkID(SerialNumber, OldTrunkID, OldRecordID, TrunkID, NewRecordID);
                                }

                                $(this).dialog("close");
                            },
                            "Cancel": function () {
                                //do nothing here...
                                $(this).dialog("close");
                            }
                        }, //end buttons
                        close: function () {

                        }
                    }); //end SwapTrunkIDValidDiv dialog

                    $("#SwapTrunkIDValidDiv").dialog("open");

                } //end if(bValid)
            } //end SubmitSwapTrunkIDForm()

            ////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////SWAP TRUNK ID PROCESSING END//////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////WEBSERVICE AJAX CALLS START//////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////

            function GetAccountAddition(AccountCode, Unit, AccountComments) {
                jQuery.ajax({
                    url: "WebService.asmx/AccountAddition",
                    type: "POST",
                    data: "{AccountCode:'" + AccountCode.toUpperCase() + "', 'Unit':'" + Unit.toUpperCase() + "', 'AccountComments':'" + AccountComments + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    //async: "false", //stops other processes until this completes
                    beforeSend: function () {
                        $("#AccountFormProcessing").show();
                    },
                    complete: function () {
                        $("#AccountFormProcessing").hide();
                    },
                    success: function (data) {
                        if (data.d.IsError == true) {
                            ThrowError("#DivTopMessage", data.d.ErrorMessage);
                            $("#AccountCode").addClass("ui-state-highlight");
                            $("#AccountCode").addClass("ui-state-error");
                        } else {
                            ThrowSuccess("#DivTopMessage", data.d.str);
                            $("#AccountCode").removeClass("ui-state-highlight");
                            $("#AccountCode").removeClass("ui-state-error");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetAccountAddition() ajax call

            function GetAssetAddition(TrunkID, SerialNumber, ModelNumber, ModelDescription, AssetComments, Cost, Baud, AES, OTAR, OTAP, QueryType) {
                var AjaxData = {
                    "TrunkID": TrunkID,
                    "SerialNumber": SerialNumber,
                    "ModelNumber": ModelNumber,
                    "ModelDescription": ModelDescription,
                    "AssetComments": AssetComments,
                    "Cost": Cost,
                    "Baud": Baud,
                    "AES": AES,
                    "OTAR": OTAR,
                    "OTAP": OTAP,
                    "QueryType": QueryType
                }

                jQuery.ajax({
                    url: "WebService.asmx/AssetAddition",
                    type: "POST",
                    data: JSON.stringify(AjaxData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: false, //stops other processes until this completes
                    beforeSend: function () {
                        $("#AssetFormProcessing").show();
                    },
                    complete: function () {
                        $("#AssetFormProcessing").hide();
                    },
                    success: function (data) {
                        if (data.d.IsError == true) {
                            ThrowError("#DivTopMessage", data.d.ErrorMessage);
                            $("#AccountCode").addClass("ui-state-highlight");
                            $("#AccountCode").addClass("ui-state-error");
                        } else {
                            ThrowSuccess("#DivTopMessage", data.d.str);
                            $("#AccountCode").removeClass("ui-state-highlight");
                            $("#AccountCode").removeClass("ui-state-error");

                            //clear the Trunk ID and Serial Number fields to prevent the user from over-writing the data they just entered
                            //this could happen if they hit submit again without updating one of these fields first
                            $("#TrunkID").val("");
                            $("#TrunkID_Message").text("");
                            $("#SerialNumber").val("");
                            $("#SerialNumber_Message").text("");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetAssetAddition() ajax call

            function GetSwapTrunkID(SerialNumber, OldTrunkID, OldRecordID, NewTrunkID, NewRecordID) {
                var AjaxData = { SerialNumber: SerialNumber, OldTrunkID: OldTrunkID, OldRecordID: OldRecordID, NewTrunkID: NewTrunkID, NewRecordID: NewRecordID }

                jQuery.ajax({
                    url: "WebService.asmx/SwapTrunkID",
                    type: "POST",
                    data: JSON.stringify(AjaxData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    //async: "false", //stops other processes until this completes
                    beforeSend: function () {
                        $("#SwapTrunkIDFormProcessing").show();
                    },
                    complete: function () {
                        $("#SwapTrunkIDFormProcessing").hide();
                    },
                    success: function (data) {
                        if (data.d.IsError == true) {
                            ThrowError("#DivTopMessage", data.d.ErrorMessage);
                            //$("#AccountCode").addClass("ui-state-highlight");
                            //$("#AccountCode").addClass("ui-state-error");
                        } else {
                            ThrowSuccess("#DivTopMessage", data.d.str);
                            //$("#AccountCode").removeClass("ui-state-highlight");
                            //$("#AccountCode").removeClass("ui-state-error");

                            //clear the Trunk ID and Serial Number fields to prevent the user from over-writing the data they just entered
                            //this could happen if they hit submit again without updating one of these fields first
                            $("#SwapTrunkID_TrunkID").val("");
                            $("#SwapTrunkID_TrunkID_Message").text("");
                            $("#SwapTrunkID_SerialNumber").val("");
                            $("#SwapTrunkID_SerialNumber_Message").text("");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                });
            } //end of GetAssetAddition() ajax call

            function GetCheckTrunkIDAvailable(TrunkID, HTMLElementID) {
                jQuery.ajax({
                    url: "WebService.asmx/CheckTrunkIDAvailable",
                    type: "POST",
                    data: "{TrunkID:'" + TrunkID + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: "false", //stops other processes until this completes
                    success: function (data) {
                        if (data.d.IsError == true) {
                            //$("#TrunkID").addClass("ui-state-highlight").addClass("ui-state-error");
                            //$("#TrunkID_Message").text(data.d.ErrorMessage);
                            //$("#TrunkID_Message").css("color", "Red");
                            $("#" + HTMLElementID).addClass("ui-state-highlight").addClass("ui-state-error");
                            $("#" + HTMLElementID + "_Message").text(data.d.ErrorMessage);
                            $("#" + HTMLElementID + "_Message").css("color", "Red");
                            Valid_TrunkID = false;
                            Valid_TrunkID_str = data.d.ErrorMessage;
                            Valid_SwapTrunkID = false;
                            Valid_SwapTrunkID_str = data.d.ErrorMessage;
                        } else {
                            //$("#TrunkID").removeClass("ui-state-highlight").removeClass("ui-state-error");
                            $("#" + HTMLElementID + "_Message").text(data.d.str);
                            $("#" + HTMLElementID + "_Message").css("color", "Green");
                            $(".ValidateTips").removeClass("ui-state-error");
                            $(".ValidateTips").removeClass("ui-state-highlight");
                            $(".ValidateTips").text("");
                            Valid_TrunkID = true;
                            Valid_TrunkID_str = data.d.str;
                            Valid_SwapTrunkID = true;
                            Valid_SwapTrunkID_str = data.d.str;

                            //needed for the SwapTrunkID functions
                            NewRecordID = data.d.NewValue;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                }); //end ajax call
            } //end of GetCheckTrunkIDAvailable() ajax call

            function GetCheckSerialNumberAvailable(SerialNumber, HTMLElementID, ActionIfFound) {
                //SerialNumber - a serial number value must be passed in the form of a string - not an object

                //HTMLElementID - will usually be the id of a div or a span element that can be updated based on the success/failure of this ajax call

                //ActionIfFound - Acceptable inputs: "Good" or "Bad", this parameter will tell the webservice function to return a true or false value based on 
                //if we want the serial number to exist in the database or not.  e.g. when adding a serial number to the database, it would be "Bad" if the serial number
                //already existed.  It would be "Good" if it did not exist.  

                jQuery.ajax({
                    url: "WebService.asmx/CheckSerialNumberAvailable",
                    type: "POST",
                    data: "{SerialNumber:'" + SerialNumber + "', ActionIfFound:'" + ActionIfFound + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: "false", //stops other processes until this completes
                    success: function (data) {
                        if (data.d.IsError == true) {
                            $("#" + HTMLElementID).addClass("ui-state-highlight").addClass("ui-state-error");
                            $("#" + HTMLElementID + "_Message").text(data.d.ErrorMessage);
                            $("#" + HTMLElementID + "_Message").css("color", "Red");
                            Valid_SerialNumber = false;
                            Valid_SerialNumber_str = data.d.ErrorMessage;
                            Valid_SwapSerialNumber = false;
                            Valid_SwapSerialNumber_str = data.d.ErrorMessage;
                        } else {
                            //$("#TrunkID").removeClass("ui-state-highlight").removeClass("ui-state-error");
                            $("#" + HTMLElementID + "_Message").text(data.d.str);
                            $("#" + HTMLElementID + "_Message").css("color", "Green");
                            $(".ValidateTips").removeClass("ui-state-error");
                            $(".ValidateTips").removeClass("ui-state-highlight");
                            $(".ValidateTips").text("");
                            Valid_SerialNumber = true;
                            Valid_SerialNumber_str = data.d.str;
                            Valid_SwapSerialNumber = true;
                            Valid_SwapSerialNumber_str = data.d.str;

                            //needed for the SwapTrunkID functions
                            OldRecordID = data.d.NewValue;
                            OldTrunkID = data.d.OldTrunkID
                            //alert(data.d.NewValue);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                }); //end ajax call
            } //end of GetCheckTrunkIDAvailable() ajax call

            function GetCopyAsset(SerialNumber) {
                jQuery.ajax({
                    url: "WebService.asmx/CopyAsset",
                    type: "POST",
                    data: "{SerialNumber:'" + $.trim(SerialNumber) + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: "false",
                    async: false, //stops other processes until this completes
                    success: function (data) {
                        if (data.d.IsError == true) {
                            ThrowError("#DivTopMessage", data.d.ErrorMessage);
                        } else {
                            ThrowSuccess("#DivTopMessage", data.d.str);

                            var ModelNumber, ModelDescription, Cost, Baud, AES, OTAR, OTAP, AssetComments;

                            $.each(data.d.DataArrayList, function (x) {
                                //alert(x + ": " + this);

                                if (x == 1) {
                                    ModelNumber = this;
                                } else if (x == 2) {
                                    ModelDescription = this;
                                } else if (x == 3) {
                                    Cost = this;
                                } else if (x == 4) {
                                    //alert(this); // - example: "U" or "Y" or "X" or "N" or "NA"
                                    Baud = this;
                                } else if (x == 5) {
                                    //alert(this); // - example: "U" or "Y" or "X" or "N" or "NA"
                                    AES = this;
                                } else if (x == 6) {
                                    //alert(this); // - example: "U" or "Y" or "X" or "N" or "NA"
                                    OTAR = this;
                                } else if (x == 7) {
                                    //alert(this); // - example: "U" or "Y" or "X" or "N" or "NA"
                                    OTAP = this;
                                } else if (x == 8) {
                                    AssetComments = this;
                                }
                            });

                            $("#ModelNumber").val(ModelNumber);
                            $("#ModelDescription").val(ModelDescription);
                            $("#AssetComments").val(AssetComments);
                            //$("#Cost").val(Cost);
                            $("#Baud_" + Baud).attr("checked", "checked");
                            $("#AES_" + AES).attr("checked", "checked");
                            $("#OTAR_" + OTAR).attr("checked", "checked");
                            $("#OTAP_" + OTAP).attr("checked", "checked");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ThrowError("#DivTopMessage", "Error: " + xhr.status + " " + thrownError);
                    },
                    statusCode: WebStatusCodes
                }); //end ajax call
            } //end of GetCopyAsset() ajax call

            //////////////////////////////WEBSERVICE AJAX CALLS END////////////////////////////////

        }); 
           
    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />

    <div id="DivTopMessage" class="ValidateTips"></div>
    <br />
    <div id="tabs">
        
        <ul>
            <li><a href="#tabs-1">New Asset</a></li>
            <li><a href="#tabs-2">New Account</a></li>
            <li><a href="#tabs-3">Swap Trunk ID</a></li>
        </ul>
        <!----------------------------------TAB 1---------------------------------->
        <div id="tabs-1">
            <div id="AssetsDiv" class="Form">
                <div id="AssetsDivHeading">
                    <p><span class="HeaderText">Add a New Asset</span></p>
                    <div class="Seperator"></div>
                    <p class="Comment">*NOTE: Newly added assets are automatically added to the PWCS account.&nbsp;&nbsp;A transfer will need to be initiated in order to move the asset to another account.</p>
                </div>
                
                <div id="FormLabel_CopyAssetCheckbox" class="">
                <input type="checkbox" id="CopyAssetCheckbox" name="CopyAssetCheckbox" />    
                &nbsp;Copy existing asset
                </div>

                <p id="FormLabel_TrunkID" class="FormLabel"><span class="RedText">* </span>Trunk ID: <span class="SmallText">(e.g. 707114)</span></p>
                <input type="text" id="TrunkID" maxlength="6" size="4" />
                &nbsp;&nbsp;or&nbsp;&nbsp;
                <input type="checkbox" id="TrunkID_Checkbox" name="TrunkID_Checkbox" value="TrunkID_Unknown" /> Unknown
                <br />
                <span id="TrunkID_Message"></span>
                <br />

                <p id="FormLabel_SerialNumber" class="FormLabel"><span class="RedText">* </span>Serial Number: <span class="SmallText">(e.g. 320CEG1614)</span></p>
                <input type="text" id="SerialNumber" maxlength="25" size="" />
                <br />
                <span id="SerialNumber_Message"></span>
                <br />
                
                <p id="FormLabel_ModelNumber" class="FormLabel"><span class="RedText">* </span>Model Number: <span class="SmallText">(e.g. H18QDF9PW6AN)</span></p>
                <input type="text" id="ModelNumber" maxlength="25" size="" />
                <br />
                <p id="FormLabel_ModelDescription" class="FormLabel"><span class="RedText">* </span>Model Description: <span class="SmallText">(e.g. XTS 5000 II)</span></p>
                <input type="text" id="ModelDescription" maxlength="50" size="" /> 

                <!-- Need to hide this section and have the user select from a drop down for these attributes.  If the attribute selection they choose is
                    not available then have the option to "show" this section and they can enter it manually.
                <select>
                    <option></option>
                    <option>Something</option>
                    <option>Something Else</option>
                    <option>Why am I seeing this...</option>
                </select>
                -->
                
                <p id="FormLabel_AssetComments" class="FormLabel">Asset Comments: <span class="SmallText">(Optional - 2000 character max)</span></p>
                <textarea id="AssetComments" cols="50" rows="5" ></textarea>

                <p id="FormLabel_Cost" class="FormLabel"><span class="RedText">* </span>Cost: <span class="SmallText">(e.g. 4699.27)</span></p>
                $ <input type="text" id="Cost" maxlength="4" size="3" /> . <input type="text" id="Cost_2" maxlength="2" size="1" value="00"/>
                &nbsp;&nbsp;or&nbsp;&nbsp;
                <input type="checkbox" id="Cost_Checkbox" name="Cost_Checkbox" value="Cost_Unknown" /> Unknown 

                <p id="FormLabel_HardwareModule" class="FormLabel"><span class="RedText">* </span>Hardware Mods Installed: <span class="SmallText">(Please select all that apply)</span></p>
                <div class="HardwareMods">
                        <div id="HM-Item_9600B" class="HM-Item">
                            <span id="Span_9600B">9600B</span>
                            <br />
                            <input type="radio" id="Baud_Y" name="Baud" value="Y" />Yes&nbsp;&nbsp;
                            <input type="radio" id="Baud_N" name="Baud" value="N" />No&nbsp;&nbsp;
                            <input type="radio" id="Baud_NA" name="Baud" value="NA" />Not Applicable&nbsp;&nbsp;
                            <input type="radio" id="Baud_X" name="Baud" value="X" />Not Capable&nbsp;&nbsp;
                            <input type="radio" id="Baud_U" name="Baud" value="U" />Unknown&nbsp;&nbsp;
                        </div>
                        <div id="HM-Item_AES" class="HM-Item">
                            <span id="Span_AES">AES</span>
                            <br />
                            <input type="radio" id="AES_Y" name="AES" value="Y" />Yes&nbsp;&nbsp;
                            <input type="radio" id="AES_N" name="AES" value="N" />No&nbsp;&nbsp;
                            <input type="radio" id="AES_NA" name="AES" value="NA" />Not Applicable&nbsp;&nbsp;
                            <input type="radio" id="AES_X" name="AES" value="X" />Not Capable&nbsp;&nbsp;
                            <input type="radio" id="AES_U" name="AES" value="U" />Unknown&nbsp;&nbsp;
                        </div>
                        <div id="HM-Item_OTAR" class="HM-Item">
                            <span id="Span_OTAR">OTAR</span>
                            <br />
                            <input type="radio" id="OTAR_Y" name="OTAR" value="Y" />Yes&nbsp;&nbsp;
                            <input type="radio" id="OTAR_N" name="OTAR" value="N" />No&nbsp;&nbsp;
                            <input type="radio" id="OTAR_NA" name="OTAR" value="NA" />Not Applicable&nbsp;&nbsp;
                            <input type="radio" id="OTAR_X" name="OTAR" value="X" />Not Capable&nbsp;&nbsp;
                            <input type="radio" id="OTAR_U" name="OTAR" value="U" />Unknown&nbsp;&nbsp;
                        </div>
                        <div id="HM-Item_OTAP" class="HM-Item">
                            <span id="Span_OTAP">OTAP</span>
                            <br />
                            <input type="radio" id="OTAP_Y" name="OTAP" value="Y" />Yes&nbsp;&nbsp;
                            <input type="radio" id="OTAP_N" name="OTAP" value="N" />No&nbsp;&nbsp;
                            <input type="radio" id="OTAP_NA" name="OTAP" value="NA" />Not Applicable&nbsp;&nbsp;
                            <input type="radio" id="OTAP_X" name="OTAP" value="X" />Not Capable&nbsp;&nbsp;
                            <input type="radio" id="OTAP_U" name="OTAP" value="U" />Unknown&nbsp;&nbsp;
                        </div>
                </div>
                <br />
                <div class="Seperator"></div>
                <br />
                <div>
                    <div style="display:inline-block;">
                        <input type="button" id="AssetFormSubmit" value="Submit" />
                    </div>
                    <div id="AssetFormProcessing" class="Processing" style="display:inline-block;  display:none;">
                        Processing...
                    </div>
                    <div style="clear:both;"></div>
                </div>
                
            </div>
        </div>
        <!----------------------------------TAB 1---------------------------------->
        
        <!----------------------------------TAB 2---------------------------------->
        <div id="tabs-2">
            <div id="AccountDiv" class="Form">
                <div id="AccountDivHeading">
                    <p><span class="HeaderText">Add a New Account</span></p>
                    <div class="Seperator"></div>
                </div>
                <p id="FormLabel_AccountCode" class="FormLabel"><span class="RedText">* </span>Account Code: <span class="SmallText">(must be exactly 4 characters)</span></p>
                <input type="text" maxlength="4" size="10" id="AccountCode" />
                <br />
                <p id="FormLabel_Unit" class="FormLabel"><span class="RedText">* </span>Account Organization: <span class="SmallText">(50 character max)</span></p>
                <input type="text" id="Unit" size="30" />
                <br />
                <p id="FormLabel_AccountComments" class="FormLabel">Account Comments: <span class="SmallText">(optional but recommended, 2000 character max)</span></p>
                <textarea id="AccountComments" cols="50" rows="5"></textarea> 
                <br />
                <br />
                <div class="Seperator"></div>
                <br />
                <div>
                    <div style="float:left;">
                        <input type="button" id="AccountFormSubmit" value="Submit" />   
                    </div>
                    <div id="AccountFormProcessing" class="Processing" style="float:left; display:none;">
                        Processing...
                    </div>
                    <div style="clear:both;"></div>
                </div>
            </div>
        </div>
        <!----------------------------------TAB 2---------------------------------->
        
        <!----------------------------------TAB 3---------------------------------->
        <div id="tabs-3">
            <script type="text/javascript">
                //This works...
                //var jsontext = '{"firstname":"Jesper","surname":"Aaberg","phone":["555-0100","555-0120"]}';
                //var contact = JSON.parse(jsontext);
                //for (var key in contact) {
                //    if (contact.hasOwnProperty(key)) {
                //        document.write("key: " + key + ", val: " + contact[key] + "<br />");
                //    }
                //}    
            </script>
            <div id="SwapTrunkIDDiv" class="Form">
                <div id="SwapTrunkIDDivHeading">
                    <p><span class="HeaderText">Swap Trunk ID for an Asset</span></p>
                    <div class="Seperator"></div>
                    <p class="Comment">*NOTE: An asset can only be moved to a trunk id that is not currently associated with an existing asset.</p>
                </div>
                <p id="FormLabel_SwapTrunkID_SerialNumber" class="FormLabel"><span class="RedText">* </span>Serial Number: <span class="SmallText">(e.g. 320CEG1614)</span></p>
                <input id="SwapTrunkID_SerialNumber" type="text" maxlength="" size="" />
                <br />
                <span id="SwapTrunkID_SerialNumber_Message"></span>
                <br />

                <p id="FormLabel_SwapTrunkID_TrunkID" class="FormLabel"><span class="RedText">* </span>New Trunk ID: <span class="SmallText">(e.g. 707114)</span></p>
                <input id="SwapTrunkID_TrunkID" type="text" maxlength="6" size="5" />
                <br />
                <span id="SwapTrunkID_TrunkID_Message"></span>
                <br />
                <br />
                <div class="Seperator"></div>
                <br />
                
                <div>
                    <div style="float:left;">
                        <input type="button" id="SwapTrunkIDFormSubmit" value="Submit" />
                    </div>
                    <div id="SwapTrunkIDFormProcessing" class="Processing" style="float:left; display:none;">
                        Processing...
                    </div>
                    <div style="clear:both;"></div>
                </div>
            </div>
        </div>
        <!----------------------------------TAB 3---------------------------------->
    </div>
    
<!--Dialog Form Div's START-->

<div id="CopyAssetDialog" class="" style="display:none;">
    <div id="CopyAssetMessage" style="text-align:center; padding:5px;"></div>
    <div style="text-align:center;">
        <p><b>Please enter a serial number from which to copy data</b></p>
        <p><input type="text" id="CopyAssetInput" /></p>
    </div>
</div>
    
<div id="SwapTrunkIDValidDiv" style="display:none;"></div>

<div id="AssetAdditionDialog" style="display:none;"></div>

<!--Dialog Form Div's END-->


    </form>
</body>
</html>
