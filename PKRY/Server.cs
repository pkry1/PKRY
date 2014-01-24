using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using PKRY;
using System.IO;

namespace PKRY
{
    public class Server
    {
        public string ClientPubKey;
        public string PublicKey;
        private string PrivateKey;
        public string userLogin;
        public string status;
        public string newUserLogin;
        public string newUserPass;
        public string decrypted;
        public string decrypted2;
        public string password;
        public string recommending;
        public double[] prices;
        public static Server server;

        MainFrame mainFrame;
        RSACryptoServiceProvider RSAserver;
        public Dictionary<string, string> dict = new Dictionary<string, string>();
        public Dictionary<string, int> recom = new Dictionary<string, int>();
        public Dictionary<string, double> spent = new Dictionary<string, double>();


        public Server() 
        {
            server = this;
            CipherGen();
            Login login = new Login();
            Loadfile();
            prices = new double[6];
            prices[0] = 500;
            prices[1] = 80;
            prices[2] = 120;
            prices[3] = 300;
            prices[4] = 30;
            prices[5] = 450;
        }
        
        public MainFrame Logging(Login login)
        {
            if (dict.ContainsKey(userLogin) )
            {  
                if (password == dict[userLogin])
                {
                    status = "Connected";
                    mainFrame = login.Logging();
                    UpdatePrices();
                }
                else
                {
                    status = "Wrong password";
                    mainFrame = login.Logging();
                }
            }
            else
            {
                status = "User doesn't exist";
                mainFrame = login.Logging();
            }
            return mainFrame;
        }

        public void CipherGen()
        {
            RSAserver = new RSACryptoServiceProvider(1024);
            PrivateKey = RSAserver.ToXmlString(true);
            PublicKey = RSAserver.ToXmlString(false);
        }

        public void action(MainFrame x)
        {
            x.Show();
        }

        public static string GetHash(string text)
        {
            string hash = "";
            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(text));
            hash = Convert.ToBase64String(result);
            return hash;
        }

        public void Loadfile()
        {
            try
            {
                string fileName = "client_database.txt";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    ReadFromFile(sr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read the file", ex);
            }
        }

        private void ReadFromFile(StreamReader sr)
        {
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;
                HandleLine(line);
            }
        }

        public void HandleLine(string line)
        {
            string[] splitted;
            splitted = line.Split(';');
            AddClient(splitted[0], splitted[1]);
            
        }

        public void ReceiveData(byte[] encrypted, byte[] encrypted2)
        {
            RSAserver.FromXmlString(PrivateKey);
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            userLogin = new string(encoding.GetChars(RSAserver.Decrypt(encrypted, false)));
            password = new string(encoding.GetChars(RSAserver.Decrypt(encrypted2, false)));
        }

        public void NewClientRequest(byte[] encrypted, byte[] encrypted2, byte[] encrypted3)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAserver.FromXmlString(PrivateKey);
            newUserLogin = new string(encoding.GetChars(RSAserver.Decrypt(encrypted, false)));
            newUserPass = new string(encoding.GetChars(RSAserver.Decrypt(encrypted2, false)));
            recommending = new string(encoding.GetChars(RSAserver.Decrypt(encrypted3, false)));
        }

        public void RegisterClient()
        {
            dict.Add(newUserLogin, newUserPass);
            recom.Add(newUserLogin, 0);
            spent.Add(newUserLogin, 0);
            int x = recom[userLogin];
            recom[userLogin] = x + 1;
        }

        public void AddClient(string a, string b)
        {
            dict.Add(a, GetHash(b));
            recom.Add(a, 0);
            spent.Add(a, 0);
        }

        public void RemoveClient(string a)
        {
            dict.Remove(a);
            recom.Remove(a);
            spent.Remove(a);
        }

        public void AddSpent(byte[] encrypted, byte[] encrypted2)
        {
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAserver.FromXmlString(PrivateKey);
            userLogin = new string(encoding.GetChars(RSAserver.Decrypt(encrypted2, false)));
            double x = spent[userLogin];
            spent[userLogin] = x + Convert.ToDouble(new string(encoding.GetChars(RSAserver.Decrypt(encrypted, false))));
        }

        public void UpdatePrices()
        {
            double x = new double();
            if (spent[userLogin] > 2500)
            {
                if (spent[userLogin] > 5000)
                {
                    if (spent[userLogin] > 7500)
                    {
                        x = 0.15;
                    }
                    else x = 0.1;
                }
                else x = 0.05;
            }
            else x = 0;
            double discount = new double();
            discount = 1 - (0.02 * recom[userLogin] + x);
            for (int i = 0; i < 6; i++)
            {
                prices[i] = prices[i] * discount;
            }
            System.Text.Encoding encoding = System.Text.Encoding.ASCII;
            RSAserver.FromXmlString(ClientPubKey);
            mainFrame.LoadPrices(RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[0])), false), RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[1])), false), RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[2])), false), RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[3])), false), RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[4])), false), RSAserver.Encrypt(encoding.GetBytes(Convert.ToString(prices[5])), false));
            ResetPrices();
        }

        public void ResetPrices()
        {
            prices[0] = 500;
            prices[1] = 80;
            prices[2] = 120;
            prices[3] = 300;
            prices[4] = 30;
            prices[5] = 450;
        }
    }
}
