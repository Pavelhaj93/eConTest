//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using eContracting.Kernel.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=4.0.30319.33440.
// 


/// <remarks/>
// CODEGEN: The optional WSDL extension element 'Policy' from namespace 'http://schemas.xmlsoap.org/ws/2004/09/policy' was not handled.
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="ZCCH_CACHE_API", Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_API : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback ZCCH_CACHE_PUTOperationCompleted;
    
    private System.Threading.SendOrPostCallback ZCCH_CACHE_STATUS_SETOperationCompleted;
    
    private System.Threading.SendOrPostCallback ZCCH_CACHE_GETOperationCompleted;
    
    /// <remarks/>
    public ZCCH_CACHE_API():this(SystemHelpers.ReadConfig("eContracting.ServiceUrl"))
    {
    }

    public ZCCH_CACHE_API(string url)
    {
        this.Url = url;
    }

    /// <remarks/>
    public event ZCCH_CACHE_PUTCompletedEventHandler ZCCH_CACHE_PUTCompleted;
    
    /// <remarks/>
    public event ZCCH_CACHE_STATUS_SETCompletedEventHandler ZCCH_CACHE_STATUS_SETCompleted;
    
    /// <remarks/>
    public event ZCCH_CACHE_GETCompletedEventHandler ZCCH_CACHE_GETCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:rfc:functions:ZCCH_CACHE_API:ZCCH_CACHE_PUTRequest", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
    [return: System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_PUTResponse", Namespace="urn:sap-com:document:sap:rfc:functions")]
    public ZCCH_CACHE_PUTResponse ZCCH_CACHE_PUT([System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_PUT", Namespace="urn:sap-com:document:sap:rfc:functions")] ZCCH_CACHE_PUT ZCCH_CACHE_PUT1) {
        object[] results = this.Invoke("ZCCH_CACHE_PUT", new object[] {
                    ZCCH_CACHE_PUT1});
        return ((ZCCH_CACHE_PUTResponse)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginZCCH_CACHE_PUT(ZCCH_CACHE_PUT ZCCH_CACHE_PUT1, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("ZCCH_CACHE_PUT", new object[] {
                    ZCCH_CACHE_PUT1}, callback, asyncState);
    }
    
    /// <remarks/>
    public ZCCH_CACHE_PUTResponse EndZCCH_CACHE_PUT(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((ZCCH_CACHE_PUTResponse)(results[0]));
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_PUTAsync(ZCCH_CACHE_PUT ZCCH_CACHE_PUT1) {
        this.ZCCH_CACHE_PUTAsync(ZCCH_CACHE_PUT1, null);
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_PUTAsync(ZCCH_CACHE_PUT ZCCH_CACHE_PUT1, object userState) {
        if ((this.ZCCH_CACHE_PUTOperationCompleted == null)) {
            this.ZCCH_CACHE_PUTOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZCCH_CACHE_PUTOperationCompleted);
        }
        this.InvokeAsync("ZCCH_CACHE_PUT", new object[] {
                    ZCCH_CACHE_PUT1}, this.ZCCH_CACHE_PUTOperationCompleted, userState);
    }
    
    private void OnZCCH_CACHE_PUTOperationCompleted(object arg) {
        if ((this.ZCCH_CACHE_PUTCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.ZCCH_CACHE_PUTCompleted(this, new ZCCH_CACHE_PUTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:rfc:functions:ZCCH_CACHE_API:ZCCH_CACHE_STATUS_SETReques" +
        "t", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
    [return: System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_STATUS_SETResponse", Namespace="urn:sap-com:document:sap:rfc:functions")]
    public ZCCH_CACHE_STATUS_SETResponse ZCCH_CACHE_STATUS_SET([System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_STATUS_SET", Namespace="urn:sap-com:document:sap:rfc:functions")] ZCCH_CACHE_STATUS_SET ZCCH_CACHE_STATUS_SET1) {
        object[] results = this.Invoke("ZCCH_CACHE_STATUS_SET", new object[] {
                    ZCCH_CACHE_STATUS_SET1});
        return ((ZCCH_CACHE_STATUS_SETResponse)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginZCCH_CACHE_STATUS_SET(ZCCH_CACHE_STATUS_SET ZCCH_CACHE_STATUS_SET1, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("ZCCH_CACHE_STATUS_SET", new object[] {
                    ZCCH_CACHE_STATUS_SET1}, callback, asyncState);
    }
    
    /// <remarks/>
    public ZCCH_CACHE_STATUS_SETResponse EndZCCH_CACHE_STATUS_SET(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((ZCCH_CACHE_STATUS_SETResponse)(results[0]));
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_STATUS_SETAsync(ZCCH_CACHE_STATUS_SET ZCCH_CACHE_STATUS_SET1) {
        this.ZCCH_CACHE_STATUS_SETAsync(ZCCH_CACHE_STATUS_SET1, null);
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_STATUS_SETAsync(ZCCH_CACHE_STATUS_SET ZCCH_CACHE_STATUS_SET1, object userState) {
        if ((this.ZCCH_CACHE_STATUS_SETOperationCompleted == null)) {
            this.ZCCH_CACHE_STATUS_SETOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZCCH_CACHE_STATUS_SETOperationCompleted);
        }
        this.InvokeAsync("ZCCH_CACHE_STATUS_SET", new object[] {
                    ZCCH_CACHE_STATUS_SET1}, this.ZCCH_CACHE_STATUS_SETOperationCompleted, userState);
    }
    
    private void OnZCCH_CACHE_STATUS_SETOperationCompleted(object arg) {
        if ((this.ZCCH_CACHE_STATUS_SETCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.ZCCH_CACHE_STATUS_SETCompleted(this, new ZCCH_CACHE_STATUS_SETCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:rfc:functions:ZCCH_CACHE_API:ZCCH_CACHE_GETRequest", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
    [return: System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_GETResponse", Namespace="urn:sap-com:document:sap:rfc:functions")]
    public ZCCH_CACHE_GETResponse ZCCH_CACHE_GET([System.Xml.Serialization.XmlElementAttribute("ZCCH_CACHE_GET", Namespace="urn:sap-com:document:sap:rfc:functions")] ZCCH_CACHE_GET ZCCH_CACHE_GET1) {
        object[] results = this.Invoke("ZCCH_CACHE_GET", new object[] {
                    ZCCH_CACHE_GET1});
        return ((ZCCH_CACHE_GETResponse)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginZCCH_CACHE_GET(ZCCH_CACHE_GET ZCCH_CACHE_GET1, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("ZCCH_CACHE_GET", new object[] {
                    ZCCH_CACHE_GET1}, callback, asyncState);
    }
    
    /// <remarks/>
    public ZCCH_CACHE_GETResponse EndZCCH_CACHE_GET(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((ZCCH_CACHE_GETResponse)(results[0]));
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_GETAsync(ZCCH_CACHE_GET ZCCH_CACHE_GET1) {
        this.ZCCH_CACHE_GETAsync(ZCCH_CACHE_GET1, null);
    }
    
    /// <remarks/>
    public void ZCCH_CACHE_GETAsync(ZCCH_CACHE_GET ZCCH_CACHE_GET1, object userState) {
        if ((this.ZCCH_CACHE_GETOperationCompleted == null)) {
            this.ZCCH_CACHE_GETOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZCCH_CACHE_GETOperationCompleted);
        }
        this.InvokeAsync("ZCCH_CACHE_GET", new object[] {
                    ZCCH_CACHE_GET1}, this.ZCCH_CACHE_GETOperationCompleted, userState);
    }
    
    private void OnZCCH_CACHE_GETOperationCompleted(object arg) {
        if ((this.ZCCH_CACHE_GETCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.ZCCH_CACHE_GETCompleted(this, new ZCCH_CACHE_GETCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_PUT {
    
    private ZCCH_ST_ATTRIB[] iT_ATTRIBField;
    
    private ZCCH_ST_FILE[] iT_FILESField;
    
    private string iV_CCHKEYField;
    
    private string iV_CCHTYPEField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_ATTRIB[] IT_ATTRIB {
        get {
            return this.iT_ATTRIBField;
        }
        set {
            this.iT_ATTRIBField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_FILE[] IT_FILES {
        get {
            return this.iT_FILESField;
        }
        set {
            this.iT_FILESField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHKEY {
        get {
            return this.iV_CCHKEYField;
        }
        set {
            this.iV_CCHKEYField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHTYPE {
        get {
            return this.iV_CCHTYPEField;
        }
        set {
            this.iV_CCHTYPEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_ST_ATTRIB {
    
    private string aTTRIDField;
    
    private string aTTRINDXField;
    
    private string aTTRVALField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ATTRID {
        get {
            return this.aTTRIDField;
        }
        set {
            this.aTTRIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ATTRINDX {
        get {
            return this.aTTRINDXField;
        }
        set {
            this.aTTRINDXField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ATTRVAL {
        get {
            return this.aTTRVALField;
        }
        set {
            this.aTTRVALField = value;
        }
    }

    public override string ToString()
    {
        return $"ATTRID = '{this.ATTRID}', ATTRVAL = '{this.ATTRVAL}'";
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_ST_HEADER {
    
    private string cCHTYPEField;
    
    private string cCHKEYField;
    
    private string cCHSTATField;
    
    private string cCHVALTOField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CCHTYPE {
        get {
            return this.cCHTYPEField;
        }
        set {
            this.cCHTYPEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CCHKEY {
        get {
            return this.cCHKEYField;
        }
        set {
            this.cCHKEYField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CCHSTAT {
        get {
            return this.cCHSTATField;
        }
        set {
            this.cCHSTATField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CCHVALTO {
        get {
            return this.cCHVALTOField;
        }
        set {
            this.cCHVALTOField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class BAPIRET2 {
    
    private string tYPEField;
    
    private string idField;
    
    private string nUMBERField;
    
    private string mESSAGEField;
    
    private string lOG_NOField;
    
    private string lOG_MSG_NOField;
    
    private string mESSAGE_V1Field;
    
    private string mESSAGE_V2Field;
    
    private string mESSAGE_V3Field;
    
    private string mESSAGE_V4Field;
    
    private string pARAMETERField;
    
    private int rOWField;
    
    private string fIELDField;
    
    private string sYSTEMField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string TYPE {
        get {
            return this.tYPEField;
        }
        set {
            this.tYPEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ID {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string NUMBER {
        get {
            return this.nUMBERField;
        }
        set {
            this.nUMBERField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MESSAGE {
        get {
            return this.mESSAGEField;
        }
        set {
            this.mESSAGEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string LOG_NO {
        get {
            return this.lOG_NOField;
        }
        set {
            this.lOG_NOField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string LOG_MSG_NO {
        get {
            return this.lOG_MSG_NOField;
        }
        set {
            this.lOG_MSG_NOField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MESSAGE_V1 {
        get {
            return this.mESSAGE_V1Field;
        }
        set {
            this.mESSAGE_V1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MESSAGE_V2 {
        get {
            return this.mESSAGE_V2Field;
        }
        set {
            this.mESSAGE_V2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MESSAGE_V3 {
        get {
            return this.mESSAGE_V3Field;
        }
        set {
            this.mESSAGE_V3Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MESSAGE_V4 {
        get {
            return this.mESSAGE_V4Field;
        }
        set {
            this.mESSAGE_V4Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PARAMETER {
        get {
            return this.pARAMETERField;
        }
        set {
            this.pARAMETERField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int ROW {
        get {
            return this.rOWField;
        }
        set {
            this.rOWField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FIELD {
        get {
            return this.fIELDField;
        }
        set {
            this.fIELDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SYSTEM {
        get {
            return this.sYSTEMField;
        }
        set {
            this.sYSTEMField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_ST_FILE {
    
    private string fILEINDXField;
    
    private string fILENAMEField;
    
    private string mIMETYPEField;
    
    private byte[] fILECONTENTField;
    
    private ZCCH_ST_ATTRIB[] aTTRIBField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FILEINDX {
        get {
            return this.fILEINDXField;
        }
        set {
            this.fILEINDXField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FILENAME {
        get {
            return this.fILENAMEField;
        }
        set {
            this.fILENAMEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MIMETYPE {
        get {
            return this.mIMETYPEField;
        }
        set {
            this.mIMETYPEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
    public byte[] FILECONTENT {
        get {
            return this.fILECONTENTField;
        }
        set {
            this.fILECONTENTField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_ATTRIB[] ATTRIB {
        get {
            return this.aTTRIBField;
        }
        set {
            this.aTTRIBField = value;
        }
    }

    public override string ToString()
    {
        return $"FILENAME = {this.FILENAME}";
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_PUTResponse {
    
    private ZCCH_ST_ATTRIB[] eT_ATTRIBField;
    
    private ZCCH_ST_FILE[] eT_FILESField;
    
    private BAPIRET2[] eT_RETURNField;
    
    private int eV_RETCODEField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_ATTRIB[] ET_ATTRIB {
        get {
            return this.eT_ATTRIBField;
        }
        set {
            this.eT_ATTRIBField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_FILE[] ET_FILES {
        get {
            return this.eT_FILESField;
        }
        set {
            this.eT_FILESField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public BAPIRET2[] ET_RETURN {
        get {
            return this.eT_RETURNField;
        }
        set {
            this.eT_RETURNField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int EV_RETCODE {
        get {
            return this.eV_RETCODEField;
        }
        set {
            this.eV_RETCODEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_STATUS_SET {
    
    private string iV_CCHKEYField;
    
    private string iV_CCHTYPEField;
    
    private string iV_STATField;
    
    private decimal iV_TIMESTAMPField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHKEY {
        get {
            return this.iV_CCHKEYField;
        }
        set {
            this.iV_CCHKEYField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHTYPE {
        get {
            return this.iV_CCHTYPEField;
        }
        set {
            this.iV_CCHTYPEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_STAT {
        get {
            return this.iV_STATField;
        }
        set {
            this.iV_STATField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public decimal IV_TIMESTAMP {
        get {
            return this.iV_TIMESTAMPField;
        }
        set {
            this.iV_TIMESTAMPField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_STATUS_SETResponse {
    
    private BAPIRET2[] eT_RETURNField;
    
    private int eV_RETCODEField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public BAPIRET2[] ET_RETURN {
        get {
            return this.eT_RETURNField;
        }
        set {
            this.eT_RETURNField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int EV_RETCODE {
        get {
            return this.eV_RETCODEField;
        }
        set {
            this.eV_RETCODEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_GET {
    
    private string iV_CCHKEYField;
    
    private string iV_CCHTYPEField;
    
    private string iV_GEFILEField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHKEY {
        get {
            return this.iV_CCHKEYField;
        }
        set {
            this.iV_CCHKEYField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_CCHTYPE {
        get {
            return this.iV_CCHTYPEField;
        }
        set {
            this.iV_CCHTYPEField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IV_GEFILE {
        get {
            return this.iV_GEFILEField;
        }
        set {
            this.iV_GEFILEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
public partial class ZCCH_CACHE_GETResponse {
    
    private ZCCH_ST_HEADER eS_HEADERField;
    
    private ZCCH_ST_ATTRIB[] eT_ATTRIBField;
    
    private ZCCH_ST_FILE[] eT_FILESField;
    
    private BAPIRET2[] eT_RETURNField;
    
    private int eV_RETCODEField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ZCCH_ST_HEADER ES_HEADER {
        get {
            return this.eS_HEADERField;
        }
        set {
            this.eS_HEADERField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_ATTRIB[] ET_ATTRIB {
        get {
            return this.eT_ATTRIBField;
        }
        set {
            this.eT_ATTRIBField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public ZCCH_ST_FILE[] ET_FILES {
        get {
            return this.eT_FILESField;
        }
        set {
            this.eT_FILESField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public BAPIRET2[] ET_RETURN {
        get {
            return this.eT_RETURNField;
        }
        set {
            this.eT_RETURNField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int EV_RETCODE {
        get {
            return this.eV_RETCODEField;
        }
        set {
            this.eV_RETCODEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
public delegate void ZCCH_CACHE_PUTCompletedEventHandler(object sender, ZCCH_CACHE_PUTCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ZCCH_CACHE_PUTCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal ZCCH_CACHE_PUTCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public ZCCH_CACHE_PUTResponse Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((ZCCH_CACHE_PUTResponse)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
public delegate void ZCCH_CACHE_STATUS_SETCompletedEventHandler(object sender, ZCCH_CACHE_STATUS_SETCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ZCCH_CACHE_STATUS_SETCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal ZCCH_CACHE_STATUS_SETCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public ZCCH_CACHE_STATUS_SETResponse Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((ZCCH_CACHE_STATUS_SETResponse)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
public delegate void ZCCH_CACHE_GETCompletedEventHandler(object sender, ZCCH_CACHE_GETCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ZCCH_CACHE_GETCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal ZCCH_CACHE_GETCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public ZCCH_CACHE_GETResponse Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((ZCCH_CACHE_GETResponse)(this.results[0]));
        }
    }
}