

//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using OpenAI.GPT3.Extensions;
//using OpenAI_API.Services;

//IHost host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
//    {
//        services.AddOpenAIService(settings => settings.ApiKey = "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");
//        //services.AddHostedService<openAiCompletionService>();
//        services.AddHostedService<OpenAIImageService>();
//    }).Build();

//host.Run();

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace OpenAI_API
//{
   
//    static class Program
//    {
//        /// <summary>
//        /// 应用程序的主入口点。
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
//            var c=new Main();
//            initrr.main = c;
//            c.ShowDialog();
            

//        }
//    }
//}
