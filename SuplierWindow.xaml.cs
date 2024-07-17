using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PRN_Project
{

    public partial class SuplierWindow : Window
    {
        public class SupplierDto
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string MoreInfo { get; set; }
            public DateOnly ContractDate { get; set; }
        }

        public SuplierWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            var list = PrnContext.INSTANCE.Supliers.Include(X => X.Products).ToList();
            lvList.ItemsSource = list;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                string displayName = txtDisplayName.Text;
                string address = txtAddress.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string moreInfo = txtMoreInfo.Text;
                DateOnly date = DateOnly.Parse(txtContractDate.Text);
                Suplier suplier = new Suplier();
                suplier.DisplayName = displayName;
                suplier.Address = address;
                suplier.Phone = phone;
                suplier.Email = email;
                suplier.MoreInfo = moreInfo;
                suplier.ContractDate = date;
                PrnContext.INSTANCE.Supliers.Add(suplier);
                PrnContext.INSTANCE.SaveChanges();
                LoadData();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ngày tháng năm không đúng định dạng");
            }
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                string displayName = txtDisplayName.Text;
                string address = txtAddress.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string moreInfo = txtMoreInfo.Text;
                DateOnly date = DateOnly.Parse(txtContractDate.Text);
                if (lvList.SelectedItem is Suplier selected)
                {
                    selected.DisplayName = displayName;
                    selected.Address = address;
                    selected.Phone = phone;
                    selected.Email = email;
                    selected.MoreInfo = moreInfo;
                    selected.ContractDate = date;
                    if (PrnContext.INSTANCE.Entry(selected).State != EntityState.Modified)
                    {
                        PrnContext.INSTANCE.Entry(selected).State = EntityState.Modified;
                    }
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình cập nhật");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ngày tháng năm không đúng định dạng");
            }
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đối tượng này không?",
                                                      "Xác nhận xóa",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);

            // Kiểm tra kết quả của hộp thoại
            if (result == MessageBoxResult.Yes)
            {
                // Thực hiện hành động xóa đối tượng
                if (lvList.SelectedItem is Suplier selected)
                {
                    var product = PrnContext.INSTANCE.Products.Where(x => x.IdSuplier == selected.Id).ToList();
                    if (product.Count > 0)
                    {
                        MessageBox.Show("Còn sản phẩm trong kho ! Ngưng tiến trình xóa");
                        return;
                    }
                    PrnContext.INSTANCE.Supliers.Remove(selected);
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình xóa");
                }

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                // Tạo danh sách DTO từ danh sách Supplier
                List<SupplierDto> supplierDtoList = PrnContext.INSTANCE.Supliers.Select(s => new SupplierDto
                {
                    Id = s.Id,
                    DisplayName = s.DisplayName,
                    Address = s.Address,
                    Phone = s.Phone,
                    Email = s.Email,
                    MoreInfo = s.MoreInfo,
                    ContractDate = s.ContractDate
                }).ToList();

                string jsonData = JsonSerializer.Serialize(supplierDtoList, options);

                // Tạo SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                saveFileDialog.FileName = "Supliers.json";

                // Hiển thị SaveFileDialog và lấy kết quả
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    File.WriteAllText(fileName, jsonData);
                    // Thông báo cho người dùng biết file đã được lưu thành công
                    MessageBox.Show($"File đã được lưu tại: {fileName}");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show($"Đã có lỗi xảy ra: {ex.Message}");
            }
        }
        private void ReadJsonFromFile()
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string jsonFilePath = openFileDialog.FileName;
                    string jsonData = File.ReadAllText(jsonFilePath);

                    // Chuyển đổi dữ liệu JSON thành danh sách SupplierDto
                    var jsonElement = JsonDocument.Parse(jsonData).RootElement;
                    var supplierDtos = jsonElement.GetProperty("$values").Deserialize<List<SupplierDto>>();


                    // Thêm dữ liệu vào database
                    foreach (var supplier in supplierDtos)
                    {
                        var newSupplier = new Suplier
                        {
                            DisplayName = supplier.DisplayName,
                            Address = supplier.Address,
                            Phone = supplier.Phone,
                            Email = supplier.Email,
                            MoreInfo = supplier.MoreInfo,
                            ContractDate = supplier.ContractDate
                        };

                        PrnContext.INSTANCE.Supliers.Add(newSupplier);
                    }

                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadJsonFromFile();
        }
    }
    
}
