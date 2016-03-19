using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

           
        }
        string path;

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TEXT|*.txt";
            ofd.Title = "Open Text";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(File.OpenRead(ofd.FileName));
                textBox1.Text = sr.ReadToEnd();
                button3.Enabled = true;
                path = ofd.FileName;
                sr.Dispose();
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(File.Create(path));
            sw.Write(path);
            SimpleAES enc = new SimpleAES();
            MessageBox.Show(enc.EncryptToString(path)); 
            
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(fbd.SelectedPath + "File_encrypt.txt",true))
                {
                    writer.Write(enc.EncryptToString(path));
                }
            }
        sw.Dispose();
        SaveFileDialog sfd = new SaveFileDialog();
        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            using (StreamWriter writer = new StreamWriter(sfd.FileName, true))
            {
                writer.Write(enc.EncryptToString(path));
            }
        }
        }
        public class SimpleAES
        {
            
            // Change these keys
            private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
            private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };


            private ICryptoTransform EncryptorTransform, DecryptorTransform;
            private System.Text.UTF8Encoding UTFEncoder;

            public SimpleAES()
            {
                //This is our encryption method
                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();
            }

            /// -------------- Two Utility Methods (not used but may be useful) -----------
            /// Generates an encryption key.
            static public byte[] GenerateEncryptionKey()
            {
                //Generate a Key.
                RijndaelManaged rm = new RijndaelManaged();
                rm.GenerateKey();
                return rm.Key;
            }

            /// Generates a unique encryption vector
            static public byte[] GenerateEncryptionVector()
            {
                //Generate a Vector
                RijndaelManaged rm = new RijndaelManaged();
                rm.GenerateIV();
                return rm.IV;
            }


            /// ----------- The commonly used methods ------------------------------    
            /// Encrypt some text and return a string suitable for passing in a URL.
            public string EncryptToString(string TextValue)
            {
                return ByteArrToString(Encrypt(TextValue));
            }

            /// Encrypt some text and return an encrypted byte array.
            public byte[] Encrypt(string TextValue)
            {
                //Translates our text value into a byte array.
                Byte[] bytes = UTFEncoder.GetBytes(TextValue);

                //Used to stream the data in and out of the CryptoStream.
                MemoryStream memoryStream = new MemoryStream();

                /*
                 * We will have to write the unencrypted bytes to the stream,
                 * then read the encrypted result back from the stream.
                 */
                #region Write the decrypted value to the encryption stream
                CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                #endregion

                #region Read encrypted value back out of the stream
                memoryStream.Position = 0;
                byte[] encrypted = new byte[memoryStream.Length];
                memoryStream.Read(encrypted, 0, encrypted.Length);
                #endregion

                //Clean up.
                cs.Close();
                memoryStream.Close();

                return encrypted;
            }
            public string ByteArrToString(byte[] byteArr)
            {
                byte val;
                string tempStr = "";
                for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
                {
                    val = byteArr[i];
                    if (val < (byte)10)
                        tempStr += "00" + val.ToString();
                    else if (val < (byte)100)
                        tempStr += "0" + val.ToString();
                    else
                        tempStr += val.ToString();
                }
                return tempStr;
            }
        }

       
    }
}
