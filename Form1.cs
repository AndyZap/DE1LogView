using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class Form1 : Form
    {
        string Revision = "DE1 Log View v1.51";
        string ApplicationDirectory = "";
        string ApplicationNameNoExt = "";

        GraphPainter GraphTop = null;
        GraphPainter GraphBot = null;

        FormBigPlot FormBigPlot = new FormBigPlot();

        public string MainPlotKey = "";
        public string RefPlotKey = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Revision;

            FormBigPlot.parent = this;

            GraphTop = new GraphPainter(splitContainer2.Panel1, this.Font);
            GraphBot = new GraphPainter(splitContainer2.Panel2, this.Font);

            ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            ApplicationNameNoExt = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            LoadSettings();

            string bean_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\BeanList.csv";
            LoadBeanList(bean_fname);

            string profile_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\ProfileInfo.csv";
            ReadProfileInfo(profile_fname);

            string data_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\" + ApplicationNameNoExt + ".csv";
            string video_folder = Directory.Exists(VideoFolder) ? VideoFolder : ApplicationDirectory;
            ReadAllRecords(data_fname, video_folder);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            // btnSaveData_Click(null, EventArgs.Empty); // do not save, to avoid overriding good file on app error
        }

        private void listData_SelectedIndexChanged(object sender, EventArgs e)
        {
            listData.Refresh();

            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
            {
                MainPlotKey = "";
                return;
            }

            MainPlotKey = key;

            txtNotes.Text = String.IsNullOrEmpty(Data[key].notes) ? "" : Data[key].notes;

            PlotDataRec(GraphTop, Data[key]);
        }

        public void PlotDataRec(GraphPainter gp, DataStruct ds)
        {
            if (gp == GraphBot)
            {
                labelBotL.Text = ds.getAsInfoTextForGraph(ProfileInfoList, BeanList);
                labelBotR.Text = "";
            }
            else if (gp == GraphTop)
            {
                labelTopL.Text = ds.getAsInfoTextForGraph(ProfileInfoList, BeanList);
                labelTopR.Text = "";
            }
            else if (gp == FormBigPlot.Graph)
            {
                FormBigPlot.SetLabelText(ds.getAsInfoTextForGraph(ProfileInfoList, BeanList));
            }

            gp.SetAxisTitles("", "");

            gp.data.Clear();

            gp.SetData(0, ds.elapsed, ds.flow_goal, Color.Blue, 2, DashStyle.Dash);
            gp.SetData(1, ds.elapsed, ds.pressure_goal, Color.LimeGreen, 2, DashStyle.Dash);

            gp.SetData(2, ds.elapsed, ds.flow_smooth, Color.Blue, 3, DashStyle.Solid);
            gp.SetData(3, ds.elapsed, ds.pressure_smooth, Color.LimeGreen, 3, DashStyle.Solid);

            gp.SetData(4, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid);

            if (noTemperatureCtrlTToolStripMenuItem.Checked == false)
            {
                List<double> temperature_scaled = new List<double>();
                List<double> temperature_target_scaled = new List<double>();
                foreach (var t in ds.temperature_basket)
                    temperature_scaled.Add(t / 10.0);
                foreach (var t in ds.temperature_goal)
                    temperature_target_scaled.Add(t / 10.0);

                gp.SetData(5, ds.elapsed, temperature_target_scaled, Color.Red, 2, DashStyle.Dash);
                gp.SetData(6, ds.elapsed, temperature_scaled, Color.Red, 3, DashStyle.Solid);
            }

            var pi = ds.getPreinfTime();
            List<double> x_pi = new List<double>(); x_pi.Add(pi); x_pi.Add(pi);
            List<double> y_pi = new List<double>(); y_pi.Add(0); y_pi.Add(1);
            gp.SetData(7, x_pi, y_pi, Color.Brown, 2, DashStyle.Solid);

            gp.SetAutoLimits();

            if (noResistanceCtrlRToolStripMenuItem.Checked == false && ds.pressure_goal.Count != 0)
            {
                List<double> resistance = new List<double>();
                for (int i = 0; i < ds.elapsed.Count; i++)
                {
                    var res = ds.flow_smooth[i] == 0.0 ? 100.0 : Math.Sqrt(ds.pressure_smooth[i]) / ds.flow_smooth[i]; // use as per AdAstra
                    // var res = ds.flow_smooth[i] == 0.0 ? 100.0 : ds.pressure_smooth[i] / (ds.flow_smooth[i] * ds.flow_smooth[i]); // de1app definition
                    // resistance.Add(res/4.0);
                    if (ds.flow_goal[i] <= 0.1 && ds.pressure_goal[i] <= 0.1) // skip when no pressure/flow
                        res = 0.0;

                    resistance.Add(res);
                }

                gp.SetData(8, ds.elapsed, resistance, Color.Fuchsia, 2, DashStyle.Solid);
            }


            gp.panel.Refresh();
        }

        private void listData_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listData.Items.Count)
                return;

            string key = (string)listData.Items[e.Index];

            if (!Data.ContainsKey(key))
                return;

            DataStruct d = Data[key];

            if (checkShowNotes.Checked)
            {
                if (d.notes != "")
                    e.ItemHeight = (int)(e.ItemHeight * 1.7);
            }
        }

        private string TrimStringToDraw(string s, Graphics g, Font font, int width)
        {
            string out_str = s;

            var x = g.MeasureString(out_str, font).ToSize().Width;

            while (x >= width)
            {
                out_str = out_str.Remove(out_str.Length - 1);
                x = g.MeasureString(out_str, font).ToSize().Width;
            }

            return out_str;
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
            if (e.Index % 2 == 0)
            {
                var brush = new SolidBrush(Color.FromArgb(255, 240, 240, 240));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Rectangle myrec = e.Bounds;

            // plot color bar.  blue is the current one, RefPlotKey is red
            myrec.X = labHasPlot.Left + 2; myrec.Width = labHasPlot.Width - 5;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.Blue, myrec);
            else
                e.Graphics.FillRectangle(key == RefPlotKey ? Brushes.Red : Brushes.White, myrec);

            // Text. Move the text a bit down
            myrec.Y += 2;

            myrec.X = labName.Left; myrec.Width = labName.Width;
            e.Graphics.DrawString(d.bean_name, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labProfile.Left; myrec.Width = labProfile.Width;
            e.Graphics.DrawString(d.getShortProfileName(ProfileInfoList), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labGrind.Left; myrec.Width = labGrind.Width;
            e.Graphics.DrawString(d.grind, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            //myrec.X = labEY.Left; myrec.Width = labEY.Width;
            //e.Graphics.DrawString(d.getEY(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labBeanWeight.Left; myrec.Width = labBeanWeight.Width;
            e.Graphics.DrawString(d.bean_weight.ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labRatio.Left; myrec.Width = labRatio.Width;
            e.Graphics.DrawString(d.getRatio().ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labMaxFlow.Left; myrec.Width = labMaxFlow.Width;
            e.Graphics.DrawString(d.getMaxWeightFlow().ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labMaxPr.Left; myrec.Width = labMaxPr.Width;
            e.Graphics.DrawString(d.getMaxPressure().ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDaysSinceRoast.Left; myrec.Width = labDaysSinceRoast.Width;
            e.Graphics.DrawString(d.getAgeStr(BeanList), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            //myrec.X = labPI.Left; myrec.Width = labPI.Width;
            //e.Graphics.DrawString(d.getPreinfTime().ToString("0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDate.Left; myrec.Width = labDate.Width;
            e.Graphics.DrawString(d.getNiceDateStr(DateTime.Now), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labVideo.Left; myrec.Width = labVideo.Width;
            e.Graphics.DrawString(d.has_video ? "v": "", e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            if (checkShowNotes.Checked)
            {
                if (d.notes != "") // notes, on a separate line
                {
                    myrec.X = labGrind.Left + 5; myrec.Width = e.Bounds.Width - labName.Left - 10;
                    myrec.Y += e.Bounds.Height / 2;

                    var notes_str = TrimStringToDraw(d.notes, e.Graphics, e.Font, myrec.Width);
                    if (notes_str.StartsWith("*"))
                        myrec.X -= 20;

                    Font font1 = new Font(e.Font.FontFamily, (float)(e.Font.Size * 0.7), FontStyle.Regular);
                    e.Graphics.DrawString(notes_str, font1, myBrush, myrec, StringFormat.GenericTypographic);
                }
            }
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

        List<string> SmartOutputSort(List<string> input)
        {
            List<DataStruct> list = new List<DataStruct>();
            foreach (var i in input)
                list.Add(Data[i]);

            list.Sort(delegate (DataStruct a1, DataStruct a2)
            {
                if (a1.bean_name != a2.bean_name) { return a2.bean_name.CompareTo(a1.bean_name); }
                else if (a1.profile != a2.profile) { return a2.profile.CompareTo(a1.profile); }
                else if (a1.grind != a2.grind) { return a2.grind.CompareTo(a1.grind); }
                else if (a1.bean_weight != a2.bean_weight) { return a2.bean_weight.CompareTo(a1.bean_weight); }
                else
                {
                    if (Math.Abs(a1.getRatio() - a2.getRatio()) < 0.2) return 0;
                    else return a1.getRatio().CompareTo(a2.getRatio());
                }
            });

            List<string> output_list = new List<string>();
            foreach (var x in list)
                output_list.Add(x.date_str);

            return output_list;
        }

        private void FilterData()
        {
            List<string> sorted_keys = new List<string>();

            var flt_name = txtFilterName.Text.Trim().ToLower();
            var flt_profile = txtFilterProfile.Text.Trim().ToLower();

            int max_days = int.MaxValue;
            if (comboNumItemsToShow.Text == "Show last 31 days")
                max_days = 31;
            else if (comboNumItemsToShow.Text == "Show last 90 days")
                max_days = 90;

            foreach (var key in Data.Keys)
            {
                if (!String.IsNullOrEmpty(flt_name) && Data[key].bean_name.ToLower().Contains(flt_name) == false)
                    continue;

                if (!String.IsNullOrEmpty(flt_profile) && Data[key].getShortProfileName(ProfileInfoList).ToLower().Contains(flt_profile) == false)
                    continue;

                if (!Data[key].enabled)
                    continue;

                if ((DateTime.Now - Data[key].date).TotalDays > max_days)
                    continue;

                if (noSteamRecordsToolStripMenuItem.Checked && Data[key].bean_name == "steam")
                    continue;

                if (checkShowVideoOnly.Checked == true && Data[key].has_video == false)
                    continue;

                sorted_keys.Add(key);
            }

            if (comboSortStyle.Text == "Sort by ID")
                sorted_keys.Sort();
            else
                sorted_keys = SmartOutputSort(sorted_keys);

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

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            listData.Height = splitContainer1.Panel2.Height - panel1.Height - panel2.Height - panel3.Height
            - panel4.Height - panel5.Height - 5;
        }

        private void btnRefPlot_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex != -1)
                RefPlot(listData.SelectedIndex);
        }

        private void RefPlot(int index)
        {
            string key = (string)listData.Items[index];

            if (!Data.ContainsKey(key))
            {
                RefPlotKey = "";
                return;
            }

            if (RefPlotKey == key)
            {
                RefPlotKey = "";
                GraphBot.data.Clear();
                splitContainer2.Panel2.Refresh();
                labelBotL.Text = "";
                listData.Refresh();
                return;
            }

            RefPlotKey = key;

            PlotDataRec(GraphBot, Data[key]);

            listData.Focus();
            listData.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyValue == 66)  // Ctrl B - bean info
                    beanInfoCtrlBF3ToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 67)  // Ctrl C
                    CopyLine();
                if (e.KeyValue == 68)  // Ctrl D - big plot, incl diff plots
                    bigDiffPlotCtrlDToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 80)  // Ctrl P - show/diff profiles
                    DiffProfilesToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 83)  // Ctrl S - Save
                {
                    btnSaveNotes_Click(null, EventArgs.Empty);
                    btnSaveData_Click(null, EventArgs.Empty);
                }
                if (e.KeyValue == 82)  // Ctrl R - Resistance on/off
                {
                    noResistanceCtrlRToolStripMenuItem.Checked = !noResistanceCtrlRToolStripMenuItem.Checked;
                    listData_SelectedIndexChanged(null, EventArgs.Empty);
                }
                if (e.KeyValue == 84)  // Ctrl T - Temperature on/off
                {
                    noTemperatureCtrlTToolStripMenuItem.Checked = !noTemperatureCtrlTToolStripMenuItem.Checked;
                    listData_SelectedIndexChanged(null, EventArgs.Empty);
                }
            }
            else if (e.KeyValue == 112) // F1
            {
                scatterPlotForAllShownToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 113) // F2
            {
                PrintReport();
            }
            else if (e.KeyValue == 114) // F3
            {
                beanInfoCtrlBF3ToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 116) // F5
            {
                showVideoF4ToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 122) // F11
            {
                totalVolumePlotForAllShownF11();
            }
            else if (e.KeyValue == 123) // F12
            {
                linePlotForAllShownF12ToolStripMenuItem_Click(null, EventArgs.Empty);
            }
        }

        private void CopyLine()
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            txtCopy.Text = Data[MainPlotKey].getAsInfoTextForGraph(ProfileInfoList, BeanList);
            txtCopy.SelectAll();
            txtCopy.Copy();
        }

        private void listData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listData.IndexFromPoint(e.Location);
                if (index != -1)
                    RefPlot(index);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            List<string> sorted_keys = new List<string>();
            foreach (var key in Data.Keys)
                sorted_keys.Add(key);

            sorted_keys.Sort();

            StringBuilder sb = new StringBuilder();

            foreach (var key in sorted_keys)
                Data[key].WriteRecord(sb);

            string data_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\" + ApplicationNameNoExt + ".csv";
            File.WriteAllText(data_fname, sb.ToString());
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnMenu, new Point(0, btnMenu.Size.Height), ToolStripDropDownDirection.BelowLeft);
        }

        private void DisableRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex == -1)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            Data[key].enabled = false;

            FilterData();
        }

        private void WiresharkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fname = @"D:\platform-tools\_ble_captures\8_de_new_fw\ws_output1.txt";
            ConvertWireshark(fname);
        }

        private void DiffProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var profile_name = Data[MainPlotKey].profile;

            string fname = ProfilesFolder + "\\" + profile_name + ".tcl";
            if (!File.Exists(fname))
            {
                MessageBox.Show("Profile file does not exist: " + fname);
                return;
            }

            string temp_fname1 = ApplicationDirectory + "\\tmp1.txt";
            string content_fname1 = GetProfileInfo(fname);

            // Check if SelectedPlots exists and different from the main selection
            if (RefPlotKey != MainPlotKey && Data.ContainsKey(RefPlotKey))
            {
                var profile_name2 = Data[RefPlotKey].profile;

                string fname2 = ProfilesFolder + "\\" + profile_name2 + ".tcl";
                if (!File.Exists(fname2))
                {
                    MessageBox.Show("Profile file does not exist: " + fname2);
                    return;
                }

                string temp_fname2 = ApplicationDirectory + "\\tmp2.txt";
                string content_fname2 = GetProfileInfo(fname2);

                if (content_fname2 == content_fname1)
                {
                    FormBigPlot.ShowLog(content_fname1);
                    FormBigPlot.Show();
                }
                else
                {
                    string diff_tool = @"D:\Program Files\Beyond Compare 4\BCompare.exe";
                    if (!File.Exists(diff_tool))
                        diff_tool = @"C:\Program Files\Beyond Compare 4\BCompare.exe";

                    if (File.Exists(diff_tool))
                    {
                        try
                        {
                            File.WriteAllText(temp_fname1, content_fname1);
                            File.WriteAllText(temp_fname2, content_fname2);

                            System.Diagnostics.Process proc = new System.Diagnostics.Process();
                            proc.StartInfo.FileName = diff_tool;
                            proc.StartInfo.Arguments = temp_fname1 + " " + temp_fname2;
                            proc.StartInfo.WorkingDirectory = ApplicationDirectory;
                            proc.Start();
                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("Error running diff", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        }
                    }
                    else
                    {
                        FormBigPlot.ShowLog(content_fname1 + "\r\n\r\n" + content_fname2);
                        FormBigPlot.Show();
                    }
                }
            }
            else
            {
                FormBigPlot.ShowLog(content_fname1);
                FormBigPlot.Show();
            }
        }

        string GetProfileName(string fname)
        {
            var lines = File.ReadAllLines(fname);
            foreach (var line in lines)
            {
                if (!line.Trim().StartsWith("profile_title {"))
                    continue;

                return line.Trim().Replace("profile_title {", "").Replace("}", "").Trim();
            }
            return Path.GetFileNameWithoutExtension(fname);
        }

        private void FixProfileFileNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fnames = Directory.GetFiles(ProfilesFolder, "_*.tcl", SearchOption.TopDirectoryOnly);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Files fixed:");
            foreach (var fname in fnames)
            {
                var new_fname = GetProfileName(fname) + ".tcl";
                var old_fname = Path.GetFileNameWithoutExtension(fname) + ".tcl";

                if (new_fname != old_fname)
                {
                    sb.AppendLine(old_fname + " -> " + new_fname);

                    var txt = File.ReadAllText(fname);
                    File.Delete(fname);
                    File.WriteAllText(Path.GetDirectoryName(fname) + "\\" + new_fname, txt);
                }
            }

            MessageBox.Show(sb.ToString());
        }

        private void openProfilesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = ProfilesFolder;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening Profiles Folder", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void writeTextInfoForAllProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fnames = Directory.GetFiles(ProfilesFolder, "*.tcl", SearchOption.TopDirectoryOnly);

            var folder_name = ApplicationDirectory + "\\profile_info";
            if (!Directory.Exists(folder_name))
                Directory.CreateDirectory(folder_name);

            var fnames_in_output_folder = Directory.GetFiles(folder_name, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var f in fnames_in_output_folder)
                File.Delete(f);

            foreach (var fname in fnames)
            {
                var output_fname = folder_name + "\\" + Path.GetFileNameWithoutExtension(fname) + ".txt";
                File.WriteAllText(output_fname, GetProfileInfo(fname));
            }

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = folder_name;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening Folder " + folder_name, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnSaveNotes_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            Data[key].notes = txtNotes.Text.Replace(",", " ");

            FilterData();
        }
        private void bigDiffPlotCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);


            FormBigPlot.noTemperature = noTemperatureCtrlTToolStripMenuItem.Checked;
            FormBigPlot.noResistance = noResistanceCtrlRToolStripMenuItem.Checked;
            FormBigPlot.ShowGraph(all_keys);

            FormBigPlot.Show();
        }
        void btnImportData_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(ShotsFolder))
            {
                MessageBox.Show("ERROR: ShotsFolder location is not set");
                return;
            }
            var old_count = Data.Count;

            var files = Directory.GetFiles(ShotsFolder, "*.shot", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                if (Path.GetFileNameWithoutExtension(f) == "0") // skip 0.shot, this is a config file for DE1Win10
                    continue;

                var key = ReadDateFromShotFile(f);
                if (Data.ContainsKey(key))
                    continue;

                if (key == "")
                {
                    MessageBox.Show("ERROR: when reading date from shot file " + f);
                    return;
                }


                string read_status = "";
                DataStruct d = new DataStruct(f, DataStruct.getMaxId(Data), ref read_status);

                if (read_status != "")
                {
                    MessageBox.Show("ERROR: when reading shot file " + f + ", " + read_status);
                    return;
                }

                Data.Add(d.date_str, d); // if the record is not empty, add to the master list
            }
            FilterData();

            int num_tds = LoadTDS();
            var str_tds = num_tds == 0 ? "" : (" and " + num_tds.ToString() + " TDS recs");

            MessageBox.Show("Loaded " + (Data.Count - old_count).ToString() + " shot files" + str_tds);
        }

        private int LoadTDS()
        {
            var tds_file_name = DataFolder + "\\TDS.csv";

            if (!File.Exists(tds_file_name))
                return 0;

            // build TDS rec dictionary
            Dictionary<int, string> tds_dict = new Dictionary<int, string>();
            var lines = File.ReadAllLines(tds_file_name);
            foreach (var str in lines)
            {
                var line = str.Trim();
                if (line == "")
                    continue;

                var words = line.Split(',');
                tds_dict[Convert.ToInt32(words[0])] = line.Remove(0, words[0].Length + 1);
            }

            // build Data ID to key dict
            Dictionary<int, string> data_dict = new Dictionary<int, string>();
            foreach (string key in Data.Keys)
            {
                var rec = Data[key];
                data_dict[rec.id] = key;
            }

            // now check if we can add new records
            int num_added = 0;
            foreach (int tds_key in tds_dict.Keys)
            {
                if (!data_dict.ContainsKey(tds_key))
                    continue;

                var data_key = data_dict[tds_key];

                if(Data[data_key].tds == "")
                {
                    Data[data_key].tds = tds_dict[tds_key];
                    num_added++;
                }
            }

            return num_added;
        }

        private void beanInfoCtrlBF3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var text = "";
            var key = Data[MainPlotKey].bean_name;


            if (BeanList.ContainsKey(key))
            {
                StringBuilder sb = new StringBuilder();

                var b = BeanList[key];

                sb.AppendLine("ShortName    " + b.ShortName);
                sb.AppendLine("FullName     " + b.FullName);
                sb.AppendLine("Country      " + b.Country);
                sb.AppendLine("CountryCode  " + b.CountryCode);
                sb.AppendLine("From         " + b.From);
                sb.AppendLine("Roasted      " + (b.Roasted == DateTime.MinValue ? "" : b.Roasted.ToString("dd/MM/yyyy")));
                sb.AppendLine("Frozen       " + (b.Frozen == DateTime.MinValue ? "" : b.Frozen.ToString("dd/MM/yyyy")));
                sb.AppendLine("Defrosted    " + (b.Defrosted == DateTime.MinValue ? "" : b.Defrosted.ToString("dd/MM/yyyy")));
                sb.AppendLine("Process      " + b.Process);
                sb.AppendLine("Varietals    " + b.Varietals);
                sb.AppendLine("Notes        " + b.Notes);
                sb.AppendLine("Cupping      " + b.Cupping);
                text = sb.ToString();
            }
            else
                text = "no bean info found";

            FormBigPlot.ShowLog(text);
            FormBigPlot.Show();
        }

        private void splitContainer2_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = GraphTop.ToDataX(e.X);
            var y = GraphTop.ToDataY(e.Y);

            // try to get the dynamic ratio for the current point
            string current_ratio_txt = "";
            if (Data.ContainsKey(MainPlotKey))
            {
                var ratio = Data[MainPlotKey].getCurrentRatio(x);
                var weight = Data[MainPlotKey].getCurrentWeight(x);
                var frame = Data[MainPlotKey].getCurrentFrame(x);
                current_ratio_txt = "  Fr" + frame.ToString("0") + " " + weight.ToString("0.0") + "g/" + ratio.ToString("0.0");
            }

            labelTopR.Text = x.ToString("0.0") + ", " + y.ToString("0.0") + current_ratio_txt;
        }

        private void splitContainer2_Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            var x = GraphBot.ToDataX(e.X);
            var y = GraphBot.ToDataY(e.Y);

            labelBotR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
        }
        private void PrintReport()
        {
            StringBuilder sb = new StringBuilder();

            // get the max size for the bean and profile names
            int max_bean_len = 0;
            int max_profile_len = 0;
            foreach (string item in listData.Items)
            {
                var d = Data[item];
                max_bean_len = Math.Max(d.bean_name.Length, max_bean_len);
                max_profile_len = Math.Max(d.getShortProfileName(ProfileInfoList).Length, max_profile_len);

            }

            List<string> keys = new List<string>();
            if (comboSortStyle.Text == "Sort by ID")
            {
                for (int i = listData.Items.Count - 1; i >= 0; i--)
                {
                    var key = (string)listData.Items[i];

                    var ds = Data[key];

                    if (ds.bean_name.ToLower() == "steam")
                        continue;

                    keys.Add(key);
                }
            }
            else
            {
                foreach (string item in listData.Items)
                {
                    var key = item;

                    var ds = Data[key];

                    if (ds.bean_name.ToLower() == "steam")
                        continue;

                    keys.Add(item);
                }
            }

            if (keys.Count != 0)
            {
                double sum = 0.0;
                double sum2 = 0.0;
                int num = 0;
                List<double> all_values = new List<double>();

                var last_name = Data[keys[0]].bean_name;
                foreach (var key in keys)
                {
                    if (last_name != Data[key].bean_name)
                        sb.AppendLine("");

                    sb.AppendLine(Data[key].getAsInfoText(ProfileInfoList, BeanList, max_bean_len: max_bean_len, max_profile_len: max_profile_len));

                    last_name = Data[key].bean_name;

                    // calc retained volume stats
                    var rv = Data[key].retained_volume;
                    if (rv == 0.0)
                        continue;

                    sum += rv;
                    sum2 += rv * rv;
                    num++;
                    all_values.Add(rv);
                }

                if (sum != 0.0)
                {
                    var std = Math.Sqrt(num * sum2 - sum * sum) / num;

                    var percent_10 = 1 + (int)(num * 0.1);
                    if (percent_10 * 2 >= num)
                        percent_10 = 0;

                    all_values.Sort();


                    sb.AppendLine("");
                    sb.AppendLine("Retained Volume Av=" + (sum / num).ToString("0.0") + " Std=" + std.ToString("0.0") + " Num=" + num.ToString()
                         + " 10%=" + all_values[percent_10].ToString("0.0") + " 90%=" + all_values[all_values.Count-1-percent_10].ToString("0.0"));
                }
            }

            // START AAZ TESTING PROFILE ---------
            /*sb.AppendLine(""); sb.AppendLine("");


            var fnames = Directory.GetFiles(ProfilesFolder, "_*.tcl", SearchOption.TopDirectoryOnly);

            Dictionary<string, int> profiles_counts = new Dictionary<string, int> ();
            Dictionary<string, int> profiles_stars = new Dictionary<string, int>();
            HashSet<string> not_found_profiles = new HashSet<string>();

            foreach (var fname in fnames)
            {
                var profile_name = GetProfileName(fname);
                profiles_counts.Add(profile_name, 0);
                profiles_stars.Add(profile_name, 0);
            }

            foreach(var d in Data.Values)
            {
                if(!profiles_counts.ContainsKey(d.profile))
                {
                    if (!not_found_profiles.Contains(d.profile))
                        not_found_profiles.Add(d.profile);
                }
                else
                {
                    profiles_counts[d.profile] += 1;
                    if(d.notes.StartsWith("*"))
                        profiles_stars[d.profile] += 1;
                }
            }

            foreach(var key in profiles_counts.Keys)
                sb.AppendLine(key.Remove(0,1) + "\t" + profiles_counts[key] + "\t" + profiles_stars[key]);

            sb.AppendLine("");
            sb.AppendLine("Not found:");

            foreach (var s in not_found_profiles)
                sb.AppendLine(s);

            */
            // END AAZ TESTING PROFILE ---------

            FormBigPlot.ShowLog(sb.ToString());
            FormBigPlot.Show();
        }
        private void saveDataCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSaveNotes_Click(null, EventArgs.Empty);
            btnSaveData_Click(null, EventArgs.Empty);
        }

        private void txtFilterName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                if (txtFilterName.Text == "")
                    txtFilterProfile.Text = "";
                else
                    txtFilterName.Text = "";
            }
        }
        private void txtFilterProfile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                if (txtFilterProfile.Text == "")
                    txtFilterName.Text = "";
                else
                    txtFilterProfile.Text = "";
            }
        }
        private void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void txtFilterProfile_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void comboNumItemsToShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void comboSortStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void checkShowNotes_CheckedChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void showVideoF4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var id = Data[MainPlotKey].id;

            string fname = (Directory.Exists(VideoFolder) ? VideoFolder : ApplicationDirectory) + "\\" + id.ToString() + "-1.m4v";
            if (!File.Exists(fname))
                return;

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = fname;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening video", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
        private void openVideoFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(VideoFolder))
            {
                MessageBox.Show("Video folder is not set", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = VideoFolder;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening video", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
        private void printReportCtrlPF2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintReport();
        }
        private void scatterPlotForAllShownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);

            FormBigPlot.ShowScatterGraph(all_keys);

            FormBigPlot.Show();
        }
        public void SetSelected()
        {
            if (MainPlotKey != "" && RefPlotKey == "")
            {
                listData.SelectedItem = MainPlotKey;
                listData.Refresh();
            }
        }
        private void listFrozenBeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ShortName     ");
            sb.Append("FullName                 ");
            sb.Append("Country       ");
            sb.Append("Code ");
            sb.Append("From          ");
            sb.Append("Frozen        ");
            sb.AppendLine("");

            foreach (var b in BeanList.Values)
            {
                if (b.Frozen != DateTime.MinValue && b.Defrosted == DateTime.MinValue)
                {
                    sb.Append(b.ShortName.PadRight(14));
                    sb.Append(b.FullName.PadRight(25));
                    sb.Append(b.Country.PadRight(14));
                    sb.Append(b.CountryCode.PadRight(5));
                    sb.Append(b.From.PadRight(14));
                    sb.Append(b.Frozen.ToString("dd/MM/yyyy"));

                    sb.AppendLine("");
                }
            }

            FormBigPlot.ShowLog(sb.ToString());
            FormBigPlot.Show();
        }
        private void linePlotForAllShownF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);

            FormBigPlot.ShowLineGraphAll(all_keys);

            FormBigPlot.Show();
        }
        private void totalVolumePlotForAllShownF11()
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);

            FormBigPlot.ShowTotalVolumeGraphAll(all_keys);

            FormBigPlot.Show();
        }
        private void noSteamRecordsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void writeSRTProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(ProfilesFolder))
            {
                MessageBox.Show("Cannot find profiles folder");
                return;
            }

            string template_name = ProfilesFolder + "\\_SRT_template.txt";
            if (!File.Exists(template_name))
            {
                MessageBox.Show("Cannot find _SRT_template.txt in the profiles folder");
                return;
            }

            int num_srt_profiles = writeSRTProfiles(template_name);
            int num_lc_profiles = writeLCProfiles(template_name);
            MessageBox.Show("OK, wrote " + num_srt_profiles.ToString() + " SRT profiles and " + num_lc_profiles.ToString() + " LC profiles");
        }
        private int writeSRTProfiles(string template_name) 
        { 
            var input_fnames = Directory.GetFiles(Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory, "SRT_*.txt", SearchOption.TopDirectoryOnly);
               
            foreach(var input_fname in input_fnames)
            {
                string template = File.ReadAllText(template_name);

                var lines = File.ReadAllLines(input_fname);

                var words = lines[0].Split('\t');
                var T1 = GetTemplateVar(words[1]);
                var F1 = GetTemplateVar(words[3]);
                var P1 = GetTemplateVar(words[5]);
                template = template.Replace("$T1$", T1).Replace("$F1$", F1).Replace("$P1$", P1);

                words = lines[1].Split('\t');
                var T2 = GetTemplateVar(words[1]);
                var P2 = GetTemplateVar(words[3]);
                var S2 = GetTemplateVar(words[5], time_reading: true);
                template = template.Replace("$T2$", T2).Replace("$P2$", P2).Replace("$S2$", S2);

                words = lines[2].Split('\t');
                var T3 = GetTemplateVar(words[1]);
                var F3 = GetTemplateVar(words[3]);
                var S3 = GetTemplateVar(words[5], time_reading: true);
                template = template.Replace("$T3$", T3).Replace("$F3$", F3).Replace("$S3$", S3);

                words = lines[3].Split('\t');
                var T4 = GetTemplateVar(words[1]);
                var F4 = GetTemplateVar(words[3]);
                template = template.Replace("$T4$", T4).Replace("$F4$", F4);


                var new_fname = "_" + Path.GetFileNameWithoutExtension(input_fname);
                template = template.Replace("$NAME$", new_fname);

                File.WriteAllText(ProfilesFolder + "\\" + new_fname + ".tcl", template);
            }

            return input_fnames.Length;
        }

        private int writeLCProfiles(string template_name)
        {
            // LC_B10_R12_18_92.txt

            var input_fnames = Directory.GetFiles(Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory, "LC_*.txt", SearchOption.TopDirectoryOnly);

            foreach (var input_fname in input_fnames)
            {
                string template = File.ReadAllText(template_name);


                var words = Path.GetFileNameWithoutExtension(input_fname).Split('_');
                if (words.Length != 5)
                    return 0;

                var __T = words[4] + ".0";
                var __F = words[3][0] + "." + words[3][1];
                var __B = words[1].Remove(0, 1);
                var __R = words[2].Remove(0, 1);

                var T1 = __T;
                var F1 = "4.0";
                var P1 = "4.0";
                template = template.Replace("$T1$", T1).Replace("$F1$", F1).Replace("$P1$", P1);

                var T2 = __T;
                var P2 = "2.5";
                var S2 = __B;
                template = template.Replace("$T2$", T2).Replace("$P2$", P2).Replace("$S2$", S2);

                var T3 = __T;
                var F3 = __F;
                var S3 = __R;
                template = template.Replace("$T3$", T3).Replace("$F3$", F3).Replace("$S3$", S3);

                var T4 = __T;
                var F4 = __F;
                template = template.Replace("$T4$", T4).Replace("$F4$", F4);


                var new_fname = "_" + Path.GetFileNameWithoutExtension(input_fname);
                template = template.Replace("$NAME$", new_fname);

                File.WriteAllText(ProfilesFolder + "\\" + new_fname + ".tcl", template);
            }

            return input_fnames.Length;
        }

        string GetTemplateVar(string input, bool time_reading = false)
        {
            if(time_reading)
                return Convert.ToDouble(input.Trim()).ToString("0") + ".00";
            else
                return Convert.ToDouble(input.Trim()).ToString("0.0") + "0";
        }

        private void checkShowVideoOnly_CheckedChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        // ------------------------------------------------------------------------------

        private void pulseWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder = @"D:\_Kar\_PulseWatch";
            var files_names = Directory.GetFiles(folder, "*.csv", SearchOption.TopDirectoryOnly);

            StringBuilder sb_summary = new StringBuilder();
            int summary_counter = 0;
            List<string> summary_names = new List<string>();

            foreach(var file_name in files_names)
            {
                var txt_file_name = file_name.Replace(".csv", ".txt");
                if (!File.Exists(txt_file_name))
                {
                    var lines = File.ReadAllLines(file_name);

                    StringBuilder sb = new StringBuilder();
                    foreach (var line in lines)
                    {
                        if (line == "")
                            continue;

                        if (line == "Time,Heart Rate")
                            continue;


                        var words = line.Split(',');
                        DateTime dt = DateTime.Parse(words[0]);
                        DateTime dt0 = DateTime.Parse("00:00:00");

                        TimeSpan ts = dt - dt0;
                        sb.AppendLine(ts.TotalHours.ToString("0.000") + "  " + words[1]);

                    }

                    File.WriteAllText(txt_file_name, sb.ToString());
                }

                summary_names.Add(Path.GetFileNameWithoutExtension(txt_file_name));
                summary_counter++;

                // summary updates
                if (summary_counter == 4)
                {
                    sb_summary.AppendLine("NUM_PLOTS      2");
                    sb_summary.AppendLine("X_SIZE         8");
                    sb_summary.AppendLine("PLOT           1");
                    sb_summary.AppendLine("HEIGHT         700");
                    sb_summary.AppendLine("DATA           \"" + summary_names[0].Remove(0, 5) + "\" \"line solid\" \"blue\"    2 \"" + summary_names[0] + ".txt\"");
                    sb_summary.AppendLine("DATA           \"" + summary_names[1].Remove(0, 5) + "\" \"line solid\" \"red\"     2 \"" + summary_names[1] + ".txt\"");

                    sb_summary.AppendLine("PLOT           2");
                    sb_summary.AppendLine("HEIGHT         700");
                    sb_summary.AppendLine("DATA           \"" + summary_names[2].Remove(0, 5) + "\" \"line solid\" \"fuchsia\" 2 \"" + summary_names[2] + ".txt\"");
                    sb_summary.AppendLine("DATA           \"" + summary_names[3].Remove(0, 5) + "\" \"line solid\" \"silver\"  2 \"" + summary_names[3] + ".txt\"");

                    var summary_file_name = folder + "\\_" + summary_names[0].Remove(0, 5) + "___" + summary_names[3].Remove(0, 5) + ".ini";
                    File.WriteAllText(summary_file_name, sb_summary.ToString());

                    sb_summary.Clear();
                    summary_names.Clear();
                    summary_counter = 0;
                }
            }

            // the final summary
            if (summary_counter != 0)
            {
                while (summary_names.Count < 4)
                    summary_names.Add(summary_names.Last());

                sb_summary.AppendLine("NUM_PLOTS      2");
                sb_summary.AppendLine("X_SIZE         8");
                sb_summary.AppendLine("PLOT           1");
                sb_summary.AppendLine("HEIGHT         700");
                sb_summary.AppendLine("DATA           \"" + summary_names[0].Remove(0, 5) + "\" \"line solid\" \"blue\"    2 \"" + summary_names[0] + ".txt\"");
                sb_summary.AppendLine("DATA           \"" + summary_names[1].Remove(0, 5) + "\" \"line solid\" \"red\"     2 \"" + summary_names[1] + ".txt\"");

                sb_summary.AppendLine("PLOT           2");
                sb_summary.AppendLine("HEIGHT         700");
                sb_summary.AppendLine("DATA           \"" + summary_names[2].Remove(0, 5) + "\" \"line solid\" \"fuchsia\" 2 \"" + summary_names[2] + ".txt\"");
                sb_summary.AppendLine("DATA           \"" + summary_names[3].Remove(0, 5) + "\" \"line solid\" \"silver\"  2 \"" + summary_names[3] + ".txt\"");

                var summary_file_name = folder + "\\_" + summary_names[0].Remove(0, 5) + "___" + summary_names[3].Remove(0, 5) + ".ini";
                File.WriteAllText(summary_file_name, sb_summary.ToString());
            }
        }

        private void noTemperatureCtrlTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listData_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void noResistanceCtrlRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listData_SelectedIndexChanged(null, EventArgs.Empty);
        }


        /*
        void SearchByNotes()
        {
            var words = txtNotes.Text.ToLower().Trim().Split(' ');
            if (words.Length < 7)
                return;

            var name = words[0].Trim();
            double bean_w = 0.0;
            double coffee_w = 0.0;
            string grind = "";

            for (int i = 1; i < words.Length - 1; i++)
            {
                if (words[i] == "->")
                {
                    bean_w = Convert.ToDouble(words[i - 1]);
                    coffee_w = Convert.ToDouble(words[i + 1]);
                }

                if (words[i] == "grind")
                    grind = words[i + 1];
            }

            foreach(var key in Data.Keys)
            {
                var d = Data[key];

                if(    d.bean_weight == bean_w  
                    && d.coffee_weight == coffee_w
                    && d.name == name
                    && d.grind == grind)
                {
                    MainPlotKey = key;
                    RefPlotKey = "";
                    SetSelected();
                    break;
                }
            }
        }
        */

    }
}