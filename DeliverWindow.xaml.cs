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
    /// Interaction logic for DeliverWindow.xaml
    /// </summary>
    public partial class DeliverWindow : Window
    {
        private User _user { get; set; }
        public DeliverWindow(User user)
        {
            InitializeComponent();
            _user = user;
            txtUser.Text = _user.DisplayName;
            LoadData();
            if (user.IdRole == 2)
            {
                lbNhapHang.Visibility = Visibility.Collapsed;
                txtNhapHang.Visibility = Visibility.Collapsed;
                btXoa.Visibility = Visibility.Collapsed;
            }
        }
        
        public void LoadData()
        {
            if(_user.IdRole == 1)
            {
                lvList.ItemsSource = PrnContext.INSTANCE.Delivers.Include(de => de.IdUserNavigation).ToList();
            }else
            {

                lvList.ItemsSource = PrnContext.INSTANCE.Delivers.Include(de => de.IdUserNavigation).Where(x=>x.IdUser == _user.Id).ToList();

            }
            
        }

        private void lvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectItem = (Deliver)lvList.SelectedItem;
            if (selectItem != null)
            {
                lvList.SelectedItem = null;
                var page = new DeliverInfoWindow(selectItem);
                this.Close();
                page.ShowDialog();

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Deliver de = new Deliver();
            de.DateOutput = DateOnly.Parse(txtDate.Text);
            de.IdUser = _user.Id;
            PrnContext.INSTANCE.Delivers.Add(de);
            PrnContext.INSTANCE.SaveChanges();
            LoadData();
        }
    }
}
