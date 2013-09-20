/*
Copyright (c) 2008 Joseph Scott, http://josephscott.org/

http://josephscott.org/code/javascript/jquery-edit-in-place/

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// version: 0.1.2

(function ($) {
    $.fn.eip = function (save_url, options) {
        // Defaults
        var opt = {
            save_url: save_url,

            save_on_enter: true,
            cancel_on_esc: true,
            focus_edit: true,
            select_text: false,
            edit_event: "click",
            select_options: false,
            data: false,

            // added the "date" option below to allow .datepicker() - MCP Aug 2012
            //added the "email" option to allow for a wider text area - MCP Oct 2012
            form_type: "text", // text, textarea, select, date, email 
            size: false, // calculate at run time
            max_size: 60,
            rows: false, // calculate at run time
            max_rows: 10,

            //added new parameters to allow for a wider text field (specifically for an email address) - MCP Oct 2012
            normal_size: 25, //new parameter - MCP Oct 2012
            email_size: 35, //new parameter - MCP Oct 2012

            cols: 60, //change to 50 to accomidate a smaller screen if needed - MCP

            //added new parameter to allow a textarea to be opened up with no text in it.  By default, this edit-in-place 
            //puts the current content located in the <span> tag when it is clicked.  If you click on a long comment for instance, 
            //all the text will be pasted into the textarea.  If this parameter is set to "blank" on the front end, the textarea will 
            //open up with no text inside - e.g.  .eip("WebService.asmx/AccountInfoUpdate", { form_type: "date", textarea_text: "blank" })
            //added on 14 Nov 2012 - MCP
            textarea_text: "",

            //added new parameters to allow elements on the page to be refreshed after the edit-in-place executes
            //an example of this is the Managers Information on the default page.  That div is called via ajax with the account code
            //passed in as a parameter.  In order to make that ajax call again, we need to pass that account code into the edit-in-place
            //as well.  Example account code: "74LM"
            account_code: "",

            savebutton_text: "SAVE",
            savebutton_class: "jeip-savebutton",
            cancelbutton_text: "CANCEL",
            cancelbutton_class: "jeip-cancelbutton",

            mouseover_class: "jeip-mouseover",
            editor_class: "jeip-editor",
            editfield_class: "jeip-editfield",

            //saving_text: "Saving ...", //made a modification to add the circle loading graphic, more appealing to the user - MCP Nov 2012
            saving_text: "<img src='images/CircleBallSpinner.gif' height='16' width='16' /> Saving ...",
            saving_class: "jeip-saving",

            saving: '<span id="saving-#{id}" class="#{saving_class}" style="display: none;">#{saving_text}</span>',

            start_form: '<span id="editor-#{id}" class="#{editor_class}" style="display: none;">',

            //Added the <br /> to force the select options onto the next line MCP
            form_buttons: '<br /><span><input type="button" id="save-#{id}" class="#{savebutton_class}" value="#{savebutton_text}" /> OR <input type="button" id="cancel-#{id}" class="#{cancelbutton_class}" value="#{cancelbutton_text}" /></span>',
            stop_form: '</span>',

            //added the "size" option - MCP Oct 2012
            //text_form: '<input type="text" id="edit-#{id}" class="#{editfield_class}" value="#{value}" /> <br />',
            text_form: '<input type="text" id="edit-#{id}" class="#{editfield_class}" value="#{value}" size="#{size}" /> <br />',

            //Testing MCP
            //text_form_string: '<input type="text" id="string-edit-#{id}" class="#{editfield_class}" value="#{value}" /> <br />',
            //Testing MCP

            textarea_form: '<textarea cols="#{cols}" rows="#{rows}" id="edit-#{id}" class="#{editfield_class}">#{value}</textarea> <br />',

            start_select_form: '<select id="edit-#{id}" class="#{editfield_class}">',
            select_option_form: '<option id="edit-option-#{id}-#{option_value}" value="#{option_value}" #{selected}>#{option_text}</option>',
            stop_select_form: '</select>',

            after_save: function (self) {
                for (var i = 0; i < 2; i++) {
                    $(self).fadeOut("fast");
                    $(self).fadeIn("fast");
                }
            },
            on_error: function (msg) {
                alert("Error: " + msg);
            }
        }; // defaults

        if (options) {
            $.extend(opt, options);
        }

        this.each(function () {
            var self = this;

            $(this).bind("mouseenter mouseleave", function (e) {
                $(this).toggleClass(opt.mouseover_class);
            });

            $(this).bind(opt.edit_event, function (e) {
                _editMode(this);
            });
        }); // this.each

        // Private functions
        var _editMode = function (self) {
            $(self).unbind(opt.edit_event);

            $(self).removeClass(opt.mouseover_class);
            $(self).fadeOut("fast", function (e) {
                var id = self.id;
                var value = $(self).html();

                var safe_value = value.replace(/</g, "&lt;");
                safe_value = value.replace(/>/g, "&gt;");
                safe_value = value.replace(/"/g, "&quot;");

                var orig_option_value = false;

                var form = _template(opt.start_form, {
                    id: self.id,
                    editor_class: opt.editor_class
                });

                //added 'date' here - MCP Aug 2012
                if (opt.form_type == 'text' || opt.form_type == 'date') {
                    form += _template(opt.text_form, {
                        id: self.id,
                        editfield_class: opt.editfield_class,
                        value: value,
                        //size is a new parameter - MCP Oct 2012
                        size: opt.normal_size
                    });
                    // end form_type - text, date
                } else if (opt.form_type == 'email') {
                    form += _template(opt.text_form, {
                        id: self.id,
                        editfield_class: opt.editfield_class,
                        value: value,
                        //size is a new parameter - MCP Oct 2012
                        size: opt.email_size
                    });
                    //end form_type - email
                } else if (opt.form_type == 'textarea') {

                    //this is a new parameter, find the textarea_text option at the top of this document for a full explanation - MCP 14 Nov 2012
                    if (opt.textarea_text == 'blank') {
                        value = "";
                    }

                    var length = value.length;
                    var rows = (length / opt.cols) + 2;

                    for (var i = 0; i < length; i++) {
                        if (value.charAt(i) == "\n") {
                            rows++;
                        }
                    }

                    if (rows > opt.max_rows) {
                        rows = opt.max_rows;
                    }
                    if (opt.rows != false) {
                        rows = opt.rows;
                    }
                    rows = parseInt(rows);

                    form += _template(opt.textarea_form, {
                        id: self.id,
                        cols: opt.cols,
                        rows: rows,
                        editfield_class: opt.editfield_class,
                        value: value
                    });
                    // end textarea form
                } else if (opt.form_type == 'select') {
                    form += _template(opt.start_select_form, {
                        id: self.id,
                        editfield_class: opt.editfield_class
                    });

                    $.each(opt.select_options, function (k, v) {
                        var selected = '';
                        if (v == value) {
                            selected = 'selected="selected"';
                        }

                        if (value == v) {
                            orig_option_value = k;
                        }

                        form += _template(opt.select_option_form, {
                            id: self.id,
                            option_value: k,
                            option_text: v,
                            selected: selected
                        });
                    });

                    form += _template(opt.stop_select_form, {});
                } // end select form

                form += _template(opt.form_buttons, {
                    id: self.id,
                    savebutton_class: opt.savebutton_class,
                    savebutton_text: opt.savebutton_text,
                    cancelbutton_class: opt.cancelbutton_class,
                    cancelbutton_text: opt.cancelbutton_text
                });

                form += _template(opt.stop_form, {});

                $(self).after(form);
                $("#editor-" + self.id).fadeIn("fast");

                if (opt.focus_edit) {
                    $("#edit-" + self.id).focus();
                    //Added the datepicker for user entry control - MCP
                    if (opt.form_type == 'date') {
                        $("#edit-" + self.id).datepicker({
                            showButtonPanel: true,
                            closeText: "Cancel"
                        });
                    }
                }

                if (opt.select_text) {
                    $("#edit-" + self.id).select();
                }

                $("#cancel-" + self.id).bind("click", function (e) {
                    _cancelEdit(self);
                });

                $("#edit-" + self.id).keydown(function (e) {
                    // cancel
                    if (e.which == 27) {
                        _cancelEdit(self);
                    }

                    // save
                    if (opt.form_type != "textarea" && e.which == 13) {
                        _saveEdit(self, orig_option_value);
                    }
                });

                $("#save-" + self.id).bind("click", function (e) {
                    return _saveEdit(self, orig_option_value);
                }); // save click
            }); // this fadeOut
        } // function _editMode

        var _template = function (template, values) {
            var replace = function (str, match) {
                return typeof values[match] === "string" || typeof values[match] === 'number' ? values[match] : str;
            };
            return template.replace(/#\{([^{}]*)}/g, replace);
        };

        var _trim = function (str) {
            return str.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
        }

        var _cancelEdit = function (self) {
            $("#editor-" + self.id).fadeOut("fast");
            $("#editor-" + self.id).remove();

            $(self).bind(opt.edit_event, function (e) {
                _editMode(self);
            });

            $(self).removeClass(opt.mouseover_class);
            $(self).fadeIn("fast");
        };

        var _saveEdit = function (self, orig_option_value) {
            var orig_value = $(self).html();
            var new_value = $("#edit-" + self.id).prop("value");

            if (orig_value == new_value) {
                $("#editor-" + self.id).fadeOut("fast");
                $("#editor-" + self.id).remove();

                $(self).bind(opt.edit_event, function (e) {
                    _editMode(self);
                });

                $(self).removeClass(opt.mouseover_class);
                $(self).fadeIn("fast");

                return true;
            }

            $("#editor-" + self.id).after(_template(opt.saving, {
                id: self.id,
                saving_class: opt.saving_class,
                saving_text: opt.saving_text
            }));
            $("#editor-" + self.id).fadeOut("fast", function () {
                $("#saving-" + self.id).fadeIn("fast");
            });

            var ajax_data = {
                url: location.href,
                id: self.id,
                form_type: opt.form_type,
                orig_value: orig_value,
                new_value: $("#edit-" + self.id).prop("value"),
                data: opt.data
            }

            //alert(JSON.stringify(ajax_data));

            //var ajax_data_string = '{url:"' + location.href ", id: "self.id", form_type: "opt.form_type", orig_value: "orig_value" , new_value:  $("#edit-" + self.id).attr("value"),data: opt.data }'

            if (opt.form_type == 'select') {
                ajax_data.orig_option_value = orig_option_value;
                ajax_data.orig_option_text = orig_value;
                ajax_data.new_option_text = $("#edit-option-" + self.id + "-" + new_value).html();
            }

            jQuery.ajax({
                url: opt.save_url,
                type: "POST",
                timeout: 20000,
                contentType: "application/json",
                dataType: "json",
                //data: ajax_data,
                data: JSON.stringify(ajax_data),
                cache: "false",
                success: function (data) {
                    //alert(data.d.html);
                    $("#editor-" + self.id).fadeOut("fast");
                    $("#editor-" + self.id).remove();
                    if (data.d.is_error == true) {
                        opt.on_error(data.error_text);
                    }
                    else {
                        $(self).html(data.d.html);
                    }

                    $("#saving-" + self.id).fadeOut("fast");
                    $("#saving-" + self.id).remove();

                    $(self).bind(opt.edit_event, function (e) {
                        _editMode(self);
                    });

                    $(self).addClass(opt.mouseover_class);
                    $(self).fadeIn("fast");

                    //Setting the after_save parameter to false in the front end will allow the "else" portion of this statement to execute.
                    //This was designed specifically for textareas so that the <span> tag that was clicked on to call this edit-in-place would
                    //reload with the data specified below instead of the data entered into the textarea or input field.
                    //An example of this can be found on the Default.aspx page when an account or asset comment is added.
                    //Here is an example of setting the after_save option to false:
                    //e.g. - $("[id^='AccountComments_']").eip("WebService.asmx/AccountInfoUpdate", { form_type: "textarea", after_save: false, textarea_text: "blank", rows: 5, cols: 50 });
                    //Added on Dec 2012 - MCP
                    if (opt.after_save != false) {
                        opt.after_save(self);
                    } else {
                        if (self.id.match("^AccountComments_")) {
                            $(self).html("Add New Account Comment");
                        } else if (self.id.match("^AssetComments_")) {
                            $(self).html("Add New Asset Comment");
                        }

                    }

                    $(self).removeClass(opt.mouseover_class);

                    //alert(self.id); // Example: "Inventory_44" or "AccountValidation_34" MCP
                    //Thought about trying to refresh the Div's on the default page here... MCP

                    //This function is defined below and will call various functions needed to reload data via ajax
                    //on the page.
                    ReloadData(self.id, opt.account_code);

                }, //Success
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#DivTopMessage").empty();
                    $("#DivTopMessage").append('Error: ' + xhr.status + ' --ThownError:' + thrownError + ' --Status Text:' + xhr.statusText);
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                },
                statusCode: {
                    404: function () {
                        alert('WebService Not Found.');
                    }
                }
            }); // ajax
        }; // _saveEdit


    }; // inplaceEdit
})(jQuery);


/*
This function is used to reload a div element on the edit-in-place save.
The "AccountCode" variable below only contains a value if a value is passed into the eip function on the page.

Function References:
GetGrabActionType(ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue, HTMLElement) 
*/
function ReloadData(ID, AccountCode) {
    if (ID.match("^AccountComments_")) {
        //e.g. "AccountComments_14"
        
        var RowID = ID.replace("AccountComments_", "");

        //GetGrabActionType() is found in js/RetrieveLogData.js and makes an ajax call to GrabLogData.asmx/GrabActionType
        //these parameters are used to pull unique data from the "Log" table
        GetGrabActionType("Account Update", "", RowID, "account_comments", "", "AdditionalAccountComments");
        
    }

    if (ID.match("^AssetComments_")) {
        //e.g. "AssetComments_7625"
        
        var RowID = ID.replace("AssetComments_", "");

        //GetGrabActionType() is found in js/RetrieveLogData.js and makes an ajax call to GrabLogData.asmx/GrabActionType
        //these parameters are used to pull unique data from the "Log" table
        GetGrabActionType("Asset Update", "", RowID, "assetComments", "", "AdditionalAssetComments");
        //Reference: GetGrabActionType(ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue, HTMLElement)
    }

    if (ID.match("^trained_")) {
        //e.g. "trained_85"
        //GetManagersInformation() is found in js/Default_Page.js
        GetManagersInformation(AccountCode);
    }

    if (ID.match("^AppointmentLetter_") || ID.match("^Inventory_") || ID.match("^AccountValidation_")) {
        //e.g. "AppointmentLetter_85" or "Inventory_23" or "AccountValidation_52"

        //GetAccountList() is found in js/Default_Page.js
        GetAccountList();

        //alert(AccountCode); // - e.g. "74LM"
        //alert(ID); // - e.g. "AppointmentLetter_75"

    }
}