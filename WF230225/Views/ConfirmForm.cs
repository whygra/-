using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF230225.Views
{
    public partial class ConfirmForm : Form
    {
        string _message;

        public ConfirmForm(string message = null)
        {
            InitializeComponent();
            _message = message ?? "";
        }

        // по загрузке формы передаем сообщение в TextBox
        private void ConfirmForm_Load(object sender, EventArgs e)
        {
            txbConfirmMessage.Text = _message;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
