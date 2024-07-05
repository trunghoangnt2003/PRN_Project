using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
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

namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for ReceiveWindow.xaml
    /// </summary>
    public partial class ReceiveInfoWindow : Window
    {
        private Receive _receive;
      
        public ReceiveInfoWindow(Receive receive)
        {
            InitializeComponent();
            _receive = receive;
            txtIdPhieu.Text = "Id Phiếu Nhập Hàng : "+receive.Id;
            
            cbProduct.ItemsSource = PrnContext.INSTANCE.Products.ToList();
            cbProduct.SelectedValuePath = "Id";
            cbProduct.DisplayMemberPath = "DisplayName";
            LoadData();
            
        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.ReceiveInfos.Include(ri => ri.IdProductNavigation).Where(ri => ri.IdReceive == _receive.Id).ToList();
        }
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if(cbProduct.SelectedItem is Product select)
            {
                try
                {
                    ReceiveInfo receiveInfo = new ReceiveInfo();
                    receiveInfo.IdProduct = select.Id;
                    receiveInfo.InputPrice = Int32.Parse(txtGianhap.Text);
                    receiveInfo.Count = Int32.Parse(txtSoluong.Text);
                    receiveInfo.IdReceive = _receive.Id;
                    PrnContext.INSTANCE.ReceiveInfos.Add(receiveInfo);
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }catch (Exception ex)
                {
                    MessageBox.Show("Lỗi định dạng trong quá trình thêm!");
                }
                
            }else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm");
            }
        }
    }
}
