<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modal.aspx.cs" Inherits="eContracting.Website.Areas.eContracting.Views.Content.ModalWindows.Modal" %>

<!DOCTYPE html>

<head runat="server">
  <title>My Modal Window</title>
  <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:Button ID="btnOK" runat="server" text="OK" onclick="btnOK_Click" />
      <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" />
    </div>
    </form>
</body>
</html>
