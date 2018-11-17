<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dataanalycs.aspx.cs" Inherits="farmarproject2.dataanalycs" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Chart ID="Chart1" runat="server" DataSourceID="SqlDataSource1">
                <series>
                    <asp:Series Name="Series1" XValueMember="category" YValueMembers="Expr1">
                    </asp:Series>
                </series>
                <chartareas>
                    <asp:ChartArea Name="ChartArea1">
                    </asp:ChartArea>
                </chartareas>
            </asp:Chart>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:farmarConnectionString %>" SelectCommand="SELECT category, COUNT(category) AS Expr1 FROM order_detail GROUP BY category"></asp:SqlDataSource>
            <br />
            <asp:Chart ID="Chart2" runat="server" DataSourceID="SqlDataSource1">
                <series>
                    <asp:Series ChartType="Pie" Name="Series1" XValueMember="category" YValueMembers="Expr1">
                    </asp:Series>
                </series>
                <chartareas>
                    <asp:ChartArea Name="ChartArea1">
                    </asp:ChartArea>
                </chartareas>
            </asp:Chart>
        </div>
    </form>
</body>
</html>
