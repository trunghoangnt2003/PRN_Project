using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            LoadData();
            LoadRole();
        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.Users.Include(role => role.IdRoleNavigation).ToList();
        }
        private void LoadRole()
        {
            cbRole.ItemsSource = PrnContext.INSTANCE.UserRoles.ToList();
            cbRole.DisplayMemberPath = "DisplayName";
            cbRole.SelectedItem = "Id";
        }

        private void lvList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvList.SelectedItem is User selected)
            {
                cbRole.SelectedValue = selected.IdRoleNavigation.Id;
            }
            else
            {
                cbRole.SelectedValue = null;
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            string displayName = txtDisplayName.Text;
            string userName = txtUserName.Text;
            string password = PasswordHelper.HashPasswordSHA1("123");
            int idRole = (int)cbRole.SelectedValue;
            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Không được nhập tên rỗng ! ");
            }
            else
            {
                User user = new User();
                user.UserName = userName;
                user.Password = password;
                user.IdRole = idRole;
                user.DisplayName = displayName;
                PrnContext.INSTANCE.Users.Add(user);
                PrnContext.INSTANCE.SaveChanges();
                LoadData();
            }
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            string displayName = txtDisplayName.Text;
            string userName = txtUserName.Text;
            int idRole = (int)cbRole.SelectedValue;
            if (lvList.SelectedItem is User selected)
            {
                selected.DisplayName = displayName;
                selected.IdRole = idRole;
                selected.UserName = userName;
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
