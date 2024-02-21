<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="EmployeeWebForm.aspx.cs" Inherits="emptyWebForm.EmployeeWebForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>Employee Data</h2>

        <%-- <asp:DropDownList ID="SectorDropDown" runat="server" DataSource='<%# GetAllSectors() %>' AutoPostBack="true" OnSelectedIndexChanged="SectorDropDown_SelectedIndexChanged" Text='<%# Bind("sector_name") %>'>
        </asp:DropDownList>
        <asp:DropDownList ID="DropDownList2" runat="server">
        </asp:DropDownList>
        --%>
        <p>
            <asp:TextBox ID="txtEmpName" placeholder="name" runat="server"></asp:TextBox>
            &nbsp;
                <asp:TextBox ID="txtSalaryFrom" placeholder="salary from" runat="server" ></asp:TextBox>
           <%-- <br />--%>
            &nbsp;
                 <asp:TextBox ID="txtSalaryTo" placeholder="salary to" runat="server"></asp:TextBox>
            &nbsp;
                <asp:CheckBox ID="CheckBox1" runat="server" Text="is terminated" />
            &nbsp;
                 <asp:DropDownList ID="DropDownList5" runat="server" DataSource='<%# GetAllSectors() %>' AutoPostBack="true" OnSelectedIndexChanged="DropDownList5_SelectedIndexChanged">
                 </asp:DropDownList>
            &nbsp;
                <asp:DropDownList ID="DropDownList3" runat="server">
                </asp:DropDownList>

            <br />
            <asp:Button ID="btnFilter" runat="server" ForeColor="#009933" OnClick="btn_filter" Text="Filter" ValidationGroup="filter" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtSalaryFrom" ForeColor="Red"
    ErrorMessage="Only Numbers allowed" ValidationExpression="\d+" ValidationGroup="filter">
</asp:RegularExpressionValidator>
        </p>
        <p>
            &nbsp;</p>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="employee_id" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting" ShowFooter="True">
            <Columns>
                <asp:TemplateField HeaderText="Employee Name" SortExpression="employee_name">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmployeeName" runat="server" Text='<%# Bind("employee_name") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="this field is required" ControlToValidate="txtEmployeeName" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("employee_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtNameFooter" runat="server"></asp:TextBox>
                        <asp:Label ID="lbRequired1" runat="server" ForeColor="red" Text="*"></asp:Label>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="this field is required" ControlToValidate="txtnameFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Date of Birth" SortExpression="date_of_birth">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmployeeDOB" runat="server" Text='<%# Bind("date_of_birth", "{0:d}") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="this field is required" ControlToValidate="txtEmployeeDOB" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("date_of_birth", "{0:d}") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtDOBFooter" runat="server"></asp:TextBox>
                        <asp:Label ID="lbRequired2" runat="server" ForeColor="red" Text="*"></asp:Label>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="this field is required" ControlToValidate="txtDOBFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Salary" SortExpression="salary">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmployeeSalary" runat="server" Text='<%# Bind("salary") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="this field is required" ControlToValidate="txtEmployeeSalary" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("salary") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtSalaryFooter" runat="server"></asp:TextBox>
                        <asp:Label ID="lbRequired3" runat="server" ForeColor="red" Text="*"></asp:Label>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="this field is required" ControlToValidate="txtSalaryFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Termination date" SortExpression="termination_date">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmployeeEndDate" runat="server" Text='<%# Bind("termination_date", "{0:d}") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("termination_date", "{0:d}") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtEndDateFooter" runat="server"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sector Name" SortExpression="sector_name">
                    <EditItemTemplate>
                        <asp:DropDownList ID="SectorDropDownEdit" runat="server" DataSource='<%# GetAllSectors() %>' AutoPostBack="true" OnSelectedIndexChanged="SectorDropDownEdit_SelectedIndexChanged" Text='<%# Bind("sector_name") %>'>
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required" runat="server" ControlToValidate="SectorDropDownEdit"
                            InitialValue="Select Sector" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="EditValidation" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("sector_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="SectorDropDown" runat="server" DataSource='<%# GetAllSectors() %>' AutoPostBack="true" OnSelectedIndexChanged="SectorDropDown_SelectedIndexChanged" Text='<%# Bind("sector_name") %>'>
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required" runat="server" ControlToValidate="SectorDropDown"
                            InitialValue="Select Sector" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Department Name" SortExpression="department_name">
                    <EditItemTemplate>
                       
                        <asp:Label ID="lblDepartmentName" runat="server" Text='<%# Eval("department_name") %>' Visible="false"></asp:Label>


                        <asp:DropDownList ID="DropDownList1" runat="server" >
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="required2" runat="server" ControlToValidate="DropDownList1"
                            InitialValue="Select Department" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="EditValidation" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("department_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="DropDownList2" runat="server">
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="rfvDropDownList2" runat="server" ControlToValidate="DropDownList2"
                            InitialValue="select Department" ForeColor="red" ErrorMessage="* this field is required." ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <EditItemTemplate>
                        <asp:Button runat="server" Text="Update" CommandName="Update" ValidationGroup="EditValidation" />
                        <asp:Button runat="server" Text="Cancel" CommandName="Cancel" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Button runat="server" Text="Edit" CommandName="Edit" />
                        <asp:Button runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Button runat="server" Text="Add New" CommandName="AddNew" OnClick="AddNew" ValidationGroup="FooterValidation" />
                    </FooterTemplate>
                </asp:TemplateField>


            </Columns>
        </asp:GridView>
        <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" Visible="false"></asp:Label>

    </div>
    <%-- <p>
            <asp:Button ID="Button1" runat="server" Text="view all departments " OnClick="btn_navigation" />
            &nbsp;&nbsp;
     <asp:Button ID="Button3" runat="server" OnClick="ProjectNavigation" Text="view projects" />
        </p>--%>
</asp:Content>
