﻿using System;
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

    public partial class MainFrame : Form
    {
        public string ServerPubKey;
        private string PrivateKey;
        public string userLogin;
        public double sum;

        RSACryptoServiceProvider RSAmainFrame;


        // Wczytuje klucz pub serwera i priv klienta - dostaje je od klasy Login
        public MainFrame(string y, string a, string b)
        {
            InitializeComponent();
            this.userLogin = y;
            ServerPubKey = a;
            PrivateKey = b;
            RSAmainFrame = new RSACryptoServiceProvider(1024);
            RSAmainFrame.FromXmlString(ServerPubKey);
            
        }

        // Przypisuje klucz prywatny klienta
        public void loadKey(string privKey)
        {
            PrivateKey = privKey;
        }

        // Zamyka okienko kupowania i otwiera okno logowania
        private void MainFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            Login login = new Login();
            Server.server.ResetPrices();
        }

        // Dodaje sumy wszystkich produktow wymnozone razy ich ilosc
        public void sumUp()
        {
            label24.Text = Convert.ToString(numericUpDown1.Value * Convert.ToDecimal(label18.Text) + numericUpDown2.Value * Convert.ToDecimal(label19.Text) + numericUpDown3.Value * Convert.ToDecimal(label20.Text) + numericUpDown4.Value * Convert.ToDecimal(label21.Text) + numericUpDown5.Value * Convert.ToDecimal(label22.Text) + numericUpDown6.Value * Convert.ToDecimal(label23.Text));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            sumUp();
        }

        // Uruchamia okienka rekomendacji
        private void button1_Click(object sender, EventArgs e)
        {
            Invite register = new Invite(userLogin, ServerPubKey, PrivateKey);
        }


        // Wysyla kwote zamowienia na serwera i kaze mu zauktualizowac ceny, ustawia numericUpDown na 0
        private void button2_Click(object sender, EventArgs e)
        {
            sum = Convert.ToDouble(label24.Text);
            SendToServer(sum);
            Server.server.UpdatePrices();
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            numericUpDown3.Value = 0;
            numericUpDown4.Value = 0;
            numericUpDown5.Value = 0;
            numericUpDown6.Value = 0;
            sumUp();
        }


        // Wysyla na serwer zaszyfrowane dane dotyczace kwoty zamowienia i kaze je doliczyc do ogolnej sumy zamowien
        private void SendToServer(double x)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAmainFrame.FromXmlString(ServerPubKey);
            Server.server.AddSpent(RSAmainFrame.Encrypt(encoding.GetBytes(Convert.ToString(x)), false), RSAmainFrame.Encrypt(encoding.GetBytes(userLogin), false));
        }


        // Odszyfrowuje i wczytuje ceny otrzymane z serwera
        public void LoadPrices(byte [] encrypted, byte [] encrypted2, byte [] encrypted3, byte [] encrypted4, byte [] encrypted5, byte [] encrypted6)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAmainFrame.FromXmlString(PrivateKey);
            label18.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted, false)))).ToString("#.##");
            label19.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted2, false)))).ToString("#.##");
            label20.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted3, false)))).ToString("#.##");
            label21.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted4, false)))).ToString("#.##");
            label22.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted5, false)))).ToString("#.##");
            label23.Text = Convert.ToDouble(new string(encoding.GetChars(RSAmainFrame.Decrypt(encrypted6, false)))).ToString("#.##");
        }
    }
}
