using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace PKRY
{
    public partial class Login : Form
    {
        Server server;
        RSACryptoServiceProvider RSAclient;

        public string ServerPubKey;
        private string PrivateKey;


        public Login()
        {
            InitializeComponent();
            this.server = Server.server;
            Show();
            CipherGen();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SendToServer(textBox1.Text, textBox2.Text);
            server.Logging(this);
          //  server.UpdatePrices();
        }

        public void CipherGen()
        {
            RSAclient = new RSACryptoServiceProvider(1024);
            PrivateKey = RSAclient.ToXmlString(true);
            string PublicKey = RSAclient.ToXmlString(false);
            server.ClientPubKey = PublicKey;
            ServerPubKey = server.PublicKey;
        }


        public MainFrame Logging()
        {
            MainFrame mainFrame = new MainFrame(textBox1.Text, ServerPubKey, PrivateKey);
           // server.UpdatePrices();
            if (server.status == "Connected")
            {
                this.label3.Text = "Connected";
                this.Refresh();
                System.Threading.Thread.Sleep(1500);
                this.Hide();
                server.action(mainFrame);
                mainFrame.ServerPubKey = ServerPubKey;
                mainFrame.loadKey(PrivateKey);
            }
            else
            {
                this.label3.Text = server.status;
            }
            return mainFrame;
        }

        public void SendToServer(string a, string b)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAclient.FromXmlString(ServerPubKey);
            server.ReceiveData(RSAclient.Encrypt(encoding.GetBytes(a), false), RSAclient.Encrypt(encoding.GetBytes(Server.GetHash(b)), false));
        }
    }
}
