
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
            var xuatKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.DeliverInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              xuatKho = g.Sum(x => x.Count)
                          };

            var nhapKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.ReceiveInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              nhapKho = g.Sum(x => x.Count)
                          };

            var tonKho = from x in xuatKho
                         join y in nhapKho on x.Id equals y.Id
                         select new
                         {
                             x.Id,
                             x.DisplayName,
                             tonKho = y.nhapKho - x.xuatKho
                         };
            //products = tonKho.ToList();
            lvList.ItemsSource = tonKho.Where(tk => tk.tonKho>0).ToList();
        }
        private void LoadTotal()
        {
            var xuatKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.DeliverInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              xuatKho = g.Sum(x => x.Count)
                          };
            txtHangXuat.Text = xuatKho.Sum(x => x.xuatKho).ToString();
            var nhapKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.ReceiveInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              nhapKho = g.Sum(x => x.Count)
                          };
            txtHangNhap.Text = nhapKho.Sum(x => x.nhapKho).ToString();
            var tonKho = from x in xuatKho
                         join y in nhapKho on x.Id equals y.Id
                         select new
                         {
                             x.Id,
                             x.DisplayName,
                             tonKho = y.nhapKho - x.xuatKho
                         };
            txtHangTonKho.Text = tonKho.Sum(x => x.tonKho).ToString();
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
            var xuatKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.DeliverInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              xuatKho = g.Sum(x => x.Count)
                          };

            var nhapKho = from p in PrnContext.INSTANCE.Products
                          from ri in PrnContext.INSTANCE.ReceiveInfos.Where(di => di.IdProduct == p.Id).DefaultIfEmpty()
                          group new { p.Id, p.DisplayName, ri.Count } by new { p.Id, p.DisplayName } into g
                          select new
                          {
                              g.Key.Id,
                              g.Key.DisplayName,
                              nhapKho = g.Sum(x => x.Count)
                          };

            var tonKho = from x in xuatKho
                         join y in nhapKho on x.Id equals y.Id
                         select new
                         {
                             x.Id,
                             x.DisplayName,
                             tonKho = y.nhapKho - x.xuatKho
                         };
            lvList.ItemsSource = tonKho.Where(tk => tk.DisplayName.Contains(name)).ToList();
        }
    }
}