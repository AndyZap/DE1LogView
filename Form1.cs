using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class Form1 : Form
    {
        string Revision = "DE1 Log View v1.3";
        string ApplicationDirectory = "";
        string ApplicationNameNoExt = "";

        // to draw GBR bars in listData_DrawItem
        readonly int _BP1 = 20;
        readonly int _BP2 = 30;
        readonly double _REF_BEAN = 18.0;

        // these are used to color-code values, in listData_DrawItem only
        readonly double _MIN_R = 1.5;
        readonly double _RANGE_R = 1.2;

        GraphPainter GraphTop = null;
        GraphPainter GraphBot = null;

        List<int> WeightPoints = new List<int>();

        public Form1()
        {
            InitializeComponent();

            // TODO: make this configurable
            WeightPoints.Add(_BP1);
            WeightPoints.Add(_BP2);

            HeatMapP = getHeatmap(1);
            HeatMapR = getHeatmap(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Revision;

            GraphTop = new GraphPainter(splitContainer2.Panel1, this.Font);
            GraphBot = new GraphPainter(splitContainer2.Panel2, this.Font);

            ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            ApplicationNameNoExt = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            LoadSettings();


            string data_fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".csv";
            string old_data_fname = ApplicationDirectory + "\\CoffeeLogger.csv";
            if ((File.Exists(data_fname) == false) && (File.Exists(old_data_fname) == true))
                ReadOldFileFormat(old_data_fname);
            else if (File.Exists(data_fname))
                ReadAllRecords(data_fname);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void listData_SelectedIndexChanged(object sender, EventArgs e)
        {
            listData.Refresh();

            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            labNotes.Text = String.IsNullOrEmpty(Data[key].notes) ? "" : Data[key].notes;

            PlotDataRec(GraphTop, Data[key]);

            // plot pressure
            /*
            GraphBot.data.Clear();

            y = Data[key].pressure;
            x.Clear();
            for (int i = 0; i < y.Count; i++)
            x.Add(i);

            GraphBot.SetData(0, x, y, Color.Blue, 4, GraphPainter.Style.Solid);

            for (int si = 0; si < SelectedPlots.Count; si++)
            {
            var skey = SelectedPlots[si];

            List<double> sy = Data[skey].pressure;
            List<double> sx = new List<double>();
            for (int i = 0; i < sy.Count; i++)
            sx.Add(i);

            GraphBot.SetData(si + 1, sx, sy, GetPenColor(si), 4, GraphPainter.Style.Solid);
            }

            GraphBot.SetAutoLimits();

            splitContainer2.Panel2.Refresh();
            */
        }

        private void PlotDataRec(GraphPainter gp, DataStruct ds)
        {
            labelTopL.Text = ds.name + "  " + ds.profile;
            labelTopR.Text = ds.coffee_weight.ToString() + "g";

            gp.SetAxisTitles("", "");

            gp.data.Clear();

            gp.SetData(0, ds.elapsed, ds.flow_goal, Color.Blue, 2, DashStyle.Dash);
            gp.SetData(1, ds.elapsed, ds.pressure_goal, Color.LimeGreen, 2, DashStyle.Dash);

            gp.SetData(2, ds.elapsed, ds.flow, Color.Blue, 3, DashStyle.Solid);
            gp.SetData(3, ds.elapsed, ds.pressure, Color.LimeGreen, 3, DashStyle.Solid);

            gp.SetData(4, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid);

            gp.SetAutoLimits();

            gp.panel.Refresh();
        }


        private void listData_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listData.Items.Count)
                return;

            string key = (string)listData.Items[e.Index];

            if (!Data.ContainsKey(key))
                return;

            DataStruct d = Data[key];

            Brush myBrush = Brushes.Black;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Rectangle myrec = e.Bounds;

            myrec.Y += 5;  // move the text a bit down

            myrec.X = labName.Left; myrec.Width = labName.Width;
            e.Graphics.DrawString(d.name, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labID.Left; myrec.Width = labID.Width;
            e.Graphics.DrawString(d.id.ToString(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDate.Left; myrec.Width = labDate.Width;

            var dn = DateTime.Now;
            var dn1 = new DateTime(dn.Year, dn.Month, dn.Day, d.date.Hour, d.date.Minute, d.date.Second);

            TimeSpan ts = dn1 - d.date;
            string ddd = "";
            if (ts.TotalDays < 1)
                ddd = "T0 " + d.date.ToString("HH:mm");
            else
            {
                var total = ts.TotalDays;

                int years = (int)(total / 365.0);
                total -= years * 365;

                int months = (int)(total / 30.0);
                total -= months * 30;

                ddd = (years == 0 ? "" : years.ToString() + "y") + (months == 0 ? "" : months.ToString() + "m") + total.ToString("0") + "d";

                if (years == 0 && months == 0)
                    ddd += " " + d.date.ToString("HH:mm");
            }

            var wp = d.getWeightPoints(WeightPoints);

            e.Graphics.DrawString(ddd, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labBrewType.Left; myrec.Width = labBrewType.Width;
            //e.Graphics.DrawString(d.getBrewType().ToString().Substring(0, 1), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labBeanWeight.Left; myrec.Width = labBeanWeight.Width;
            e.Graphics.DrawString((d.bean_weight - _REF_BEAN).ToString("0.0").PadLeft(4), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labCoffeeWeight.Left; myrec.Width = labCoffeeWeight.Width;
            e.Graphics.DrawString(d.coffee_weight.ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labRatio.Left; myrec.Width = labRatio.Width;

            var ratio = (d.coffee_weight / d.bean_weight);
            if (!PrintGBR)
            {
                var c = getHeatmapColor((ratio - _MIN_R) / _RANGE_R, HeatMapR);
                e.Graphics.FillRectangle(c, myrec);
            }

            e.Graphics.DrawString(PrintGBR ? (wp[0] * 100).ToString("0") : ratio.ToString("0.00"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labGrind.Left; myrec.Width = labGrind.Width;
            e.Graphics.DrawString(PrintGBR ? (wp[1] * 100).ToString("0") : d.grind, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labTime.Left; myrec.Width = labTime.Width;
            e.Graphics.DrawString(PrintGBR ? (wp[2] * 100).ToString("0") : d.time.ToString(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);



            myrec.Y -= 5; // adjust back

            // plot color.  0th plot is the current one, plotted as Blue
            myrec.X = labHasPlot.Left + 2; myrec.Width = labHasPlot.Width - 5;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.Blue, myrec);
            else
                e.Graphics.FillRectangle(GetBrushColor(d.saved_plot_index), myrec);

            // plot weight points - LAST! - as we change myrec size

            myrec.X = labGBR.Left - 5; myrec.Width = labGBR.Width / 3;

            var original_height = myrec.Height;
            var original_y = myrec.Y;

            myrec.Height = (int)(original_height * wp[0]);
            myrec.Y = original_y + (original_height - myrec.Height);
            e.Graphics.FillRectangle(Brushes.SeaGreen, myrec);

            myrec.X += myrec.Width;
            myrec.Height = (int)(original_height * wp[1]);
            myrec.Y = original_y + (original_height - myrec.Height);
            e.Graphics.FillRectangle(Brushes.Blue, myrec);

            myrec.X += myrec.Width;
            myrec.Height = (int)(original_height * wp[2]);
            myrec.Y = original_y + (original_height - myrec.Height);
            e.Graphics.FillRectangle(Brushes.Red, myrec);
        }

        private bool CanAddLine()
        {
            return SelectedPlots.Count < 7;
        }
        private Brush GetBrushColor(int index)
        {
            if (index == -1) return Brushes.White;
            else if (index == 0) return Brushes.Red;
            else if (index == 1) return Brushes.Green;
            else if (index == 2) return Brushes.Fuchsia;
            else if (index == 3) return Brushes.Black;
            else if (index == 4) return Brushes.Lime;
            else if (index == 5) return Brushes.Chocolate;
            else if (index == 6) return Brushes.Gold;
            else return Brushes.White;
        }
        private Color GetPenColor(int index)
        {
            if (index == -1) return Color.White;
            else if (index == 0) return Color.Red;
            else if (index == 1) return Color.Green;
            else if (index == 2) return Color.Fuchsia;
            else if (index == 3) return Color.Black;
            else if (index == 4) return Color.Lime;
            else if (index == 5) return Color.Chocolate;
            else if (index == 6) return Color.Gold;
            else return Color.White;
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (GraphTop != null)
                GraphTop.Plot(e.Graphics);
        }
        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (GraphBot != null)
                GraphBot.Plot(e.Graphics);
        }

        private void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            List<string> sorted_keys = new List<string>();

            var flt_name = txtFilterName.Text.Trim().ToLower();

            foreach (var key in Data.Keys)
            {
                if (!String.IsNullOrEmpty(flt_name) && Data[key].name.Contains(flt_name) == false)
                    continue;

                if (Data[key].bean_weight < 0)
                    continue;

                sorted_keys.Add(key);
            }

            sorted_keys.Sort();


            string saved_key = "";
            if (listData.SelectedIndex != -1)
                saved_key = (string)listData.Items[listData.SelectedIndex];

            listData.Items.Clear();
            bool saved_key_set = false;
            for (int i = sorted_keys.Count - 1; i >= 0; i--)
            {
                listData.Items.Add(sorted_keys[i]);
                if (sorted_keys[i] == saved_key)
                {
                    listData.SelectedItem = sorted_keys[i];
                    saved_key_set = true;
                }
            }
            if (!saved_key_set && listData.Items.Count != 0)
                listData.SelectedIndex = 0;

            listData.Refresh();
        }

        private void txtFilterName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
                txtFilterName.Text = "";
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            listData.Height = splitContainer1.Panel2.Height - panel1.Height - panel2.Height - panel3.Height
            - panel4.Height - panel5.Height - 5;
        }

        List<string> SelectedPlots = new List<string>();

        private void btnAddPlot_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex == -1)
                return;

            AddPlot(listData.SelectedIndex);
        }

        private void AddPlot(int index)
        {
            string key = (string)listData.Items[index];

            if (!Data.ContainsKey(key))
                return;

            foreach (string s in SelectedPlots)  // already selected
            {
                if (s == key)
                    return;
            }

            if (!CanAddLine())
            {
                MessageBox.Show("Cannot add more plots");
                return;
            }

            Data[key].saved_plot_index = SelectedPlots.Count;
            SelectedPlots.Add(key);

            listData.Focus();
            listData_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void btnDelPlots_Click(object sender, EventArgs e)
        {
            foreach (var key in Data.Keys)
                Data[key].saved_plot_index = -1;

            SelectedPlots.Clear();

            listData.Focus();
            listData_SelectedIndexChanged(null, EventArgs.Empty);
        }

        bool PrintGBR = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyValue == 67)  // Ctrl C
                    CopyLine();
                if (e.KeyValue == 80)  // Ctrl P
                    CopyPressure();
            }

            if (e.KeyCode == Keys.Menu)
            {
                labRatio.Text = "G";
                labGrind.Text = "B";
                labTime.Text = "R";

                if (!PrintGBR)
                {
                    PrintGBR = true;
                    listData.Refresh();
                }

                e.Handled = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Menu)
            {
                labRatio.Text = "Ratio";
                labGrind.Text = "Grind";
                labTime.Text = "Time";
                PrintGBR = false;
                listData.Refresh();
                e.Handled = true;
            }
        }

        private void CopyLine()
        {
            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            var d = Data[key];

            StringBuilder sb = new StringBuilder();

            sb.Append("#" + d.id.ToString() + " " + d.date.ToString("MMM:y").Replace(":", "'") + " ");

            string t = "";
            sb.Append(t + ": ");
            sb.Append(d.bean_weight.ToString() + " -> ");
            sb.Append(d.coffee_weight.ToString("0.0") + " in ");
            sb.Append(d.time.ToString() + " sec, ratio ");
            sb.Append((d.coffee_weight / d.bean_weight).ToString("0.00") + " grind ");
            sb.Append(d.grind + " press ");

            txtCopy.Text = sb.ToString();
            txtCopy.SelectAll();
            txtCopy.Copy();
        }

        private void CopyPressure()
        {
            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            var d = Data[key];

            StringBuilder sb = new StringBuilder();

            foreach (var p in d.pressure)
                sb.Append(p.ToString("0.0" + "\t"));

            txtCopy.Text = sb.ToString();
            txtCopy.SelectAll();
            txtCopy.Copy();
        }

        private void listData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listData.IndexFromPoint(e.Location);
                if (index != -1)
                    AddPlot(index);
            }
        }

        private void btnDisableRec_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex == -1)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            Data[key].bean_weight *= -1;

            FilterData();
        }

        private void radioFlow_CheckedChanged(object sender, EventArgs e)
        {
            listData.Focus();
            listData_SelectedIndexChanged(null, EventArgs.Empty);
        }
    }
}