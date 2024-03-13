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
    public partial class PhongBan : Form
    {
        public string conn = "Server=DESKTOP-M9OV124\\HINH;Database=EmployeeManager;Integrated Security=SSPI;";
        public SqlConnection sqlConnection;
        public DuLieu form1;
        public PhongBan(DuLieu form)
        {
            form1 = form;
            InitializeComponent();
            sqlConnection = new SqlConnection(conn);

        }
        public void viewData()
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                string sql = "SELECT maPB AS MaPB, tenPB AS TenPhongBan " +
                      "FROM PhongBan";
                SqlCommand command = new SqlCommand(sql, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dataTable;

                }

                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        int phongbanId;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                form1.DisplayCellContent(selectedRow, "TenPhongBan", txtdonvi);
                if (dataGridView1.Columns.Contains("maPB"))
                {
                    object giaovienIdObj = selectedRow.Cells["maPB"].Value;
                    phongbanId = giaovienIdObj != DBNull.Value ? Convert.ToInt32(giaovienIdObj) : 0;
                }

            }
        }


        private void PhongBan_Load(object sender, EventArgs e)
        {
            viewData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
                string phongban = txtdonvi.Text;
                if (!string.IsNullOrEmpty(phongban))
                {
                    string sql = "INSERT INTO PhongBan(tenPB) " +
                                 "VALUES (@phongban) ";


                    using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@phongban", phongban);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm dữ liệu thành công");
                            txtdonvi.Text = "";
                            viewData();
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi xảy ra");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ky Tự Không hop le");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
                string phongban = txtdonvi.Text;
                string sql1 = @"UPDATE PhongBan
                                SET 
                                tenPB = @phongban 
                                WHERE maPB = @maCVid";
                SqlCommand command = new SqlCommand(sql1, sqlConnection);
                command.Parameters.AddWithValue("@phongban", phongban);
                command.Parameters.AddWithValue("@maCVid", phongbanId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật thành công!");
                    txtdonvi.Text = "";
                    viewData();
                }
                else
                {
                    MessageBox.Show("Không có gì được cập nhật.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {


                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }
                    string sql = "DELETE FROM PhongBan WHERE maPB = " + phongbanId;
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        txtdonvi.Text = "";

                        viewData();
                    }
                    else
                    {
                        MessageBox.Show("Không có gì được Xóa.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);

                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        private void ThemChucVu_FormClosed(object sender, FormClosedEventArgs e)
        {
            form1.UpdateDonViComboBox();

        }
    }
}
