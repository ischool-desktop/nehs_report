using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using K12.Data;

namespace ClassStudentScoreReport_P.DAO
{
    public class StudentScore
    {
        public string StudentID { get; set; }

        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string StudentName { get; set; }

        public string StudentNumber { get; set; }
        public int? SeatNo { get; set; }

        List<JHExamRecord> _ExamList = new List<JHExamRecord>();

        JHSemesterScore SemesterScore = new JHSemesterScore();

        
    }
}
