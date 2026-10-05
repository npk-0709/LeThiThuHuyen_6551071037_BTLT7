using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Bai1.Models; // Gọi namespace chứa Models vừa được scaffold

namespace Bai1
{
    public partial class Form1 : Form
    {
        // Khai báo các biến Control UI
        private TextBox txtMaTL, txtTenTL, txtMoTa, txtTimKiem;
        private Label lblNgayTao;
        private Button btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private DataGridView dgvTheLoai;

        // Khai báo DbContext
        private LeThiThuHuyenBtlt7Context dbContext;

        public Form1()
        {
            // Thay vì InitializeComponent() của designer, ta gọi hàm tự viết
            TaoGiaoDienBangCode();
            dbContext = new LeThiThuHuyenBtlt7Context();
            this.Load += Form1_Load;
        }

        // --- HÀM TẠO GIAO DIỆN BẰNG CODE ---
        private void TaoGiaoDienBangCode()
        {
            this.Text = "Quản Lý Thể Loại Sách - Tri Thức Books";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Labels & TextBoxes
            Label lblMa = new Label { Text = "Mã thể loại", Location = new Point(20, 20), AutoSize = true };
            txtMaTL = new TextBox { Location = new Point(100, 20), Width = 250, ReadOnly = true, BackColor = Color.WhiteSmoke };

            Label lblTen = new Label { Text = "Tên thể loại", Location = new Point(20, 60), AutoSize = true };
            txtTenTL = new TextBox { Location = new Point(100, 60), Width = 250 };

            Label lblMo = new Label { Text = "Mô tả", Location = new Point(20, 100), AutoSize = true };
            txtMoTa = new TextBox { Location = new Point(100, 100), Width = 250, Height = 80, Multiline = true };

            lblNgayTao = new Label { Text = "Ngày tạo: ...", Location = new Point(100, 190), AutoSize = true, ForeColor = Color.Gray };

            // Buttons Thêm, Sửa, Xóa, Làm Mới
            btnThem = new Button { Text = "Thêm", Location = new Point(450, 20), Width = 80 };
            btnSua = new Button { Text = "Sửa", Location = new Point(540, 20), Width = 80 };
            btnXoa = new Button { Text = "Xóa", Location = new Point(630, 20), Width = 80 };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(720, 20), Width = 80 };

            // TextBox & Button Tìm kiếm
            txtTimKiem = new TextBox { Location = new Point(20, 230), Width = 330 };
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(360, 228), Width = 90 };

            // DataGridView
            dgvTheLoai = new DataGridView
            {
                Location = new Point(20, 270),
                Size = new Size(780, 220),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            // Gắn sự kiện (Event handlers)
            btnLamMoi.Click += BtnLamMoi_Click;
            btnThem.Click += async (s, e) => await BtnThem_ClickAsync();
            btnSua.Click += async (s, e) => await BtnSua_ClickAsync();
            btnXoa.Click += async (s, e) => await BtnXoa_ClickAsync();
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvTheLoai.SelectionChanged += DgvTheLoai_SelectionChanged;

            // Thêm tất cả Control vào Form
            this.Controls.Add(lblMa); this.Controls.Add(txtMaTL);
            this.Controls.Add(lblTen); this.Controls.Add(txtTenTL);
            this.Controls.Add(lblMo); this.Controls.Add(txtMoTa);
            this.Controls.Add(lblNgayTao);
            this.Controls.Add(btnThem); this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa); this.Controls.Add(btnLamMoi);
            this.Controls.Add(txtTimKiem); this.Controls.Add(btnTimKiem);
            this.Controls.Add(dgvTheLoai);
        }

        // --- CÁC HÀM XỬ LÝ LOGIC ---

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Load dữ liệu lên DataGridView
        private void LoadData()
        {
            dgvTheLoai.DataSource = dbContext.TheLoaiSaches
                .Select(t => new
                {
                    Mã_TL = t.MaTl,
                    Tên_Thể_Loại = t.TenTheLoai,
                    Mô_Tả = t.MoTa,
                    Số_Lượng_Sách = t.SoLuongSach,
                    Ngày_Tạo = t.NgayTao
                }).ToList();
        }

        // Tự động đổ dữ liệu dòng đang chọn lên Textbox
        private void DgvTheLoai_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTheLoai.CurrentRow != null)
            {
                txtMaTL.Text = dgvTheLoai.CurrentRow.Cells["Mã_TL"].Value?.ToString();
                txtTenTL.Text = dgvTheLoai.CurrentRow.Cells["Tên_Thể_Loại"].Value?.ToString();
                txtMoTa.Text = dgvTheLoai.CurrentRow.Cells["Mô_Tả"].Value?.ToString();
                lblNgayTao.Text = "Ngày tạo: " + dgvTheLoai.CurrentRow.Cells["Ngày_Tạo"].Value?.ToString();
            }
        }

        // Nút Làm Mới
        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaTL.Clear();
            txtTenTL.Clear();
            txtMoTa.Clear();
            txtTimKiem.Clear();
            lblNgayTao.Text = "Ngày tạo: ...";
            LoadData();
        }

        // Nút Thêm Mới
        private async Task BtnThem_ClickAsync()
        {
            if (string.IsNullOrWhiteSpace(txtTenTL.Text))
            {
                MessageBox.Show("Tên thể loại không được bỏ trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra trùng tên
            bool isDuplicate = dbContext.TheLoaiSaches.Any(t => t.TenTheLoai.ToLower() == txtTenTL.Text.Trim().ToLower());
            if (isDuplicate)
            {
                MessageBox.Show("Tên thể loại đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newTheLoai = new TheLoaiSach
            {
                TenTheLoai = txtTenTL.Text.Trim(),
                MoTa = txtMoTa.Text.Trim(),
                SoLuongSach = 0, // Mặc định theo yêu cầu DB
                NgayTao = DateTime.Now
            };

            dbContext.TheLoaiSaches.Add(newTheLoai);
            await dbContext.SaveChangesAsync();

            MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData(); // Làm mới DataGridView ngay lập tức
        }

        // Nút Cập Nhật (Sửa)
        private async Task BtnSua_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaTL.Text)) return;

            if (string.IsNullOrWhiteSpace(txtTenTL.Text))
            {
                MessageBox.Show("Tên thể loại không được bỏ trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = int.Parse(txtMaTL.Text);

            // Kiểm tra trùng tên (nhưng bỏ qua chính nó)
            bool isDuplicate = dbContext.TheLoaiSaches.Any(t => t.TenTheLoai.ToLower() == txtTenTL.Text.Trim().ToLower() && t.MaTl != id);
            if (isDuplicate)
            {
                MessageBox.Show("Tên thể loại đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tlSua = dbContext.TheLoaiSaches.Find(id);
            if (tlSua != null)
            {
                tlSua.TenTheLoai = txtTenTL.Text.Trim();
                tlSua.MoTa = txtMoTa.Text.Trim();

                await dbContext.SaveChangesAsync();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        // Nút Xóa
        private async Task BtnXoa_ClickAsync()
        {
            if (string.IsNullOrEmpty(txtMaTL.Text)) return;

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa thể loại này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    int id = int.Parse(txtMaTL.Text);
                    var tlXoa = dbContext.TheLoaiSaches.Find(id);
                    if (tlXoa != null)
                    {
                        dbContext.TheLoaiSaches.Remove(tlXoa);
                        await dbContext.SaveChangesAsync();
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BtnLamMoi_Click(null, null); // Reset form và reload
                    }
                }
                catch (DbUpdateException)
                {
                    // Bắt lỗi ràng buộc (nếu bảng này có quan hệ khóa ngoại với bảng Sách sau này)
                    MessageBox.Show("Không thể xóa thể loại này vì đang có sách tham chiếu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Nút Tìm Kiếm
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim().ToLower();

            // Tìm bằng LINQ (Where + Contains) theo tên thể loại
            var query = dbContext.TheLoaiSaches
                .Where(t => t.TenTheLoai.ToLower().Contains(keyword))
                .Select(t => new
                {
                    Mã_TL = t.MaTl,
                    Tên_Thể_Loại = t.TenTheLoai,
                    Mô_Tả = t.MoTa,
                    Số_Lượng_Sách = t.SoLuongSach,
                    Ngày_Tạo = t.NgayTao
                }).ToList();

            dgvTheLoai.DataSource = query; // Hiển thị trực tiếp lên DataGridView mà không tải lại toàn Form
        }
    }
}