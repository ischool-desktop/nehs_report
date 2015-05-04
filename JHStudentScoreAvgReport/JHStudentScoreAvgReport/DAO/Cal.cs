using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using JHSchool.Data;

namespace JHStudentScoreAvgReport.DAO
{
    /// <summary>
    /// 計算成績
    /// </summary>
    public class Cal
    {
        /// <summary>
        /// 計算來自分項成績平均
        /// </summary>
        /// <param name="EntryName"></param>        
        /// <param name="isBetter"></param>
        /// <returns></returns>
        public static decimal CalEntryAvgScore(string EntryName, List<JHSemesterScoreRecord> StudScoreList)
        {
            decimal retVal = 0;
            string key = EntryName;
            decimal sum = 0, count = 0;

            foreach (JHSemesterScoreRecord srec in StudScoreList)
            {
                // 只處理學業分項，來自學習領域
                if (EntryName == "學業")
                {
                    if(srec.LearnDomainScore.HasValue)
                    {
                        sum += srec.LearnDomainScore.Value;
                        count++;
                    }
                    
                }            
            }

            if (count > 0)
                retVal = Math.Round((sum / count), 1, MidpointRounding.AwayFromZero);

            return retVal;
        }

        /// <summary>
        /// 計算來自科目加權平均
        /// </summary>
        /// <param name="SubjList"></param>
        /// <param name="rowData"></param>
        /// <param name="isBetter"></param>
        /// <returns></returns>
        public static decimal CalSubjGroupAvgScore(List<string> SubjList, List<JHSemesterScoreRecord> StudScoreList)
        {
            decimal retVal = 0;
            decimal sum = 0, credit = 0;

            foreach (string SubjName in SubjList)
            {
                foreach (JHSemesterScoreRecord sRec in StudScoreList)
                {
                    if (sRec.Subjects.ContainsKey(SubjName))
                    {
                        K12.Data.SubjectScore sss = sRec.Subjects[SubjName];
                        if (sss.Score.HasValue && sss.Credit.HasValue)
                        {
                            sum += sss.Score.Value * sss.Credit.Value;
                            credit += sss.Credit.Value;
                        }                    
                    }                
                }            
            }

            if (credit > 0)
                retVal = Math.Round(sum / credit, 1, MidpointRounding.AwayFromZero);

            return retVal;
        }

        /// <summary>
        /// 排名暫存用
        /// </summary>
        public static Dictionary<string, List<decimal>> _RanksTmp = new Dictionary<string, List<decimal>>();
    }
}
