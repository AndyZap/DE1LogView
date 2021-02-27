using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class Form1 : Form
    {
        string ShotsFolder = "";
        string ProfilesFolder = "";
        string DataFolder = "";
        string VideoFolder = "";

        public Dictionary<string, DataStruct> Data = new Dictionary<string, DataStruct>();
        public Dictionary<string, BeanEntryClass> BeanList = new Dictionary<string, BeanEntryClass>();
        public Dictionary<string, ProfileInfo> ProfileInfoList = new Dictionary<string, ProfileInfo>();
        public class DataStruct
        {
            public bool enabled = true;
            public string notes = "";
            public string tds = "";
            public bool has_video = false;
            public double retained_volume = 0;

            // readonly part
            public readonly string date_str = "";
            public readonly DateTime date = DateTime.MinValue;
            public readonly int id = 0;
            public readonly string bean_name = "";
            public readonly double bean_weight = 0;
            public readonly double coffee_weight = 0;
            public readonly string grind = "";
            public readonly double shot_time = 0;
            public readonly string profile = "";

            public  readonly List<double> elapsed = new List<double>();
            private readonly List<double> flow = new List<double>();      // use the smoothed versions outside the class
            private readonly List<double> pressure = new List<double>();  // use the smoothed versions outside the class
            public  readonly List<double> weight = new List<double>();
            public  readonly List<double> flow_weight = new List<double>();
            public  readonly List<double> temperature_basket = new List<double>();
            public  readonly List<double> temperature_mix = new List<double>();
            public  readonly List<double> pressure_goal = new List<double>();
            public  readonly List<double> flow_goal = new List<double>();
            public  readonly List<double> temperature_goal = new List<double>();
            public  readonly List<double> espresso_frame = new List<double>();

            public  readonly List<double> pressure_smooth = new List<double>();
            public  readonly List<double> flow_smooth = new List<double>();

            public DataStruct(string shot_fname, int record_id, ref string read_error) // ctor from shot record
            {
                var lines = File.ReadAllLines(shot_fname);
                try
                {
                    foreach (var s in lines)
                    {
                        var line = s.Trim().Replace("  ", " ").Replace("  ", " ").Replace("  ", " "); // trim and remove double spaces
                        if (String.IsNullOrEmpty(line))
                            continue;

                        if (line.StartsWith("clock "))
                        {
                            string clock_str = line.Replace("clock ", "").Trim();
                            date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(clock_str)).LocalDateTime;
                            date_str = date.ToString("yyyy MM dd ddd HH:mm");

                        }
                        else if (line.StartsWith("espresso_elapsed {"))
                        {
                            elapsed = ReadList(line, "espresso_elapsed ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_pressure {"))
                        {
                            pressure = ReadList(line, "espresso_pressure ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_weight {"))
                        {
                            weight = ReadList(line, "espresso_weight ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_flow {"))
                        {
                            flow = ReadList(line, "espresso_flow ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_flow_weight {"))
                        {
                            flow_weight = ReadList(line, "espresso_flow_weight ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_temperature_basket {"))
                        {
                            temperature_basket = ReadList(line, "espresso_temperature_basket ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_temperature_mix {"))
                        {
                            temperature_mix = ReadList(line, "espresso_temperature_mix ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_pressure_goal {"))
                        {
                            pressure_goal = ReadList(line, "espresso_pressure_goal ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_flow_goal {"))
                        {
                            flow_goal = ReadList(line, "espresso_flow_goal ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_temperature_goal {"))
                        {
                            temperature_goal = ReadList(line, "espresso_temperature_goal ", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_frame {"))
                        {
                            espresso_frame = ReadList(line, "espresso_frame ", min_limit: -100); // no min limit here
                        }
                        else if (line.StartsWith("drink_weight "))
                        {
                            coffee_weight = ReadDouble(line, "drink_weight ");
                        }
                        else if (line.StartsWith("dsv2_bean_weight ") && bean_weight == 0.0)      // old string from DSV skin
                        {
                            bean_weight = ReadDouble(line, "dsv2_bean_weight ");
                        }
                        else if (line.StartsWith("DSx_bean_weight ") && bean_weight == 0.0)       // old string from DSV skin
                        {
                            bean_weight = ReadDouble(line, "DSx_bean_weight ");
                        }
                        else if (line.StartsWith("grinder_dose_weight ") && bean_weight == 0.0)   // new string from insight skin
                        {
                            bean_weight = ReadDouble(line, "grinder_dose_weight ");
                        }
                        else if (line.StartsWith("grinder_setting {"))
                        {
                            grind = ReadString(line, "grinder_setting ");
                        }
                        else if (line.StartsWith("bean_brand {"))
                        {
                            bean_name = ReadString(line, "bean_brand ");
                        }
                        else if (line.StartsWith("espresso_notes {"))
                        {
                            notes = ReadString(line, "espresso_notes ");
                        }
                        else if (line.StartsWith("profile_title {"))
                        {
                            profile = ReadString(line, "profile_title ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    read_error = "Exception: " + ex.Message;
                    return;
                }

                if (elapsed.Count == 0)
                {
                    read_error = "Error: no time data or empty record";
                    return;
                }

                if (bean_name != "steam" && bean_name != "filter" && (weight[weight.Count - 1] == 0.0 || bean_weight == 0.0))
                {
                    read_error = "Error: no weight data from scale or bean weight is zero";
                    return;
                }


                // trim data at the end of the shot if the weight does not change. Do not do for steam!
                {
                    int last = weight.Count - 1;
                    while (bean_name != "steam" && bean_name != "filter" && weight[last] == weight[last - 1])
                    {
                        if (weight[last] == 0.0)
                            break;

                        weight.RemoveAt(last);

                        while (elapsed.Count != weight.Count)
                        {
                            elapsed.RemoveAt(last);
                            pressure.RemoveAt(last);
                            flow.RemoveAt(last);
                            flow_weight.RemoveAt(last);
                            temperature_basket.RemoveAt(last);
                            temperature_mix.RemoveAt(last);
                            pressure_goal.RemoveAt(last);
                            flow_goal.RemoveAt(last);
                            temperature_goal.RemoveAt(last);
                            espresso_frame.RemoveAt(last);
                        }

                        last = weight.Count - 1;
                    }
                }

                // setup the fields which are not saved in the file
                shot_time = elapsed[elapsed.Count - 1];
                id        = record_id;

                //while (espresso_frame.Count != elapsed.Count) // fix for old records without espresso_frame, do not need this
                //    espresso_frame.Add(0.0);

                if (bean_name == "steam") // fixes for the STEAM
                {
                    int target_length = elapsed.Count;
                    for (int i = 0; i < elapsed.Count; i++)
                    {
                        if (flow[i] < 0.3 && pressure[i] < 0.5)
                        {
                            target_length = i;
                            break;
                        }
                    }

                    while (elapsed.Count > target_length)
                    {
                        int last = elapsed.Count - 1;
                        weight.RemoveAt(last);
                        elapsed.RemoveAt(last);
                        pressure.RemoveAt(last);
                        flow.RemoveAt(last);
                        flow_weight.RemoveAt(last);
                        temperature_basket.RemoveAt(last);
                        temperature_mix.RemoveAt(last);
                        pressure_goal.RemoveAt(last);
                        flow_goal.RemoveAt(last);
                        temperature_goal.RemoveAt(last);
                        espresso_frame.RemoveAt(last);
                    }

                    for (int i = 0; i < elapsed.Count; i++)
                    {
                        weight[i] = 0.0;
                        flow_weight[i] = 0.0;
                        temperature_goal[i] = 0.0;
                    }

                    bean_weight = 1;
                    coffee_weight = 1;
                    grind = "";
                    shot_time = 1;

                    profile = "";
                    tds = "";
                    has_video = false;
                    retained_volume = 1;
                }
                else // fixes for the REGULAR records
                {
                    // old way to fix the end of the shot
                    /*
                    if (flow_goal.Count != 0 && flow.Count != 0)  // remove the flow at 4 mls target at the end of the shot
                    {
                        for (int i = elapsed.Count - 1; i > 0; i--)
                        {
                            if (flow_goal[i] == 4.0)
                            {
                                flow[i] = 0.0;
                                flow_goal[i] = 0.0;
                            }
                            else
                                break;
                        }
                    }*/

                    // new way to fix the end of the shot
                    for (int i = 0; i < elapsed.Count; i++)
                    {
                        if (espresso_frame[i] == -1)
                        {
                            flow_goal[i] = 0.0;
                            pressure_goal[i] = 0.0;
                        }
                    }
                }

                // generate the smooth flow and pressure arrays
                flow_smooth = SmoothArrayData(flow);
                pressure_smooth = SmoothArrayData(pressure);
            }

            public DataStruct(List<string> lines, ref string read_error)
            {
                try
                {
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("clock "))
                        {
                            date_str = line.Replace("clock ", "").Trim();
                            date = DateTime.Parse(date_str);
                        }
                        else if (line.StartsWith("enabled "))
                        {
                            enabled = 1 == (int)ReadDouble(line, "enabled ");
                        }
                        else if (line.StartsWith("record_id "))
                        {
                            id = (int)ReadDouble(line, "record_id ");
                        }
                        else if (line.StartsWith("name "))
                        {
                            bean_name = ReadString(line, "name ");
                        }
                        else if (line.StartsWith("bean_weight "))
                        {
                            bean_weight = ReadDouble(line, "bean_weight ");
                        }
                        else if (line.StartsWith("coffee_weight "))
                        {
                            coffee_weight = ReadDouble(line, "coffee_weight ");
                        }
                        else if (line.StartsWith("grind "))
                        {
                            grind = ReadString(line, "grind ");
                        }
                        else if (line.StartsWith("shot_time "))
                        {
                            shot_time = ReadDouble(line, "shot_time ");
                        }
                        else if (line.StartsWith("notes "))
                        {
                            notes = ReadString(line, "notes ");
                        }
                        else if (line.StartsWith("profile "))
                        {
                            profile = ReadString(line, "profile ");
                        }
                        else if (line.StartsWith("tds "))
                        {
                            tds = ReadString(line, "tds ");
                        }
                        else if (line.StartsWith("elapsed {"))
                        {
                            elapsed = ReadList(line, "elapsed {", min_limit: 0);
                        }
                        else if (line.StartsWith("pressure {"))
                        {
                            pressure = ReadList(line, "pressure {", min_limit: 0);
                        }
                        else if (line.StartsWith("weight {"))
                        {
                            weight = ReadList(line, "weight {", min_limit: 0);
                        }
                        else if (line.StartsWith("flow {"))
                        {
                            flow = ReadList(line, "flow {", min_limit: 0);
                        }
                        else if (line.StartsWith("flow_weight {"))
                        {
                            flow_weight = ReadList(line, "flow_weight {", min_limit: 0);
                        }
                        else if (line.StartsWith("temperature_basket {"))
                        {
                            temperature_basket = ReadList(line, "temperature_basket {", min_limit: 0);
                        }
                        else if (line.StartsWith("temperature_mix {"))
                        {
                            temperature_mix = ReadList(line, "temperature_mix {", min_limit: 0);
                        }
                        else if (line.StartsWith("pressure_goal {"))
                        {
                            pressure_goal = ReadList(line, "pressure_goal {", min_limit: 0);
                        }
                        else if (line.StartsWith("flow_goal {"))
                        {
                            flow_goal = ReadList(line, "flow_goal {", min_limit: 0);
                        }
                        else if (line.StartsWith("temperature_goal {"))
                        {
                            temperature_goal = ReadList(line, "temperature_goal {", min_limit: 0);
                        }
                        else if (line.StartsWith("espresso_frame {")) // no min limit here
                        {
                            espresso_frame = ReadList(line, "espresso_frame {", min_limit: -100);
                        }
                        else
                        {
                            read_error = "Unknown line: " + line;
                        }
                    }
                }
                catch (Exception ex)
                {
                    read_error = "Exception: " + ex.Message;
                }

                // generate the smooth flow and pressure arrays
                flow_smooth = SmoothArrayData(flow);
                pressure_smooth = SmoothArrayData(pressure);
            }

            private List<double> SmoothArrayData(List<double> list)
            {
                List<double> out_list = new List<double>();
                foreach (var x in list)
                    out_list.Add(x);

                if (list.Count < 3)
                    return out_list;

                for (int i = 1; i < list.Count - 1; i++)
                    out_list[i] = (list[i - 1] + list[i] + list[i + 1]) / 3.0;

                return out_list;
            }

            public void WriteRecord(StringBuilder sb)
            {
                sb.AppendLine("clock " + date_str);
                sb.AppendLine("enabled " + (enabled ? "1" : "0"));
                sb.AppendLine("record_id " + id.ToString());
                sb.AppendLine("name " + bean_name);
                sb.AppendLine("bean_weight " + bean_weight.ToString());
                sb.AppendLine("coffee_weight " + coffee_weight.ToString());
                sb.AppendLine("grind " + grind);
                sb.AppendLine("shot_time " + shot_time.ToString());
                sb.AppendLine("notes " + notes);
                sb.AppendLine("profile " + profile);
                sb.AppendLine("tds " + tds);

                sb.AppendLine(WriteList(elapsed, "elapsed", "0.0##"));
                sb.AppendLine(WriteList(pressure, "pressure", "0.0#"));
                sb.AppendLine(WriteList(weight, "weight", "0.0"));
                sb.AppendLine(WriteList(flow, "flow", "0.0#"));
                sb.AppendLine(WriteList(flow_weight, "flow_weight", "0.0#"));
                sb.AppendLine(WriteList(temperature_basket, "temperature_basket", "0.0"));
                sb.AppendLine(WriteList(temperature_mix, "temperature_mix", "0.0"));
                sb.AppendLine(WriteList(pressure_goal, "pressure_goal", "0.0"));
                sb.AppendLine(WriteList(flow_goal, "flow_goal", "0.0"));
                sb.AppendLine(WriteList(temperature_goal, "temperature_goal", "0.0"));
                sb.AppendLine(WriteList(espresso_frame, "espresso_frame", "0"));
                sb.AppendLine();
            }

            public string WriteList(List<double> list, string key, string format)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(key + " {");
                foreach (var x in list)
                    sb.Append(x.ToString(format) + " ");

                sb.Append("}");

                return sb.ToString().Replace(" }", "}");
            }

            public double getMaxPressure()
            {
                if (pressure_smooth.Count == 0 || pressure_goal.Count == 0)
                    return 0.0;

                var first_drop = getFirstDropTime();

                double max_press = 0.0;
                for (int i = 1; i < pressure_smooth.Count; i++)
                {
                    if (elapsed[i] < first_drop)
                        continue;

                    if (flow_goal[i] <= 0.1 && pressure_goal[i] <= 0.1) // skip when no pressure/flow goal
                        continue;

                    if (pressure_smooth[i] > max_press)
                        max_press = pressure_smooth[i];
                }

                return max_press;
            }

            public string getEY()
            {
                if (tds == "")
                    return "";

                var words = tds.Split(',');
                for(int i = words.Length-1; i >= 0; i--)
                {
                    if (words[i].StartsWith("*"))
                        continue;

                    var val = Convert.ToDouble(words[i]);

                    return (val * getRatio()).ToString("0.0");
                }

                return "";
            }

            private double getFirstDropTime()
            {
                for(int i = 0; i < weight.Count; i++)
                {
                    if (weight[i] > 0.1)
                        return elapsed[i];
                }

                return 0.0;
            }

            private double flowTimeAdjustment(double f, double first_drop)
            {
                double flow_time = f;

                if (flow_weight.Count == 0 || flow_goal.Count == 0)
                    return flow_time;

                // remove 0 flow points
                for (int i = 1; i < elapsed.Count; i++)
                {
                    if (elapsed[i] < first_drop)
                        continue;

                    if (flow_goal[i] <= 0.1 && pressure_goal[i] <= 0.1) // skip when no pressure/flow goal
                        continue;

                    if ((flow_smooth.Count != 0 && flow_smooth[i] < 0.2) || (flow_weight.Count != 0 && flow_weight[i] < 0.07))
                        flow_time -= elapsed[i] - elapsed[i - 1];
                }

                return flow_time;
            }

            public double getMaxWeightFlow()
            {
                if (flow_weight.Count == 0 || flow_goal.Count == 0)
                    return 0.0;

                var first_drop = getFirstDropTime();

                double max_flow = 0.0;
                for (int i = 1; i < flow_weight.Count; i++)
                {
                    if (elapsed[i] < first_drop)
                        continue;

                    if (flow_goal[i] <= 0.1 && pressure_goal[i] <= 0.1) // skip when no pressure/flow goal
                        continue;

                    if (flow_weight[i] > max_flow)
                        max_flow = flow_weight[i];
                }

                return max_flow;
            }
            public double getPreinfTime()
            {
                var first_drop = getFirstDropTime();
                double flow_time = shot_time - first_drop;

                flow_time = flowTimeAdjustment(flow_time, first_drop);

                return shot_time - flow_time;
            }

            public string getShortProfileName()
            {
                if (profile.StartsWith("_"))
                    return profile.Remove(0, 1);
                else
                    return profile;
            }

            public double getRatio()
            {
                return coffee_weight / bean_weight;
            }

            public double getCurrentRatio(double time)
            {
                if (weight.Count == 0)
                    return 0.0;

                for (int i = 0; i < elapsed.Count; i++)
                {
                    if (time <= elapsed[i])
                        return weight[i] / bean_weight;
                }

                // return final ratio
                return getRatio();
            }
            public double getCurrentWeight(double time)
            {
                if (weight.Count == 0)
                    return 0.0;

                for (int i = 0; i < elapsed.Count; i++)
                {
                    if (time <= elapsed[i])
                        return weight[i];
                }

                // return final weight
                return coffee_weight;
            }
            public double getCurrentFrame(double time)
            {
                if (espresso_frame.Count == 0)
                    return 0.0;

                for (int i = 0; i < elapsed.Count; i++)
                {
                    if (time <= elapsed[i])
                        return espresso_frame[i];
                }

                return -1; // end of shot, -1 frame
            }

            public string getAgeStr(Dictionary<string, BeanEntryClass> bean_list)
            {
                if (!bean_list.ContainsKey(bean_name))
                    return "";

                var age = bean_list[bean_name].DatesSinceRoast(date);

                if (age == 0)
                    return "";

                var age_str = age >= 0 ? (age.ToString() + "d") : ((-age).ToString() + "d*");

                return age_str;
            }

            public string getNiceDateStr(DateTime dt)
            {
                var dt1 = new DateTime(dt.Year, dt.Month, dt.Day, date.Hour, date.Minute, date.Second);

                TimeSpan ts = dt1 - date;
                string nice_d = "";
                if (ts.TotalDays < 1)
                    nice_d = "T0 " + date.ToString("HH:mm");
                else if (ts.TotalDays > 28)
                    nice_d = date.ToString("dd/MM/yy");
                else
                    nice_d += date.ToString("dd/MM") + " " + date.ToString("HH:mm");

                return nice_d;
            }

            public string getAsInfoText(Dictionary<string, BeanEntryClass> bean_list,
                                        int max_bean_len, int max_profile_len)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(bean_name.PadRight(max_bean_len) + "   ");
                sb.Append(getShortProfileName().PadRight(max_profile_len) + "   ");
                sb.Append(grind.PadRight(6));
                sb.Append("R" + getRatio().ToString("0.0").PadRight(5) + " ");
                sb.Append("Ey" + getEY().PadRight(4) + " ");
                //sb.Append("Pi" + getPreinfTime().ToString("0").PadRight(4));
                sb.Append("F" + getMaxWeightFlow().ToString("0.0").PadRight(4));
                sb.Append("P" + getMaxPressure().ToString("0.0").PadRight(4));

                var age = getAgeStr(bean_list);
                sb.Append(age.PadLeft(age.Contains("*") ? 6: 5).PadRight(7));
                sb.Append("B" + bean_weight.ToString("0.0") + "    ");
                sb.Append("#" + id.ToString().PadRight(5) + " ");
                sb.Append(getNiceDateStr(DateTime.MaxValue).PadRight(9));

                //getTotalWaterVolume();
                //sb.Append("Rv" + retained_volume.ToString("0.0") + " ");

                sb.Append((notes.StartsWith("*") ? "" : "  ") + notes);

                return sb.ToString();
            }
            public string getAsInfoTextForGraph(Dictionary<string, BeanEntryClass> bean_list)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("#" + id.ToString().PadRight(5));
                sb.Append(bean_name + "    ");
                sb.Append("\""+ getShortProfileName() + "\"    ");
                sb.Append("G" + grind + "    ");
                sb.Append("R" + getRatio().ToString("0.0") + "    ");
                sb.Append("Ey" + getEY() + "    ");
                //sb.Append("Pi" + getPreinfTime().ToString("0") + "   ");
                sb.Append("F" + getMaxWeightFlow().ToString("0.0") + "   ");
                sb.Append("P" + getMaxPressure().ToString("0.0") + "    ");
                sb.Append(getAgeStr(bean_list) + "    ");
                sb.Append("B" + bean_weight.ToString("0.0") + "    ");
                sb.Append(getNiceDateStr(DateTime.MaxValue) + "    ");
#if STEAM_STUDY
                sb.Append(getAveragePressure_7_22() + "    ");
#endif
                sb.Append(notes);

                return sb.ToString();
            }

            public static int getMaxId(Dictionary<string, DataStruct> data)
            {
                int max_id = -1;

                foreach(var value in data.Values)
                    max_id = Math.Max(value.id, max_id);

                return max_id + 1;
            }

            public List<double> getTotalWaterVolume()
            {
                List<double> total_list = new List<double>();

                if (flow.Count == 0)
                    return total_list;

                double total = 0;
                total_list.Add(total);
                for (int i = 1; i < elapsed.Count; i++)
                {
#if STEAM_STUDY
                    if (elapsed[i] >= 7 && elapsed[i] <= 22)  // to estimate 7-22 sec interval
#endif
                    total += flow[i]*(elapsed[i] - elapsed[i - 1]);  // method 1 (rectangular)
                    //total += 0.5 * (flow[i] + flow[i-1]) * (elapsed[i] - elapsed[i - 1]);  // method 2 (trapeziodal)

                    total_list.Add(total);
                }

                retained_volume = total_list[total_list.Count - 1] - weight[weight.Count - 1];

                return total_list;
            }

            public double GetTimeRef(Dictionary<string, ProfileInfo> prof_dict)
            {
                // get the ref frame from the profile
                int ref_frame = 0;
                if (prof_dict.ContainsKey(profile))
                    ref_frame = prof_dict[profile].ref_frame;

                double ref_time = 0;

                for(int i = 1; i < elapsed.Count; i++)
                {
                    if (espresso_frame[i - 1] != ref_frame && espresso_frame[i] == ref_frame)
                        ref_time = elapsed[i];
                }

                return ref_time;
            }

#if STEAM_STUDY
            public string getAveragePressure_7_22()
            {
                if (pressure_smooth.Count == 0)
                    return "";

                double sum = 0.0;
                double sum2 = 0.0;
                int num = 0;

                for (int i = 1; i < elapsed.Count; i++)
                {
                    if (elapsed[i] >= 7 && elapsed[i] <= 22)  // to estimate 7-22 sec interval
                    {
                        sum += pressure_smooth[i];
                        sum2 += pressure_smooth[i] * pressure_smooth[i];
                        num++;
                    }
                }

                var average = sum / num;
                var std = Math.Sqrt(num * sum2 - sum * sum) / num;

                return "AvP" + average.ToString("0.0") +  " Std" + std.ToString("0.00");
            }
#endif
        } // End of DataStruct

        static List<double> ReadList(string line, string key, double min_limit)
        {
            var str = line.Remove(0, key.Length);

            str = str.Replace("{", "").Replace("}", "").Trim();

            List<double> res = new List<double>();

            if (String.IsNullOrEmpty(str))
                return res;

            var words = str.Split(' ');
            foreach (var w in words)
            {
                var x = Convert.ToDouble(w.Trim());
                x = Math.Max(x, min_limit);
                res.Add(x);
            }

            return res;
        }
        static string ReadString(string line, string key)
        {
            var str = line.Remove(0, key.Length);
            return str.Replace("{", "").Replace("}", "").Trim();
        }
        static double ReadDouble(string line, string key)
        {
            return Convert.ToDouble(ReadString(line, key));
        }
        string ReadDateFromShotFile(string fname)
        {
            var lines = File.ReadAllLines(fname);
            foreach (var s in lines)
            {
                var line = s.Trim();
                if (String.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("clock "))
                {
                    string clock_str = line.Replace("clock ", "").Trim();
                    DateTime dt = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(clock_str)).LocalDateTime;

                    return dt.ToString("yyyy MM dd ddd HH:mm");
                }
            }

            return "";
        }

        private void ReadAllRecords(string fname, string video_folder = "")
        {
            List<string> record_lines = new List<string>();

            var lines = File.ReadAllLines(fname);
            var counter = 0;
            foreach (var s in lines)
            {
                counter++;
                var line = s.TrimStart();
                if (String.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("clock "))
                {
                    if (record_lines.Count != 0)
                    {
                        string read_status = "";
                        DataStruct d = new DataStruct(record_lines, ref read_status);
                        if (read_status != "")
                        {
                            MessageBox.Show("ERROR reading DE1LogView.csv file, see record which ends at line " + (counter - 1).ToString() 
                                + " Error:" + read_status);
                            return;
                        }

                        // flag if the video exists
                        string video_file = video_folder + "\\" + d.id.ToString() + "-1.m4v";
                        d.has_video = File.Exists(video_file);

                        Data.Add(d.date_str, d);
                    }
                    record_lines.Clear();
                }

                record_lines.Add(line);
            }

            if (record_lines.Count != 0)
            {
                string read_status = "";
                DataStruct d = new DataStruct(record_lines, ref read_status);
                if (read_status != "")
                {
                    MessageBox.Show("ERROR reading DE1LogView.csv file, see record which ends at line " + (counter - 1).ToString()
                        + " Error:" + read_status);
                    return;
                }

                // flag if the video exists
                string video_file = video_folder + "\\" + d.id.ToString() + "-1.m4v";
                d.has_video = File.Exists(video_file);

                Data.Add(d.date_str, d);
            }

            FilterData();
        }

        // Beanlist file --------------------------------------

        public class BeanEntryClass
        {
            public string ShortName = "";
            public string FullName = "";
            public string Country = "";
            public string From = "";
            public DateTime Roasted = DateTime.MinValue;
            public DateTime Frozen = DateTime.MinValue;
            public DateTime Defrosted = DateTime.MinValue;
            public string Process = "";
            public string Varietals = "";
            public string Notes = "";

            public BeanEntryClass(string s)
            {
                var words = s.Split(',');

                ShortName = words[0].Trim().ToLower();
                FullName = words[1].Trim();
                Country = words[2].Trim();
                From = words[3].Trim();

                var dt = words[4].Trim();
                Roasted = dt == "" ? DateTime.MinValue : DateTime.Parse(dt);

                dt = words[5].Trim();
                Frozen = dt == "" ? DateTime.MinValue : DateTime.Parse(dt);

                dt = words[6].Trim();
                Defrosted = dt == "" ? DateTime.MinValue : DateTime.Parse(dt);

                Process = words[7].Trim();
                Varietals = words[8].Trim();
                Notes = words[9].Trim();
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ShortName + ",");
                sb.Append(FullName + ",");
                sb.Append(Country + ",");
                sb.Append(From + ",");
                sb.Append(Roasted == DateTime.MinValue ? "," : Roasted.ToString("dd/MM/yyyy") + ",");
                sb.Append(Frozen == DateTime.MinValue ? "," : Frozen.ToString("dd/MM/yyyy") + ",");
                sb.Append(Defrosted == DateTime.MinValue ? "," : Defrosted.ToString("dd/MM/yyyy") + ",");
                sb.Append(Process + ",");
                sb.Append(Varietals + ",");
                sb.Append(Notes);

                return sb.ToString();
            }

            public int DatesSinceRoast(DateTime dt)  // returns 0 on error. "+" is was not frozen, "-" otherwise
            {
                if (Roasted == DateTime.MinValue)
                    return int.MinValue;

                if (Defrosted == DateTime.MinValue && Frozen == DateTime.MinValue)
                {
                    var diff = (int)(dt - Roasted).TotalDays;
                    if (diff < 0)
                        return 0;

                    return diff;
                }

                if (Defrosted == DateTime.MinValue || Frozen == DateTime.MinValue)  // not consistent data
                    return 0;

                var diff_total = (int)(dt - Roasted).TotalDays;
                if (diff_total < 0)
                    return 0;

                var diff_gap = (int)(Defrosted - Frozen).TotalDays;
                if (diff_gap < 0)
                    return 0;

                return (diff_total - diff_gap) * -1; // *-1 to indicate defrosted
            }
        }
        private void LoadBeanList(string fname)
        {
            BeanList.Clear();

            if (!File.Exists(fname))
                return;

            var lines = File.ReadAllLines(fname);
            foreach (var line in lines)
            {
                if (line.StartsWith("Short name,Full name,"))
                    continue;

                if (line.Trim() == "")
                    continue;

                var bn = new BeanEntryClass(line);

                BeanList[bn.ShortName] = bn;
            }

            /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Short name,Full name,Country,Country code,From,Roasted,Frozen,Defrosted,Process,Varietals,Notes,Cupping");
            foreach (var key in blist.Keys)
                sb.AppendLine(blist[key].ToString());
            File.WriteAllText(fname, sb.ToString());

            var bbb = blist["rock"];
            var da = bbb.DatesSinceRoast(DateTime.Now); */
        }


        // Profile info ------------------

        public enum KpiTypeEnum { Pressure, Flow }
        public class ProfileInfo
        {
            public string full_name = "";
            public int    ref_frame = 0;
            public Color  color = Color.Yellow;

            public ProfileInfo(string s)
            {
                var words = s.Split(',');
                full_name = words[0].Trim();
                ref_frame = Convert.ToInt32(words[1].Trim());

                var cl_str = words[2].Trim();
                if     (cl_str == "Blue") color = Color.Blue;
                else if(cl_str == "Fuchsia") color = Color.Fuchsia;
                else if(cl_str == "Lime") color = Color.Lime;
                else if(cl_str == "Black") color = Color.Black;
                else if(cl_str == "YellowGreen") color = Color.YellowGreen;
                else if(cl_str == "Green") color = Color.Green;
                else if(cl_str == "Aqua") color = Color.Aqua;
                else if(cl_str == "Silver") color = Color.Silver;
                else color = Color.Silver;
            }
        }

        void ReadProfileInfo(string fname)
        {
            ProfileInfoList.Clear();

            if (!File.Exists(fname))
                return;

            var lines = File.ReadAllLines(fname);
            foreach (var line in lines)
            {
                if (line.StartsWith("Name,"))
                    continue;

                if(String.IsNullOrEmpty(line.Trim()))
                        continue;

                ProfileInfo p = new ProfileInfo(line);
                ProfileInfoList[p.full_name] = p;
            }
        }
    }
}
