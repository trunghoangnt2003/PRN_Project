using PRN_Project.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;


namespace PRN_Project
{
    public class TonKho{
        public int Id;
        public string DisplayName;
        public int tonKho;
    }
    /// <summary>
    /// Interaction logic for DeliverWindow.xaml
    /// </summary>
    public partial class DeliverInfoWindow : Window
    {
        private Deliver _deliver;
        public DeliverInfoWindow(Deliver deliver)
        {
            InitializeComponent();
           
            _deliver = deliver;
            txtIdPhieu.Text = "Id Phiếu Bán Hàng : " + _deliver.Id;

            cbProduct.ItemsSource = PrnContext.INSTANCE.Products.ToList();
            cbProduct.SelectedValuePath = "Id";
            cbProduct.DisplayMemberPath = "DisplayName";
            LoadData();

        }
        private void LoadData()
        {
            lvList.ItemsSource = PrnContext.INSTANCE.DeliverInfos.Where(de => de.IdDeliver == _deliver.Id).ToList();
            txtTongTien.Text = "Tổng tiền xuất hàng : " + PrnContext.INSTANCE.DeliverInfos.Where(de => de.IdDeliver == _deliver.Id).Sum(de => de.OutputPrice * de.Count);

            if (_deliver.Status == true)
            {
                btn1.Visibility = Visibility.Collapsed;
                btn2.Visibility = Visibility.Collapsed;
                btn3.Visibility = Visibility.Collapsed;
                btn4.Visibility = Visibility.Collapsed;
            }
        }
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbProduct.SelectedItem is Product select)
                {



                    if (select.Count < Int32.Parse(txtSoluong.Text))
                    {
                        MessageBox.Show("Sản phẩm này hiện tại chỉ còn " + select.Count + " sản phẩm trong kho");
                        return;
                    }


                    try
                    {
                        DeliverInfo receiveInfo = new DeliverInfo();
                        receiveInfo.IdProduct = select.Id;
                        receiveInfo.OutputPrice = Int32.Parse(txtGiaxuat.Text);
                        receiveInfo.Count = Int32.Parse(txtSoluong.Text);
                        receiveInfo.IdDeliver = _deliver.Id;
                        PrnContext.INSTANCE.DeliverInfos.Add(receiveInfo);
                        PrnContext.INSTANCE.SaveChanges();
                        LoadData();
                        MessageBox.Show("Thêm sản phẩm thành công vào phiếu xuất hàng");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi định dạng trong quá trình thêm!");
                    }

                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm");
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //private void PrintInvoice_Click(object sender, RoutedEventArgs e)
        //{
        //    var list = lvList.ItemsSource as List<DeliverInfo>;
        //    // Tạo một PrintDialog để cho phép người dùng chọn máy in
        //    PrintDialog printDialog = new PrintDialog();

        //    if (printDialog.ShowDialog() == true)
        //    {
        //        // Tạo một FlowDocument mới để chứa nội dung cần in
        //        FlowDocument doc = new FlowDocument();

        //        // Tạo một Paragraph chứa các dòng trong hóa đơn
        //        Paragraph paragraph = new Paragraph();

        //        // Thêm các dòng vào Paragraph
        //        paragraph.Inlines.Add(new Run($"Nhân viên in : {_deliver.IdUserNavigation.DisplayName}"));
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new Run($"Ngày in : {new DateOnly().ToString("dd/MM/yyyy")}"));
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new Run($"Sản phẩm - Số lượng - Giá tiền"));
        //        paragraph.Inlines.Add(new LineBreak());
        //        foreach (DeliverInfo de in list)
        //        {
        //            paragraph.Inlines.Add(new Run(de.IdProductNavigation.DisplayName + "  -  SL " + de.Count + "  -  " + de.OutputPrice));
        //            paragraph.Inlines.Add(new LineBreak());
        //        }
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new Run($"Tổng số tiền: {list.Sum(de => de.OutputPrice * de.Count)}"));

        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new LineBreak());
        //        paragraph.Inlines.Add(new Run($"Chữ kí"));

        //        // Thêm Paragraph vào FlowDocument
        //        doc.Blocks.Add(paragraph);

        //        // In FlowDocument
        //        printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Invoice Printing");
        //    }

        //}

        private void PrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = lvList.ItemsSource as List<DeliverInfo>;
            for (int i = 0; i < list.Count; i++)
            {
                var product = PrnContext.INSTANCE.Products.Where(x => x.Id == list[i].IdProduct).FirstOrDefault();
                if (product.Count < list[i].Count)
                {
                    MessageBox.Show("Sản phẩm :" + product.DisplayName + "( ID : " + product.Id + ") hiện tại còn " + product.Count + " sản phẩm");
                    return;
                }
            }
            // Create a PrintDialog
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Create a FixedDocument
                FixedDocument fixedDoc = new FixedDocument();

                // Add a PageContent
                PageContent pageContent = new PageContent();
                FixedPage fixedPage = new FixedPage();
                pageContent.Child = fixedPage;
                fixedDoc.Pages.Add(pageContent);

                // Add content to FixedPage
                AddInvoiceContent(fixedPage);

                // Send FixedDocument to the printer
                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Invoice Printing");
                MessageBox.Show("In hóa đơn thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("In hóa đơn gặp lỗi");
            }
        }

        private void AddInvoiceContent(FixedPage fixedPage)
        {
             var list = lvList.ItemsSource as List<DeliverInfo>;

            double xPosition = 50;
            double yPosition = 50;
            AddTextBlock(fixedPage, "HÓA ĐƠN XUẤT HÀNG", xPosition, yPosition, 1);
            yPosition += 20;
            AddTextBlock(fixedPage, $"Ngày: {DateTime.Now.ToShortDateString()}", xPosition, yPosition, 0);
            yPosition += 20;

            // Add Printed By
            AddTextBlock(fixedPage, "Nhân viên :" + _deliver.IdUserNavigation.DisplayName, xPosition, yPosition, 0);
            yPosition += 50;
            AddTextBlock(fixedPage, "Sản phẩm", xPosition, yPosition,1);
            AddTextBlock(fixedPage, "Số lượng", xPosition + 150, yPosition,1);
            AddTextBlock(fixedPage, "Giá", xPosition + 300, yPosition,1);

            yPosition += 30;

            AddTextBlock(fixedPage, "-------------------------------------------------------------------", xPosition, yPosition, 1);

            yPosition += 20;
            // Add invoice items
            for (int i = 0; i < list.Count; i++)
            {
                AddTextBlock(fixedPage, list[i].IdProductNavigation.DisplayName, xPosition, yPosition,0);
                AddTextBlock(fixedPage, list[i].Count+"", xPosition + 150, yPosition,0);
                AddTextBlock(fixedPage, list[i].OutputPrice+"", xPosition + 300, yPosition,0);

                yPosition += 30;
                list[i].IdProductNavigation.Count -= list[i].Count;
                if (PrnContext.INSTANCE.Entry(list[i].IdProductNavigation).State != EntityState.Modified)
                {
                    PrnContext.INSTANCE.Entry(list[i].IdProductNavigation).State = EntityState.Modified;
                }
                PrnContext.INSTANCE.SaveChanges();
                _deliver.Status = true;
                if (PrnContext.INSTANCE.Entry(_deliver).State != EntityState.Modified)
                {
                    PrnContext.INSTANCE.Entry(_deliver).State = EntityState.Modified;
                }
                PrnContext.INSTANCE.SaveChanges();

                btn1.Visibility = Visibility.Collapsed;
                btn2.Visibility = Visibility.Collapsed;
                btn3.Visibility = Visibility.Collapsed;
                btn4.Visibility = Visibility.Collapsed; 
            }
            AddTextBlock(fixedPage, "-------------------------------------------------------------------", xPosition, yPosition, 1);


            yPosition += 20;
            AddTextBlock(fixedPage, $"Tổng tiền hóa đơn : {list.Sum(de => de.OutputPrice * de.Count)}", xPosition, yPosition, 1);
            yPosition += 50;
            AddTextBlock(fixedPage, $"Chữ kí", xPosition +300, yPosition, 0);





        }

        private void AddTextBlock(FixedPage fixedPage, string text, double x, double y, int check)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            
            if(check == 1) tb.FontWeight = FontWeights.Bold;
            else tb.FontWeight = FontWeights.Normal;
            tb.FontSize = 12;
            tb.Margin = new Thickness(x, y, 0, 0);
            fixedPage.Children.Add(tb);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đối tượng này không?",
                                          "Xác nhận xóa",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            // Kiểm tra kết quả của hộp thoại
            if (result == MessageBoxResult.Yes)
            {
                // Thực hiện hành động xóa đối tượng
                if (lvList.SelectedItem is DeliverInfo selected)
                {
                    PrnContext.INSTANCE.DeliverInfos.Remove(selected);
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình xóa");
                }

            }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try {
                if (lvList.SelectedItem is DeliverInfo receiveInfo)
                {
                    receiveInfo.IdProduct = (int)cbProduct.SelectedValue;
                    receiveInfo.OutputPrice = Int32.Parse(txtGiaxuat.Text);
                    receiveInfo.Count = Int32.Parse(txtSoluong.Text);


                    if (PrnContext.INSTANCE.Entry(receiveInfo).State != EntityState.Modified)
                    {
                        PrnContext.INSTANCE.Entry(receiveInfo).State = EntityState.Modified;
                    }
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không hợp lệ ! Ngưng tiến trình cập nhật");
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void cbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbProduct.SelectedItem is Product product)
            {
                txtGiaxuat.Text = product.OutPrice.ToString();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đối tượng này không?",
                                             "Xác nhận xóa",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                // Kiểm tra kết quả của hộp thoại
                if (result == MessageBoxResult.Yes)
                {
                    // Thực hiện hành động xóa đối tượng

                    var DeliverInfo = PrnContext.INSTANCE.DeliverInfos.Where(x => x.IdDeliver == _deliver.Id).ToList();
                    PrnContext.INSTANCE.DeliverInfos.RemoveRange(DeliverInfo);

                    PrnContext.INSTANCE.Delivers.Remove(_deliver);
                    PrnContext.INSTANCE.SaveChanges();
                    DeliverWindow deliverWindow = new DeliverWindow(_deliver.IdUserNavigation);
                    this.Close();
                    deliverWindow.Show();


                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lvList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lvList.SelectedItem is DeliverInfo receiveInfo)
                {
                    cbProduct.SelectedValue = receiveInfo.IdProduct;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
