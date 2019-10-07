using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CoffeeLogger
{
    public partial class Form1 : Form
    {
        string Revision = "$Revision: 2.1 $";
        string ApplicationDirectory = "";
        string ApplicationNameNoExt = "";
        string AcaiaLoggerFile = "";

        // to draw GBR bars in listData_DrawItem
        readonly int _BP1 = 20;
        readonly int _BP2 = 30;
        readonly double _REF_BEAN = 18.5;

        // these are used to color-code values, in listData_DrawItem only
        readonly double _MIN_P = 3.1;
        readonly double _RANGE_P = 7.1;
        readonly double _MIN_R = 1.5;
        readonly double _RANGE_R = 1.2;

        GraphPainter GraphWeight = null;
        GraphPainter GraphPressure = null;

        List<HeatmapEntry> HeatMapP = new List<HeatmapEntry>();
        List<HeatmapEntry> HeatMapR = new List<HeatmapEntry>();

        private bool LoadLineContainsKey(string line, string key)
        {
            return line.StartsWith(key);
        }
        private string LoadString(string line, string key)
        {
            if (!line.StartsWith(key)) { return ""; }
            return line.Remove(0, key.Length).Trim();
        }
        private int LoadInt(string line, string key)
        {
            if (!line.StartsWith(key)) { return 0; }
            string str = line.Remove(0, key.Length).Trim();

            int result = 0;
            try
            {
                result = Convert.ToInt32(str);
            }
            catch (Exception) { }
            return result;
        }
        private void LoadSettings()
        {
            string fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".dat";
            if (File.Exists(fname))
            {
                string[] lines = File.ReadAllLines(fname);
                foreach (string s in lines)
                {
                    if (LoadLineContainsKey(s, "this.Top")) { this.Top = LoadInt(s, "this.Top"); }
                    else if (LoadLineContainsKey(s, "this.Left")) { this.Left = LoadInt(s, "this.Left"); }
                    else if (LoadLineContainsKey(s, "this.Height")) { this.Height = LoadInt(s, "this.Height"); }
                    else if (LoadLineContainsKey(s, "this.Width")) { this.Width = LoadInt(s, "this.Width"); }
                    else if (LoadLineContainsKey(s, "this.WindowState")) { this.WindowState = (FormWindowState)LoadInt(s, "this.WindowState"); }

                    else if (LoadLineContainsKey(s, "InitialDirectory")) { openFileDialog1.InitialDirectory = LoadString(s, "InitialDirectory"); }
                    else if (LoadLineContainsKey(s, "txtFltImportName")) { txtFltImportName.Text = LoadString(s, "txtFltImportName"); }
                    else if (LoadLineContainsKey(s, "txtFltImportGrind")) { txtFltImportGrind.Text = LoadString(s, "txtFltImportGrind"); }

                    else if (LoadLineContainsKey(s, "splitContainer1")) { splitContainer1.SplitterDistance = LoadInt(s, "splitContainer1"); }
                    else if (LoadLineContainsKey(s, "splitContainer2")) { splitContainer2.SplitterDistance = LoadInt(s, "splitContainer2"); }

                    else if (LoadLineContainsKey(s, "AcaiaLoggerFile")) { AcaiaLoggerFile = LoadString(s, "AcaiaLoggerFile"); }

                    else if (LoadLineContainsKey(s, "chkNoPreinf")) { chkNoPreinf.Checked = LoadString(s, "chkNoPreinf") == "TRUE"; }
                }
            }

        }
        private void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("this.Top                 " + (this.Top < 0 ? "0" : this.Top.ToString()));
            sb.AppendLine("this.Left                " + (this.Left < 0 ? "0" : this.Left.ToString()));
            sb.AppendLine("this.Height              " + (this.Height < 200 ? "200" : this.Height.ToString()));
            sb.AppendLine("this.Width               " + (this.Width < 200 ? "200" : this.Width.ToString()));
            sb.AppendLine("this.WindowState         " + ((int)this.WindowState).ToString());

            sb.AppendLine("InitialDirectory         " + openFileDialog1.InitialDirectory);

            sb.AppendLine("txtFltImportName         " + txtFltImportName.Text);
            sb.AppendLine("txtFltImportGrind        " + txtFltImportGrind.Text);
            sb.AppendLine("splitContainer1          " + splitContainer1.SplitterDistance.ToString());
            sb.AppendLine("splitContainer2          " + splitContainer2.SplitterDistance.ToString());
            sb.AppendLine("AcaiaLoggerFile          " + AcaiaLoggerFile);

            sb.AppendLine("chkNoPreinf              " + (chkNoPreinf.Checked ? "TRUE" : "FALSE"));

            string fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".dat";
            File.WriteAllText(fname, sb.ToString());
        }

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
            this.Text = "Coffee Logger  v" + Revision.Replace("Revision:", "").Replace("$", "").Trim() +
                "   Brew points = " + _BP1.ToString() + "/" + _BP2.ToString() + "  Ref Bean weight = " + _REF_BEAN.ToString();

            GraphWeight = new GraphPainter(splitContainer2.Panel1, this.Font);
            GraphWeight.SetAxisTitles("", "Weight/Flow, g");

            GraphPressure = new GraphPainter(splitContainer2.Panel2, this.Font);
            GraphPressure.SetAxisTitles("Time, sec", "Pressure, bar");

            ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            ApplicationNameNoExt = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            LoadSettings();

            string data_fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".csv";
            if (File.Exists(data_fname))
                ReadDataFromFile(data_fname, true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        enum BrewType { Espro, EsproBloom, Am, Flt }

        class DataStruct
        {
            readonly double _MAX_ESPRO_WEIGHT = 60;
            readonly double _MAX_AM_WEIGHT = 150;
            readonly double _GOOD_FLOW_VALUE = 1;
            readonly double _BLOOM_TIME_INDEX = 30;
            readonly double _GOOD_FLOW_SMALL_VALUE = 0.1;

            public int id = 0;
            public string name = "";
            public string date_str = "";
            public DateTime date = DateTime.MinValue;
            public double bean_weight = 0;
            public double coffee_weight = 0;
            public string grind = "";
            public double time = 0;
            public string notes = "";

            private List<double> data = new List<double>();

            public List<double> data_increasing = new List<double>();
            public List<double> flow = new List<double>();
            public List<double> pressure = new List<double>();

            // data without preinf

            private int good_flow_index = 0;

            public List<double> data_increasing_nopi = new List<double>();
            public List<double> flow_nopi = new List<double>();
            public List<double> pressure_nopi = new List<double>();

            public int saved_plot_index = -1;  // -1 means this has not been saved in the data struct

            public DataStruct(string s, string flt_name, string flt_grind, FileVersion version) // init from the csv file line
            {
                var words = s.Replace("\"", "").Split(',');

                if (version == FileVersion.Brewmaster)
                {
                    name = words[1].ToLower();
                    date_str = words[2];
                    bean_weight = Convert.ToDouble(words[3]);
                    grind = words[4];
                    notes = words[6].Trim();
                    time = Convert.ToDouble(words[7]);
                    coffee_weight = Convert.ToDouble(words[9]);

                    var vector_data = words[10].Split(';');
                    int num = 0;
                    double sum = 0;
                    foreach (var v in vector_data)
                    {
                        if (String.IsNullOrEmpty(v.Trim()))
                            continue;

                        sum += Convert.ToDouble(v);
                        num++;

                        if (num == 5) // 5 times per set sampling rate
                        {
                            data.Add(sum / (double)num);
                            num = 0;
                            sum = 0;
                        }
                    }

                    UpdateBrewTimeBrewM();
                }
                else if (version == FileVersion.AcaiaLogger)
                {
                    date_str = words[0];
                    name = words[1].ToLower();
                    bean_weight = Convert.ToDouble(words[2]);
                    coffee_weight = Convert.ToDouble(words[3]);
                    grind = words[4];
                    time = Convert.ToDouble(words[5]);
                    notes = words[6].Trim();

                    var vector_data = words[7].Split(';');
                    foreach (var v in vector_data)
                    {
                        if (String.IsNullOrEmpty(v.Trim()))
                            continue;

                        data.Add(Convert.ToDouble(v));
                    }

                    if (words.Length > 8)
                    {
                        vector_data = words[8].Split(';');
                        foreach (var v in vector_data)
                        {
                            if (String.IsNullOrEmpty(v.Trim()))
                                continue;

                            pressure.Add(Convert.ToDouble(v));
                        }
                    }
                }

                // fix for the filter
                if (coffee_weight > _MAX_AM_WEIGHT)
                {
                    if (flt_name != "")
                        name = flt_name;
                    if (flt_grind != "")
                        grind = flt_grind;
                }

                SetupIncreasingAndFlowArrays();

                SetupNoPreinfArrays();

                date = DateTime.Parse(date_str);
                date_str = date.ToString("yyyy MM dd ddd HH:mm");
            }

            public override string ToString()
            {
                // Brewmaster format
                /*
                string str = ",";                       // 0
                str += name + ",";                      // 1
                str += date_str + ",";                  // 2
                str += bean_weight.ToString() + ",";    // 3
                str += grind + ",";                     // 4
                str += ",";                             // 5
                str += notes + ",";                     // 6
                str += time.ToString() + ",";           // 7
                str += ",";                             // 8
                str += coffee_weight.ToString() + ",";  // 9
                */

                // Acaia logger format
                string str = date_str + ",";                // 0
                str += name + ",";                          // 1
                str += bean_weight.ToString("0.0") + ",";   // 2
                str += coffee_weight.ToString("0.0") + ","; // 3
                str += grind + ",";                         // 4
                str += time.ToString() + ",";               // 5
                str += notes + ",";                         // 6

                foreach (var d in data)
                    str += d.ToString("0.0") + ";";         // 7
                str += ",";                                 // closing comma

                foreach (var d in pressure)
                    str += d.ToString("0.0") + ";";         // 7
                str += ",";                                 // closing comma

                return str;
            }

            public void UpdateBrewTimeBrewM()
            {
                var last_different = data.Count - 1;
                while (last_different > 1)
                {
                    if (data[last_different] == data[last_different - 1])
                        last_different--;
                    else
                        break;
                }

                time = last_different;
            }

            public void SetupIncreasingAndFlowArrays()
            {
                List<double> tmp = new List<double>();
                foreach (var d in data)
                {
                    data_increasing.Add(d);
                    flow.Add(0.0);
                    tmp.Add(0.0);
                    if (pressure.Count < data.Count)
                        pressure.Add(0.0);
                }

                if (data.Count < 3)  // no processing, simply copy the data
                    return;

                // chop the possible bump in the weight graphs at the beginning - e.g. after tare
                if (data.Count > 5)
                {
                    for (int i = 5; i >= 1; i--)
                    {
                        if (data[i] < data[i - 1])
                            data[i - 1] = data[i];
                    }
                }

                // chop the bump in the pressure graphs at the beginning
                if (pressure.Count > 5)
                {
                    for (int i = 5; i >= 1; i--)
                    {
                        if (pressure[i] < pressure[i - 1])
                            pressure[i - 1] = pressure[i];
                    }
                }

                var value = data_increasing[0];
                for (int i = 0; i < data_increasing.Count; i++)
                {
                    if (data_increasing[i] < value)
                        data_increasing[i] = value;


                    value = data_increasing[i];
                }


                for (int i = 1; i < data_increasing.Count; i++)
                    tmp[i] = data_increasing[i] - data_increasing[i - 1];

                for (int i = 1; i < data_increasing.Count - 1; i++)  // average over 3 sec
                    flow[i] = (tmp[i] + tmp[i - 1] + tmp[i + 1]) / 3.0;

                flow[data_increasing.Count - 1] = flow[data_increasing.Count - 2];


            }

            public BrewType getBrewType()
            {
                if (coffee_weight < _MAX_ESPRO_WEIGHT)
                {
                    if (good_flow_index > _BLOOM_TIME_INDEX)
                        return BrewType.EsproBloom;
                    else
                        return BrewType.Espro;
                }
                else if (coffee_weight < _MAX_AM_WEIGHT)
                    return BrewType.Am;
                else
                    return BrewType.Flt;
            }

            public void SetupNoPreinfArrays()
            {
                // copy data
                foreach (var d in data_increasing)
                    data_increasing_nopi.Add(d);

                foreach (var d in pressure)
                    pressure_nopi.Add(d);

                foreach (var d in flow)
                    flow_nopi.Add(d);

                // find first point when the flow is about 1 g/s
                for (int i = 0; i < flow.Count; i++)
                {
                    if (flow[i] > _GOOD_FLOW_VALUE)
                    {
                        good_flow_index = i;
                        break;
                    }
                }

                // now find the starting index to chop the pre-infusion/bloom part
                int tokeep_index = 0;
                for (int i = good_flow_index; i >= 0; i--)
                {
                    if (good_flow_index > _BLOOM_TIME_INDEX)
                    {
                        if (flow[i] < _GOOD_FLOW_VALUE / 2) // Bloom algo - get the half-point
                        {
                            tokeep_index = i;
                            break;
                        }
                    }
                    else
                    {
                        if (flow[i] < _GOOD_FLOW_SMALL_VALUE) // all the rest algo - check against small value
                        {
                            tokeep_index = i;
                            break;
                        }
                    }
                }
                if (good_flow_index > _BLOOM_TIME_INDEX)  // extra adjustment for Bloom algo
                    tokeep_index -= good_flow_index - tokeep_index;

                data_increasing_nopi.RemoveRange(0, tokeep_index);
                pressure_nopi.RemoveRange(0, tokeep_index);
                flow_nopi.RemoveRange(0, tokeep_index);
            }

            public string GetPressureString()
            {
                if (pressure.Count < 2)
                    return "--";

                // find the middle
                List<double> sorted_pressure = new List<double>();
                foreach (var p in pressure)
                    sorted_pressure.Add(p);

                sorted_pressure.Sort();
                double median = sorted_pressure[sorted_pressure.Count / 2];

                if (median == 0)
                    return "--";

                // option 1 - use above median

                // option 2 - use above 1 bar
                median = 1.0;

                double max = double.MinValue;
                double sum = 0.0;
                int num = 0;

                for (int i = 0; i < pressure.Count - 3; i++)
                {
                    var p = pressure[i];

                    max = Math.Max(max, p);

                    if (p < median)
                        continue;

                    sum += p;
                    num++;
                }

                double value = sum / (double)num;

                return value.ToString("0.0") + "/" + max.ToString("0.0");
            }

            public double GetMaxPressure()
            {
                if (pressure.Count < 2)
                    return double.MinValue;

                double max = double.MinValue;

                foreach (var p in pressure)
                    max = Math.Max(max, p);

                if (max == 0.0)
                    return double.MinValue;

                return max;
            }

            public List<double> getWeightPoints(List<int> points)
            {
                if (points.Count != 2)
                    throw new Exception("WeightPoints not set correctly");

                List<double> output = new List<double>(points.Count + 1);

                // make an array which extends after the last point
                List<double> values = new List<double>();
                foreach (var d in data_increasing)
                    values.Add(d);

                while (values.Count < points[1] + 10)
                    values.Add(coffee_weight);


                // here are the 3 bars:
                output.Add(values[points[0]] / coffee_weight);

                output.Add(values[points[1]] / coffee_weight - output[0]);

                output.Add(1.0 - (output[1] + output[0]));

                return output;
            }
        }

        Dictionary<string, DataStruct> Data = new Dictionary<string, DataStruct>();

        List<int> WeightPoints = new List<int>();

        private void btnImportDataBrewM_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            var fname = openFileDialog1.FileName;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(fname);

            ImportData(fname);
        }

        private void btnImportDataAcaiaLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(AcaiaLoggerFile))
                ImportData(AcaiaLoggerFile);
            else
                MessageBox.Show("ERROR: AcaiaLoggerFile location is not set");

        }
        private void ImportData(string fname)
        {
            int records_added = ReadDataFromFile(fname, false);

            MessageBox.Show(records_added.ToString() + " records added", this.Text);
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

                BrewType bt = Data[key].getBrewType();
                if (radioEspro.Checked && bt != BrewType.Espro)
                    continue;

                if (radioAm.Checked && bt != BrewType.Am)
                    continue;

                if (radioFlt.Checked && bt != BrewType.Flt)
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

        enum FileVersion { Brewmaster, AcaiaLogger }

        private int ReadDataFromFile(string fname, bool from_database)
        {
            int records_added = 0;

            FileVersion version = FileVersion.Brewmaster;

            var lines = File.ReadAllLines(fname);
            foreach (var s in lines)
            {
                var line = s.Trim();
                if (String.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("id,name,createdAt"))
                {
                    version = FileVersion.Brewmaster;
                    continue;
                }
                else if (line.StartsWith("date,beanName,beanWeight,coffeeWeight"))
                {
                    version = FileVersion.AcaiaLogger;
                    continue;
                }

                DataStruct d = new DataStruct(line, from_database ? "" : txtFltImportName.Text,
                                                    from_database ? "" : txtFltImportGrind.Text,
                                              version);

                if (!Data.ContainsKey(d.date_str))
                {
                    d.id = Data.Count;
                    Data.Add(d.date_str, d);
                    records_added++;
                }
            }

            FilterData();

            return records_added;
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            List<string> sorted_keys = new List<string>();
            foreach (var key in Data.Keys)
                sorted_keys.Add(key);

            sorted_keys.Sort();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("date,beanName,beanWeight,coffeeWeight,grind,time,notes,weightEverySec,pressureEverySec");  // Acaia Logger format

            foreach (var key in sorted_keys)
                sb.AppendLine(Data[key].ToString());

            string data_fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".csv";
            File.WriteAllText(data_fname, sb.ToString());

            data_fname = "D:\\" + ApplicationNameNoExt + ".csv";
            File.WriteAllText(data_fname, sb.ToString());
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

            // plot weight
            GraphWeight.SetAxisTitles("", radioWeight.Checked ? "Weight, g" : "Flow, g/s");

            GraphWeight.data.Clear();

            List<double> y = radioWeight.Checked ? Data[key].data_increasing : Data[key].flow;
            List<double> x = new List<double>();
            for (int i = 0; i < y.Count; i++)
                x.Add(i);

            GraphWeight.SetData(0, x, y, Color.Blue, 4, GraphPainter.Style.Solid);

            for (int si = 0; si < SelectedPlots.Count; si++)
            {
                var skey = SelectedPlots[si];

                List<double> sy = radioWeight.Checked ?
                                    (chkNoPreinf.Checked ? Data[skey].data_increasing_nopi : Data[skey].data_increasing)
                                  : (chkNoPreinf.Checked ? Data[skey].flow_nopi : Data[skey].flow);
                List<double> sx = new List<double>();
                for (int i = 0; i < sy.Count; i++)
                    sx.Add(i);

                GraphWeight.SetData(si + 1, sx, sy, GetPenColor(si), 4, GraphPainter.Style.Solid);
            }

            GraphWeight.SetAutoLimits();
            splitContainer2.Panel1.Refresh();

            // plot pressure
            GraphPressure.data.Clear();

            y = Data[key].pressure;
            x.Clear();
            for (int i = 0; i < y.Count; i++)
                x.Add(i);

            GraphPressure.SetData(0, x, y, Color.Blue, 4, GraphPainter.Style.Solid);

            for (int si = 0; si < SelectedPlots.Count; si++)
            {
                var skey = SelectedPlots[si];

                List<double> sy = chkNoPreinf.Checked ? Data[skey].pressure_nopi : Data[skey].pressure;
                List<double> sx = new List<double>();
                for (int i = 0; i < sy.Count; i++)
                    sx.Add(i);

                GraphPressure.SetData(si + 1, sx, sy, GetPenColor(si), 4, GraphPainter.Style.Solid);
            }

            GraphPressure.SetAutoLimits();

            splitContainer2.Panel2.Refresh();
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
            e.Graphics.DrawString(d.getBrewType().ToString().Substring(0, 1), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

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


            myrec.X = labPressure.Left; myrec.Width = labPressure.Width;
            var max_p = d.GetMaxPressure();
            if (max_p != double.MinValue)
            {
                var c = getHeatmapColor((max_p - _MIN_P) / _RANGE_P, HeatMapP);
                e.Graphics.FillRectangle(c, myrec);
            }
            e.Graphics.DrawString(d.GetPressureString(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);


            myrec.Y -= 5; // adjust back

            // plot color.  0th plot is the current one, plotted as Blue
            myrec.X = labHasPlot.Left + 2; myrec.Width = labHasPlot.Width - 5;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.Blue, myrec);
            else
                e.Graphics.FillRectangle(GetBrushColor(d.saved_plot_index), myrec);

            // plot weight points - LAST! - as we change myrec size
            if (d.getBrewType() != BrewType.Flt)
            {

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
            if (GraphWeight != null)
                GraphWeight.Plot(e.Graphics);
        }
        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (GraphPressure != null)
                GraphPressure.Plot(e.Graphics);
        }

        private void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void radioEspro_CheckedChanged(object sender, EventArgs e)
        {
            FilterData();
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

            string t = d.getBrewType().ToString().Substring(0, 1);
            sb.Append(t + ": ");
            sb.Append(d.bean_weight.ToString() + " -> ");
            sb.Append(d.coffee_weight.ToString("0.0") + " in ");
            sb.Append(d.time.ToString() + " sec, ratio ");
            sb.Append((d.coffee_weight / d.bean_weight).ToString("0.00") + " grind ");
            sb.Append(d.grind + " press ");
            sb.Append(d.GetPressureString());

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

        // Heatmap
        class HeatmapEntry
        {
            public double x;
            public Color col;

            public HeatmapEntry(double _x, Color _col) { x = _x; col = _col; }
        }

        List<HeatmapEntry> getHeatmap(int _HEATMAP)
        {
            List<HeatmapEntry> heatmap = new List<HeatmapEntry>();
            if (_HEATMAP == 0)
            {
                // no-op: Polar mapping of R and G
            }
            else if (_HEATMAP == 1)
            {
                // 5 color visible spectrum
                heatmap.Add(new HeatmapEntry(0.00, Color.FromArgb(255, 0, 255)));
                heatmap.Add(new HeatmapEntry(0.25, Color.FromArgb(0, 0, 255)));
                heatmap.Add(new HeatmapEntry(0.50, Color.FromArgb(0, 255, 0)));
                heatmap.Add(new HeatmapEntry(0.75, Color.FromArgb(255, 255, 0)));
                heatmap.Add(new HeatmapEntry(1.00, Color.FromArgb(255, 0, 0)));

            }
            else if (_HEATMAP == 2)
            {
                // 3 color EGB with softer R
                heatmap.Add(new HeatmapEntry(0.00, Color.FromArgb(0, 255, 0)));
                heatmap.Add(new HeatmapEntry(0.50, Color.FromArgb(255, 255, 0)));
                heatmap.Add(new HeatmapEntry(1.00, Color.FromArgb(230, 50, 0)));
            }

            return heatmap;
        }

        Brush getHeatmapColor(double value, List<HeatmapEntry> map) // value must be between 0 and 1
        {
            if (map.Count == 0) // polar mapping which does not need map
            {
                if (value < 0 || value > 1)
                    return new SolidBrush(Color.FromArgb(255, 255, 255));

                var r = 1 / Math.Sqrt(2.0);
                var theta = Math.Acos((r - 0.99999 * value * Math.Sqrt(2.0)) / r) - Math.PI / 4.0;

                double x = 0.5 + r * Math.Cos(theta);
                double y = 0.5 + r * Math.Sin(theta);

                int Gr = Math.Max(0, Math.Min(255, (int)Math.Round(255 * x)));
                int Rd = Math.Max(0, Math.Min(255, (int)Math.Round(255 * y)));

                return new SolidBrush(Color.FromArgb(Rd, Gr, 0));
            }

            // mapping with preset color points

            Color c1 = map[0].col;
            Color c2 = map[map.Count - 1].col;
            double fraction = 0.0;

            if (value <= 0)
            {
                c1 = map[0].col;
                c2 = map[0].col;
            }
            else if (value >= 1)
            {
                c1 = map[map.Count - 1].col;
                c2 = map[map.Count - 1].col;
            }
            else
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var x1 = map[i].x;
                    var x2 = map[i + 1].x;
                    if (value >= x1 && value < x2)
                    {
                        c1 = map[i].col;
                        c2 = map[i + 1].col;
                        fraction = value - x1;
                        break;
                    }
                }
            }

            int R = Math.Min(255, c1.R + (int)Math.Round((c2.R - c1.R) * fraction));
            int G = Math.Min(255, c1.G + (int)Math.Round((c2.G - c1.G) * fraction));
            int B = Math.Min(255, c1.B + (int)Math.Round((c2.B - c1.B) * fraction));

            return new SolidBrush(Color.FromArgb(R, G, B));
        }

        private void LabName_Click(object sender, EventArgs e)
        {
            string fname = @"D:\platform-tools\__data\7_de1_1\ws_output4.txt";
            var lines = File.ReadAllLines(fname);

            string frame = "";
            string time = "";
            string bits = "";
            string opcode = "";
            string charact = "";

            bool read_header = true;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Frame" + "\t" + "time" + "\t" + "opcode_ch" + "\t" + "bits");

            int counter = 0;
            foreach (var s in lines)
            {
                counter++;
                var line = s.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (   line.StartsWith("Frame")
                    || line.StartsWith("Bluetooth")
                    || line.StartsWith("Length: ")
                    || line.StartsWith("Attribute")
                    || line.StartsWith("[Handle:")
                    || line.StartsWith("[Service")
                    || line.StartsWith("[Request")
                    || line.StartsWith("[Response")
                    || line.StartsWith("[UUID: Client Characteristic")
                    || line.StartsWith("[UUID: GATT")
                    || line.StartsWith("[UUID: Generic")
                    || line.StartsWith("[UUID: Peripheral")
                    || line.StartsWith("[UUID: Service")
                    || line.StartsWith("Starting")
                    || line.StartsWith("Ending")
                    || line.StartsWith("UUID")
                    || line.StartsWith("Request")
                    || line.StartsWith("Handle")
                    || line.StartsWith("Error")
                    || line.StartsWith("Information")
                    || line.StartsWith("Characteristic")
                    
                    )
                    continue;

                if (line.StartsWith("0000") || line.StartsWith("0010"))
                {
                    if (opcode != "")
                    {
                        sb.AppendLine(frame + "\t" + time + "\t" + opcode + charact + "\t" + bits);
                        opcode = "";
                        bits = "";
                        charact = "";
                    }

                    read_header = true;
                }
                else if(read_header)
                {
                    var words = line.Trim().Split(' ');
                    frame = words[0].PadLeft(10);
                    time = Convert.ToDouble(words[1]).ToString("0.00").PadLeft(12);
                    read_header = false;
                }
                else if (line.StartsWith("Opcode:"))
                {
                    if      (line == "Opcode: Handle Value Notification (0x1b)")       opcode = " N_";

                    else if (line.StartsWith("Opcode: Read By Group Type Request"))    opcode = "";
                    else if (line.StartsWith("Opcode: Read By Group Type Response"))   opcode = "";
                    else if (line.StartsWith("Opcode: Read By Type Request"))          opcode = "";
                    else if (line.StartsWith("Opcode: Read By Type Response"))         opcode = "";
                    else if (line.StartsWith("Opcode: Error Response"))                opcode = "";
                    else if (line.StartsWith("Opcode: Find Information Request"))      opcode = "";
                    else if (line.StartsWith("Opcode: Find Information Response"))     opcode = "";
                    else if (line.StartsWith("Opcode: Write Request (0x12)"))          opcode = " W_";
                    else if (line.StartsWith("Opcode: Write Response (0x13)"))         opcode = "";
                    else if (line.StartsWith("Opcode: Read Request (0x0a)"))           opcode = "";
                    else if (line.StartsWith("Opcode: Read Response (0x0b)"))          opcode = " R_";


                    else { MessageBox.Show("Do not know line: " + line + " " + counter.ToString()); break; }
                }
                else if (line.StartsWith("Value:"))
                {
                    bits = line.Replace("Value:", "").Trim();
                }
                else if (line.StartsWith("[Characteristic UUID: Unknown"))
                {
                    charact = line.Replace("[Characteristic UUID: Unknown", "").Replace("[", "").Replace("]", "")
                        .Replace(")", "").Replace("(", "").Replace("0x", "")
                        .Trim().ToUpper();
                }
                else if (line.StartsWith("[UUID: Unknown"))
                {
                    charact = line.Replace("[UUID: Unknown", "").Replace("[", "").Replace("]", "")
                        .Replace(")", "").Replace("(", "").Replace("0x", "")
                        .Trim().ToUpper();
                }
                else
                {
                    MessageBox.Show("Do not know line: " + line + " " + counter.ToString());
                    break;
                }
            }
            File.WriteAllText(fname.Replace(".txt", "_out.txt"), sb.ToString());
            MessageBox.Show("OK");
        }
    }
}