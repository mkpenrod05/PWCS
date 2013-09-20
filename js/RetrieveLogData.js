
    function GetGrabActionType(ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue, HTMLElement) {
        jQuery.ajax({
            url: "WebService.asmx/GrabActionType",
            type: "POST",
            data: "{ActionType:'" + ActionType + "', 'AffectedTable':'" + AffectedTable + "', 'AffectedTableID':'" + AffectedTableID + "', 'ColumnName':'" + ColumnName + "', 'UniqueValue':'" + UniqueValue + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: "false",
            //async: "false", //stops other processes until this completes
            success: function (data) {

                var ReturnData = data.d.str;

                //Replace all new line characters with a break tag.  New line characters are not affected
                //by the HtmlEncode function in VB so this still works regardless.
                ReturnData = ReturnData.replace(/\n/g, "<br />");

                //$("#" + HTMLElement).html(data.d.str);
                $("#" + HTMLElement).html(ReturnData);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $("#DivTopMessage").empty();
                $("#DivTopMessage").append('Error: ' + xhr.status + ' ' + thrownError);
            },
            statusCode: {
                404: function () {
                    alert('WebService Not Found!');
                }
            }
        });
    } //end of GetGrabActionType() ajax call

