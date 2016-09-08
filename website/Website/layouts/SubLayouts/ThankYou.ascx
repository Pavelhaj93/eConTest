<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_ThankYou" CodeFile="ThankYou.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <p>Vitame vas, pane Novaku,<br>
                děkujeme, že jste akceptoval naši nabídku a sjednal produkt <strong>zemního plynu OPTIMAL</strong>.</p>
            <p>V následujících hodinách obdržíte e-mail s odkazem k potvreným smluvním dokumentům.</p>
            <p>Děkujeme,
                <br>
                Vaše Innogy.</p>
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
