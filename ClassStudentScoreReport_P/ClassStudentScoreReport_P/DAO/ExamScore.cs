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

        /*
        <Extension>
            <Score>90</Score>
            <AssignmentScore>90</AssignmentScore>
            <Text/>
        </Extension>
         */

        /// <summary>
        /// 定期
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// 平時
        /// </summary>
        public decimal? AssignmentScore { get; set; }
    }
}
