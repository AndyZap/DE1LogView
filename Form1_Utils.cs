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

        // Heatmap -------------------------------------------------

        List<HeatmapEntry> HeatMapP = new List<HeatmapEntry>();
        List<HeatmapEntry> HeatMapR = new List<HeatmapEntry>();

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