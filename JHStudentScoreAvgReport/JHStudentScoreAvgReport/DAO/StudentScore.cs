using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHStudentScoreAvgReport.DAO
{
    /// <summary>
    /// 學生成績
    /// </summary>
    public class StudentScore
    {
        /// <summary>
        /// Class ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// Student ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// Class Name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Student Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set; }

        /// <summary>
        /// 各育成績
        /// </summary>
        Dictionary<string, decimal> EntryScoreDict = new Dictionary<string, decimal>();

        /// <summary>
        /// 新增分項成績
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        public void AddEnteryScore(string name, decimal score)
        {
            if (!EntryScoreDict.ContainsKey(name))
                EntryScoreDict.Add(name, score);
        }

        /// <summary>
        /// 取得分項成績
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public decimal? GetEntryScore(string name)
        {
            if (EntryScoreDict.ContainsKey(name))
                return EntryScoreDict[name];
            else
                return null;
        }

        /// <summary>
        /// 總分
        /// </summary>
        /// <returns></returns>
        public decimal GetSumScore()
        {
            decimal retVal = 0;
            foreach (string key in EntryScoreDict.Keys)
                retVal += EntryScoreDict[key];

            return retVal;
        }

        /// <summary>
        /// 平均
        /// </summary>
        /// <returns></returns>
        public decimal GetAvgScore()
        {
            decimal retVal = 0;
            decimal sum = 0;
            foreach (string key in EntryScoreDict.Keys)
                sum += EntryScoreDict[key];

            // 四捨五入至小數第1位
            if (EntryScoreDict.Count > 0)
                retVal = Math.Round((sum / EntryScoreDict.Count), 1, MidpointRounding.AwayFromZero);

            return retVal;
        }

        /// <summary>
        /// 取得排名
        /// </summary>
        /// <returns></returns>
        public int GetRank(string name, decimal score)
        {
            int r = 0;
            string key = name + ClassName;
            if (Cal._RanksTmp.ContainsKey(key))
            {
                r = Cal._RanksTmp[key].IndexOf(score) + 1;
            }

            return r;
        }

    }
}
