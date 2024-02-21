using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace emptyWebForm
{
    public partial class departmentForm : System.Web.UI.Page
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
            DropDownSector.DataSource = GetAllSectors();
            DropDownSector.DataBind();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT                    
                    D.department_id,
                    D.department_name,
                    D.sector_id,
                    S.sector_name 
                FROM
                    
                    Department D 
                JOIN
                    Sector S ON D.sector_id = S.sector_id";

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
        }

        protected void btn_filter(object sender, EventArgs e)
        {
            string dep = txtdep.Text;
            string selectedSector = DropDownSector.SelectedValue;

            string query = @" 
                SELECT              
                D.department_id, D.department_name, 
                S.sector_id, S.sector_name
                FROM 
                    Department as D 
                JOIN 
                    Sector as S ON D.sector_id=S.sector_id";

            string queryCondition = "";
          
            if (!string.IsNullOrEmpty(dep))
            {
                queryCondition += $" AND D.department_name LIKE '%{dep}%' ";
            }
            if (selectedSector != "Select Sector")
            {
                queryCondition += $" AND sector_name LIKE '%{selectedSector}%' ";
            }
            if (!string.IsNullOrEmpty(queryCondition))
            {
                query += " WHERE 1=1 " + queryCondition;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                }

            }
      
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridView();
        }
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int departmentId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["department_id"]);
            TextBox txtDepName = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtDepName");
            DropDownList DropDownList1 = GridView1.Rows[GridView1.EditIndex].FindControl("DropDownList1") as DropDownList;
            string selectedSectorName = DropDownList1.SelectedValue;
            int sectorId = GetSectorIdByName(selectedSectorName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Department SET department_name = @DepartmentName, sector_id=@sectorId WHERE department_id =@departmentId ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@departmentId", departmentId);
                    command.Parameters.AddWithValue("@DepartmentName", txtDepName.Text);
                    command.Parameters.AddWithValue("@sectorId", sectorId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int departmentId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["department_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from Department where department_id = @departmentId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@departmentId", departmentId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        public List<string> GetAllSectors()
        {
            List<string> sectors = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT sector_id, sector_name FROM Sector";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sectorName = reader["sector_name"].ToString();
                            sectors.Add(sectorName);
                        }
                    }
                }
            }
            sectors.Insert(0, "Select Sector");
            return sectors;
        }

        public int GetSectorIdByName(string sectorName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT sector_id FROM Sector WHERE sector_name = @sectorName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sectorName", sectorName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if department name is not found
        }

        protected void AddNew(object sender, EventArgs e)
        {
            TextBox txtNameFooter = GridView1.FooterRow.FindControl("txtNameFooter") as TextBox;
            DropDownList DropDownList2 = GridView1.FooterRow.FindControl("DropDownList2") as DropDownList;
            string selectedSectorName = DropDownList2.SelectedValue;
            int sectorId = GetSectorIdByName(selectedSectorName);
            SqlConnection conn = new SqlConnection(connectionString);
            string checkQuery = "SELECT COUNT(*) FROM Department WHERE department_name = @DepartmentName AND sector_id = @SectorId";

                

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, conn))
                {
                checkCommand.Parameters.AddWithValue("@DepartmentName", txtNameFooter.Text);
                checkCommand.Parameters.AddWithValue("@SectorId", sectorId);
                conn.Open();
                int count = (int)checkCommand.ExecuteScalar();
                conn.Close();
                if (count > 0)
                    {
                        // The combination is not unique, display an error message
                        ErrorMessageLabel.Text = "The record already exist.";
                        ErrorMessageLabel.Visible = true;
                        return; // Exit the method without inserting the record
                    }
                }

                string query = "INSERT INTO Department ([department_name],[sector_id]) VALUES (@DepartmentName,@SectorId) ";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@DepartmentName", txtNameFooter.Text);
                command.Parameters.AddWithValue("@SectorId", sectorId);

                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            ErrorMessageLabel.Visible = false;
            //GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void EmployeeNavigation(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeWebForm.aspx");
        }
        protected void SectorNavigation(object sender, EventArgs e)
        {
            Response.Redirect("SectorForm.aspx");
        }
        protected void ProjectNavigation(object sender, EventArgs e)
        {
            Response.Redirect("ProjectForm.aspx");
        }
    }
}