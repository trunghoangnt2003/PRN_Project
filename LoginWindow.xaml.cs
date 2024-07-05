using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public User UserLogin { get; set; } = null;
        public LoginWindow()
        {
            InitializeComponent();
        }
        
        private void Button_ClickLogin(object sender, RoutedEventArgs e)
        {
            
            string userName = txtUser.Text.Trim();
            string passWord = PasswordHelper.HashPasswordSHA1(txtPassWord.Password.Trim());
            UserLogin = PrnContext.INSTANCE.Users.FirstOrDefault((user) => user.UserName.Equals(userName) && user.Password.Equals(passWord));
            if (UserLogin != null)
            {
                MainWindow mainWindow = new MainWindow(UserLogin);
                mainWindow.Show();
                this.Close();

            }
            else
            {
                MessageBox.Show("Sai : " + passWord);
                Console.WriteLine(passWord);

            }


        }
    }
}
