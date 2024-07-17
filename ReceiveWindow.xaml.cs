using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for ReceiveWindow.xaml
    /// </summary>
    public partial class ReceiveWindow : Window
    {
        private User _user { get; set; }

        public ReceiveWindow(User user  )
        {
            _user = user;
            InitializeComponent();
            if (user.IdRole == 2)
            {
                lblNhapHang.Visibility = Visibility.Collapsed;
                txtNhapHang.Visibility = Visibility.Collapsed;
                btDelete.Visibility = Visibility.Collapsed;
            }
            txtUser.Text = user.DisplayName;
            LoadData();
   
        }
        private void LoadData()
        {
            if (_user.IdRole == 1)
            {
                lvList.ItemsSource = PrnContext.INSTANCE.Receives.Include(r => r.IdUserNavigation).ToList();
            }else
            {
                lvList.ItemsSource = PrnContext.INSTANCE.Receives.Include(r => r.IdUserNavigation).Where(x=>x.IdUser==_user.Id).ToList();
            }
        }

        private void lvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectItem = (Receive)lvList.SelectedItem;
            if(selectItem != null )
            {
                lvList.SelectedItem = null;
                var page = new ReceiveInfoWindow(selectItem);
                this.Close();
                page.ShowDialog();
                
            }
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try { 
            Receive receive = new Receive();
            receive.DateInput = DateOnly.FromDateTime(DateTime.Now);
                receive.IdUser = _user.Id;
            PrnContext.INSTANCE.Receives.Add( receive );
            PrnContext.INSTANCE.SaveChanges();
            LoadData();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đối tượng này không?",
                                          "Xác nhận xóa",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            // Kiểm tra kết quả của hộp thoại
            if (result == MessageBoxResult.Yes)
            {
                // Thực hiện hành động xóa đối tượng
                if (lvList.SelectedItem is Receive selected)
                {
                    var DeliverInfo = PrnContext.INSTANCE.ReceiveInfos.Where(x => x.IdReceive == selected.Id).ToList();
                    PrnContext.INSTANCE.ReceiveInfos.RemoveRange(DeliverInfo);

                    PrnContext.INSTANCE.Receives.Remove(selected);
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
