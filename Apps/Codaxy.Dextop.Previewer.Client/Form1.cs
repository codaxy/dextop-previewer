﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Codaxy.Dextop.Previewer.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //webBrowser1.p
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                var data = GetEncodedPostData(null, new Dictionary<string, string> { { "file", tbSrcPath.Text } });
                webBrowser1.Navigate(tbServer.Text, "", data.data, data.headers);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}