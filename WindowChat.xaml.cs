using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace OpenAI_API {
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

            ComboBox1.ItemsSource = new List<string>(){
                "Ada","Babbage","Curie","Davinci","TextAdaV1","TextBabbageV1","TextCurieV1","TextDavinciV1","TextDavinciV2","TextDavinciV3",
                "CurieInstructBeta","DavinciInstructBeta","CurieSimilarityFast","TextSimilarityAdaV1","TextSimilarityBabbageV1","TextSimilarityCurieV1",
                "TextSimilarityDavinciV1","TextSearchAdaDocV1","TextSearchBabbageDocV1","TextSearchCurieDocV1","TextSearchDavinciDocV1",
                "TextSearchAdaQueryV1","TextSearchBabbageQueryV1","TextSearchCurieQueryV1","TextSearchDavinciQueryV1","TextEditDavinciV1",
                "CodeEditDavinciV1","CodeSearchAdaCodeV1","CodeSearchBabbageCodeV1","CodeSearchAdaTextV1","CodeSearchBabbageTextV1","CodeDavinciV1","CodeCushmanV1","CodeDavinciV2"
            };

            ComboBox1.SelectedIndex = 9;


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
            OpenAIService service = new OpenAIService(new OpenAiOptions() { ApiKey = initrr.OPENAPI_TOKEN});
            CompletionCreateRequest createRequest = new CompletionCreateRequest()
            {
                User = "sally1",
                Prompt = TextBox_input.Text.Trim(),
                Temperature = 0.8f,
                MaxTokens = 2048,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0.6f,
                //Stop = "停止stop",
                N = 1
                
                

            };
            try
            {
                var a = ComboBox1.SelectedIndex;
  
                var c = Models.EnumToString((Models.Model)a);

               var res = await service.Completions.CreateCompletion(createRequest, c);
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
            catch (Exception)
            {
                TextBox_return.Text = "出错了";
                this.Button_OK.Content = "发送";
            }
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
