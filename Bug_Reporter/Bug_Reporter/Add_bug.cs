using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using ICSharpCode.TextEditor.Document;
using System.IO;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;

namespace Bug_Reporter
{
    public partial class Add_bug : Form
    {

        private string CurrentPath = "/";

        SqlCeConnection mySqlConnection;

        public Add_bug()
        {
            InitializeComponent();
            populateListBox();
        }

        private void Add_bug_Load(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
            {
                this.GetAccessToken();
            }
            else
            {
                this.GetFiles();
            }
        }
        //Connect to Dropbox Through API Key
        private void GetAccessToken()
        {
            var login = new DropboxLogin("fzn733yzll43oiu", "gct1gbat6tz6k3j");
            login.Owner = this;
            login.ShowDialog();

            if (login.IsSuccessfully)
            {
                Properties.Settings.Default.AccessToken = login.AccessToken.Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("error...");
            }
        }
        //Get the files from Dropbox
        private void GetFiles()
        {
            OAuthUtility.GetAsync
                (
                "https://api.dropboxapi.com/1/metadata/auto/",
                new HttpParameterCollection
                {
                    { "path", this.CurrentPath },
                    { "access_token", Properties.Settings.Default.AccessToken }
                },
                callback: GetFiles_Result
                );
        }
        //Add Files to list Box
        private void GetFiles_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(GetFiles_Result), result);
                return;
            }
            if (result.StatusCode == 200)
            {
                listBox1.Items.Clear();
                listBox1.DisplayMember = "path";


                foreach (UniValue file in result["contents"])
                {
                    listBox1.Items.Add(file);

                }
                if (this.CurrentPath != "/")
                {
                    listBox1.Items.Insert(0, UniValue.ParseJson("{path: '..'}"));
                }

            }
            else
            {
                MessageBox.Show("Error");
            }
        }
        
        private void Upload_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(Upload_Result), result);
                return;
            }

            if (result.StatusCode == 200)
            {
                this.GetFiles();
            }
            else
            {
                if (result["error"].HasValue)
                {
                    MessageBox.Show(result["error"].ToString());

                }
                else
                {
                    MessageBox.Show(result.ToString());


                }

            }
        }
       

        //Code for selecting SQL Statement
        public void populateListBox()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            String selcmd = "SELECT bug_id, code, status, importance, line FROM tblBug ORDER BY bug_id";

            SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

            try
            {
                //Open SQL
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            }
            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void cleartxtBoxes()
        {
            txtId.Text = txtCode.Text = txtStatus.Text = cmbImportance.Text = "";
        }

        public bool checkInputs()
        {
            bool rtnvalue = true;

            //Check if empty
            if (string.IsNullOrEmpty(txtId.Text) ||
                string.IsNullOrEmpty(txtCode.Text) ||
                string.IsNullOrEmpty(txtStatus.Text) ||
                string.IsNullOrEmpty(cmbImportance.Text) ||
                string.IsNullOrEmpty(txtLine.Text))
            {
                MessageBox.Show("Error: Please check your inputs");
                rtnvalue = false;
            }

            return (rtnvalue);

        }

        public void insertRecord(String ID, String code, String status, String importance, String line, String commandString)
        {

            try
            {
                SqlCeCommand cmdInsert = new SqlCeCommand(commandString, mySqlConnection);
                //Add parameters to textsbox to link with SQL
                cmdInsert.Parameters.AddWithValue("@ID", ID);
                cmdInsert.Parameters.AddWithValue("@code", code);
                cmdInsert.Parameters.AddWithValue("@status", status);
                cmdInsert.Parameters.AddWithValue("@importance", importance);
                cmdInsert.Parameters.AddWithValue("@line", line);
                cmdInsert.ExecuteNonQuery();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(ID + " .." + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //When the button is clicked run the path to load file to dropbox
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            OAuthUtility.PutAsync("https://content.dropboxapi.com/1/files_put/auto/", new HttpParameterCollection
            {
                {"access_token",Properties.Settings.Default.AccessToken},
                {"path", Path.Combine(this.CurrentPath, Path.GetFileName(openFileDialog1.FileName)).Replace("\\","/")},
                {"overwrite","true"},
                {"autorename", "true"},
                {openFileDialog1.OpenFile()},
            },
            callback: Upload_Result
                );

            if (checkInputs())
            {
                //Write an instert SQL statement to add fields to database
                String commandString = "INSERT INTO tblBug(bug_id, code, status, importance, line) VALUES (@ID, @code, @status, @importance, @line)";

                insertRecord(txtId.Text, txtCode.Text, txtStatus.Text, cmbImportance.Text, txtLine.Text, commandString);
                populateListBox();
                cleartxtBoxes();
                MessageBox.Show("Succesfully reported your bug to the database!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleartxtBoxes();
        }

        //Code for Text Editor
        private void txtCode_Load(object sender, EventArgs e)
        {
            string dirc = Application.StartupPath;
            FileSyntaxModeProvider fsmp;
            if (Directory.Exists(dirc))
            {
                fsmp = new FileSyntaxModeProvider(dirc);
                HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmp);
                txtCode.SetHighlighting("C#");
            }
        }
        //code so you can double click through the files on dropbox
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            {
                if (listBox1.SelectedItem == null) { return; }
                UniValue file = (UniValue)listBox1.SelectedItem;

                if (file["path"] == "..")
                {
                    if (this.CurrentPath != "/")
                    {
                        this.CurrentPath = Path.GetDirectoryName(this.CurrentPath).Replace("\\", "/");
                    }
                }
                else
                {
                    if (file["is_dir"] == 1)
                    {
                        this.CurrentPath = file["path"].ToString();

                    }
                    else
                    {
                        saveFileDialog1.FileName = Path.GetFileName(file["path"].ToString());
                        if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }

                        //var web = new WebClient();
                        //web.DownloadFile(String.Format("https://content.dropboxapi.com/1/files/auto/{0}?access_token={1}", file["path"], Properties.Settings.Default.AccessToken), saveFileDialog1.FileName);
                    }
                }
                this.GetFiles();
            }
        }

        
    }
}
