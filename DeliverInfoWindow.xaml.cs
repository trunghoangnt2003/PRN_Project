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
        private List<TonKho> listTonKho;
        public DeliverInfoWindow(Deliver deliver)
        {
            InitializeComponent();
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
                         select new TonKho
                         {
                             Id=x.Id,
                             DisplayName=x.DisplayName,
                             tonKho = y.nhapKho - x.xuatKho
                         };
            listTonKho = tonKho.ToList();
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
        }
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (cbProduct.SelectedItem is Product select)
            {
                foreach(var hangTonKho in listTonKho)
                {
                    if(hangTonKho.Id == select.Id)
                    {
                        if(hangTonKho.tonKho < Int32.Parse(txtSoluong.Text))
                        {
                            MessageBox.Show("Sản phẩm này hiện tại chỉ còn " + hangTonKho.tonKho + " sản phẩm trong kho");
                            return;
                        }
                    }
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
    }
}
