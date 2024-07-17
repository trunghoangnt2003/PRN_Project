using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Interaction logic for UnitWindow.xaml
    /// </summary>
    public partial class UnitWindow : Window
    {
        public UnitWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.Units.Include(x=>x.Products).ToList();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            string displayName = txtDisplayName.Text;
            if (string.IsNullOrEmpty(displayName))
            {
                MessageBox.Show("Không được nhập tên rỗng ! ");
            }
            else
            {
                Unit unit = new Unit();
                unit.DisplayName = displayName;
                PrnContext.INSTANCE.Units.Add(unit);
                PrnContext.INSTANCE.SaveChanges();
                LoadData();
            }

        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                string displayName = txtDisplayName.Text;
                if (lvList.SelectedItem is Unit selected)
                {
                    selected.DisplayName = displayName;
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
                if (lvList.SelectedItem is Unit selected)
                {
                    var product = PrnContext.INSTANCE.Products.Where(x=>x.IdUnit == selected.Id).ToList();
                    if(product.Count > 0)
                    {
                        MessageBox.Show("Còn sản phẩm trong kho ! Ngưng tiến trình xóa");
                        return;
                    }
                    PrnContext.INSTANCE.Units.Remove(selected);
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình xóa");
                }

            }
            
        }
    }
}
