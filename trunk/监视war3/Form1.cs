using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;


namespace 监视war3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.label1.BackColor = System.Drawing.Color.Transparent;  
            if (File.Exists(Application.StartupPath + "\\moni.ini"))
            {
                textBox1.Text = inireadvalue("root", "gamepath_and_name1", Application.StartupPath + "\\moni.ini");
                this.WindowState =  FormWindowState.Minimized;


            }
            else
            {
                this.WindowState = FormWindowState.Normal;

            }
            this.myNotifyIcon.Visible = true;

        }
        #region 声明读写ini的api
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion 声明读写ini的api

        public void iniwritevalue(string section, string key, string value, string filepath)//对ini文件进行写操作的函数
        {
            WritePrivateProfileString(section, key, value, filepath);
        }

        public string inireadvalue(string section, string key, string filepath)//对ini文件进行读操作的函数
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, filepath);
            
            return temp.ToString();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            //textBox1.Text = openFileDialog1.SelectedPath.ToString();
            textBox1.Text = openFileDialog1.FileName.ToString();

        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("请选择文件路径！","提示:");
                return;
            }
            #region 保存游戏路径名和游戏名
            iniwritevalue("root", "gamepath_and_name1", textBox1.Text, Application.StartupPath + "\\moni.ini");
            #endregion 保存游戏路径名和游戏名

            #region 保存游戏名
            string gamename="";
            int i=0;
            int pos_of_last = 0;
            while(i!=-1)
            {
                i=textBox1.Text.IndexOf("\\",i+1);
                if (i != -1)
                { pos_of_last = i; }
            }
            gamename = textBox1.Text.Substring(pos_of_last+1, textBox1.Text.Length - pos_of_last-1);
            i = gamename.IndexOf(".",0);
            gamename = gamename.Substring(0, i);
            iniwritevalue("root", "gamename1", gamename, Application.StartupPath + "\\moni.ini");
            #endregion 保存游戏名

            #region 保存游戏路径
            string gamepath = "";
            gamepath = textBox1.Text.Substring(0, pos_of_last+1);
            iniwritevalue("root", "gamepath1", gamepath, Application.StartupPath + "\\moni.ini");
            #endregion 保存游戏路径
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //this.Close();  //在关闭窗体
            this.myNotifyIcon.Visible = false;//关闭托盘显示
            Application.Exit();//退出应用程序
            
        }

        private void myNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            { return; }
            else
            {
                e.Cancel = true;  //取消窗体的关闭
                this.Hide();      //隐藏当前窗体
                this.ShowInTaskbar = false; //在任务栏中不显示此窗体
                //this.myNotifyIcon.Visible = true;  //把此窗体隐藏到托盘程序中
            }
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            //this.Close();  //在关闭窗体
            this.myNotifyIcon.Visible = false;//关闭托盘显示
            Application.Exit();//退出应用程序
        }





    }
}