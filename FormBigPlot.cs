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
            labelTopL.Visible = false;
            labelTopL1.Visible = false;
            labelTopR.Visible = false;

            richLog.Text = text;
            splitBigPlot.Dock = DockStyle.Fill;
            splitBigPlot.Visible = true;
        }
        public void ShowGraph(List<string> all_keys)
        {
            splitBigPlot.Visible = false;
            labelTopL.Visible = true;
            labelTopL1.Visible = true;
            labelTopR.Visible = true;

            PlotType = PlotTypeEnum.Lines;
            AllKeys = all_keys;

            if (parent.MainPlotKey != "" && (parent.RefPlotKey == "" || parent.MainPlotKey == parent.RefPlotKey))
                parent.PlotDataRec(Graph, parent.Data[parent.MainPlotKey]);
            else
            {
                Form1.DataStruct ds1 = parent.Data[parent.MainPlotKey];
                Form1.DataStruct ds2 = parent.Data[parent.RefPlotKey];

                labelTopL.Text = ds1.getAsInfoTextForGraph (parent.ProfileInfoList, parent.BeanList);
                labelTopL1.Text = ds2.getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList);

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


                var pi = ds2.getPreinfTime();
                List<double> x_pi = new List<double>(); x_pi.Add(pi); x_pi.Add(pi);
                List<double> y_pi = new List<double>(); y_pi.Add(0); y_pi.Add(1);
                Graph.SetData(8, x_pi, y_pi, Color.Brown, 2, DashStyle.Dash);

                pi = ds1.getPreinfTime();
                x_pi.Clear(); x_pi.Add(pi); x_pi.Add(pi);
                Graph.SetData(9, x_pi, y_pi, Color.Brown, 2, DashStyle.Solid);

                Graph.SetAutoLimits();

                Graph.panel.Refresh();
            }
        }

        List<string> SmartPlotSort(List<string> input)
        {
            List<Form1.DataStruct> list = new List<Form1.DataStruct>();
            foreach (var i in input)
                list.Add(parent.Data[i]);

            list.Sort(delegate (Form1.DataStruct a1, Form1.DataStruct a2)
            {
                bool a1star = a1.notes.StartsWith("*");
                bool a2star = a2.notes.StartsWith("*");

                if (a1star != a2star)  { return a1star.CompareTo(a2star); }
                else if (a1.profile != a2.profile) { return a2.profile.CompareTo(a1.profile); }
                else
                {
                     return a1.date.CompareTo(a2.date);
                }
            });

            List<string> output_list = new List<string>();
            foreach (var x in list)
                output_list.Add(x.date_str);

            return output_list;
        }
        List<string> IdSort(List<string> input)
        {
            List<Form1.DataStruct> list = new List<Form1.DataStruct>();
            foreach (var i in input)
                list.Add(parent.Data[i]);

            list.Sort(delegate (Form1.DataStruct a1, Form1.DataStruct a2)
            {
                return a1.id.CompareTo(a2.id);
            });

            List<string> output_list = new List<string>();
            foreach (var x in list)
                output_list.Add(x.date_str);

            return output_list;
        }

        public enum PlotTypeEnum { Lines, AvFlow, Kpi, Pi, Ratio, AllLines}
        public List<string> AllKeys = new List<string>();
        public PlotTypeEnum PlotType;
        string BestKey = "";

        public void ShowScatterGraph(List<string> all_keys, PlotTypeEnum plot_type = PlotTypeEnum.AvFlow)
        {
            splitBigPlot.Visible = false;
            labelTopL.Visible = true;
            labelTopL1.Visible = true;
            labelTopR.Visible = true;
            PlotType = plot_type;

            labelTopL1.Text = "";

            AllKeys = SmartPlotSort(all_keys);
            
            if (plot_type == PlotTypeEnum.AvFlow)
                Graph.SetAxisTitles("GRIND", "Av Flow");
            else if (plot_type == PlotTypeEnum.Kpi)
                Graph.SetAxisTitles("GRIND", "KPI");
            else if (plot_type == PlotTypeEnum.Pi)
                Graph.SetAxisTitles("GRIND", "PreINF");
            else if (plot_type == PlotTypeEnum.Ratio)
                Graph.SetAxisTitles("GRIND", "Ratio");

            Graph.data.Clear();
            foreach (var key in AllKeys)
            {
                Form1.DataStruct ds = parent.Data[key];

                double val = 0.0;
                if (plot_type == PlotTypeEnum.AvFlow)
                    val = ds.getAverageWeightFlow();
                else if (plot_type == PlotTypeEnum.Kpi)
                    val = ds.getKpi(parent.ProfileInfoList);
                else if (plot_type == PlotTypeEnum.Pi)
                    val = ds.getPreinfTime();
                else if (plot_type == PlotTypeEnum.Ratio)
                    val = ds.getRatio();

                if (ds.notes.StartsWith("*"))
                    Graph.SetDotsOrTriangles(int.MaxValue, Convert.ToDouble(ds.grind), val, Color.Red, 50, GraphPainter.SeriesTypeEnum.Triangles);
                else
                {
                    var color = Color.Gray;
                    if (parent.ProfileInfoList.ContainsKey(ds.profile))
                        color = parent.ProfileInfoList[ds.profile].color;

                    Graph.SetDotsOrTriangles(int.MaxValue, Convert.ToDouble(ds.grind), val, color, 10, GraphPainter.SeriesTypeEnum.Dots);
                }

            }
            Graph.SetAutoLimits();

            Graph.xmin -= 0.3;
            Graph.xmax += 0.3;
            Graph.ymin -= Graph.ymax * 0.1;
            Graph.ymax += Graph.ymax * 0.1;

            Graph.panel.Refresh();
        }

        public void ShowLineGraphAll(List<string> all_keys)
        {
            splitBigPlot.Visible = false;
            labelTopL.Visible = true;
            labelTopL1.Visible = true;
            labelTopR.Visible = true;
            PlotType = PlotTypeEnum.AllLines;

            AllKeys = IdSort(all_keys);

            labelTopL.Text = "";
            labelTopL1.Text = "";

            Graph.SetAxisTitles("", "");
            Graph.data.Clear();
            int counter = 0;
            foreach (var key in AllKeys)
            {
                Form1.DataStruct ds = parent.Data[key];

                Graph.SetData(counter, ds.elapsed, ds.flow, Color.Blue, 1, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.pressure, Color.LimeGreen, 1, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.flow_weight, Color.Brown, 1, DashStyle.Solid); counter++;

                List<double> temperature_scaled = new List<double>();
                foreach (var t in ds.temperature_basket)
                    temperature_scaled.Add(t / 10.0);
                Graph.SetData(counter, ds.elapsed, temperature_scaled, Color.Red, 1, DashStyle.Solid); counter++;
            }

            if(parent.Data.ContainsKey(parent.MainPlotKey))
            {
                Form1.DataStruct ds = parent.Data[parent.MainPlotKey];

                labelTopL.Text = ds.getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList);

                Graph.SetData(counter, ds.elapsed, ds.flow, Color.Blue, 3, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.pressure, Color.LimeGreen, 3, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid); counter++;

                List<double> temperature_scaled = new List<double>();
                foreach (var t in ds.temperature_basket)
                    temperature_scaled.Add(t / 10.0);
                Graph.SetData(counter, ds.elapsed, temperature_scaled, Color.Red, 3, DashStyle.Solid); counter++;
            }

            Graph.SetAutoLimits();
            Graph.panel.Refresh();
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
            else if (e.KeyValue == 112) // F1
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.AvFlow);
            }
            else if (e.KeyValue == 113) // F2
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.Kpi);
            }
            else if (e.KeyValue == 114) // F3
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.Pi);
            }
            else if (e.KeyValue == 115) // F4
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.Ratio);
            }
            else if (e.KeyValue == 123) // F12
            {
                ShowLineGraphAll(AllKeys);
            }
            else if (e.KeyValue == 38) // Up
            {
                if (PlotType == PlotTypeEnum.AllLines)
                {
                    int current_index = 0;
                    for (int i = 0; i < AllKeys.Count; i++)
                    {
                        if (AllKeys[i] == parent.MainPlotKey)
                        {
                            current_index = i;
                            break;
                        }
                    }

                    current_index++;
                    if (current_index >= AllKeys.Count)
                        current_index = 0;

                    parent.MainPlotKey = AllKeys[current_index];
                    parent.RefPlotKey = "";
                    parent.SetSelected();

                    ShowLineGraphAll(AllKeys);
                }
            }
            else if (e.KeyValue == 40) // Down
            {
                if (PlotType == PlotTypeEnum.AllLines)
                {
                    int current_index = 0;
                    for (int i = 0; i < AllKeys.Count; i++)
                    {
                        if (AllKeys[i] == parent.MainPlotKey)
                        {
                            current_index = i;
                            break;
                        }
                    }
                    current_index--;
                    if (current_index < 0)
                        current_index = AllKeys.Count - 1;

                    parent.MainPlotKey = AllKeys[current_index];
                    parent.RefPlotKey = "";
                    parent.SetSelected();

                    ShowLineGraphAll(AllKeys);
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = Graph.ToDataX(e.X);
            var y = Graph.ToDataY(e.Y);

            labelTopR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
            
            if (PlotType == PlotTypeEnum.Lines || PlotType == PlotTypeEnum.AllLines)
                return;

            // find the closest Data point to the mouse points. Search from the last painted
            double min_dist = double.MaxValue;
            BestKey = "";

            for (int i = AllKeys.Count-1; i >= 0; i--)
            {
                Form1.DataStruct ds = parent.Data[AllKeys[i]];

                double val = 0.0;
                if (PlotType == PlotTypeEnum.AvFlow)
                    val = ds.getAverageWeightFlow();
                else if (PlotType == PlotTypeEnum.Kpi)
                    val = ds.getKpi(parent.ProfileInfoList);
                else if (PlotType == PlotTypeEnum.Pi)
                    val = ds.getPreinfTime();
                else if (PlotType == PlotTypeEnum.Ratio)
                    val = ds.getRatio();

                double g = Convert.ToDouble(ds.grind);

                var graph_x = Graph.ToGraphX(g);
                var graph_y = Graph.ToGraphY(val);

                double dist = Math.Sqrt((e.X - graph_x) * (e.X - graph_x) + (e.Y - graph_y) * (e.Y - graph_y));
                if(dist < min_dist)
                {
                    min_dist = dist;
                    BestKey = AllKeys[i];
                }
            }

            if (min_dist <= 10)
                labelTopL.Text = parent.Data[BestKey].getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList);
            else
                labelTopL.Text = "";
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (PlotType == PlotTypeEnum.Lines || PlotType == PlotTypeEnum.AllLines)
                return;

            parent.MainPlotKey = BestKey;
            parent.RefPlotKey = "";
            parent.SetSelected();

            ShowGraph(AllKeys);
        }
    }
}
