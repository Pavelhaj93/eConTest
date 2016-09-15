<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_Authentication" CodeFile="Authentication.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <p>Vítáme Vás, pane
                <asp:Literal runat="server" ID="nameLit" /></p>
            <p>pro zobrazení nabídky právě pro Vás bychom potřebovali ověřit níže uvedené údaje.</p>
            <div id="authentication" class="form">
                <div class="status"></div>
                <div class="row">
                    <div class="col-md-8 col-lg-6">
                        <div class="input-group">
                            <label for="birth">
                                <asp:Literal runat="server" ID="firstTxt" />
                            </label>
                            <asp:TextBox runat="server" ID="birth" ClientIDMode="Static" CssClass="input-date" autocomplete="off" required />
                            <a class="ui-datepicker-trigger" href="" tabindex="-1">
                                <svg class="icon" preserveAspectRatio="none">
                                    <use xlink:href="gfx/icon/svg.svg#calendar" />
                                </svg>
                            </a>
                        </div>
                        <div class="input-group">
                            <label for="additional">
                                <asp:Literal runat="server" ID="secondTxt" />
                            </label>
                            <asp:TextBox runat="server" ID="additional" ClientIDMode="Static" name="additional"
                                autocomplete="off" required />
                        </div>
                    </div>
                </div>
                <asp:Button runat="server" CssClass="button button-submit" ID="mainBtn" OnClick="mainBtn_ServerClick" Text="sdsdsd" />
            </div>
        </div>
    </div>
</div>