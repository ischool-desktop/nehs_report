using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHStudentScoreAvgReport.DAO;
using FISCA.Data;
using System.Data;
using Aspose.Cells;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHStudentScoreAvgReport
{
    public class Utility
    {
        /// <summary>
        /// 取得班級學生
        /// </summary>
        /// <param name="ClassIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, StudentScore>> GetClassStudentDict(List<string> ClassIDList)
        {
            Dictionary<string, Dictionary<string, StudentScore>> retVal = new Dictionary<string, Dictionary<string, StudentScore>>();
            if (ClassIDList.Count > 0)
            {
                // -- ClassID,StudentID,ClassName,StudentName,SeatNo
                string strSQL = "select class.id as classid,student.id as studentid,class_name as classname,student.name as studentname,student.seat_no from student inner join class on student.ref_class_id=class.id where student.status=1 and class.id in(" + string.Join(",", ClassIDList.ToArray()) + ") order by class_name,student.seat_no";

                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select(strSQL);

                foreach (DataRow dr in dt.Rows)
                {
                    StudentScore ss = new StudentScore();
                    ss.ClassID = dr["classid"].ToString();
                    ss.ClassName = dr["classname"].ToString();
                    ss.Name = dr["studentname"].ToString();
                    ss.StudentID = dr["studentid"].ToString();
                    ss.SeatNo = dr["seat_no"].ToString();

                    if (!retVal.ContainsKey(ss.ClassName))
                        retVal.Add(ss.ClassName, new Dictionary<string, StudentScore>());

                    if (!retVal[ss.ClassName].ContainsKey(ss.StudentID))
                        retVal[ss.ClassName].Add(ss.StudentID, ss);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 匯出 Excel
        /// </summary>
        /// <param name="inputReportName"></param>
        /// <param name="inputXls"></param>
        public static void CompletedXls(string inputReportName, Workbook inputXls)
        {
            string reportName = inputReportName;

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xls");

            Workbook wb = inputXls;

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                wb.Save(path, SaveFormat.Excel97To2003);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, SaveFormat.Excel97To2003);

                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}
