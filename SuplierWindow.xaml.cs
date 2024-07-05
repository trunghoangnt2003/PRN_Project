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

    public partial class SuplierWindow : Window
    {
        public SuplierWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            var list = PrnContext.INSTANCE.Supliers.ToList();
            lvList.ItemsSource = list;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                string displayName = txtDisplayName.Text;
                string address = txtAddress.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string moreInfo = txtMoreInfo.Text;
                DateOnly date = DateOnly.Parse(txtContractDate.Text);
                Suplier suplier = new Suplier();
                suplier.DisplayName = displayName;
                suplier.Address = address;
                suplier.Phone = phone;
                suplier.Email = email;
                suplier.MoreInfo = moreInfo;
                suplier.ContractDate = date;
                PrnContext.INSTANCE.Supliers.Add(suplier);
                PrnContext.INSTANCE.SaveChanges();
                LoadData();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ngày tháng năm không đúng định dạng");
            }
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                string displayName = txtDisplayName.Text;
                string address = txtAddress.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string moreInfo = txtMoreInfo.Text;
                DateOnly date = DateOnly.Parse(txtContractDate.Text);
                if (lvList.SelectedItem is Suplier selected)
                {
                    selected.DisplayName = displayName;
                    selected.Address = address;
                    selected.Phone = phone;
                    selected.Email = email;
                    selected.MoreInfo = moreInfo;
                    selected.ContractDate = date;
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
            if (lvList.SelectedItem is Suplier selected)
            {
                PrnContext.INSTANCE.Supliers.Remove(selected);
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
