using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using anko2Sample;

namespace anko2unity
{
    public class Anko2UnityPlugin : APlugin
    {

        private Setting form;

        private TCPSender tcpSender;

        void host_Initialized(object sender, EventArgs e)
        {
            form = new Setting(host);
            form.VisibleChanged += (sender2, args) =>
            {
                if (!form.Visible && tcpSender != null && tcpSender.Port != form.Port)
                {
                    tcpSender.Stop();
                    tcpSender.Start(form.Port);
                }
            };

            host.ReceiveChat += (o, args) =>
            {
                var json = new CommentData(args.Chat).ToJson();
                if (tcpSender != null && tcpSender.IsRunning)
                {
                    tcpSender.SendToAll(json);
                }
            };
            
        }

        #region IPlugin メンバー

        public override string Description { get { return AssemblyDescription; } }

        public override bool IsAlive
        {
            get { return form == null || form.IsDisposed ? false : form.IsAlive; }
        }

        public override string Name { get { return AssemblyTitle; } }

        public override void Run()
        {
            if (form == null || form.IsDisposed)
            {
                form = new Setting(host);
                form.VisibleChanged += (sender, args) =>
                {
                    if (tcpSender.Port != form.Port)
                    {
                        tcpSender.Stop();
                        tcpSender.Start(form.Port);
                    }
                };
            }

            if (!form.Visible)
            {
                form.Show((IWin32Window)host.Win32WindowOwner);
            }
            else if (form.WindowState != FormWindowState.Normal)
            {
                form.WindowState = FormWindowState.Normal;
            }

            if (tcpSender == null)
            {
                tcpSender = new TCPSender();
                tcpSender.Start(form.Port);
            }
        }

        public override ankoPlugin2.IPluginHost host
        {
            set
            {
                base.host = value;
                host.Initialized += host_Initialized;
            }
        }

        #endregion

        #region アセンブリ属性アクセサー

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        #endregion

    }
}
