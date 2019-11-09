using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class FormBigPlot : Form
    {
        public Form1 parent = null;
        public GraphPainter Graph = null;

        public FormBigPlot()
        {
            InitializeComponent();
        }

        private void FormBigPlot_Load(object sender, EventArgs e)
        {
            Graph = new GraphPainter(panel1, this.Font);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (Graph != null)
                Graph.Plot(e.Graphics);
        }

        private void FormBigPlot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27) // F12
            {
                Hide();
            }
        }

        private void FormBigPlot_Shown(object sender, EventArgs e)
        {
            parent.PlotDataRec(Graph, parent.Data[parent.SelectedPlots]);
        }
    }
}
