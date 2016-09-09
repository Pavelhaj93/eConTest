<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_AcceptedOffer" CodeFile="AcceptedOffer.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
           <asp:Literal runat="server" ID="MainText" />
        </div>
    </div>
</div>
<div class="row">
    <div class="col-sm-12 col-md-10 col-md-offset-1">
        <div class="form-background">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-10 col-md-offset-1">
                        <ul id="accepted-documents" class="list list-documents loading"></ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
