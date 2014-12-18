using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassStudentScoreReport_P.DAO
{
    /// <summary>
    ///  單一評量成績
    /// </summary>
    public class ExamScore
    {
        public string StudentID { get; set; }

        public string ExamName { get; set; }

        public string SubjectName { get; set; }

        public string DomainName { get; set; }

        private decimal? _Score { get; set; }

        private decimal? _AssignmentScore { get; set; }
        /*
        <Extension>
            <Score>90</Score>
            <AssignmentScore>90</AssignmentScore>
            <Text/>
        </Extension>
         */

        /// <summary>
        /// 定期評量比例
        /// </summary>
        private decimal scoreP = 50;

        public void SetScore(decimal? score)
        {
            _Score = score;        
        }

        public void SetAssignmentScore(decimal? assignmentScore)
        {
            _AssignmentScore = assignmentScore;
        }

        /// <summary>
        /// 設定定期評量比例
        /// </summary>
        /// <param name="dd"></param>
        public void SetScoreP(decimal dd)
        {
            scoreP=dd;
        }

        /// <summary>
        /// 取得定期評量先四捨五入到整數
        /// </summary>
        /// <returns></returns>
        public decimal? GetScore()
        {
            if (_Score.HasValue)
                return Math.Round(_Score.Value, 0);
            else
                return null;
        }

        public decimal? GetAssignmentScore()
        {
            if (_AssignmentScore.HasValue)
                return Math.Round(_AssignmentScore.Value, 0);
            else
                return null;
        }

        public decimal? GetAverage()
        {
            if (GetAssignmentScore().HasValue == false && GetScore().HasValue == false)
            {
                return null;
            }
            else
            {
                // 只有定期，沒有平時，定期就是平均
                if (GetAssignmentScore().HasValue == false && GetScore().HasValue)
                    return GetScore().Value;
                
                // 只有平時，沒有定期，平時就是平均
                if (GetAssignmentScore().HasValue && GetScore().HasValue==false)
                    return GetAssignmentScore().Value;

                // 透過比例計算後，四捨五入到小數下一位
                return Math.Round((scoreP * GetScore().Value + (100 - scoreP) * GetAssignmentScore().Value) * 0.01M,1);
            }               
        }
      
        /// <summary>
        /// 課程編號
        /// </summary>
        public string CourseID { get; set; }

    }
}
