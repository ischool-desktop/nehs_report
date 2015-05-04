using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHStudentScoreAvgReport
{
    public class Global
    {
        /// <summary>
        /// 智育,true:來自科目,false 讀取分項
        /// </summary>
        public static bool Score1isSubjGroup = false;

        /// <summary>
        /// 智育,true 擇優, false 原始
        /// </summary>
        public static bool Score1isMax = true;

        /// <summary>
        /// 體育,true:來自科目,false 讀取分項
        /// </summary>
        public static bool Score2isSubjGroup = false;

        /// <summary>
        /// 體育,true 擇優, false 原始
        /// </summary>
        public static bool Score2isMax = true;

        /// <summary>
        /// 美育,true:來自科目,false 讀取分項
        /// </summary>
        public static bool Score3isSubjGroup = false;

        /// <summary>
        /// 美育,true 擇優, false 原始
        /// </summary>
        public static bool Score3isMax = true;

        /// <summary>
        /// 班排名使用
        /// </summary>
        public static Dictionary<string, List<decimal>> ClassRankDict = new Dictionary<string, List<decimal>>();
    }
}
