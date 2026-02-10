using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    public partial class MyLicencesHistory : Form
    {
        private int _PersonID;
        public MyLicencesHistory(int personID)
        {
            InitializeComponent();
            _PersonID = personID;
        }

        private void ctrlPersonCardWithFilter1_Load(object sender, EventArgs e)
        {

        }

        private void MyLicencesHistory_Load(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.LoadPersonInfo(_PersonID);
        }
    }
}
