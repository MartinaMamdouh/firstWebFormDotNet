<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="departmentForm.aspx.cs" Inherits="emptyWebForm.departmentForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>Department Data</h2>
        <asp:TextBox ID="txtdep" placeholder="department" runat="server"></asp:TextBox>
        &nbsp;
         
        <asp:DropDownList ID="DropDownSector" runat="server" DataSource='<%# GetAllSectors() %>'>
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnFilter" runat="server" ForeColor="#009933" OnClick="btn_filter" Text="Filter" />

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="department_id" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting" ShowFooter="True">
            <Columns>
                <asp:TemplateField HeaderText="Department Name" SortExpression="department_name">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtDepName" runat="server" Text='<%# Bind("department_name") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="this field is required" ControlToValidate="txtDepName" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("department_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtnameFooter" runat="server"></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="this field is required" ControlToValidate="txtnameFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sector Name" SortExpression="sector_name">
                    <EditItemTemplate>
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSource='<%# GetAllSectors() %>' Text='<%# Bind("sector_name") %>'> </asp:DropDownList>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvDropDownList1" runat="server" ControlToValidate="DropDownList1"
                                InitialValue="Select Sector" ForeColor="red" ErrorMessage="* Sector is required." ValidationGroup="EditValidation" />
                       
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("sector_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="DropDownList2" runat="server" DataSource='<%# GetAllSectors() %>' Text='<%# Bind("sector_name") %>'>
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="rfvDropDownList2" runat="server" ControlToValidate="DropDownList2"
                            InitialValue="Select Sector" ForeColor="red" ErrorMessage="* Sector is required." ValidationGroup="FooterValidation" />
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
        <br />
        <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" Visible="false"></asp:Label>
    </div>
    <%--   &nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="SectorNavigation" Text="view Sectors" />
        &nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" OnClick="EmployeeNavigation" Text="view employees" />
    &nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" OnClick="ProjectNavigation" Text="view projects" />--%>
</asp:Content>



