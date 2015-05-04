using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHStudentScoreAvgReport.DAO;
using Aspose.Cells;
using System.IO;
using FISCA.UDT;
using JHSchool.Data;

namespace JHStudentScoreAvgReport.Forms
{
    public partial class MainForm : BaseForm
    {
        List<string> _ClassIDList;
        Dictionary<string, Dictionary<string, StudentScore>> _ClassStudentScoreDict;
        Dictionary<string, List<JHSemesterScoreRecord>> _StudentSemesterScoreDict;

        List<string> _StudentIDList;
        Workbook _wbRpt;
        List<string> SubjNameList2;
        List<string> SubjNameList3;
        UDTUserConfig _UserConfig;

        BackgroundWorker _bgWork;
        public MainForm(List<string> ClassIDList)
        {
            InitializeComponent();
            _StudentIDList = new List<string>();
            SubjNameList2 = new List<string>();
            SubjNameList3 = new List<string>();
            _StudentSemesterScoreDict = new Dictionary<string, List<JHSemesterScoreRecord>>();
            _bgWork = new BackgroundWorker();
            _ClassStudentScoreDict = new Dictionary<string, Dictionary<string, StudentScore>>();
            _bgWork.RunWorkerCompleted += _bgWork_RunWorkerCompleted;
            _bgWork.WorkerReportsProgress = true;
            _bgWork.ProgressChanged += _bgWork_ProgressChanged;
            _bgWork.DoWork += _bgWork_DoWork;
            _ClassIDList = ClassIDList;
        }

        void _bgWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("國中各育成績總平均一覽表產生中", e.ProgressPercentage);
        }

        void _bgWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 匯出報表
            if (_wbRpt != null)
                Utility.CompletedXls("國中各育成績總平均一覽表", _wbRpt);

            btnRun.Enabled = true;
        }

        void _bgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudentIDList.Clear();
            _bgWork.ReportProgress(0);
            Cal._RanksTmp.Clear();

            // 取得所選在校班級並排序(班級名稱、座號)
            _ClassStudentScoreDict = Utility.GetClassStudentDict(_ClassIDList);

            foreach (string key in _ClassStudentScoreDict.Keys)
            {
                foreach (string sid in _ClassStudentScoreDict[key].Keys)
                {
                    if (!_StudentIDList.Contains(sid))
                        _StudentIDList.Add(sid);
                }
            }
            _bgWork.ReportProgress(10);
            // 取得成績(科目、領域)
            List<JHSemesterScoreRecord> StudScoreList = JHSemesterScore.SelectByStudentIDs(_StudentIDList);
            foreach (JHSemesterScoreRecord ss in StudScoreList)
            {
                if (!_StudentSemesterScoreDict.ContainsKey(ss.RefStudentID))
                    _StudentSemesterScoreDict.Add(ss.RefStudentID, new List<JHSemesterScoreRecord>());

                _StudentSemesterScoreDict[ss.RefStudentID].Add(ss);
            }
            _bgWork.ReportProgress(40);
            // 解析畫面上資料。



            _bgWork.ReportProgress(50);
            // 分類運算資料(加總、平均、班排名)
            foreach (string className in _ClassStudentScoreDict.Keys)
            {
                foreach (string studID in _ClassStudentScoreDict[className].Keys)
                {
                    if (_ClassStudentScoreDict[className].ContainsKey(studID))
                    {
                        // 分項
                        if (_StudentSemesterScoreDict.ContainsKey(studID))
                        {
                            _ClassStudentScoreDict[className][studID].AddEnteryScore("智育", Cal.CalEntryAvgScore("學業",_StudentSemesterScoreDict[studID]));
                        }

                        // 科目
                        if (_StudentSemesterScoreDict.ContainsKey(studID))
                        {
                            _ClassStudentScoreDict[className][studID].AddEnteryScore("體育", Cal.CalSubjGroupAvgScore(SubjNameList2, _StudentSemesterScoreDict[studID]));
                            _ClassStudentScoreDict[className][studID].AddEnteryScore("美育", Cal.CalSubjGroupAvgScore(SubjNameList3, _StudentSemesterScoreDict[studID]));
                        }
                    }
                }

            }
            _bgWork.ReportProgress(80);
            // 計算排名
            foreach (string ClassName in _ClassStudentScoreDict.Keys)
            {
                #region 班級總分排名
                string sortName = "班級總分排名" + ClassName;
                if (!Cal._RanksTmp.ContainsKey(sortName))
                    Cal._RanksTmp.Add(sortName, new List<decimal>());

                foreach (string studID in _ClassStudentScoreDict[ClassName].Keys)
                    Cal._RanksTmp[sortName].Add(_ClassStudentScoreDict[ClassName][studID].GetSumScore());

                // 排名
                Cal._RanksTmp[sortName].Sort();
                Cal._RanksTmp[sortName].Reverse();
                #endregion

                #region 班級智育排名
                string sortName1 = "班級智育排名" + ClassName;
                if (!Cal._RanksTmp.ContainsKey(sortName1))
                    Cal._RanksTmp.Add(sortName1, new List<decimal>());

                foreach (string studID in _ClassStudentScoreDict[ClassName].Keys)
                    if (_ClassStudentScoreDict[ClassName][studID].GetEntryScore("智育").HasValue)
                        Cal._RanksTmp[sortName1].Add(_ClassStudentScoreDict[ClassName][studID].GetEntryScore("智育").Value);

                // 排名
                Cal._RanksTmp[sortName1].Sort();
                Cal._RanksTmp[sortName1].Reverse();
                #endregion
            }

            _bgWork.ReportProgress(90);
            // 產生報表。(依班級用工作表分類)            
            ExportRpt();
            _bgWork.ReportProgress(100);



        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // 檢查畫面設定
            if (string.IsNullOrWhiteSpace(txtScore1.Text))
            {
                MsgBox.Show("智育必須填值!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtScore2.Text))
            {
                MsgBox.Show("體育必須填值!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtScore3.Text))
            {
                MsgBox.Show("美育必須填值!");
                return;
            }

            #region 解析畫面科目
            SubjNameList2.Clear();
            SubjNameList3.Clear();
            string[] Str2 = txtScore2.Text.Split(',');
            string[] Str3 = txtScore3.Text.Split(',');
            foreach (string ss in Str2)
                SubjNameList2.Add(ss.Replace(" ", ""));

            foreach (string ss in Str3)
                SubjNameList3.Add(ss.Replace(" ", ""));

            #endregion

            btnRun.Enabled = false;

            // 儲存資料
            _UserConfig.Score1Text = txtScore1.Text;
            _UserConfig.Score2Text = txtScore2.Text;
            _UserConfig.Score3Text = txtScore3.Text;
            _UserConfig.Save();
            _bgWork.RunWorkerAsync();

        }

        /// <summary>
        /// 產生報表
        /// </summary>
        private void ExportRpt()
        {
            // 讀取樣版
            _wbRpt = new Workbook(new MemoryStream(Properties.Resources.國中各育成績總平均樣版));

            foreach (string name in _ClassStudentScoreDict.Keys)
            {
                int wstIdx = _wbRpt.Worksheets.AddCopy("樣版");
                _wbRpt.Worksheets[wstIdx].Name = name;
            }
            _wbRpt.Worksheets.RemoveAt("樣版");

            string TitleName = K12.Data.School.ChineseName + " 各育成績總平均一覽表";
            int rowIdx = 4;
            foreach (string className in _ClassStudentScoreDict.Keys)
            {
                Worksheet wst = _wbRpt.Worksheets[className];
                wst.Cells[0, 0].PutValue(TitleName);
                wst.Cells[1, 0].PutValue(className);
                rowIdx = 4;
                foreach (string key in _ClassStudentScoreDict[className].Keys)
                {
                    StudentScore ss = _ClassStudentScoreDict[className][key];

                    wst.Cells[rowIdx, 0].PutValue(ss.SeatNo);
                    wst.Cells[rowIdx, 1].PutValue(ss.Name);
                    if (ss.GetEntryScore("智育").HasValue)
                    {
                        wst.Cells[rowIdx, 2].PutValue(ss.GetEntryScore("智育").Value);
                        wst.Cells[rowIdx, 8].PutValue(ss.GetRank("班級智育排名", ss.GetEntryScore("智育").Value));
                    }
                    if (ss.GetEntryScore("體育").HasValue)
                        wst.Cells[rowIdx, 3].PutValue(ss.GetEntryScore("體育").Value);
                    if (ss.GetEntryScore("美育").HasValue)
                        wst.Cells[rowIdx, 4].PutValue(ss.GetEntryScore("美育").Value);
                    wst.Cells[rowIdx, 5].PutValue(ss.GetSumScore());
                    wst.Cells[rowIdx, 6].PutValue(ss.GetAvgScore());
                    wst.Cells[rowIdx, 7].PutValue(ss.GetRank("班級總分排名", ss.GetSumScore()));

                    rowIdx++;
                }
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtScore1.Text = "學習領域成績(七大學習領域)";
            txtScore1.ReadOnly = true;
            LoadUDTConfig();
            txtScore2.Text = _UserConfig.Score2Text;
            txtScore3.Text = _UserConfig.Score3Text;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("智育：讀取學生各學期學習領域成績(七大學習領域)，成績計算使用算數平均。");
            sb.AppendLine("體育、美育：讀取各學期科目成績，科目名稱1個以上請用,相隔，成績計算使用加權平均。");
            sb.AppendLine("總分：智育、體育、美育，分數加總。");
            sb.AppendLine("平均：總分除以3。");
            sb.AppendLine("名次：依照總分高至低排名。");
            sb.AppendLine("分數進位方式：四捨五入至小數下1位。");
            MsgBox.Show(sb.ToString(), "各育成績總平均計算說明");
        }

        private void LoadUDTConfig()
        {
            // 取得設定值放置畫面  
            AccessHelper accHelper = new AccessHelper();
            List<UDTUserConfig> uList = accHelper.Select<UDTUserConfig>();
            if (uList.Count > 0)
                _UserConfig = uList[0];
            else
            {
                _UserConfig = new UDTUserConfig();
            }
        }
    }
}
