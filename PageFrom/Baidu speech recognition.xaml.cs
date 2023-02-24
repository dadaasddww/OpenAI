using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using Baidu.Aip;
namespace OpenAI_API.PageFrom {
    /// <summary>
    /// Baidu_speech_recognition.xaml 的交互逻辑
    /// </summary>
    public partial class Baidu_speech_recognition : Page 
    {
        public Baidu_speech_recognition() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            // 设置APPID/AK/SK
            var APP_ID = "6647681";
            var API_KEY = "aRQVWDZOWw02hycCVvDoMCD8";
            var SECRET_KEY = "45b9fab182b10a3e871b1a9eb5bfd36d";

            var client = new Baidu.Aip.Speech.Asr(APP_ID, API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            switch ((sender as System.Windows.Controls.Button).Name) {
                case "Button_音频转写":

                    var data = File.ReadAllBytes("");
                    //https://cloud.baidu.com/product/speech/realtime_asr
                    // 可选参数
                    var options = new Dictionary<string, object>
                    {        {"dev_pid", 1537}     };
                    client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
                    var result = client.Recognize(data, "pcm", 16000, options);



                    break;
            }



        }
    }
}
