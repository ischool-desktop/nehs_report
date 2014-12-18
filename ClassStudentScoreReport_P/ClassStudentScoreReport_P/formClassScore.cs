using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using ClassStudentScoreReport_P.DAO;
using Aspose.Cells;
using System.IO;
using K12.Data;

namespace ClassStudentScoreReport_P
{
    public partial class formClassScore : FISCA.Presentation.Controls.BaseForm
    {
        List<string> _ClassIDList;
        Dictionary<string, JHClassRecord> _classRecDict;
        int _SchoolYear=0, _Semester=0;
        BackgroundWorker _bgLoadData;

        public formClassScore(List<string> ClassIDList)
        {
            InitializeComponent();
            _ClassIDList = ClassIDList;
            _classRecDict = new Dictionary<string, JHClassRecord>();
            _bgLoadData = new BackgroundWorker();
            _bgLoadData.DoWork += _bgLoadData_DoWork;
            _bgLoadData.RunWorkerCompleted += _bgLoadData_RunWorkerCompleted;
            _bgLoadData.WorkerReportsProgress = true;
            _bgLoadData.ProgressChanged += _bgLoadData_ProgressChanged;
        }

        void _bgLoadData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        void _bgLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                Workbook wbe = (Workbook)e.Result;

                if (wbe != null)
                {
                    // 儲存資料
                    Utiltiy.CompletedXls("學生成績總表", wbe);
                }
                else
                    FISCA.Presentation.Controls.MsgBox.Show("產生 Excel 檔案發生錯誤");
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("產生報表發生錯誤," + e.Error.Message);
            }

            btnPrint.Enabled = true;

        }

        void _bgLoadData_DoWork(object sender, DoWorkEventArgs e)
        {

            #region 資料讀取

            // 取得所選班級
            _classRecDict.Clear();
            List<JHClassRecord> classRecList = JHClass.SelectByIDs(_ClassIDList);
            foreach (JHClassRecord rec in classRecList)
                _classRecDict.Add(rec.ID, rec);

            // 讀取學生資料
            Dictionary<string, List<string>> classStudentIDDict = QueryData.GetStudentIDListByClassID(_ClassIDList);

            List<string> StudentIDList = new List<string>();
            foreach (string key in classStudentIDDict.Keys)
                StudentIDList.AddRange(classStudentIDDict[key]);

            // 學生基本資料
            List<JHStudentRecord> StudentRecList = JHStudent.SelectByIDs(StudentIDList);
            Dictionary<string, JHStudentRecord> StudentRecDict = new Dictionary<string, JHStudentRecord>();
            foreach (JHStudentRecord rec in StudentRecList)
                StudentRecDict.Add(rec.ID, rec);

            // 取得評量成績
            Dictionary<string, List<ExamScore>> StudExamScoreDict = QueryData.GetStudentExamScoreDictByStudentIDList(StudentIDList, _SchoolYear, _Semester);

      

            // 取得學期成績
            List<JHSemesterScoreRecord> SemesterScoreRecordList = JHSemesterScore.SelectBySchoolYearAndSemester(StudentIDList, _SchoolYear, _Semester);
            Dictionary<string, JHSemesterScoreRecord> StudSemesterScoreRecordDict = new Dictionary<string, JHSemesterScoreRecord>();
            foreach (JHSemesterScoreRecord rec in SemesterScoreRecordList)
                if (!StudSemesterScoreRecordDict.ContainsKey(rec.RefStudentID))
                    StudSemesterScoreRecordDict.Add(rec.RefStudentID, rec);


            //// 取得缺曠,學生系統編號,缺曠類別,缺曠數
            //Dictionary<string, Dictionary<string, int>> AttendanceRecordDict = new Dictionary<string, Dictionary<string, int>>(); 
            //List<JHAttendanceRecord> AttendanceRecordList = JHAttendance.SelectBySchoolYearAndSemester(StudentRecList, _SchoolYear, _Semester);
            
            //foreach (JHAttendanceRecord rec in AttendanceRecordList)
            //{
            //    foreach (AttendancePeriod ap in rec.PeriodDetail)
            //    {
            //        if (!AttendanceRecordDict.ContainsKey(ap.RefStudentID))
            //            AttendanceRecordDict.Add(ap.RefStudentID, new Dictionary<string, int>());

            //        if (!AttendanceRecordDict[rec.RefStudentID].ContainsKey(ap.AbsenceType))
            //            AttendanceRecordDict[rec.RefStudentID].Add(ap.AbsenceType, 0);

            //        AttendanceRecordDict[rec.RefStudentID][ap.AbsenceType]++;

            //        // 缺曠索引
            //        if (!AttendColIdxDict.ContainsKey(ap.AbsenceType))
            //            AttendColIdxDict.Add(ap.AbsenceType, 0);
            //    }            
            //}
            
            // 取得缺曠資料
            Dictionary<string, AbsenceObj> StudAbsenceObjDict = Utiltiy.GetStudAbsenceDict(_SchoolYear, _Semester);


            #endregion

            
            #region 處理Excel
            
           // 讀取 Template
            Workbook wbTemp = new Workbook(new MemoryStream(Properties.Resources.實驗中學國小部成績總表_樣版));
            Worksheet wstTemp = wbTemp.Worksheets[0];

            // 呈現資料報表
            Workbook wb = new Workbook();

            for (int i = 1; i < classStudentIDDict.Keys.Count; i++)
                wb.Worksheets.Add();

            string ClassName = "";
            int classIDIdx = 0;

            Dictionary<string, List<string>> DomainSubjectDict = new Dictionary<string, List<string>>();

            // 科目開始 Idx
            Dictionary<string, int> SubjBeginIdxDict = new Dictionary<string, int>();

            // 領域開始 Idx
            Dictionary<string, int> DomainBeginDict = new Dictionary<string, int>();

            // 期中平均Idx
            List<int> examColIdx1 = new List<int>();
            // 期末平均 Idx
            List<int> examColIdx2 = new List<int>();

            // 班級
            foreach (string ClassID in classStudentIDDict.Keys)
            {
                DomainSubjectDict.Clear();
                SubjBeginIdxDict.Clear();
                examColIdx1.Clear();
                examColIdx2.Clear();
                // 領域科目名稱                                 
                // 動態建立與收集成績表頭
                foreach (string sid in classStudentIDDict[ClassID])
                {
                    // 整理 DomainName 集合
                    if (StudSemesterScoreRecordDict.ContainsKey(sid))
                    {
                        foreach (string dName in StudSemesterScoreRecordDict[sid].Domains.Keys)
                            if (!DomainSubjectDict.ContainsKey(dName))
                                DomainSubjectDict.Add(dName, new List<string>());

                        foreach (SubjectScore ss in StudSemesterScoreRecordDict[sid].Subjects.Values)
                        {
                            string sDName = "彈性課程";

                            if (!string.IsNullOrEmpty(ss.Domain))
                                sDName = ss.Domain;

                            if (!DomainSubjectDict.ContainsKey(sDName))
                                DomainSubjectDict.Add(sDName, new List<string>());
                            if (!DomainSubjectDict[sDName].Contains(ss.Subject))
                                DomainSubjectDict[sDName].Add(ss.Subject);
                        }                    
                    }
                }               
                
                // 動態處理領域成績報表                
                if (_classRecDict.ContainsKey(ClassID))
                    ClassName = _classRecDict[ClassID].Name;
                
                wb.Worksheets[classIDIdx].Name = ClassName;

                // 班級學生
                wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 0, 0, 3);
                wb.Worksheets[classIDIdx].Cells[0, 0].PutValue(ClassName);

                // 評量所引開始
                int scoreCoIdx = 3;
                int aSubjetLengh = 7;
                int dsNameLenth = 2;
                int mgSNameIdx = 3;
                int mgDNameIdx = 3;


                foreach (string dName in DomainSubjectDict.Keys)
                {
                    int ddNameLengh=0;
                    // 科目
                    foreach (string sName in DomainSubjectDict[dName])
                    {
                        // 放入科目開始 Idx
                        string key = dName + "_" + sName;
                        if (!SubjBeginIdxDict.ContainsKey(key))
                            SubjBeginIdxDict.Add(key, scoreCoIdx);

                        // 評量與科目成績
                        wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 3, scoreCoIdx, aSubjetLengh);
                        scoreCoIdx += aSubjetLengh;
                        wb.Worksheets[classIDIdx].Cells[2, scoreCoIdx-1].PutValue(sName+"成績");
                        // 合併科目名稱
                        wb.Worksheets[classIDIdx].Cells.Merge(1, mgSNameIdx, 1, aSubjetLengh);
                        wb.Worksheets[classIDIdx].Cells[1, mgSNameIdx].PutValue(sName);
                        Style subjSty = wb.Worksheets[classIDIdx].Cells[1, mgSNameIdx].GetStyle();
                        subjSty.HorizontalAlignment = TextAlignmentType.Center;
                        wb.Worksheets[classIDIdx].Cells[1, mgSNameIdx].SetStyle(subjSty);
                        mgSNameIdx = scoreCoIdx;
                        ddNameLengh+=aSubjetLengh;
                    }

                    if (!DomainBeginDict.ContainsKey(dName))
                        DomainBeginDict.Add(dName, scoreCoIdx);

                    // 領域
                    wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 10, scoreCoIdx, dsNameLenth);
                    scoreCoIdx += dsNameLenth;
                    wb.Worksheets[classIDIdx].Cells[2, scoreCoIdx - 2].PutValue(dName + "成績");
                    wb.Worksheets[classIDIdx].Cells[2, scoreCoIdx - 1].PutValue(dName + "名次");
                    // 合併領域名稱
                    wb.Worksheets[classIDIdx].Cells.Merge(0, mgDNameIdx, 1, ddNameLengh + dsNameLenth);
                    wb.Worksheets[classIDIdx].Cells[0, mgDNameIdx].PutValue(dName);
                    Style dominSty = wb.Worksheets[classIDIdx].Cells[0, mgDNameIdx].GetStyle();
                    dominSty.HorizontalAlignment = TextAlignmentType.Center;
                    wb.Worksheets[classIDIdx].Cells[0, mgDNameIdx].SetStyle(dominSty);
                    mgDNameIdx = scoreCoIdx;
                    mgSNameIdx += 2;
                }

                // 課程成績
                wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 12, scoreCoIdx, 2);

                // 期中總平均,期中排名,期末總平均,期末排名,進步成績,進步排名
                wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 14, scoreCoIdx+2, 6);

                // 缺曠
                wb.Worksheets[classIDIdx].Cells.CopyColumns(wstTemp.Cells, 20, scoreCoIdx + 8, 2);

                // 取得平均位置
                for (int col = 3; col <= wb.Worksheets[classIDIdx].Cells.MaxDataColumn; col++)
                {
                    if (wb.Worksheets[classIDIdx].Cells[2, col].StringValue.Contains("期中平均"))
                        examColIdx1.Add(col);

                    if (wb.Worksheets[classIDIdx].Cells[2, col].StringValue.Contains("期末平均"))
                        examColIdx2.Add(col);
                }


                int attColdx = -1;

                // 開始填值
                int rowIdx = 3;
                // 學生
                foreach (string StudentID in classStudentIDDict[ClassID])
                {

                    // 學生基本資料
                    if (StudentRecDict.ContainsKey(StudentID))
                    {
                        wb.Worksheets[classIDIdx].Cells[rowIdx, 0].PutValue(StudentRecDict[StudentID].StudentNumber);
                        wb.Worksheets[classIDIdx].Cells[rowIdx, 1].PutValue(StudentRecDict[StudentID].Name);
                        if (StudentRecDict[StudentID].SeatNo.HasValue)
                            wb.Worksheets[classIDIdx].Cells[rowIdx, 2].PutValue(StudentRecDict[StudentID].SeatNo.Value);
                    }


                    foreach (string dName in DomainSubjectDict.Keys)
                    {
                        foreach (string sName in DomainSubjectDict[dName])
                        {
                            int BIdx = 0;
                            // 取得學生試別並填
                            if (StudExamScoreDict.ContainsKey(StudentID))
                            {
                                string key = dName + "_" + sName;

                                if (SubjBeginIdxDict.ContainsKey(key))
                                    BIdx = SubjBeginIdxDict[key];

                                foreach (ExamScore es in StudExamScoreDict[StudentID])
                                {
                                    if (es.SubjectName == sName && es.DomainName == dName)
                                    {
                                        // 期中
                                        if (es.ExamName.Contains("期中"))
                                        {                                           

                                            if (es.GetAssignmentScore().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx].PutValue(es.GetAssignmentScore().Value);

                                            if (es.GetScore().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx + 1].PutValue(es.GetScore().Value);

                                            if (es.GetAverage().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx + 2].PutValue(es.GetAverage().Value);
                                        }

                                        // 期末
                                        if (es.ExamName.Contains("期末"))
                                        {
                                            if (es.GetAssignmentScore().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx+3].PutValue(es.GetAssignmentScore().Value);

                                            if (es.GetScore().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx + 4].PutValue(es.GetScore().Value);

                                            if (es.GetAverage().HasValue)
                                                wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx + 5].PutValue(es.GetAverage().Value);
                                        }


                                    }
                                }

                                // 學期科目成績
                                if (StudSemesterScoreRecordDict.ContainsKey(StudentID))
                                {
                                    if (StudSemesterScoreRecordDict[StudentID].Subjects.ContainsKey(sName))
                                    { 
                                        if(StudSemesterScoreRecordDict[StudentID].Subjects[sName].Score.HasValue)
                                            wb.Worksheets[classIDIdx].Cells[rowIdx, BIdx + 6].PutValue(StudSemesterScoreRecordDict[StudentID].Subjects[sName].Score.Value);
                                    }                                
                                }
                            }
                        }
                        
                        // 領域成績
                        if (StudSemesterScoreRecordDict.ContainsKey(StudentID))
                        {
                            int dBeginIdx = 0;
                            if (DomainBeginDict.ContainsKey(dName))
                                dBeginIdx = DomainBeginDict[dName];
                        
                            if(StudSemesterScoreRecordDict[StudentID].Domains.ContainsKey(dName))
                            {
                                if (StudSemesterScoreRecordDict[StudentID].Domains[dName].Score.HasValue)
                                    wb.Worksheets[classIDIdx].Cells[rowIdx, dBeginIdx].PutValue(StudSemesterScoreRecordDict[StudentID].Domains[dName].Score.Value);
                            }
                        }
                    }

                    // 課程成績
                    if (StudSemesterScoreRecordDict.ContainsKey(StudentID))
                    {
                        if(StudSemesterScoreRecordDict[StudentID].CourseLearnScore.HasValue)
                            wb.Worksheets[classIDIdx].Cells[rowIdx, scoreCoIdx].PutValue(StudSemesterScoreRecordDict[StudentID].CourseLearnScore.Value);
                    }

                                        
                    // 期中總平均即時運算                    
                    decimal examScore1 = 0;
                    decimal examCount1 = 0;
                    wb.CalculateFormula();
                    foreach (int idx in examColIdx1)
                    {   
                        decimal score1;                        
                        if(decimal.TryParse(wb.Worksheets[classIDIdx].Cells[rowIdx, idx].StringValue,out score1))
                        {
                            examScore1+=score1;
                            examCount1++;
                        }
                    }

                    if(examCount1>0)
                        wb.Worksheets[classIDIdx].Cells[rowIdx, scoreCoIdx+2].PutValue(Math.Round(examScore1/examCount1,0));

                    decimal examScore2 = 0;
                    decimal examCount2 = 0;
                    foreach (int idx in examColIdx2)
                    {   
                        decimal score2;                        
                        if (decimal.TryParse(wb.Worksheets[classIDIdx].Cells[rowIdx, idx].StringValue, out score2))
                        {
                            examScore2 += score2;
                            examCount2++;
                        }
                    }

                    if (examCount2 > 0)
                        wb.Worksheets[classIDIdx].Cells[rowIdx, scoreCoIdx + 4].PutValue(Math.Round(examScore2 / examCount2, 0));

                    
                    // 缺曠
                    if (StudAbsenceObjDict.ContainsKey(StudentID))
                    { 
                        // 事假
                        if(StudAbsenceObjDict[StudentID].PersonalDays.HasValue)
                            wb.Worksheets[classIDIdx].Cells[rowIdx, scoreCoIdx + 8].PutValue(StudAbsenceObjDict[StudentID].PersonalDays.Value);

                        // 病假
                        if(StudAbsenceObjDict[StudentID].SickDays.HasValue)
                            wb.Worksheets[classIDIdx].Cells[rowIdx, scoreCoIdx + 9].PutValue(StudAbsenceObjDict[StudentID].SickDays.Value);
                    }


                    rowIdx++;
                }

                classIDIdx++;
            }

            e.Result = wb;
            #endregion

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            _SchoolYear = iptSchoolYear.Value;
            _Semester = iptSemester.Value;

            _bgLoadData.RunWorkerAsync();
        }

        private void formClassScore_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;

            // 畫面預設値
            int sy,ss;
            if(int.TryParse(K12.Data.School.DefaultSchoolYear,out sy))
                iptSchoolYear.Value = sy;

            if (int.TryParse(K12.Data.School.DefaultSemester, out ss))
                iptSemester.Value = ss;

        }
    }
}
