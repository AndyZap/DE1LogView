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

        public void ShowLog(string text)
        {
            richLog.Text = text;
            richLog.Visible = true;
        }
        public void ShowGraph()
        {
            richLog.Visible = false;

            if (Graph == null)
                Graph = new GraphPainter(panel1, this.Font);

            if (parent.SecondPlotKey != "")
                parent.PlotDataRec(Graph, parent.Data[parent.SecondPlotKey]);
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
    }
}
