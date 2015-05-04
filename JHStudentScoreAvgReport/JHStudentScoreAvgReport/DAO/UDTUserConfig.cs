using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JHStudentScoreAvgReport.DAO
{
      [TableName("jh.shstudentscoreavgreport.userconfig")]
    public class UDTUserConfig:ActiveRecord
    {
        ///<summary>
        /// 智育對應項目
        ///</summary>
        [Field(Field = "score1_text", Indexed = false)]
        public string Score1Text { get; set; }

        ///<summary>
        /// 體育對應項目
        ///</summary>
        [Field(Field = "score2_text", Indexed = false)]
        public string Score2Text { get; set; }

        ///<summary>
        /// 美育對應項目
        ///</summary>
        [Field(Field = "score3_text", Indexed = false)]
        public string Score3Text { get; set; }
    }
}
