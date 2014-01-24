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

        // Uruchamia formularz i kaze generowac klucz prywatny i publiczny klienta
        public Login()
        {
            InitializeComponent();
            this.server = Server.server;
            Show();
            CipherGen();
        }

        // Kaze wyslac na serwer login i haslo, uruchamia funkcje weryfikacji klienta na serwerze
        private void button1_Click(object sender, EventArgs e)
        {
            SendToServer(textBox1.Text, textBox2.Text);
            server.Logging(this);
        }


        // Generuje klucz prywatny i publiczny klienta i dodatkowo ozapisuje klucz publiczny serwera
        public void CipherGen()
        {
            RSAclient = new RSACryptoServiceProvider(1024);
            PrivateKey = RSAclient.ToXmlString(true);
            string PublicKey = RSAclient.ToXmlString(false);
            server.ClientPubKey = PublicKey;
            ServerPubKey = server.PublicKey;
        }


        // Procedura logowania, wyswietla status weryfikacji klienta przez serwer
        public MainFrame Logging()
        {
            MainFrame mainFrame = new MainFrame(textBox1.Text, ServerPubKey, PrivateKey);
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

        // Szyfruje i wysyla dane logowania na serwer, haslo dodatkowo skraca
        public void SendToServer(string a, string b)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAclient.FromXmlString(ServerPubKey);
            server.ReceiveData(RSAclient.Encrypt(encoding.GetBytes(a), false), RSAclient.Encrypt(encoding.GetBytes(Server.GetHash(b)), false));
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


    }
}
