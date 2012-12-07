using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace 监视war3
{
    public class WMIProcessWatch
    {
        ManagementEventWatcher watch_crt = null;
        ManagementEventWatcher watch_del = null;

        [DllImport("user32.dll")]
        private static extern bool
        SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        // 消息函数
        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strclassName, string strWindowName);
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MAXIMIZE = 0xF030;

        public WMIProcessWatch()
        {
            Console.WriteLine("Waiting for an event...");

            StartWatchCreateProcess();
            StartWatchDeleteProcess();
        }

        ~WMIProcessWatch()
        {
            if (watch_crt != null)
                watch_crt.Stop();
            if (watch_del != null)
                watch_del.Stop();
        }

        protected void StartWatchCreateProcess()
        {
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent",
                         new TimeSpan(0, 0, 1),
                         "TargetInstance isa \"Win32_Process\"");
            watch_crt = new ManagementEventWatcher(query);
            watch_crt.EventArrived += new EventArrivedEventHandler(HandleProcessCreateEvent);
            watch_crt.Start();
        }

        protected void StartWatchDeleteProcess()
        {
            WqlEventQuery query = new WqlEventQuery("__InstanceDeletionEvent",
                         new TimeSpan(0, 0, 1),
                         "TargetInstance isa \"Win32_Process\"");
            watch_del = new ManagementEventWatcher(query);
            watch_del.EventArrived += new EventArrivedEventHandler(HandleProcessDeleteEvent);
            watch_del.Start();
        }
        //监视进程启动
        protected void HandleProcessCreateEvent(object sender,EventArrivedEventArgs e)
        {
            ManagementBaseObject MBO = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string temp = MBO["Name"].ToString();
            if (temp.ToUpper() == "WAR3.EXE")//魔兽争霸被启动
            {

                Process process = new Process();
                Form1 f1 = new Form1();
                string gamepath_and_name_read = f1.inireadvalue("root", "gamepath_and_name1", Application.StartupPath + "\\moni.ini");
                string gamepath_read = f1.inireadvalue("root", "gamepath1", Application.StartupPath + "\\moni.ini");


                process.StartInfo.FileName = gamepath_and_name_read;
                process.StartInfo.WorkingDirectory = gamepath_read;//指定工作目录,不指定的话有些程序无法运行
                process.Start();//启动程序
                #region 让war3获得焦点
                Process[] war3_process = Process.GetProcessesByName("war3");
                if (war3_process.Length > 0)
                {
                    IntPtr hWnd = war3_process[0].MainWindowHandle;

                    //  if (IsIconic(hWnd))
                    //    ShowWindowAsync(hWnd, 9);// 9就是SW_RESTORE标志，表示还原窗体
                    SendMessage(hWnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                    SetForegroundWindow(hWnd);
                }
                #endregion 让war3获得焦点

            }

            //ReportEventMessage(e);
        }
        //监视进程退出
        protected void HandleProcessDeleteEvent(object sender,EventArrivedEventArgs e)
        {
            ManagementBaseObject MBO = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string temp = MBO["Name"].ToString();
            if (temp.ToUpper() == "WAR3.EXE")//魔兽争霸被退出
            {
                #region 关闭war3辅助工具(杀进程)
                Form1 f1 = new Form1();
                string gamename_read = f1.inireadvalue("root", "gamename1", Application.StartupPath + "\\moni.ini");


                //Process[] process = Process.GetProcessesByName(gamename_read);
                Process[] process = Process.GetProcessesByName(gamename_read);
                foreach (Process p in process)
                {
                    p.Kill();
                }
                #endregion 关闭war3辅助工具(杀进程)
            }

            //ReportEventMessage(e);
        }

        protected void ReportEventMessage(EventArrivedEventArgs e)
        {
            ManagementBaseObject o = e.NewEvent; //__InstanceCreationEvent
            ManagementBaseObject mo = (ManagementBaseObject)o["TargetInstance"];//Win32_Process
            Console.WriteLine("Name={0},ExecPath={1},CommandLine={2}",
                mo["Name"], mo["ExecutablePath"], mo["CommandLine"]);
        }

        //public static void Main()
        //{
        //    WMIProcessWatch wmipw = new WMIProcessWatch();
        //    Console.ReadLine();
        //}
    }

}
