<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebForm1.aspx.cs" Inherits="WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WebForm1</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	
		<asp:Table ID="Table1" runat="server" Height="139px" Width="361px">
			<asp:TableRow runat="server">
				<asp:TableCell runat="server"><asp:Label ID="Label1" runat="server" Text="Identity"></asp:Label></asp:TableCell>
				<asp:TableCell runat="server"><asp:TextBox ID="Identity" runat="server"/></asp:TableCell>
			</asp:TableRow>
			<asp:TableRow runat="server">
				<asp:TableCell runat="server"><asp:Label ID="Label2" runat="server" Text="Item"></asp:Label></asp:TableCell>
				<asp:TableCell runat="server"><asp:TextBox ID="Item" runat="server"/></asp:TableCell>
			</asp:TableRow>
			<asp:TableRow runat="server">
				<asp:TableCell runat="server"><asp:Label ID="Label3" runat="server" Text="Quantity"></asp:Label></asp:TableCell>
				<asp:TableCell runat="server"><asp:TextBox ID="Quantity" runat="server"/></asp:TableCell>
			</asp:TableRow>
			<asp:TableRow runat="server">
				<asp:TableCell runat="server"></asp:TableCell>
				<asp:TableCell runat="server"><asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Submit" /></asp:TableCell>
			</asp:TableRow>
		</asp:Table>
    
    </div>
    </form>
</body>
</html>
