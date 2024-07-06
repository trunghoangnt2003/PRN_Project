using Microsoft.VisualBasic.ApplicationServices;
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
using static PRN_Project.NhanVienWindow;

namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for NhanVienWindow.xaml
    /// </summary>
    public partial class NhanVienWindow : Window
    {
        public class MonthItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
        }
        List<MonthItem>  Months = new List<MonthItem>
        {
            new MonthItem { Id = 1, DisplayName = "Tháng Một" },
            new MonthItem { Id = 2, DisplayName = "Tháng Hai" },
            new MonthItem { Id = 3, DisplayName = "Tháng Ba" },
            new MonthItem { Id = 4, DisplayName = "Tháng Tư" },
            new MonthItem { Id = 5, DisplayName = "Tháng Năm" },
            new MonthItem { Id = 6, DisplayName = "Tháng Sáu" },
            new MonthItem { Id = 7, DisplayName = "Tháng Bảy" },
            new MonthItem { Id = 8, DisplayName = "Tháng Tám" },
            new MonthItem { Id = 9, DisplayName = "Tháng Chín" },
            new MonthItem { Id = 10, DisplayName = "Tháng Mười" },
            new MonthItem { Id = 11, DisplayName = "Tháng Mười Một" },
            new MonthItem { Id = 12, DisplayName = "Tháng Mười Hai" }
        };
        private Models.User user {  get; set; }

        public NhanVienWindow(Models.User user)
        {
            this.user = user;
            InitializeComponent();
            cbMonth.ItemsSource = Months;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ReceiveWindow receiveWindow = new ReceiveWindow(user);
            receiveWindow.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DeliverWindow deliverWindow = new DeliverWindow(user);
            deliverWindow.ShowDialog();
        }

        private void cbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbMonth.SelectedItem is MonthItem select)
            {
                txtXuatHang.Text = "Tổng số đơn đã xuất trong tháng "+select.Id +" là : "+ PrnContext.INSTANCE.Delivers.Where(x => x.IdUser == user.Id && x.DateOutput.Month == select.Id).Count()+" đơn";
                txtNhapHang.Text = "Tổng số đơn đã nhập trong tháng " + select.Id + " là : " + PrnContext.INSTANCE.Receives.Where(x => x.IdUser == user.Id && x.DateInput.Month == select.Id).Count()+" đơn";
            }
            
        }
    }
}
