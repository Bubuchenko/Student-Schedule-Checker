using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Student_Schedule_Checker
{
    public partial class Form1 : Form
    {
        public static Form1 form;
        public Form1()
        {
            InitializeComponent();
            form = this;
        }

        public const string LDAP_URL = "LDAP://OU=MCO Leerlingen,OU=MCO,DC=MCO,DC=local";
        public const string USERNAME = "roosterzoeker";
        public const string PASSWORD = "Welkom1337!";


        private async Task<string> GetStudentClass(string studentnumber)
        {
            await Task.Delay(0);
            try
            {
                using (DirectoryEntry dir = new DirectoryEntry(LDAP_URL)) //Instantiate dir entry and pass the domain
                {
                    dir.Username = USERNAME;
                    dir.Password = PASSWORD;

                    using (DirectorySearcher search = new DirectorySearcher(dir)) //Search query instance
                    {
                        search.Filter = "(&(objectClass=user)(pager=" + studentnumber + "))"; //Filter by pager (Student number)
                        search.PropertiesToLoad.Add("telephoneNumber"); //Allows us to use the "pager" property to search by student ID
                        SearchResult searchresult = search.FindOne();

                        using (DirectoryEntry uEntry = searchresult.GetDirectoryEntry())
                        {
                            string leerlingnaam = uEntry.Properties["givenName"].Value.ToString() + " " + uEntry.Properties["sn"].Value.ToString(); //Store full student name in string
                            string LDAPDescription = uEntry.Properties["memberOf"][1].ToString();

                            //Clean it up to only return the students class id
                            return LDAPDescription.Substring(LDAPDescription.IndexOf('=') + 1, LDAPDescription.IndexOf(',') - 3) + "@" + leerlingnaam;
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private async void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                textBox1.BackColor = Color.White;
                string student = textBox1.Text;
                string StudentInfo = await Task.Run(() => GetStudentClass(student));

                if (StudentInfo.Length > 0)
                {
                    string Name = StudentInfo.Split('@')[1];
                    string Class = StudentInfo.Split('@')[0];

                    RoosterView roosterWindow = new RoosterView(Class, Name);
                    roosterWindow.Show();
                    this.Hide();
                }
                else
                {
                    textBox1.BackColor = Color.Red;
                    textBox1.Text = "";
                }
            }
        }
    }
}
