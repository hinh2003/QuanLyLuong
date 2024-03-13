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
    public partial class DuLieu : Form
    {
        public string conn = "Server=DESKTOP-M9OV124\\HINH;Database=EmployeeManager;Integrated Security=SSPI;";
        public SqlConnection sqlConnection;
        SqlCommand command;
        public DuLieu()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(conn);

        }

        private void DuLieu_Load(object sender, EventArgs e)
        {
            viewData();
            info();
        }

        public void viewData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                string sql = "SELECT NhanVien.maNV AS MaNV, hoTen AS HoTen, diaChi AS DiaChi, soDienThoai AS SDT, ngaySinh AS Date, " +
                  "CASE WHEN gioiTinh = 0 THEN N'Nam' ELSE N'Nữ' END AS GioiTinh, " +
                  "danToc, PhongBan.tenPB AS TenPhongBan, ChucVu.tenCV AS TenChucVu " +
                  "FROM NhanVien " +
                  "INNER JOIN PhongBan ON NhanVien.maPB = PhongBan.maPB " +
                  "INNER JOIN ChucVu ON NhanVien.maCV = ChucVu.maCV ;";



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
        public void info()
        {
            sqlConnection.Open();
            string sqlchucvu = "SELECT tenCV FROM ChucVu";
            SqlCommand commandchucvu = new SqlCommand(sqlchucvu, sqlConnection);
            SqlDataReader readerchucvu = commandchucvu.ExecuteReader();

            while (readerchucvu.Read())
            {
                cbchucvu.Items.Add(readerchucvu["tenCV"].ToString());
            }

            readerchucvu.Close();
            string sqldonvi = "SELECT tenPB FROM PhongBan";
            SqlCommand commanddonvi = new SqlCommand(sqldonvi, sqlConnection);
            SqlDataReader readerdonvi = commanddonvi.ExecuteReader();

            while (readerdonvi.Read())
            {
                cbphongban.Items.Add(readerdonvi["tenPB"].ToString());
            }

            readerdonvi.Close();
            sqlConnection.Close();
        }
        public void DisplayCellContent(DataGridViewRow row, string columnName, Control control)
        {
            if (row.Cells[columnName].Value != DBNull.Value)
            {
                control.Text = row.Cells[columnName].Value.ToString();
            }
            else
            {
                control.Text = string.Empty;
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                DisplayCellContent(selectedRow, "MaNV", txtmanv);
                DisplayCellContent(selectedRow, "HoTen", txtten);
                DisplayCellContent(selectedRow, "DiaChi", txtdiachi);
                DisplayCellContent(selectedRow, "SDT", txtsdt);
                DisplayCellContent(selectedRow, "Date", txtngaysinh);
                DisplayCellContent(selectedRow, "TenChucVu", cbchucvu);
                DisplayCellContent(selectedRow, "TenPhongBan", cbphongban);
                DisplayCellContent(selectedRow, "GioiTinh", txtgioitinh);
                DisplayCellContent(selectedRow, "danToc", txtdantoc);


            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            string name = txtten.Text;
            string MaNV = txtmanv.Text;
            string DiaChi = txtdiachi.Text;
            string SDT = txtsdt.Text;
            string Date = txtngaysinh.Text;
            string GioiTinh = txtgioitinh.Text;
            string danToc = txtdantoc.Text;

            string TenChucVu = cbchucvu.SelectedItem?.ToString();
            string TenPhongBan = cbphongban.SelectedItem?.ToString();

            int gioiTinhValue = (GioiTinh.ToLower() == "nam") ? 0 : 1;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(MaNV) && !string.IsNullOrEmpty(DiaChi) &&
                !string.IsNullOrEmpty(SDT) && !string.IsNullOrEmpty(Date) &&
                !string.IsNullOrEmpty(GioiTinh) && !string.IsNullOrEmpty(danToc))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        string sql = "INSERT INTO NhanVien (maNV, hoTen, diaChi, soDienThoai, ngaySinh, gioiTinh, " +
                                    "danToc, maPB, maCV) " +
                                    "VALUES (@maNV, @hoTen, @diaChi, @soDienThoai, @ngaySinh, @gioiTinh, " +
                                    "@danToc," +
                                    " (SELECT maPB FROM PhongBan WHERE tenPB = @maPB)," +
                                    " (SELECT maCV FROM ChucVu WHERE tenCV = @maCV))";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@maNV", MaNV);
                            command.Parameters.AddWithValue("@hoTen", name);
                            command.Parameters.AddWithValue("@diaChi", DiaChi);
                            command.Parameters.AddWithValue("@soDienThoai", SDT);
                            command.Parameters.AddWithValue("@ngaySinh", Date);
                            command.Parameters.AddWithValue("@gioiTinh", gioiTinhValue);
                            command.Parameters.AddWithValue("@danToc", danToc);
                            command.Parameters.AddWithValue("@maPB", TenPhongBan);
                            command.Parameters.AddWithValue("@maCV", TenChucVu);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Thêm dữ liệu thành công");
                                viewData();
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi xảy ra");
                            }

                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        MessageBox.Show("Mã nhân viên đã tồn tại. Vui lòng chọn mã khác.");
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Số điện thoại phải là số nguyên");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            string name = txtten.Text;
            string MaNV = txtmanv.Text;
            string DiaChi = txtdiachi.Text;
            string SDT = txtsdt.Text;
            string Date = txtngaysinh.Text;
            string GioiTinh = txtgioitinh.Text;
            string danToc = txtdantoc.Text;

            // Map gioiTinh to 0 (Nam) or 1 (Nữ)
            int gioiTinhValue = (GioiTinh.ToLower() == "nam") ? 0 : 1;

            // Get the selected items from the ComboBoxes
            string TenChucVu = cbchucvu.SelectedItem?.ToString();
            string TenPhongBan = cbphongban.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(MaNV) && !string.IsNullOrEmpty(DiaChi) &&
                !string.IsNullOrEmpty(SDT) && !string.IsNullOrEmpty(Date) &&
                !string.IsNullOrEmpty(GioiTinh) && !string.IsNullOrEmpty(danToc))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        string sql = "UPDATE NhanVien " +
                                     "SET hoTen = @hoTen, diaChi = @diaChi, soDienThoai = @soDienThoai, " +
                                     "ngaySinh = @ngaySinh, gioiTinh = @gioiTinh, danToc = @danToc, " +
                                     "maPB = (SELECT maPB FROM PhongBan WHERE tenPB = @maPB)," +
                                     "maCV = (SELECT maCV FROM ChucVu WHERE tenCV = @maCV) " +
                                     "WHERE maNV = @maNV";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@maNV", MaNV);
                            command.Parameters.AddWithValue("@hoTen", name);
                            command.Parameters.AddWithValue("@diaChi", DiaChi);
                            command.Parameters.AddWithValue("@soDienThoai", SDT);
                            command.Parameters.AddWithValue("@ngaySinh", Date);
                            command.Parameters.AddWithValue("@gioiTinh", gioiTinhValue);
                            command.Parameters.AddWithValue("@danToc", danToc);
                            command.Parameters.AddWithValue("@maPB", TenPhongBan);
                            command.Parameters.AddWithValue("@maCV", TenChucVu);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cập nhật thông tin thành công");
                                viewData();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy nhân viên có mã: " + MaNV);
                            }
                        }
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Số điện thoại phải là số nguyên");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string MaNV = txtmanv.Text;

            DialogResult result = MessageBox.Show("Bạn có muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    string sql = "DELETE FROM NhanVien WHERE maNV = @maNV";
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@maNV", MaNV);

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
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
        public void UpdateDonViComboBox()
        {
            cbchucvu.Items.Clear();
            cbphongban.Items.Clear();
            info();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ThemChucVu themchucVu = new ThemChucVu(this);
            this.Hide();
            themchucVu.ShowDialog();
            this.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            PhongBan phongban = new PhongBan(this);
            this.Hide();
            phongban.ShowDialog();
            this.Show();
        }



        private void button2_Click_1(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            ChamCong ChamCong = new ChamCong(this);
            this.Hide();
            ChamCong.ShowDialog();
            this.Show();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}