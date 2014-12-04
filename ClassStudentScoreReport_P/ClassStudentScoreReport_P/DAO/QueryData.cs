﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.Data;
using JHSchool.Data;
using System.Xml.Linq;

namespace ClassStudentScoreReport_P.DAO
{
    public class QueryData
    {
        /// <summary>
        /// 透過班級取得該班學生,班座排序 
        /// </summary>
        /// <param name="classIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetStudentIDListByClassID(List<string> classIDList)
        {
            Dictionary<string, List<string>> retVal = new Dictionary<string, List<string>>();
            if(classIDList.Count>0)
            {
                QueryHelper qh1 = new QueryHelper();
                string query1 = "select class.id as cid,student.id as sid from student inner join class on student.ref_class_id=class.id where student.status=1 and class.id in("+string.Join(",",classIDList.ToArray())+") order by class.class_name,seat_no;";
                DataTable dt1 = qh1.Select(query1);
                foreach (DataRow dr1 in dt1.Rows)
                {
                    string cid=dr1["cid"].ToString();
                    string sid=dr1["sid"].ToString();
                    if (!retVal.ContainsKey(cid))
                        retVal.Add(cid, new List<string>());

                    retVal[cid].Add(sid);
                }

            }
            return retVal;                
        }

        /// <summary>
        /// 透過學生 ID 取得
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<ExamScore>> GetStudentExamScoreDictByStudentIDList(List<string> StudentIDList,int SchoolYear,int Semester)
        {
            Dictionary<string, List<ExamScore>> retVal = new Dictionary<string, List<ExamScore>>();
            QueryHelper qh1 = new QueryHelper();
            string query1 = "select ref_student_id as sid,subject as subject_name,domain as domain_name,exam_name,sce_take.extension as extension from sc_attend inner join sce_take on sc_attend.id=sce_take.ref_sc_attend_id inner join exam on sce_take.ref_exam_id=exam.id inner join course on sc_attend.ref_course_id=course.id where ref_student_id in(" + string.Join(",", StudentIDList.ToArray()) + ") and course.school_year=" + SchoolYear + " and semester=" + Semester + " order by sid,domain_name,subject_name,exam_name";
            DataTable dt1 = qh1.Select(query1);
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr1 in dt1.Rows)
            {
                ExamScore es = new ExamScore();
                string sid = dr1["sid"].ToString();
                es.StudentID = sid;
                if (dr1["subject_name"] == null)
                    es.SubjectName = "";
                else
                    es.SubjectName = dr1["subject_name"].ToString();

                if (dr1["domain_name"] == null)
                    es.DomainName = "";
                else
                    es.DomainName = dr1["domain_name"].ToString();

                es.ExamName = dr1["exam_name"].ToString();

                string extension = "";
                if (dr1["extension"] != null)
                {
                    sb.Clear();
                    sb.Append("<root>");
                    extension = dr1["extension"].ToString();
                    sb.Append(extension);
                    sb.Append("</root>");
                    try
                    {
                        XElement elmRoot = XElement.Parse(sb.ToString());
                        foreach (XElement elm in elmRoot.Elements("Extension"))
                        {
                            if (elm.Element("Score") != null)
                            {
                                decimal ss;
                                if (decimal.TryParse(elm.Element("Score").Value, out ss))
                                    es.Score = ss;                                
                            }

                            if (elm.Element("AssignmentScore") != null)
                            {
                                decimal ass;
                                if (decimal.TryParse(elm.Element("AssignmentScore").Value, out ass))
                                    es.AssignmentScore = ass;
                            }
                        }
                    }
                    catch (Exception ex)
                    {  }
                }

                if (!retVal.ContainsKey(sid))
                    retVal.Add(sid, new List<ExamScore>());

                retVal[sid].Add(es);
            }

            return retVal;                 
        }
    }
}
