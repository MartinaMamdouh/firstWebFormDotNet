<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="PrjDepEmpForm.aspx.cs" Inherits="emptyWebForm.PrjDepEmpForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div>

        <h2>Projects, Department, Employee Data</h2>
        <p>
            <asp:TextBox ID="txtprj" placeholder="project" runat="server"></asp:TextBox>
            &nbsp;
            <%--                <asp:TextBox ID="txtsec" placeholder="sector" runat="server"></asp:TextBox>
                &nbsp;
                <asp:TextBox ID="txtdep" placeholder="department" runat="server"></asp:TextBox>--%>
            <asp:DropDownList ID="DropDownList5" runat="server" DataSource='<%# GetAllSectors() %>' AutoPostBack="true" OnSelectedIndexChanged="DropDownList5_SelectedIndexChanged">
            </asp:DropDownList>
            &nbsp;
            <asp:DropDownList ID="DropDownList3" runat="server" OnSelectedIndexChanged="DepartmentDropDownFilter_SelectedIndexChanged" AutoPostBack="true">
            </asp:DropDownList>
            &nbsp;
              <asp:DropDownList ID="DropDown" runat="server">
              </asp:DropDownList>
            <%--<asp:TextBox ID="txtemp" placeholder="employee" runat="server"></asp:TextBox>--%>
            <br />
            <asp:Button ID="btnFilter" runat="server" ForeColor="#009933" OnClick="btn_filter" Text="Filter" />

        </p>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id, project_id, department_id, employee_id" OnRowEditing="GridView1_RowEditing" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting" ShowFooter="True">
            <Columns>
                <asp:TemplateField HeaderText="project" SortExpression="project_name">

                    <ItemTemplate>
                        <%# Eval("project_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSource='<%# GetAllProjects() %>' Text='<%# Bind("project_name") %>'>
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required1" runat="server" ControlToValidate="DropDownList1"
                            InitialValue="select Project" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="sector" SortExpression="sector">

                    <ItemTemplate>
                        <%# Eval("sector_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="SectorDropDown" runat="server" OnSelectedIndexChanged="SectorDropDown_SelectedIndexChanged" AutoPostBack="true" DataSource='<%# GetAllSectors() %>' Text='<%# Bind("sector_name") %>'>
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required2" runat="server" ControlToValidate="SectorDropDown"
                            InitialValue="Select Sector" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="department" SortExpression="department">

                    <ItemTemplate>
                        <%# Eval("department_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="depDropDown" runat="server" Enabled="false" OnSelectedIndexChanged="DepartmentDropDown_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required3" runat="server" ControlToValidate="depDropDown"
                            InitialValue="select Department" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="employee" SortExpression="employee">

                    <ItemTemplate>
                        <%# Eval("employee_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="empDropDown" runat="server" Enabled="false">
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required4" runat="server" ControlToValidate="empDropDown"
                            InitialValue="select Employee" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">

                    <ItemTemplate>
                        <asp:Button runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Button runat="server" Text="Add New" CommandName="AddNew" OnClick="AddNew" ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
    </div>
    <div>
        <%--&nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" Text="view Projects" OnClick="ProjectNavigation" />
            &nbsp;&nbsp;
            <asp:Button ID="Button2" runat="server" Text="view Departments" OnClick="DepartmentNavigation" />
            &nbsp;&nbsp;
            <asp:Button ID="Button3" runat="server" Text="view Employees" OnClick="EmployeeNavigation" />

            <br />--%>

        <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" Visible="false"></asp:Label>
    </div>
    </form>

</asp:Content>
