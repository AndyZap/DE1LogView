using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            Graph = new GraphPainter(panel1, this.Font);
            Graph.border_y2 = 100;
        }

        public void SetSplitter(int w)
        {
            splitBigPlot.SplitterDistance = w;
        }
        public int GetSplitter()
        {
            return splitBigPlot.SplitterDistance;
        }

        public void ShowLog(string text)
        {
            richLog.Text = text;
            splitBigPlot.Dock = DockStyle.Fill;
            splitBigPlot.Visible = true;
        }
        public void ShowGraph()
        {
            splitBigPlot.Visible = false;

            if (parent.MainPlotKey != "" && (parent.RefPlotKey == "" || parent.MainPlotKey == parent.RefPlotKey))
                parent.PlotDataRec(Graph, parent.Data[parent.MainPlotKey]);
            else
            {
                Form1.DataStruct ds1 = parent.Data[parent.MainPlotKey];
                Form1.DataStruct ds2 = parent.Data[parent.RefPlotKey];

                labelTopL.Text = ds1.getAsInfoTextForGraph (parent.ProfileInfoList, parent.BeanList, DateTime.Now);
                labelTopL1.Text = ds2.getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList, DateTime.Now);

                Graph.SetAxisTitles("", "");
                Graph.data.Clear();

                Graph.SetData(0, ds2.elapsed, ds2.flow, Color.Blue, 3, DashStyle.Dash);
                Graph.SetData(1, ds2.elapsed, ds2.pressure, Color.LimeGreen, 3, DashStyle.Dash);
                Graph.SetData(2, ds2.elapsed, ds2.flow_weight, Color.Brown, 3, DashStyle.Dash);

                Graph.SetData(3, ds1.elapsed, ds1.flow, Color.Blue, 3, DashStyle.Solid);
                Graph.SetData(4, ds1.elapsed, ds1.pressure, Color.LimeGreen, 3, DashStyle.Solid);
                Graph.SetData(5, ds1.elapsed, ds1.flow_weight, Color.Brown, 3, DashStyle.Solid);

                List<double> temperature_scaled1 = new List<double>();
                List<double> temperature_scaled2 = new List<double>();
                foreach (var t in ds1.temperature_basket)
                    temperature_scaled1.Add(t / 10.0);
                foreach (var t in ds2.temperature_basket)
                    temperature_scaled2.Add(t / 10.0);

                Graph.SetData(6, ds2.elapsed, temperature_scaled2, Color.Red, 3, DashStyle.Dash);
                Graph.SetData(7, ds1.elapsed, temperature_scaled1, Color.Red, 3, DashStyle.Solid);

                Graph.SetAutoLimits();

                Graph.panel.Refresh();
            }
        }
        public void SetLabelText (string s)
        {
            labelTopL.Text = s;
            labelTopL1.Text = "";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (Graph != null)
                Graph.Plot(e.Graphics);
        }

        private void FormBigPlot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27) // Esc
            {
                Hide();
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = Graph.ToDataX(e.X);
            var y = Graph.ToDataY(e.Y);

            labelTopR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
        }
    }
}
