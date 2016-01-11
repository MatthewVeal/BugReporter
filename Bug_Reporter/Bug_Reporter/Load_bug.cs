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


namespace Bug_Reporter
{
    public partial class Load_bug : Form
    {


        SqlCeConnection mySqlConnection;
        Font defaultFont = SystemFonts.DefaultFont;
        public Load_bug()
        {
            InitializeComponent();
            populateListBox();
        }

        private void Load_bug_Load(object sender, System.EventArgs e)
        {
          
            throw new System.NotImplementedException();
        }

   

        public void populateListBox()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            //Writes a sql query to select everything from tbl_bug

            String selcmd = "SELECT bug_id, code, status, importance, line FROM tblBug ORDER BY bug_id";

            SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);
            try
            {
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                listView1.Items.Clear();

                listView1.Font = new Font(defaultFont.FontFamily, defaultFont.Size, FontStyle.Bold);

                while (mySqlDataReader.Read())
                {
                    //Adds Everything from the the database in to a listbox
                    ListViewItem item1 = new ListViewItem(mySqlDataReader["bug_id"] + ":");
                    item1.SubItems.Add(mySqlDataReader["line"] + "");
                    item1.SubItems.Add(mySqlDataReader["code"] + "");
                    item1.SubItems.Add(mySqlDataReader["status"] + "");
                    item1.SubItems.Add(mySqlDataReader["importance"] + "");


                    listView1.Items.AddRange(new ListViewItem[] { item1 });


                }
            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            edit_bug frmchild = new edit_bug();
            frmchild.Show();
        }

      

        }
    }

