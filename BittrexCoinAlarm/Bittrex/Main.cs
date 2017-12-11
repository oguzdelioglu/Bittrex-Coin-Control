using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Media;

namespace Bittrex
{

    public partial class main : Form
    {
        MyProg.INIFile inif = new MyProg.INIFile(AppDomain.CurrentDomain.BaseDirectory + "\\config.ini");
        public string coinnameString = "";
        public string currency = "https://bittrex.com/api/v1.1/public/getmarketsummary?market=BTC-" + "{0}";
        public string data = "";
        public string info1 = "Coin Lower Limit!";
        public string info2 = "Coin Upper Limit!";
        double price = 0;
        public main()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            time.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("config.ini"))
            {
                    try
                    {
                    coin.Text = inif.Read("Settings", "coinname");
                    toplimit.Text = inif.Read("Settings", "toplimit");
                    lowlimit.Text = inif.Read("Settings", "lowlimit");
                    }
                    catch
                    {

                    }
            }
        }
        public void getinfo()
        {
            JObject o = JObject.Parse(data);
            string getinfo = "";
            foreach (var x in o)
            {
                if (x.Key.ToString() == "result")
                {
                    getinfo = x.Value.Last["Last"].ToString();
                    price = Convert.ToDouble(getinfo);
                    coinname.Text = coinnameString;
                    lblPrice.Text = price.ToString();
                    this.Text = coinnameString + " | " + price.ToString();
                }
            }
        }
        public void control()
        {
            double alt = Convert.ToDouble(lowlimit.Text);
            double ust = Convert.ToDouble(toplimit.Text);
            if (price >= ust)
            {
                alarm(info2);
            }
            else if (price < alt)
            {
                alarm(info1);
            }
            else
            {
                time.Start();
            }
        }
        public void alarm(String bilgitur)
        {
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipTitle = coinnameString;
            notifyIcon1.BalloonTipText = bilgitur + Environment.NewLine+ "Now:" + lblPrice.Text;
            notifyIcon1.ShowBalloonTip(20000);
            SoundPlayer player = new SoundPlayer();
            string path = "C:\\windows\\media\\notify.wav"; // Müzik adresi
            player.SoundLocation = path;
            player.PlayLooping();
        }
        public void getdata()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(currency);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            data = reader.ReadToEnd();
        }
        public class Item
        {
            public string asks;
        }
        public void setprice(String coin)
        {
            coinnameString = coin;
            currency = String.Format(currency, coin);
        }
        private void time_Tick(object sender, EventArgs e)
        {
            time.Stop();
            try
            {
                setprice(coin.Text);
                getdata();
                getinfo();
                control();
            }
            catch
            {
                time.Start();
            } 
        }
        public void stopalarm()
        {
            SoundPlayer player = new SoundPlayer();
            string path = "C:\\windows\\media\\notify.wav"; // Müzik adresi
            player.SoundLocation = path;
            player.Stop();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            stopalarm();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            inif.Write("Settings", "coinname", coin.Text);
            inif.Write("Settings", "lowlimit", lowlimit.Text);
            inif.Write("Settings", "toplimit", toplimit.Text);
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void stopalarmturToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopalarm();
        }
    }
  
}
