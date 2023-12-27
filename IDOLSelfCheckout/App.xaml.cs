using IDOLSelfCheckout.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IDOLSelfCheckout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //return;
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //return;
            var wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            var wp = new System.Security.Principal.WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            //if (!runAsAdmin)
            //{
            //    var processInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            //    processInfo.UseShellExecute = true;
            //    processInfo.Verb = "runas";
            //    try
            //    {
            //        Process.Start(processInfo);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("程序自动以管理员身份运行出错，请手动设置以管理员身份运行程序" + ex, "ERROR");
            //        throw;
            //    }
            //    Environment.Exit(0);
            //}
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            CCL_Lamp lamp = new CCL_Lamp();
            lamp.BlueClose();
            lamp.RedClose();
            lamp.GreenClose();
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
                MessageBox.Show("UI线程异常:" + e.Exception.Message);
            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                MessageBox.Show("UI线程发生致命错误！" + ex.Message);
            }

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("非UI线程发生致命错误");
            }
            sbEx.Append("非UI线程异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show(sbEx.ToString());
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            MessageBox.Show("Task线程异常：" + e.Exception.Message);
            e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
        }

    }
}
