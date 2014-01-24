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
    public partial class Register : Form
    {
        RSACryptoServiceProvider RSAregister;

        public string ServerPubKey;
        private string PrivateKey;
        public string userLogin;

        // Wczytuje klucz pub serwera i priv klienta
        public Register(string c, string a, string b)
        {
            InitializeComponent();
            ServerPubKey = a;
            PrivateKey = b;
            userLogin = c;
            RSAregister = new RSACryptoServiceProvider(1024);
            RSAregister.FromXmlString(ServerPubKey);
            Show();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            SendToServer();
        }

        // Szyfruje i wysyla dane do serwera, uaktualnia 
        private void SendToServer()
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            Server.server.NewClientRequest(RSAregister.Encrypt(encoding.GetBytes(textBox1.Text), false), RSAregister.Encrypt(encoding.GetBytes(Server.GetHash(textBox2.Text)), false), RSAregister.Encrypt(encoding.GetBytes(userLogin), false));
            Server.server.RegisterClient();
            this.Hide();
        }
    }
}
