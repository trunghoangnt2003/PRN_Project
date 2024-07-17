using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using PRN_Project.Models;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static PRN_Project.SuplierWindow;

namespace PRN_Project
{
    /// <summary>
    /// Interaction logic for ReceiveWindow.xaml
    /// </summary>
    
    
    public partial class ReceiveInfoWindow : Window
    {
        
        private Receive _receive;

        public ReceiveInfoWindow(Receive receive)
        {
            InitializeComponent();
            _receive = receive;
            txtIdPhieu.Text = "Id Phiếu Nhập Hàng : " + receive.Id;

            cbProduct.ItemsSource = PrnContext.INSTANCE.Products.ToList();
            cbProduct.SelectedValuePath = "Id";
            cbProduct.DisplayMemberPath = "DisplayName";
            LoadData();




        }
        private void LoadData()
        {
            var ri = PrnContext.INSTANCE.ReceiveInfos.Include(ri => ri.IdProductNavigation).Where(ri => ri.IdReceive == _receive.Id).ToList();
            lvList.ItemsSource = ri;
            txtTongTien.Text = "Hóa đơn nhập hàng: " + ri.Sum(ri => ri.Count * ri.InputPrice) + "";
            if (_receive.Status == true)
            {
                btn1.Visibility = Visibility.Collapsed;
                btn2.Visibility = Visibility.Collapsed;
                btn3.Visibility = Visibility.Collapsed;
                btn4.Visibility = Visibility.Visible;
                tbLoi.Visibility = Visibility.Visible;
                lbLoi.Visibility = Visibility.Visible;
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
                    try
                    {
                        ReceiveInfo receiveInfo = new ReceiveInfo();
                        receiveInfo.IdProduct = select.Id;
                        receiveInfo.InputPrice = Int32.Parse(txtGianhap.Text);
                        receiveInfo.Count = Int32.Parse(txtSoluong.Text);
                        receiveInfo.IdReceive = _receive.Id;
                        PrnContext.INSTANCE.ReceiveInfos.Add(receiveInfo);
                        PrnContext.INSTANCE.SaveChanges();
                        LoadData();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var list = lvList.ItemsSource as List<ReceiveInfo>;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = _receive.IdUserNavigation.DisplayName;
                    p.Workbook.Properties.Title = "Danh mục mặt hàng nhập kho";
                    p.Workbook.Worksheets.Add("Phieu");
                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "Phiếu nhập : " + _receive.Id;
                    ws.Cells.Style.Font.Name = "Time New Romans";
                    ws.Cells.Style.Font.Size = 13;
                    string[] arr = { "Tên sản phẩm", "Số lượng nhập", "Giá nhập" };
                    var countColHeader = arr.Count();

                    ws.Cells[1, 1].Value = "Danh sách sản phẩm nhập của phiếu " + _receive.Id;
                    ws.Cells[1, 1, 1, countColHeader].Merge = true;
                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;

                    int colIndex = 1;
                    int rowIndex = 2;

                    ws.Cells[2, 1].Value = arr[0];
                    ws.Cells[2, 2].Value = arr[1];
                    ws.Cells[2, 3].Value = arr[2];

                    for (int i = 0; i < list.Count(); i++)
                    {
                        ws.Cells[i + 3, 1].Value = list[i].IdProductNavigation.DisplayName;
                        ws.Cells[i + 3, 2].Value = list[i].Count;
                        ws.Cells[i + 3, 3].Value = list[i].InputPrice;
                    }
                    var tongSoTien = list.Sum(p => p.InputPrice * p.Count);
                    ws.Cells[list.Count + 3, 3].Value = "Tổng số tiền : ";
                    ws.Cells[list.Count + 3, 4].Value = tongSoTien;

                    ws.Cells[list.Count + 5, 4].Value = "Người thực hiện";
                    ws.Cells[list.Count + 6, 4].Value = _receive.IdUserNavigation.DisplayName;

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx",
                        FileName = "XuatPhieuSo" + _receive.Id + ".xlsx",
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var file = new FileInfo(saveFileDialog.FileName);
                        p.SaveAs(file);
                        MessageBox.Show("Xuất file thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        for (int i = 0; i < list.Count(); i++)
                        {
                            var product = list[i].IdProductNavigation;
                            if (product == null) continue;
                            product.Count += list[i].Count;
                            if (PrnContext.INSTANCE.Entry(product).State != EntityState.Modified)
                            {
                                PrnContext.INSTANCE.Entry(product).State = EntityState.Modified;
                            }
                            PrnContext.INSTANCE.SaveChanges();
                            


                        }
                        _receive.Status = true;
                        if (PrnContext.INSTANCE.Entry(_receive).State != EntityState.Modified)
                        {
                            PrnContext.INSTANCE.Entry(_receive).State = EntityState.Modified;
                        }
                        PrnContext.INSTANCE.SaveChanges();
                        btn1.Visibility = Visibility.Collapsed;
                        btn2.Visibility = Visibility.Collapsed;
                        btn3.Visibility = Visibility.Collapsed;
                    }

                }
                // Lưu file


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)          
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
                    if (lvList.SelectedItem is ReceiveInfo selected)
                    {
                        PrnContext.INSTANCE.ReceiveInfos.Remove(selected);
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {      
            try
            {
                if (lvList.SelectedItem is ReceiveInfo receiveInfo)
                {
                    receiveInfo.IdProduct = (int)cbProduct.SelectedValue;
                    receiveInfo.InputPrice = Int32.Parse(txtGianhap.Text);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvList.SelectedItem is ReceiveInfo receiveInfo)
                {
                    int countLoi = Int32.Parse(tbLoi.Text);
                    receiveInfo.CountReport = countLoi;
                    if (receiveInfo.Count < countLoi)
                    {
                        MessageBox.Show("Không được quá lượng nhập");
                        return;
                    }
                    receiveInfo.StatusReport = false;
                    if (PrnContext.INSTANCE.Entry(receiveInfo).State != EntityState.Modified)
                    {
                        PrnContext.INSTANCE.Entry(receiveInfo).State = EntityState.Modified;
                    }
                    PrnContext.INSTANCE.SaveChanges();
                    LoadData();
                    MessageBox.Show("Đã báo cáo lên quản lí !");
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Không hợp lệ !");
            }
            
        }

        private void lvList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (lvList.SelectedItem is ReceiveInfo receiveInfo)
                {
                    cbProduct.SelectedValue = receiveInfo.IdProduct;
                    if (receiveInfo.CountReport != null)
                    {
                        txtLoi.Visibility = Visibility.Visible;

                        if (receiveInfo.StatusReport == true)
                        {
                            txtLoi.Content = "Đã trả lại " + receiveInfo.CountReport + " sản phẩm";
                        }
                        else
                        {
                            txtLoi.Content = "Đã gửi yêu cầu cho quản lí";
                        }
                    }
                    else
                    {
                        txtLoi.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    MessageBox.Show("Không hợp lệ !");
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Không hợp lệ !");
            }
            
        }
    }
}
