using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
//using Xceed.Words.NET;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


namespace OpenAI_API {
    /// <summary>
    /// PageDOCX.xaml 的交互逻辑
    /// </summary>
    public partial class PageDOCX : Page {
        public const string DatabaseFilename = "find.db";
        private DispatcherTimer timer;
        public PageDOCX() {
            InitializeComponent();
            SSQlite_Load();

        }
        private void SSQlite_Load() {
            //判断目录下有没有find.db文件，如果有就读取，如果没有就创建数据库
            if (!File.Exists("find.db")) return;
            //打开连接
            SQLiteConnection cn = new SQLiteConnection("Data Source=" + DatabaseFilename);
            if (cn.State != System.Data.ConnectionState.Open) cn.Open();
            //创建一个sql命令对象
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = cn;
            //关闭同步
            cmd.CommandText = "pragma synchronous = 0";
            cmd.ExecuteNonQuery();
            //选择所有数据
            DataSet dt = new DataSet();
            SQLiteDataAdapter mAdapter = new SQLiteDataAdapter("select * from findcondition GROUP BY id", cn);
            mAdapter.Fill(dt, "条件");
            mAdapter = new SQLiteDataAdapter("SELECT * FROM findReturn GROUP BY id", cn);
            mAdapter.Fill(dt, "结果");
            List<string> listCommBox = new List<string>();

            //导入管网
            foreach (DataRow item in dt.Tables["条件"].Rows) {
                try {
                    listCommBox.Add(Convert.ToString(item["keyword"].ToString()));
                }
                catch (Exception) {
                    System.Windows.MessageBox.Show($"错误提示!\n错误数据在{item["keyword"]}……");
                    continue;
                }
            }
            ComboBox_history.ItemsSource = listCommBox;
        }

        private void SSQlite_Read(string keyword) {
            //判断目录下有没有find.db文件，如果有就读取，如果没有就创建数据库
            if (!File.Exists("find.db")) return;
            //打开连接
            SQLiteConnection cn = new SQLiteConnection("Data Source=" + DatabaseFilename);
            if (cn.State != System.Data.ConnectionState.Open) cn.Open();
            //创建一个sql命令对象
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = cn;
            //关闭同步
            cmd.CommandText = "pragma synchronous = 0";
            cmd.ExecuteNonQuery();
            //选择所有数据
            DataSet dt = new DataSet();
            SQLiteDataAdapter mAdapter = new SQLiteDataAdapter($"SELECT * FROM findcondition where keyword=\"{keyword}\"", cn);
            mAdapter.Fill(dt, "条件");
            if (dt.Tables["条件"].Rows.Count == 0) return;
            int id = Convert.ToInt32(dt.Tables["条件"].Rows[0]["id"].ToString());
            mAdapter = new SQLiteDataAdapter($"SELECT * FROM findReturn where findID={id} GROUP BY id", cn);
            mAdapter.Fill(dt, "结果");
            List<string> listCommBox = new List<string>();
            //导入管网
            foreach (DataRow item in dt.Tables["结果"].Rows) {
                try {
                    listCommBox.Add(Convert.ToString(item["fileName"].ToString()));
                }
                catch (Exception) {
                    System.Windows.MessageBox.Show($"错误提示!\n错误数据在{item["keyword"]}……");
                    continue;
                }
            }
            ListBox1.ItemsSource = listCommBox;
        }

        private void SSQlite_Write() {
            //判断目录下有没有find.db文件，如果有就读取，如果没有就创建数据库
            if (!File.Exists("find.db")) return;
            //打开连接
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + DatabaseFilename);
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            //创建一个sql命令对象
            //获取记录数
            string strCommond = string.Format("select count(*)  from {0} ", "findcondition");
            SQLiteCommand cmd1 = new SQLiteCommand(strCommond, conn);
            int RowCount = Convert.ToInt32(cmd1.ExecuteScalar());
            strCommond = string.Format("select count(*)  from {0} ", "findReturn");
            cmd1 = new SQLiteCommand(strCommond, conn);
            int RowCount1 = Convert.ToInt32(cmd1.ExecuteScalar());
            //加入数据库
            using (SQLiteTransaction tran = conn.BeginTransaction()) {
                try {
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO findcondition(id,keyword,date,count,beizhu) values (@id,@keyword,@date,@count,@beizhu)", conn)) {
                        command.Parameters.Add("id", DbType.Int32).Value = RowCount;
                        command.Parameters.Add("keyword", DbType.String).Value = TextBox_find.Text.Trim();
                        command.Parameters.Add("date", DbType.DateTime).Value = System.DateTime.Now;
                        command.Parameters.Add("count", DbType.Int32).Value = ListBox1.Items.Count;
                        command.Parameters.Add("beizhu", DbType.String).Value = null;
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    for (int i = 0; i < ListBox1.Items.Count; i++) {
                        using (SQLiteCommand command = new SQLiteCommand("INSERT INTO findReturn(id,findID,fileName,beizhu) values (@id,@findID,@fileName,@beizhu)", conn)) {
                            command.Parameters.Add("id", DbType.Int32).Value = RowCount1 + i;
                            command.Parameters.Add("findID", DbType.Int32).Value = RowCount;
                            command.Parameters.Add("fileName", DbType.String).Value = ListBox1.Items[i].ToString();
                            command.Parameters.Add("beizhu", DbType.String).Value = "";
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                    tran.Commit();
                    conn.Close();//关闭数据库
                }
                catch {
                    tran.Rollback();
                    conn.Close();//关闭数据库
                    throw;
                }

            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            switch (((System.Windows.Controls.MenuItem)sender).Header.ToString()) {
                case "加载查找历史":
                    SSQlite_Load();
                    break;
                case "保存查找记录":
                    SSQlite_Write();
                    break;

            }
        }
        private void timer_Tick(object sender, EventArgs e) {

            ClearMemory();


        }
        private void ComboBox_history_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SSQlite_Read(ComboBox_history.SelectedItem.ToString());
        }

        private void Button_查找_Click(object sender, RoutedEventArgs e) {

            //设置定时器          
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(100000);   //时间间隔为0.1秒
            timer.Tick += new EventHandler(timer_Tick);
            //开启定时器          
            timer.Start();








            // 使用LINQ查询
            //string path = @"E:\word文档";

            var files = Directory.GetFiles(TextBox_file.Text, "*.docx", SearchOption.AllDirectories);
            //var files = (from file in Directory.EnumerateFiles(path, "*.docx", SearchOption.AllDirectories) select file).ToList();
            List<string> list = new List<string>();

            // 计时
            int a = System.Environment.TickCount;
            string keyword = TextBox_find.Text;
            /*
             * *当不区分大小写时，string.IndexOf方法的效率明显高于string.Contains方法；
            *当区分大小写时，string.Contains方法的效率明显高于string.IndexOf方法；
            *如果判断的是中文，没有大小写之分，还是string.Contains方法的效率高；
            **/
            int i = 0;
            //Parallel.ForEach(files, (string fileName) => {
            //    try {
            //        DocX document = DocX.Load(fileName);
            //        if (document.Text.Contains(keyword)) // 使用Contains方法代替IndexOf方法
            //        {
            //            list.Add(fileName); // 将包含TextBox_find.Text的段落文字加入列表
            //        }
            //        document = null;

            //    }
            //    catch (Exception) {
            //        cccc.Add(fileName);
            //    }
            //    i++;
            //    //Console.WriteLine(i.ToString());

            //});





            //list = GetFilesWithKeyword(keyword).ToList();



            Parallel.ForEach(files, (string file) => {
                try {

                    using (WordprocessingDocument doc = WordprocessingDocument.Open(file, false)) {
                        MainDocumentPart mainPart = doc.MainDocumentPart;
                        Body body = mainPart.Document.Body;
                        bool t1 = false; bool t2 = false;
                        foreach (var paragraph in body.Elements<Paragraph>()) {
                            if (t1 == true) break;
                            foreach (var run in paragraph.Elements<Run>()) {
                                if (t1 == true) {
                                    break;
                                }
                                foreach (var text in run.Elements<Text>()) {
                                    if (text.Text.Contains(keyword)) {
                                        list.Add(file);
                                        t1 = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception) {
                    cccc.Add(file);

                }
                i++;
            });












            ListBox1.ItemsSource = list;
            long time = System.Environment.TickCount - a;
            //System.Windows.MessageBox.Show((time / 1000).ToString());
            Lable1.Content = $"搜索到{list.Count}个文档，耗时{(time / 1000)}秒";

        }
        public List<string> cccc = new List<string>();

        //IEnumerable<string> GetFilesWithKeyword(string keyword) {
        //    // 使用Directory.EnumerateFiles方法来返回一个可枚举的文件名序列
        //    List<string> c = new List<string>();
        //    int i = 0;
        //    foreach (var fileName in Directory.EnumerateFiles(@"E:\word文档", "*.docx", SearchOption.AllDirectories)) {
        //        try {
        //            // 使用using块来确保每个文件在使用完毕后被释放
        //            using (DocX document = DocX.Load(fileName)) {
        //                if (document.Text.Contains(keyword)) {
        //                    // 使用yield return来返回一个可枚举的文件名序列，不需要创建一个额外的列表
        //                    c.Add(fileName);
        //                }
        //            }
        //        }
        //        catch {
        //            cccc.Add(fileName);
        //        }
        //        i++;
        //    }
        //    return c;
        //}


        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                PageDOCX.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

        private void ListBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = ListBox1.SelectedItem.ToString();
                p.Start();
            }
            catch (Exception ex) {

            }
        }


    }



}




