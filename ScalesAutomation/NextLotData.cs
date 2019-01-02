using System;
using System.Windows.Forms;

namespace ScalesAutomation
{
    public partial class NextLotData : Form
    {
        public delegate void CloseEvent();
        public CloseEvent WindowClosed;

        public delegate void ApplyClickedEvent();
        public CloseEvent ApplyClicked;

        public NextLotData()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyClicked();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NextLotData_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowClosed();
        }

        public LotInfo GetLotInfo()
        {
            return uctlLotData.GetLotInfo();
        }
    }
}
