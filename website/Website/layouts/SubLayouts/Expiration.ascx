﻿<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_Expiration" CodeFile="Expiration.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <p>
                Vážený pane Nováku,<br>
                naše obchodní nabídka pozbyla platnosti <strong>28. dubna 2016</strong>.
            </p>
            <p>Pokud byste se rád dozvěděl detaily Vaši nabídky, prosím kontaktujte nás na telefonu <a href="tel:800113355">800 11 33 55</a>.</p>
            <p>
                Děkujeme,
                <br>
                Vaše Innogy.
            </p>
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