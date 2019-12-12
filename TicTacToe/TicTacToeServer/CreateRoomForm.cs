using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToeServer
{
    public partial class CreateRoomForm : Form
    {
        public CreateRoomForm()
        {
            InitializeComponent();
        }

        public int deskSize = 20;
        private void CreateRoomForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.deskSize = Convert.ToInt32(txtDeskSize.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
