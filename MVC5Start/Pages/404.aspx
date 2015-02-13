<%@ Page Language="C#" %>
<%
    this.Response.StatusCode = 401;
    this.Response.ContentType = "text/html; charset=utf-8";
    this.Response.WriteFile(this.MapPath("~/Pages/404.html"));
%>