
<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="ProjectForm.aspx.cs" Inherits="emptyWebForm.ProjectForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <div>
            <h2>Projects Data</h2>
            <p>
                <asp:TextBox ID="txtprjName" placeholder="project name" runat="server"></asp:TextBox>
                &nbsp;
                <asp:TextBox ID="txtstartRange" placeholder="From Date" runat="server"></asp:TextBox>
                &nbsp;
                <asp:TextBox ID="txtendRange" placeholder="To Date" runat="server"></asp:TextBox>
                &nbsp;
                <asp:TextBox ID="txtfundFrom" placeholder="Funds from" runat="server"></asp:TextBox>
                &nbsp;
                <asp:TextBox ID="txtfundTo" placeholder="Funds to" runat="server"></asp:TextBox>
                 <br />
                <asp:CustomValidator ID="cvEndDate" runat="server" ControlToValidate="txtendRange" OnServerValidate="ValidateEndDateFilter" ForeColor="red" ValidationGroup="Validation" ErrorMessage="End date must be after start date." ValidateEmptyText="true"></asp:CustomValidator>
                <br />
                <asp:Button ID="btnFilter" runat="server" ForeColor="#009933" OnClick="btn_filter" Text="Filter" ValidationGroup="Validation"/>

            </p>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="project_id" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting" ShowFooter="True" style="direction: ltr" AllowPaging="true" PageSize="3">
                <Columns>
                    <asp:TemplateField HeaderText="Project Name" SortExpression="project_name">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPrjName" runat="server" Text='<%# Bind("project_name") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="this field is required" ControlToValidate="txtPrjName" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("project_name") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtnameFooter" runat="server"></asp:TextBox>
                            <asp:Label ID="lbRequired1" runat="server" ForeColor="red" Text="*"></asp:Label>
                            <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="this field is required" ControlToValidate="txtnameFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Start Date" SortExpression="starting_date">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPrjStartDate" runat="server" Text='<%# Bind("starting_date", "{0:d}") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="this field is required" ControlToValidate="txtPrjStartDate" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("starting_date", "{0:d}") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtStartDateFooter" runat="server"></asp:TextBox>
                            <asp:Label ID="lbRequired2" runat="server" ForeColor="red" Text="*"></asp:Label>
                            <br />

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="this field is required" ControlToValidate="txtStartDateFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                             <br />
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="End Date" SortExpression="end_date">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPrjEndDate" runat="server" Text='<%# Bind("end_date", "{0:d}") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="this field is required" ControlToValidate="txtPrjEndDate" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                             <asp:CustomValidator ID="cvEndDateEdit" runat="server" ControlToValidate="txtPrjEndDate" OnServerValidate="ValidateEndDateEdit" ForeColor="red" ValidationGroup="EditValidation" ErrorMessage="End date must be after start date." ValidateEmptyText="true"></asp:CustomValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("end_date", "{0:d}") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtEndDateFooter" runat="server"></asp:TextBox>
                            <asp:Label ID="lbRequired3" runat="server" ForeColor="red" Text="*"></asp:Label>
                            <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="this field is required" ControlToValidate="txtEndDateFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
                             <br />
                            <asp:CustomValidator ID="cvEndDate" runat="server" ControlToValidate="txtEndDateFooter" OnServerValidate="ValidateEndDate" ForeColor="red" ValidationGroup="FooterValidation" ErrorMessage="End date must be after start date." ValidateEmptyText="true"></asp:CustomValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Available Fund" SortExpression="available_funds">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPrjFund" runat="server" Text='<%# Bind("available_funds") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="this field is required" ControlToValidate="txtPrjFund" ForeColor="Red" ValidationGroup="EditValidation"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# Eval("available_funds") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtFundFooter" runat="server"></asp:TextBox>
                            <asp:Label ID="lbRequired4" runat="server" ForeColor="red" Text="*"></asp:Label>
                            <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="this field is required" ControlToValidate="txtFundFooter" ForeColor="Red" ValidationGroup="FooterValidation"></asp:RequiredFieldValidator>
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
                            <asp:Button runat="server" Text="Add New" CommandName="AddNew" OnClick="AddNew" ValidationGroup="FooterValidation"  CausesValidation="true"/>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
             <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            <br />
           
        </div>
        <div>
            <asp:TextBox ID="TextBox1" runat="server" type="date"></asp:TextBox>
            <asp:Button ID="excelBtn" runat="server" Text="export to excel" OnClick="excelBtn_Click" />
<%--            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="DepartmentNavigation" Text="view Departments" />
            &nbsp;&nbsp;
     <asp:Button ID="Button3" runat="server" OnClick="AllProjectNavigation" Text="view report" />
            --%>
            
        </div>
</asp:Content>
