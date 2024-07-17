using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private string _base64ImageString = null;
        private string img = null;

        private string _selectedQrcode;

        public ProductWindow()
        {
            InitializeComponent();
            cbUnit.ItemsSource = PrnContext.INSTANCE.Units.ToList();
            cbUnit.SelectedValuePath = "Id";
            cbUnit.DisplayMemberPath = "DisplayName";

            cbSuplier.ItemsSource = PrnContext.INSTANCE.Supliers.ToList();
            cbSuplier.SelectedValuePath = "Id";
            cbSuplier.DisplayMemberPath = "DisplayName";

            cbTimkiem.ItemsSource = PrnContext.INSTANCE.Supliers.ToList();
            cbTimkiem.SelectedValuePath = "Id";
            cbTimkiem.DisplayMemberPath = "DisplayName";
            LoadData();
        }

        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.Products.Include(p => p.IdUnitNavigation).Include(p => p.IdSuplierNavigation).ToList();
        }
        private void UploadImage(object sender, RoutedEventArgs e)
        {
            // Hiển thị một OpenFileDialog để người dùng chọn ảnh
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                PreviewImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                //// Đọc dữ liệu ảnh từ file
                byte[] imageData;
                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, (int)fs.Length);
                }
                string imageDataBase64 = Convert.ToBase64String(imageData);
                img = imageDataBase64;

            }
        }
        private void UpdatePreviewImage(string qrcodeBase64)
        {
            try
            {
                // Convert the Base64 string to a byte array
                byte[] imageBytes = Convert.FromBase64String(qrcodeBase64);

                // Create a BitmapImage from the byte array
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();

                // Set the Image control's Source
                PreviewImage.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the image conversion
                Console.WriteLine($"Error updating preview image: {ex.Message}");
            }
        }

        private void lvList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvList.SelectedItem is Product selected)
            {
                cbUnit.SelectedValue = selected.IdUnitNavigation.Id;
                cbSuplier.SelectedValue = selected.IdSuplierNavigation.Id;
                UpdatePreviewImage(selected.Qrcode);
            }
            else
            {
                cbSuplier.SelectedValue = null;
                cbUnit.SelectedValue = null;
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            string displayName = txtDisplayName.Text;
            int unitId = (int)cbUnit.SelectedValue;
            int suplierId = (int)cbSuplier.SelectedValue;
            Product product = new Product();
            product.DisplayName = displayName;
            product.IdSuplier = suplierId;
            product.IdUnit = unitId;
            product.Qrcode = img;
            product.OutPrice = Int32.Parse(txtGia.Text);
            PrnContext.INSTANCE.Products.Add(product);
            PrnContext.INSTANCE.SaveChanges();
            LoadData();
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            string displayName = txtDisplayName.Text;
            int unitId = (int)cbUnit.SelectedValue;
            int suplierId = (int)cbSuplier.SelectedValue;
            if (lvList.SelectedItem is Product selected)
            {
                selected.DisplayName = displayName;
                selected.IdSuplier = suplierId;
                selected.IdUnit = unitId;
                selected.OutPrice = Int32.Parse(txtGia.Text);
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
                if (lvList.SelectedItem is Product selected)
                {
                    var DeliverInfo = PrnContext.INSTANCE.DeliverInfos.Where(x => x.IdProductNavigation.Id == selected.Id).ToList();
                    PrnContext.INSTANCE.DeliverInfos.RemoveRange(DeliverInfo);

                    var ReceiveInfo = PrnContext.INSTANCE.ReceiveInfos.Where(x => x.IdProductNavigation.Id == selected.Id).ToList();
                    PrnContext.INSTANCE.ReceiveInfos.RemoveRange(ReceiveInfo);
                    PrnContext.INSTANCE.Products.Remove(selected);
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình xóa");
                }

            }
        }

        private void Button_Click_Timkiem(object sender, RoutedEventArgs e)
        {
            var result = PrnContext.INSTANCE.Products.ToList();
            string nameProduct = txtTimkiem.Text;
            if (!string.IsNullOrEmpty(nameProduct))
            {
                result = result.Where(p => p.DisplayName.IndexOf(nameProduct, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            }

            if (result != null && cbTimkiem.SelectedValue != null)
            {
                int suplierId = (int)cbTimkiem.SelectedValue;
                result = result.Where(p => p.IdSuplier == suplierId).ToList();
            }

            lvList.ItemsSource = result;
            txtKetqua.Text = "Tìm kiếm được " + result.Count + " kết quả trả về!";


        }

        private void Button_Click_X(object sender, RoutedEventArgs e)
        {
            txtKetqua.Text = "";
            cbTimkiem.Text = "";
            txtTimkiem.Text = "";
            LoadData();
        }
    }
}
