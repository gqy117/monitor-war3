using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace 监视war3
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            WMIProcessWatch wmipw = new WMIProcessWatch();
            Console.ReadLine();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }

    }
}