using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Project2Language
{
    public partial class Form1 : Form
    {
        private List<FileInfo> files;

        //private string pattern = "^((?!(\\*|//)).)+[\u4e00-\u9fa5]";
        //private string pattern = "(?<=\")([\u4e00-\u9fa5]+)(?=\")";
        private string pattern = ">(.{0,}?)<|\'(.{0,}?)\'|\"(.{0,}?)\"";  // 第一步匹配所有双引号
        private string pattern2 = "[\u4e00-\u9fa5]+";  // 第二步匹配中文

        private string keyStr = "xunhao_";
        private int keyIndex = 20000;
        private string h = ".java";//扩展名 
        private Dictionary<string, ResStr> dic = new Dictionary<string, ResStr>();


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                files = new List<FileInfo>();
                richTextBox1.AppendText("收集"+h+"文件中。。。\n");
                handlerFiles(dialog.SelectedPath);
                //DirectoryInfo root = new DirectoryInfo(dialog.SelectedPath);
                //DirectoryInfo[] infos = root.GetDirectories();
                //FileInfo[] files = root.GetFiles();
                //MessageBox.Show(dialog.SelectedPath);
                richTextBox1.AppendText("总共收集到："+files.Count.ToString() + "个文件\n");


                richTextBox1.AppendText("读取文件中执行匹配替换中。。。\n");
                handlerReadLine();

                richTextBox1.AppendText("匹配替换完成==总共匹配" + dic.Count + "项\n");
                
            }
        }

        private void handlerFiles(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)     //判断是否为文件夹  
                {
                    handlerFiles(fsinfo.FullName);//递归调用  
                }
                else
                {
                    string extension = System.IO.Path.GetExtension(fsinfo.FullName);//扩展名 
                    if (extension == h)
                    {
                        files.Add(fsinfo as FileInfo);
                    }
                    //richTextBox1.AppendText(fsinfo.FullName + '\n');
                }
            }
        }

        private void handlerReadLine()
        {
            FileStream fs;
            StreamReader read;


            foreach (FileInfo info in files)
            {
                Console.WriteLine("文件名"+info.FullName);
                fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
                read = new StreamReader(fs, Encoding.UTF8);
                string strReadLine;
                while ((strReadLine = read.ReadLine()) != null)
                {
                    if (Regex.IsMatch(strReadLine,pattern))
                    {

                        checkLine(strReadLine);
                        //Console.WriteLine(strReadLine);
                    }
                    //richTextBox1.AppendText(strReadLine + '\n');
                }
                fs.Close();
                read.Close();
            }
        }

        /// <summary>
        /// 查找行
        /// </summary>
        /// <param name="str"></param>
        private void checkLine(string str)
        {
            MatchCollection matches = Regex.Matches(str, pattern);

            ResStr rs;
            for (int i = 0; i < matches.Count; i++)
            {
                string v2 = matches[i].Value;

                if (Regex.IsMatch(v2,pattern2))
                {
                    rs = new ResStr();
                    //Console.WriteLine(v2);
                    string newV = v2.Substring(1, v2.Length - 2);  // 去掉< > " '符号
                    Console.WriteLine(v2+"@"+newV);
                    if (dic.ContainsKey(v2) == false)
                    {
                        rs.keyStr = keyStr + keyIndex;
                        rs.oldValueStr = v2;
                        rs.newValueStr = newV;

                        dic[v2] = rs;
                        keyIndex++;
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //string str = "<a href='#' onclick='clearServerData(\"+  row.id +\");'>重置开服时间</a>&nbsp;&nbsp;<a href='#' onclick='openServer(\"+  row.id +\");'>开服</a>";
            //string str = "<title>登陆</title>fjhjjh\'登Vip陆\'fghfgh\"中4566陆\"";
            string str = "{\"playerId\":\"用户ID\",\"subTypeId\":\"子类型ID\"," +
                "\"playerName\":\"玩家昵称\",\"param\":\"描述\",\"typeId\":\"主类型ID\"," +
                "\"level\":\"玩家等级\",\"exp\":\"当前经验\",\"gold\":\"金币\"," +
                "\"copper\":\"铜钱\",\"coupon\":\"礼券\",\"reiki\":\"灵力\",\"gas\":\"真气\",\"xueLian\":\"雪莲\",\"ipAddr\":\"IP地址\"," +
                "\"account\":\"用户名\",\"actionTime\":\"操作时间\"}";

            //string str = "var _loadAssets:Array=[{url:ResUtils.getModuleResPath(\"BattleSWFAssets\"),name:\"战斗UI资源2\"},{url:ResUtils.getModuleResPath(\"BattleRes\"),name:\"战斗UI资源1\"},{url:ResUtils.getModuleResPath(\"DungeonPassSWFAssets\"),name:\"副本通关奖励资源\"}];";

            //string pattern = "(?<=\")([\u4e00-\u9fa5]+\\w*)(?=\")";
           // string pattern = "(?<=\"|'|>).*?(?=\"|'|<)";  // 第一步匹配所有双引号
            string pattern = ">([\u4e00-\u9fa5]{0,}?)<|\'([\u4e00-\u9fa5]{0,}?)\'|\"([\u4e00-\u9fa5]{0,}?)\"";

            Console.WriteLine(str);

            MatchCollection matches = Regex.Matches(str, pattern);

            string pattern2 = "[\u4e00-\u9fa5]+";  // 第二步匹配中文
            Regex re2 = new Regex(pattern2);

            for (int i = 0; i < matches.Count; i++)
            {
                string v2 = matches[i].Value;
                if (re2.IsMatch(v2))
                {
                    //str = str.Replace(v2, "@@");
                    Console.WriteLine(v2);
                }
            }

            Console.WriteLine(str);


        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            if (dic.Count <= 0)
            {
                MessageBox.Show("请先读取数据");
                return;
            }

            richTextBox1.AppendText("开始写配置文件。。。\n");

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string xmlPath = dir + "\\language_zh_CN.properties";

            //NiceFileProduce.CheckAndCreatPath(NiceFileProduce.DecomposePathAndName(filePath, NiceFileProduce.DecomposePathEnum.PathOnly));

            //XmlDocument xmlDoc = new XmlDocument();
            //XmlNode header = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            //xmlDoc.AppendChild(header);
            //XmlElement rootNode = xmlDoc.CreateElement("language");
            //rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            //xmlDoc.AppendChild(rootNode);

            StreamWriter sw = new StreamWriter(xmlPath, false, new UTF8Encoding(false));
            string lineStr = "";
            foreach (KeyValuePair<string,ResStr> v in dic)
            {
                //XmlElement itemNode = xmlDoc.CreateElement("item");
                //itemNode.SetAttribute("id", v.Value);
                //itemNode.InnerText = System.Security.SecurityElement.Escape(v.Key);
                //XmlCDataSection  xcs = xmlDoc.CreateCDataSection(v.Key);
                //itemNode.AppendChild(xcs);


                //rootNode.AppendChild(itemNode);

                lineStr = v.Value.keyStr + "=" + v.Value.newValueStr;
                sw.WriteLine(lineStr);
            }
            //xmlDoc.Save(sw);
            //sw.WriteLine();
            sw.Flush();
            sw.Close();

            richTextBox1.AppendText("配置结束写稿，保存路径"+xmlPath+"\n");
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            richTextBox1.AppendText("开始替换\n");

            ReplaceHandler(files);

            /*
            List<List<FileInfo>> ElevList = new List<List<FileInfo>>(5);
            ElevList[0] = files.Skip(0 * 430).Take(430).ToList();
            ElevList[1] = files.Skip(1 * 430).Take(430).ToList();
            ElevList[2] = files.Skip(2 * 430).Take(430).ToList();
            ElevList[3] = files.Skip(3 * 430).Take(430).ToList();
            ElevList[4] = files.Skip(4 * 430).Take(431).ToList();

            */

            //Thread thread = new Thread(new ThreadStart(methodHandler));//创建线程
            //thread.Start();                                                           //启动线程

            //Thread thread1 = new Thread(new ThreadStart(methodHandler));//创建线程
            //thread1.Start();

            richTextBox1.AppendText("替换完成\n");
            this.Cursor = Cursors.Default;//正常状态
        }

        private void methodHandler()
        {
            string threadName = Thread.CurrentThread.Name;
            Console.WriteLine("methodHandler-"+threadName);
        }


        /// <summary>
        /// 替换处理
        /// </summary>
        private void ReplaceHandler(List<FileInfo> list)
        {
            //const string imports = "<%@page import=\"com.huoji.xunhao.admin.utils.CommonUtils\"%>";  // 导入的类
            const string imports = "import com.huoji.xunhao.admin.utils.CommonUtils;";

            const string replaceM = "MMMMMMMMMMMMMMMMMMMMMMMM";  // 导入类替换符

            FileStream fs;
            StreamReader read;
            foreach (FileInfo info in list)
            {
                //Console.WriteLine("文件名" + info.FullName);
                string extension = System.IO.Path.GetExtension(info.FullName);//扩展名 

                fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
                read = new StreamReader(fs, Encoding.UTF8);
                StringBuilder readContent = new StringBuilder();
                string strReadLine;
                string newLine;
                int lineNum = 0;
                bool isWriteImport = false;  // 是否已写导入类行
                bool isUserImport = false;  // 是否已有导入类
                bool isNeedImport = false;  // 是否需要导入类
                while ((strReadLine = read.ReadLine()) != null)
                {
                    lineNum++;
                    if (lineNum > 1 && !isWriteImport)
                    //if (!isWriteImport)
                    {
                        readContent.AppendLine(replaceM);
                        isWriteImport = true;
                    }
                    if (isUserImport == false) isUserImport = strReadLine.IndexOf(imports) != -1;

                    //Console.WriteLine("旧行：" + strReadLine);
                    if (Regex.IsMatch(strReadLine, pattern))
                    {
                        newLine = ReplaceLine(strReadLine, ref isNeedImport);
                    }
                    else
                    {
                        newLine = strReadLine;
                    }
                    //Console.WriteLine("新行：" + newLine);
                    readContent.AppendLine(newLine);

                    //richTextBox1.AppendText(strReadLine + '\n');
                }

                read.Close();
                fs.Close();

                if (isNeedImport) readContent.Replace(replaceM, isUserImport ? "" : imports);
                else readContent.Replace(replaceM, "");

                if (isNeedImport)
                {
                    fs = new FileStream(info.FullName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(readContent.ToString());
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }

                //Console.WriteLine(readContent);
            }
        }

        /// <summary>
        /// 替换行
        /// </summary>
        /// <param name="str"></param>
        private string ReplaceLine(string str,ref bool bo)
        {
            if (!Regex.IsMatch(str, pattern2)) return str;  // 此行一个中文也没有直接返回

            //const string methodName = "ResUtils.getLanguageData(\"-\")";
            //const string methodName = "<%=CommonUtils.getLanguage(\"-\")%>";
            const string methodName = "CommonUtils.getLanguage(\"-\")";


            //Console.WriteLine("旧行-ReplaceLine：" + str);

            MatchCollection matches = Regex.Matches(str, pattern);

            for (int i = 0; i < matches.Count; i++)
            {
                string v2 = matches[i].Value;
                if (String.IsNullOrEmpty(v2)) continue;
                if (Regex.IsMatch(v2, pattern2))
                {
                    ResStr rs = dic[v2];
                    //str = str.Replace(v2, "123");
                    //Console.WriteLine(v2);
                    if (rs == null) continue;
                    if (rs.oldValueStr.IndexOf(">") == -1)
                    {
                        //str = str.Replace(rs.oldValueStr, "\""+methodName.Replace("-", rs.keyStr)+ "\"");
                        str = str.Replace(rs.oldValueStr, methodName.Replace("-", rs.keyStr));
                    }
                    else
                    {
                        str = str.Replace(rs.newValueStr, methodName.Replace("-", rs.keyStr));
                    }
                    if (bo == false) bo = true;
                }
            }

            //Console.WriteLine("新行-ReplaceLine：" + str);
            return str;

        }
    }

    class ResStr
    {
        public string keyStr = "";
        public string oldValueStr = "";
        public string newValueStr = "";
    }
}
