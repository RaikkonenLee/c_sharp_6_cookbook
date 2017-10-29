using System;
using System.Web;

public partial class WebForm1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            WriteOrderResponse();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        WriteOrderResponse();
    }

    private void WriteOrderResponse()
    {
        string response = "Thanks for the order!<br/>";
        response += "Identity: " + Request.Form["Identity"] + "<br/>";
        response += "Item: " + Request.Form["Item"] + "<br/>";
        response += "Quantity: " + Request.Form["Quantity"] + "<br/>";
        Response.Write(response);
    }
}
