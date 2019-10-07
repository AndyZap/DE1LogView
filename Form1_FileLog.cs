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

    }
}