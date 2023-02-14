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
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3;
using System.Speech.Synthesis;

namespace OpenAI_API
{
    /// <summary>
    /// WindowChat.xaml 的交互逻辑
    /// </summary>
    public partial class WindowChat : Page
    {
        SpeechSynthesizer synthesizer;
        public WindowChat()
        {
            InitializeComponent();
            TextBox_input.Focus();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //initrr.main.Visibility = Visibility.Hidden;
            synthesizer = new SpeechSynthesizer();
            synthesizer.Rate = 2;//-10到10
            var tts1 = synthesizer.GetInstalledVoices();

        }

        private void Button_clean_Click(object sender, RoutedEventArgs e)
        {
            TextBox_return.Text = "";
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Chat();
        }
        /// <summary>
        /// 对话
        /// </summary>
        private async void Chat()
        {
            this.Button_OK.Content = "发送中……";
            //https://www.cnblogs.com/qsnn/p/17058303.html
            OpenAIService service = new OpenAIService(new OpenAiOptions() { ApiKey = initrr.OPENAPI_TOKEN });
            CompletionCreateRequest createRequest = new CompletionCreateRequest()
            {
                Prompt = TextBox_input.Text.Trim(),
                Temperature = 0.3f,
                MaxTokens = 2048,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0.6f,
                //Stop = "停止stop",
                N = 1
            };

            var res = await service.Completions.CreateCompletion(createRequest, Models.TextDavinciV3);

            if (res.Successful)
            {
                TextBox_return.Text = res.Choices.FirstOrDefault().Text;
            }
            else

            {
                System.Windows.Forms.MessageBox.Show("生成错误");
            }
            this.Button_OK.Content = "发送";
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

        private void TextBox_input_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    // 在此处添加回车键按下时要执行的代码
            //    Chat();
            //}
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            synthesizer.SetOutputToDefaultAudioDevice();

            this.Cursor = System.Windows.Input.Cursors.Wait;
            synthesizer.SpeakAsync(TextBox_return.Text);
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void MenuItem_停止_Click(object sender, RoutedEventArgs e)
        {
            if (synthesizer.State == SynthesizerState.Speaking)
            {
                synthesizer.Pause();
            }

        }
    }
}
