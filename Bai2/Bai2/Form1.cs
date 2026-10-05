using Bai2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai2
{
    public partial class Form1 : Form
    {
        private TextBox txtMaHV, txtHoTen, txtSDT, txtEmail, txtTimKiem;
        private RadioButton radNam, radNu;
        private DateTimePicker dtpNgaySinh;
        private ComboBox cbHangThanhVien, cbTimKiemHang;
        private CheckBox chkTrangThai;
        private Button btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private DataGridView dgvHoiVien;
        private LeThiThuHuyenBtlt7Context dbContext;

        public Form1()
        {
            TaoGiaoDienBangCode();
            dbContext = new LeThiThuHuyenBtlt7Context();
            this.Load += Form1_Load;
        }

        private void TaoGiaoDienBangCode()
        {
            this.Text = "Quản Lý Hội Viên Phòng Gym FitZone";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblHoTen = new Label { Text = "Họ tên", Location = new Point(20, 20), AutoSize = true };
            txtHoTen = new TextBox { Location = new Point(120, 20), Width = 200 };

            Label lblSDT = new Label { Text = "Số điện thoại", Location = new Point(20, 60), AutoSize = true };
            txtSDT = new TextBox { Location = new Point(120, 60), Width = 200 };

            Label lblEmail = new Label { Text = "Email", Location = new Point(20, 100), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(120, 100), Width = 200 };

            Label lblNgaySinh = new Label { Text = "Ngày sinh", Location = new Point(20, 140), AutoSize = true };
            dtpNgaySinh = new DateTimePicker { Location = new Point(120, 140), Width = 200, Format = DateTimePickerFormat.Short };

            Label lblHang = new Label { Text = "Hạng thành viên", Location = new Point(20, 180), AutoSize = true };
            cbHangThanhVien = new ComboBox { Location = new Point(120, 180), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cbHangThanhVien.Items.AddRange(new object[] { "Basic", "VIP", "Premium" });

            chkTrangThai = new CheckBox { Text = "Đang hoạt động", Location = new Point(20, 220), AutoSize = true };

            GroupBox grpGioiTinh = new GroupBox { Text = "Giới tính", Location = new Point(350, 15), Size = new Size(200, 60) };
            radNam = new RadioButton { Text = "Nam", Location = new Point(20, 25), AutoSize = true, Checked = true };
            radNu = new RadioButton { Text = "Nữ", Location = new Point(100, 25), AutoSize = true };
            grpGioiTinh.Controls.Add(radNam);
            grpGioiTinh.Controls.Add(radNu);

            btnThem = new Button { Text = "Thêm", Location = new Point(750, 20), Width = 100 };
            btnSua = new Button { Text = "Sửa", Location = new Point(750, 60), Width = 100 };
            btnXoa = new Button { Text = "Xóa", Location = new Point(750, 100), Width = 100 };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(750, 140), Width = 100 };

            txtTimKiem = new TextBox { Location = new Point(20, 270), Width = 350 };
            cbTimKiemHang = new ComboBox { Location = new Point(390, 270), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTimKiemHang.Items.AddRange(new object[] { "", "Basic", "VIP", "Premium" });
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(560, 268), Width = 100 };

            txtMaHV = new TextBox { Visible = false };

            dgvHoiVien = new DataGridView
            {
                Location = new Point(20, 310),
                Size = new Size(890, 230),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            btnLamMoi.Click += BtnLamMoi_Click;
            btnThem.Click += async (s, e) => await BtnThem_ClickAsync();
            btnSua.Click += async (s, e) => await BtnSua_ClickAsync();
            btnXoa.Click += async (s, e) => await BtnXoa_ClickAsync();
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvHoiVien.SelectionChanged += DgvHoiVien_SelectionChanged;

            this.Controls.Add(lblHoTen); this.Controls.Add(txtHoTen);
            this.Controls.Add(lblSDT); this.Controls.Add(txtSDT);
            this.Controls.Add(lblEmail); this.Controls.Add(txtEmail);
            this.Controls.Add(lblNgaySinh); this.Controls.Add(dtpNgaySinh);
            this.Controls.Add(lblHang); this.Controls.Add(cbHangThanhVien);
            this.Controls.Add(chkTrangThai);
            this.Controls.Add(grpGioiTinh);
            this.Controls.Add(btnThem); this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa); this.Controls.Add(btnLamMoi);
            this.Controls.Add(txtTimKiem); this.Controls.Add(cbTimKiemHang); this.Controls.Add(btnTimKiem);
            this.Controls.Add(dgvHoiVien);
            this.Controls.Add(txtMaHV);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = dbContext.HoiViens.Select(h => new
            {
                Mã_HV = h.MaHv,
                Họ_tên = h.HoTen,
                Giới_tính = h.GioiTinh == true ? "Nam" : "Nữ",
                Ngày_sinh = h.NgaySinh,
                SĐT = h.Sdt,
                Hạng_thành_viên = h.HangThanhVien,
                Trạng_thái = h.TrangThai == true ? "Đang hoạt động" : "Tạm ngưng",
                Email = h.Email
            }).ToList();
            dgvHoiVien.DataSource = data;
        }

        private void DgvHoiVien_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoiVien.CurrentRow != null)
            {
                txtMaHV.Text = dgvHoiVien.CurrentRow.Cells["Mã_HV"].Value?.ToString();
                txtHoTen.Text = dgvHoiVien.CurrentRow.Cells["Họ_tên"].Value?.ToString();
                txtSDT.Text = dgvHoiVien.CurrentRow.Cells["SĐT"].Value?.ToString();
                txtEmail.Text = dgvHoiVien.CurrentRow.Cells["Email"].Value?.ToString();

                if (dgvHoiVien.CurrentRow.Cells["Ngày_sinh"].Value != null)
                {
                    dtpNgaySinh.Value = Convert.ToDateTime(dgvHoiVien.CurrentRow.Cells["Ngày_sinh"].Value);
                }

                string hang = dgvHoiVien.CurrentRow.Cells["Hạng_thành_viên"].Value?.ToString();
                if (cbHangThanhVien.Items.Contains(hang))
                {
                    cbHangThanhVien.SelectedItem = hang;
                }

                string gioiTinh = dgvHoiVien.CurrentRow.Cells["Giới_tính"].Value?.ToString();
                if (gioiTinh == "Nam") radNam.Checked = true;
                else radNu.Checked = true;

                string trangThai = dgvHoiVien.CurrentRow.Cells["Trạng_thái"].Value?.ToString();
                chkTrangThai.Checked = (trangThai == "Đang hoạt động");
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaHV.Clear();
            txtHoTen.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtTimKiem.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cbHangThanhVien.SelectedIndex = -1;
            cbTimKiemHang.SelectedIndex = -1;
            radNam.Checked = true;
            chkTrangThai.Checked = false;
            LoadData();
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{9,11}$"))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa chữ số và có độ dài từ 9-11 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ (phải chứa ký tự '@')!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (cbHangThanhVien.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn hạng thành viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int age = DateTime.Now.Year - dtpNgaySinh.Value.Year;
            if (dtpNgaySinh.Value.Date > DateTime.Now.AddYears(-age)) age--;
            if (age < 15)
            {
                MessageBox.Show("Hội viên phải từ 15 tuổi trở lên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private async Task BtnThem_ClickAsync()
        {
            if (!ValidateData()) return;

            var hv = new HoiVien
            {
                HoTen = txtHoTen.Text.Trim(),
                Sdt = txtSDT.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value),
                HangThanhVien = cbHangThanhVien.SelectedItem.ToString(),
                GioiTinh = radNam.Checked,
                TrangThai = chkTrangThai.Checked,
                NgayDangKy = DateTime.Now
            };

            dbContext.HoiViens.Add(hv);
            await dbContext.SaveChangesAsync();
            MessageBox.Show("Thêm hội viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }

        private async Task BtnSua_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaHV.Text)) return;
            if (!ValidateData()) return;

            int id = int.Parse(txtMaHV.Text);
            var hv = dbContext.HoiViens.Find(id);
            if (hv != null)
            {
                hv.HoTen = txtHoTen.Text.Trim();
                hv.Sdt = txtSDT.Text.Trim();
                hv.Email = txtEmail.Text.Trim();
                hv.NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value);
                hv.HangThanhVien = cbHangThanhVien.SelectedItem.ToString();
                hv.GioiTinh = radNam.Checked;
                hv.TrangThai = chkTrangThai.Checked;

                await dbContext.SaveChangesAsync();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private async Task BtnXoa_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaHV.Text)) return;

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa hội viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                int id = int.Parse(txtMaHV.Text);
                var hv = dbContext.HoiViens.Find(id);
                if (hv != null)
                {
                    dbContext.HoiViens.Remove(hv);
                    await dbContext.SaveChangesAsync();
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BtnLamMoi_Click(null, null);
                }
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            string kw = txtTimKiem.Text.Trim().ToLower();
            string hang = cbTimKiemHang.SelectedItem?.ToString() ?? "";

            var query = dbContext.HoiViens
                .Where(h => h.HoTen.ToLower().Contains(kw) && h.HangThanhVien.Contains(hang))
                .Select(h => new
                {
                    Mã_HV = h.MaHv,
                    Họ_tên = h.HoTen,
                    Giới_tính = h.GioiTinh == true ? "Nam" : "Nữ",
                    Ngày_sinh = h.NgaySinh,
                    SĐT = h.Sdt,
                    Hạng_thành_viên = h.HangThanhVien,
                    Trạng_thái = h.TrangThai == true ? "Đang hoạt động" : "Tạm ngưng",
                    Email = h.Email
                }).ToList();

            dgvHoiVien.DataSource = query;
        }
    }
}