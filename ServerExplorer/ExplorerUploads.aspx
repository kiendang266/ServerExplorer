<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExplorerUploads.aspx.cs"
    Inherits="ServerExplorer.ExplorerUploads" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        html, body
        {
            margin: 10px;
            height: 100%;
            font-family: Verdana;
            overflow: hidden;
            background-color: #eee;
        }
        label, input
        {
            float: left;
            margin-left: 10px;
            font-size: 11px;
            display: block;
            height: 20px;
        }
        label
        {
            line-height: 20px;
        }
        input
        {
            background-color: #fff;
        }
        .hidden
        {
            display: none;
        }
        span.error
        {
            font-size: 9px;
            margin-left: 20px;
        }
    </style>

    <script src="scripts/jsTree/jquery.js" type="text/javascript"></script>

    <script type="text/javascript">
        function checkDir() {
            if ($('#dir').val() == "") {
                alert("You must select a directory first");
            }
            else if ($('#id').val() == "") {
                alert("You must select a file");
            }
            else {
                $(formulario).submit();
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox CssClass="hidden" runat="server" ID="dir" Text=""></asp:TextBox>
        <label>New File</label>
        <asp:FileUpload runat="server" ID="fu" />
        <asp:Button ID="Button1" runat="server" Text="Upload" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="error" Display="Dynamic" runat="server" ControlToValidate="dir" ErrorMessage="You must select a directory first."></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="error" Display="Dynamic" runat="server" ControlToValidate="fu" ErrorMessage="You must select a file"></asp:RequiredFieldValidator>
    </div>
    </form>
</body>
</html>
