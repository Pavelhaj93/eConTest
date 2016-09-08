<%@ Page Language="c#" CodePage="65001" AutoEventWireup="true" %>
<%@ OutputCache Location="None" VaryByParam="none" %>
<%@ Register TagPrefix="rwe" TagName="CookieLaw"  Src="~/layouts/CookieLaw.ascx" %>
<!DOCTYPE html>
<html lang="cs">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="initial-scale=1, user-scalable=no">
    <title><%=Sitecore.Context.Item["PageTitle"] %></title>
    <link type="image/x-icon" rel="shortcut icon" href="gfx/favicon/blob.png" />
    <link href="gfx/favicon/57x57.png" sizes="57x57" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/114x114.png" sizes="114x114" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/72x72.png" sizes="72x72" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/144x144.png" sizes="144x144" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/60x60.png" sizes="60x60" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/120x120.png" sizes="120x120" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/76x76.png" sizes="76x76" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/152x152.png" sizes="152x152" rel="apple-touch-icon-precomposed" />
    <link href="gfx/favicon/196x196.png" sizes="196x196" type="image/png" rel="icon" />
    <link href="gfx/favicon/96x96.png" sizes="96x96" type="image/png" rel="icon" />
    <link href="gfx/favicon/32x32.png" sizes="32x32" type="image/png" rel="icon" />
    <link href="gfx/favicon/16x16.png" sizes="16x16" type="image/png" rel="icon" />
    <link href="gfx/favicon/128x128.png" sizes="128x128" type="image/png" rel="icon" />
    <meta content="Innogy" name="application-name" />
    <meta content="#FFFFFF" name="msapplication-TileColor" />
    <meta content="gfx/favicon/144x144.png" name="msapplication-TileImage" />
    <meta content="gfx/favicon/70x70.png" name="msapplication-square70x70logo" />
    <meta content="gfx/favicon/150x150.png" name="msapplication-square150x150logo" />
    <meta content="gfx/favicon/310x150.png" name="msapplication-wide310x150logo" />
    <meta content="gfx/favicon/310x310.png" name="msapplication-square310x310logo" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <%--   <link href="/default.css" rel="stylesheet" />--%>
    <link rel="stylesheet" href="css/main.min.css">
    <sc:visitoridentification runat="server" />
</head>
<body>
    <rwe:CookieLaw runat="server" id="cookieLaw" />
    <header class="header">
        <div class="container">
            <div class="row">
                <a class="logo" href="index.html">
                    <svg class="icon" preserveAspectRatio="none">
                        <use xlink:href="gfx/icon/svg.svg#logo" />
                    </svg>
                </a><a class="phone" href="tel:800113355">800 11 33 55</a>
            </div>
        </div>
    </header>
    <div class="main">
        <div class="container">
            <form method="post" runat="server" id="mainform">
                <sc:placeholder key="main" runat="server" />
            </form>
        </div>
    </div>
    <div class="pre-footer">
        <div class="container">
            <div class="row">
                <div class="col-sm-6">
                    <p>© <span id="copyright">2016</span> Innogy. Všechna práva vyhrazena.</p>
                </div>
                <div class="col-sm-6 text-right">
                    <div class="contact-info"><a class="email" href="mailto:info@innogy.cz">info@innogy.cz</a> <a class="phone" href="tel:800113355">800 11 33 55</a> </div>
                </div>
            </div>
        </div>
    </div>
    <footer class="footer">
        <div class="container">
            <p><a href="disclaimer.html">Ochrana osobnich údajů</a></p>
        </div>
    </footer>
    <script src="js/app.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"></script>
    <script>
        var dob = '30.08.2016';
    </script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.4/js/bootstrap-datepicker.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.4/locales/bootstrap-datepicker.cs.min.js"></script>
    <script>
        app.start(
        {});
    </script>
</body>
</html>
