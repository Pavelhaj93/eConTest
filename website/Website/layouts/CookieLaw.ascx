<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CookieLaw.ascx.cs" Inherits="website_Website_WebControls_CookieLaw" %>

<div id="notification">
    <div class="container-fluid">
        <p>
            <asp:Literal runat="server" ID="mainText" />
        </p>
        <div class="actions">
            <a runat="server" id="PersonalDataLink" class="external" target="_blank">
                <asp:Literal runat="server" ID="PersonalDataText" />
            </a>

            <a class="button agree" href="">
                <asp:Literal runat="server" ID="AgreeButton" />
            </a>
        </div>
    </div>
</div>
