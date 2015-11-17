using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Windows;

namespace Student_Schedule_Checker
{
    public partial class RoosterView : Form
    {
        string url;

        public RoosterView(string Class, string StudentName)
        {
            InitializeComponent();
            ((Control)webBrowser1).Enabled = false;
            url = string.Format("http://www.meetingpointmco.nl/roostersMCO/dagroosterklas/Kla1_{0}.htm", Class.Replace("-", "_"));
            this.Text = string.Format("Rooster van {0} uit klas {1}", StudentName, Class);
        }

        private async void RoosterView_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = await readStream.ReadToEndAsync();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(data);
                data = doc.DocumentNode.SelectSingleNode("/html/body/center/table[1]").OuterHtml;

                //Center the content
                data = data.Insert(0, "<center>");
                data = data.Insert(data.Length, "</center>");

                //Reset the content if it something was already loaded
                webBrowser1.Navigate("about:blank");
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.Write(string.Empty);
                }
                webBrowser1.DocumentText = data;
                response.Close();
                readStream.Close();
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int width = webBrowser1.Document.Body.ScrollRectangle.Size.Width + 75;
            int height = webBrowser1.Document.Body.ScrollRectangle.Size.Height + 30;

            this.Width = width;
            this.Height = height;

        }

        private void RoosterView_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Form1.form.Show();
        }
    }
}
