using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.DynamicData;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;

namespace emptyWebForm
{
    public partial class EmployeeWebForm : System.Web.UI.Page
    {
        string connectionString = "Data Source=CDWIN-HICT8010\\SQLEXPRESS;Initial Catalog=firstDB;Integrated Security=True;TrustServerCertificate=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
                //DropDownList3.DataSource = GetAllDepartments();
                //DropDownList3.DataBind();
                DropDownList5.DataSource = GetAllSectors();
                DropDownList5.DataBind();

            }
        }
        private void BindGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT
                    E.employee_id,
                    E.employee_name,
                    E.date_of_birth,
                    E.salary,
                    E.termination_date,
                    D.department_name,
                    D.department_id,
                    S.sector_name 
                FROM
                    Employee E
                JOIN
                    Department D ON E.department_id = D.department_id
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
            DropDownList DropDownList2 = GridView1.FooterRow.FindControl("DropDownList2") as DropDownList;
            DropDownList2.Items.Insert(0, "select Department");
            DropDownList3.Items.Insert(0, "select Department");
            ErrorMessageLabel.Visible = false;
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;           
            BindGridView();
            DropDownList DropDownList1 = GridView1.Rows[GridView1.EditIndex].FindControl("DropDownList1") as DropDownList;

            DropDownList SectorDropDownEdit = GridView1.Rows[GridView1.EditIndex].FindControl("SectorDropDownEdit") as DropDownList;
            string currentDepartmentName = ((Label)GridView1.Rows[e.NewEditIndex].FindControl("lblDepartmentName")).Text;

            string selectedSectorName = SectorDropDownEdit.SelectedValue;
            List<string> departmentForSector = GetAllDepartmentBySectors(selectedSectorName);
            DropDownList1.DataSource = departmentForSector;
            DropDownList1.DataBind();
            DropDownList1.SelectedValue = currentDepartmentName;
            DropDownList1.Items.Insert(0, "Select Department");

        }
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["employee_id"]);
            TextBox txtEmployeeName = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtEmployeeName");
            TextBox txtEmployeeDOB = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtEmployeeDOB");
            TextBox txtEmployeeSalary = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtEmployeeSalary");
            TextBox txtEmployeeEndDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtEmployeeEndDate");
            DropDownList DropDownList1 = GridView1.Rows[GridView1.EditIndex].FindControl("DropDownList1") as DropDownList;
            string selectedDepartmentName = DropDownList1.SelectedValue;
            int departmentId = GetDepartmentIdByName(selectedDepartmentName);

            int projectId = GetProjectByEmp(employeeId);
            System.Diagnostics.Debug.WriteLine("projectId"+ projectId);
            int numMonths = GetNumMonths(projectId);
            System.Diagnostics.Debug.WriteLine("numMonths"+numMonths);
            decimal sumSalary = GetSumSalary(projectId, employeeId) + Convert.ToDecimal(txtEmployeeSalary.Text);
            System.Diagnostics.Debug.WriteLine("sumSalary"+ sumSalary);

            if (IsAvailableFund(numMonths, sumSalary, projectId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string query = "UPDATE Employee SET employee_name = @EmployeeName, date_of_birth=@EmployeeDOB, salary=@EmployeeSalary, termination_date=@EmployeeEndDate, department_id=@DepartmentId WHERE employee_id =@employeeId ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId);
                        command.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text);
                        command.Parameters.AddWithValue("@EmployeeDOB", txtEmployeeDOB.Text);
                        command.Parameters.AddWithValue("@EmployeeSalary", txtEmployeeSalary.Text);
                        //command.Parameters.AddWithValue("@EmployeeEndDate", txtEmployeeEndDate.Text);
                        command.Parameters.AddWithValue("@EmployeeEndDate", string.IsNullOrEmpty(txtEmployeeEndDate.Text) ? DBNull.Value : (object)txtEmployeeEndDate.Text);
                        command.Parameters.AddWithValue("@DepartmentId", departmentId);

                        //command.Parameters.AddWithValue("@EmployeeEndDate", Calendar1.SelectedDate.ToShortDateString());
                        //command.Parameters.AddWithValue("@EmployeeDepId", txtEmployeeDepId.Text);
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
        protected int GetProjectByEmp(int employeeId)
        {
            int projectId=0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "select project_id from projDepEmp A join employee E ON E.employee_id=A.employee_id where E.employee_id=@employeeId ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        projectId = Convert.ToInt32(result);
                    }
                }
            }
            return projectId;
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

        protected decimal GetSumSalary(int projectId, int excludedEmployeeId)
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
                                WHERE p.project_id = @projectId AND e.employee_id != @excludedEmployeeId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@excludedEmployeeId", excludedEmployeeId);
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        sumOfSalaries = Convert.ToDecimal(result);
                        System.Diagnostics.Debug.WriteLine("sumOfSalaries"+sumOfSalaries);
                    }

                }
            }
            return sumOfSalaries;
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["employee_id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from Employee where employee_id = @EmployeeId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void AddNew(object sender, EventArgs e)
        {
            TextBox txtNameFooter = GridView1.FooterRow.FindControl("txtNameFooter") as TextBox;
            TextBox txtDOBFooter = GridView1.FooterRow.FindControl("txtDOBFooter") as TextBox;
            TextBox txtSalaryFooter = GridView1.FooterRow.FindControl("txtSalaryFooter") as TextBox;
            TextBox txtEndDateFooter = GridView1.FooterRow.FindControl("txtEndDateFooter") as TextBox;
            DropDownList DropDownList2 = GridView1.FooterRow.FindControl("DropDownList2") as DropDownList;
            string selectedDepartmentName = DropDownList2.SelectedValue;
            int departmentId = GetDepartmentIdByName(selectedDepartmentName);

            SqlConnection conn = new SqlConnection(connectionString);
            string query = "INSERT INTO Employee ([employee_name],[date_of_birth], [salary], [termination_date],[department_id]) VALUES (@EmployeeName,@EmployeeDOB, @EmployeeSalary, @EmployeeEndDate,@DepartmentId) ";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@EmployeeName", txtNameFooter.Text);
            command.Parameters.AddWithValue("@EmployeeDOB", txtDOBFooter.Text);
            command.Parameters.AddWithValue("@EmployeeSalary", txtSalaryFooter.Text);
            //command.Parameters.AddWithValue("@EmployeeEndDate", txtEndDateFooter.Text);
            command.Parameters.AddWithValue("@EmployeeEndDate", string.IsNullOrEmpty(txtEndDateFooter.Text) ? DBNull.Value : (object)txtEndDateFooter.Text);

            command.Parameters.AddWithValue("@DepartmentId", departmentId);

            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();

            //GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        public int GetDepartmentIdByName(string departmentName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT department_id FROM Department WHERE department_name = @DepartmentName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentName", departmentName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if department name is not found
        }
        public List<string> GetAllDepartments()
        {
            List<string> departments = new List<string>();
            //departments.Add("Select Department");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT department_id, department_name FROM Department";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int depId = Convert.ToInt32(reader["department_id"]);
                            string departmentName = reader["department_name"].ToString();
                            departments.Add(departmentName);
                        }
                    }
                }
            }
            departments.Insert(0, "Select Department");

            return departments;
        }

        public List<string> GetAllDepartmentBySectors(string sectorName)
        {
            List<string> departments = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct the query to select sectors based on the department name
                string query = @"SELECT d.department_name
                         FROM Department AS d 
                         JOIN Sector AS s ON s.sector_id = d.sector_id
                         WHERE s.sector_name = @SectorName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parameterize the query to protect against SQL injection
                    command.Parameters.AddWithValue("@SectorName", sectorName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string departmentName = reader["department_name"].ToString();
                            departments.Add(departmentName);
                        }
                    }
                }
            }

            return departments;
        }
        public List<string> GetAllSectors()
        {
            List<string> sectors = new List<string>();
            //departments.Add("Select Department");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT s.sector_name FROM Sector AS s";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int depId = Convert.ToInt32(reader["department_id"]);
                            string sectorName = reader["sector_name"].ToString();
                            sectors.Add(sectorName);
                        }
                    }
                }
            }
            sectors.Insert(0, "Select Sector");
            return sectors;
        }
        protected void SectorDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList SectorDropDown = GridView1.FooterRow.FindControl("SectorDropDown") as DropDownList;
            string selectedSectorName = SectorDropDown.SelectedValue;
            List<string> departmentForSector = GetAllDepartmentBySectors(selectedSectorName);
            //foreach (var sector in sectorsForDepartment)
            //{
            //    System.Diagnostics.Debug.WriteLine(sector);
            //}
            DropDownList DropDownList2 = GridView1.FooterRow.FindControl("DropDownList2") as DropDownList;
            DropDownList2.Enabled = true;
            DropDownList2.DataSource = departmentForSector;
            DropDownList2.DataBind();
            DropDownList2.Items.Insert(0, "select Department");
        }
        protected void SectorDropDownEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList SectorDropDownEdit = GridView1.Rows[GridView1.EditIndex].FindControl("SectorDropDownEdit") as DropDownList;
            string selectedSectorName = SectorDropDownEdit.SelectedValue;
            List<string> departmentForSector = GetAllDepartmentBySectors(selectedSectorName);
            //foreach (var sector in departmentForSector)
            //{
            //    System.Diagnostics.Debug.WriteLine(sector);
            //}
            DropDownList DropDownList1 = GridView1.Rows[GridView1.EditIndex].FindControl("DropDownList1") as DropDownList;
            DropDownList1.DataSource = departmentForSector;
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, "Select Department");

        }


        protected void DropDownList5_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSectorName = DropDownList5.SelectedValue;
            List<string> departmentForSector = GetAllDepartmentBySectors(selectedSectorName);

            DropDownList3.DataSource = departmentForSector;
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, "select Department");


        }
        protected void btn_filter(object sender, EventArgs e)
        {
            //Response.Redirect("SearchForm.aspx");
            string empName = txtEmpName.Text;
            string empSalaryFrom = txtSalaryFrom.Text;
            string empSalaryTo = txtSalaryTo.Text;
            string selectedSectorName = DropDownList5.SelectedValue;
            string selectedDepartmentName = DropDownList3.SelectedValue;
            int departmentId = GetDepartmentIdByName(selectedDepartmentName);

            string query = "select * from Employee E JOIN Department D ON E.department_id = D.department_id JOIN Sector S ON D.sector_id=S.sector_id";
            string queryCondition = "";
            if (!string.IsNullOrEmpty(empName))
            {
                queryCondition += $" AND E.employee_name LIKE '%{empName}%' ";
            }
            if (!string.IsNullOrEmpty(empSalaryFrom))
            {
                queryCondition += $" AND E.salary >= {empSalaryFrom} ";
            }
            if (!string.IsNullOrEmpty(empSalaryTo))
            {
                queryCondition += $" AND E.salary <= {empSalaryTo} ";
            }
            if (CheckBox1.Checked)
            {
                queryCondition += "AND termination_date < GETDATE() ";
            }
            if (selectedDepartmentName != "select Department" && departmentId != 0)
            {
                queryCondition += $" AND D.department_id= {departmentId} ";
            }
            if (selectedSectorName != "Select Sector")
            {
                queryCondition += $" AND S.sector_name LIKE '%{selectedSectorName}%' ";
            }
            if (!string.IsNullOrEmpty(queryCondition))
            {
                query += " WHERE 1=1 " + queryCondition;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@txtEmpName", txtEmpName.Text);
                    //command.Parameters.AddWithValue("@txtsalary", txtSalary.Text);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                    DropDownList DropDownList2 = GridView1.FooterRow.FindControl("DropDownList2") as DropDownList;
                    DropDownList2.Items.Insert(0, "select Department");
                }

            }
        }
        protected void btn_navigation(object sender, EventArgs e)
        {
            Response.Redirect("departmentForm.aspx");
        }
        protected void ProjectNavigation(object sender, EventArgs e)
        {
            Response.Redirect("ProjectForm.aspx");
        }
    }
}