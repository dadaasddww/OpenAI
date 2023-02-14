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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.GPT3.Extensions;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels.ImageResponseModel;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using System.Reflection.Metadata;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using System.IO;
using System.Net.Http;

namespace OpenAI_API
{
    /// <summary>
    /// Window_Image_generation.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Image_generation : Page
    {
        int Index = 0;
        public Window_Image_generation()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //initrr.main.Visibility = Visibility.Hidden;
        }

        private void Button_down_Click(object sender, RoutedEventArgs e)
        {
            if (initrr.ImageUrl.Count > 0 && Index < initrr.ImageUrl.Count - 1)
            {
                Index++;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(initrr.ImageUrl[Index]);
                bi3.EndInit();
                Image1.Source = bi3;
            }
        }

        private void Button_up_Click(object sender, RoutedEventArgs e)
        {
            if (initrr.ImageUrl.Count > 0 && Index > 0)
            {
                Index--;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(initrr.ImageUrl[Index]);
                bi3.EndInit();
                Image1.Source = bi3;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            ////Window_Image_generation
            initrr.ImageServicePrompt = TextBox_input.Text;
            OpenAIImageService1();

        }

        void s_sss()
        {
            if (initrr.ImageUrl.Count > 0)
            {
                initrr.ImageUrl.ForEach(async delegate (string c)
                {

                    HttpClient httpClient = new HttpClient();
                    using (HttpResponseMessage response = await httpClient.GetAsync(c))
                    using (Stream fileStream = await response.Content.ReadAsStreamAsync())
                    using (Stream fileToSave = System.IO.File.OpenWrite($"D:\\images\\image{c.Substring(c.Length - 8).Replace("%", "_").Replace("\\", "_").Replace("/", "_").Replace("?", "_").Replace("|", "_").Replace("<", "_").Replace(">", "_")}.jpg"))
                    {
                        await fileStream.CopyToAsync(fileToSave);
                    };

                });
                Index = 0;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(initrr.ImageUrl[Index]);
                bi3.EndInit();
                Image1.Source = bi3;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("出错，请再点击生成");
            }
        }

        /// <summary>
        /// 文字生成图片
        /// </summary>
        private async void OpenAIImageService1()
        {
            this.Button_OK.Content = "生成中……";
            OpenAIService service = new OpenAIService(new OpenAiOptions() { ApiKey = initrr.OPENAPI_TOKEN });

            var res = await service.Image.CreateImage(new ImageCreateRequest()
            {
                Prompt = TextBox_input.Text,
                N = 10,
                Size = StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                User = "sally"
            });
            
            if (res.Successful)
            {
                initrr.ImageUrl = res.Results.Select(r => r.Url).ToList();
                s_sss();
            }
            this.Button_OK.Content = "生成";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (initrr.main != null && initrr.main.IsVisible == false)
            {
                initrr.main.Visibility = Visibility.Visible;
                //1、设置窗体的Visibility属性：
                //Visibility = "Visible"：显示窗体
                //Visibility = "Collapsed"：不显示窗体
                //2、调用Show()和Hide()方法：
                //Show()：显示窗体
                //Hide()：不显示窗体
            }
        }
    }
    public static class initrr
    {
        public const string OPENAPI_TOKEN = "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";//输入自己的api-key
        public static string ImageServicePrompt = "";
        public static List<string> ImageUrl = new List<string>();
        public static Main main;

    }

}
