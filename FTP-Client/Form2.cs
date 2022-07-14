using System;
using System.Net;
using System.Web;
using System.IO;
using System.Net.Cache;
using System.Text;

namespace FTP_Client
{
    public partial class Form2 : Form
    {
        private string server_address;
        private string login;
        private string password;
        public Form2(string server_addr, string log, string pass)
        {
            server_address = server_addr;
            login = log;
            password = pass;

            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = server_address;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = textBox1.Text == "" ? "" : textBox1.Text + "/";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{server_address}/{path}");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(login, password);
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                MessageBox.Show(reader.ReadToEnd());
                reader.Close();
                response.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\\";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create($@"ftp://{server_address}/" + textBox2.Text);
                    request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(login, password);

                    StreamReader sourceStream = new StreamReader(openFileDialog.FileName);
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    MessageBox.Show("Upload File Complete, status {0}", response.StatusDescription);

                    response.Close();
                }
                catch (WebException exc)
                {
                    MessageBox.Show(exc.Message.ToString());
                    String status = ((FtpWebResponse)exc.Response).StatusDescription;
                    MessageBox.Show(status);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.ToString());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = @"C:\\";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{server_address}/{textBox3.Text}");
                    request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(login, password);
                    request.KeepAlive = false;
                    request.UseBinary = true;
                    request.UsePassive = true;

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();

                    using (Stream fs = saveFileDialog.OpenFile())
                    {
                        responseStream.CopyTo(fs);
                    }

                    MessageBox.Show("Download Complete", response.StatusDescription);

                    responseStream.Close();
                    response.Close();
                }
                catch (WebException exc)
                {
                    MessageBox.Show(exc.Message.ToString());
                    String status = ((FtpWebResponse)exc.Response).StatusDescription;
                    MessageBox.Show(status);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.ToString());
                }
            }
        }
    }
}
