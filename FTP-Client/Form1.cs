using System;
using System.Net;
using System.Web;
using System.IO;
using System.Net.Cache;

namespace FTP_Client
{
    public partial class Form1 : Form
    {
        private string? server_address;
        private string? login;
        private string? password;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server_address = textBox1.Text;
            login = textBox2.Text;
            password = textBox3.Text;
            try
            { 
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create($"ftp://{server_address}");
                request.Credentials = new NetworkCredential(login, password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = request.GetResponse();
                Form2 newForm = new Form2(server_address, login, password);
                this.Hide();
                newForm.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}