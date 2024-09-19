using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tayyab_Cloth_House
{
    public partial class Main : Form
    {
        string role;
        string type = "user";
        private const int AnimationDuration = 500;
        private Timer slideTimer;
        private int startY;
        public void loadform(object Form)
        {
            if (this.mainpanel.Controls.Count > 0)
                this.mainpanel.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainpanel.Controls.Add(f);
            this.mainpanel.Tag = f;
            f.Show();

        }
        public Main()
        {
            InitializeComponent();
        }

      

     

        private void itemsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            loadform(new Add_Item() );
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you want to sure to Exit?", "confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {

                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Application.Exit();
            }
        }

        private void stocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Form1());
        }

        private void investorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Investor());
        }

        private void stocksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadform(new view_stocks());
        }

        private void profitDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new ProfitDistribution());
        }

        private void defectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Defected());
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Dashboard());
        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Sales());
        }

        private void salesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadform(new viewsales());
        }

        private void returnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Return());
        }

        private void returnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadform(new viewreturn());
        }

        private void mainpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void termsAndPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Terms_and_Conditions());
        }

        private void profitDsitributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadform(new Showprofitdist());

        }
    }
}
