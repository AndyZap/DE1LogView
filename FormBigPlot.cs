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
        Graphics graphics = null;

        public bool noTemperature = true;
        public bool noResistance = true;

        public FormBigPlot()
        {
            InitializeComponent();
            Graph = new GraphPainter(panel1, this.Font);
            Graph.border_y2 = 100;
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
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

                bool two_steam_plots = ds1.bean_name.ToLower() == "steam" && ds1.bean_name.ToLower() == "steam";

                labelTopL.Text = ds1.getAsInfoTextForGraph (parent.ProfileInfoList, parent.BeanList);
                labelTopL1.Text = ds2.getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList);

                Graph.SetAxisTitles("", "");
                Graph.data.Clear();

                Graph.SetData(0, ds2.elapsed, ds2.flow_smooth, Color.Blue, 3, DashStyle.Dash);
                Graph.SetData(1, ds2.elapsed, ds2.pressure_smooth, Color.LimeGreen, 3, DashStyle.Dash);
                Graph.SetData(2, ds2.elapsed, ds2.flow_weight, Color.Brown, 3, DashStyle.Dash);

                Graph.SetData(3, ds1.elapsed, ds1.flow_smooth, Color.Blue, 3, DashStyle.Solid);
                Graph.SetData(4, ds1.elapsed, ds1.pressure_smooth, Color.LimeGreen, 3, DashStyle.Solid);
                Graph.SetData(5, ds1.elapsed, ds1.flow_weight, Color.Brown, 3, DashStyle.Solid);

                List<double> temperature_scaled1 = new List<double>();
                List<double> temperature_scaled2 = new List<double>();
                foreach (var t in ds1.temperature_basket)
                    temperature_scaled1.Add(t / 10.0);
                foreach (var t in ds2.temperature_basket)
                    temperature_scaled2.Add(t / 10.0);


#if STEAM_STUDY
                if (two_steam_plots == false) // with STEAM_STUDY disable temperature plots
                {
#endif

                if (noTemperature == false)
                {
                    Graph.SetData(6, ds2.elapsed, temperature_scaled2, Color.Red, 3, DashStyle.Dash);
                    Graph.SetData(7, ds1.elapsed, temperature_scaled1, Color.Red, 3, DashStyle.Solid);
                }

#if STEAM_STUDY == false
                if (two_steam_plots == false) // otherwise enable temperature plots
                {
#endif
                    var pi = ds2.getPreinfTime();
                    List<double> x_pi = new List<double>(); x_pi.Add(pi); x_pi.Add(pi);
                    List<double> y_pi = new List<double>(); y_pi.Add(0); y_pi.Add(1);
                    Graph.SetData(8, x_pi, y_pi, Color.Brown, 2, DashStyle.Dash);

                    pi = ds1.getPreinfTime();
                    x_pi.Clear(); x_pi.Add(pi); x_pi.Add(pi);
                    Graph.SetData(9, x_pi, y_pi, Color.Brown, 2, DashStyle.Solid);
                }

                Graph.SetAutoLimits();


                if (noResistance == false)
                {
                    {
                        var ds_t = ds1;
                        List<double> res_t = new List<double>();
                        for (int i = 0; i < ds_t.elapsed.Count; i++)
                        {
                            var res = ds_t.flow_smooth[i] == 0.0 ? 100.0 : Math.Sqrt(ds_t.pressure_smooth[i]) / ds_t.flow_smooth[i]; // use as per AdAstra

                            if (ds_t.flow_goal[i] <= 0.1 && ds_t.pressure_goal[i] <= 0.1) // skip when no pressure/flow
                                res = 0.0;

                            res_t.Add(res);
                        }

                        Graph.SetData(10, ds_t.elapsed, res_t, Color.Fuchsia, 2, DashStyle.Solid);
                    }

                    {
                        var ds_t = ds2;
                        List<double> res_t = new List<double>();
                        for (int i = 0; i < ds_t.elapsed.Count; i++)
                        {
                            var res = ds_t.flow_smooth[i] == 0.0 ? 100.0 : Math.Sqrt(ds_t.pressure_smooth[i]) / ds_t.flow_smooth[i]; // use as per AdAstra

                            if (ds_t.flow_goal[i] <= 0.1 && ds_t.pressure_goal[i] <= 0.1) // skip when no pressure/flow
                                res = 0.0;

                            res_t.Add(res);
                        }

                        Graph.SetData(10, ds_t.elapsed, res_t, Color.Fuchsia, 2, DashStyle.Dash);
                    }
                }


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

        public enum PlotTypeEnum { Lines, MaxFlow, MaxPress, Pi, Ratio, EY, AllLines, TotalVolumeAll}
        public List<string> AllKeys = new List<string>();
        public PlotTypeEnum PlotType;
        string BestKey = "";

        public void ShowScatterGraph(List<string> all_keys, PlotTypeEnum plot_type = PlotTypeEnum.MaxFlow)
        {
            splitBigPlot.Visible = false;
            labelTopL.Visible = true;
            labelTopL1.Visible = true;
            labelTopR.Visible = true;
            PlotType = plot_type;

            labelTopL1.Text = "";

            AllKeys = SmartPlotSort(all_keys);
            
            if (plot_type == PlotTypeEnum.MaxFlow)
                Graph.SetAxisTitles("GRIND", "Max Flow");
            else if (plot_type == PlotTypeEnum.MaxPress)
                Graph.SetAxisTitles("GRIND", "Max Pressure");
            else if (plot_type == PlotTypeEnum.Pi)
                Graph.SetAxisTitles("GRIND", "PreINF");
            else if (plot_type == PlotTypeEnum.Ratio)
                Graph.SetAxisTitles("GRIND", "Ratio");
            else if (plot_type == PlotTypeEnum.EY)
                Graph.SetAxisTitles("GRIND", "EY");

            Graph.data.Clear();
            foreach (var key in AllKeys)
            {
                Form1.DataStruct ds = parent.Data[key];

                if (ds.bean_name.ToLower() == "steam")
                    continue;

                double val = 0.0;
                if (plot_type == PlotTypeEnum.MaxFlow)
                    val = ds.getMaxWeightFlow();
                else if (plot_type == PlotTypeEnum.MaxPress)
                    val = ds.getMaxPressure();
                else if (plot_type == PlotTypeEnum.Pi)
                    val = ds.getPreinfTime();
                else if (plot_type == PlotTypeEnum.Ratio)
                    val = ds.getRatio();
                else if (plot_type == PlotTypeEnum.EY)
                {
                    var ey_str = ds.getEY();
                    if (ey_str == "")
                        continue;

                    val = Convert.ToDouble(ey_str);
                }

                double grind = 0.0;
                if (ds.grind.EndsWith("-"))
                    grind = Convert.ToDouble(ds.grind.Replace("-", "")) - 0.13;
                else if (ds.grind.EndsWith("+"))
                    grind = Convert.ToDouble(ds.grind.Replace("+", "")) + 0.13;
                else
                    grind = Convert.ToDouble(ds.grind);

                if (ds.notes.StartsWith("*"))
                    Graph.SetDotsOrTriangles(int.MaxValue, grind, val, Color.Red, 50, GraphPainter.SeriesTypeEnum.Triangles);
                else
                {
                    var color = Color.Gray;
                    if (parent.ProfileInfoList.ContainsKey(ds.profile))
                        color = parent.ProfileInfoList[ds.profile].color;
                    else if (ds.profile.StartsWith("_SRT")) // fix for SRT profiles
                        color = Color.Black;

                    Graph.SetDotsOrTriangles(int.MaxValue, grind, val, color, 10, GraphPainter.SeriesTypeEnum.Dots);
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

                if (ds.bean_name.ToLower() == "steam")
                    continue;

                Graph.SetData(counter, ds.elapsed, ds.flow_smooth, Color.Blue, 1, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.pressure_smooth, Color.LimeGreen, 1, DashStyle.Solid); counter++;
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

                Graph.SetData(counter, ds.elapsed, ds.flow_smooth, Color.Blue, 3, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.pressure_smooth, Color.LimeGreen, 3, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid); counter++;

                List<double> temperature_scaled = new List<double>();
                foreach (var t in ds.temperature_basket)
                    temperature_scaled.Add(t / 10.0);
                Graph.SetData(counter, ds.elapsed, temperature_scaled, Color.Red, 3, DashStyle.Solid); counter++;
            }

            Graph.SetAutoLimits();
            Graph.panel.Refresh();
        }

        public void ShowTotalVolumeGraphAll(List<string> all_keys)
        {
            splitBigPlot.Visible = false;
            labelTopL.Visible = true;
            labelTopL1.Visible = true;
            labelTopR.Visible = true;
            PlotType = PlotTypeEnum.TotalVolumeAll;

            AllKeys = IdSort(all_keys);

            labelTopL.Text = "";
            labelTopL1.Text = "";

            Graph.SetAxisTitles("", "");
            Graph.data.Clear();
            int counter = 0;
            foreach (var key in AllKeys)
            {
                Form1.DataStruct ds = parent.Data[key];

#if STEAM_STUDY
                TimeSpan ts = DateTime.Now - ds.date;
                  if (ts.TotalDays > 1)
                    continue;
#endif

                Graph.SetData(counter, ds.elapsed, ds.getTotalWaterVolume(), Color.Blue, 1, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.weight, Color.Brown, 1, DashStyle.Solid); counter++;
            }

            if (parent.Data.ContainsKey(parent.MainPlotKey))
            {
                Form1.DataStruct ds = parent.Data[parent.MainPlotKey];

                labelTopL.Text = ds.getAsInfoTextForGraph(parent.ProfileInfoList, parent.BeanList);

                Graph.SetData(counter, ds.elapsed, ds.getTotalWaterVolume(), Color.Blue, 3, DashStyle.Solid); counter++;
                Graph.SetData(counter, ds.elapsed, ds.weight, Color.Brown, 3, DashStyle.Solid); counter++;
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
            graphics = e.Graphics;

            if (Graph != null)
                Graph.Plot(graphics);
        }

        private void FormBigPlot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27) // Esc
            {
                Hide();
            }
            else if (e.KeyValue == 112) // F1
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.MaxFlow);
            }
            else if (e.KeyValue == 113) // F2
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.MaxPress);
            }
            else if (e.KeyValue == 114) // F3
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.Pi);
            }
            else if (e.KeyValue == 115) // F4
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.Ratio);
            }
            else if (e.KeyValue == 116) // F5
            {
                ShowScatterGraph(AllKeys, PlotTypeEnum.EY);
            }
            else if (e.KeyValue == 122) // F11
            {
                ShowTotalVolumeGraphAll(AllKeys);
            }
            else if (e.KeyValue == 123) // F12
            {
                ShowLineGraphAll(AllKeys);
            }
            else if (e.KeyValue == 38) // Up
            {
                if (PlotType == PlotTypeEnum.AllLines || PlotType == PlotTypeEnum.TotalVolumeAll)
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

                    if (PlotType == PlotTypeEnum.AllLines)
                        ShowLineGraphAll(AllKeys);
                    else if (PlotType == PlotTypeEnum.TotalVolumeAll)
                        ShowTotalVolumeGraphAll(AllKeys);
                }
            }
            else if (e.KeyValue == 40) // Down
            {
                if (PlotType == PlotTypeEnum.AllLines || PlotType == PlotTypeEnum.TotalVolumeAll)
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

                    if (PlotType == PlotTypeEnum.AllLines)
                        ShowLineGraphAll(AllKeys);
                    else if (PlotType == PlotTypeEnum.TotalVolumeAll)
                        ShowTotalVolumeGraphAll(AllKeys);
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = Graph.ToDataX(e.X);
            var y = Graph.ToDataY(e.Y);

            labelTopR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
            
            if (PlotType == PlotTypeEnum.Lines || PlotType == PlotTypeEnum.AllLines || PlotType == PlotTypeEnum.TotalVolumeAll)
                return;

            // find the closest Data point to the mouse points. Search from the last painted
            double min_dist = double.MaxValue;
            BestKey = "";

            for (int i = AllKeys.Count-1; i >= 0; i--)
            {
                Form1.DataStruct ds = parent.Data[AllKeys[i]];

                if (ds.bean_name.ToLower() == "steam")
                    continue;

                double val = 0.0;
                if (PlotType == PlotTypeEnum.MaxFlow)
                    val = ds.getMaxWeightFlow();
                else if (PlotType == PlotTypeEnum.MaxPress)
                    val = ds.getMaxPressure();
                else if (PlotType == PlotTypeEnum.Pi)
                    val = ds.getPreinfTime();
                else if (PlotType == PlotTypeEnum.Ratio)
                    val = ds.getRatio();
                else if (PlotType == PlotTypeEnum.EY)
                {
                    var ey_str = ds.getEY();
                    if (ey_str == "")
                        continue;

                    val = Convert.ToDouble(ey_str);
                }

                double g = 0.0;
                if (ds.grind.EndsWith("-"))
                    g = Convert.ToDouble(ds.grind.Replace("-", "")) - 0.13;
                else if (ds.grind.EndsWith("+"))
                    g = Convert.ToDouble(ds.grind.Replace("+", "")) + 0.13;
                else
                    g = Convert.ToDouble(ds.grind);

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
            if (PlotType == PlotTypeEnum.Lines || PlotType == PlotTypeEnum.AllLines  || PlotType == PlotTypeEnum.TotalVolumeAll)
                return;

            parent.MainPlotKey = BestKey;
            parent.RefPlotKey = "";
            parent.SetSelected();

            ShowGraph(AllKeys);
        }

        private void FormBigPlot_Resize(object sender, EventArgs e)
        {
            panel1.Refresh();
        }
    }
}
