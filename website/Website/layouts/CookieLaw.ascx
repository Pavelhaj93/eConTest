<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CookieLaw.ascx.cs" Inherits="website_Website_WebControls_CookieLaw" %>

<div id="notification">
    <div class="container-fluid">
        <p>
            <asp:Literal runat="server" ID="mainText" />
        </p>
        <div class="actions">
            <p>
                <a runat="server" id="PersonalDataLink" class="external" target="_blank">
                    <asp:Literal runat="server" ID="PersonalDataText" />
                </a>
            </p>

            <p>
                <a class="button agree" href="">
                    <asp:Literal runat="server" ID="AgreeButton" />
                </a>
            </p>
        </div>
    </div>
</div>
