<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_SystemError" CodeFile="SystemError.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <h1>Nedostupnost aplikace eContracting</h1>
            <p>Vážený pane Novaku,</p>
            <p>omlouváme se Vám, ale aplikaci eContracting se nepodařilo spustit. Příčinou může být právě probíhající plánovaná odstávka aplikace od <strong>20. 6. 2016 12:30</strong> do <strong>20. 6. 2016 15:30</strong>.</p>
            <p>V případě potřeby využijte naši zákaznickou linku uvedenou v pravém horním rohy stránky.</p>
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
