using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyTienLuongCtyVM
{
    public partial class ChamCong : Form
    {
        public string conn = "Server=DESKTOP-M9OV124\\HINH;Database=EmployeeManager;Integrated Security=SSPI;";
        public SqlConnection sqlConnection;
        SqlCommand command;
        public DuLieu form1;
        public ChamCong(DuLieu form)
        {
            form1 = form;
            sqlConnection = new SqlConnection(conn);
            InitializeComponent();
        }
        public void viewData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                string sql = "SELECT NhanVien.maNV AS MaNV, hoTen AS HoTen, COUNT(ChamCong.idChamCong) AS SoNgayChamCong " +
                                  "FROM NhanVien " +
                                  "LEFT JOIN ChamCong ON NhanVien.maNV = ChamCong.maNV " +
                                  "GROUP BY NhanVien.maNV, hoTen";

                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(Chamcongid))
            {
                MarkAttendance(Chamcongid);
            }
            else
            {
                MessageBox.Show("Please select an employee before marking attendance.");
            }
        }

        private void ChamCong_Load(object sender, EventArgs e)
        {
            viewData();
        }
        string Chamcongid;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                if (dataGridView1.Columns.Contains("maNV"))
                {
                    object giaovienIdObj = selectedRow.Cells["maNV"].Value;

                    if (giaovienIdObj != null)
                    {
                        Chamcongid = giaovienIdObj.ToString();
                        form1.DisplayCellContent(selectedRow, "maNV", txtdonvi);
                    }
                    else
                    {
                        // Handle the case where the value is null or empty
                        MessageBox.Show("Invalid employee ID.");
                    }
                }
            }

        }

        private void MarkAttendance(string maNV)
        {
            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                string sql = "INSERT INTO ChamCong (maNV, ngayChamCong) VALUES (@maNV, GETDATE())";

                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@maNV", maNV);

                    // Execute the SQL command to mark attendance
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show($"Chấm Công thành công cho nhân viên : {maNV}.");
            viewData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void CalculateSalary(string maNV)
        {
            // Example:
            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                // Use a parameterized query to avoid SQL injection
                string sqlDaysWorked = "SELECT COUNT(idChamCong) AS SoNgayLam FROM ChamCong WHERE maNV = @maNV AND ngayChamCong BETWEEN DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) AND GETDATE()";

                string sqlSalaryInfo = "SELECT NhanVien.hoTen AS HoTen, ChucVu.HeSoLuong FROM NhanVien " +
                                       "INNER JOIN ChucVu ON NhanVien.maCV = ChucVu.maCV " +
                                       "WHERE NhanVien.maNV = @maNV";

                using (SqlCommand commandDaysWorked = new SqlCommand(sqlDaysWorked, sqlConnection))
                using (SqlCommand commandSalaryInfo = new SqlCommand(sqlSalaryInfo, sqlConnection))
                {
                    // Add parameters
                    commandDaysWorked.Parameters.AddWithValue("@maNV", maNV);
                    commandSalaryInfo.Parameters.AddWithValue("@maNV", maNV);

                    // Execute the SQL command to get the number of days worked
                    int soNgayLam = Convert.ToInt32(commandDaysWorked.ExecuteScalar());

                    // Execute the SQL command to get the salary information
                    using (SqlDataReader reader = commandSalaryInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hoTen = reader["HoTen"].ToString();
                            double heSoLuong = Convert.ToDouble(reader["HeSoLuong"]);

                            // Calculate the salary
                            double luong = soNgayLam * heSoLuong * 200000;

                            // Display the calculated salary or save it to the database, as needed
                            MessageBox.Show($"Lương của  {hoTen}: {luong} VND");
                        }
                    }
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Chamcongid))
            {
                CalculateSalary(Chamcongid);
            }
            else
            {
                MessageBox.Show("Please select an employee before calculating salary.");
            }
        }
    }
    
}
