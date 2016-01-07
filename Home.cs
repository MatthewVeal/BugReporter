using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bug_Reporter
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bug Reporting Program              \n\r\n\rLeeds Beckett University               \n\r\n\r(C) 2015 Matthew Veal");
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void addBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_bug frmchild = new Add_bug();
            frmchild.Show();
        }

        private void loadBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Load_bug frmchild = new Load_bug();
            frmchild.Show();
        }




    }
}
