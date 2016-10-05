<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_Disclaimer" CodeFile="Disclaimer.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <h1>
                <asp:Literal runat="server" ID="headerTxt" /></h1>
            <asp:Literal runat="server" ID="mainTxt" />
        </div>
    </div>
</div>
<div class="row">
    <div class="actions wide">
        <a runat="server" id="firstLinkA" target="_blank">
            <p>
                <asp:Literal runat="server" ID="firstLinkText" />
            </p>
        </a>
        <a runat="server" id="secondLinkA" target="_blank">
            <p>
                <asp:Literal runat="server" ID="secondLinkText" />
            </p>
        </a>
        <a runat="server" id="thirdLinkA" target="_blank">
            <p>
                <asp:Literal runat="server" ID="thirdLinkText" />
            </p>
        </a>
    </div>
</div>
