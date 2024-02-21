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
    public partial class SectorForm : System.Web.UI.Page
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
                string query = "SELECT sector_id, sector_name FROM Sector " ;

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
        protected void btn_filter(object sender, EventArgs e)
        {
            string selectedSector = DropDownSector.SelectedValue;
            //System.Diagnostics.Debug.WriteLine("selectedSector"+selectedSector);
            string query = @" 
                SELECT 
                sector_id, sector_name
                FROM 
                    Sector ";

            string queryCondition = "";
           
            if(selectedSector!= "Select Sector")
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

        public List<string> GetAllSectors()
        {
            List<string> sectors = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT sector_name FROM Sector";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sectorName = reader["sector_name"].ToString();
                            //System.Diagnostics.Debug.WriteLine("sectorName"+sectorName);
                            sectors.Add(sectorName);
                        }
                    }
                }
            }
            sectors.Insert(0, "Select Sector");

            return sectors;
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
            int sectorId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["sector_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from Sector where sector_id = @sectorId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sectorId", sectorId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int sectorId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["sector_id"]);
            TextBox txtSecName = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtSecName");
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM Sector WHERE sector_name = @SectorName AND sector_id != @sectorId";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@SectorName", txtSecName.Text);
                    checkCommand.Parameters.AddWithValue("@sectorId", sectorId);

                    connection.Open();
                    int existingRecordsCount = (int)checkCommand.ExecuteScalar();
                    connection.Close();
                    if (existingRecordsCount > 0)
                    {
                        // Sector name already exists, handle the duplicate case
                        ErrorMessageLabel.Text = "Sector name already exists.";
                        ErrorMessageLabel.Visible = true;
                        return; // Stop further execution
                    }
                }

                string query = "UPDATE Sector SET sector_name = @SectorName WHERE sector_id =@sectorId ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sectorId", sectorId);
                    command.Parameters.AddWithValue("@SectorName", txtSecName.Text);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void AddNew(object sender, EventArgs e)
        {
            TextBox txtNameFooter = GridView1.FooterRow.FindControl("txtNameFooter") as TextBox;

            SqlConnection conn = new SqlConnection(connectionString);
            string checkQuery = "SELECT COUNT(*) FROM Sector WHERE sector_name = @SectorName";
            using (SqlCommand checkCommand = new SqlCommand(checkQuery, conn))
            {
                checkCommand.Parameters.AddWithValue("@SectorName", txtNameFooter.Text);

                conn.Open();
                int existingRecordsCount = (int)checkCommand.ExecuteScalar();
                conn.Close();
                if (existingRecordsCount > 0)
                {
                    // Sector name already exists, handle the duplicate case
                    ErrorMessageLabel.Text = "Sector name already exists.";
                    ErrorMessageLabel.Visible = true;
                    return; // Stop further execution
                }
            }
            string query = "INSERT INTO Sector ([sector_name]) VALUES (@SectorName) ";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@SectorName", txtNameFooter.Text);

            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();

            //GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void DepartmentNavigation(object sender, EventArgs e)
        {
            Response.Redirect("departmentForm.aspx");
        }

      
    }
}