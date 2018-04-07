using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Megadotnet.WinService
{
    partial class MQConsumeService : ServiceBase
    {
        /// <summary>
        /// The BackgroundJobServer of hangfire
        /// </summary>
        //private BackgroundJobServer _server;

        public MQConsumeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Reference hangfire
            //_server = new BackgroundJobServer(new SqlServerStorage("ScheduleHostDb"));
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
