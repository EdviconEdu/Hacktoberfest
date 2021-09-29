using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurePassword_MD5
{
    public partial class SecurePassword : Form
    {
        public SecurePassword()
        {
            InitializeComponent();
        }

        private string hash() {
            var hash = new Salt();
            return hash.ToString();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            lblHash.Visible = true;
            lblResults.Visible = false;
            byte[] data = UTF8Encoding.UTF8.GetBytes(txtPassword.Text);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash()));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    lblHash.Text = Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (txtValidation.Text != "")
            {
                byte[] data = Convert.FromBase64String(lblHash.Text); // decrypt the incrypted text
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash()));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        string decrypted = UTF8Encoding.UTF8.GetString(results);
                        if (decrypted == txtValidation.Text)
                        {
                            lblResults.Visible = true;
                            lblResults.ForeColor = Color.Green;
                            lblResults.Text = "Valid Password Detected!";
                        }
                        else
                        {
                            lblResults.Visible = true;
                            lblResults.ForeColor = Color.Red;
                            lblResults.Text = "Invalid Password Detected!";
                        }

                    }
                }
            }
            else {
                lblResults.Visible = true;
                lblResults.ForeColor = Color.Red;
                lblResults.Text = "CAUTION! Enter a Password for the Validation!";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:nimesh.ekanayaka7@gmail.com");  
        }
    }
}
