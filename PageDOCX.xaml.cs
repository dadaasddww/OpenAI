using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Words.NET;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenAI_API
{
    /// <summary>
    /// PageDOCX.xaml 的交互逻辑
    /// </summary>
    public partial class PageDOCX : Page
    {
        public const string DatabaseFilename = "find.db";
        public PageDOCX()
        {
            InitializeComponent();
            SSQlite_Load();

        }
        private void SSQlite_Load()
        {
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
            foreach (DataRow item in dt.Tables["条件"].Rows)
            {
                try
                {
                    listCommBox.Add(Convert.ToString(item["keyword"].ToString()));
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show($"错误提示!\n错误数据在{item["keyword"]}……");
                    continue;
                }
            }
            ComboBox_history.ItemsSource = listCommBox;
        }

        private void SSQlite_Read(string keyword)
        {
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
            int id = Convert.ToInt32( dt.Tables["条件"].Rows[0]["id"].ToString());
             mAdapter = new SQLiteDataAdapter($"SELECT * FROM findReturn where findID={id} GROUP BY id", cn);
            mAdapter.Fill(dt, "结果");
            List<string> listCommBox = new List<string>();
            //导入管网
            foreach (DataRow item in dt.Tables["结果"].Rows)
            {
                try
                {
                    listCommBox.Add(Convert.ToString(item["fileName"].ToString()));
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show($"错误提示!\n错误数据在{item["keyword"]}……");
                    continue;
                }
            }
            ListBox1.ItemsSource = listCommBox;
        }

        private void SSQlite_Write()
        {
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
            using (SQLiteTransaction tran = conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO findcondition(id,keyword,date,count,beizhu) values (@id,@keyword,@date,@count,@beizhu)", conn))
                    {
                        command.Parameters.Add("id", DbType.Int32).Value = RowCount;
                        command.Parameters.Add("keyword", DbType.String).Value = TextBox_find.Text.Trim();
                        command.Parameters.Add("date", DbType.DateTime).Value = System.DateTime.Now;
                        command.Parameters.Add("count", DbType.Int32).Value = ListBox1.Items.Count;
                        command.Parameters.Add("beizhu", DbType.String).Value = null;
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    for (int i = 0; i < ListBox1.Items.Count; i++)
                    {
                        using (SQLiteCommand command = new SQLiteCommand("INSERT INTO findReturn(id,findID,fileName,beizhu) values (@id,@findID,@fileName,@beizhu)", conn))
                        {
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
                catch
                {
                    tran.Rollback();
                    conn.Close();//关闭数据库
                    throw;
                }

            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (((System.Windows.Controls.MenuItem)sender).Header.ToString())
            {
                case "加载查找历史":
                    SSQlite_Load();
                    break;
                case "保存查找记录":
                    SSQlite_Write();
                    break;
                
            }
        }
        private void ComboBox_history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SSQlite_Read(ComboBox_history.SelectedItem.ToString());
        }
        private void Button_查找_Click(object sender, RoutedEventArgs e)
        {
            // 使用LINQ查询
            //string path = @"E:\word文档";

            var files = Directory.GetFiles(TextBox_file.Text, "*.docx", SearchOption.AllDirectories);
            //var files = (from file in Directory.EnumerateFiles(path, "*.docx", SearchOption.AllDirectories) select file).ToList();
            List<string> list = new List<string>();
            System.Timers.Timer clockTimer = new System.Timers.Timer(30 * 1000); // 60秒是1分钟
            clockTimer.Elapsed += (sender, e) => { GC.Collect(); };
            clockTimer.Start();

            // 计时
            int a = System.Environment.TickCount;
            string keyword = TextBox_find.Text;
            /*
             * *当不区分大小写时，string.IndexOf方法的效率明显高于string.Contains方法；
            *当区分大小写时，string.Contains方法的效率明显高于string.IndexOf方法；
            *如果判断的是中文，没有大小写之分，还是string.Contains方法的效率高；
            **/

            Parallel.ForEach(files, async (string fileName) =>
            {
                try
                {
                    using (DocX document = DocX.Load(fileName))
                    {
                        if (document.Text.Contains(keyword)) // 使用Contains方法代替IndexOf方法
                        {
                            list.Add(fileName); // 将包含TextBox_find.Text的段落文字加入列表
                        }
                    }

                }
                catch (Exception)
                {
                }
            });

            ListBox1.ItemsSource = list;
            long time = System.Environment.TickCount - a;
            //System.Windows.MessageBox.Show((time / 1000).ToString());
            Lable1.Content = $"搜索到{list.Count}个文档，耗时{(time / 1000)}秒";
        }
        private void ListBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = ListBox1.SelectedItem.ToString();
                p.Start();
            }
            catch (Exception ex)
            {

            }
        }

      
    }
}
