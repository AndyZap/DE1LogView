using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class Form1 : Form
    {
        // Save/Load settings -------------------------------------------------

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

                    else if(LoadLineContainsKey(s,  "FormBigPlot.Top")) { FormBigPlot.Top = LoadInt(s, "FormBigPlot.Top"); }
                    else if (LoadLineContainsKey(s, "FormBigPlot.Left")) { FormBigPlot.Left = LoadInt(s, "FormBigPlot.Left"); }
                    else if (LoadLineContainsKey(s, "FormBigPlot.Height")) { FormBigPlot.Height = LoadInt(s, "FormBigPlot.Height"); }
                    else if (LoadLineContainsKey(s, "FormBigPlot.Width")) { FormBigPlot.Width = LoadInt(s, "FormBigPlot.Width"); }

                    else if (LoadLineContainsKey(s, "splitContainer1")) { splitContainer1.SplitterDistance = LoadInt(s, "splitContainer1"); }
                    else if (LoadLineContainsKey(s, "splitContainer2")) { splitContainer2.SplitterDistance = LoadInt(s, "splitContainer2"); }

                    else if (LoadLineContainsKey(s, "ShotsFolder")) { ShotsFolder = LoadString(s, "ShotsFolder"); }
                    else if (LoadLineContainsKey(s, "ProfilesFolder")) { ProfilesFolder = LoadString(s, "ProfilesFolder"); }
                    else if (LoadLineContainsKey(s, "DataFolder")) { DataFolder = LoadString(s, "DataFolder"); }
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

            sb.AppendLine("FormBigPlot.Top                 " + (FormBigPlot.Top < 0 ? "0" : FormBigPlot.Top.ToString()));
            sb.AppendLine("FormBigPlot.Left                " + (FormBigPlot.Left < 0 ? "0" : FormBigPlot.Left.ToString()));
            sb.AppendLine("FormBigPlot.Height              " + (FormBigPlot.Height < 200 ? "200" : FormBigPlot.Height.ToString()));
            sb.AppendLine("FormBigPlot.Width               " + (FormBigPlot.Width < 200 ? "200" : FormBigPlot.Width.ToString()));

            sb.AppendLine("splitContainer1          " + splitContainer1.SplitterDistance.ToString());
            sb.AppendLine("splitContainer2          " + splitContainer2.SplitterDistance.ToString());
            sb.AppendLine("ShotsFolder              " + ShotsFolder);
            sb.AppendLine("ProfilesFolder           " + ProfilesFolder);
            sb.AppendLine("DataFolder               " + DataFolder);

            string fname = ApplicationDirectory + "\\" + ApplicationNameNoExt + ".dat";
            File.WriteAllText(fname, sb.ToString());
        }

        // Wireshark converter  -------------------------------------------------
        private void ConvertWireshark(string fname) // @"D:\platform-tools\__data\7_de1_1\ws_output4.txt"
        {
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

                if (line.StartsWith("Frame")
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
                else if (read_header)
                {
                    var words = line.Trim().Split(' ');
                    frame = words[0].PadLeft(10);
                    time = Convert.ToDouble(words[1]).ToString("0.00").PadLeft(12);
                    read_header = false;
                }
                else if (line.StartsWith("Opcode:"))
                {
                    if (line == "Opcode: Handle Value Notification (0x1b)") opcode = " N_";

                    else if (line.StartsWith("Opcode: Read By Group Type Request")) opcode = "";
                    else if (line.StartsWith("Opcode: Read By Group Type Response")) opcode = "";
                    else if (line.StartsWith("Opcode: Read By Type Request")) opcode = "";
                    else if (line.StartsWith("Opcode: Read By Type Response")) opcode = "";
                    else if (line.StartsWith("Opcode: Error Response")) opcode = "";
                    else if (line.StartsWith("Opcode: Find Information Request")) opcode = "";
                    else if (line.StartsWith("Opcode: Find Information Response")) opcode = "";
                    else if (line.StartsWith("Opcode: Write Request (0x12)")) opcode = " W_";
                    else if (line.StartsWith("Opcode: Write Response (0x13)")) opcode = "";
                    else if (line.StartsWith("Opcode: Read Request (0x0a)")) opcode = "";
                    else if (line.StartsWith("Opcode: Read Response (0x0b)")) opcode = " R_";


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
            MessageBox.Show("Converted " + fname);
        }


        // Profile decoder  -------------------------------------------------

        public class De1ShotHeaderClass    // proc spec_shotdescheader
        {
            public byte HeaderV = 1;    // hard-coded
            public byte NumberOfFrames = 0;    // total num frames
            public byte NumberOfPreinfuseFrames = 0;    // num preinf frames
            public byte MinimumPressure = 0;    // hard-coded, read as {
            public byte MaximumFlow = 0x60; // hard-coded, read as {

            public byte[] bytes;  // to compare bytes

            public De1ShotHeaderClass() { }

            public override string ToString()
            {
                return NumberOfFrames.ToString() + "(" + NumberOfPreinfuseFrames.ToString() + ")";
            }
        }
        public class De1ShotFrameClass  // proc spec_shotframe

        {
            public byte FrameToWrite = 0;
            public byte Flag = 0;
            public double SetVal = 0;         // {
            public double Temp = 0;           // {
            public double FrameLen = 0.0;     // convert_F8_1_7_to_float
            public double TriggerVal = 0;     // {
            public double MaxVol = 0.0;       // convert_bottom_10_of_U10P0

            public byte[] bytes;  // to compare bytes

            public De1ShotFrameClass() { }

            public override string ToString()
            {
                var ctrlF = (Flag & CtrlF) != 0;
                var doCompare = (Flag & DoCompare) != 0;
                var dC_GT = (Flag & DC_GT) != 0;
                var dC_CompF = (Flag & DC_CompF) != 0;
                var tMixTemp = (Flag & TMixTemp) != 0;
                var interpolate = (Flag & Interpolate) != 0;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Temp       " + Temp.ToString("0.#"));
                if (tMixTemp)
                    sb.AppendLine("Temp Mix");

                sb.AppendLine("Length     " + FrameLen.ToString("0.#"));

                if (ctrlF)
                    sb.AppendLine("Flow       " + SetVal.ToString("0.##"));
                else
                    sb.AppendLine("Pressure   " + SetVal.ToString("0.##"));

                if (doCompare)
                {
                    var str1 = dC_CompF ? "Move if Flow" : "Move if Pressure";

                    if (dC_GT)
                        sb.AppendLine(str1 + " > " + TriggerVal.ToString("0.#"));
                    else
                        sb.AppendLine(str1 + " < " + TriggerVal.ToString("0.#"));
                }

                if (interpolate)
                    sb.AppendLine("Interpolate");

                return sb.ToString();
            }
        }

        byte convert_float_to_F8_1_7(double x)
        {
            if (x >= 12.75)  // need to set the high bit on (0x80);
            {
                if (x > 127)
                    return 127 | 0x80;

                else
                    return (byte)(0x80 | (int)(0.5 + x));

            }
            else
            {
                return (byte)(0.5 + x * 10);
            }
        }

        void convert_float_to_U10P0(double x, byte[] data, int index)
        {
            int ival = (int)x;
            var b1 = (byte)(ival / 256);

            data[index] = (byte)(b1 | 0x4); // this is "| 1024" in DE code, need to flip this bit
            data[index + 1] = (byte)(ival - b1 * 256);
        }
        private byte[] EncodeDe1ShotHeader(De1ShotHeaderClass shot_header)
        {
            byte[] data = new byte[5];

            int index = 0;
            data[index] = shot_header.HeaderV; index++;
            data[index] = shot_header.NumberOfFrames; index++;
            data[index] = shot_header.NumberOfPreinfuseFrames; index++;
            data[index] = shot_header.MinimumPressure; index++;
            data[index] = shot_header.MaximumFlow; index++;

            return data;

        }
        private byte[] EncodeDe1ShotFrame(De1ShotFrameClass shot_frame)
        {
            byte[] data = new byte[8];

            int index = 0;
            data[index] = shot_frame.FrameToWrite; index++;
            data[index] = shot_frame.Flag; index++;
            data[index] = (byte)(0.5 + shot_frame.SetVal * 16.0); index++; // note to add 0.5, as "round" is used, not truncate
            data[index] = (byte)(0.5 + shot_frame.Temp * 2.0); index++;
            data[index] = convert_float_to_F8_1_7(shot_frame.FrameLen); index++;
            data[index] = (byte)(0.5 + shot_frame.TriggerVal * 16.0); index++;
            convert_float_to_U10P0(shot_frame.MaxVol, data, index);

            return data;
        }

        private bool ShotTclParser(IList<string> lines, De1ShotHeaderClass shot_header, List<De1ShotFrameClass> shot_frames)
        {
            foreach (var line in lines)
            {
                if (line == ("settings_profile_type settings_2a"))
                    return ShotTclParserPressure(lines, shot_header, shot_frames);
                else if (line == ("settings_profile_type settings_2b"))
                    return ShotTclParserFlow(lines, shot_header, shot_frames);
                else if (line == ("settings_profile_type settings_2c") || line == ("settings_profile_type settings_2c2"))
                    return ShotTclParserAdvanced(lines, shot_header, shot_frames);
            }

            return false;
        }
        private static void TryToGetDoubleFromTclLine(string line, string key, ref double var)
        {

            if (!line.StartsWith(key + " ")) // need space separator to match string
                return;

            var = Convert.ToDouble(line.Replace(key, "").Trim());
        }

        // FrameFlag of zero and pressure of 0 means end of shot, unless we are at the tenth frame, in which case it's the end of shot no matter what
        const byte CtrlF = 0x01; // Are we in Pressure or Flow priority mode?
        const byte DoCompare = 0x02; // Do a compare, early exit current frame if compare true
        const byte DC_GT = 0x04; // If we are doing a compare, then 0 = less than, 1 = greater than
        const byte DC_CompF = 0x08; // Compare Pressure or Flow?
        const byte TMixTemp = 0x10; // Disable shower head temperature compensation. Target Mix Temp instead.
        const byte Interpolate = 0x20; // Hard jump to target value, or ramp?
        const byte IgnoreLimit = 0x40; // Ignore minimum pressure and max flow settings
        private bool ShotTclParserPressure(IList<string> lines, De1ShotHeaderClass shot_header, List<De1ShotFrameClass> shot_frames)
        {
            // header
            shot_header.NumberOfFrames = 3;
            shot_header.NumberOfPreinfuseFrames = 1;

            // preinfusion vars
            double preinfusion_flow_rate = double.MinValue;
            double espresso_temperature = double.MinValue;
            double preinfusion_time = double.MinValue;
            double preinfusion_stop_pressure = double.MinValue;

            // hold vars
            double espresso_pressure = double.MinValue;
            double espresso_hold_time = double.MinValue;

            // decline vars

            double pressure_end = double.MinValue;
            double espresso_decline_time = double.MinValue;

            try
            {
                foreach (var line in lines)
                {
                    TryToGetDoubleFromTclLine(line, "preinfusion_flow_rate", ref preinfusion_flow_rate);
                    TryToGetDoubleFromTclLine(line, "espresso_temperature", ref espresso_temperature);
                    TryToGetDoubleFromTclLine(line, "preinfusion_time", ref preinfusion_time);
                    TryToGetDoubleFromTclLine(line, "preinfusion_stop_pressure", ref preinfusion_stop_pressure);

                    TryToGetDoubleFromTclLine(line, "espresso_pressure", ref espresso_pressure);
                    TryToGetDoubleFromTclLine(line, "espresso_hold_time", ref espresso_hold_time);

                    TryToGetDoubleFromTclLine(line, "pressure_end", ref pressure_end);
                    TryToGetDoubleFromTclLine(line, "espresso_decline_time", ref espresso_decline_time);
                }
            }
            catch (Exception)
            {
                return false;
            }

            // make sure all is loaded
            if (preinfusion_flow_rate == double.MinValue || espresso_temperature == double.MinValue ||
            preinfusion_time == double.MinValue || preinfusion_stop_pressure == double.MinValue ||
            espresso_pressure == double.MinValue || espresso_hold_time == double.MinValue ||
            pressure_end == double.MinValue || espresso_decline_time == double.MinValue
            )
                return false;



            // build the shot frames

            // preinfusion
            De1ShotFrameClass frame1 = new De1ShotFrameClass();
            frame1.FrameToWrite = 0;
            frame1.Flag = CtrlF | DoCompare | DC_GT | IgnoreLimit;
            frame1.SetVal = preinfusion_flow_rate;
            frame1.Temp = espresso_temperature;
            frame1.FrameLen = preinfusion_time;
            frame1.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame1.TriggerVal = preinfusion_stop_pressure;
            shot_frames.Add(frame1);

            // hold
            De1ShotFrameClass frame2 = new De1ShotFrameClass();
            frame2.FrameToWrite = 1;
            frame2.Flag = IgnoreLimit;
            frame2.SetVal = espresso_pressure;
            frame2.Temp = espresso_temperature;
            frame2.FrameLen = espresso_hold_time;
            frame2.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame2.TriggerVal = 0;
            shot_frames.Add(frame2);

            // decline
            De1ShotFrameClass frame3 = new De1ShotFrameClass();
            frame3.FrameToWrite = 2;
            frame3.Flag = IgnoreLimit | Interpolate;
            frame3.SetVal = pressure_end;
            frame3.Temp = espresso_temperature;
            frame3.FrameLen = espresso_decline_time;
            frame3.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame3.TriggerVal = 0;

            shot_frames.Add(frame3);

            // update the byte array inside shot header and frame, so we are ready to write it to DE
            EncodeHeaderAndFrames(shot_header, shot_frames);

            return true;
        }
        private bool ShotTclParserFlow(IList<string> lines, De1ShotHeaderClass shot_header, List<De1ShotFrameClass> shot_frames)
        {
            // header
            shot_header.NumberOfFrames = 4;
            shot_header.NumberOfPreinfuseFrames = 1;

            // preinfusion vars
            double preinfusion_flow_rate = double.MinValue;
            double espresso_temperature = double.MinValue;
            double preinfusion_time = double.MinValue;
            double preinfusion_stop_pressure = double.MinValue;

            // pressure rise vars
            double preinfusion_guarantee = double.MinValue;
            double flow_rise_timeout = double.MinValue;

            // hold vars
            double flow_profile_hold = double.MinValue;
            double espresso_hold_time = double.MinValue;

            // decline vars
            double flow_profile_decline = double.MinValue;
            double espresso_decline_time = double.MinValue;

            try
            {

                foreach (var line in lines)
                {
                    TryToGetDoubleFromTclLine(line, "preinfusion_flow_rate", ref preinfusion_flow_rate);
                    TryToGetDoubleFromTclLine(line, "espresso_temperature", ref espresso_temperature);
                    TryToGetDoubleFromTclLine(line, "preinfusion_time", ref preinfusion_time);
                    TryToGetDoubleFromTclLine(line, "preinfusion_stop_pressure", ref preinfusion_stop_pressure);

                    TryToGetDoubleFromTclLine(line, "preinfusion_guarantee", ref preinfusion_guarantee);
                    TryToGetDoubleFromTclLine(line, "flow_rise_timeout", ref flow_rise_timeout);

                    TryToGetDoubleFromTclLine(line, "flow_profile_hold", ref flow_profile_hold);
                    TryToGetDoubleFromTclLine(line, "espresso_hold_time", ref espresso_hold_time);

                    TryToGetDoubleFromTclLine(line, "flow_profile_decline", ref flow_profile_decline);
                    TryToGetDoubleFromTclLine(line, "espresso_decline_time", ref espresso_decline_time);
                }
            }
            catch (Exception)
            {
                return false;
            }

            // make sure all is loaded
            if (preinfusion_flow_rate == double.MinValue || espresso_temperature == double.MinValue ||
            preinfusion_time == double.MinValue || preinfusion_stop_pressure == double.MinValue ||
            preinfusion_guarantee == double.MinValue ||
            flow_profile_hold == double.MinValue || espresso_hold_time == double.MinValue ||
            flow_profile_decline == double.MinValue || espresso_decline_time == double.MinValue
            )
                return false;

            // flow_rise_timeout is only set with preinfusion_guarantee
            if (preinfusion_guarantee == 1 && flow_rise_timeout == double.MinValue)
                flow_rise_timeout = 10.0;  // AAZ TODO hard-coded - read from 0.shot instead

            // build the shot frames

            // preinfusion
            De1ShotFrameClass frame1 = new De1ShotFrameClass();
            frame1.FrameToWrite = 0;
            frame1.Flag = CtrlF | DoCompare | DC_GT | IgnoreLimit;
            frame1.SetVal = preinfusion_flow_rate;
            frame1.Temp = espresso_temperature;
            frame1.FrameLen = preinfusion_time;
            frame1.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame1.TriggerVal = preinfusion_stop_pressure;
            shot_frames.Add(frame1);

            // pressure rise
            De1ShotFrameClass frame2 = new De1ShotFrameClass();
            frame2.FrameToWrite = 1;
            frame2.Flag = DoCompare | DC_GT | IgnoreLimit;
            frame2.SetVal = preinfusion_stop_pressure;
            frame2.Temp = espresso_temperature;
            frame2.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame2.TriggerVal = preinfusion_stop_pressure;

            if (preinfusion_guarantee == 1 && preinfusion_time > 0)
                frame2.FrameLen = flow_rise_timeout;
            else
                frame2.FrameLen = 0; // a length of zero means the DE1+ will skip this frame

            shot_frames.Add(frame2);

            // hold
            De1ShotFrameClass frame3 = new De1ShotFrameClass();

            frame3.FrameToWrite = 2;
            frame3.Flag = CtrlF | IgnoreLimit;
            frame3.SetVal = flow_profile_hold;
            frame3.Temp = espresso_temperature;
            frame3.FrameLen = espresso_hold_time;
            frame3.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame3.TriggerVal = 0;
            shot_frames.Add(frame3);


            // decline
            De1ShotFrameClass frame4 = new De1ShotFrameClass();
            frame4.FrameToWrite = 3;
            frame4.Flag = CtrlF | IgnoreLimit | Interpolate;
            frame4.SetVal = flow_profile_decline;
            frame4.Temp = espresso_temperature;
            frame4.FrameLen = espresso_decline_time;
            frame4.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
            frame4.TriggerVal = 0;
            shot_frames.Add(frame4);

            // update the byte array inside shot header and frame, so we are ready to write it to DE
            EncodeHeaderAndFrames(shot_header, shot_frames);

            return true;
        }
        string TryGetStringFromDict(string key, Dictionary<string, string> dict)
        {
            if (!dict.ContainsKey(key))
                return "";

            return dict[key];

        }
        double TryGetDoubleFromDict(string key, Dictionary<string, string> dict)
        {
            var s = TryGetStringFromDict(key, dict);

            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception)
            {
                return double.MinValue;
            }
        }
        string FixNamesWithSpaces(string str)
        {
            int level_counter = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c == '{')
                    level_counter++;

                if (level_counter >= 3)
                {
                    if (c != '{' && c != '}' && c != ' ')
                        sb.Append(c);
                }
                else
                    sb.Append(c);

                if (c == '}')

                    level_counter--;
            }
            return sb.ToString();
        }
        private bool ShotTclParserAdvanced(IList<string> lines, De1ShotHeaderClass shot_header, List<De1ShotFrameClass> shot_frames)
        {
            string adv_shot_line_orig = "";
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("advanced_shot"))
                {
                    adv_shot_line_orig = line.Trim();
                    break;
                }
            }
            if (adv_shot_line_orig == "")
                return false;

            var adv_shot_line = FixNamesWithSpaces(adv_shot_line_orig);

            var frame_strings = adv_shot_line.Split('{');
            foreach (var frame_str in frame_strings)
            {
                var fs = frame_str.Replace("}", "").Trim();
                if (fs == "" || fs == "advanced_shot")
                    continue;

                var words = fs.Split(' ');

                if (words.Length % 2 == 1)  // odd number of words, this is a mistake, cannot make a dictionary
                    return false;

                Dictionary<string, string> dict = new Dictionary<string, string>();

                for (int i = 0; i < words.Length; i += 2)
                    dict[words[i]] = words[i + 1];

                // OK, dict is ready, build the frame

                De1ShotFrameClass frame = new De1ShotFrameClass();
                var features = IgnoreLimit;

                // flow control
                var pump = TryGetStringFromDict("pump", dict); if (pump == "") return false;
                if (pump == "flow")
                {
                    features |= CtrlF;
                    var flow = TryGetDoubleFromDict("flow", dict); if (flow == double.MinValue) return false;
                    frame.SetVal = flow;
                }
                else
                {
                    var pressure = TryGetDoubleFromDict("pressure", dict); if (pressure == double.MinValue) return false;
                    frame.SetVal = pressure;
                }

                // use boiler water temperature as the goal
                var sensor = TryGetStringFromDict("sensor", dict); if (sensor == "") return false;
                if (sensor == "water")
                    features |= TMixTemp;

                var transition = TryGetStringFromDict("transition", dict); if (transition == "") return false;

                if (transition == "smooth")
                    features |= Interpolate;

                // "move on if...."
                var exit_if = TryGetDoubleFromDict("exit_if", dict); if (exit_if == double.MinValue) return false;
                if (exit_if == 1)
                {
                    var exit_type = TryGetStringFromDict("exit_type", dict);
                    if (exit_type == "pressure_under")
                    {
                        features |= DoCompare;
                        var exit_pressure_under = TryGetDoubleFromDict("exit_pressure_under", dict);
                        if (exit_pressure_under == double.MinValue) return false;
                        frame.TriggerVal = exit_pressure_under;
                    }
                    else if (exit_type == "pressure_over")
                    {
                        features |= DoCompare | DC_GT;
                        var exit_pressure_over = TryGetDoubleFromDict("exit_pressure_over", dict);
                        if (exit_pressure_over == double.MinValue) return false;
                        frame.TriggerVal = exit_pressure_over;
                    }
                    else if (exit_type == "flow_under")
                    {
                        features |= DoCompare | DC_CompF;
                        var exit_flow_under = TryGetDoubleFromDict("exit_flow_under", dict);
                        if (exit_flow_under == double.MinValue) return false;
                        frame.TriggerVal = exit_flow_under;
                    }
                    else if (exit_type == "flow_over")
                    {
                        features |= DoCompare | DC_GT | DC_CompF;
                        var exit_flow_over = TryGetDoubleFromDict("exit_flow_over", dict);

                        if (exit_flow_over == double.MinValue) return false;
                        frame.TriggerVal = exit_flow_over;
                    }
                    else if (exit_type == "") // no exit condition was checked
                        frame.TriggerVal = 0;
                }
                else
                    frame.TriggerVal = 0; // no exit condition was checked

                var temperature = TryGetDoubleFromDict("temperature", dict); if (temperature == double.MinValue) return false;
                var seconds = TryGetDoubleFromDict("seconds", dict); if (seconds == double.MinValue) return false;

                frame.FrameToWrite = (byte)shot_frames.Count;
                frame.Flag = features;
                frame.Temp = temperature;
                frame.FrameLen = seconds;
                frame.MaxVol = 0; // MaxVol feature has been disabled 5/11/18
                shot_frames.Add(frame);
            }

            // header
            shot_header.NumberOfFrames = (byte)shot_frames.Count;
            shot_header.NumberOfPreinfuseFrames = 1;

            // update the byte array inside shot header and frame, so we are ready to write it to DE
            EncodeHeaderAndFrames(shot_header, shot_frames);

            return true;
        }
        private void EncodeHeaderAndFrames(De1ShotHeaderClass shot_header, List<De1ShotFrameClass> shot_frames)
        {
            shot_header.bytes = EncodeDe1ShotHeader(shot_header);
            foreach (var frame in shot_frames)

                frame.bytes = EncodeDe1ShotFrame(frame);
        }
        private string GetProfileInfo(string fname)
        {
            var lines = File.ReadAllLines(fname);
            De1ShotHeaderClass header_my = new De1ShotHeaderClass();
            List<De1ShotFrameClass> frames_my = new List<De1ShotFrameClass>();
            if (!ShotTclParser(lines, header_my, frames_my))
                return (Path.GetFileNameWithoutExtension(fname) + ": ShotTclParser failed");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Profile: " + Path.GetFileNameWithoutExtension(fname));
            sb.AppendLine("");

            foreach (var fr in frames_my)
                sb.AppendLine(fr.ToString());

            return sb.ToString();
        }
    }
}
