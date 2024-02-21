using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace emptyWebForm
{
    public partial class ProjectForm : System.Web.UI.Page
    {
        string connectionString = "Data Source=CDWIN-HICT8010\\SQLEXPRESS;Initial Catalog=firstDB;Integrated Security=True;TrustServerCertificate=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
            }
        }

        private void BindGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT project_id, project_name, starting_date, end_date, available_funds FROM Project ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                }
            }
            ErrorMessageLabel.Visible = false;


        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridView();
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int projectId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["project_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from Project where project_id = @projectId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Page.IsValid)
            {

                int projectId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["project_id"]);
                TextBox txtPrjName = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrjName");
                TextBox txtPrjStartDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrjStartDate");
                TextBox txtPrjEndDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrjEndDate");
                TextBox txtPrjFund = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrjFund");
                int numMonths = GetNumMonths(projectId);
                decimal sumSalary = GetSumSalary(projectId);
                if (Convert.ToDecimal(txtPrjFund.Text) >= numMonths* sumSalary)
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Project SET project_name = @projectName, starting_date=@startDate, end_date=@endDate, available_funds=@funds WHERE project_id =@projectId ";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@projectId", projectId);
                            command.Parameters.AddWithValue("@projectName", txtPrjName.Text);
                            command.Parameters.AddWithValue("@startDate", txtPrjStartDate.Text);
                            command.Parameters.AddWithValue("@endDate", txtPrjEndDate.Text);
                            command.Parameters.AddWithValue("@funds", txtPrjFund.Text);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    GridView1.EditIndex = -1; //to exit the edit mood
                    BindGridView();
                    ErrorMessageLabel.Visible = false;
                }
                else
                {
                    ErrorMessageLabel.Text = "Insufficient funds for this project.";
                    ErrorMessageLabel.Visible = true;
                }

            }
        }

        protected bool IsAvailableFund(int numMonths, decimal sumSalary, int projectId)
        {
            bool isAvailable = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "select available_funds From project where project_id=@projectId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        decimal availableFunds = Convert.ToDecimal(result);
                        decimal requiredFunds = numMonths * sumSalary;
                        System.Diagnostics.Debug.WriteLine("availableFunds" + availableFunds);
                        System.Diagnostics.Debug.WriteLine("requiredFunds" + requiredFunds);
                        isAvailable = availableFunds >= requiredFunds;
                        System.Diagnostics.Debug.WriteLine(isAvailable);
                    }
                }
            }
            return isAvailable;
        }

        public int GetNumMonths(int projectId)
        {
            int numMonths = 0;
            DateTime startDate;
            DateTime endDate;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "select starting_date, end_date From project where project_id=@projectId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            startDate = Convert.ToDateTime(reader["starting_date"]);
                            //System.Diagnostics.Debug.WriteLine(startDate.Month);
                            endDate = Convert.ToDateTime(reader["end_date"]);
                            //System.Diagnostics.Debug.WriteLine(endDate.Month);
                            numMonths = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month);
                            //System.Diagnostics.Debug.WriteLine(numMonths);
                        }
                    }

                }
            }
            //System.Diagnostics.Debug.WriteLine(numMonths);
            return numMonths;

        }

        protected decimal GetSumSalary(int projectId)
        {
            decimal sumOfSalaries = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                                SELECT SUM(e.salary) 
                                FROM Employee AS e 
                                JOIN ProjDepEmp AS a ON e.employee_id = a.employee_id 
                                JOIN Project AS p ON a.project_id = p.project_id 
                                WHERE p.project_id = @projectId ;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        sumOfSalaries = Convert.ToDecimal(result);
                        System.Diagnostics.Debug.WriteLine(sumOfSalaries);
                    }

                }
            }
            return sumOfSalaries;
        }

        protected void AddNew(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                TextBox txtNameFooter = GridView1.FooterRow.FindControl("txtNameFooter") as TextBox;
                TextBox txtStartDateFooter = GridView1.FooterRow.FindControl("txtStartDateFooter") as TextBox;
                TextBox txtEndDateFooter = GridView1.FooterRow.FindControl("txtEndDateFooter") as TextBox;
                TextBox txtFundFooter = GridView1.FooterRow.FindControl("txtFundFooter") as TextBox;

                SqlConnection conn = new SqlConnection(connectionString);
                string query = "INSERT INTO Project ([project_name],[starting_date],[end_date],[available_funds]) VALUES (@ProjectName, @startDate, @endDate, @funds) ";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@ProjectName", txtNameFooter.Text);
                command.Parameters.AddWithValue("@startDate", txtStartDateFooter.Text);
                command.Parameters.AddWithValue("@endDate", txtEndDateFooter.Text);
                command.Parameters.AddWithValue("@funds", txtFundFooter.Text);

                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();

                //GridView1.EditIndex = -1; //to exit the edit mood
                BindGridView();
            }

        }
        protected void btn_filter(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {

                //Response.Redirect("SearchForm.aspx");
                string prjName = txtprjName.Text;
                DateTime startDate;
                DateTime endDate;
                string fundFrom = txtfundFrom.Text;
                string fundTo = txtfundTo.Text;
                string query = "select * from Project ";
                string queryCondition = "";
                if (!string.IsNullOrEmpty(prjName))
                {
                    queryCondition += $" AND project_name LIKE '%{prjName}%' ";
                }
                if (DateTime.TryParse(txtstartRange.Text, out startDate))
                {
                    // Construct the SQL query to check if the date falls within the specified range
                    queryCondition += $" AND starting_date >= @StartDate ";
                }
                if (DateTime.TryParse(txtendRange.Text, out endDate))
                {
                    queryCondition += $" AND starting_date <= @EndDate";

                }
                if (!string.IsNullOrEmpty(fundFrom))
                {
                    queryCondition += $" AND available_funds >= {fundFrom} ";
                }
                if (!string.IsNullOrEmpty(fundTo))
                {
                    queryCondition += $" AND available_funds <= {fundTo} ";
                }
                if (!string.IsNullOrEmpty(queryCondition))
                {
                    query += " WHERE 1=1 " + queryCondition;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (DateTime.TryParse(txtstartRange.Text, out startDate))
                        {
                            command.Parameters.AddWithValue("@StartDate", startDate);
                        }
                        if (DateTime.TryParse(txtendRange.Text, out endDate))
                        {
                            command.Parameters.AddWithValue("@EndDate", endDate);
                        }
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        GridView1.DataSource = dataTable;
                        GridView1.DataBind();
                    }

                }
            }
        }


        protected void ValidateEndDate(object source, ServerValidateEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("end: ");

            DateTime startDate;
            DateTime endDate;
            TextBox txtStartDateFooter = GridView1.FooterRow.FindControl("txtStartDateFooter") as TextBox;
            TextBox txtEndDateFooter = GridView1.FooterRow.FindControl("txtEndDateFooter") as TextBox;
            if (DateTime.TryParse(txtStartDateFooter.Text, out startDate) && DateTime.TryParse(txtEndDateFooter.Text, out endDate))
            {
                args.IsValid = startDate < endDate;
            }
            else
            {
                args.IsValid = false; // Invalid date format
            }
        }
        protected void ValidateEndDateEdit(object source, ServerValidateEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("end: ");

            DateTime startDate;
            DateTime endDate;

            TextBox txtPrjStartDate = GridView1.Rows[GridView1.EditIndex].FindControl("txtPrjStartDate") as TextBox;
            TextBox txtPrjEndDate = GridView1.Rows[GridView1.EditIndex].FindControl("txtPrjEndDate") as TextBox;
            if (DateTime.TryParse(txtPrjStartDate.Text, out startDate) && DateTime.TryParse(txtPrjEndDate.Text, out endDate))
            {
                args.IsValid = startDate < endDate;
            }
            else
            {
                args.IsValid = false; // Invalid date format
            }
        }
        protected void ValidateEndDateFilter(object source, ServerValidateEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("end: ");

            DateTime startDate;
            DateTime endDate;
            if(!string.IsNullOrWhiteSpace(txtstartRange.Text) && !string.IsNullOrWhiteSpace(txtendRange.Text))
            {
                if (DateTime.TryParse(txtstartRange.Text, out startDate) && DateTime.TryParse(txtendRange.Text, out endDate))
                {
                    args.IsValid = startDate < endDate;
                }
                else
                {
                    args.IsValid = false; // Invalid date format
                }
            }
           
        }
        protected void DepartmentNavigation(object sender, EventArgs e)
        {
            Response.Redirect("departmentForm.aspx");
        }
        protected void AllProjectNavigation(object sender, EventArgs e)
        {
            Response.Redirect("PrjDepEmpForm.aspx");
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {

        }

        protected void excelBtn_Click(object sender, EventArgs e)
        {
            // Retrieve data from the database
            DataTable dataTable;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT project_id, project_name, starting_date, end_date, available_funds FROM Project ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            }

            // Export data to Excel
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=projects.xls");
            Response.ContentType = "application/excel";            
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);
            GridView gridView = new GridView();
            gridView.DataSource = dataTable;
            gridView.DataBind();
            gridView.RenderControl(htmlTextWriter);
            Response.Write(stringWriter.ToString());
            Response.End();

        }
        public override void VerifyRenderingInServerForm(Control control)
        {
        }
    }
}