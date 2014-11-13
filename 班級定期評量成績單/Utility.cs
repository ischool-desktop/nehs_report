using FISCA.DSAUtil;
using SmartSchool.Customization.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ClassExamReport_nehs
{
    public class Utility
    {
        /// <summary>
        /// 取得學生及格與補考標準，參數用學生IDList,回傳:key:StudentID,1_及,數字
        /// </summary>
        /// <param name="StudRecList"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, decimal>> GetStudentApplyLimitDict(List<SmartSchool.Customization.Data.StudentRecord> StudRecList)
        {

            Dictionary<string, Dictionary<string, decimal>> retVal = new Dictionary<string, Dictionary<string, decimal>>();


            foreach (SmartSchool.Customization.Data.StudentRecord studRec in StudRecList)
            {
                //及格標準<年級,及格與補考標準>
                if (!retVal.ContainsKey(studRec.StudentID))
                    retVal.Add(studRec.StudentID, new Dictionary<string, decimal>());

                XmlElement scoreCalcRule = SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studRec.StudentID) == null ? null : SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studRec.StudentID).ScoreCalcRuleElement;
                if (scoreCalcRule == null)
                {

                }
                else
                {

                    DSXmlHelper helper = new DSXmlHelper(scoreCalcRule);
                    decimal tryParseDecimal;
                    decimal tryParseDecimala;

                    foreach (XmlElement element in helper.GetElements("及格標準/學生類別"))
                    {
                        string cat = element.GetAttribute("類別");
                        bool useful = false;
                        //掃描學生的類別作比對
                        foreach (CategoryInfo catinfo in studRec.StudentCategorys)
                        {
                            if (catinfo.Name == cat || catinfo.FullName == cat)
                                useful = true;
                        }
                        //學生是指定的類別或類別為"預設"
                        if (cat == "預設" || useful)
                        {
                            for (int gyear = 1; gyear <= 4; gyear++)
                            {
                                switch (gyear)
                                {
                                    case 1:
                                        if (decimal.TryParse(element.GetAttribute("一年級及格標準"), out tryParseDecimal))
                                        {
                                            string k1s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k1s))
                                                retVal[studRec.StudentID].Add(k1s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k1s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k1s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("一年級補考標準"), out tryParseDecimala))
                                        {
                                            string k1a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k1a))
                                                retVal[studRec.StudentID].Add(k1a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k1a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k1a] = tryParseDecimala;
                                        }

                                        break;
                                    case 2:
                                        if (decimal.TryParse(element.GetAttribute("二年級及格標準"), out tryParseDecimal))
                                        {
                                            string k2s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k2s))
                                                retVal[studRec.StudentID].Add(k2s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k2s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k2s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("二年級補考標準"), out tryParseDecimala))
                                        {
                                            string k2a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k2a))
                                                retVal[studRec.StudentID].Add(k2a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k2a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k2a] = tryParseDecimala;

                                        }

                                        break;
                                    case 3:
                                        if (decimal.TryParse(element.GetAttribute("三年級及格標準"), out tryParseDecimal))
                                        {
                                            string k3s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k3s))
                                                retVal[studRec.StudentID].Add(k3s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k3s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k3s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("三年級補考標準"), out tryParseDecimala))
                                        {
                                            string k3a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k3a))
                                                retVal[studRec.StudentID].Add(k3a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k3a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k3a] = tryParseDecimala;
                                        }

                                        break;
                                    case 4:
                                        if (decimal.TryParse(element.GetAttribute("四年級及格標準"), out tryParseDecimal))
                                        {
                                            string k4s = gyear + "_及";

                                            if (!retVal[studRec.StudentID].ContainsKey(k4s))
                                                retVal[studRec.StudentID].Add(k4s, tryParseDecimal);

                                            if (retVal[studRec.StudentID][k4s] > tryParseDecimal)
                                                retVal[studRec.StudentID][k4s] = tryParseDecimal;
                                        }

                                        if (decimal.TryParse(element.GetAttribute("四年級補考標準"), out tryParseDecimala))
                                        {
                                            string k4a = gyear + "_補";

                                            if (!retVal[studRec.StudentID].ContainsKey(k4a))
                                                retVal[studRec.StudentID].Add(k4a, tryParseDecimala);

                                            if (retVal[studRec.StudentID][k4a] > tryParseDecimala)
                                                retVal[studRec.StudentID][k4a] = tryParseDecimala;
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }


        /// <summary>
        /// 暫存學生及格標準
        /// </summary>
        public static Dictionary<string, Dictionary<string, decimal>> tmpStudentApplyLimitDict = new Dictionary<string, Dictionary<string, decimal>>();
    }
}
