using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class WorkAreaUserControl : UserControl
    {
        public WorkAreaUserControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
    }
}
