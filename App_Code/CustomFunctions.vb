Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data.CommandBehavior
Imports System.Web.HttpUtility

Public Class ReturnObject
    Public Property IsError As Boolean
    Public Property ErrorMessage As String
    Public Property str As String
    Public Property Data As New List(Of Dictionary(Of String, String))
End Class

Public Class jeipRecord2
    Public Property is_error As Boolean
    Public Property error_text As String
    Public Property html As String
End Class

Public Class JSONObject
    Public Property str As String
    Public Property IsError As Boolean
    Public Property ErrorMessage As String
    Public Property OldValue As String
    Public Property NewValue As String
    Public Property OldTrunkID As String
    Public Property DataArrayList As List(Of String)
End Class

Public Class AccountInfoObject
    'Information pulled from dbo.accounts
    Public Property ID As String
    Public Property AccountCode As String
    Public Property Organization As String
    Public Property AppointmentLetter As String
    Public Property Inventory As String
    Public Property AccountValidation As String
    Public Property EmailAccountValidation1 As String
    Public Property EmailAccountValidation2 As String
    Public Property EmailAccountValidation3 As String
    Public Property EmailInventory1 As String
    Public Property EmailInventory2 As String
    Public Property EmailInventory3 As String
    Public Property EmailAppointmentLetter1 As String
    Public Property EmailAppointmentLetter2 As String
    Public Property EmailAppointmentLetter3 As String
    Public Property EmailTraining1 As String
    Public Property EmailTraining2 As String
    Public Property EmailTraining3 As String
    Public Property AccountComments As String

    'additional return elements
    Public Property str As String
    Public Property IsError As Boolean
    Public Property ErrorMessage As String
End Class

Public Class AccountManagerObject
    'Information pulled from dbo.managers
    Public Property Position As String
    Public Property Rank As String
    Public Property FirstName As String
    Public Property LastName As String
    Public Property Organization As String
    Public Property Phone As String
    Public Property Email As String
    Public Property Trained As String
End Class

Public Class KendoObject
    Public Property Data As New List(Of Dictionary(Of String, String))
    Public Property IsError As Boolean
    Public Property ErrorMessage As String
    Public Property TotalRows As Integer
End Class

Public Class CustomFunctions

    Public Shared Function AccountList(ByVal Instance As String) As String

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String

        strSQLQuery = "SELECT account_code FROM dbo.accounts WHERE active = 'True' ORDER BY account_code"

        Dim str As String = ""

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            'With objCommand.Parameters
            '.Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", "%" & SerialNumber.ToUpper & "%"))
            'End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<select id='AccountList_" & Instance & "' class=''>" & _
                "<option></option>"

            While objDataReader.Read()
                If objDataReader("account_code").ToString <> "" Or objDataReader("account_code").ToString Is DBNull.Value Then
                    str = str & "<option>" & objDataReader("account_code") & "</option>"
                End If
            End While

            objDataReader.Close()

            str = str & "</select>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function TransferNumber() As String

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim str As String = ""

        strSQLQuery = "SELECT DISTINCT MAX(transfer_number) + 1 AS NewTransferNumber FROM dbo.transfers"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            'With objCommand.Parameters
            '.Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", "%" & SerialNumber.ToUpper & "%"))
            'End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            While objDataReader.Read()
                str = str & objDataReader("NewTransferNumber")
            End While

            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function ActiveTransfers() As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim TransferedInAimYes As String = ""
        Dim TransferedInAimNo As String = ""

        strSQLQuery = "SELECT * FROM transfers WHERE returned = 'False' ORDER BY transfer_number"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            'With objCommand.Parameters
            '.Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", "%" & SerialNumber.ToUpper & "%"))
            'End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows Then
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "<p class='ActiveTransferHeading'>Active Transfers</p><hr /><br />" & _
                    "<table id='ActiveTransfers' class='MainStyle'><tr>" & _
                    "<th>Transfer Number</th>" & _
                    "<th>From</th>" & _
                    "<th>To</th>" & _
                    "<th>Assets</th>" & _
                    "<th>Date Initiated</th>" & _
                    "<th>Assets Transfered In AIM</th>" & _
                    "<th>Action</th>" & _
                    "</tr>"
                While objDataReader.Read()

                    If (objDataReader("transfered_in_aim") = False) Then
                        TransferedInAimYes = ""
                        TransferedInAimNo = "checked='checked'"
                    Else
                        TransferedInAimYes = "checked='checked'"
                        TransferedInAimNo = ""
                    End If

                    MyRecord.str = MyRecord.str & "<tr>" & _
                        "<td style='text-align:center; font-weight:bold;'>" & objDataReader("transfer_number") & "</td>" & _
                        "<td>" & objDataReader("from_account") & "</td>" & _
                        "<td>" & objDataReader("to_account") & "</td>" & _
                        "<td id='" & objDataReader("transfer_number") & "' class='ViewTransferItems'>view</td>" & _
                        "<td>" & objDataReader("date") & "</td>" & _
                        "<td>No <input type='radio' id='TransferedInAimNo_" & objDataReader("ID") & "' name='TransferedInAim_" & objDataReader("transfer_number") & "' value='False' " & TransferedInAimNo & " />" & _
                            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & _
                            "Yes <input type='radio' id='TransferedInAimYes_" & objDataReader("ID") & "' name='TransferedInAim_" & objDataReader("transfer_number") & "' value='True' " & TransferedInAimYes & " />" & _
                        "</td>" & _
                        "<td><input type='button' id='TransferCompleteButton_" & objDataReader("transfer_number") & "' value='Completed' /> or " & _
                            "<input type='button' id='TransferCancelledButton_" & objDataReader("transfer_number") & "' value='Cancelled' />" & _
                        "</td>" & _
                        "</tr>"
                End While
                MyRecord.str = MyRecord.str & "</table>"
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "<div>There is currently no active transfers to display</div>"
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

    Public Shared Function RecordIDFromTable(ByVal ID As String, ByVal UniqueValue As String, ByVal Table As String, ByVal ColumnName As String, ByVal ColumnValue As String) As ReturnObject

        Dim MyRecord As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim IDClean As String = ID.Trim()
        Dim UniqueValueClean As String = UniqueValue.Trim()
        Dim TableClean As String = Table.Trim()
        Dim ColumnNameClean As String = ColumnName.Trim()
        Dim ColumnValueClean As String = ColumnValue.Trim()
        Dim str As String = ""

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        If IDClean = "" And UniqueValueClean <> "" Then
            str = "ID"
            strSQLQuery = "SELECT TOP(1) ID FROM " & TableClean & " WHERE (" & ColumnNameClean & " = '" & ColumnValueClean & "')"
        ElseIf IDClean <> "" And UniqueValueClean = "" Then
            str = "Column"
            strSQLQuery = "SELECT TOP(1) " & ColumnNameClean & " FROM " & TableClean & " WHERE (ID = '" & IDClean & "')"
        End If

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            'With objCommand.Parameters
            '.Add(New System.Data.SqlClient.SqlParameter("@Value", ColumnValueClean))
            '.Add(New System.Data.SqlClient.SqlParameter("@ID", IDClean))
            'End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows Then

                While objDataReader.Read()
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""

                    If str = "ID" Then
                        MyRecord.str = objDataReader("ID")
                    ElseIf str = "Column" Then
                        MyRecord.str = objDataReader("" & ColumnNameClean & "")
                    End If
                End While

            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "No Records Found!"
                MyRecord.str = "0"
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = "Exception in CustomFunctions.RecordIDFromTable()"
        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function PrepAssetOnConfirmTransfer(ByVal TransferNumber As String, ByVal Cancelled As String) As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim SubFunction As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim ActionType As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OldValue As String = ""
        Dim NewValue As String = ""

        If Cancelled = "False" Then

            strSQLQuery = "SELECT transfers.from_account AS 'FromAccount', transfers.to_account AS 'ToAccount', TransfersList.SerialNumber AS 'SerialNumber' " & _
                "FROM transfers INNER JOIN TransfersList ON TransfersList.TransferNumber = transfers.transfer_number " & _
                "WHERE transfers.transfer_number = @TransferNumber"
            Try
                objConnection.Open()
                objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

                With objCommand.Parameters
                    .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                End With

                Dim objDataReader As System.Data.SqlClient.SqlDataReader
                objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

                If objDataReader.HasRows Then

                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""
                    MyRecord.str = "Transfer number " & TransferNumber & " has been processed"

                    While objDataReader.Read()

                        'need a function here to update each asset in the assets table with the to_account number - DONE
                        'added a trim function to each parameter to ensure clean data is written to the database
                        SubFunction = UpdateAssetOnConfirmTransfer(objDataReader("FromAccount").Trim(), objDataReader("ToAccount").Trim(), objDataReader("SerialNumber").Trim(), TransferNumber.Trim())

                        MyRecord.str = MyRecord.str & SubFunction.str
                        MyRecord.ErrorMessage = MyRecord.ErrorMessage & SubFunction.ErrorMessage

                    End While

                    If MyRecord.ErrorMessage <> "" Then
                        MyRecord.ErrorMessage = " with one or more errors: " & MyRecord.ErrorMessage
                    End If

                Else 'Select statement returned 0 rows

                    MyRecord.IsError = True
                    MyRecord.ErrorMessage = "Error: No records were found for the selected transfer!"
                    MyRecord.str = ""

                End If 'end HasRows If statement

                objDataReader.Close()

            Catch ex As Exception
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Error: " & ex.Message
                MyRecord.str = "An Exception has occured in CustomFunctions.vb/PrepAssetOnConfirmTransfer()"
            Finally
                objConnection.Close()
            End Try

            '*****No log events needed in this section, select query only!
            '*****Log events will be tracked in the UpdateAssetOnConfirmTransfer() function located above.

        Else 'Cancelled = True

            Action = "{'TransferNumber':'" & TransferNumber & "', 'Cancelled':'True'}"
            OldValue = "False"
            NewValue = "True"

            strSQLQuery = "UPDATE dbo.TransfersList SET cancelled = 'True', modified_by = @modified_by, modified_date = @modified_date " & _
                "WHERE TransferNumber = @TransferNumber"
            Try
                objConnection.Open()
                objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

                With objCommand.Parameters
                    .Add(New System.Data.SqlClient.SqlParameter("@TransferNumber", TransferNumber))
                    .Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                    .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
                End With

                Dim objDataReader As System.Data.SqlClient.SqlDataReader
                objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

                If objDataReader.RecordsAffected >= 1 Then
                    MyRecord.IsError = False
                    MyRecord.ErrorMessage = ""
                    MyRecord.str = "Transfer " & TransferNumber & " has been successfully cancelled!"
                    ActionType = "Transfer List Update"
                    Reason = MyRecord.str
                Else
                    MyRecord.IsError = True
                    MyRecord.ErrorMessage = "There are no assets associated with transfer " & TransferNumber & "!"
                    MyRecord.str = ""
                    ActionType = "Transfer List Update Failed"
                    Reason = MyRecord.ErrorMessage
                End If

                objDataReader.Close()

            Catch ex As Exception
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Error: " & ex.Message
                MyRecord.str = "An Exception has occured in CustomFunctions.vb/PrepAssetOnConfirmTransfer()"
                ActionType = "Exception in CustomFunctions.vb/PrepAssetOnConfirmTransfer()"
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

            AddToLog("TransfersList", "TransferNumber", "", TransferNumber, ActionType, Action, Reason, OldValue, NewValue)
            '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
            '*****Log Events

        End If 'end Cancelled If statement

        Return MyRecord

    End Function

    Public Shared Function UpdateAssetOnConfirmTransfer(ByVal FromAccount As String, ByVal ToAccount As String, ByVal SerialNumber As String, ByVal TransferNumber As String) As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim ActionType As String = ""
        Dim Action As String = ""
        Dim Reason As String = ""
        Dim OldValue As String = FromAccount
        Dim NewValue As String = ToAccount


        Action = "{'SerialNumber':'" & SerialNumber & "', 'FromAccount':'" & FromAccount & "', 'ToAccount':'" & ToAccount & "', 'TransferNumber':'" & TransferNumber & "'}"

        strSQLQuery = "UPDATE assets SET account = @ToAccount, modified_by = @modified_by, modified_date = @modified_date " & _
            "WHERE ((serialNum = @SerialNumber) AND (account = @FromAccount))"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@FromAccount", FromAccount.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@ToAccount", ToAccount.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = ""
                ActionType = "Asset Update"
                Reason = MyRecord.str
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Serial number " & SerialNumber & " failed to transfer from account " & FromAccount & " to account " & ToAccount & "! "
                MyRecord.str = ""
                ActionType = "Asset Update Failed"
                Reason = MyRecord.ErrorMessage
                OldValue = FromAccount
                NewValue = FromAccount
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = "An Exception has occured in CustomFunctions.vb/UpdateAssetOnConfirmTransfer()"
            ActionType = "Exception in CustomFunctions.vb/UpdateAssetOnConfirmTransfer()"
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
        GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "assets", "serialNum", SerialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        AddToLog("assets", "serialNum", GetRecordID.str, SerialNumber, ActionType, Action, Reason, OldValue, NewValue)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function FreeTrunkID(ByVal SerialNumber As String) As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim MyRecord As New ReturnObject
        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim Action As String = "{'SerialNumber':'" & SerialNumber.Trim() & "'}"
        Dim Reason As String = "" '"Cleared Trunk ID Record In Assets Table Where Serial Number " & SerialNumber.Trim() & " Existed"
        Dim ActionType As String = ""

        strSQLQuery = "UPDATE dbo.assets SET serialNum = '0', account = NULL, partNum = NULL, modelNum = NULL, modelDesc = NULL, " & _
            "cost = NULL, [9600B] = NULL, AES = NULL, OTAR = NULL, OTAP = NULL, assetComments = NULL, assetDisabled = NULL, " & _
            "assetDisabledDate = NULL, assetDisabledComments = NULL, modified_by = @modified_by, modified_date = @modified_date " & _
            "WHERE (serialNum = @SerialNumber)"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetRecordID As New ReturnObject
        GetRecordID = CustomFunctions.RecordIDFromTable("", SerialNumber, "assets", "serialNum", SerialNumber)
        '*****Reference: ID, UniqueValue, Table, Column, Value
        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber.Trim()))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Cleared Trunk ID Record In Assets Table Where Serial Number " & SerialNumber.Trim() & " Existed"
                ActionType = "Clear Trunk ID"
                Reason = MyRecord.str
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Serial number " & SerialNumber & " does not exist or failed to clear!"
                MyRecord.str = ""
                ActionType = "Clear Trunk ID Failed"
                Reason = MyRecord.ErrorMessage
            End If

            objDataReader.Close()

        Catch ex As Exception
            'str = "Error: " & ex.Message
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            ActionType = "Exception in CustomFunctions.FreeTrunkID()"
            Reason = MyRecord.ErrorMessage
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        AddToLog("assets", "serialNum", GetRecordID.str, SerialNumber, ActionType, Action, Reason, SerialNumber, "")
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function CustomFieldSearch(ByVal Column1 As String, ByVal Table As String, ByVal Column2 As String, ByVal Value As String) As String

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim str As String = ""

        'strSQLQuery = "SELECT DISTINCT MAX(transfer_number) + 1 AS NewTransferNumber FROM dbo.transfers"

        strSQLQuery = "SELECT " & Column1 & " FROM " & Table & " WHERE " & Column2 & " LIKE @Value ORDER BY " & Column1

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                '.Add(New System.Data.SqlClient.SqlParameter("@Column1", Column1))
                '.Add(New System.Data.SqlClient.SqlParameter("@Table", "assets"))
                '.Add(New System.Data.SqlClient.SqlParameter("@Column2", Column2))
                .Add(New System.Data.SqlClient.SqlParameter("@Value", "%" & Value.Trim() & "%"))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<div class='lightGrayBorder' style='float:left;'>" & _
                "<table><tr>" & _
                "<th>Column 1</th>" & _
                "<th>Action</th>" & _
                "</tr>"

            While objDataReader.Read()
                str = str & "<tr>" & _
                    "<td>" & objDataReader(Column1) & "</td>" & _
                    "<td style='text-align:center;'><input type='checkbox' id='ID_" & objDataReader(Column1) & "' class='' style='' /></td>" & _
                    "</tr>"
            End While

            If objDataReader.HasRows Then
                str = str & "<tr><td colspan='2' style='text-align:center;'><input type='button' id='InitiateArchive' value='Submit' /></td></tr>"
            Else
                str = str & "<tr><td colspan='2'>No Records Found!</td></tr>"
            End If

            objDataReader.Close()

            str = str & "</table></div>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function ChangeAccountStatus(ByVal Account As String, ByVal Status As String) As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim str As String = ""
        Dim MyRecord As New ReturnObject
        Dim OppositeStatus As String
        Dim Action As String = "{'Account':'" & Account & "', 'Status':'" & Status & "'}"
        Dim ActionType As String = ""
        Dim Reason As String = ""

        If Status = "True" Then OppositeStatus = "False" Else OppositeStatus = "True"

        strSQLQuery = "UPDATE dbo.accounts SET active = @Status WHERE account_code = @Account"

        '*****Get missing value, either record ID or a column value, for the input into the Log table
        'the record ID or column value is returned in the ".str" parameter of this object
        Dim GetUniqueValue As New ReturnObject
        GetUniqueValue = CustomFunctions.RecordIDFromTable("", Account, "accounts", "ID", Account)
        '*****Reference: ID, UniqueValue, Table, Column, Value

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@Account", Account))
                .Add(New System.Data.SqlClient.SqlParameter("@Status", Status))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_date", Now.ToShortDateString))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.RecordsAffected = 1 Then
                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Account " & Account & " has been successfully deleted!"
                Reason = "Account " & Account & " was successfully deleted, active state was set from true to false."
                ActionType = "Account Status Change"
            Else
                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Account " & Account & " was not deleted!  0 rows affected!"
                MyRecord.str = ""
                Reason = "Account " & Account & " was not deleted!  0 rows affected!"
                ActionType = "Account Status Change Failed"
            End If

            objDataReader.Close()

        Catch ex As Exception
            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = "An exception has occured in CustomFunctions.ChangeAccountStatus()"
            Reason = MyRecord.str & MyRecord.ErrorMessage
            ActionType = "Exception"
        Finally
            objConnection.Close()
        End Try

        '*****Log Events
        AddToLog("accounts", "account_code", GetUniqueValue.str, Account, ActionType, Action, Reason, Status, OppositeStatus)
        '*****Reference: Table, Column, Table ID, Unique Value, Action Type, Action, Reason, Old Value, New Value
        '*****Log Events

        Return MyRecord

    End Function

    Public Shared Function AccountManagers() As String

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String
        Dim str As String = ""
        Dim account As String = ""
        Dim counter As Integer = 0

        'strSQLQuery = "SELECT DISTINCT MAX(transfer_number) + 1 AS NewTransferNumber FROM dbo.transfers"
        strSQLQuery = "SELECT * FROM dbo.managers WHERE active = 'True' ORDER BY account_code, position DESC"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                '.Add(New System.Data.SqlClient.SqlParameter("@Column1", Column1))
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            str = str & "<table id='grid_Managers'><tr>" & _
                "<th id='Managers_Account'>Account</th>" & _
                "<th>Position</th>" & _
                "<th>Rank</th>" & _
                "<th>First Name</th>" & _
                "<th>Last Name</th>" & _
                "<th>Organization</th>" & _
                "<th>Phone</th>" & _
                "<th>Email</th>" & _
                "<th>Training Date</th>" & _
                "</tr>"

            While objDataReader.Read()

                If account <> objDataReader("account_code") Then
                    counter = counter + 1
                End If

                If counter Mod 2 = 0 Then
                    str = str & "<tr id='managers_" & objDataReader("ID") & "' style='background-color:#EEE;'>"
                Else
                    str = str & "<tr id='managers_" & objDataReader("ID") & "'>"
                End If

                account = objDataReader("account_code")

                str = str & "<td id='AccountID_" & objDataReader("ID") & "'>" & objDataReader("account_code") & "</td>" & _
                    "<td>" & objDataReader("position") & "</td>" & _
                    "<td>" & objDataReader("rank") & "</td>" & _
                    "<td>" & objDataReader("fname") & "</td>" & _
                    "<td>" & objDataReader("lname") & "</td>" & _
                    "<td>" & objDataReader("org") & "</td>" & _
                    "<td>" & objDataReader("phone") & "</td>" & _
                    "<td>" & objDataReader("email") & "</td>" & _
                    "<td class='" & StyleChange.SetColor(objDataReader("trained").ToString) & "'>" & objDataReader("trained") & "</td>" & _
                    "</tr>"

            End While

            'If objDataReader.HasRows Then
            'str = str & "<tr><td colspan='2' style='text-align:center;'><input type='button' id='InitiateArchive' value='Submit' /></td></tr>"
            'Else
            'str = str & "<tr><td colspan='2'>No Records Found!</td></tr>"
            'End If

            objDataReader.Close()

            str = str & "</table>"

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    'Public Shared Function ArchivedAssets() As String

    '    Dim objConnection As System.Data.SqlClient.SqlConnection
    '    objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
    '    Dim objCommand As System.Data.SqlClient.SqlCommand

    '    Dim strSQLQuery As String
    '    Dim str As String = ""
    '    Dim counter As Integer = 0

    '    strSQLQuery = "SELECT * FROM dbo.[archive-assets] WHERE trunkID NOT LIKE '%test%' ORDER BY serialNum"

    '    Try
    '        objConnection.Open()
    '        objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

    '        With objCommand.Parameters
    '            '.Add(New System.Data.SqlClient.SqlParameter("@Column1", Column1))
    '        End With

    '        Dim objDataReader As System.Data.SqlClient.SqlDataReader
    '        objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

    '        str = str & "<table>" & _
    '            "<th>Trunk ID</th>" & _
    '            "<th>Trunk ID Range</th>" & _
    '            "<th>Serial Number</th>" & _
    '            "<th>Account</th>" & _
    '            "<th>Model Number</th>" & _
    '            "<th>Model Description</th>" & _
    '            "<th>Cost</th>" & _
    '            "<th>9600B</th>" & _
    '            "<th>AES</th>" & _
    '            "<th>OTAR</th>" & _
    '            "<th>OTAP</th>" & _
    '            "<th>Original Asset Comments</th>" & _
    '            "<th>Archive Reason</th>" & _
    '            "<th>Archive Date</th>" & _
    '            "</tr>"

    '        While objDataReader.Read()

    '            If counter Mod 2 = 0 Then
    '                str = str & "<tr id='asset_" & objDataReader("ID") & "' style='background-color:#EEE;'>"
    '            Else
    '                str = str & "<tr id='asset_" & objDataReader("ID") & "'>"
    '            End If

    '            str = str & "<td>" & objDataReader("trunkID") & "</td>" & _
    '                "<td>" & objDataReader("trunkIDRange") & "</td>" & _
    '                "<td>" & objDataReader("serialNum") & "</td>" & _
    '                "<td>" & objDataReader("account") & "</td>" & _
    '                "<td>" & objDataReader("modelNum") & "</td>" & _
    '                "<td>" & objDataReader("modelDesc") & "</td>" & _
    '                "<td>" & objDataReader("cost") & "</td>" & _
    '                "<td>" & objDataReader("9600B") & "</td>" & _
    '                "<td>" & objDataReader("AES") & "</td>" & _
    '                "<td>" & objDataReader("OTAR") & "</td>" & _
    '                "<td>" & objDataReader("OTAP") & "</td>" & _
    '                "<td>" & objDataReader("assetComments") & "</td>" & _
    '                "<td>" & objDataReader("archiveReason") & "</td>" & _
    '                "<td>" & objDataReader("modified_date") & "</td>" & _
    '                "</tr>"
    '            counter = counter + 1
    '        End While

    '        'If objDataReader.HasRows Then
    '        'str = str & "<tr><td colspan='2' style='text-align:center;'><input type='button' id='InitiateArchive' value='Submit' /></td></tr>"
    '        'Else
    '        'str = str & "<tr><td colspan='2'>No Records Found!</td></tr>"
    '        'End If

    '        objDataReader.Close()

    '        str = str & "</table>"

    '    Catch ex As Exception
    '        str = "Error: " & ex.Message
    '    Finally
    '        objConnection.Close()
    '    End Try

    '    Return str

    'End Function

    'Public Shared Function UnaccountedFor() As String

    '    Dim objConnection As System.Data.SqlClient.SqlConnection
    '    objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
    '    Dim objCommand As System.Data.SqlClient.SqlCommand

    '    Dim strSQLQuery As String
    '    Dim str As String = ""
    '    Dim counter As Integer = 0

    '    strSQLQuery = "SELECT ID, trunkID, trunkIDRange, serialNum, account, modelNum, modelDesc, " & _
    '        "assetComments, assetDisabled " & _
    '        "FROM dbo.assets WHERE " & _
    '        "(account = 'NONE') ORDER BY trunkIDRange, serialNum"

    '    Try
    '        objConnection.Open()
    '        objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

    '        'With objCommand.Parameters
    '        '.Add(New System.Data.SqlClient.SqlParameter("@SerialNumber", "%" & SerialNumber.ToUpper & "%"))
    '        'End With

    '        Dim objDataReader As System.Data.SqlClient.SqlDataReader
    '        objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

    '        str = str & "<table id='grid_UnaccountedFor'><thead><tr>" & _
    '            "<th data-field='TrunkID'>Trunk ID</th>" & _
    '            "<th data-field='TrunkIDRange'>Trunk ID Range</th>" & _
    '            "<th data-field='SerialNumber'>Serial Number</th>" & _
    '            "<th data-field='Account'>Account</th>" & _
    '            "<th data-field='ModelNumber'>Model Number</th>" & _
    '            "<th data-field='ModelDescription'>Model Description</th>" & _
    '            "<th data-field='AssetComments'>Asset Comments</th>" & _
    '            "<th data-field='Disabled'>Disabled</th>" & _
    '            "</tr></thead>"

    '        While objDataReader.Read()
    '            If counter Mod 2 = 0 Then
    '                str = str & "<tr id='asset_" & objDataReader("ID") & "' style='background-color:#EEE;'>"
    '            Else
    '                str = str & "<tr id='asset_" & objDataReader("ID") & "'>"
    '            End If

    '            str = str & "<td>" & objDataReader("trunkID") & _
    '                        "<a id='ViewTrunkLog_" & objDataReader("trunkID") & "' style='float:right;' class='ui-icon ui-icon-clipboard' alt='View Log' title='View Log'></a></td>" & _
    '                "<td>" & objDataReader("trunkIDRange") & "</td>" & _
    '                "<td>" & objDataReader("serialNum") & "</td>" & _
    '                "<td class='UnaccountedFor_Account'>" & _
    '                    "<span id='UnaccountedFor_Account_" & objDataReader("ID") & "'>" & objDataReader("account") & "</span>" & _
    '                "</td>" & _
    '                "<td>" & objDataReader("modelNum") & "</td>" & _
    '                "<td>" & objDataReader("modelDesc") & "</td>" & _
    '                "<td>" & objDataReader("assetComments") & "</td>" & _
    '                "<td>" & objDataReader("assetDisabled") & "</td>" & _
    '                "</tr>"
    '            counter = counter + 1
    '        End While

    '        objDataReader.Close()

    '        str = str & "</table>"

    '    Catch ex As Exception
    '        str = "Error: " & ex.Message
    '    Finally
    '        objConnection.Close()
    '    End Try

    '    Return str

    'End Function

    Public Shared Function AddToLog(ByVal AffectedTable As String, ByVal ColumnName As String, ByVal AffectedTableID As String,
                                    ByVal UniqueValue As String, ByVal ActionType As String, ByVal Action As String,
                                    ByVal Reason As String, ByVal OldValue As String, ByVal NewValue As String) As String

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand
        Dim strSQLQuery As String

        strSQLQuery = "INSERT INTO dbo.Log (AffectedTable, ColumnName, AffectedTableID, UniqueValue, ActionType, Action, Reason, OldValue, NewValue, modified_by) " & _
            "VALUES (@AffectedTable, @ColumnName, @AffectedTableID, @UniqueValue, @ActionType, @Action, @Reason, @OldValue, @NewValue, @modified_by)"

        Dim str As String = "No Errors Found!"

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@AffectedTable", AffectedTable))
                .Add(New System.Data.SqlClient.SqlParameter("@ColumnName", ColumnName))
                .Add(New System.Data.SqlClient.SqlParameter("@AffectedTableID", AffectedTableID))
                .Add(New System.Data.SqlClient.SqlParameter("@UniqueValue", UniqueValue))
                .Add(New System.Data.SqlClient.SqlParameter("@ActionType", ActionType))
                .Add(New System.Data.SqlClient.SqlParameter("@Action", Action))
                .Add(New System.Data.SqlClient.SqlParameter("@Reason", Reason))
                .Add(New System.Data.SqlClient.SqlParameter("@OldValue", OldValue))
                .Add(New System.Data.SqlClient.SqlParameter("@NewValue", NewValue))
                .Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                ' do not need to insert modified_date, current date is set by default in the database
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            objDataReader.Close()

        Catch ex As Exception
            str = "Error: " & ex.Message
        Finally
            objConnection.Close()
        End Try

        Return str

    End Function

    Public Shared Function GrabActionTypeFromLog(ByVal ActionType As String, ByVal AffectedTable As String, ByVal AffectedTableID As String,
                                                 ByVal ColumnName As String, ByVal UniqueValue As String) As ReturnObject

        Dim objConnection As System.Data.SqlClient.SqlConnection
        objConnection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        Dim objCommand As System.Data.SqlClient.SqlCommand

        Dim strSQLQuery As String = ""
        Dim str As String = ""
        Dim Data As New ReturnObject

        If ActionType = "Account Update" Then
            strSQLQuery = "SELECT TOP(5) * FROM dbo.Log WHERE (ActionType = @ActionType) AND (ColumnName = @ColumnName) AND (AffectedTableID = @AffectedTableID) " & _
                "ORDER BY modified_date DESC, ID DESC"
        ElseIf ActionType = "Asset Update" Then
            strSQLQuery = "SELECT TOP(5) * FROM dbo.Log WHERE (ActionType = @ActionType) AND (ColumnName = @ColumnName) AND (AffectedTableID = @AffectedTableID) " & _
                "ORDER BY modified_date DESC, ID DESC"
        End If

        Try
            objConnection.Open()
            objCommand = New System.Data.SqlClient.SqlCommand(strSQLQuery, objConnection)

            With objCommand.Parameters
                .Add(New System.Data.SqlClient.SqlParameter("@AffectedTable", AffectedTable))
                .Add(New System.Data.SqlClient.SqlParameter("@ColumnName", ColumnName))
                .Add(New System.Data.SqlClient.SqlParameter("@AffectedTableID", AffectedTableID))
                .Add(New System.Data.SqlClient.SqlParameter("@UniqueValue", UniqueValue))
                .Add(New System.Data.SqlClient.SqlParameter("@ActionType", ActionType))
                '.Add(New System.Data.SqlClient.SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
                ' do not need to insert modified_date, current date is set by default in the database
            End With

            Dim objDataReader As System.Data.SqlClient.SqlDataReader
            objDataReader = objCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

            If objDataReader.HasRows Then
                Data.IsError = False
                Data.ErrorMessage = "No error on HasRows"
                Data.str = "<div id='' class='' style='padding:5px;'>"

                While objDataReader.Read()
                    Data.str = Data.str & "<div>" & _
                        "<div id='' class='' style='float:left; padding:2px;'>" & objDataReader("modified_date") & "</div>" & _
                        "<div id='' class='' style='float:right; padding:2px;'>Added By: " & ReplaceDomain(objDataReader("modified_by")) & "</div>" & _
                        "<div id='' class='Seperator' style='clear:both;'></div>" & _
                        "<div id='' class='' style='padding:2px 2px 10px 10px;'>" & objDataReader("NewValue") & "</div></div>"
                End While

                Data.str = Data.str & "</div>"
            Else
                Data.IsError = True
                Data.ErrorMessage = ""
                Data.str = ""
            End If

            objDataReader.Close()

        Catch ex As Exception
            Data.IsError = True
            Data.ErrorMessage = "Error: " & ex.Message
            Data.str = ""
        Finally
            objConnection.Close()
        End Try

        Return Data

    End Function

    Public Shared Function ReplaceDomain(ByVal Data As String) As String

        Data = Data.ToUpper()
        Data = Data.Replace("HILL-2K\", "")
        Return Data

    End Function

    Public Shared Function CheckIsNull(ByVal param As String) As String
        Dim data As String = ""
        If ((param Is Nothing) Or (param.Length < 1)) Then data = "NULL" Else data = param
        Return data
    End Function

    Public Shared Function NullCheck(ByVal DatabaseValue As Object) As String
        Dim NewValue As String = ""
        If (Not DatabaseValue Is Nothing) Then
            NewValue = DatabaseValue.ToString
        End If
        Return NewValue
    End Function

    Public Shared Function PageTabText(ByVal Page As String) As String

        Dim Text As String = ""
        Dim Server As String = HttpContext.Current.Request.ServerVariables("SERVER_NAME")

        If (Server.Contains("wbhill03")) Then
            Text = "HAFB PWCS - " & Page
        ElseIf (Server.Contains("wbhill08dev")) Then
            Text = "Server Development - " & Page
        ElseIf (Server.Contains("localhost")) Then
            Text = "Localhost Development - " & Page
        End If

        Return Text

    End Function

    Public Shared Function GetSettingsByPage(ByVal Page As String) As ReturnObject

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spGetSettingsByPage")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim MyRecord As New ReturnObject
        Dim DataDictionary As New Dictionary(Of String, String)

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@SettingPage", Page))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.HasRows Then

                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Settings successfully retrieved."

                While objDataReader.Read()
                    Dim Temp As New Dictionary(Of String, String)
                    Temp.Add(objDataReader("SettingName"), objDataReader("SettingValue"))
                    MyRecord.Data.Add(Temp)
                End While

            Else

                DataDictionary.Add("", "")

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "No settings found!"
                MyRecord.str = ""
                MyRecord.Data.Add(DataDictionary)

            End If

            objDataReader.Close()

        Catch ex As Exception

            DataDictionary.Add("Error", "Error: " & ex.Message)

            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""
            MyRecord.Data.Add(DataDictionary)

        Finally
            objConnection.Close()
        End Try

        Return MyRecord

    End Function

    Public Shared Function UpdateSettingsByPageAndName(ByVal SettingPage As String, ByVal SettingName As String, ByVal SettingValue As String) As ReturnObject

        SettingPage = HtmlEncode(SettingPage)
        SettingName = HtmlEncode(SettingName)
        SettingValue = HtmlEncode(SettingValue)

        Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
        'This SQLCommand needs to be the name of the stored procedure
        Dim objCommand As New SqlCommand("spUpdateSettingsByPageAndName")

        objCommand.CommandType = Data.CommandType.StoredProcedure
        objCommand.Connection = objConnection

        Dim MyRecord As New ReturnObject
        Dim Action As String = "{'table':'Settings', 'modifiedColumn':'SettingValue', 'modifiedColumnValue':'" & SettingValue & "', 'determiningColumn':'SettingName', 'determiningColumnValue':'" & SettingName & "'}"
        Dim Reason As String = ""

        Try
            'these parameters must match the paramters set in the stored procedure
            With objCommand.Parameters
                .Add(New SqlParameter("@SettingPage", SettingPage))
                .Add(New SqlParameter("@SettingName", SettingName))
                .Add(New SqlParameter("@SettingValue", SettingValue))
                .Add(New SqlParameter("@modified_by", HttpContext.Current.Request.ServerVariables("AUTH_USER")))
            End With

            objConnection.Open()

            Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

            If objDataReader.RecordsAffected = 1 Then

                MyRecord.IsError = False
                MyRecord.ErrorMessage = ""
                MyRecord.str = "Settings successfully updated."

                Reason = "Updated Value for Setting Name " & SettingName

            ElseIf objDataReader.RecordsAffected > 1 Then

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Error: " & objDataReader.RecordsAffected & " settings were modified!"
                MyRecord.str = ""

                Reason = MyRecord.ErrorMessage

            Else

                MyRecord.IsError = True
                MyRecord.ErrorMessage = "Error: No settings were updated successfully!"
                MyRecord.str = ""

                Reason = MyRecord.ErrorMessage

            End If

            objDataReader.Close()

        Catch ex As Exception

            MyRecord.IsError = True
            MyRecord.ErrorMessage = "Error: " & ex.Message
            MyRecord.str = ""

            Reason = MyRecord.ErrorMessage

        Finally
            objConnection.Close()

            Dim GetID As New ReturnObject
            GetID = RecordIDFromTable("", SettingName, "Settings", "SettingName", SettingName)

            CustomFunctions.AddToLog("Settings", "SettingValue", GetID.str, SettingName, "Settings Update", Action, Reason, "", SettingValue)

        End Try

        Return MyRecord

    End Function

End Class
