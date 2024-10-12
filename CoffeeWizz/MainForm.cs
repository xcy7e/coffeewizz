#region header

// CoffeeWizz - MainForm.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/01/24 1:57 AM.

#endregion

#region using

using System;
using System.Windows.Forms;

using Xcy7e.CoffeeWizz.Properties;

#endregion

namespace Xcy7e.CoffeeWizz
{
    public partial class MainForm : Form
    {
        /// <summary>
        ///     Constructor for use by the form designer.
        /// </summary>
        public MainForm()
            : this(wizzleOnStartup: false, minimizeOnStartup: false, zenWizzleEnabled: false, wizzlePeriod: 1)
        { }

        public MainForm(bool wizzleOnStartup, bool minimizeOnStartup, bool zenWizzleEnabled, int wizzlePeriod)
        {
            this.InitializeComponent();

            // Wizzling on startup?
            this.WizzleOnStartup = wizzleOnStartup;

            // Set settings properties
            // We do this by setting the controls, and letting them set the properties.

            this.cbMinimize.Checked = minimizeOnStartup;
            this.cbZen.Checked = zenWizzleEnabled;
            this.tbPeriod.Value = wizzlePeriod;
        }

        public bool WizzleOnStartup { get; }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.WizzleOnStartup)
                this.cbWizzling.Checked = true;
        }

        private void cmdAbout_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(owner: this);
        }

        #region Property synchronization

        private void cbSettings_CheckedChanged(object sender, EventArgs e)
        {
            this.panelSettings.Visible = this.cbSettings.Checked;
        }

        private void cbMinimize_CheckedChanged(object sender, EventArgs e)
        {
            this.MinimizeOnStartup = this.cbMinimize.Checked;
        }

        private void cbZen_CheckedChanged(object sender, EventArgs e)
        {
            this.ZenWizzleEnabled = this.cbZen.Checked;
        }

        private void tbPeriod_ValueChanged(object sender, EventArgs e)
        {
            this.WizzlePeriod = this.tbPeriod.Value;
        }

        #endregion Property synchronization

        #region Do the Wizzle!

        protected bool Zig = true;

        private void cbWizzling_CheckedChanged(object sender, EventArgs e)
        {
            this.wizzleTimer.Enabled = this.cbWizzling.Checked;
        }

        private void wizzleTimer_Tick(object sender, EventArgs e)
        {
            if (this.ZenWizzleEnabled)
                Helpers.Wizzle(delta: 0);
            else if (this.Zig)
                Helpers.Wizzle(delta: 4);
            else //zag
                Helpers.Wizzle(delta: -4);

            this.Zig = !this.Zig;
        }

        #endregion Do the Wizzle!

        #region Minimize and restore

        private void cmdTrayify_Click(object sender, EventArgs e)
        {
            this.MinimizeToTray();
        }

        private void niTray_DoubleClick(object sender, EventArgs e)
        {
            this.RestoreFromTray();
        }

        private void MinimizeToTray()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.niTray.Visible = true;
        }

        private void RestoreFromTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.niTray.Visible = false;
        }

        #endregion Minimize and restore

        #region Settings property backing fields

        private int wizzlePeriod;

        private bool minimizeOnStartup;

        private bool zenWizzleEnabled;

        #endregion Settings property backing fields

        #region Settings properties

        public bool MinimizeOnStartup
        {
            get => this.minimizeOnStartup;
            set
            {
                this.minimizeOnStartup = value;
                Settings.Default.MinimizeOnStartup = value;
                Settings.Default.Save();
            }
        }

        public bool ZenWizzleEnabled
        {
            get => this.zenWizzleEnabled;
            set
            {
                this.zenWizzleEnabled = value;
                Settings.Default.ZenWizzle = value;
                Settings.Default.Save();
            }
        }

        public int WizzlePeriod
        {
            get => this.wizzlePeriod;
            set
            {
                this.wizzlePeriod = value;
                Settings.Default.WizzlePeriod = value;
                Settings.Default.Save();

                this.wizzleTimer.Interval = value * 1000;
                this.lbPeriod.Text = $"{value} s";
            }
        }

        #endregion Settings properties

        #region Minimize on start

        private bool firstShown = true;

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (this.firstShown && this.MinimizeOnStartup)
                this.MinimizeToTray();

            this.firstShown = false;
        }

#endregion
    }
}
