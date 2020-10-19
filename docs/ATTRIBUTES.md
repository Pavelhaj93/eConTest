# Attributes

## Offer attributes

### IV_CCHKEY

Unique offer identifier.

### IV_CCHTYPE

Type of offer document (NABIDKA, NABIDKA_PDF, NABIDKA_ARCH)

#### - NABIDKA

Contains 2 files
1. basic information about offer
2. texts for website.

#### - NABIDKA_PDF

Contains 1+ files for not accepted offer (PDF files).

#### - NABIDKA_ARCH

Contains 1+ files for accepted offer (PDF files).

### IV_STAT

States in what offer could be.

- 1 - Zapsáno / Written (not used)
- 2 - Automatizase / Automation (not used)
- 3 - Aktivní / Active (is readable and documents were generated correctly)
- 4 - Přečteno / Read (when user logged in and saw offer page)
- 5 - Akceptováno / Accepted (if offer was accepted)
- 6 - Přihlášeno / Logged-in
- 8 - Chyba / Error (not used)
- 9 - Zastaralé / Obsolote (when offer is cancelled)

### IV_TIMESTAMP

Current timestamp as decimal number.

## TEMPLATE attributes

When we are reading XML response from SAP, there are few attributes good to know.

#### EED
Identifies a file with name "Dodatek" for electricity offer type for **default** and **retention**.

#### EPD
Identifies a file with name "Dodatek" for gas offer type for **default** and **retention**.

#### EES
Identifies a file with name "Smlouva" for electricity offery type for **acquisition**.

#### EPS
Idenfifies a file with name "Smlouva" for gas offer type for **acquisition**.