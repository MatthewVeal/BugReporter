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
    public partial class Register : Form
    {

        SqlCeConnection mySqlConnection;

        public Register()
        {
            InitializeComponent();
            populateListBox();
        }

        public void populateListBox()
        {
            //establish a connection
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            String selcmd = "SELECT user_name, password FROM users ORDER BY user_name";

            SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

            try
            {
                //Open the connection with database
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public bool checkInputs()
        {
            bool rtnvalue = true;
            //Check to make sure the inputs are not empty
            if (string.IsNullOrEmpty(new_username.Text) ||
                string.IsNullOrEmpty(new_password.Text))
            {
                MessageBox.Show("Error: Please check your inputs");
                rtnvalue = false;
            }

            return (rtnvalue);

        }

        public void insertRecord(String user, String pass, String commandString)
        {

            try
            {
                SqlCeCommand cmdInsert = new SqlCeCommand(commandString, mySqlConnection);
                //Adds fields from textbox to the query
                cmdInsert.Parameters.AddWithValue("@user", user);
                cmdInsert.Parameters.AddWithValue("@pass", pass);
                cmdInsert.ExecuteNonQuery();
                MessageBox.Show("Succesfully signed up, you can now log in!");
                this.Close();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(" This username - " + user + " has already been used, try another!");
            }

        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            if (checkInputs())
            {
                //SQL statement to insert data
                String commandString = "INSERT INTO users(user_name, password) VALUES (@user, @pass)";
                insertRecord(new_username.Text, new_password.Text, commandString);
                populateListBox();
            }
        }
                    
    }
}
