﻿<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Inherits="website_Website_WebControls_Disclaimer" CodeFile="Disclaimer.cs" %>

<div class="row">
    <div class="col-md-8 col-md-offset-2">
        <div class="info">
            <h1>Ochrana osobních údajů</h1>
            <h2>Souhlas se zpracovaním osobních údajů</h2>
            <p>
                Zaškrtnutím příslušného políčka a následným odesláním souvisejícího formuláře uživatel uděluje souhlas ve smyslu zákona č. 101/2000 Sb., o ochraně osobních údajů, společnostem skupiny Innogy, a to Innogy Česká republika a.s., IČ 24275051,
                se sídlem Limuzská 3135/12, PSČ 100 98, Praha 10 – Strašnice a Innogy Energie, s.r.o., IČ 49903209, se sídlem Limuzská 3135/12, PSČ 108 00, Praha 10 – Strašnice, dále jen „správce“, se zpracováním jeho e-mailové adresy, jména, příjmení
                a telefonního čísla výhradně pro marketingové účely správce, tj. nabízení výrobků a služeb, včetně zasílání informací o pořádaných akcích, výrobcích a jiných aktivitách, jakož i zasílání obchodních sdělení prostřednictvím elektronických
                prostředků dle zákona č. 480/2004 Sb., a to na dobu 10 let. Tento souhlas platí rovněž pro správcem pověřené zpracovatele, za předpokladu, že budou splněny podmínky § 5 odst. 6 zákona č. 101/2000 Sb., o ochraně osobních údajů. Uživatel
                bere na vědomí, že má práva dle § 11, 21 zák. č. 101/2000 Sb., tj. zejména že poskytnutí údajů je dobrovolné, že svůj souhlas může bezplatně kdykoliv na adrese správce odvolat, že má právo přístupu k osobním údajům a právo na opravu těchto
                osobních údajů, blokování nesprávných osobních údajů, jejich likvidaci atd.
            </p>
            <h2>Cookie a session</h2>
            <p>
                Plnohodnotné použití web stránek není podmíněno registrací. Abychom každému uživateli web stránek dokázali vždy zobrazit správný obsah, automaticky zakládáme pro každého uživatele tzv. session. Pro potřeby správy session může webová stránka
                uchovávat na Vašem počítači soubory cookies. Session a cookie používáme výhradně pro uchování informace o průchodu webem a pro správné zobrazení obsahu každému uživateli, přičemž session i cookie jsou zcela anonymní a neobsahují žádná
                osobní data.
            </p>
            <p>Soubory cookies můžete smazat nebo zakázat změnou nastavení na vašem počítači, může tím ale dojít k omezení funkcí webu www.innogy.cz/online-servis.</p>
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