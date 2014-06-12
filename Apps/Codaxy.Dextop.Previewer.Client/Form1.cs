using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Codaxy.Dextop.Previewer.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //webBrowser1.p

            watcher = new FileSystemWatcher();
            watcher.EnableRaisingEvents = false;
            watcher.Changed += watcher_Changed;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var fileInfo = new FileInfo(tbSrcPath.Text);
            if (e.ChangeType == WatcherChangeTypes.Changed && e.Name.ToLower().StartsWith(fileInfo.Name.ToLower()))
                Reload(true);
        }

        object lockObject = new object();

        private void btnGo_Click(object sender, EventArgs e)
        {
            Reload(false);
        }

        private void Reload(bool silent)
        {
            if (!Monitor.TryEnter(lockObject))
                return;

            try
            {
                var fileInfo = new FileInfo(tbSrcPath.Text);
                if (fileInfo.Exists)
                    watcher.Path = fileInfo.DirectoryName;
                var data = GetEncodedPostData(
                    new Dictionary<String, String> { { "FormWidth", (this.Width - 80).ToString() } }, 
                    new Dictionary<string, string> { { "file", tbSrcPath.Text } });
                webBrowser1.Navigate(tbServer.Text, "", data.data, data.headers);
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }

        public struct PostData
        {
            public byte[] data;
            public string headers;
        }
        
        static PostData GetEncodedPostData(Dictionary<String, String> values, Dictionary<String, String> files)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            if (values != null)
                foreach (var v in values)
                {
                    string formitem = string.Format(formdataTemplate, v.Key, v.Value);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

            if (files != null)
                foreach (var v in files)
                {

                    FileStream fileStream = new FileStream(v.Value, FileMode.Open, FileAccess.Read);

                    string header = string.Format(headerTemplate, v.Key, v.Value);
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }

                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    fileStream.Close();
                }

            PostData result = new PostData();

            memStream.Position = 0;
            result.data = new byte[memStream.Length];
            memStream.Read(result.data, 0, result.data.Length);
            memStream.Close();

            result.headers = "Content-Type: multipart/form-data; boundary=" + boundary + "\r\n" +
                             "Content-Length: " + result.data.Length + "\r\n" +
                             "\r\n";

            return result;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        FileSystemWatcher watcher;

        private void btnWatch_Click(object sender, EventArgs e)
        {
            var watching = btnWatch.Text != "Watch";            

            if (watching)
            {
                watcher.EnableRaisingEvents = false;

                btnWatch.Text = "Watch";
            }
            else
            {
                btnGo_Click(sender, e);

                try
                {
                    watcher.EnableRaisingEvents = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                

                btnWatch.Text = "Stop Watching";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
