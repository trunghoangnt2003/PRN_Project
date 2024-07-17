
using PRN_Project.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using PRN_Project.MVVM;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<object> products = null;
        public User User { get; set; }
        public MainWindow(User user)
        {
            InitializeComponent();
            LoadTotal();
            LoadData();
            User = user;
        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.Products.Where(x=>x.Count>0).ToList();
        }
        private void LoadTotal()
        {
            
            txtHangXuat.Text = PrnContext.INSTANCE.DeliverInfos.Where(x=>x.IdDeliverNavigation.Status == true).Sum(x=>x.Count).ToString();

            txtHangTra.Text = PrnContext.INSTANCE.ReceiveInfos.Where(x => x.IdReceiveNavigation.Status == true && x.StatusReport == true).Sum(x=>x.CountReport).ToString();

            txtHangNhap.Text = PrnContext.INSTANCE.ReceiveInfos.Where(x => x.IdReceiveNavigation.Status == true).Sum(x => x.Count).ToString();

            txtHangTonKho.Text = PrnContext.INSTANCE.Products.Sum(x => x.Count).ToString();
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserWindow user = new UserWindow();
            user.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UnitWindow unitWindow = new UnitWindow();
            unitWindow.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SuplierWindow supplierWindow = new SuplierWindow();
            supplierWindow.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DeliverWindow deliver = new DeliverWindow(User);
            deliver.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ReceiveWindow receiveWindow = new ReceiveWindow(User);
            receiveWindow.ShowDialog();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            txtTimkiem.Text = "";
            LoadData();
            LoadTotal();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = txtTimkiem.Text;


            var tonKho = PrnContext.INSTANCE.Products.ToList().Where(x => x.DisplayName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            lvList.ItemsSource = tonKho;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            ReportProduct reportProduct = new ReportProduct();
            reportProduct.ShowDialog();
        }
    }
}