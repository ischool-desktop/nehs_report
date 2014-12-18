using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace ClassStudentScoreReport_P.DAO
{
    [FISCA.UDT.TableName("ischool.elementaryabsence")]
    public class AbsenceObj:ActiveRecord
    {
        [FISCA.UDT.Field(Field = "ref_student_id")]
        public int RefStudentId { get; set; }

        [FISCA.UDT.Field(Field = "school_year")]
        public int SchoolYear { get; set; }

        [FISCA.UDT.Field(Field = "semester")]
        public int Semester { get; set; }

        [FISCA.UDT.Field(Field = "personal_days")]
        public int? PersonalDays { get; set; }

        [FISCA.UDT.Field(Field = "sick_days")]
        public int? SickDays { get; set; }
    }
}
