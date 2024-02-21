<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="SectorForm.aspx.cs" Inherits="emptyWebForm.SectorForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>Sectors Data</h2>
        <asp:DropDownList ID="DropDownSector" runat="server" DataSource='<%# GetAllSectors() %>'>
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnFilter" runat="server" ForeColor="#009933" OnClick="btn_filter" Text="Filter" />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="sector_id" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting" ShowFooter="True">
            <Columns>
                <asp:TemplateField HeaderText="Sector Name" SortExpression="sector_name">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtSecName" runat="server" Text='<%# Bind("sector_name") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="this field is required" ControlToValidate="txtSecName" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("sector_name") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtnameFooter" runat="server"></asp:TextBox>
                        <asp:Label ID="lbRequired" runat="server" ForeColor="red" Text="*"></asp:Label>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="this field is required" ControlToValidate="txtnameFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
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
    <%-- <div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="DepartmentNavigation" Text="view Departments" />
        </div>--%>
</asp:Content>
