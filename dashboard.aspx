<%@ Page Language="VB" %>
<%@ Register TagPrefix="UserControl" TagName="SourceFiles" Src="~/userControls/SourceFiles.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="PageHeader" Src="~/userControls/PageHeader.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="SiteNavigation" Src="~/userControls/SiteNavigation.ascx" %>

<!DOCTYPE html>
<!--https://wbhill03.hill.afmc.ds.af.mil/PWCS/-->

<script runat="server">
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        If UserValidation.PageAccess(HttpContext.Current.Request.ServerVariables("AUTH_USER").ToLower()) = False Then
            Response.Write("Access Denied!")
            Response.End()
        End If
        
        Dim query As String = Request.QueryString("query")
        'Static WebServiceUrl As String = ""
        
        If query = "Managers" Then
            'ContentDiv.InnerHtml = CustomFunctions.AccountManagers()
            'WebServiceUrl = "WebService.asmx/JsonManagersList"
        ElseIf query = "Archive" Then
            'ContentDiv.InnerHtml = CustomFunctions.ArchivedAssets()
        ElseIf query = "UnaccountedFor" Then
            'ContentDiv.InnerHtml = CustomFunctions.UnaccountedFor()
        End If
        
    End Sub
</script>

<html>
<head id="Head1" runat="server">
    
    <title><%=CustomFunctions.PageTabText("Dashboard")%></title>

    <UserControl:SourceFiles runat="server" />
    <script type="text/javascript" src="js/Default_Page.js"></script>

    <style type="text/css">
        .clickable { cursor:pointer; background-color:#80C8E5; }
        .clickableNoCursor { background-color:#80C8E5; }
        .ui-menu { width:150px; }
    </style>



    <script type="text/javascript">
        $(document).ready(function () {

            var url = window.location.href;

            var Model, Columns, TableName;

            var Options = {
                TransportData: {},
                Toolbar: {
                    Template: "",
                    CreateTemplate: function (TableName) {
                        var data = { template: "<h1 style='text-align:center;'>" + TableName + "</h1>" }
                        return data;
                    },
                    Create: { name: "create" },
                    Save: { name: "save" },
                    Cancel: { name: "cancel" }
                },

                //You will need to fill this array with the columns you want right before you load the grid.
                //For now, leave it empty.
                Columns: [],
                
                CreateColumns: {

                    //Each of the names on this level must match the key name of key/value pairs returned in the
                    //JSON object returned from the webservice. 
                    AccountCode: {
                        ColumnSetup: {
                            field: "AccountCode",
                            title: "Account",
                            width: "75px",
                            template: "<span class='link' value='#: AccountCode #'>#: AccountCode #</span>"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    Position: {
                        ColumnSetup: {
                            field: "Position",
                            width: "100px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    Rank: {
                        ColumnSetup: {
                            field: "Rank",
                            width: "75px",
                            editor: ""
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    FirstName: {
                        ColumnSetup: {
                            field: "FirstName",
                            title: "First Name"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    LastName: {
                        ColumnSetup: {
                            field: "LastName",
                            title: "Last Name"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    Organization: {
                        ColumnSetup: {
                            field: "Organization"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    Phone: {
                        ColumnSetup: {
                            field: "Phone",
                            width: "100px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    Email: {
                        ColumnSetup: {
                            field: "Email",
                            width: "225px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    TrainingDate: {
                        ColumnSetup: {
                            field: "TrainingDate",
                            title: "Training Date",
                            width: "150px",
                            format: "{0:MM/dd/yyyy}"
                        },
                        FieldSetup: {
                            editable: false,
                            type: "date"
                        }
                    },
                    TrunkID: {
                        ColumnSetup: {
                            field: "TrunkID",
                            title: "Trunk ID",
                            width: "85px",
                            template: "#: TrunkID #<a id='ViewTrunkLog_#: TrunkID #' class='ui-icon ui-icon-clipboard' style='display:inline-block; float:right;'></a>"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    TrunkIDRange: {
                        ColumnSetup: {
                            field: "TrunkIDRange",
                            title: "Trunk ID Range",
                            width: "175px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    SerialNumber: {
                        ColumnSetup: {
                            field: "SerialNumber",
                            title: "Serial Number",
                            width: "125px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    ModelNumber: {
                        ColumnSetup: {
                            field: "ModelNumber",
                            title: "Model Number"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    ModelDescription: {
                        ColumnSetup: {
                            field: "ModelDescription",
                            title: "Model Description"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    AimTransactionNumber: {
                        ColumnSetup: {
                            field: "AimTransactionNumber",
                            title: "AIM Transaction Number",
                            width: "225px"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    ArchiveReason: {
                        ColumnSetup: {
                            field: "ArchiveReason",
                            title: "Archive Reason"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    AssetComments: {
                        ColumnSetup: {
                            field: "AssetComments",
                            title: "Asset Comments"
                        },
                        FieldSetup: {
                            editable: false
                        }
                    },
                    AssetDisabled: {
                        ColumnSetup: {
                            field: "AssetDisabled",
                            title: "Asset Disabled"
                            //, template: "#= AssetDisabled ? 'Yes' : 'No' #" 
                        },
                        FieldSetup: {
                            editable: false,
                            type: "boolean"
                        }
                    },
                    Command: { command: ["edit"], title: " ", width: "100px" }
                },
                Model: {
                    //These parameters need to stay here as place holders so that they can be set below.
                    id: "",
                    fields: {}
                }
            }

            if (url.match(/query=Managers/)) {

                Options.Columns = [
                    Options.CreateColumns.AccountCode.ColumnSetup,
                    Options.CreateColumns.Position.ColumnSetup,
                    Options.CreateColumns.Rank.ColumnSetup,
                    Options.CreateColumns.FirstName.ColumnSetup,
                    Options.CreateColumns.LastName.ColumnSetup,
                    Options.CreateColumns.Organization.ColumnSetup,
                    Options.CreateColumns.Phone.ColumnSetup,
                    Options.CreateColumns.Email.ColumnSetup,
                    Options.CreateColumns.TrainingDate.ColumnSetup
                //, Options.CreateColumns.Command
                ]
                Options.Model.id = "AccountCode";
                Options.Model.fields = {
                    AccountCode: Options.CreateColumns.AccountCode.FieldSetup,
                    Position: Options.CreateColumns.Position.FieldSetup,
                    Rank: Options.CreateColumns.Rank.FieldSetup,
                    FirstName: Options.CreateColumns.FirstName.FieldSetup,
                    LastName: Options.CreateColumns.FirstName.FieldSetup,
                    Organization: Options.CreateColumns.Organization.FieldSetup,
                    Phone: Options.CreateColumns.Phone.FieldSetup,
                    Email: Options.CreateColumns.Email.FieldSetup,
                    TrainingDate: Options.CreateColumns.TrainingDate.FieldSetup
                }
                Options.Toolbar.Template = Options.Toolbar.CreateTemplate("Unit PWCS Managers");
                LoadGrid("WebService.asmx/JsonManagersList", Options);

            } else if (url.match(/query=Archive/)) {

                Options.Columns = [
                    Options.CreateColumns.TrunkID.ColumnSetup,
                    Options.CreateColumns.TrunkIDRange.ColumnSetup,
                    Options.CreateColumns.SerialNumber.ColumnSetup,
                    Options.CreateColumns.AccountCode.ColumnSetup,
                    Options.CreateColumns.ModelNumber.ColumnSetup,
                    Options.CreateColumns.ModelDescription.ColumnSetup,
                    Options.CreateColumns.AimTransactionNumber.ColumnSetup,
                    Options.CreateColumns.ArchiveReason.ColumnSetup
                //,Options.CreateColumns.Command
                ]
                Options.Model.id = "TrunkID";
                Options.Model.fields = {
                    TrunkID: Options.CreateColumns.TrunkID.FieldSetup,
                    TrunkIDRange: Options.CreateColumns.TrunkIDRange.FieldSetup,
                    SerialNumber: Options.CreateColumns.SerialNumber.FieldSetup,
                    AccountCode: Options.CreateColumns.AccountCode.FieldSetup,
                    ModelNumber: Options.CreateColumns.ModelNumber.FieldSetup,
                    ModelDescription: Options.CreateColumns.ModelDescription.FieldSetup,
                    AimTransactionNumber: Options.CreateColumns.AimTransactionNumber.FieldSetup,
                    ArchiveReason: Options.CreateColumns.ArchiveReason.FieldSetup
                }
                Options.Toolbar.Template = Options.Toolbar.CreateTemplate("Archived Assets");
                LoadGrid("WebService.asmx/ArchivedAssets", Options);

            } else if (url.match(/query=UnaccountedFor/)) {

                Options.Columns = [
                    Options.CreateColumns.TrunkID.ColumnSetup,
                    Options.CreateColumns.TrunkIDRange.ColumnSetup,
                    Options.CreateColumns.SerialNumber.ColumnSetup,
                    Options.CreateColumns.AccountCode.ColumnSetup,
                    Options.CreateColumns.ModelNumber.ColumnSetup,
                    Options.CreateColumns.ModelDescription.ColumnSetup,
                    Options.CreateColumns.AssetComments.ColumnSetup,
                    Options.CreateColumns.AssetDisabled.ColumnSetup
                //, Options.CreateColumns.Command
                ]
                Options.Model.id = "TrunkID";
                Options.Model.fields = {
                    TrunkID: Options.CreateColumns.TrunkID.FieldSetup,
                    TrunkIDRange: Options.CreateColumns.TrunkIDRange.FieldSetup,
                    SerialNumber: Options.CreateColumns.SerialNumber.FieldSetup,
                    AccountCode: Options.CreateColumns.AccountCode.FieldSetup,
                    ModelNumber: Options.CreateColumns.ModelNumber.FieldSetup,
                    ModelDescription: Options.CreateColumns.ModelDescription.FieldSetup,
                    AssetComments: Options.CreateColumns.AssetComments.FieldSetup,
                    AssetDisabled: Options.CreateColumns.AssetDisabled.FieldSetup
                }
                Options.Toolbar.Template = Options.Toolbar.CreateTemplate("Assets Unaccounted For");
                LoadGrid("WebService.asmx/UnaccountedFor", Options);

            } else if (url.match(/query=AllOpenTrunkID/)) {

                Options.Columns = [
                    Options.CreateColumns.TrunkID.ColumnSetup,
                    Options.CreateColumns.TrunkIDRange.ColumnSetup,
                    Options.CreateColumns.SerialNumber.ColumnSetup,
                    Options.CreateColumns.AssetComments.ColumnSetup
                //,Options.CreateColumns.Command
                ]
                Options.Model.id = "TrunkID";
                Options.Model.fields = {
                    TrunkID: Options.CreateColumns.TrunkID.FieldSetup,
                    TrunkIDRange: Options.CreateColumns.TrunkIDRange.FieldSetup,
                    SerialNumber: Options.CreateColumns.SerialNumber.FieldSetup,
                    AssetComments: Options.CreateColumns.AssetComments.FieldSetup
                }
                Options.Toolbar.Template = Options.Toolbar.CreateTemplate("All Open Trunk ID's");
                LoadGrid("WebService.asmx/AllOpenTrunkID", Options);

            } //end url.match if statement

            function LoadGrid(WebService, Options) {

                //alert(JSON.stringify(Options.Columns.AccountCode));

                var Grid = $("#GridArea").kendoGrid({
                    height: 700,

                    toolbar: [
                            Options.Toolbar.Template
                        ],

                    columns: Options.Columns,

                    //prevents the user from entering a second filter criteria per column - http://docs.kendoui.com/api/web/grid#configuration-filterable.extra
                    ////filterable: { extra: false },

                    sortable: true,
                    reorderable: true,
                    groupable: true,

                    //columnMenu: true,

                    pageable: {
                        pageSize: 50,
                        pageSizes: [50, 100, 250],
                        refresh: true
                    },

                    editable: "inline",

                    //batch: true,

                    dataSource: {
                        transport: {
                            read: {
                                //url: "WebService.asmx/JsonManagersList",
                                url: WebService,
                                data: Options.TransportData,
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                type: "POST"
                            },
                            update: {
                                //alert("working");
                                url: WebService,
                                data: Options.TransportData,
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                type: "POST"
                            },
                            destroy: {},
                            parameterMap: function (data, type) {
                                //"type" referes to the transport type - "read", "create", etc...
                                //http://docs.kendoui.com/api/framework/datasource#configuration-transport.create

                                /*
                                The first time the grid is lodaed there is no filter variable:
                                {"take":30,"skip":0,"page":1,"pageSize":30}
                                
                                To avoid extra coding in the back end, let's add a default value for the filter variable if it doesn't exist:  
                                data.filter = "none";

                                Results will be like:
                                {"take":30,"skip":0,"page":1,"pageSize":30,"filter":"none"}
                                */
                                //////                                if (!data.filter) {
                                //////                                    data.filter = "none";
                                //////                                }

                                //After a filter is selected on any given column, a filter array is added to the request:
                                //{"take":30,"skip":0,"page":1,"pageSize":30,"filter":{"filters":[{"field":"TrunkIDRange","operator":"eq","value":"388"}],"logic":"and"}}

                                //If you clear the filter from all columns, the filter variable will still exist as a null value:
                                //{"take":30,"skip":0,"page":1,"pageSize":30,"filter":null}

                                //window.console.log(type);

                                return JSON.stringify(data);
                            }
                        },
                        schema: {
                            data: function (data) {

                                //If "IsError" comes back True, then the "schema.errors" data will be passed into the 
                                //error function below.  
                                if (data.d.IsError == false) {
                                    return data.d.Data;
                                }

                            },
                            total: "d.TotalRows",

                            //If an error occurs with this ajax call or we return an error from the backend function,
                            //the value in this "errors" parameter will be passed into the "error" function below.
                            errors: "d.ErrorMessage",

                            model: Options.Model
                        },
                        error: function (e) {

                            //The "errors" value is set in the "schema.errors" parameter above.
                            ThrowError("#DivTopMessage", e.errors);

                        },
                        //pageSize: 30
                        serverPaging: true
                        //serverFiltering: true
                        //serverSorting: true
                    },

                    //"dataBound" acts like the "complete" function in a jQuery ajax call.  When all the data has been
                    //loaded into the grid, this function will fire.
                    dataBound: function (data) {
                        $(".link").click(function () {
                            var TextValue = $(this).text();
                            //alert(AccountCode);
                            sessionStorage.IsPostData = "True"
                            sessionStorage.AccountCode = TextValue.toString();
                            window.open("Default.aspx", "_self");
                        });

                        $("[id^='ViewTrunkLog_']").hover(function () {
                            $(this).toggleClass("clipboard")
                        }).click(function () {
                            var TrunkID = $(this).attr("id");
                            TrunkID = TrunkID.replace("ViewTrunkLog_", "");
                            //this function is located in Default_Page.js
                            GetTrunkingSystemLogByID(TrunkID, "#TrunkingSystemLogByIDDiv");
                        });

                        //need to add something here to clear any errors written to the DivTopMessage area...

                    }
                });

            } //end LoadGrid()
        });

        function InitEvents() {

            alert("test");

        } //end InitEvents()
    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    <UserControl:PageHeader runat="server" />
    <UserControl:SiteNavigation runat="server" />
    
    <div id="DivTopMessage" style=""></div>

    <div id="ContentDiv" style="" runat="server"></div>

    <div id="DivBottomMessage" runat="server"></div>

    <div id="TrunkingSystemLogByIDDiv" style="text-align:center;"></div>

    <div id="GridArea"></div>
    
    </form>

</body>
</html>
