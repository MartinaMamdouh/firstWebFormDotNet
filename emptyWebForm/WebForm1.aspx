<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="emptyWebForm.WebForm1" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     

        <div>
            <h2>Employee Data</h2>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="employee_id">
                <Columns>
                    <asp:TemplateField>
                        
                    </asp:TemplateField>
                    <asp:BoundField DataField="employee_id" HeaderText="Employee ID" ReadOnly="True" SortExpression="employee_id" />
                    <asp:BoundField DataField="employee_name" HeaderText="Employee Name" SortExpression="employee_name" />
                    <asp:BoundField DataField="age" HeaderText="Employee Age" SortExpression="employee_age" />
                     <asp:BoundField DataField="salary" HeaderText="Employee Salary" SortExpression="employee_salary" />
                     <asp:BoundField DataField="termination_date" HeaderText="Employee Termination Date" SortExpression="employee_end_date" />
                </Columns>
            </asp:GridView>

            <br />

            <h2>Modify Employee</h2>
            <asp:TextBox ID="txtEmployeeId" runat="server" placeholder="Employee ID"></asp:TextBox>
            <asp:TextBox ID="txtEmployeeName" runat="server" placeholder="Employee Name"></asp:TextBox>
            <br />
            <asp:TextBox ID="txtEmployeeAge" runat="server" placeholder="Employee Age"></asp:TextBox>
            <asp:TextBox ID="txtEmployeeSalary" runat="server" placeholder="Employee Salary"></asp:TextBox>
            <br />
            <asp:TextBox ID="txtEmployeeDepId" runat="server" placeholder="Employee Department ID"></asp:TextBox>
            <br />
            <asp:Label ID="empEndDate" runat="server" Text="termination date: "></asp:Label>
            <asp:Calendar ID="Calendar1" runat="server" Height="16px" style="margin-right: 0px" Width="205px"></asp:Calendar>
      
           
            <br />
            
            
            <asp:Button ID="btnInsert" runat="server" Text="Insert Data" OnClick="btnInsert_Click"  />
            <asp:Button ID="btnView" runat="server" Text="View Data" OnClick="btnView_Click" />
            <asp:Button ID="btnModify" runat="server" Text="Modify Data" OnClick="btnModify_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="Delete Data" OnClick="btnDelete_Click" />
            
            </div>
    
    </form>
</body>
</html>
