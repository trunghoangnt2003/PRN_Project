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
            InitializeComponent();
            txtUser.Text = user.DisplayName;
            LoadData();
            _user = user;   
        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.Receives.Include(r=>r.IdUserNavigation).ToList();
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
            receive.DateInput =DateOnly.Parse( txtDate.Text );
            receive.IdUser = _user.Id;
            PrnContext.INSTANCE.Receives.Add( receive );
            PrnContext.INSTANCE.SaveChanges();
            LoadData();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
