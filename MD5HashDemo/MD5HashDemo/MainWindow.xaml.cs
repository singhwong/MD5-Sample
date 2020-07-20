using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading;
using Windows.UI.Popups;
using System.Security.Cryptography;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD5HashDemo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        //private string temporary_Md5;
        //private string temporary_path;
        private CancellationTokenSource tokenSource;
        private byte[] hashBytes;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string fileName = await FilePicker();
                if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(Md5Hash_textBox.Text))
                {
                    await new MessageDialog("md5 hash or path can not be empty!").ShowAsync();
                    return;
                }
                //if (textBox.Text.Equals(temporary_path) && Md5Hash_textBox.Text.Equals(temporary_Md5))
                //{
                //    return;
                //}
                //temporary_Md5 = Md5Hash_textBox.Text;
                //temporary_path = textBox.Text;
                Progress_ring.IsActive = true;
                tokenSource = new CancellationTokenSource();
                using (var stream = File.Open(textBox.Text, FileMode.Open))
                {
                    switch (Algorithm_options.SelectedIndex)
                    {
                        case 0:
                            using (var hashDlg = MD5.Create())
                            {
                                hashBytes = await hashDlg.ComputeHashAsync(stream, tokenSource.Token);

                            }
                            break;
                        case 1:
                            using (var hashDlg = SHA512.Create())
                            {
                                hashBytes = await hashDlg.ComputeHashAsync(stream, tokenSource.Token);
                            }
                            break;
                        default:
                            await new MessageDialog("哈希算法不能为空！").ShowAsync();
                            break;
                    }
                    Result_tb.Text = Convert.ToBase64String(hashBytes).Equals(Md5Hash_textBox.Text) ? "哈希值一致，文件完整。" :
    "哈希值不一致，文件不完整或输入有误。";
                    Hash_tb.Text = $"计算出的{(Algorithm_options.SelectedItem as ComboBoxItem).Content}哈希值：{Convert.ToBase64String(hashBytes)}";
                }
                Progress_ring.IsActive = false;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }

        }
        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource?.Dispose();
                tokenSource = null;
                Progress_ring.IsActive = false;
                await new MessageDialog("已取消验证！").ShowAsync();
            }            
        }
        private void Algorithm_options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Md5Hash_textBox.Header = $"需要提供的{(Algorithm_options.SelectedItem as ComboBoxItem).Content}哈希值";
        }
        //public string FileOpenPicker()
        //{
        //    OpenFileDialog dialog = new OpenFileDialog();
        //    dialog.Filter = "所有文件(*.*)|*.*";
        //    if (dialog.ShowDialog().Value)
        //    {
        //        return dialog.FileName;
        //    }
        //    return null;
        //}
        //public async Task<string> FilePicker()
        //{
        //    var filePicker = new FileOpenPicker();
        //    filePicker.FileTypeFilter.Add(".mp3");
        //    filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
        //    var file = await filePicker.PickSingleFileAsync();
        //    return file.Path;
    }
}
