<%@ WebService Language="VB" Class="WebService" %>

'Supplies classes and interfaces that enable browser-server communication, also includes classes for cookie manipulation, 
'file transfer, exception information, and output cache control.
Imports System.Web

'The System.Web.Services namespace consists of the classes that enable you to create XML Web services using ASP.NET and XML Web service clients.
Imports System.Web.Services

'The System.Web.Services.Protocols namespace consists of the classes that define the protocols used to transmit data across 
'the wire during the communication between XML Web service clients and XML Web services created using ASP.NET.
Imports System.Web.Services.Protocols

'The System.Web.Script.Services namespace provides attributes that let you customize Web service support for AJAX functionality in ASP.NET.
Imports System.Web.Script.Services

'Contains classes that provide JavaScript Object Notation (JSON) serialization and deserialization for managed types.
'Imports System.Web.Script.Serialization

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class WebService
    Inherits System.Web.Services.WebService
    
    <WebMethod()> _
    Public Function AccountList() As String
        'Return WebServiceFunctions.AccountList()
    End Function
    
    <WebMethod()> _
    Public Function SerialNumberSearch(ByVal SearchValue As String, ByVal SearchConstraint As String) As String
        Return WebServiceFunctions.SerialNumberSearch(SearchValue, SearchConstraint)
    End Function
    
    <WebMethod()> _
    Public Function TrunkIdAndSerialNumber(ByVal Account As String) As String
        'Return WebServiceFunctions.TrunkIdAndSerialNumber(Account)
    End Function
    
    <WebMethod()> _
    Public Function ManagersInformation(ByVal Account As String) As String
        'Return WebServiceFunctions.ManagersInformation(Account)
    End Function
    
    <WebMethod()> _
    Public Function SerialNumberInformation(ByVal SerialNum As String) As String
        Return WebServiceFunctions.SerialNumberInformation(SerialNum)
    End Function
    
    <WebMethod()> _
    Public Function SerialNumberMaintenanceHistory(ByVal SerialNum As String) As String
        Return WebServiceFunctions.SerialNumberMaintenanceHistory(SerialNum)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AccountInfo(ByVal Account As String) As AccountInfoObject
        Return WebServiceFunctions.AccountInfo(Account)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AddManager(ByVal NewPosition As String, ByVal NewRank As String, ByVal NewFirstName As String,
                               ByVal NewLastName As String, ByVal NewOrg As String, ByVal NewPhone As String,
                               ByVal NewEmail As String, ByVal NewTrainingDate As String, ByVal Account As String) As AccountInfoObject
        Return WebServiceFunctions.AddManager(NewPosition, NewRank, NewFirstName, NewLastName, NewOrg, NewPhone, NewEmail, NewTrainingDate, Account)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ChangeManagerStatus(ByVal Status As String, ByVal ManagerID As String) As ReturnObject
        Return WebServiceFunctions.ChangeManagerStatus(Status, ManagerID)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function CheckAccountStatus(ByVal Status As String, ByVal Account As String) As AccountInfoObject
        Return WebServiceFunctions.CheckAccountStatus(Status, Account)
    End Function
    
    <WebMethod()> _
    Public Function TrunkingSystemLogByID(ByVal TrunkID As String) As String
        Return WebServiceFunctions.TrunkingSystemLogByID(TrunkID)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AccountInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String,
                                      ByVal new_value As String) As jeipRecord2
        Return WebServiceFunctions.AccountInfoUpdate(url, id, form_type, orig_value, new_value)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ManagerInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String,
                                      ByVal new_value As String) As jeipRecord2
        Return WebServiceFunctions.ManagerInfoUpdate(url, id, form_type, orig_value, new_value)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function SerialNumberInfoUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String,
                                           ByVal new_value As String) As jeipRecord2
        Return WebServiceFunctions.SerialNumberInfoUpdate(url, id, form_type, orig_value, new_value)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ArchiveAsset(ByVal SerialNumber As String, ByVal ArchiveReason As String, ByVal AIMTransactionNumber As String,
                                 ByVal AIMCageCode As String, ByVal AIMModelNumber As String, ByVal AIMModelDescription As String) As ReturnObject
        Return WebServiceFunctions.ArchiveAsset(SerialNumber, ArchiveReason, AIMTransactionNumber, AIMCageCode, AIMModelNumber, AIMModelDescription)
    End Function
    
    <WebMethod()> _
    Public Function addMaintenanceAction(ByVal invoiceNumber As String, ByVal serialNumber As String, ByVal description As String,
                                         ByVal dateOfAction As String, ByVal cost As String) As String
        Return WebServiceFunctions.addMaintenanceAction(invoiceNumber, serialNumber, description, dateOfAction, cost)
    End Function
    
    <WebMethod()> _
    Public Function SerialNumberSearchForTransfer(ByVal Account As String) As String
        Return WebServiceFunctions.SerialNumberSearchForTransfer(Account)
    End Function
    
    <WebMethod()> _
    Public Function AddTransfer(ByVal TransferNumber As String, ByVal FromAccount As String, ByVal ToAccount As String, ByVal TransferDate As String) As String
        Return WebServiceFunctions.AddTransfer(TransferNumber, FromAccount, ToAccount, TransferDate)
    End Function
    
    <WebMethod()> _
    Public Function AddTransfersList(ByVal TransferNumber As String, ByVal SerialNumber As String) As String
        Return WebServiceFunctions.AddTransfersList(TransferNumber, SerialNumber)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ConfirmTransfer(ByVal TransferNumber As String, ByVal Cancelled As String) As ReturnObject
        Return WebServiceFunctions.ConfirmTransfer(TransferNumber, Cancelled)
    End Function
    
    <WebMethod()> _
    Public Function TransferList(ByVal TransferNumber As String) As String
        Return WebServiceFunctions.TransferList(TransferNumber)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ActiveTransfers() As ReturnObject
        Return CustomFunctions.ActiveTransfers()
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AccountAddition(ByVal AccountCode As String, ByVal Unit As String, ByVal AccountComments As String) As JSONObject
        Return WebServiceFunctions.AccountAddition(AccountCode, Unit, AccountComments)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AssetAddition(ByVal TrunkID As String, ByVal SerialNumber As String, ByVal ModelNumber As String, ByVal ModelDescription As String,
                                  ByVal AssetComments As String, ByVal Cost As String, ByVal Baud As String, ByVal AES As String, ByVal OTAR As String,
                                  ByVal OTAP As String, ByVal QueryType As String) As JSONObject
        Return WebServiceFunctions.AssetAddition(TrunkID, SerialNumber, ModelNumber, ModelDescription, AssetComments, Cost, Baud, AES, OTAR, OTAP, QueryType)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function SwapTrunkID(ByVal SerialNumber As String, ByVal OldTrunkID As String, ByVal OldRecordID As String, ByVal NewTrunkID As String, ByVal NewRecordID As String) As JSONObject
        Return WebServiceFunctions.SwapTrunkID(SerialNumber, OldTrunkID, OldRecordID, NewTrunkID, NewRecordID)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function CheckTrunkIDAvailable(ByVal TrunkID As String) As JSONObject
        Return WebServiceFunctions.CheckTrunkIDAvailable(TrunkID)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function CheckSerialNumberAvailable(ByVal SerialNumber As String, ByVal ActionIfFound As String) As JSONObject
        Return WebServiceFunctions.CheckSerialNumberAvailable(SerialNumber, ActionIfFound)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function EIPAssetUpdate(ByVal url As String, ByVal id As String, ByVal form_type As String, ByVal orig_value As String, ByVal new_value As String, ByVal data As String) As jeipRecord2
        Return WebServiceFunctions.EIPAssetUpdate(url, id, form_type, orig_value, new_value, data)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GrabActionType(ByVal ActionType As String, ByVal AffectedTable As String, ByVal AffectedTableID As String,
                                   ByVal ColumnName As String, ByVal UniqueValue As String) As ReturnObject
        Return CustomFunctions.GrabActionTypeFromLog(ActionType, AffectedTable, AffectedTableID, ColumnName, UniqueValue)
    End Function
    
    <WebMethod()> _
    Public Function MaintenanceBadActors() As String
        Return WebServiceFunctions.MaintenanceBadActors()
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function TransferedInAim(ByVal RecordNumber As String, ByVal TransferNumber As String, ByVal TransferValue As String) As ReturnObject
        Return WebServiceFunctions.TransferedInAim(RecordNumber, TransferNumber, TransferValue)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function CopyAsset(ByVal SerialNumber As String) As JSONObject
        Return WebServiceFunctions.CopyAsset(SerialNumber)
    End Function
    
    
    '*****************************************************************
    'KENDO UI WEB METHODS
    '*****************************************************************
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function JsonManagersList(ByVal page As Integer, ByVal take As Integer) As KendoObject
        Return WebServiceFunctions.JsonManagersList(page, take)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function ArchivedAssets(ByVal page As Integer, ByVal take As Integer) As KendoObject
        Return WebServiceFunctions.ArchivedAssets(page, take)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function UnaccountedFor(ByVal page As Integer, ByVal take As Integer) As KendoObject
        Return WebServiceFunctions.UnaccountedFor(page, take)
    End Function
    
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function AllOpenTrunkID(ByVal page As Integer, ByVal take As Integer) As KendoObject
        Return WebServiceFunctions.AllOpenTrunkID(page, take)
    End Function
    
End Class
