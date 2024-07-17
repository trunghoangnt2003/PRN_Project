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
    /// Interaction logic for ReportProduct.xaml
    /// </summary>
    public partial class ReportProduct : Window
    {
        public ReportProduct()
        {
            InitializeComponent();
            LoadData();
        }
        public void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.ReceiveInfos.Where(x=>x.CountReport!= null && x.StatusReport == false).ToList();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvList.SelectedItem is ReceiveInfo selected) {
                    if (selected.CountReport != null && selected.StatusReport != null)
                    {


                        selected.StatusReport = true;
                        if (PrnContext.INSTANCE.Entry(selected).State != EntityState.Modified)
                        {
                            PrnContext.INSTANCE.Entry(selected).State = EntityState.Modified;
                        }
                        PrnContext.INSTANCE.SaveChanges();
                        var product = selected.IdProductNavigation;
                        if (product.Count < selected.CountReport)
                        {
                            MessageBox.Show("Lỗi do sản phẩm không còn trong kho");
                            return;
                        }
                        product.Count -= (int)selected.CountReport;
                        if (PrnContext.INSTANCE.Entry(product).State != EntityState.Modified)
                        {
                            PrnContext.INSTANCE.Entry(product).State = EntityState.Modified;
                        }
                        PrnContext.INSTANCE.SaveChanges();
                        LoadData();
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Lỗi báo cáo để xét duyệt");
            }
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvList.SelectedItem is ReceiveInfo selected)
                {
                    selected.CountReport = null;
                    selected.StatusReport = null;
                    if (PrnContext.INSTANCE.Entry(selected).State != EntityState.Modified)
                    {
                        PrnContext.INSTANCE.Entry(selected).State = EntityState.Modified;
                    }
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi báo cáo để xét duyệt");
            }
        }
    }
}
