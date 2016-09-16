<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentsPanel.ascx.cs" Inherits="website_Website_layouts_DocumentsPanel" %>

<script>
    function CheckIfReady() {
        $.ajax({
            type: "POST",
            url: 'DoxReady.ashx?id=<%# this.ClientId%>',
            dataType: "json",
            timeout: 30000,
            error: function () {
                documentsReceived(false);
            },
            success: function (documents) {
                documentsReceived(documents);
            },
        });
    }

    CheckIfReady();

    window.handleClick = function (e, key) {
        e.preventDefault();
        $.post("GetFile.ashx?file=" + key, null);
        window.location.href = "GetFile.ashx?file=" + key;
    }
</script>

<div class="row">
    <div class="col-sm-12 col-md-10 col-md-offset-1">
        <div class="form-background">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-10 col-md-offset-1">
                        <div id="offer" class="form">
                            <ul class="list list-documents loading">
                                <a class="check-all" href="">Označit vše</a>
                            </ul>
                            <asp:Button runat="server" CssClass="button button-submit" ID="mainBtn" OnClick="btnNext_Click" Text="Akceptuji" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

