<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="FormSim.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>WebForm1</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    </HEAD>
    <body MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" runat="server">
            <asp:TextBox id="Identity" style="Z-INDEX: 101; LEFT: 194px; 
POSITION: absolute; TOP: 52px" runat="server"></asp:TextBox>
            <asp:TextBox id="Item" style="Z-INDEX: 102; LEFT: 193px; 
POSITION: absolute; TOP: 93px" runat="server"></asp:TextBox>
            <asp:TextBox id="Quantity" style="Z-INDEX: 103; LEFT: 193px; 
POSITION: absolute; TOP: 132px"
                runat="server"></asp:TextBox>
            <asp:Button id="Button1" style="Z-INDEX: 104; LEFT: 203px; 
POSITION: absolute; TOP: 183px" runat="server"
                Text="Submit"></asp:Button>
            <asp:Label id="Label1" style="Z-INDEX: 105; LEFT: 58px; 
POSITION: absolute; TOP: 54px" runat="server"
                Width="122px" Height="24px">Identity:</asp:Label>
            <asp:Label id="Label2" style="Z-INDEX: 106; LEFT: 57px; 
POSITION: absolute; TOP: 94px" runat="server"
                Width="128px" Height="25px">Item:</asp:Label>
            <asp:Label id="Label3" style="Z-INDEX: 107; LEFT: 57px; 
POSITION: absolute; TOP: 135px" runat="server"
                Width="124px" Height="20px">Quantity:</asp:Label>
        </form>
    </body>
</HTML>
