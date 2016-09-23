﻿<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_AcceptedOffer" CodeFile="AcceptedOffer.cs" %>
<%@ Register Src="~/layouts/SubLayouts/DocumentsPanel.ascx" TagPrefix="pan" TagName="docPanel" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
           <asp:Literal runat="server" ID="MainText" />
        </div>
    </div>
</div>

<pan:docPanel runat="server" id="PanelDox" IsAccepted="true" /> 
