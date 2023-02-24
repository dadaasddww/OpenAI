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
using System.Speech.Synthesis;
using Xceed.Words.NET;
using OpenAI_API.PageFrom;

namespace OpenAI_API
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {

        public WindowChat chat;
        public Window_Image_generation Image_generation;
        public PageDOCX Docx;
        public Baidu_speech_recognition baidu;

        public Main()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            initrr.main = this;
            chat = new WindowChat();
            MainFrame.Content = chat;
        }

        private void Button_chat_Click(object sender, RoutedEventArgs e)
        {
            if (chat == null)
            {
                chat = new WindowChat();
                MainFrame.Content = chat;
            }
            else
            {
                MainFrame.Content = chat;
            }


        }

        private void Button_image_Click(object sender, RoutedEventArgs e)
        {

            if (Image_generation == null)
            {
                Image_generation = new Window_Image_generation();
                MainFrame.Content = Image_generation;
            }
            else
            {
                MainFrame.Content = Image_generation;
            }


        }

        private void Button_Docx_Click(object sender, RoutedEventArgs e)
        {
            if (Docx == null)
            {
                Docx = new PageDOCX();
                MainFrame.Content = Docx;
            }
            else
            {
                MainFrame.Content = Docx;
            }
        }

        private void Button_Baidu_Click(object sender, RoutedEventArgs e) {
            if (baidu == null) {
                baidu = new Baidu_speech_recognition();
                MainFrame.Content = baidu;
            }
            else {
                MainFrame.Content = baidu;
            }
        }
    }
}
