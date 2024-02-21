using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.NetworkInformation;

namespace emptyWebForm
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        string connectionString = "Data Source=CDWIN-HICT8010\\SQLEXPRESS;Initial Catalog=firstDB;Integrated Security=True;TrustServerCertificate=True";


        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    //string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(connectionString);
        //    string query = "INSERT INTO [dbo].[Sector] ([sector_id],[sector_name]) VALUES  (@SectorId, @SectorName)";
        //    SqlCommand cmd = new SqlCommand(query, conn);
            
        //        cmd.Parameters.AddWithValue("@SectorId", txtID.Text);
        //        cmd.Parameters.AddWithValue("@SectorName", txtName.Text);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        Response.Write("data inseted successfully");          
        //        conn.Close();

        //}

        protected void btnView_Click(object sender, EventArgs e)
        {
            BindGridView();
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            
                ModifyEmployee();
                BindGridView();
            
        }
        private void BindGridView()
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //connection.Open();
                string query = "SELECT * FROM Employee";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                GridView1.DataSource = dataTable;
                GridView1.DataBind();
            }
        }

        private void ModifyEmployee()
        {
        using (SqlConnection connection = new SqlConnection(connectionString))
            {
            string query = "UPDATE Employee SET employee_name = @EmployeeName,age=@EmployeeAge,salary=@EmployeeSalary,termination_date=@EmployeeEndDate,department_id=@EmployeeDepId WHERE employee_id = @EmployeeId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", txtEmployeeId.Text);
                    command.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text);
                    command.Parameters.AddWithValue("@EmployeeAge", txtEmployeeAge.Text);
                    command.Parameters.AddWithValue("@EmployeeSalary", txtEmployeeSalary.Text);
                    command.Parameters.AddWithValue("@EmployeeEndDate", Calendar1.SelectedDate.ToShortDateString());
                    command.Parameters.AddWithValue("@EmployeeDepId", txtEmployeeDepId.Text);
                    connection.Open();  
                    command.ExecuteNonQuery();
                }
                   
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteEmployee();
            BindGridView();
        }
        private void DeleteEmployee()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from Employee where employee_id = @EmployeeId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", txtEmployeeId.Text);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            InsertEmployee();
            BindGridView();
        }
        private void InsertEmployee()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "INSERT INTO [dbo].[Employee] ([employee_name],[age],[salary],[termination_date],[department_id])VALUES (@EmployeeName,@EmployeeAge,@EmployeeSalary,@EmployeeEndDate,@EmployeeDepId) ";
            SqlCommand command = new SqlCommand(query, conn);

            //command.Parameters.AddWithValue("@EmployeeId", txtEmployeeId.Text);
            command.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text);
            command.Parameters.AddWithValue("@EmployeeAge", txtEmployeeAge.Text);
            command.Parameters.AddWithValue("@EmployeeSalary", txtEmployeeSalary.Text);
            command.Parameters.AddWithValue("@EmployeeEndDate", Calendar1.SelectedDate);
            command.Parameters.AddWithValue("@EmployeeDepId", txtEmployeeDepId.Text);
            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();
        } 
    }
}