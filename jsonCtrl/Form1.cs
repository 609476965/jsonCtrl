using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace jsonCtrl {
    public enum stateType {
        close,
        open,
        exist
    }
    public partial class Form1 : Form {
        public string path;
        public string bsd = "BattleServerD.exe";
        public string dbcd = "DBCacheD.exe";
        public string gsd = "GameServerD.exe";
        public string gd = "GateD.exe";
        public string wsd = "WorldServerD.exe";
        public Dictionary<int, List<Process>> group = new Dictionary<int, List<Process>>();
        public stateType flag = stateType.close;
        public bool switchFlag = false;
        public string startPath = Application.StartupPath;
        


        public Form1() {
            InitializeComponent();
            group.Clear();
            timer1.Start();
            path = Application.StartupPath + "/config.json";
            if (File.Exists(path) == false) {
                var a = File.Open(path, FileMode.OpenOrCreate);

                string jsonTest = "{ \"1\":{\"id\":1,\"name\":\"吃鱼测试\",\"path\":\"/bin_eatgame/\",\"exeList\":[\"BattleServerD\",\"DBCacheD\",\"GameServerD\",\"GateD\",\"WorldServerD\"] },\"2\":{\"id\":2,\"name\":\"抛圈测试\",\"path\":\"/bin_throwgame/\",\"exeList\":[\"BattleServerD\",\"DBCacheD\",\"GameServerD\",\"GateD\",\"WorldServerD\"]}}";
                StreamWriter sw = new StreamWriter(a);
                sw.Write(jsonTest);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();

                a.Close();
            }
            JsonReader();
        }

        [STAThread]
        public void JsonReader() {
            var s = File.ReadAllText(path);
            //richTextBox1.Text = s;
            JObject c = (JObject)JsonConvert.DeserializeObject(s);
            listView1.BeginUpdate();
            //Process[] pcs = Process.GetProcesses();
            foreach (var a in c) {
                var name = a.Value["name"];
                var path = a.Value["path"];
                ListViewItem lvi = new ListViewItem();
                lvi.Text = name.ToString();
                lvi.SubItems.Add(path.ToString());
                lvi.SubItems.Add("关闭");
                lvi.SubItems.Add(a.Value["exeList"].ToString());
                listView1.Items.Add(lvi);
                List<Process> proc = new List<Process>();
                JArray ja = (JArray)JsonConvert.DeserializeObject(a.Value["exeList"].ToString());
                for (int j = 0; j < ja.Count; j++) {
                    try {
                        string strUpPath = startPath.Substring(0, startPath.LastIndexOf("\\"));
                        string str = strUpPath + path.ToString() + ja[j].ToString();
                        strUpPath = strUpPath.Replace("\\", "/");
                        str = str.Replace("\\", "/");
                        Process[] app = Process.GetProcessesByName(ja[j].ToString());
                        if (app.Length > 0) {
                            for (int l = 0; l < app.Length; l++) {
                                string fName = app[l].MainModule.FileName.Replace("\\", "/");
                                if (fName == str + ".exe") {
                                    app[l].StartInfo.FileName = str;
                                    app[l].StartInfo.WorkingDirectory = strUpPath + path.ToString();
                                    proc.Add(app[l]);
                                }
                            }
                        }
                        //string fName = p.MainModule.FileName;
                        //if (fName == startPath + path.ToString() + ja[j]) {
                        //    proc.Add(p.Id);
                        //    listView1.Items[lvi.Index].SubItems[2].Text = "已启动";
                        //    break;
                        //}
                    }
                    catch (Exception e) {

                    }
                }
                if (proc.Count > 0) {
                    listView1.Items[lvi.Index].SubItems[2].Text = "已启动";
                    group.Add(lvi.Index, proc);
                }
            }
            listView1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void button2_Click(object sender, EventArgs e) {
            int i = 0;
            foreach (ListViewItem lvi in listView1.SelectedItems) {
                i = lvi.Index;

                string pathStr = listView1.Items[lvi.Index].SubItems[1].Text;
                string exeList = listView1.Items[lvi.Index].SubItems[3].Text;
                JArray ja = (JArray)JsonConvert.DeserializeObject(exeList);
                string str = listView1.Items[i].SubItems[2].Text;

                switch (str) {
                    case "已启动":
                        // flag = stateType.close;
                        listView1.Items[i].SubItems[2].Text = "关闭";
                        switchFlag = false;
                        break;
                    case "关闭":
                        if (!group.ContainsKey(i)) {
                            List<Process> proc = new List<Process>();
                            string strUpPath = startPath.Substring(0, startPath.LastIndexOf("\\"));
                            strUpPath = strUpPath.Replace("\\", "/");
                            for (int j = 0; j < ja.Count; j++) {
                                Process pj = new Process();
                                pj.StartInfo.FileName = strUpPath + pathStr + ja[j];
                                pj.StartInfo.WorkingDirectory = strUpPath + pathStr;
                                //pj.Start();
                                proc.Add(pj);
                            }
                            group.Add(i, proc);
                        }
                        //if (group[i].Count > 0) break;

                        // flag = stateType.open;
                        listView1.Items[i].SubItems[2].Text = "已启动";
                        switchFlag = true;
                        break;
                }
                foreach (var a in group[i]) {
                    if (listView1.Items[i].SubItems[2].Text == "关闭")
                        a.Kill();
                    else if (listView1.Items[i].SubItems[2].Text == "已启动")
                        a.Start();
                }
                //using (StreamWriter writer = new StreamWriter(openFileDialog1.FileName)) {
                //    writer.Write(this.richTextBox1.Text);
                //    writer.Close();
                //    JObject c = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(path));
                //    MessageBox.Show(c["1"]["path"].ToString());
                //}
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            foreach (ListViewItem lvi in listView1.SelectedItems) {
                if (lvi.SubItems[2].Text == "关闭")
                    button1.Text = "开启";
                else
                    button1.Text = "关闭";
            }
            JArray ja = (JArray)JsonConvert.DeserializeObject(a.Value["exeList"].ToString());
            for (int j = 0; j < ja.Count; j++) {
                try {
                    string strUpPath = "./" + startPath;
                    string str = strUpPath + path.ToString() + ja[j].ToString();
                    strUpPath = strUpPath.Replace("\\", "/");
                    str = str.Replace("\\", "/");
                    Process[] app = Process.GetProcessesByName(ja[j].ToString());
                    if (app.Length > 0) {
                        for (int l = 0; l < app.Length; l++) {
                            string fName = app[l].MainModule.FileName.Replace("\\", "/");
                            if (fName == str + ".exe") {
                                
                            }
                        }
                    }
                }
                catch (Exception ex) {

                }
            }
        }
    }
}
