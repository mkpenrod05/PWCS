Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data.CommandBehavior

Public Class ClassModels

    Public Class ErrorCheck
        Public Property Message As String
        Public Property IsError As Boolean
        Public Property NullDate As String = "Empty"
    End Class

    Public Class Account
        Inherits ErrorCheck
        'Information pulled from dbo.accounts
        Public Property ID As Integer
        Public Property Active As Boolean
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

        'Default constructor which takes no arguments
        Public Sub New()

        End Sub

        'Overloaded constructor which takes an account code as an argument
        Public Sub New(ByVal AccountCode As String)
            Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
            'This SQLCommand needs to be the name of the stored procedure
            Dim objCommand As New SqlCommand("spSelectSingleAccount")
            objCommand.CommandType = Data.CommandType.StoredProcedure
            objCommand.Connection = objConnection

            Try
                'these parameters must match the paramters set in the stored procedure
                With objCommand.Parameters
                    .Add(New SqlParameter("@AccountCode", AccountCode))
                End With

                objConnection.Open()

                Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

                If objDataReader.HasRows Then

                    Me.IsError = False
                    Me.Message = "Information for account " & AccountCode.ToUpper() & " has been successfully retrieved from the database."

                    While objDataReader.Read()
                        'These columns need to be in the same order as how they appear in the database...
                        'Not sure why this is so but need to do some research on it!
                        If Not IsDBNull(objDataReader("ID")) Then Me.ID = objDataReader("ID")
                        If Not IsDBNull(objDataReader("active")) Then Me.Active = objDataReader("active")
                        If Not IsDBNull(objDataReader("account_code")) Then Me.AccountCode = objDataReader("account_code")
                        If Not IsDBNull(objDataReader("organization")) Then Me.Organization = objDataReader("organization")
                        If Not IsDBNull(objDataReader("appt_ltr")) Then Me.AppointmentLetter = objDataReader("appt_ltr") Else Me.AppointmentLetter = NullDate
                        If Not IsDBNull(objDataReader("inventory")) Then Me.Inventory = objDataReader("inventory") Else Me.Inventory = NullDate
                        If Not IsDBNull(objDataReader("account_validation")) Then Me.AccountValidation = objDataReader("account_validation") Else Me.AccountValidation = NullDate
                        If Not IsDBNull(objDataReader("email_acct_val_1")) Then Me.EmailAccountValidation1 = objDataReader("email_acct_val_1") Else Me.EmailAccountValidation1 = NullDate
                        If Not IsDBNull(objDataReader("email_acct_val_2")) Then Me.EmailAccountValidation2 = objDataReader("email_acct_val_2") Else Me.EmailAccountValidation2 = NullDate
                        If Not IsDBNull(objDataReader("email_acct_val_3")) Then Me.EmailAccountValidation3 = objDataReader("email_acct_val_3") Else Me.EmailAccountValidation3 = NullDate
                        If Not IsDBNull(objDataReader("email_inv_1")) Then Me.EmailInventory1 = objDataReader("email_inv_1") Else Me.EmailInventory1 = NullDate
                        If Not IsDBNull(objDataReader("email_inv_2")) Then Me.EmailInventory2 = objDataReader("email_inv_2") Else Me.EmailInventory2 = NullDate
                        If Not IsDBNull(objDataReader("email_inv_3")) Then Me.EmailInventory3 = objDataReader("email_inv_3") Else Me.EmailInventory3 = NullDate
                        If Not IsDBNull(objDataReader("email_appt_ltr_1")) Then Me.EmailAppointmentLetter1 = objDataReader("email_appt_ltr_1") Else Me.EmailAppointmentLetter1 = NullDate
                        If Not IsDBNull(objDataReader("email_appt_ltr_2")) Then Me.EmailAppointmentLetter2 = objDataReader("email_appt_ltr_2") Else Me.EmailAppointmentLetter2 = NullDate
                        If Not IsDBNull(objDataReader("email_appt_ltr_3")) Then Me.EmailAppointmentLetter3 = objDataReader("email_appt_ltr_3") Else Me.EmailAppointmentLetter3 = NullDate
                        If Not IsDBNull(objDataReader("account_comments")) Then Me.AccountComments = objDataReader("account_comments")
                    End While

                Else

                    Me.IsError = True
                    Me.Message = "No information found for account " & AccountCode.ToUpper() & "."

                End If

                objDataReader.Close()

            Catch ex As Exception
                Me.IsError = True
                Me.Message = "Error: " & ex.Message
            Finally
                objConnection.Close()
            End Try

        End Sub

    End Class

    Public Class Manager
        Inherits ErrorCheck

        Public Property ID As Integer
        Public Property Active As Boolean
        Public Property AccountCode As String
        Public Property Position As String
        Public Property Rank As String
        Public Property FirstName As String
        Public Property LastName As String
        Public Property Organization As String
        Public Property Phone As String
        Public Property Email As String
        Public Property TrainingDate As String

        'Default constructor which takes no arguments
        Public Sub New()

        End Sub

        'Overloaded constructor which takes an account code as an argument
        Public Sub New(ByVal ManagerID As Integer)

            Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
            'This SQLCommand needs to be the name of the stored procedure
            Dim objCommand As New SqlCommand("spSelectSingleManagerByID")
            objCommand.CommandType = Data.CommandType.StoredProcedure
            objCommand.Connection = objConnection

            Try
                'these parameters must match the paramters set in the stored procedure
                With objCommand.Parameters
                    .Add(New SqlParameter("@ID", ManagerID))
                End With

                objConnection.Open()

                Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

                If objDataReader.HasRows Then

                    Me.IsError = False
                    Me.Message = "Manager information has been successfully retrieved from the database."

                    While objDataReader.Read()
                        'These columns need to be in the same order as how they appear in the database...
                        'Not sure why this is so but need to do some research on it!
                        If Not IsDBNull(objDataReader("ID")) Then Me.ID = objDataReader("ID")
                        If Not IsDBNull(objDataReader("active")) Then Me.Active = objDataReader("active")
                        If Not IsDBNull(objDataReader("account_code")) Then Me.AccountCode = objDataReader("account_code")
                        If Not IsDBNull(objDataReader("position")) Then Me.Position = objDataReader("position")
                        If Not IsDBNull(objDataReader("rank")) Then Me.Rank = objDataReader("rank")
                        If Not IsDBNull(objDataReader("fname")) Then Me.FirstName = objDataReader("fname")
                        If Not IsDBNull(objDataReader("lname")) Then Me.LastName = objDataReader("lname")
                        If Not IsDBNull(objDataReader("org")) Then Me.Organization = objDataReader("org")
                        If Not IsDBNull(objDataReader("phone")) Then Me.Phone = objDataReader("phone")
                        If Not IsDBNull(objDataReader("email")) Then Me.Email = objDataReader("email")
                        If Not IsDBNull(objDataReader("trained")) Then Me.TrainingDate = objDataReader("trained") Else Me.TrainingDate = NullDate
                    End While

                Else

                    Me.IsError = True
                    Me.Message = "No information found for this manager."

                End If

                objDataReader.Close()

            Catch ex As Exception
                Me.IsError = True
                Me.Message = "Error: " & ex.Message
            Finally
                objConnection.Close()
            End Try

        End Sub

    End Class

    Public Class Asset
        Inherits ErrorCheck

        Public Property ID As Integer
        Public Property TrunkID As String
        Public Property TrunkIDRange As String
        Public Property SerialNumber As String
        Public Property AccountCode As String
        Public Property ModelNumber As String
        Public Property ModelDescription As String
        Public Property Cost As Decimal
        Public Property BaudRate As String
        Public Property AES As String
        Public Property OTAR As String
        Public Property OTAP As String
        Public Property AssetComments As String
        Public Property AssetDisabled As Boolean
        Public Property AssetDisabledDate As String
        Public Property AssetDisabledComments As String

        'Default constructor which takes no arguments
        Public Sub New()

        End Sub

        Public Sub New(ByVal AssetID As Integer)
            Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
            'This SQLCommand needs to be the name of the stored procedure
            Dim objCommand As New SqlCommand("spSelectAssetByID")
            objCommand.CommandType = Data.CommandType.StoredProcedure
            objCommand.Connection = objConnection

            Try
                'these parameters must match the paramters set in the stored procedure
                With objCommand.Parameters
                    .Add(New SqlParameter("@ID", AssetID))
                End With

                objConnection.Open()

                Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

                If objDataReader.HasRows Then

                    Me.IsError = False
                    Me.Message = "Asset information has been successfully retrieved from the database."

                    While objDataReader.Read()
                        'These columns need to be in the same order as how they appear in the database...
                        'Not sure why this is so but need to do some research on it!
                        If Not IsDBNull(objDataReader("ID")) Then Me.ID = objDataReader("ID")
                        If Not IsDBNull(objDataReader("trunkID")) Then Me.TrunkID = objDataReader("trunkID") Else Me.TrunkID = "0"
                        If Not IsDBNull(objDataReader("trunkIDRange")) Then Me.TrunkIDRange = objDataReader("trunkIDRange")
                        If Not IsDBNull(objDataReader("serialNum")) Then Me.SerialNumber = objDataReader("serialNum")
                        If Not IsDBNull(objDataReader("account")) Then Me.AccountCode = objDataReader("account")
                        If Not IsDBNull(objDataReader("modelNum")) Then Me.ModelNumber = objDataReader("modelNum")
                        If Not IsDBNull(objDataReader("modelDesc")) Then Me.ModelDescription = objDataReader("modelDesc")
                        If Not IsDBNull(objDataReader("cost")) Then Me.Cost = objDataReader("cost")
                        If Not IsDBNull(objDataReader("9600B")) Then Me.BaudRate = objDataReader("9600B")
                        If Not IsDBNull(objDataReader("AES")) Then Me.AES = objDataReader("AES")
                        If Not IsDBNull(objDataReader("OTAR")) Then Me.OTAR = objDataReader("OTAR")
                        If Not IsDBNull(objDataReader("OTAP")) Then Me.OTAP = objDataReader("OTAP")
                        If Not IsDBNull(objDataReader("assetComments")) Then Me.AssetComments = objDataReader("assetComments")
                        If Not IsDBNull(objDataReader("assetDisabled")) Then Me.AssetDisabled = objDataReader("assetDisabled")
                        If Not IsDBNull(objDataReader("assetDisabledDate")) Then Me.AssetDisabledDate = objDataReader("assetDisabledDate") Else Me.AssetDisabledDate = NullDate
                        If Not IsDBNull(objDataReader("assetDisabledComments")) Then Me.AssetDisabledComments = objDataReader("assetDisabledComments")
                    End While

                Else

                    Me.IsError = True
                    Me.Message = "No information found for this asset!"

                End If

                objDataReader.Close()

            Catch ex As Exception
                Me.IsError = True
                Me.Message = "Error: " & ex.Message
            Finally
                objConnection.Close()
            End Try

        End Sub

    End Class

    Public Class LogEvent
        Inherits ErrorCheck

        Public Property ID As Integer
        Public Property AffectedTable As String
        Public Property ColumnName As String
        Public Property AffectedTableID As Integer
        Public Property UniqueValue As String
        Public Property ActionType As String
        Public Property Reason As String
        Public Property OldValue As String
        Public Property NewValue As String
        Public Property ModifiedBy As String
        Public Property ModifiedDate As String
        Public Property RowNumber As Integer
        Public Property TotalRows As Integer

        'Default constructor which takes no arguments
        Public Sub New()

        End Sub

        Public Sub New(ByVal EventID As Integer)
            Dim objConnection As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("PWCS_DBConn").ConnectionString)
            'This SQLCommand needs to be the name of the stored procedure
            Dim objCommand As New SqlCommand("spSelectRecordFromLogByID")
            objCommand.CommandType = Data.CommandType.StoredProcedure
            objCommand.Connection = objConnection

            Try
                'these parameters must match the paramters set in the stored procedure
                With objCommand.Parameters
                    .Add(New SqlParameter("@ID", EventID))
                End With

                objConnection.Open()

                Dim objDataReader As SqlDataReader = objCommand.ExecuteReader(CloseConnection)

                If objDataReader.HasRows Then

                    Me.IsError = False
                    Me.Message = "Log event #" & EventID & " has been successfully retrieved from the database."

                    While objDataReader.Read()
                        'These columns need to be in the same order as how they appear in the database...
                        'Not sure why this is so but need to do some research on it!
                        If Not IsDBNull(objDataReader("ID")) Then Me.ID = objDataReader("ID")
                        If Not IsDBNull(objDataReader("AffectedTable")) Then Me.AffectedTable = objDataReader("AffectedTable")
                        If Not IsDBNull(objDataReader("ColumnName")) Then Me.ColumnName = objDataReader("ColumnName")
                        If Not IsDBNull(objDataReader("AffectedTableID")) Then Me.AffectedTableID = objDataReader("AffectedTableID")
                        If Not IsDBNull(objDataReader("UniqueValue")) Then Me.UniqueValue = objDataReader("UniqueValue")
                        If Not IsDBNull(objDataReader("ActionType")) Then Me.ActionType = objDataReader("ActionType")
                        If Not IsDBNull(objDataReader("Reason")) Then Me.Reason = objDataReader("Reason")
                        If Not IsDBNull(objDataReader("OldValue")) Then Me.OldValue = objDataReader("OldValue")
                        If Not IsDBNull(objDataReader("NewValue")) Then Me.NewValue = objDataReader("NewValue")
                        If Not IsDBNull(objDataReader("modified_by")) Then Me.ModifiedBy = objDataReader("modified_by")
                        If Not IsDBNull(objDataReader("modified_date")) Then Me.ModifiedDate = objDataReader("modified_date") Else Me.ModifiedDate = NullDate
                        'Me.RowNumber = objDataReader("RowNumber")
                        'Me.TotalRows = objDataReader("TotalRows")
                    End While

                Else

                    Me.IsError = True
                    Me.Message = "No information was found for log event #" & EventID & "!"

                End If

                objDataReader.Close()

            Catch ex As Exception
                Me.IsError = True
                Me.Message = "Error: " & ex.Message
            Finally
                objConnection.Close()
            End Try

        End Sub

    End Class

End Class
