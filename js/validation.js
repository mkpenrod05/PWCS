var WebStatusCodes = {
    403: function () {
        ThrowError("#DivTopMessage", "Error 403: Forbidden Request!");
    },
    404: function () {
        ThrowError("#DivTopMessage", "Error 404: WebService Not Found!");
    },
    500: function () {
        ThrowError("#DivTopMessage", "Error 500: Internal Server Error!");
    },
    503: function () {
        ThrowError("#DivTopMessage", "Error 503: Server Unavailable to Process the Request!");
    }
}

var HardwareOptions = {
    "Y" : "Y - Hardware Module Installed",
    "N" : "N - Hardware Module Not Installed",
    "X" : "X - Hardware Module Not Compatible",
    "U" : "U - Unknown Hardware Module Installation",
    "NA" : "NA - Hardware Module Not Applicable"
};

var DisabledStateOptions = {
    "Selective Inhibit - In Passive" : "Selective Inhibit - In Passive",
    "Selective Inhibit - Successful" : "Selective Inhibit - Successful",
    "Cancel Inhibit - In Passive" : "Cancel Inhibit - In Passive",
    "Cancel Inhibit - Successful" : "Cancel Inhibit - Successful"
}

var RankOptions = {
    "" : "",
    "CIV" : "CIV",
    "SA" : "SA",
    "AB" : "AB",
    "AMN" : "AMN",
    "A1C" : "A1C",
    "SRA" : "SRA",
    "SSGT" : "SSGT",
    "TSGT" : "TSGT",
    "MSGT" : "MSGT",
    "SMSGT" : "SMSGT",
    "CMSGT" : "CMSGT",
    "Lt2nd" : "Lt2nd",
    "Lt1st" : "Lt1st",
    "Capt" : "Capt",
    "Maj" : "Maj",
    "LtCol": "LtCol",
    "Col" : "Col" 
}

function BuildRankSelectList(HtmlElement) {
    $.each(RankOptions, function (key, value) {
        $(HtmlElement).append($("<option/>", {
            value: key,
            text: value
        }));
    });
}

function UpdateValidationTips(str) {
    $(".ValidateTips").addClass("ui-state-error");
    $(".ValidateTips").text(str).addClass("ui-state-highlight");
    setTimeout(function () { $(".ValidateTips").removeClass("ui-state-error"); }, 2500);
}

function ValidationPassed(str) {
    $(".ValidateTips").addClass("ui-state-validated");
    $(".ValidateTips").text(str);
}

function CheckLength(input, min, max, str) {
    if (input.val().length > max || input.val().length < min) {
        input.addClass("ui-state-error");
        UpdateValidationTips(str);
        return false;
    } else {
        return true;
    }
}

function CheckSelectList(input, message) {
    if (input.val() == "") {
        input.addClass("ui-state-error");
        UpdateValidationTips(message);
        return false;
    } else {
        return true;
    }
}

function CheckForUndefinedValues(input, str) {
    if (input.val() == "undefined") {
        input.addClass("ui-state-error");
        UpdateValidationTips(str);
        return false;
    } else {
        return true;
    }
}

function CheckForEmptyString(input, str) {
    if (input == "" || input == null) {
        UpdateValidationTips(str);
        return false;
    } else {
        return true;
    }
}

function IsNumeric(input, str) {
    if ($.isNumeric(input.val())) {
        return true;
    } else {
        input.addClass("ui-state-error");
        UpdateValidationTips(str);
        return false;
    }
}

function CleanObjectValue(input, characterCase) {
    var NewValue = $.trim(input.val());
    if (characterCase = "UpperCase") {
        NewValue = NewValue.toUpperCase();
    } else if (characterCase = "LowerCase") {
        NewValue = NewValue.toLowerCase();
    } else {
        NewValue = NewValue;
    }
    return NewValue;
}

function CleanStringValue(input, characterCase) {
    var NewValue = $.trim(input);
    if (characterCase = "UpperCase") {
        NewValue = NewValue.toUpperCase();
    } else if (characterCase = "LowerCase") {
        NewValue = NewValue.toLowerCase();
    } else {
        NewValue = NewValue;
    }
    return NewValue;
}

function LoaderGraphic(DivElementClass) {
    //in order for this function to work you need to have a div on the page set 
    //with a class='blankContainer' and it must have a fixed width!  The class 'addLoader'
    //is defined within style.css
    $("." + DivElementClass).append("<div id='addLoaderDiv' class='addLoader'></div>");
}

function KillLoaderGraphic() {
    $("#addLoaderDiv").remove();
}

function Processing(HtmlElement) {
    $(HtmlElement).html(
        "<div class='Processing'>Processing...</div>"
    );
}

function ThrowError(HtmlElement, str) {
    var Message = $(HtmlElement);
    Message.removeClass();
    Message.text(str);
    Message.addClass("ui-state-highlight");
    Message.addClass("ui-state-error");
    setTimeout(function () {
        $(HtmlElement).removeClass("ui-state-error"); 
    }, 2500);
}

function ThrowSuccess(HtmlElement, str) {
    var Message = $(HtmlElement);
    Message.removeClass();
    Message.text(str);
    Message.addClass("ui-state-validated");
}

function ReplaceSpecialCharacters(Data) {
    Data = Data.replace(/'/g, "&#39;");
    Data = Data.replace(/"/g, "&#34;");
    Data = Data.replace(/</g, "&#60;");
    Data = Data.replace(/>/g, "&#62;");
    return Data;
}

function ValidateTrainingDate(TrainingDate) {

    alert(TrainingDate);

    var Expired = new Date();
    var Due = new Date();

    Expired.setDate(Expired.getDate() - 365);
    Due.setDate(Due.getDate() - 60);

    if (TrainingDate < Expired) {
        return "invalidCheck";
    } else if (TrainingDate < Due) {
        return "warningCheck";
    } else {
        return "validCheck";
    }

}
function test() { return "background-color: red;"; }
//ValidateTrainingDate("6/24/2012 12:00:00 AM");