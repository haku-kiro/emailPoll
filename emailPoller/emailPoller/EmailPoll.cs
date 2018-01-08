using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;

namespace emailPoller
{
    public partial class EmailPoll : ServiceBase
    {
        Timer timer;

        public EmailPoll()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //log that the service has started
            double time = TimeSpan.FromMinutes(5).TotalMilliseconds;
            timer = new System.Timers.Timer(time);
            timer.Elapsed += new ElapsedEventHandler(RunPoll);
            timer.Start();
        }

        protected override void OnStop()
        {
            //log stop here
        }

        private void RunPoll(object sender, ElapsedEventArgs e)
        {
            helperMethods helper = new helperMethods();
            string filter = ConfigurationManager.AppSettings["subj"];
            helper.pollServer(filter); // get the filter
        }
    }
}
