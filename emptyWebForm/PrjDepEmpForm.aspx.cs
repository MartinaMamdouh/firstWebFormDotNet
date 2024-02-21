using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.InteropServices.ComTypes;

namespace emptyWebForm
{
    public partial class PrjDepEmpForm : System.Web.UI.Page
    {

        string connectionString = "Data Source=CDWIN-HICT8010\\SQLEXPRESS;Initial Catalog=firstDB;Integrated Security=True;TrustServerCertificate=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
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
                id,
                E.employee_id, E.employee_name,                   
                D.department_id, D.department_name,
                P.project_id, P.project_name,
                S.sector_id, S.sector_name
                FROM 
                    projDepEmp 
                JOIN
                    Department as D ON projDepEmp.department_id = D.department_id 
                JOIN
                    Employee as E ON projDepEmp.employee_id = E.employee_id
                JOIN
                    project as P ON projDepEmp.project_id = P.project_id 
                JOIN 
                Sector as S ON D.sector_id=S.sector_id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                }
                DropDownList DropDownList1 = GridView1.FooterRow.FindControl("DropDownList1") as DropDownList;
                DropDownList1.Items.Insert(0, "select Project");
                DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
                depDropDown.Items.Insert(0, "select Department");
                DropDownList empDropDown = GridView1.FooterRow.FindControl("empDropDown") as DropDownList;
                empDropDown.Items.Insert(0, "select Employee");
                DropDownList3.Items.Insert(0, "select Department");
                DropDown.Items.Insert(0, "select Employee");

            }
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
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["id"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "Delete from ProjDepEmp where id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            GridView1.EditIndex = -1; //to exit the edit mood
            BindGridView();
        }
        protected void AddNew(object sender, EventArgs e)
        {
            DropDownList DropDownList1 = GridView1.FooterRow.FindControl("DropDownList1") as DropDownList;
            string selectedProjectName = DropDownList1.SelectedValue;
            int projectId = GetPojectIdByName(selectedProjectName);
            DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
            string selectedDepartmentName = depDropDown.SelectedValue;
            int departmentId = GetDepartmentIdByName(selectedDepartmentName);
            DropDownList empDropDown = GridView1.FooterRow.FindControl("empDropDown") as DropDownList;
            string selectedEmployeeName = empDropDown.SelectedValue;
            int employeeId = GetEmployeeIdByName(selectedEmployeeName);
            //Label ErrorLabel = GridView1.FooterRow.FindControl("ErrorLabel") as Label;
            int numMonths = GetNumMonths(projectId);
            System.Diagnostics.Debug.WriteLine("numMonths:: " + numMonths);
            decimal sumSalary = GetSumSalary(projectId) + GetEmployeeSalaryForProject(projectId, employeeId);
            System.Diagnostics.Debug.WriteLine("sumSalary:: " + GetSumSalary(projectId));
            System.Diagnostics.Debug.WriteLine("oneSalary:: " + GetEmployeeSalaryForProject(projectId, employeeId));
            if (IsAvailableFund(numMonths, sumSalary, projectId))
            {
                SqlConnection conn = new SqlConnection(connectionString);
                string checkQuery = "SELECT COUNT(*) FROM ProjDepEmp WHERE project_id = @projectId AND department_id = @DepartmentId AND employee_id = @employeeId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, conn);
                checkCommand.Parameters.AddWithValue("@projectId", projectId);
                checkCommand.Parameters.AddWithValue("@DepartmentId", departmentId);
                checkCommand.Parameters.AddWithValue("@employeeId", employeeId);
                conn.Open();
                int existingRecordsCount = (int)checkCommand.ExecuteScalar();
                conn.Close();
                if (existingRecordsCount > 0)
                {
                    // Record already exists, handle the duplicate case
                    ErrorMessageLabel.Text = "Record already exists.";
                    ErrorMessageLabel.Visible = true;
                }
                else
                {
                    string query = "INSERT INTO ProjDepEmp ([project_id],[department_id], [employee_id]) VALUES (@projectId, @DepartmentId, @employeeId) ";
                    SqlCommand command = new SqlCommand(query, conn);
                    {
                        command.Parameters.AddWithValue("@projectId", projectId);
                        command.Parameters.AddWithValue("@DepartmentId", departmentId);
                        command.Parameters.AddWithValue("@employeeId", employeeId);

                        conn.Open();
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            ErrorMessageLabel.Text = "An unexpected error occurred." + ex.Message;
                            ErrorMessageLabel.Visible = true;
                        }
                    }


                    //conn.Close();

                    //GridView1.EditIndex = -1; //to exit the edit mood
                    BindGridView();
                    ErrorMessageLabel.Visible = false;
                }

            }
            else
            {
                ErrorMessageLabel.Text = "Insufficient funds for this project.";
                ErrorMessageLabel.Visible = true;
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
                        //System.Diagnostics.Debug.WriteLine("availableFunds" + availableFunds);
                        //System.Diagnostics.Debug.WriteLine("requiredFunds"+requiredFunds);
                        isAvailable = availableFunds >= requiredFunds;
                        //System.Diagnostics.Debug.WriteLine(isAvailable);
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
                            //System.Diagnostics.Debug.WriteLine("month"+startDate.Month);
                            //System.Diagnostics.Debug.WriteLine("year"+startDate.Year);

                            endDate = Convert.ToDateTime(reader["end_date"]);
                            //System.Diagnostics.Debug.WriteLine("month" + endDate.Month);
                            //System.Diagnostics.Debug.WriteLine("year" + endDate.Year);
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
            //System.Diagnostics.Debug.WriteLine("prid" + projectId);
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
                        //System.Diagnostics.Debug.WriteLine("sumOfSalaries "+sumOfSalaries);
                    }

                }
            }
            return sumOfSalaries;
        }
        public decimal GetEmployeeSalaryForProject(int projectId, int employeeId)
        {
            decimal employeeSalary = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT e.salary  
            FROM Employee AS e where e.employee_id = @employeeId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        employeeSalary = Convert.ToDecimal(result);
                        //System.Diagnostics.Debug.WriteLine("employeeSalary: "+employeeSalary);
                    }
                }
            }

            return employeeSalary;
        }
        public List<string> GetAllDepartments()
        {
            List<string> departments = new List<string>();

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

            return departments;
        }

        public List<string> GetAllEmployees()
        {
            List<string> employees = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT employee_id, employee_name FROM employee";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int depId = Convert.ToInt32(reader["department_id"]);
                            string employeeName = reader["employee_name"].ToString();
                            employees.Add(employeeName);
                        }
                    }
                }
            }

            return employees;
        }
        protected List<string> GetAllProjects()

        {
            List<string> projects = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT project_id, project_name FROM Project";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string projectName = reader["project_name"].ToString();
                            projects.Add(projectName);
                        }
                    }
                }
            }
            return projects;
        }
        public int GetPojectIdByName(string projectName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT project_id FROM Project WHERE project_name = @projectName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectName", projectName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if department name is not found
        }
        public int GetDepartmentIdByName(string departmentName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT department_id FROM department WHERE department_name = @departmentName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@departmentName", departmentName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if department name is not found
        }
        public int GetEmployeeIdByName(string employeeName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT employee_id FROM Employee WHERE employee_name = @employeeName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeName", employeeName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if department name is not found
        }

        protected void btn_filter(object sender, EventArgs e)
        {
            //Response.Redirect("SearchForm.aspx");
            string prj = txtprj.Text;
            //string dep = txtdep.Text;
            //string emp = txtemp.Text;
            string selectedSectorName = DropDownList5.SelectedValue;
            string selectedDepartmentName = DropDownList3.SelectedValue;
            int departmentId = GetDepartmentIdByName(selectedDepartmentName);
            string selectedEmployeeName = DropDown.SelectedValue;
            //System.Diagnostics.Debug.WriteLine("selectedEmployeeName "+ selectedEmployeeName);
            //string sector = txtsec.Text;
            string query = @" 
                SELECT 
                id,
                P.project_id, P.project_name,              
                D.department_id, D.department_name,                
                E.employee_id, E.employee_name,
                S.sector_id, S.sector_name
                FROM 
                    projDepEmp 
                JOIN
                    Department as D ON projDepEmp.department_id = D.department_id 
                JOIN
                    Employee as E ON projDepEmp.employee_id = E.employee_id
                JOIN
                    project as P ON projDepEmp.project_id = P.project_id 
                JOIN 
                    Sector as S ON D.sector_id=S.sector_id";

            string queryCondition = "";
            if (!string.IsNullOrEmpty(prj))
            {
                queryCondition += $" AND P.project_name LIKE '%{prj}%' ";
            }

            if (selectedDepartmentName != "select Department" && departmentId != 0)
            {
                queryCondition += $" AND D.department_id= {departmentId} ";
            }
            if (selectedSectorName != "Select Sector")
            {
                queryCondition += $" AND S.sector_name LIKE '%{selectedSectorName}%' ";
            }
            if (selectedEmployeeName != "select Employee")
            {
                queryCondition += $" AND E.employee_name LIKE '%{selectedEmployeeName}%' ";
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
            DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
            depDropDown.Items.Insert(0, "select Department");
            DropDownList empDropDown = GridView1.FooterRow.FindControl("empDropDown") as DropDownList;
            empDropDown.Items.Insert(0, "select Employee");
            DropDownList DropDownList1 = GridView1.FooterRow.FindControl("DropDownList1") as DropDownList;
            DropDownList1.Items.Insert(0, "select Project");
        }
        protected void DepartmentDropDownFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDepartmentName = DropDownList3.SelectedValue;
            List<string> employeeForDepartment = GetAllEmployeesByDepartment(selectedDepartmentName); DropDown.DataSource = employeeForDepartment;
            DropDown.DataBind();
            DropDown.Items.Insert(0, "select Employee");


            //DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
            //string selectedDepartmentName = depDropDown.SelectedValue;
            //List<string> employeeForDepartment = GetAllEmployeesByDepartment(selectedDepartmentName);
            //DropDownList empDropDown = GridView1.FooterRow.FindControl("empDropDown") as DropDownList;
            //empDropDown.Enabled = true;
            //empDropDown.DataSource = employeeForDepartment;
            //empDropDown.DataBind();
            //empDropDown.Items.Insert(0, "select Employee");
        }

        protected void DropDownList5_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSectorName = DropDownList5.SelectedValue;
            List<string> departmentForSector = GetAllDepartmentBySectors(selectedSectorName);

            DropDownList3.DataSource = departmentForSector;
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, "select Department");


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

        protected void DepartmentDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
            string selectedDepartmentName = depDropDown.SelectedValue;
            //int departmentId = GetDepartmentIdByName(selectedDepartmentName);
            List<string> employeeForDepartment = GetAllEmployeesByDepartment(selectedDepartmentName);
            //foreach (var sector in employeeForDepartment)
            //{
            //    System.Diagnostics.Debug.WriteLine(sector);
            //}
            DropDownList empDropDown = GridView1.FooterRow.FindControl("empDropDown") as DropDownList;
            empDropDown.Enabled = true;
            empDropDown.DataSource = employeeForDepartment;
            empDropDown.DataBind();
            empDropDown.Items.Insert(0, "select Employee");
        }
        public List<string> GetAllEmployeesByDepartment(string departmentName)
        {
            List<string> employees = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct the query to select sectors based on the department name
                string query = @"SELECT e.employee_name
                         FROM Employee AS e
                         JOIN Department AS d ON e.department_id=d.department_id
                         WHERE d.department_name = @departmentName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parameterize the query to protect against SQL injection
                    command.Parameters.AddWithValue("@departmentName", departmentName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string empName = reader["employee_name"].ToString();
                            employees.Add(empName);
                        }
                    }
                }
            }

            return employees;
        }
        protected void SectorDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList sectorDropDown = GridView1.FooterRow.FindControl("SectorDropDown") as DropDownList;
            string selectedSectorName = sectorDropDown.SelectedValue;
            //int departmentId = GetDepartmentIdByName(selectedDepartmentName);
            List<string> departmentForSector = GetAllDepartmentsBySector(selectedSectorName);
            foreach (var sector in departmentForSector)
            {
                System.Diagnostics.Debug.WriteLine(sector);
            }
            DropDownList depDropDown = GridView1.FooterRow.FindControl("depDropDown") as DropDownList;
            depDropDown.Enabled = true;
            depDropDown.DataSource = departmentForSector;
            depDropDown.DataBind();
            depDropDown.Items.Insert(0, "select Department");
        }
        public List<string> GetAllDepartmentsBySector(string sectorName)
        {
            List<string> departments = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct the query to select sectors based on the department name
                string query = @"SELECT d.department_name
                         FROM Department AS d
                         JOIN Sector AS s ON d.sector_id = s.sector_id 
                         WHERE S.sector_name = @sectorName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parameterize the query to protect against SQL injection
                    command.Parameters.AddWithValue("@sectorName", sectorName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string depName = reader["department_name"].ToString();
                            departments.Add(depName);
                        }
                    }
                }
            }

            return departments;
        }
        protected void ProjectNavigation(object sender, EventArgs e)
        {
            Response.Redirect("ProjectForm.aspx");
        }

        protected void DepartmentNavigation(object sender, EventArgs e)
        {
            Response.Redirect("departmentForm.aspx");
        }

        protected void EmployeeNavigation(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeWebForm.aspx");
        }
    }

    public class boolean
    {
    }
}