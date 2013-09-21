Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data.CommandBehavior
Imports System.Web.HttpUtility
Imports System.Web.Script.Serialization


Public Class WebServiceFunctions
    'Converted to Stored Procedure - YES
    'Pushed to Production - YES
    Public Shared Function AccountList() As String

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("SelectActiveAccountList")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim str As String = ""
        Dim AppointmentLetterTD As String = ""
        Dim InventoryTD As String = ""
        Dim ReplacementPlanTD As String = ""
        Dim counter As Integer = 0

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                '.Add(New SqlParameter("@GLCode", GLCode))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            str = str & "<div class='FormLabel' style='text-align:center; border-bottom:1px solid #333; margin-bottom:5px;'>Accounts</div>"
            str = str & "<table id='AccountListTable' class='AccountListTableClass'>"

            If objDataReader.HasRows Then

                While objDataReader.Read()
                    If objDataReader("account_code").ToString <> "" Or objDataReader("account_code").ToString Is DBNull.Value Then
                        str = str & "<tr id='" & objDataReader("account_code") & "'>" & _
                            "<td><b>" & objDataReader("account_code").ToString & "</b></td>" & _
                            "<td class='SmallFont " & StyleChange.SetColor(objDataReader("appt_ltr").ToString) & "' style=''>A</td>" & _
                            "<td class='SmallFont " & StyleChange.SetColor(objDataReader("inventory").ToString) & "' style=''>I</td>" & _
                            "<td class='SmallFont " & StyleChange.SetColor(objDataReader("account_validation").ToString) & "' style=''>R</td>" & _
                            "</tr>"
                    End If
                    counter = counter + 1
                End While

            Else

                str = str & "<tr><td>No Records Found!</td></tr>"

            End If

            objDataReader.Close()

            str = str & "</table>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    'Converted to Stored Procedure - YES
    'Pushed to Production - YES
    Public Shared Function TrunkIdAndSerialNumber(ByVal Account As String) As String

        Account = HtmlEncode(Account)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("SelectAccountAssets")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim str As String = ""
        Dim counter As Integer = 0
        Dim rowColor As String = ""

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@Account", Account))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            str = str & "<table id='TrunkAndSerialNumberTable' class='TrunkIDAndSerialNumberClass'><tr class='borderOnly'>" & _
                "<th><b>Trunk ID</b></th>" & _
                "<th><b>Serial Number</b></th>" & _
                "</tr>"

            While objDataReader.Read()

                If objDataReader("assetDisabled") = "Yes" Then
                    'grayRow is a class defined on css/style.css
                    rowColor = "grayRow"
                ElseIf objDataReader("trunkID") = "0" And objDataReader("assetDisabled") <> "Yes" Then
                    'lightRed is a class defined on css/style.css
                    rowColor = "lightRed"
                Else
                    'borderOnly is a class defined on css/style.css
                    rowColor = "borderOnly"
                End If
                str = str & "<tr id='TrunkSNRow_" & objDataReader("serialNum") & "' class='" & rowColor & "'>" & _
                    "<td>" & objDataReader("trunkID").ToString & "</td>" & _
                    "<td>" & objDataReader("serialNum").ToString & "</td>" & _
                "</tr>"

                counter = counter + 1
            End While

            str = str & "<tr id='AssetCount'>" & _
                "<td><strong>Total: </strong></td>" & _
                "<td><strong>" & counter & " assets</strong></td>" & _
            "</tr></table>"

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    'Converted to Stored Procedure - Yes
    'Pushed to Production - YES
    Public Shared Function SerialNumberSearch(ByVal Value As String) As String

        Value = HtmlEncode(Value)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("SearchAllAssets")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim str As String = ""
        Dim TrClass As String = ""

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@Value", "%" & Value & "%"))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            str = str & "<div class='lightGrayBorder' style=''>" & _
            "<table id='SNID' class='SearchResultsTable' align='center'><tr>" & _
            "<th id='SNID_Account'><b>Account</b></th>" & _
            "<th id='SNID_TrunkID'><b>Trunk ID</b></th>" & _
            "<th><b>Trunk ID Range</b></th>" & _
            "<th><b>Serial Number</b></th>" & _
            "<th><b>Model Number</b></th>" & _
            "<th><b>Model Description</b></th>" & _
            "<th><b>Asset Comments</b></th>" & _
            "<th><b>Status</b></th>" & _
            "</tr>"

            While objDataReader.Read()

                If objDataReader("status") = "Archived" Then
                    'We're giving this row a fake ID to avoid breaking our jQuery in the Default_Page.js file
                    'and we are adding this style to show the user that this row cannot be clicked on.
                    str = str & "<tr id='FAKE_' style='background-color:#FF9999; cursor:default;'>"
                Else
                    str = str & "<tr id='" & objDataReader("account") & "_" & objDataReader("serialNum") & "'>"
                End If

                str = str & "<td>" & CustomFunctions.CheckIsNull(objDataReader("account").ToString) & "</td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("trunkID").ToString) & _
                        "<a id='ViewTrunkLog_" & objDataReader("trunkID") & "' style='float:right;' class='ui-icon ui-icon-clipboard' alt='View Log' title='View Log'></a></td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("trunkIDRange").ToString) & "</td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("serialNum").ToString) & "</td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("modelNum").ToString) & "</td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("modelDesc").ToString) & "</td>" & _
                    "<td>" & CustomFunctions.CheckIsNull(objDataReader("assetComments").ToString) & "</td>" & _
                    "<td>" & objDataReader("status") & "</td>"
                str = str & "</tr>"

            End While

            objDataReader.Close()

            str = str & "</table></div>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    'Converted to Stored Procedure - YES
    'Pushed to Production - YES
    Public Shared Function ChangeManagerStatus(ByVal Status As String, ByVal ManagerID As String) As ReturnObject

        Status = HtmlEncode(Status)
        ManagerID = HtmlEncode(ManagerID)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spUpdateManagerStatus")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim MyRecord As New ReturnObject
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OriginalValue As String = ""
        Dim str As String = ""

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@Status", Status))
                .Add(New SqlParameter("@ManagerID", ManagerID))
                '.Add(New SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.RecordsAffected = 1 Then
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Manager was successfully deleted!"
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "A fatal error occured!  Please refresh the page and try again..."
                MyRecord.str = ""
            End If

            objDataReader.Close()

            'str = str & "Manager Addition Successful"

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        If Status = "True" Then OriginalValue = "False" Else OriginalValue = "True"
        Action = "{'status':'" & Status & "', 'record':" & ManagerID & "}"
        Reason = "Updated Manager Record Number " & ManagerID & " to " & Status
        CustomFunctions.AddToLog("managers", "active", ManagerID, Status, "Manager Status Change", Action, Reason, OriginalValue, Status)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    'Converted to Stored Procedure - YES
    'Pushed to Production - YES
    Public Shared Function TrunkingSystemLogByID(ByVal TrunkID As String) As String

        TrunkID = HtmlEncode(TrunkID)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spTransmitHistoryByID")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim str As String = ""

        Try
            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TrunkID", TrunkID))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            str = str & "<p>Results contain the last 20 transmits recorded for ID " & TrunkID & ".</p>"

            str = str & "<table class='MainStyle' align='center'><tr>" & _
                "<th>Transmit Date</th>" & _
                "<th>Trunk ID</th>" & _
                "<th>Serial Number</th>" & _
                "<th>Target Group Name</th>" & _
                "</tr>"

            If objDataReader.HasRows Then
                While objDataReader.Read()
                    str = str & "<tr>" & _
                        "<td>" & objDataReader("TransmitDate") & "</td>" & _
                        "<td>" & objDataReader("Radio") & "</td>" & _
                        "<td>" & objDataReader("SerialNumber") & "</td>" & _
                        "<td>" & objDataReader("TargetGroupName") & "</td>" & _
                        "</tr>"
                End While
            Else
                str = str & "<tr>" & _
                    "<td colspan='4'>0 records returned for " & TrunkID & "</td>" & _
                    "</tr>"
            End If

            str = str & "</table>"

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message

        Finally
            objConnection.Close()
        End Try

        Return str
        'Return strSQLQuery

    End Function

    Public Shared Function AccountInfo(ByVal Account As String) As AccountInfoObject

        Account = HtmlEncode(Account)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String

        strSQLQuery = "SELECT * FROM accounts WHERE (account_code = @Account) AND (active = 'True')"

        Dim str As String = ""
        Dim counter As Integer = 0
        Dim MyRecord As New AccountInfoObject

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account.ToUpper))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            While objDataReader.Read()
                MyRecord.ID = objDataReader("ID")
                MyRecord.AccountCode = objDataReader("account_code")
                MyRecord.Organization = objDataReader("organization")
                If IsDBNull(objDataReader("appt_ltr")) Then MyRecord.AppointmentLetter = "NULL" Else MyRecord.AppointmentLetter = objDataReader("appt_ltr")
                If IsDBNull(objDataReader("inventory")) Then MyRecord.Inventory = "NULL" Else MyRecord.Inventory = objDataReader("inventory")
                If IsDBNull(objDataReader("account_validation")) Then MyRecord.AccountValidation = "NULL" Else MyRecord.AccountValidation = objDataReader("account_validation")
                If IsDBNull(objDataReader("email_acct_val_1")) Then MyRecord.EmailAccountValidation1 = "NULL" Else MyRecord.EmailAccountValidation1 = objDataReader("email_acct_val_1")
                If IsDBNull(objDataReader("email_acct_val_2")) Then MyRecord.EmailAccountValidation2 = "NULL" Else MyRecord.EmailAccountValidation2 = objDataReader("email_acct_val_2")
                If IsDBNull(objDataReader("email_acct_val_3")) Then MyRecord.EmailAccountValidation3 = "NULL" Else MyRecord.EmailAccountValidation3 = objDataReader("email_acct_val_3")
                If IsDBNull(objDataReader("email_inv_1")) Then MyRecord.EmailInventory1 = "NULL" Else MyRecord.EmailInventory1 = objDataReader("email_inv_1")
                If IsDBNull(objDataReader("email_inv_2")) Then MyRecord.EmailInventory2 = "NULL" Else MyRecord.EmailInventory2 = objDataReader("email_inv_2")
                If IsDBNull(objDataReader("email_inv_3")) Then MyRecord.EmailInventory3 = "NULL" Else MyRecord.EmailInventory3 = objDataReader("email_inv_3")
                If IsDBNull(objDataReader("email_appt_ltr_1")) Then MyRecord.EmailAppointmentLetter1 = "NULL" Else MyRecord.EmailAppointmentLetter1 = objDataReader("email_appt_ltr_1")
                If IsDBNull(objDataReader("email_appt_ltr_2")) Then MyRecord.EmailAppointmentLetter2 = "NULL" Else MyRecord.EmailAppointmentLetter2 = objDataReader("email_appt_ltr_2")
                If IsDBNull(objDataReader("email_appt_ltr_3")) Then MyRecord.EmailAppointmentLetter3 = "NULL" Else MyRecord.EmailAppointmentLetter3 = objDataReader("email_appt_ltr_3")
                If IsDBNull(objDataReader("email_training_1")) Then MyRecord.EmailTraining1 = "NULL" Else MyRecord.EmailTraining1 = objDataReader("email_training_1")
                If IsDBNull(objDataReader("email_training_2")) Then MyRecord.EmailTraining2 = "NULL" Else MyRecord.EmailTraining2 = objDataReader("email_training_2")
                If IsDBNull(objDataReader("email_training_3")) Then MyRecord.EmailTraining3 = "NULL" Else MyRecord.EmailTraining3 = objDataReader("email_training_3")
                If IsDBNull(objDataReader("account_comments")) Then MyRecord.AccountComments = "NULL" Else MyRecord.AccountComments = objDataReader("account_comments")

                'MyRecord = objDataReader("")

                counter = counter + 1
            End While
            objDataReader.Close()

        Catch ex As Exception
            'data.str = "Error: " & ex.Message
            'data.str = ex.Message
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function ManagersInformation(ByVal Account As String) As String

        Account = HtmlEncode(Account)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String
        'strSQLQuery = "SELECT * FROM [PWCS].[dbo].[assets] INNER JOIN [PWCS].[dbo].[accounts] ON [assets].account = [accounts].account_code WHERE account = @Account ORDER BY aims_sn"
        strSQLQuery = "SELECT * FROM managers WHERE account_code = @Account AND active = 'True' ORDER BY position DESC"

        Dim str As String = ""
        Dim TrainedDate As String = ""
        'Dim counter As Integer = 0

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account.ToUpper))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<p style='text-align:center;'><b>Account Managers</b><hr /><p>"

            str = str & "<table id='ManagersInformationTable' class='MainStyle' align='center' style='width:95%;'><tr>" & _
                "<th style=''><b>Position</b></th>" & _
                "<th style=''><b>Rank</b></th>" & _
                "<th style=''><b>First Name</b></th>" & _
                "<th style=''><b>Last Name</b></th>" & _
                "<th style=''><b>Organization</b></th>" & _
                "<th style=''><b>Phone</b></th>" & _
                "<th style=''><b>Email</b></th>" & _
                "<th style=''><b>Training Date</b></th>" & _
                "</tr>"

            While objDataReader.Read()
                If IsDBNull(objDataReader("trained")) Then TrainedDate = "Null" Else TrainedDate = objDataReader("trained")
                'CheckIsNull() is a user-defined function //Stan Holmes Awesome...
                str = str & "<tr>" & _
                "<td><span id='position_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("position").ToString) & "</span>" & _
                    "<a id='DeleteManager_" & objDataReader("ID") & "' style='float:right;' class='ui-icon ui-icon-circle-close' alt='Delete Manager' title='Delete Manager'></a></td>" & _
                "<td><span id='rank_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("rank").ToString) & "</span></td>" & _
                "<td><span id='fname_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("fname").ToString) & "</span></td>" & _
                "<td><span id='lname_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("lname").ToString) & "</span></td>" & _
                "<td><span id='org_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("org").ToString) & "</span></td>" & _
                "<td><span id='phone_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("phone").ToString) & "</span></td>" & _
                "<td><span id='email_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("email").ToString) & "</span></td>" & _
                "<td class='" & StyleChange.SetColor(objDataReader("trained").ToString) & "'>" & _
                    "<span id='trained_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(TrainedDate) & "</span></td>" & _
                "</tr>"
            End While
            str = str & "</table>" & _
                "<div style='padding:5px;'>" & _
                    "<div style='width:150px; margin-left:auto; margin-right:auto;'>" & _
                        "<p style='text-align:center;'>" & _
                            "<input id='addManager' type='button' value='Add Manager' />" & _
                        "</p>" & _
                    "</div>" & _
                "</div>"

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function SerialNumberInformation(ByVal SerialNum As String) As String

        SerialNum = HtmlEncode(SerialNum)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim str As String = ""
        Dim AssetDisabledDate As String = ""
        'Dim counter As Integer = 0

        'strSQLQuery = "SELECT * FROM [PWCS].[dbo].[assets] INNER JOIN [PWCS].[dbo].[accounts] ON [assets].account = [accounts].account_code WHERE account = @Account ORDER BY aims_sn"
        strSQLQuery = "SELECT * FROM assets WHERE serialNum = @SerialNum "

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNum", SerialNum.ToUpper))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<div style=''>" & _
                    "<p style='text-align:center;'>" & _
                        "<b>Asset Information</b>" & _
                        "<hr />" & _
                    "<p>" & _
                "</div>"

            str = str & "<table class='MainStyle' align='center' style='width:95%;'><tr>" & _
                "<th style=''><b>Trunk ID</b></th>" & _
                "<th style=''><b>Serial Number</b></th>" & _
                "<th style=''><b>Model Number</b></th>" & _
                "<th style=''><b>Model Description</b></th>" & _
                "<th style=''><b>9600B</b></th>" & _
                "<th style=''><b>AES</b></th>" & _
                "<th style=''><b>OTAR</b></th>" & _
                "<th style=''><b>OTAP</b></th>" & _
                "</tr>"

            If objDataReader.HasRows Then

                While objDataReader.Read()
                    If IsDBNull(objDataReader("AssetDisabledDate")) Then
                        AssetDisabledDate = "NULL"
                    ElseIf objDataReader("AssetDisabledDate") Is Nothing Then
                        AssetDisabledDate = "NULL"
                    Else
                        AssetDisabledDate = objDataReader("AssetDisabledDate").ToShortDateString()
                    End If
                    'CheckIsNull() is a user-defined function //Stan Holmes Awesome...
                    str = str & "<tr>" & _
                    "<td id='trunkID_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("trunkID").ToString) & _
                        "<a id='ViewTrunkLog_" & objDataReader("trunkID") & "' style='float:right;' class='ui-icon ui-icon-clipboard' alt='View Log' title='View Log'></a></td>" & _
                    "<td><span id='serialNum_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("serialNum").ToString) & "</span></td>" & _
                    "<td><span id='modelNum_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("modelNum").ToString) & "</span></td>" & _
                    "<td><span id='modelDesc_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("modelDesc").ToString) & "</span></td>" & _
                    "<td><span id='9600B_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("9600B").ToString) & "</span></td>" & _
                    "<td><span id='AES_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("AES").ToString) & "</span></td>" & _
                    "<td><span id='OTAR_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("OTAR").ToString) & "</span></td>" & _
                    "<td><span id='OTAP_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("OTAP").ToString) & "</span></td>" & _
                    "</tr></table><br />"

                    'str = str & "<tr>" & _
                    '"<td colspan='2'><b>Asset Comments</b></td>" & _
                    '"<td colspan='6'><span id='assetComments_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("assetComments").ToString) & "</span></td>" & _
                    '"</tr></table><br />"

                    str = str & "<div style='padding:2px 10px 2px 10px; verticle-align:center;'>" & _
                            "<div class='newItem' style='float:left;'></div>" & _
                            "<div style='position:relative; top:3px'><span id='AssetComments_" & objDataReader("ID") & "' class='link'>Add New Asset Comment</span></div>" & _
                        "</div>" & _
                        "<div id='AdditionalAssetComments' style='padding:2px 10px 2px 10px;'></div>"

                    str = str & "<p style='text-align:center;'><b>Disabled Status</b><hr /><p>" & _
                    "<table class='MainStyle' align='center' style='width:95%;'><tr>" & _
                    "<th style='' width='100'><b>Disabled</b></th>" & _
                    "<th style='' width='125'><b>Date</b></th>" & _
                    "<th style='' width='300'><b>Comments</b></th>" & _
                    "</tr><tr>" & _
                    "<td><span id='assetDisabled_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("assetDisabled").ToString) & "</span></td>" & _
                    "<td><span id='assetDisabledDate_" & objDataReader("ID") & "'>" & AssetDisabledDate & "</span></td>" & _
                    "<td><span id='assetDisabledComments_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("assetDisabledComments").ToString) & "</span></td>" & _
                    "</tr>"

                End While
            Else
                str = str & "<tr>" & _
                    "<td colspan='8'>Please select an asset to view its information <span id='AssetComments_False'></span> </td>" & _
                    "</tr>"
            End If
            str = str & "</table>"

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function SerialNumberMaintenanceHistory(ByVal SerialNum As String) As String

        SerialNum = HtmlEncode(SerialNum)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim MaintenanceDate As String = ""
        Dim TotalCost As Decimal = 0
        Dim CheckForNullCost As Decimal = 0

        strSQLQuery = "SELECT * FROM maintenance WHERE sn = @SerialNum ORDER BY date "

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNum", SerialNum.ToUpper))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<p style='text-align:center;padding-right:10px;'><b>Asset Maintenance History</b>" & _
                "<a id='AddMaintenanceActionLink' style='float:right;' class='ui-icon ui-icon-circle-plus' title='Add Maintenance Action' alt='Add Maintenance Action'></a><hr /><p>"

            str = str & "<table class='MainStyle' align='center' style='width:95%;'><tr>" & _
                "<th style=''><b>Date</b></th>" & _
                "<th style=''><b>Invoice</b></th>" & _
                "<th style=''><b>Description</b></th>" & _
                "<th style=''><b>Cost</b></th>" & _
                "</tr>"
            If objDataReader.HasRows Then
                While objDataReader.Read()
                    If IsDBNull(objDataReader("date")) Then MaintenanceDate = "Null" Else MaintenanceDate = objDataReader("date")
                    If IsDBNull(objDataReader("cost")) Then CheckForNullCost = 0 Else CheckForNullCost = objDataReader("cost")

                    'CheckIsNull() is a user-defined function //Stan Holmes Awesome...
                    str = str & "<tr>" & _
                    "<td id='date_" & objDataReader("ID") & "'>" & MaintenanceDate & "</td>" & _
                    "<td id='invoice_" & objDataReader("ID") & "'>" & CustomFunctions.CheckIsNull(objDataReader("invoice").ToString) & "</td>" & _
                    "<td id='description_" & objDataReader("ID") & "' class='MaintenanceDescription'>" & CustomFunctions.CheckIsNull(objDataReader("description").ToString) & "</td>" & _
                    "<td id='cost_" & objDataReader("ID") & "' align='right'>" & CustomFunctions.CheckIsNull(objDataReader("cost").ToString) & "</td>" & _
                    "</tr>"

                    TotalCost = TotalCost + CheckForNullCost
                End While
                str = str & "</table><br /><hr />"
                str = str & "<p style='text-align:right; padding-right:10px;'>Total Maintenance Cost to Date: &nbsp;&nbsp;<b>$" & TotalCost & "</b></p>"
            Else
                str = str & "<tr><td colspan='4'>No maintenance actions have been logged for this asset</td></tr>" & _
                    "</table>"
            End If

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function AddManager(ByVal NewPosition As String, ByVal NewRank As String, ByVal NewFirstName As String,
                               ByVal NewLastName As String, ByVal NewOrg As String, ByVal NewPhone As String,
                               ByVal NewEmail As String, ByVal NewTrainingDate As String, ByVal Account As String) As AccountInfoObject

        NewPosition = HtmlEncode(NewPosition)
        NewRank = HtmlEncode(NewRank)
        NewFirstName = HtmlEncode(NewFirstName)
        NewLastName = HtmlEncode(NewLastName)
        NewOrg = HtmlEncode(NewOrg)
        NewPhone = HtmlEncode(NewPhone)
        NewEmail = HtmlEncode(NewEmail)
        NewTrainingDate = HtmlEncode(NewTrainingDate)
        Account = HtmlEncode(Account)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim Action As String = "{'Account':'" & Account.ToUpper().Trim() & "', 'Position':'" & NewPosition.Trim() & "', 'Rank':'" & NewRank.Trim() & "', 'FirstName':'" & NewFirstName.Trim() & "', 'LastName':'" & NewLastName.Trim() & "', 'Org':'" & NewOrg.Trim() & "', 'Phone':'" & NewPhone.Trim() & "', 'Email':'" & NewEmail.Trim() & "'}"
        Dim ActionType As String = ""
        Dim Reason As String = ""
        Dim MyRecord As New AccountInfoObject

        strSQLQuery = "INSERT INTO dbo.managers (account_code, position, rank, fname, lname, org, phone, email, trained, modified_by, modified_date) " & _
            "VALUES (@Account, @NewPosition, @NewRank, @NewFirstName, @NewLastName, @NewOrg, @NewPhone, @NewEmail, @NewTrainingDate, @modified_by, @modified_date)"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account.ToUpper().Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewPosition", NewPosition.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewRank", NewRank.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewFirstName", NewFirstName.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewLastName", NewLastName.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewOrg", NewOrg.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewPhone", NewPhone.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@NewEmail", NewEmail.Trim()))
                If NewTrainingDate = "" Then
                    .Add(New SqlParameter("@NewTrainingDate", DBNull.Value))
                Else
                    .Add(New SqlParameter("@NewTrainingDate", CDate(NewTrainingDate.Trim())))
                End If
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then

                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "The manager " & NewFirstName & " " & NewLastName & " has been successfully added to account " & Account & "!"
                ActionType = "Manager Addition"
                Reason = "Added Manager " & NewFirstName & " " & NewLastName & " to Account " & Account.ToUpper()

            Else

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "The addition of " & NewFirstName & " " & NewLastName & " for account " & Account & " failed!  0 records were affected!"
                MyRecord.str = ""
                ActionType = "Manager Addition Failed"
                Reason = "The manager " & NewFirstName & " " & NewLastName & " was not added to account " & Account.ToUpper() & ". Insert statement failed to execute!"

            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = "There was an exception with WebService.asmx/AddManager()!"
        Finally
            objConnection.Close()
        End Try

        '*****Log Events

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", Account, "managers", "account_code", Account)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("managers", "account_code", GetRecordID.str, Account.ToUpper(), ActionType, Action, Reason, "NA", "NA")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value

        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function CheckAccountStatus(ByVal Status As String, ByVal Account As String) As AccountInfoObject

        Status = HtmlEncode(Status)
        Account = HtmlEncode(Account)

        Dim objConnection As SqlConnection
        objConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As SqlCommand

        Dim strSQLQuery As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OriginalValue As String = ""
        Dim str As String = ""
        Dim MyRecord As New AccountInfoObject
        Dim AccountStatus As New ReturnObject

        'need to check and see if there are still assets that belong to this account, if so 
        'we do not want the user to be able to delete the account because those assets will end up in limbo
        strSQLQuery = "SELECT serialNum FROM dbo.assets WHERE account = @Account"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New SqlParameter("@Account", Account))
                .Add(New SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows Then

                'return a string here telling the user that there are still assets assigned to this account
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Account " & Account & " still has assets assigned to it!"
                MyRecord.str = "An account may only be deleted if all assigned assets are transferred to another account or removed from the system."

                '*****Log Events
                Action = "{'Account':'" & Account & "', 'Status':'" & Status & "'}"
                Reason = HttpContext.Current.Request.ServerVariables("AUTH_USER") & " attempted to delete account " & Account & " but there were still assets assigned to it! "

                CustomFunctions.AddToLog("assets", "account", "", Account, "Account Status Change Failed", Action, Reason, "", "")
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events

            Else

                '*****Log events for this condition are located within the CustomFunctions.ChangeAccountStatus() function
                AccountStatus = CustomFunctions.ChangeAccountStatus(Account, Status)
                MyRecord.str = AccountStatus.str
                MyRecord.ErrorMessage = AccountStatus.ErrorMessage

                If AccountStatus.IsError = False Then
                    MyRecord.IsError = False
                Else
                    MyRecord.IsError = True
                End If

            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = "An exception has occured in WebService.asmx/CheckAccountStatus()"
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function AccountInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String, ByVal new_value As String) As jeipRecord2

        orig_value = HtmlEncode(orig_value)
        new_value = HtmlEncode(new_value)

        Dim MyRecord As New jeipRecord2
        Dim DataColumn As String = ""
        Dim RecordNumber As Integer = 0
        Dim Action As String = ""
        Dim Reason As String = ""

        If id.StartsWith("AppointmentLetter_") Then
            DataColumn = "appt_ltr"
            RecordNumber = id.Replace("AppointmentLetter_", "")
            Reason = "Updated Account Appointment Letter Date"

        ElseIf id.StartsWith("EmailAppointmentLetter1_") Then
            DataColumn = "email_appt_ltr_1"
            RecordNumber = id.Replace("EmailAppointmentLetter1_", "")
            Reason = "Updated Account First Email Appointment Letter Date"

        ElseIf id.StartsWith("EmailAppointmentLetter2_") Then
            DataColumn = "email_appt_ltr_2"
            RecordNumber = id.Replace("EmailAppointmentLetter2_", "")
            Reason = "Updated Account Second Email Appointment Letter Date"

        ElseIf id.StartsWith("EmailAppointmentLetter3_") Then
            DataColumn = "email_appt_ltr_3"
            RecordNumber = id.Replace("EmailAppointmentLetter3_", "")
            Reason = "Updated Account Third Email Appointment Letter Date"

        ElseIf id.StartsWith("Inventory_") Then
            DataColumn = "inventory"
            RecordNumber = id.Replace("Inventory_", "")
            Reason = "Updated Account Inventory Date"

        ElseIf id.StartsWith("EmailInventory1_") Then
            DataColumn = "email_inv_1"
            RecordNumber = id.Replace("EmailInventory1_", "")
            Reason = "Updated Account First Email Inventory Date"

        ElseIf id.StartsWith("EmailInventory2_") Then
            DataColumn = "email_inv_2"
            RecordNumber = id.Replace("EmailInventory2_", "")
            Reason = "Updated Account Second Email Inventory Date"

        ElseIf id.StartsWith("EmailInventory3_") Then
            DataColumn = "email_inv_3"
            RecordNumber = id.Replace("EmailInventory3_", "")
            Reason = "Updated Account Third Email Inventory Date"

        ElseIf id.StartsWith("AccountValidation_") Then
            DataColumn = "account_validation"
            RecordNumber = id.Replace("AccountValidation_", "")
            Reason = "Updated Account Validation Date"

        ElseIf id.StartsWith("EmailAccountValidation1_") Then
            DataColumn = "email_acct_val_1"
            RecordNumber = id.Replace("EmailAccountValidation1_", "")
            Reason = "Updated Account First Email Validation Date"

        ElseIf id.StartsWith("EmailAccountValidation2_") Then
            DataColumn = "email_acct_val_2"
            RecordNumber = id.Replace("EmailAccountValidation2_", "")
            Reason = "Updated Account Second Email Validation Date"

        ElseIf id.StartsWith("EmailAccountValidation3_") Then
            DataColumn = "email_acct_val_3"
            RecordNumber = id.Replace("EmailAccountValidation3_", "")
            Reason = "Updated Account Third Email Validation Date"

        ElseIf id.StartsWith("AccountComments_") Then
            DataColumn = "account_comments"
            RecordNumber = id.Replace("AccountComments_", "")
            Reason = "Updated Account Comments"

        ElseIf id.StartsWith("Organization_") Then
            DataColumn = "organization"
            RecordNumber = id.Replace("Organization_", "")
            Reason = "Updated Account Organization"

        End If

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String

        strSQLQuery = "UPDATE dbo.accounts SET " & DataColumn & " = @DataColumn, modified_by = @modified_by, modified_date = @modified_date WHERE ID = @RecordNumber"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                If new_value <> "" Then
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", new_value))
                Else
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", DBNull.Value))
                End If
                .Add(New System.Data.SqlClient.SqlParameter("@RecordNumber", RecordNumber))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

            MyRecord.is_error = False
            MyRecord.error_text = "None"
            MyRecord.html = new_value

        Catch ex As Exception
            MyRecord.is_error = True
            MyRecord.error_text = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        'Action = "Adjusted asset information for record: [" & RecordNumber & "] on column: [" & DataColumn & "] in table: [assets]"
        Action = "{'table':'accounts', 'column':'" & DataColumn & "', 'record':" & RecordNumber & "}"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetUniqueValue As New ReturnObject
        GetUniqueValue = CustomFunctions.RecordIDFromTable(RecordNumber, "", "assets", "serialNum", RecordNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("accounts", DataColumn, RecordNumber, GetUniqueValue.str, "Account Update", Action, Reason, orig_value, new_value)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function ManagerInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String, ByVal new_value As String) As jeipRecord2

        orig_value = HtmlEncode(orig_value)
        new_value = HtmlEncode(new_value)

        Dim MyRecord As New jeipRecord2
        Dim DataColumn As String = ""
        Dim RecordNumber As Integer = 0
        Dim Action As String = ""
        Dim Reason As String = ""

        If id.StartsWith("position_") Then
            DataColumn = "position"
            RecordNumber = id.Replace("position_", "")
            Reason = "Updated Manager Position"

        ElseIf id.StartsWith("rank_") Then
            DataColumn = "rank"
            RecordNumber = id.Replace("rank_", "")
            Reason = "Updated Manager Rank"

        ElseIf id.StartsWith("fname_") Then
            DataColumn = "fname"
            RecordNumber = id.Replace("fname_", "")
            Reason = "Updated Manager First Name"

        ElseIf id.StartsWith("lname_") Then
            DataColumn = "lname"
            RecordNumber = id.Replace("lname_", "")
            Reason = "Updated Manager Last Name"

        ElseIf id.StartsWith("org_") Then
            DataColumn = "org"
            RecordNumber = id.Replace("org_", "")
            Reason = "Updated Manager Organization"

        ElseIf id.StartsWith("phone_") Then
            DataColumn = "phone"
            RecordNumber = id.Replace("phone_", "")
            Reason = "Updated Manager Phone Number"

        ElseIf id.StartsWith("email_") Then
            DataColumn = "email"
            RecordNumber = id.Replace("email_", "")
            Reason = "Updated Manager Email"

        ElseIf id.StartsWith("trained_") Then
            DataColumn = "trained"
            RecordNumber = id.Replace("trained_", "")
            Reason = "Updated Manager Training Date"

        End If

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String

        strSQLQuery = "UPDATE dbo.managers SET " & DataColumn & " = @DataColumn, modified_by = @modified_by, modified_date = @modified_date WHERE ID = @RecordNumber"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                If new_value <> "" Then
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", new_value))
                Else
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", DBNull.Value))
                End If
                .Add(New System.Data.SqlClient.SqlParameter("@RecordNumber", RecordNumber))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

            MyRecord.is_error = False
            MyRecord.error_text = "None"
            MyRecord.html = new_value

        Catch ex As Exception
            MyRecord.is_error = True
            MyRecord.error_text = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        'Action = "Adjusted asset information for record: [" & RecordNumber & "] on column: [" & DataColumn & "] in table: [assets]"
        Action = "{'table':'managers', 'column':'" & DataColumn & "', 'record':" & RecordNumber & "}"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetUniqueValue As New ReturnObject
        GetUniqueValue = CustomFunctions.RecordIDFromTable(RecordNumber, "", "assets", "serialNum", RecordNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("managers", DataColumn, RecordNumber, GetUniqueValue.str, "Manager Update", Action, Reason, orig_value, new_value)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function SerialNumberInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String, ByVal new_value As String) As jeipRecord2

        orig_value = HtmlEncode(orig_value)
        new_value = HtmlEncode(new_value)

        Dim MyRecord As New jeipRecord2
        Dim DataColumn As String = ""
        Dim RecordNumber As Integer = 0
        Dim Action As String = ""
        Dim Reason As String = ""

        If id.StartsWith("9600B_") Then
            DataColumn = "[9600B]"
            RecordNumber = id.Replace("9600B_", "")
            Reason = "Updated Hardware Information"

        ElseIf id.StartsWith("serialNum_") Then
            DataColumn = "serialNum"
            RecordNumber = id.Replace("serialNum_", "")
            Reason = "Updated Serial Number"
            new_value = new_value.ToUpper

        ElseIf id.StartsWith("modelNum_") Then
            DataColumn = "modelNum"
            RecordNumber = id.Replace("modelNum_", "")
            Reason = "Updated Model Number"
            new_value = new_value.ToUpper

        ElseIf id.StartsWith("modelDesc_") Then
            DataColumn = "modelDesc"
            RecordNumber = id.Replace("modelDesc_", "")
            Reason = "Updated Model Description"
            new_value = new_value.ToUpper

        ElseIf id.StartsWith("AES_") Then
            DataColumn = "AES"
            RecordNumber = id.Replace("AES_", "")
            Reason = "Updated Hardware Information"

        ElseIf id.StartsWith("OTAR_") Then
            DataColumn = "OTAR"
            RecordNumber = id.Replace("OTAR_", "")
            Reason = "Updated Hardware Information"

        ElseIf id.StartsWith("OTAP_") Then
            DataColumn = "OTAP"
            RecordNumber = id.Replace("OTAP_", "")
            Reason = "Updated Hardware Information"

        ElseIf id.StartsWith("AssetComments_") Then
            DataColumn = "assetComments"
            RecordNumber = id.Replace("AssetComments_", "")
            Reason = "Updated Asset Comment Field"

        ElseIf id.StartsWith("assetDisabled_") Then
            DataColumn = "assetDisabled"
            RecordNumber = id.Replace("assetDisabled_", "")
            Reason = "Updated Asset Disabled Status"

        ElseIf id.StartsWith("assetDisabledDate_") Then
            DataColumn = "assetDisabledDate"
            RecordNumber = id.Replace("assetDisabledDate_", "")
            Reason = "Updated Asset Disabled Date"

        ElseIf id.StartsWith("assetDisabledComments_") Then
            DataColumn = "assetDisabledComments"
            RecordNumber = id.Replace("assetDisabledComments_", "")
            Reason = "Updated Asset Disabled Comments"

        End If

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String

        strSQLQuery = "UPDATE dbo.assets SET " & DataColumn & " = @DataColumn, modified_by = @modified_by, modified_date = @modified_date WHERE ID = @RecordNumber"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                If new_value <> "" Then
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", new_value))
                Else
                    .Add(New System.Data.SqlClient.SqlParameter("@DataColumn", DBNull.Value))
                End If
                .Add(New System.Data.SqlClient.SqlParameter("@RecordNumber", RecordNumber))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

            MyRecord.is_error = False
            MyRecord.error_text = "None"
            MyRecord.html = new_value

        Catch ex As Exception
            MyRecord.is_error = True
            MyRecord.error_text = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        'Action = "Adjusted asset information for record: [" & RecordNumber & "] on column: [" & DataColumn & "] in table: [assets]"
        Action = "{'table':'assets', 'column':'" & DataColumn & "', 'record':" & RecordNumber & "}"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetUniqueValue As New ReturnObject
        GetUniqueValue = CustomFunctions.RecordIDFromTable(RecordNumber, "", "assets", "serialNum", RecordNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("assets", DataColumn, RecordNumber, GetUniqueValue.str, "Asset Update", Action, Reason, orig_value, new_value)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function EIPAssetUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String, ByVal new_value As String, ByVal data As String) As jeipRecord2

        orig_value = HtmlEncode(orig_value)
        new_value = HtmlEncode(new_value)
        data = HtmlEncode(data)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = ""
        Dim Action As String = ""
        Dim Column As String = ""
        Dim RecordID As String = ""
        Dim ActionType As String = ""
        Dim Reason As String = ""
        Dim MyRecord As New jeipRecord2

        If id.StartsWith("UnaccountedFor_Account_") Then
            Column = "account"
            RecordID = id.Replace("UnaccountedFor_Account_", "")
            Reason = "Updated the account code on record number " & id & " from " & orig_value & " to " & new_value & "."
        End If

        strSQLQuery = "UPDATE dbo.assets SET " & Column & " = @NewValue, modified_by = @modified_by, modified_date = @modified_date WHERE ID = @RecordID"
        Action = "{'url':'" & url & "', 'id':'" & id & "', 'form_type':'" & form_type & "', 'orig_value':'" & orig_value & "', 'new_value':'" & new_value & "', 'data':'" & data & "'}"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@NewValue", new_value))
                .Add(New System.Data.SqlClient.SqlParameter("@RecordID", RecordID))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then

                MyRecord.is_error = False
                MyRecord.error_text = "Account has been successfully updated!"
                MyRecord.html = new_value
                ActionType = "Asset Update"

            Else

                MyRecord.is_error = True
                MyRecord.error_text = "A fatal error has occured! Please refresh the page and try again..."
                MyRecord.html = orig_value
                ActionType = "Asset Update Failed"
                Reason = "The account code for record number " & id & " failed to update correctly!"

            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.is_error = True
            MyRecord.error_text = "Error: " & ex.Message
            MyRecord.html = orig_value
            ActionType = "Exception in Dashboard.asmx/EIPAssetUpdate"
            Reason = MyRecord.error_text
        Finally
            objConnection.Close()
        End Try

        '*****Log Events

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetUniqueValue As New ReturnObject
        GetUniqueValue = CustomFunctions.RecordIDFromTable(RecordID, "", "assets", "ID", RecordID)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("assets", Column, RecordID, GetUniqueValue.str, ActionType, Action, Reason, orig_value, new_value)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function ArchiveAsset(ByVal SerialNumber As String, ByVal ArchiveReason As String, ByVal AIMTransactionNumber As String, ByVal AIMCageCode As String, ByVal AIMModelNumber As String, ByVal AIMModelDescription As String) As ReturnObject

        ArchiveReason = HtmlEncode(ArchiveReason)
        AIMTransactionNumber = HtmlEncode(AIMTransactionNumber)
        AIMCageCode = HtmlEncode(AIMCageCode)
        AIMModelNumber = HtmlEncode(AIMModelNumber)
        AIMModelDescription = HtmlEncode(AIMModelDescription)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim SubFunction As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim Action As String = ""
        Dim ActionType As String = ""
        Dim Reason As String = ""

        Dim CheckSerialNumberAvailable As New JSONObject
        CheckSerialNumberAvailable = WebServiceFunctions.CheckSerialNumberAvailable(SerialNumber, "Good")

        If CheckSerialNumberAvailable.IsError = False Then
            strSQLQuery = "INSERT INTO dbo.[archive-assets] (trunkID, trunkIDRange, serialNum, account, partNum, modelNum, modelDesc, cost, [9600B], " & _
                "AES, OTAR, OTAP, assetComments, assetDisabled, assetDisabledDate, assetDisabledComments, archiveReason, modified_by, modified_date, " & _
                "AIMTransactionNumber, AIMCageCode, AIMModelNumber, AIMModelDescription, AIMStatusCode) " & _
                "SELECT trunkID, trunkIDRange, serialNum, account, partNum, modelNum, modelDesc, cost, [9600B], AES, OTAR, OTAP, " & _
                "assetComments, assetDisabled, assetDisabledDate, assetDisabledComments, @ArchiveReason, @modified_by, @modified_date, " & _
                "@AIMTransactionNumber, @AIMCageCode, @AIMModelNumber, @AIMModelDescription, @AIMStatusCode " & _
                "FROM dbo.assets WHERE (serialNum = @SerialNumber)"
        Else
            strSQLQuery = "INSERT INTO dbo.[archive-assets] (trunkID, trunkIDRange, serialNum, account, partNum, modelNum, modelDesc, cost, [9600B], " & _
                "AES, OTAR, OTAP, assetComments, assetDisabled, assetDisabledDate, assetDisabledComments, archiveReason, modified_by, modified_date, " & _
                "AIMTransactionNumber, AIMCageCode, AIMModelNumber, AIMModelDescription, AIMStatusCode) " & _
                "VALUES ('0', '', '" & SerialNumber & "', '', '" & AIMModelNumber & "', '" & AIMModelNumber & "', '" & AIMModelDescription & "', '', 'U', 'U', 'U', 'U', " & _
                "'', 'No', NULL, '', @ArchiveReason, @modified_by, @modified_date, " & _
                "@AIMTransactionNumber, @AIMCageCode, @AIMModelNumber, @AIMModelDescription, @AIMStatusCode)"
        End If



        Action = "{'SerialNumber':'" & SerialNumber & "', 'ArchiveReason':'" & ArchiveReason & "', 'AIMTransactionNumber':'" & AIMTransactionNumber & "', 'AIMCageCode':'" & AIMCageCode & "', 'AIMModelNumber':'" & AIMModelNumber & "', 'AIMModelDescription':'" & AIMModelDescription & "', 'AIMStatusCode':'11'}"
        Reason = "Archived Data For Serial Number " & SerialNumber

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@ArchiveReason", ArchiveReason.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@AIMTransactionNumber", AIMTransactionNumber.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@AIMCageCode", AIMCageCode.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@AIMModelNumber", AIMModelNumber.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@AIMModelDescription", AIMModelDescription.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@AIMStatusCode", "11"))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader

            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then 'record was successfully inserted

                '*****Free Trunk ID Record
                SubFunction = CustomFunctions.FreeTrunkID(SerialNumber.Trim())
                '*****Free Trunk ID Record

                MyRecord.IsError = False
                MyRecord.ErrorMessage = SubFunction.ErrorMessage

                If SubFunction.IsError = False Then
                    MyRecord.str = "Serial number " & SerialNumber & " was successfully archived. " & SubFunction.str
                Else
                    MyRecord.str = "Serial number " & SerialNumber & " was successfully archived with an exception: " & SubFunction.ErrorMessage
                End If

                ActionType = "Archived Asset"
                Reason = "Serial number " & SerialNumber & " was successfully archived"

            Else 'no record was inserted because the serial number was not found in the dbo.assets table

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Failed to archive data for serial number " & SerialNumber & " because no matching record was found!"
                MyRecord.str = ""
                ActionType = "Archived Asset Failed"
                Reason = MyRecord.ErrorMessage

            End If

            objDataReader.Close()

        Catch ex As Exception
            'str = "Error: " & ex.Message
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            ActionType = "Exception in WebServiceFuntions.vb/ArchiveAsset()"
            Reason = MyRecord.ErrorMessage
        Finally
            objConnection.Close()
        End Try

        '*****Log Events

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        Dim ForceInt As Integer
        GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "assets", "serialNum", SerialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value
        If GetRecordID.IsError = False Then
            ForceInt = GetRecordID.str
        Else
            ForceInt = 0
        End If
        CustomFunctions.AddToLog("archive-assets", "serialNum", ForceInt, SerialNumber, ActionType, Action, Reason, "NA", "NA")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function addMaintenanceAction(ByVal invoiceNumber As String, ByVal serialNumber As String, ByVal description As String, ByVal dateOfAction As String, ByVal cost As String) As String

        invoiceNumber = HtmlEncode(invoiceNumber)
        serialNumber = HtmlEncode(serialNumber)
        description = HtmlEncode(description)
        dateOfAction = HtmlEncode(dateOfAction)
        cost = HtmlEncode(cost)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""

        strSQLQuery = "INSERT INTO [PWCS].[dbo].[maintenance] (sn,invoice,date,description,cost,modified_by) VALUES (@serialNumber,@invoiceNumber,@dateOfAction,@description,@cost,@modified_by)"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@serialNumber", serialNumber.ToUpper()))
                .Add(New System.Data.SqlClient.SqlParameter("@invoiceNumber", invoiceNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@dateOfAction", CDate(dateOfAction)))
                .Add(New System.Data.SqlClient.SqlParameter("@description", description))
                .Add(New System.Data.SqlClient.SqlParameter("@cost", cost))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                ' do not need to insert modified_date, current date is set by default in the database
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

            str = str & "The following maintenance action was successfully added: <br /><br />" & _
                "<table>" & _
                "<tr><td> Serial Number: </td><td><b>" & serialNumber.ToUpper() & "</b></td></tr>" & _
                "<tr><td> Invoice Number: </td><td><b>" & invoiceNumber & "</b></td></tr>" & _
                "<tr><td> Description: </td><td><b>" & description & "</b></td></tr>" & _
                "<tr><td> Date: </td><td><b>" & dateOfAction & "</b></td></tr>" & _
                "<tr><td> Cost: </td><td><b>$ " & cost & "</b></td></tr>" & _
                "</table>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        'Action = "Adjusted asset information for record: [" & RecordNumber & "] on column: [" & DataColumn & "] in table: [assets]"
        Action = "{'SerialNumber':'" & serialNumber.ToUpper() & "', 'InvoiceNumber':'" & invoiceNumber & "', 'DateOfAction':'" & dateOfAction & "', 'Description':'" & description & "', 'Cost':'" & cost & "'}"
        Reason = "Added Maintenace Action to Database"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", serialNumber, "maintenance", "sn", serialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("maintenance", "sn", GetRecordID.str, serialNumber.ToUpper(), "Maintenance", Action, Reason, "NA", "NA")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return str

    End Function

    Public Shared Function MaintenanceBadActors() As String
        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = ""
        Dim str As String = ""

        strSQLQuery = "SELECT TOP(20) sn, COUNT(sn) AS 'count', SUM(cost) AS 'total' FROM dbo.maintenance GROUP BY sn ORDER BY 'count' DESC, sn"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            'With objCommand.Parameters
            '.Add(New System.Data.SqlClient.SqlParameter("@Account", Account))
            'End With

            str = str & "<div style='text-align:center; margin-bottom:10px;'><h3>Top 20 Maintenance Actions</h3><hr /></div>"

            str = str & "<table id='' class='MainStyle' style=''><tr>" & _
            "<th><b>Serial Number</b></th>" & _
            "<th><b>Maintenance Actions</b></th>" & _
            "<th><b>Total Cost</b></th>" & _
            "</tr>"

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            While objDataReader.Read()

                str = str & "<tr>" & _
                    "<td class=''>" & objDataReader("sn") & "</td>" & _
                    "<td class=''>" & objDataReader("count") & "</td>" & _
                    "<td class=''>" & objDataReader("total") & "</td>" & _
                    "</tr>"

            End While

            objDataReader.Close()

            str = str & "</table>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function SerialNumberSearchForTransfer(ByVal Account As String) As String

        Account = HtmlEncode(Account)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = "SELECT serialNum FROM dbo.assets WHERE account = @Account ORDER BY serialNum "
        Dim str As String = ""

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account))
            End With

            str = str & "<br /><hr /><br />"

            str = str & "<table id='SerialNumberList' class='TransferList'><tr>" & _
            "<th><b>Serial Number</b></th>" & _
            "<th><b>Add to Transfer</b></th>" & _
            "</tr>"

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            While objDataReader.Read()

                str = str & "<tr>" & _
                    "<td class='serialNumber'>" & objDataReader("serialNum") & "</td>" & _
                    "<td class='checkboxes'><input type='checkbox' id='" & objDataReader("serialNum") & "' /></td>" & _
                    "</tr>"

            End While

            objDataReader.Close()

            str = str & "</table>" & _
                "<div class='Seperator'></div><br />" & _
                "<div id='ButtonPlaceHolder' runat='server'></div>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function AddTransfer(ByVal TransferNumber As String, ByVal FromAccount As String, ByVal ToAccount As String, ByVal TransferDate As String) As String

        TransferNumber = HtmlEncode(TransferNumber)
        FromAccount = HtmlEncode(FromAccount)
        ToAccount = HtmlEncode(ToAccount)
        TransferDate = HtmlEncode(TransferDate)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String

        strSQLQuery = "INSERT INTO transfers (transfer_number, from_account, to_account, date, modified_by) " & _
            "VALUES (@TransferNumber, @FromAccount, @ToAccount, @TransferDate, @modified_by)"

        Dim str As String = ""

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@FromAccount", FromAccount))
                .Add(New System.Data.SqlClient.SqlParameter("@ToAccount", ToAccount))

                If TransferDate = "" Then
                    .Add(New System.Data.SqlClient.SqlParameter("@TransferDate", Now.ToShortDateString))
                Else
                    .Add(New System.Data.SqlClient.SqlParameter("@TransferDate", CDate(TransferDate)))
                End If

                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                'date of insert is entered automatically by default in the database
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        str = str & "Transfer has been added!"

        '*****Log Events
        Dim Action As String = "{'table':'transfers', 'TransferNumber':'" & TransferNumber & "', 'FromAccount':'" & FromAccount & "', 'ToAccount':'" & ToAccount & "', 'TransferDate':'" & TransferDate & "'}"
        Dim Reason As String = "Added Transfer Number " & TransferNumber & " from Account " & FromAccount & " to Account " & ToAccount & " on " & TransferDate

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", TransferNumber, "transfers", "transfer_number", TransferNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("transfers", "transfer_number", GetRecordID.str, TransferNumber, "Transfer", Action, Reason, "NA", "NA")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return str

    End Function

    Public Shared Function AddTransfersList(ByVal TransferNumber As String, ByVal SerialNumber As String) As String

        TransferNumber = HtmlEncode(TransferNumber)
        SerialNumber = HtmlEncode(SerialNumber)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String

        strSQLQuery = "INSERT INTO dbo.TransfersList (TransferNumber, SerialNumber, modified_by) " & _
            "VALUES (@TransferNumber, @SerialNumber, @modified_by)"

        Dim str As String = ""

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        str = str & "Transfer List has been added!"

        '*****Log Events
        Dim Action As String = "{'TransferNumber':'" & TransferNumber & "', 'SerialNumber':'" & SerialNumber & "'}"
        Dim Reason As String = "Added " & SerialNumber & " To Transfer " & TransferNumber & " In The TransferList Table"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "TransferList", "SerialNumber", SerialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("TransferList", "SerialNumber", GetRecordID.str, SerialNumber, "Transfer", Action, Reason, "NA", "NA")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return str

    End Function

    Public Shared Function ConfirmTransfer(ByVal TransferNumber As String, ByVal Cancelled As String) As ReturnObject

        TransferNumber = HtmlEncode(TransferNumber)
        Cancelled = HtmlEncode(Cancelled)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim SubFunction As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim ActionType As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OldValue As String = ""
        Dim NewValue As String = ""

        strSQLQuery = "UPDATE transfers SET returned = 'True', cancelled = @Cancelled, modified_by = @modified_by, modified_date = @modified_date WHERE transfer_number = @TransferNumber"
        Action = "{'TransferNumber':'" & TransferNumber & "', 'Returned':'True', 'Cancelled':'" & Cancelled & "'}"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@Cancelled", Cancelled))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then

                'The following function is used to either confirm or cancel the assets listed for transfer.
                'If the transfer is confirmed, Cancelled will be set to "False" and all the assets listed for that transfer
                'will have their account code updated in the assets table.
                'If Cancelled is set to "True", an update statement is executed that sets the "cancelled" column to "True"
                'in the TransfersList table.
                SubFunction = CustomFunctions.PrepAssetOnConfirmTransfer(TransferNumber, Cancelled)

                If Cancelled = "False" Then
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = SubFunction.ErrorMessage
                    MyRecord.str = SubFunction.str
                    ActionType = "Confirmed Transfer"
                    Reason = "Transfer " & TransferNumber & " has been completed, all assets have been transferred to the new account."
                    OldValue = "Returned: False, Cancelled: False"
                    NewValue = "Returned: True, Cancelled: False"
                Else 'Cancelled = True
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = SubFunction.ErrorMessage
                    MyRecord.str = SubFunction.str
                    ActionType = "Cancelled Transfer"
                    Reason = "Transfer " & TransferNumber & " has been cancelled."
                    OldValue = "Returned: False, Cancelled: False"
                    NewValue = "Returned: True, Cancelled: True"
                End If

            Else 'RecordsAffected did not equal 1 and something went terribly wrong!
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "A fatal error has occured!  Please refresh the page and try again."
                MyRecord.str = ""
                Reason = "Transfer number " & TransferNumber & " failed to update!"
                OldValue = "Returned: False, Cancelled: False"
                NewValue = "Returned: False, Cancelled: False"

                If Cancelled = "False" Then
                    ActionType = "Confirmed Transfer Failed"
                Else
                    ActionType = "Cancelled Transfer Failed"
                End If

            End If 'end RecordsAffected

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            ActionType = "Exception in CustomQueries.asmx/ConfirmTransfer()"
            Reason = MyRecord.ErrorMessage
            OldValue = ""
            NewValue = ""
        Finally
            objConnection.Close()
        End Try

        '*****Log Events

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", TransferNumber, "transfers", "transfer_number", TransferNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("transfers", "transfer_number", GetRecordID.str, TransferNumber, ActionType, Action, Reason, OldValue, NewValue)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function TransferList(ByVal TransferNumber As String) As String

        TransferNumber = HtmlEncode(TransferNumber)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        'need to consider doing a JOIN here to pull additional data from the assets table
        Dim strSQLQuery As String = "SELECT SerialNumber FROM dbo.TransfersList WHERE TransferNumber = @TransferNumber ORDER BY SerialNumber"
        Dim str As String = ""

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
            End With

            str = str & "<h3>Transfer Number " & TransferNumber & "</h3><ul>"

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            While objDataReader.Read()

                str = str & "<li>" & objDataReader("SerialNumber") & "</li>"

            End While

            objDataReader.Close()

            str = str & "</ul>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function TransferedInAim(ByVal RecordNumber As String, ByVal TransferNumber As String, ByVal TransferValue As String) As ReturnObject

        RecordNumber = HtmlEncode(RecordNumber)
        TransferNumber = HtmlEncode(TransferNumber)
        TransferValue = HtmlEncode(TransferValue)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim ActionType As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OldValue As String = ""

        strSQLQuery = "UPDATE transfers SET transfered_in_aim = @TransferValue, modified_by = @modified_by, modified_date = @modified_date WHERE transfer_number = @TransferNumber"
        Action = "{'transfer_number':'" & TransferNumber & "', 'transfered_in_aim':'" & TransferValue & "'}"

        If TransferValue = "True" Then OldValue = "False" Else OldValue = "True"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TransferValue", TransferValue))
                .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then

                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "The transfered in Aim value for transfer number " & TransferNumber & " was successfully updated"

                If TransferValue = "True" Then
                    ActionType = "Confirmed Transfer In Aim"
                    Reason = "Confirmed transfer of assets in Aim to allow transfer number " & TransferNumber & " to be officially completed."
                Else
                    ActionType = "Cancelled Transfer In Aim"
                    Reason = "Cancelled transfer of assets in Aim for transfer number " & TransferNumber & "."
                End If

            Else 'RecordsAffected did not equal 1 and something went terribly wrong!

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "A fatal error has occured!  Please refresh the page and try again."
                MyRecord.str = ""
                ActionType = "Confirmed Transfer In Aim Failed"
                Reason = "Transfer number " & TransferNumber & " failed to update!"

            End If 'end RecordsAffected

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            ActionType = "Exception in CustomQueries.asmx/ConfirmTransfer()"
            Reason = MyRecord.ErrorMessage
        Finally
            objConnection.Close()
        End Try

        '*****Log Events

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        'Dim GetRecordID As New ReturnObject
        'GetRecordID = CustomFunctions.RecordIDFromTable("", TransferNumber, "transfers", "transfer_number", TransferNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        CustomFunctions.AddToLog("transfers", "transfered_in_aim", RecordNumber, TransferNumber, ActionType, Action, Reason, OldValue, TransferValue)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function AccountAddition(ByVal AccountCode As String, ByVal Unit As String, ByVal AccountComments As String) As JSONObject

        AccountCode = HtmlEncode(AccountCode)
        Unit = HtmlEncode(Unit)
        AccountComments = HtmlEncode(AccountComments)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject

        Dim strSQLQuery As String = "INSERT INTO dbo.accounts (account_code, organization, account_comments, modified_by, modified_date) " & _
            "SELECT @AccountCode, @Unit, @AccountComments, @modified_by, @modified_date " & _
            "WHERE NOT EXISTS(SELECT 1 FROM dbo.accounts WHERE account_code = @AccountCode)"

        Dim Action As String = "{'AccountCode':'" & AccountCode & "', 'Unit':'" & Unit & "', 'AccountComments':'" & AccountComments & "'}"
        Dim Reason As String = ""

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@AccountCode", AccountCode))
                .Add(New System.Data.SqlClient.SqlParameter("@Unit", Unit))
                .Add(New System.Data.SqlClient.SqlParameter("@AccountComments", AccountComments))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then 'record was successfully inserted
                MyRecord.ErrorMessage = ""
                MyRecord.IsError = False
                MyRecord.str = "Account " & AccountCode & " has been successfully added!"
                Reason = "Created account " & AccountCode & " for the " & Unit & " organization."

                '*****Log Events

                '*****Get missing value, either record ID or a column value, for the input into the Log table
                'the record ID or column value is returned in the ".str" parameter of this object
                Dim GetRecordID As New ReturnObject
                GetRecordID = CustomFunctions.RecordIDFromTable("", AccountCode, "accounts", "account_code", AccountCode)
                '*****Reference: ID, UniqueValue, Table, Column, Value

                CustomFunctions.AddToLog("accounts", "account_code", GetRecordID.str, AccountCode, "Account Addition", Action, Reason, "NA", "NA")
                If AccountComments <> "" Then
                    CustomFunctions.AddToLog("accounts", "account_comments", GetRecordID.str, AccountCode, "Account Update", Action, Reason, "NA", AccountComments)
                End If
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events

            Else 'no record was inserted because the account code submitted already exists in the dbo.accounts table
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "I'm sorry, account " & AccountCode & " already exists!  Please enter another account code..."
                MyRecord.str = ""
                Reason = HttpContext.Current.Request.ServerVariables("AUTH_USER") & " attempted to add account code " & AccountCode & " but it already exists in the database!"
                '*****Log Events
                CustomFunctions.AddToLog("accounts", "account_code", "", AccountCode, "Account Addition Failed", Action, Reason, "NA", "NA")
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events
            End If

            objDataReader.Close()

        Catch ex As Exception
            'data.str = "Error: " & ex.Message
            'data.str = ex.Message
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    'NOTE: Assets added to the database are automatically added to the PWCS account.  In AIM, an asset can only be added if an account
    'is specified.  We want to mimic this action so that after an asset is added to the database, we want to force the user to have to use
    'a transfer to move the assets to the new account.  This will allow that event to be logged as well as force the Unit PWCS Manager to
    'sign a transfer form in order to accept accountability of the assets.  Again we want to mimic the AIM actions with the PWCS website.
    Public Shared Function AssetAddition(ByVal TrunkID As String, ByVal SerialNumber As String, ByVal ModelNumber As String, ByVal ModelDescription As String,
                                  ByVal AssetComments As String, ByVal Cost As String, ByVal Baud As String, ByVal AES As String, ByVal OTAR As String,
                                  ByVal OTAP As String, ByVal QueryType As String) As JSONObject

        TrunkID = HtmlEncode(TrunkID)
        SerialNumber = HtmlEncode(SerialNumber)
        ModelNumber = HtmlEncode(ModelNumber)
        ModelDescription = HtmlEncode(ModelDescription)
        AssetComments = HtmlEncode(AssetComments)
        Cost = HtmlEncode(Cost)
        Baud = HtmlEncode(Baud)
        AES = HtmlEncode(AES)
        OTAR = HtmlEncode(OTAR)
        OTAP = HtmlEncode(OTAP)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject
        Dim strSQLQuery As String = ""
        'Dim AssetComments As String = "New asset added on " & Now.ToShortDateString & " by " & HttpContext.Current.Request.ServerVariables("AUTH_USER")
        Dim Reason As String = ""
        Dim Action As String = "{'TrunkID':'" & TrunkID & "', 'SerialNumber':'" & SerialNumber & "', 'ModelNumber':'" & ModelNumber & "', " & _
            "'ModelDescription':'" & ModelDescription & "', 'Cost':'" & Cost & "', 'Baud':'" & Baud & "', 'AES':'" & AES & "', " & _
            "'OTAR':'" & OTAR & "', 'OTAP':'" & OTAP & "', 'QueryType':'" & QueryType & "'}"

        'when calling this webservice a specific query type must be selected to either update or insert a record based off if the trunk id of the asset being added
        'is known or not.  If it is known then we want to perform an update statement, if it is not known, then a new insert statement will be used.
        'Current options for QueryType are:  "Insert" or "Update"
        If QueryType = "Insert" Then
            strSQLQuery = "INSERT INTO dbo.assets (trunkID, serialNum, account, partNum, modelNum, modelDesc, cost, [9600B], AES, OTAR, OTAP, assetComments, modified_by, modified_date) " & _
                "VALUES (@TrunkID, @SerialNumber, @Account, @ModelNumber, @ModelNumber, @ModelDescription, @Cost, @Baud, @AES, @OTAR, @OTAP, @AssetComments, @modified_by, @modified_date)"
        ElseIf QueryType = "Update" Then
            strSQLQuery = "UPDATE dbo.assets SET serialNum = @SerialNumber, account = @Account, partNum = @ModelNumber, modelNum = @ModelNumber, modelDesc = @ModelDescription, " & _
                "cost = @Cost, [9600B] = @Baud, AES = @AES, OTAR = @OTAR, OTAP = @OTAP, assetComments = @AssetComments, assetDisabled = 'No', modified_by = @modified_by, modified_date = @modified_date WHERE trunkID = @TrunkID"
        End If

        If AssetComments = "" Then
            AssetComments = "New asset added on " & Now.ToShortDateString & " by " & CustomFunctions.ReplaceDomain(HttpContext.Current.Request.ServerVariables("AUTH_USER"))
        End If

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TrunkID", TrunkID))
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@Account", "PWCS"))
                .Add(New System.Data.SqlClient.SqlParameter("@ModelNumber", ModelNumber))
                .Add(New System.Data.SqlClient.SqlParameter("@ModelDescription", ModelDescription))
                .Add(New System.Data.SqlClient.SqlParameter("@Cost", Cost))
                .Add(New System.Data.SqlClient.SqlParameter("@Baud", Baud))
                .Add(New System.Data.SqlClient.SqlParameter("@AES", AES))
                .Add(New System.Data.SqlClient.SqlParameter("@OTAR", OTAR))
                .Add(New System.Data.SqlClient.SqlParameter("@OTAP", OTAP))
                .Add(New System.Data.SqlClient.SqlParameter("@AssetComments", AssetComments))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then 'record was successfully inserted
                MyRecord.ErrorMessage = ""
                MyRecord.IsError = False
                MyRecord.str = "Serial number " & SerialNumber & " has been successfully added!"
                Reason = "Added serial number " & SerialNumber & " to trunk ID " & TrunkID & " with an " & QueryType & " statement."
                '*****Log Events

                '*****Get missing value, either record ID or a column value, for the input into the Log table
                'the record ID or column value is returned in the ".str" parameter of this object
                Dim GetRecordID As New ReturnObject
                GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "assets", "serialNum", SerialNumber)
                '*****Reference: ID, UniqueValue, Table, Column, Value

                CustomFunctions.AddToLog("assets", "serialNum", GetRecordID.str, SerialNumber, "Asset Addition", Action, Reason, "No Trunk ID", TrunkID)
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events

                'this log addition is needed to add an initial comment when a new asset is added.
                CustomFunctions.AddToLog("assets", "assetComments", GetRecordID.str, SerialNumber, "Asset Update", Action, "Initial Comment", "", AssetComments)
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events

            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = QueryType & " failed to execute! Serial number " & SerialNumber & " was not added!"
                MyRecord.str = ""
                Reason = HttpContext.Current.Request.ServerVariables("AUTH_USER") & " attempted to add asset " & SerialNumber & " but the query failed!"
                '*****Log Events
                CustomFunctions.AddToLog("assets", "serialNum", "", SerialNumber, "Asset Addition Failed", Action, Reason, "No Trunk ID", TrunkID)
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events
            End If

            objDataReader.Close()

        Catch ex As Exception
            'data.str = "Error: " & ex.Message
            'data.str = ex.Message
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function SwapTrunkID(ByVal SerialNumber As String, ByVal OldTrunkID As String, ByVal OldRecordID As String, ByVal NewTrunkID As String, ByVal NewRecordID As String) As JSONObject

        SerialNumber = HtmlEncode(SerialNumber)
        OldTrunkID = HtmlEncode(OldTrunkID)
        OldRecordID = HtmlEncode(OldRecordID)
        NewTrunkID = HtmlEncode(NewTrunkID)
        NewRecordID = HtmlEncode(NewRecordID)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject

        Dim strSQLQuery As String = "UPDATE dbo.assets SET trunkID = CASE ID " & _
            "WHEN @OldRecordID THEN @NewTrunkID " & _
            "WHEN @NewRecordID THEN @OldTrunkID " & _
            "END, " & _
            "trunkIDRange = CASE ID " & _
            "WHEN @OldRecordID THEN (SELECT trunkIDRange FROM dbo.assets WHERE ID = @NewRecordID)" & _
            "WHEN @NewRecordID THEN (SELECT trunkIDRange FROM dbo.assets WHERE ID = @OldRecordID)" & _
            "END, " & _
            "modified_by = @modified_by, modified_date = @modified_date " & _
            "WHERE (ID = @OldRecordID) OR (ID = @NewRecordID)"

        Dim Action As String = "{'SerialNumber':'" & SerialNumber & "', 'OldTrunkID':'" & OldTrunkID & "', 'OldRecordID':'" & OldRecordID & "', 'NewTrunkID':'" & NewTrunkID & "', 'NewRecordID':'" & NewRecordID & "'}"
        Dim Reason As String = ""

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "assets", "serialNum", SerialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@OldTrunkID", OldTrunkID))
                .Add(New System.Data.SqlClient.SqlParameter("@OldRecordID", OldRecordID))
                .Add(New System.Data.SqlClient.SqlParameter("@NewTrunkID", NewTrunkID))
                .Add(New System.Data.SqlClient.SqlParameter("@NewRecordID", NewRecordID))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New SqlParameter("@modified_by", UserValidation.AccessedByUser()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 2 Then 'update on two records was successful
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Updated the trunk id for serial number " & SerialNumber & " from " & OldTrunkID & " to " & NewTrunkID

                '*****Log Events
                Reason = MyRecord.str

                CustomFunctions.AddToLog("assets", "serialNum", GetRecordID.str, SerialNumber, "Trunk ID Update", Action, Reason, OldTrunkID, NewTrunkID)
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events

            Else 'the update did not run or more than two rows were updated which would be a big mistake!
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "An error has occured when processing your request!  Please verify the data for the attempted trunk ID swap."
                MyRecord.str = ""

                '*****Log Events
                Reason = HttpContext.Current.Request.ServerVariables("AUTH_USER") & " attempted to swap serial number " & SerialNumber & " from trunk id " & OldTrunkID & " to trunk id " & NewTrunkID & " but an error has occured!"
                CustomFunctions.AddToLog("assets", "account_code", GetRecordID.str, SerialNumber, "Trunk ID Update Failed", Action, Reason, OldTrunkID, NewTrunkID)
                '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '*****Log Events
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function CheckTrunkIDAvailable(ByVal TrunkID As String) As JSONObject

        TrunkID = HtmlEncode(TrunkID)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject
        Dim Action As String = ""
        Dim Reason As String = ""

        'This query is going to check if the specific trunk id has a valid serial number associated with it
        'Dim strSQLQuery As String = "SELECT * FROM dbo.assets WHERE (trunkID = @TrunkID) AND (serialNum IS NOT NULL) AND (serialNum <> '0')"
        Dim strSQLQuery As String = "SELECT * FROM dbo.assets WHERE (trunkID = @TrunkID)"

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@TrunkID", TrunkID))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows() Then
                While objDataReader.Read()
                    If ((objDataReader("serialNum") = "0") Or (objDataReader("serialNum") Is DBNull.Value)) Then
                        MyRecord.IsError = False
                        MyRecord.ErrorMessage = ""
                        MyRecord.str = "Valid ID"
                        MyRecord.NewValue = objDataReader("ID")
                    Else
                        MyRecord.IsError = True
                        MyRecord.ErrorMessage = "Trunk ID " & TrunkID & " is already associated with serial number " & objDataReader("serialNum") & "!"
                        MyRecord.str = ""
                        MyRecord.NewValue = ""
                    End If
                End While

            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Trunk ID " & TrunkID & " does not exist!"
                MyRecord.str = ""
                MyRecord.NewValue = ""
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            MyRecord.NewValue = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function CheckSerialNumberAvailable(ByVal SerialNumber As String, ByVal ActionIfFound As String) As JSONObject

        SerialNumber = HtmlEncode(SerialNumber)
        ActionIfFound = HtmlEncode(ActionIfFound)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject
        Dim RecordID As String = ""
        Dim TrunkID As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""

        Dim strSQLQuery As String = "SELECT * FROM dbo.assets WHERE (serialNum = @SerialNumber)"

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows() Then

                While objDataReader.Read()
                    TrunkID = objDataReader("trunkID")
                    RecordID = objDataReader("ID")
                End While

                If ActionIfFound = "Good" Then
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""
                    MyRecord.str = "Serial Number exists - current Trunk ID is: " & TrunkID
                    MyRecord.NewValue = RecordID
                    MyRecord.OldTrunkID = TrunkID
                ElseIf ActionIfFound = "Bad" Then
                    MyRecord.IsError = True
                    MyRecord.ErrorMessage = "Serial Number " & SerialNumber & " already exists!"
                    MyRecord.str = TrunkID
                    MyRecord.NewValue = ""
                    MyRecord.OldTrunkID = ""
                End If

            Else
                If ActionIfFound = "Good" Then
                    MyRecord.IsError = True
                    MyRecord.ErrorMessage = "Serial Number does not exist!"
                    MyRecord.str = ""
                    MyRecord.NewValue = ""
                ElseIf ActionIfFound = "Bad" Then
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""
                    MyRecord.str = "Serial Number Available"
                    MyRecord.NewValue = ""
                End If
                '    'Reason = HttpContext.Current.Request.ServerVariables("AUTH_USER") & " attempted to add account code " & AccountCode & " but it already exists in the database!"
                '    '*****Log Events
                '    'CustomFunctions.AddToLog("accounts", "account_code", "", AccountCode, "Account Addition Failed", Action, Reason, "NA", "NA")
                '    '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
                '    '*****Log Events
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = strSQLQuery
            MyRecord.NewValue = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function CopyAsset(ByVal SerialNumber As String) As JSONObject

        SerialNumber = HtmlEncode(SerialNumber)

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New JSONObject
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim DataArrayList As New List(Of String)

        'This query is going to check if the specific trunk id has a valid serial number associated with it
        'Dim strSQLQuery As String = "SELECT * FROM dbo.assets WHERE (trunkID = @TrunkID) AND (serialNum IS NOT NULL) AND (serialNum <> '0')"
        Dim strSQLQuery As String = "SELECT TOP(1) * FROM dbo.assets WHERE (serialNum = @SerialNumber)"

        Try

            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows() Then
                While objDataReader.Read()
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""
                    MyRecord.str = "Data from serial number " & SerialNumber & " has been successfully copied"

                    DataArrayList.Add(objDataReader("serialNum"))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("modelNum")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("modelDesc")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("cost")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("9600B")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("AES")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("OTAR")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("OTAP")))
                    DataArrayList.Add(CustomFunctions.NullCheck(objDataReader("assetComments")))
                    MyRecord.DataArrayList = DataArrayList
                End While
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Serial number " & SerialNumber & " does not exist!"
                MyRecord.str = ""
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function JsonManagersList(ByVal page As Integer, ByVal take As Integer) As KendoObject

        page = HtmlEncode(page)
        take = HtmlEncode(take)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spActiveManagersList")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim KendoObject As New KendoObject
        Dim RecordList As New List(Of Dictionary(Of String, String))

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@PageNumber", page))
                .Add(New SqlParameter("@PageSize", take))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.HasRows Then

                KendoObject.IsError = False
                KendoObject.ErrorMessage = ""

                While objDataReader.Read()
                    Dim Record As New Dictionary(Of String, String)
                    Record.Add("AccountCode", objDataReader("account_code").ToString())
                    Record.Add("Position", objDataReader("position").ToString())
                    Record.Add("Rank", objDataReader("rank").ToString())
                    Record.Add("FirstName", objDataReader("fname").ToString())
                    Record.Add("LastName", objDataReader("lname").ToString())
                    Record.Add("Organization", objDataReader("org").ToString())
                    Record.Add("Phone", objDataReader("phone").ToString())
                    Record.Add("Email", objDataReader("email").ToString())
                    Record.Add("TrainingDate", objDataReader("trained").ToString())
                    RecordList.Add(Record)

                    KendoObject.TotalRows = objDataReader("TotalRows")
                End While

            Else
                KendoObject.IsError = True
                KendoObject.ErrorMessage = "Error: The selected query returned 0 results!"
            End If

        Catch ex As Exception
            KendoObject.IsError = True
            KendoObject.ErrorMessage = "Error: " & ex.Message & "!"
        Finally
            objConnection.Close()
        End Try

        KendoObject.Data = RecordList

        'Dim Serializer As New JavaScriptSerializer()
        'Dim MyRecord As String = Serializer.Serialize(KendoObject)
        'Return MyRecord

        Return KendoObject

    End Function

    Public Shared Function ArchivedAssets(ByVal page As Integer, ByVal take As Integer) As KendoObject

        page = HtmlEncode(page)
        take = HtmlEncode(take)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spArchivedAssets")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim KendoObject As New KendoObject
        Dim RecordList As New List(Of Dictionary(Of String, String))

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@PageNumber", page))
                .Add(New SqlParameter("@PageSize", take))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.HasRows Then

                KendoObject.IsError = False
                KendoObject.ErrorMessage = ""

                While objDataReader.Read()
                    Dim Record As New Dictionary(Of String, String)
                    Record.Add("TrunkID", objDataReader("trunkID").ToString())
                    Record.Add("TrunkIDRange", objDataReader("trunkIDRange").ToString())
                    Record.Add("SerialNumber", objDataReader("serialnum").ToString())
                    Record.Add("AccountCode", objDataReader("account").ToString())
                    Record.Add("ModelNumber", objDataReader("modelNum").ToString())
                    Record.Add("ModelDescription", objDataReader("modelDesc").ToString())
                    Record.Add("AimTransactionNumber", objDataReader("AIMTransactionNumber").ToString())
                    Record.Add("ArchiveReason", objDataReader("archiveReason").ToString())
                    Record.Add("TotalRows", objDataReader("TotalRows").ToString())
                    RecordList.Add(Record)

                    KendoObject.TotalRows = objDataReader("TotalRows")
                End While

            Else
                KendoObject.IsError = True
                KendoObject.ErrorMessage = "Error: The selected query returned 0 results!"
            End If

        Catch ex As Exception
            KendoObject.IsError = True
            KendoObject.ErrorMessage = "Error: " & ex.Message & "!"
        Finally
            objConnection.Close()
        End Try

        KendoObject.Data = RecordList

        'Dim Serializer As New JavaScriptSerializer()
        'Dim MyRecord As String = Serializer.Serialize(KendoObject)
        'Return MyRecord

        Return KendoObject

    End Function

    Public Shared Function UnaccountedFor(ByVal page As Integer, ByVal take As Integer) As KendoObject

        page = HtmlEncode(page)
        take = HtmlEncode(take)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spUnaccountedFor")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim KendoObject As New KendoObject
        Dim RecordList As New List(Of Dictionary(Of String, String))

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@PageNumber", page))
                .Add(New SqlParameter("@PageSize", take))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.HasRows Then

                KendoObject.IsError = False
                KendoObject.ErrorMessage = ""

                While objDataReader.Read()

                    Dim Record As New Dictionary(Of String, String)

                    Record.Add("TrunkID", objDataReader("trunkID").ToString())
                    Record.Add("TrunkIDRange", objDataReader("trunkIDRange").ToString())
                    Record.Add("SerialNumber", objDataReader("serialnum").ToString())
                    Record.Add("AccountCode", objDataReader("account").ToString())
                    Record.Add("ModelNumber", objDataReader("modelNum").ToString())
                    Record.Add("ModelDescription", objDataReader("modelDesc").ToString())
                    Record.Add("AssetComments", objDataReader("assetComments").ToString())
                    Record.Add("AssetDisabled", objDataReader("assetDisabled").ToString())
                    RecordList.Add(Record)

                    KendoObject.TotalRows = objDataReader("TotalRows")

                End While

            Else

                KendoObject.IsError = True
                KendoObject.ErrorMessage = "Error: The selected query returned 0 results!"

            End If

        Catch ex As Exception
            KendoObject.IsError = True
            KendoObject.ErrorMessage = "Error: " & ex.Message & "!"
        Finally
            objConnection.Close()
        End Try

        KendoObject.Data = RecordList

        'Dim Serializer As New JavaScriptSerializer()
        'Dim MyRecord As String = Serializer.Serialize(KendoObject)
        'Return MyRecord

        Return KendoObject

    End Function

    'Public Shared Function AllOpenTrunkID(ByVal page As Integer, ByVal take As Integer) As Array

    '    Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
    '    'This SQLCommand needs to be the name of the stored procedure
    '    Dim objCommand As New SqlCommand("spAllOpenTrunkID")

    '    objCommand.CommandType = Data.CommandType.StoredProcedure
    '    objCommand.Connection = objConnection

    '    Dim List As New List(Of Dictionary(Of String, String))()

    '    Try
    '        'these parameters must match the paramters set in the stored procedure
    '        With objCommand.Parameters
    '            .Add(New SqlParameter("@PageNumber", page))
    '            .Add(New SqlParameter("@PageSize", take))
    '        End With

    '        objConnection.Open()

    '        Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

    '        If objDataReader.HasRows Then

    '            While objDataReader.Read()
    '                Dim Record As New Dictionary(Of String, String)
    '                Record.Add("TrunkID", objDataReader("trunkID").ToString())
    '                Record.Add("TrunkIDRange", objDataReader("trunkIDRange").ToString())
    '                Record.Add("SerialNumber", objDataReader("serialNum").ToString())
    '                Record.Add("AssetComments", objDataReader("assetComments").ToString())
    '                Record.Add("TotalRows", objDataReader("TotalRows").ToString())
    '                List.Add(Record)
    '            End While

    '        Else

    '        End If

    '    Catch ex As Exception
    '        Dim Record As New Dictionary(Of String, String)
    '        Record.Add("Error", "Error: " & ex.Message)
    '        List.Add(Record)
    '    Finally
    '        objConnection.Close()
    '    End Try

    '    Return List.ToArray

    'End Function

    Public Shared Function AllOpenTrunkID(ByVal page As Integer, ByVal take As Integer) As KendoObject

        page = HtmlEncode(page)
        take = HtmlEncode(take)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spAllOpenTrunkID")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim KendoObject As New KendoObject
        Dim RecordList As New List(Of Dictionary(Of String, String))

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@PageNumber", page))
                .Add(New SqlParameter("@PageSize", take))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.HasRows Then

                KendoObject.IsError = False
                KendoObject.ErrorMessage = ""

                While objDataReader.Read()
                    Dim Record As New Dictionary(Of String, String)

                    Record.Add("TrunkID", objDataReader("trunkID").ToString())
                    Record.Add("TrunkIDRange", objDataReader("trunkIDRange").ToString())
                    Record.Add("SerialNumber", objDataReader("serialNum").ToString())
                    Record.Add("AssetComments", objDataReader("assetComments").ToString())
                    RecordList.Add(Record)

                    KendoObject.TotalRows = objDataReader("TotalRows")
                End While

            Else
                KendoObject.IsError = True
                KendoObject.ErrorMessage = "Error: The selected query returned 0 results!"
            End If

        Catch ex As Exception
            KendoObject.IsError = True
            KendoObject.ErrorMessage = "Error: " & ex.Message & "!"
        Finally
            objConnection.Close()
        End Try

        KendoObject.Data = RecordList

        'Dim Serializer As New JavaScriptSerializer()
        'Dim MyRecord As String = Serializer.Serialize(KendoObject)
        'Return MyRecord

        Return KendoObject

    End Function

End Class

