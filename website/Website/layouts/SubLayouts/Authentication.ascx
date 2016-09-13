<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_Authentication" CodeFile="Authentication.cs" %>

<script>
    var dob = '<%# this.DateOfBirth %>';
</script>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <p>Vítáme Vás, pane Nováku,</p>
            <p>pro zobrazení nabídky právě pro Vás bychom potřebovali ověřit níže uvedené údaje.</p>
            <div id="authentication" class="form">
                <div class="status"></div>
                <div class="row">
                    <div class="col-md-8 col-lg-6">
                        <div class="input-group">
                            <label for="birth">
                                <asp:Literal runat="server" ID="firstTxt" />
                            </label>
                            <input id="birth" name="birth" class="input-date" placeholder="napr. 26. 12. 1966" autocomplete="off" value="" required>
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
                            <input id="additional" name="additional" placeholder="Vložte Vaše zákaznické číslo" autocomplete="off" value="" required>
                        </div>
                    </div>
                </div>
                <button type="submit">
                    <asp:Literal runat="server" ID="buttonText" />
                </button>
            </div>
        </div>
    </div>
</div>
