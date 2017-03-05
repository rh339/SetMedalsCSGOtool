using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        static public bool isrunning;
        static public string SteamAuthCode;
        public bool count;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] csgo = Process.GetProcessesByName("csgo"); ;
            if (csgo.Length > 0)
            {
                try
                {
                    csgo = Process.GetProcessesByName("csgo");

                    Process mspaintProc = csgo[0];

                    if (!mspaintProc.HasExited)
                    {
                        mspaintProc.Kill();
                    }
                }
                finally
                {
                    if (csgo != null)
                    {
                        foreach (Process p in csgo)
                        {
                            p.Dispose();
                        }
                    }
                }
            }
            Process[] steam = Process.GetProcessesByName("steam"); ;
            if (steam.Length > 0)
            {
                try
                {
                    steam = Process.GetProcessesByName("steam");

                    Process mspaintProc = steam[0];

                    if (!mspaintProc.HasExited)
                    {
                        mspaintProc.Kill();
                    }
                }
                finally
                {
                    if (steam != null)
                    {
                        foreach (Process p in steam)
                        {
                            p.Dispose();
                        }
                    }
                }
            }
            if (!isrunning)
            {
                Process steamstart = new Process();
                steamstart.StartInfo.FileName = File.ReadAllText("data/path.txt") + "\\SteaM.exe";
                steamstart.StartInfo.Arguments = String.Format(" -login " + File.ReadAllText("data/login.txt") + " " + File.ReadAllText("data/password.txt"));
                steamstart.Start();
                Task.Run(() => startnethook());
                isrunning = true;
                button1.Text = "Stop";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button2.Enabled = true;
            }
            else
            {
                isrunning = false;
                button1.Text = "Start";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button2.Enabled = false;
            }
        }
        private void startnethook()
        {
            Process[] steam = Process.GetProcessesByName("steam");
            Thread.Sleep(10000);
            Process steamstart = new Process();
            steamstart.StartInfo.FileName = "start.bat";
            steamstart.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Tick += new EventHandler(timer100ms);
            timer1.Interval = 100;
            timer1.Start();
            Directory.CreateDirectory("data");
            try
            {
                File.ReadAllText("data/path.txt");
            }
            catch (Exception)
            {
                button1.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                textBox2.Visible = false;
                button4.Visible = false;
                button2.Visible = false;
            }
            try
            {
                textBox1.Text = File.ReadAllText("data/login.txt");
            }
            catch (Exception)
            {
                File.WriteAllText("data/login.txt", "Login");
                textBox1.Text = "login";
            }
            try
            {
                textBox2.Text = File.ReadAllText("data/password.txt");
            }
            catch (Exception)
            {
                File.WriteAllText("data/password.txt", "password");
                textBox2.Text = "password";
            }
        }
        private void timer100ms(object sender, EventArgs e)
        {
            if (Otlegacheck.NeedSteamGuardKey)
            {
                textBox4.Enabled = true;
            }
            else
            {
                SteamAuthCode = null;
                textBox4.Enabled = false;
            }
            label1.Text = Otlegacheck.informationsteam;
            if (Otlegacheck.informationsteam == "Medals Set!")
            {
                listBox1.Items.Add(DateTime.Now.ToLongTimeString() + ":Medals Set");
                Otlegacheck.informationsteam = "";
                backgroundWorker1.CancelAsync();
            }
            try
            {
                if (File.ReadAllText(File.ReadAllText("data/path.txt") + "//nethookstatus.txt").Contains("call"))
                {
                    File.WriteAllText(File.ReadAllText("data/path.txt") + "//nethookstatus.txt", null);
                    if (isrunning)
                    {
                        if (!count)
                        {
                            count = true;
                            backgroundWorker1.RunWorkerAsyn​c();
                            listBox1.Items.Add(DateTime.Now.ToLongTimeString() + ":NetHook find Msg");
                        }
                        else
                        {
                            count = false;
                            listBox1.Items.Add(DateTime.Now.ToLongTimeString() + ":NetHook find Msg(Ignored)");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText("data/login.txt", textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText("data/password.txt", textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsyn​c();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Otlegacheck.SetMyMedals();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length == 5)
            {
                SteamAuthCode = textBox4.Text;
                textBox4.Text = null;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process[] csgo = Process.GetProcessesByName("csgo"); ;
            if (csgo.Length > 0)
            {
                try
                {
                    csgo = Process.GetProcessesByName("csgo");

                    Process mspaintProc = csgo[0];

                    if (!mspaintProc.HasExited)
                    {
                        mspaintProc.Kill();
                    }
                }
                finally
                {
                    if (csgo != null)
                    {
                        foreach (Process p in csgo)
                        {
                            p.Dispose();
                        }
                    }
                }
            }
            Process[] steam = Process.GetProcessesByName("steam"); ;
            if (steam.Length > 0)
            {
                try
                {
                    steam = Process.GetProcessesByName("steam");

                    Process mspaintProc = steam[0];

                    if (!mspaintProc.HasExited)
                    {
                        mspaintProc.Kill();
                    }
                }
                finally
                {
                    if (steam != null)
                    {
                        foreach (Process p in steam)
                        {
                            p.Dispose();
                        }
                    }
                }
            }
            backgroundWorker1.CancelAsync();
            Otlegacheck.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(folderBrowserDialog1.SelectedPath + "\\steam.exe"))
                {
                    File.WriteAllText("data/path.txt", folderBrowserDialog1.SelectedPath);
                    button1.Enabled = true;
                    MessageBox.Show("Success");
                    button1.Visible = true;
                    textBox1.Visible = true;
                    textBox2.Visible = true;
                    textBox2.Visible = true;
                    button4.Visible = true;
                    button2.Visible = true;
                }
                else
                {
                    MessageBox.Show("Can't find Steam.exe");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(textBox2.Text);
        }
    }
}
