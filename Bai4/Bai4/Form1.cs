using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Bai4.Models;

namespace Bai4
{
    public partial class Form1 : Form
    {
        private TextBox txtTenBenhNhan, txtSDT, txtMaLich;
        private DateTimePicker dtpNgayKham, dtpGioKham, dtpTuNgay, dtpDenNgay;
        private ComboBox cbBacSi, cbTrangThai, cbLocBacSi;
        private Button btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private DataGridView dgvLichKham;
        private LeThiThuHuyenBtlt7Context dbContext;

        public Form1()
        {
            TaoGiaoDienBangCode();
            dbContext = new LeThiThuHuyenBtlt7Context();
            this.Load += Form1_Load;
        }

        private void TaoGiaoDienBangCode()
        {
            this.Text = "Quản Lý Lịch Khám Bệnh - An Khang Clinic";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTen = new Label { Text = "Tên bệnh nhân", Location = new Point(20, 20), AutoSize = true };
            txtTenBenhNhan = new TextBox { Location = new Point(120, 20), Width = 200 };

            Label lblSDT = new Label { Text = "Số điện thoại", Location = new Point(20, 60), AutoSize = true };
            txtSDT = new TextBox { Location = new Point(120, 60), Width = 200 };

            Label lblNgay = new Label { Text = "Ngày khám", Location = new Point(20, 100), AutoSize = true };
            dtpNgayKham = new DateTimePicker { Location = new Point(120, 100), Width = 150, Format = DateTimePickerFormat.Short };

            Label lblGio = new Label { Text = "Giờ khám", Location = new Point(290, 60), AutoSize = true };
            dtpGioKham = new DateTimePicker { Location = new Point(350, 60), Width = 100, Format = DateTimePickerFormat.Time, ShowUpDown = true };

            Label lblBacSi = new Label { Text = "Bác sĩ", Location = new Point(470, 60), AutoSize = true };
            cbBacSi = new ComboBox { Location = new Point(520, 60), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblTrangThai = new Label { Text = "Trạng thái", Location = new Point(780, 60), AutoSize = true };
            cbTrangThai = new ComboBox { Location = new Point(850, 60), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTrangThai.Items.AddRange(new object[] { "Chờ khám", "Đã khám", "Đã hủy" });

            btnThem = new Button { Text = "Thêm", Location = new Point(640, 15), Width = 70 };
            btnSua = new Button { Text = "Sửa", Location = new Point(720, 15), Width = 70 };
            btnXoa = new Button { Text = "Xóa", Location = new Point(800, 15), Width = 70 };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(880, 15), Width = 80 };

            Label lblTuNgay = new Label { Text = "Từ ngày", Location = new Point(20, 150), AutoSize = true };
            dtpTuNgay = new DateTimePicker { Location = new Point(80, 148), Width = 120, Format = DateTimePickerFormat.Short };

            Label lblDenNgay = new Label { Text = "Đến ngày", Location = new Point(220, 150), AutoSize = true };
            dtpDenNgay = new DateTimePicker { Location = new Point(290, 148), Width = 120, Format = DateTimePickerFormat.Short };

            cbLocBacSi = new ComboBox { Location = new Point(430, 148), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(650, 146), Width = 80 };

            txtMaLich = new TextBox { Visible = false };

            dgvLichKham = new DataGridView
            {
                Location = new Point(20, 190),
                Size = new Size(940, 350),
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
            dgvLichKham.SelectionChanged += DgvLichKham_SelectionChanged;

            this.Controls.Add(lblTen); this.Controls.Add(txtTenBenhNhan);
            this.Controls.Add(lblSDT); this.Controls.Add(txtSDT);
            this.Controls.Add(lblNgay); this.Controls.Add(dtpNgayKham);
            this.Controls.Add(lblGio); this.Controls.Add(dtpGioKham);
            this.Controls.Add(lblBacSi); this.Controls.Add(cbBacSi);
            this.Controls.Add(lblTrangThai); this.Controls.Add(cbTrangThai);
            this.Controls.Add(btnThem); this.Controls.Add(btnSua); this.Controls.Add(btnXoa); this.Controls.Add(btnLamMoi);
            this.Controls.Add(lblTuNgay); this.Controls.Add(dtpTuNgay);
            this.Controls.Add(lblDenNgay); this.Controls.Add(dtpDenNgay);
            this.Controls.Add(cbLocBacSi); this.Controls.Add(btnTimKiem);
            this.Controls.Add(dgvLichKham); this.Controls.Add(txtMaLich);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dsBacSi = dbContext.BacSis.Select(b => new { b.MaBs, HienThi = b.HoTen + " - " + b.ChuyenKhoa }).ToList();

            cbBacSi.DataSource = dsBacSi.ToList();
            cbBacSi.DisplayMember = "HienThi";
            cbBacSi.ValueMember = "MaBs";
            cbBacSi.SelectedIndex = -1;

            var dsLoc = dsBacSi.ToList();
            dsLoc.Insert(0, new { MaBs = 0, HienThi = "Tất cả bác sĩ" });
            cbLocBacSi.DataSource = dsLoc;
            cbLocBacSi.DisplayMember = "HienThi";
            cbLocBacSi.ValueMember = "MaBs";
            cbLocBacSi.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData(DateTime? tu = null, DateTime? den = null, int maBsLoc = 0)
        {
            var query = dbContext.LichKhams.Include(l => l.MaBsNavigation).AsQueryable();

            if (tu.HasValue)
            {
                var tuDate = DateOnly.FromDateTime(tu.Value);
                query = query.Where(l => l.NgayKham >= tuDate);
            }
            if (den.HasValue)
            {
                var denDate = DateOnly.FromDateTime(den.Value);
                query = query.Where(l => l.NgayKham <= denDate);
            }
            if (maBsLoc != 0)
            {
                query = query.Where(l => l.MaBs == maBsLoc);
            }

            var list = query.Select(l => new
            {
                Mã_lịch = l.MaLich,
                Tên_bệnh_nhân = l.TenBenhNhan,
                SĐT = l.Sdt,
                Ngày_khám = l.NgayKham,
                Giờ_khám = l.GioKham,
                Bác_sĩ = l.MaBsNavigation != null ? l.MaBsNavigation.HoTen : "",
                Chuyên_khoa = l.MaBsNavigation != null ? l.MaBsNavigation.ChuyenKhoa : "",
                Trạng_thái = l.TrangThai,
                MaBS = l.MaBs
            }).ToList();

            dgvLichKham.DataSource = list;
            dgvLichKham.Columns["MaBS"].Visible = false;
        }

        private void DgvLichKham_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLichKham.CurrentRow != null)
            {
                txtMaLich.Text = dgvLichKham.CurrentRow.Cells["Mã_lịch"].Value?.ToString();
                txtTenBenhNhan.Text = dgvLichKham.CurrentRow.Cells["Tên_bệnh_nhân"].Value?.ToString();
                txtSDT.Text = dgvLichKham.CurrentRow.Cells["SĐT"].Value?.ToString();

                if (dgvLichKham.CurrentRow.Cells["Ngày_khám"].Value != null)
                {
                    if (dgvLichKham.CurrentRow.Cells["Ngày_khám"].Value is DateOnly d)
                    {
                        dtpNgayKham.Value = d.ToDateTime(TimeOnly.MinValue);
                    }
                }

                string gio = dgvLichKham.CurrentRow.Cells["Giờ_khám"].Value?.ToString();
                if (!string.IsNullOrEmpty(gio))
                {
                    dtpGioKham.Value = DateTime.ParseExact(gio, "HH:mm", null);
                }

                if (dgvLichKham.CurrentRow.Cells["MaBS"].Value != null)
                {
                    cbBacSi.SelectedValue = Convert.ToInt32(dgvLichKham.CurrentRow.Cells["MaBS"].Value);
                }

                cbTrangThai.SelectedItem = dgvLichKham.CurrentRow.Cells["Trạng_thái"].Value?.ToString();
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaLich.Clear();
            txtTenBenhNhan.Clear();
            txtSDT.Clear();
            dtpNgayKham.Value = DateTime.Now;
            dtpGioKham.Value = DateTime.Now;
            cbBacSi.SelectedIndex = -1;
            cbTrangThai.SelectedIndex = -1;
            cbLocBacSi.SelectedIndex = 0;
            LoadData();
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(txtTenBenhNhan.Text))
            {
                MessageBox.Show("Tên bệnh nhân không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (cbBacSi.SelectedIndex == -1 || cbBacSi.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn bác sĩ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (dtpNgayKham.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Không cho đặt lịch khám vào ngày trong quá khứ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private async Task BtnThem_ClickAsync()
        {
            if (!ValidateData()) return;

            var lk = new LichKham
            {
                TenBenhNhan = txtTenBenhNhan.Text.Trim(),
                Sdt = txtSDT.Text.Trim(),
                NgayKham = DateOnly.FromDateTime(dtpNgayKham.Value),
                GioKham = dtpGioKham.Value.ToString("HH:mm"),
                MaBs = (int)cbBacSi.SelectedValue,
                TrangThai = cbTrangThai.SelectedItem?.ToString() ?? "Chờ khám"
            };

            dbContext.LichKhams.Add(lk);
            await dbContext.SaveChangesAsync();
            LoadData();
        }

        private async Task BtnSua_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaLich.Text)) return;
            if (!ValidateData()) return;

            int id = int.Parse(txtMaLich.Text);
            var lk = dbContext.LichKhams.Find(id);
            if (lk != null)
            {
                lk.TenBenhNhan = txtTenBenhNhan.Text.Trim();
                lk.Sdt = txtSDT.Text.Trim();
                lk.NgayKham = DateOnly.FromDateTime(dtpNgayKham.Value);
                lk.GioKham = dtpGioKham.Value.ToString("HH:mm");
                lk.MaBs = (int)cbBacSi.SelectedValue;
                lk.TrangThai = cbTrangThai.SelectedItem?.ToString() ?? "Chờ khám";

                await dbContext.SaveChangesAsync();
                LoadData();
            }
        }

        private async Task BtnXoa_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaLich.Text)) return;

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa lịch khám này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                int id = int.Parse(txtMaLich.Text);
                var lk = dbContext.LichKhams.Find(id);
                if (lk != null)
                {
                    dbContext.LichKhams.Remove(lk);
                    await dbContext.SaveChangesAsync();
                    BtnLamMoi_Click(null, null);
                }
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tu = dtpTuNgay.Value.Date;
            DateTime den = dtpDenNgay.Value.Date;
            int maBs = (int)cbLocBacSi.SelectedValue;

            LoadData(tu, den, maBs);
        }
    }
}