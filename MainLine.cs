using System;
using System.Windows.Forms;

namespace A1_Yoyo {
    static class MainLine {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new YoyoForm());
        }
    }
}
