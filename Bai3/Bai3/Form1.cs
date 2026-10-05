using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Bai3.Models;

namespace Bai3
{
    public partial class Form1 : Form
    {
        private TextBox txtSoPhong, txtMaPhong;
        private NumericUpDown numTangSo;
        private ComboBox cbLoaiPhong, cbTinhTrang, cbLocLoaiPhong, cbLocTinhTrang;
        private PictureBox picHinhAnh;
        private Button btnChonAnh, btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private DataGridView dgvPhong;
        private LeThiThuHuyenBtlt7Context dbContext;
        private string tenFileAnh = "";
        private string thuMucAnh = Path.Combine(Application.StartupPath, "Images");

        public Form1()
        {
            TaoGiaoDienBangCode();
            dbContext = new LeThiThuHuyenBtlt7Context();

            if (!Directory.Exists(thuMucAnh))
            {
                Directory.CreateDirectory(thuMucAnh);
            }

            this.Load += Form1_Load;
        }

        private void TaoGiaoDienBangCode()
        {
            this.Text = "Quản Lý Phòng - Sunrise Homestay";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblSoPhong = new Label { Text = "Số phòng", Location = new Point(20, 20), AutoSize = true };
            txtSoPhong = new TextBox { Location = new Point(20, 40), Width = 100 };

            Label lblTang = new Label { Text = "Tầng số", Location = new Point(140, 20), AutoSize = true };
            numTangSo = new NumericUpDown { Location = new Point(140, 40), Width = 80, Minimum = 1, Maximum = 50 };

            Label lblLoaiPhong = new Label { Text = "Loại phòng", Location = new Point(240, 20), AutoSize = true };
            cbLoaiPhong = new ComboBox { Location = new Point(240, 40), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblTinhTrang = new Label { Text = "Tình trạng", Location = new Point(380, 20), AutoSize = true };
            cbTinhTrang = new ComboBox { Location = new Point(380, 40), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTinhTrang.Items.AddRange(new object[] { "Trống", "Đang ở", "Đang dọn" });

            picHinhAnh = new PictureBox { Location = new Point(520, 20), Size = new Size(100, 70), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            btnChonAnh = new Button { Text = "Chọn ảnh...", Location = new Point(520, 95), Width = 100 };

            btnThem = new Button { Text = "Thêm", Location = new Point(650, 20), Width = 70 };
            btnSua = new Button { Text = "Sửa", Location = new Point(730, 20), Width = 70 };
            btnXoa = new Button { Text = "Xóa", Location = new Point(810, 20), Width = 70 };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(890, 20), Width = 70 };

            cbLocLoaiPhong = new ComboBox { Location = new Point(20, 150), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbLocTinhTrang = new ComboBox { Location = new Point(190, 150), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbLocTinhTrang.Items.AddRange(new object[] { "Tất cả", "Trống", "Đang ở", "Đang dọn" });
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(360, 148), Width = 80 };

            txtMaPhong = new TextBox { Visible = false };

            dgvPhong = new DataGridView
            {
                Location = new Point(20, 190),
                Size = new Size(940, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 60 }
            };

            btnChonAnh.Click += BtnChonAnh_Click;
            btnLamMoi.Click += BtnLamMoi_Click;
            btnThem.Click += async (s, e) => await BtnThem_ClickAsync();
            btnSua.Click += async (s, e) => await BtnSua_ClickAsync();
            btnXoa.Click += async (s, e) => await BtnXoa_ClickAsync();
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvPhong.SelectionChanged += DgvPhong_SelectionChanged;

            this.Controls.Add(lblSoPhong); this.Controls.Add(txtSoPhong);
            this.Controls.Add(lblTang); this.Controls.Add(numTangSo);
            this.Controls.Add(lblLoaiPhong); this.Controls.Add(cbLoaiPhong);
            this.Controls.Add(lblTinhTrang); this.Controls.Add(cbTinhTrang);
            this.Controls.Add(picHinhAnh); this.Controls.Add(btnChonAnh);
            this.Controls.Add(btnThem); this.Controls.Add(btnSua); this.Controls.Add(btnXoa); this.Controls.Add(btnLamMoi);
            this.Controls.Add(cbLocLoaiPhong); this.Controls.Add(cbLocTinhTrang); this.Controls.Add(btnTimKiem);
            this.Controls.Add(dgvPhong); this.Controls.Add(txtMaPhong);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dsLoaiPhong = dbContext.LoaiPhongs.ToList();

            cbLoaiPhong.DataSource = dsLoaiPhong;
            cbLoaiPhong.DisplayMember = "TenLoai";
            cbLoaiPhong.ValueMember = "MaLoai";

            var dsLoc = dsLoaiPhong.ToList();
            dsLoc.Insert(0, new LoaiPhong { MaLoai = 0, TenLoai = "lọc theo loại phòng" });
            cbLocLoaiPhong.DataSource = dsLoc;
            cbLocLoaiPhong.DisplayMember = "TenLoai";
            cbLocLoaiPhong.ValueMember = "MaLoai";

            cbLocTinhTrang.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData(int maLoai = 0, string tinhTrang = "Tất cả")
        {
            var query = dbContext.Phongs.Include(p => p.MaLoaiNavigation).AsQueryable();

            if (maLoai != 0)
                query = query.Where(p => p.MaLoai == maLoai);
            if (tinhTrang != "Tất cả")
                query = query.Where(p => p.TinhTrang == tinhTrang);

            var list = query.ToList();

            dgvPhong.Columns.Clear();

            dgvPhong.Columns.Add("MaPhong", "Mã phòng");
            dgvPhong.Columns["MaPhong"].DataPropertyName = "MaPhong";

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "Anh";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dgvPhong.Columns.Add(imgCol);

            dgvPhong.Columns.Add("SoPhong", "Số phòng");
            dgvPhong.Columns["SoPhong"].DataPropertyName = "SoPhong";

            dgvPhong.Columns.Add("TangSo", "Tầng");
            dgvPhong.Columns["TangSo"].DataPropertyName = "TangSo";

            dgvPhong.Columns.Add("TenLoai", "Loại phòng");
            dgvPhong.Columns.Add("Gia", "Giá/đêm");

            dgvPhong.Columns.Add("TinhTrang", "Tình trạng");
            dgvPhong.Columns["TinhTrang"].DataPropertyName = "TinhTrang";

            dgvPhong.AutoGenerateColumns = false;
            dgvPhong.DataSource = list;

            foreach (DataGridViewRow row in dgvPhong.Rows)
            {
                var p = row.DataBoundItem as Phong;
                if (p != null)
                {
                    if (p.MaLoaiNavigation != null)
                    {
                        row.Cells["TenLoai"].Value = p.MaLoaiNavigation.TenLoai;
                        row.Cells["Gia"].Value = string.Format("{0:N0}", p.MaLoaiNavigation.GiaMoiDem);
                    }

                    if (!string.IsNullOrEmpty(p.HinhAnh))
                    {
                        string imgPath = Path.Combine(thuMucAnh, p.HinhAnh);
                        if (File.Exists(imgPath))
                        {
                            using (var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                            {
                                row.Cells["Anh"].Value = Image.FromStream(stream);
                            }
                        }
                    }
                }
            }
        }

        private void DgvPhong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow != null && dgvPhong.CurrentRow.DataBoundItem is Phong p)
            {
                txtMaPhong.Text = p.MaPhong.ToString();
                txtSoPhong.Text = p.SoPhong;
                numTangSo.Value = p.TangSo ?? 1;
                cbLoaiPhong.SelectedValue = p.MaLoai;
                cbTinhTrang.SelectedItem = p.TinhTrang;
                tenFileAnh = p.HinhAnh;

                if (!string.IsNullOrEmpty(tenFileAnh))
                {
                    string imgPath = Path.Combine(thuMucAnh, tenFileAnh);
                    if (File.Exists(imgPath))
                    {
                        using (var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                        {
                            picHinhAnh.Image = Image.FromStream(stream);
                        }
                    }
                    else
                    {
                        picHinhAnh.Image = null;
                    }
                }
                else
                {
                    picHinhAnh.Image = null;
                }
            }
        }

        private void BtnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(ofd.FileName);
                    tenFileAnh = Guid.NewGuid().ToString() + ext;
                    string destPath = Path.Combine(thuMucAnh, tenFileAnh);
                    File.Copy(ofd.FileName, destPath, true);

                    using (var stream = new FileStream(destPath, FileMode.Open, FileAccess.Read))
                    {
                        picHinhAnh.Image = Image.FromStream(stream);
                    }
                }
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaPhong.Clear();
            txtSoPhong.Clear();
            numTangSo.Value = 1;
            if (cbLoaiPhong.Items.Count > 0) cbLoaiPhong.SelectedIndex = 0;
            if (cbTinhTrang.Items.Count > 0) cbTinhTrang.SelectedIndex = 0;
            picHinhAnh.Image = null;
            tenFileAnh = "";
            cbLocLoaiPhong.SelectedIndex = 0;
            cbLocTinhTrang.SelectedIndex = 0;
            LoadData();
        }

        private async Task BtnThem_ClickAsync()
        {
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text)) return;

            var p = new Phong
            {
                SoPhong = txtSoPhong.Text.Trim(),
                TangSo = (int)numTangSo.Value,
                MaLoai = (int)cbLoaiPhong.SelectedValue,
                TinhTrang = cbTinhTrang.SelectedItem?.ToString(),
                HinhAnh = tenFileAnh
            };

            dbContext.Phongs.Add(p);
            await dbContext.SaveChangesAsync();
            LoadData();
        }

        private async Task BtnSua_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaPhong.Text)) return;

            int id = int.Parse(txtMaPhong.Text);
            var p = dbContext.Phongs.Find(id);
            if (p != null)
            {
                p.SoPhong = txtSoPhong.Text.Trim();
                p.TangSo = (int)numTangSo.Value;
                p.MaLoai = (int)cbLoaiPhong.SelectedValue;
                p.TinhTrang = cbTinhTrang.SelectedItem?.ToString();
                p.HinhAnh = tenFileAnh;

                await dbContext.SaveChangesAsync();
                LoadData();
            }
        }

        private async Task BtnXoa_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaPhong.Text)) return;

            int id = int.Parse(txtMaPhong.Text);
            var p = dbContext.Phongs.Find(id);
            if (p != null)
            {
                dbContext.Phongs.Remove(p);
                await dbContext.SaveChangesAsync();
                BtnLamMoi_Click(null, null);
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            int maLoai = (int)cbLocLoaiPhong.SelectedValue;
            string tinhTrang = cbLocTinhTrang.SelectedItem.ToString();
            LoadData(maLoai, tinhTrang);
        }
    }
}