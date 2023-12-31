﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.42000
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
// </auto-generated>
//------------------------------------------------------------------------------

using eContracting.Kernel.Helpers;

// 
// This source code was auto-generated by wsdl, Version=4.7.3081.0.
// 


     /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name = "CRM_SIGN_STAMP_MERGE", Namespace = "urn:sap-com:sprx:ep:cust:Z")]
public partial class CRM_SIGN_STAMP_MERGE : System.Web.Services.Protocols.SoapHttpClientProtocol
{

    private System.Threading.SendOrPostCallback invokeOperationCompleted;

    private bool useDefaultCredentialsSetExplicitly;

    public CRM_SIGN_STAMP_MERGE() : this(SystemHelpers.ReadConfig("eContracting.SigningServiceUrl"))
    {
    }

    /// <remarks/>
    public CRM_SIGN_STAMP_MERGE(string url)
    {
        this.Url = url;
        if ((this.IsLocalFileSystemWebService(this.Url) == true))
        {
            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }
        else
        {
            this.useDefaultCredentialsSetExplicitly = true;
        }
    }

    public new string Url
    {
        get
        {
            return base.Url;
        }
        set
        {
            if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                        && (this.useDefaultCredentialsSetExplicitly == false))
                        && (this.IsLocalFileSystemWebService(value) == false)))
            {
                base.UseDefaultCredentials = false;
            }
            base.Url = value;
        }
    }

    public new bool UseDefaultCredentials
    {
        get
        {
            return base.UseDefaultCredentials;
        }
        set
        {
            base.UseDefaultCredentials = value;
            this.useDefaultCredentialsSetExplicitly = true;
        }
    }

    /// <remarks/>
    public event invokeCompletedEventHandler invokeCompleted;

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:sprx:ep:cust:Z:CRM_SIGN_STAMP_MERGE:invokeRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
    [return: System.Xml.Serialization.XmlElementAttribute("invokeResponse", Namespace = "http://adobe.com/idp/services")]
    public invokeResponse invoke([System.Xml.Serialization.XmlElementAttribute("invoke", Namespace = "http://adobe.com/idp/services")] invoke invoke1)
    {
        object[] results = this.Invoke("invoke", new object[] {
                        invoke1});
        return ((invokeResponse)(results[0]));
    }

    /// <remarks/>
    public void invokeAsync(invoke invoke1)
    {
        this.invokeAsync(invoke1, null);
    }

    /// <remarks/>
    public void invokeAsync(invoke invoke1, object userState)
    {
        if ((this.invokeOperationCompleted == null))
        {
            this.invokeOperationCompleted = new System.Threading.SendOrPostCallback(this.OninvokeOperationCompleted);
        }
        this.InvokeAsync("invoke", new object[] {
                        invoke1}, this.invokeOperationCompleted, userState);
    }

    private void OninvokeOperationCompleted(object arg)
    {
        if ((this.invokeCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.invokeCompleted(this, new invokeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    public new void CancelAsync(object userState)
    {
        base.CancelAsync(userState);
    }

    private bool IsLocalFileSystemWebService(string url)
    {
        if (((url == null)
                    || (url == string.Empty)))
        {
            return false;
        }
        System.Uri wsUri = new System.Uri(url);
        if (((wsUri.Port >= 1024)
                    && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
        {
            return true;
        }
        return false;
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://adobe.com/idp/services")]
public partial class invoke
{

    private BLOB inputPDFField;

    private BLOB inputPNGSignField;

    private string overlayField;

    /// <remarks/>
    public BLOB inputPDF
    {
        get
        {
            return this.inputPDFField;
        }
        set
        {
            this.inputPDFField = value;
        }
    }

    /// <remarks/>
    public BLOB inputPNGSign
    {
        get
        {
            return this.inputPNGSignField;
        }
        set
        {
            this.inputPNGSignField = value;
        }
    }

    /// <remarks/>
    public string overlay
    {
        get
        {
            return this.overlayField;
        }
        set
        {
            this.overlayField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://adobe.com/idp/services")]
public partial class BLOB
{

    private string contentTypeField;

    private byte[] binaryDataField;

    private string attachmentIDField;

    private string remoteURLField;

    private byte[] mTOMField;

    private string swaRefField;

    private MyMapOf_xsd_string_To_xsd_anyType_Item[] attributesField;

    /// <remarks/>
    public string contentType
    {
        get
        {
            return this.contentTypeField;
        }
        set
        {
            this.contentTypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
    public byte[] binaryData
    {
        get
        {
            return this.binaryDataField;
        }
        set
        {
            this.binaryDataField = value;
        }
    }

    /// <remarks/>
    public string attachmentID
    {
        get
        {
            return this.attachmentIDField;
        }
        set
        {
            this.attachmentIDField = value;
        }
    }

    /// <remarks/>
    public string remoteURL
    {
        get
        {
            return this.remoteURLField;
        }
        set
        {
            this.remoteURLField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
    public byte[] MTOM
    {
        get
        {
            return this.mTOMField;
        }
        set
        {
            this.mTOMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
    public string swaRef
    {
        get
        {
            return this.swaRefField;
        }
        set
        {
            this.swaRefField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
    public MyMapOf_xsd_string_To_xsd_anyType_Item[] attributes
    {
        get
        {
            return this.attributesField;
        }
        set
        {
            this.attributesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://adobe.com/idp/services")]
public partial class MyMapOf_xsd_string_To_xsd_anyType_Item
{

    private string keyField;

    private object valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public string key
    {
        get
        {
            return this.keyField;
        }
        set
        {
            this.keyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public object value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://adobe.com/idp/services")]
public partial class invokeResponse
{

    private int errCodeField;

    private string errMsgField;

    private BLOB outputPDFField;

    /// <remarks/>
    public int errCode
    {
        get
        {
            return this.errCodeField;
        }
        set
        {
            this.errCodeField = value;
        }
    }

    /// <remarks/>
    public string errMsg
    {
        get
        {
            return this.errMsgField;
        }
        set
        {
            this.errMsgField = value;
        }
    }

    /// <remarks/>
    public BLOB outputPDF
    {
        get
        {
            return this.outputPDFField;
        }
        set
        {
            this.outputPDFField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
public delegate void invokeCompletedEventHandler(object sender, invokeCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class invokeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal invokeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public invokeResponse Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((invokeResponse)(this.results[0]));
        }
    }
}

#pragma warning restore 1591
