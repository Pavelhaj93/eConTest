<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentsPanel.ascx.cs" Inherits="website_Website_layouts_DocumentsPanel" %>

<script>
    function CheckIfReady() {
        var check = setInterval(function () {
            $.ajax({
                url: 'DoxReady.ashx',
                error: function () {
                    alert('Error');
                },
                success: function (result) {
                    if (result) {
                        clearInterval(check);
                        documentsReceived();
                    }
                },
                dataType: "text",
                type: 'GET'
            });
        }, 2000);

        return true;
    }
    CheckIfReady();

    function ddd(par) {
       
        //document.getElementById('xxx').click();
    }

</script>

<script>
    var documents = <%# this.FilesJson %>;
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
                                <li>
                                    <input id="document-1" type="checkbox" autocomplete="off">
                                    <label for="document-1">
                                        Souhlasím s <a class="pdf" href="http://google.com" title="Podminkami pro uzivatele" target="_blank">Podminkami pro uzivatele</a>
                                    </label>
                                </li>

                                <li>
                                    <input id="document-2" type="checkbox" autocomplete="off">
                                    <label for="document-2">
                                        Souhlasím s <a class="pdf" href="http://google.com" title="My custom document" target="_blank">My custom document</a>
                                    </label>
                                </li>
                                <li>
                                    <input id="document-3" type="checkbox" autocomplete="off">
                                    <label for="document-3">
                                        Souhlasím s <a class="pdf" href="http://google.com" title="Dodatkem ke smlouve" target="_blank">Dodatkem ke smlouve</a>
                                    </label>
                                </li>
                                <li>
                                    <input id="document-4" type="checkbox" autocomplete="off">
                                    <label for="document-4">
                                        Souhlasím s <a class="pdf" href="http://youtube.com" title="Ceníkem produktu" target="_blank">Ceníkem produktu</a>
                                    </label>
                                </li>
                                <li>
                                    <input id="document-5" type="checkbox" autocomplete="off">
                                    <label for="document-5">
                                        Souhlasím s <a class="pdf" href="http://vimeo.com" title="Ceníkem služeb" target="_blank">Ceníkem služeb</a>
                                    </label>
                                </li>
                                <li>
                                    <input id="document-6" type="checkbox" autocomplete="off">
                                    <label for="document-6">
                                        Souhlasím s <a class="pdf" href="http://envato.com" title="Obchodními podmínkami" target="_blank">Obchodními podmínkami</a>
                                    </label>
                                </li>
                            </ul>
                            <button type="submit" disabled>Akceptuji</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

