using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xbox_360_BadUpdate_USB_Tool
{
    public partial class Form1 : Form
    {
        public string currentver = "V1.0B";
        private bool IsRunAsAdmin()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public Form1()
        {
            InitializeComponent();
            if (!IsRunAsAdmin())
            {
                MessageBox.Show("This program must be run as administrator. The program will now exit.", "Admin Rights Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No internet connection detected. The application will now exit.", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            try
            {
                using (var client = new WebClient())
                {
                    string latestVersion = client.DownloadString("https://pastebin.com/raw/6LQSHFYF").Trim();
                    if (latestVersion != currentver)
                    {
                        var result = MessageBox.Show(
                            $"An update for BadStick is available!\n\nYour Version: {currentver}\nLatest Version: {latestVersion}\n\nWould you like to update now?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("https://github.com/32BitKlepto/BadStick");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates:\n{ex.Message}", "Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreditsBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is for everyone who worked tirelessly to develop and bring BadUpdate to the community. " +
                "Thank you to everyone for your undying dedication and devotion to this community. " +
                "\n\n\nBadStick Developer & Creator:\n" +
                "- Thomas Shelby (@1xShelby / Klepto) \n\n\nBadUpdate Exploit Credits:\n" +
                "- Grimdoomer (Ryan Miceli)\n- InvoxiPlayGames (Emma)\n- kmx360 (Mate Kukri)\n\n\n" +
                "Bill Gates (no jk. fuck you bill microdick)\n\n" +
                "And thank you to all of the homebrew developers for bringing such " +
                "programs and tools to the community. Your work has done so much over" +
                "the last 20 years for everyone in this community. You are all legends." +
                "\n\n Honorable Mentions:\n" +
                "- MrMario2011\n" +
                "- ModdedWarfare\n" +
                "- Sharkys Customs / DavisorNaw\n" +
                "- Modern Vintage Gamer" + 
                "- Element18592", "Credits Where They Are Due <3", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ContinueBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 Next = new Form2();
            Next.Show();
        }
    }
}
