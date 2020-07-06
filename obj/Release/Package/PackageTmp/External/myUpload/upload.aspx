<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="PKLib_Data.External.myUpload.upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />

            <asp:Button ID="Button1" runat="server" Text="GO" OnClick="Button1_Click" Width="100" Height="100" />
            
            <hr />
            <a href="upload.aspx">重新整理</a>
        </div>
        <hr />
        <div>
            <asp:Button ID="Button2" runat="server" Text="Button" OnClick="Button2_Click" Enabled="false" />
        </div>
    </form>
</body>
</html>
