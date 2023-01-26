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
namespace OpenAI_API
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            initrr.main = this;
        }

        private void Button_chat_Click(object sender, RoutedEventArgs e)
        {
           
            var c = new WindowChat();
            
            c.ShowDialog();
            
        }

        private void Button_image_Click(object sender, RoutedEventArgs e)
        {
           
            var c = new Window_Image_generation();
            
            c.ShowDialog();
            
        }
    }
}
