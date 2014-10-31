using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassStudentScoreReport_P
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            RibbonBarItem rbItem2 = MotherForm.RibbonBarItems["班級", "資料統計"];
            rbItem2["報表"]["成績相關報表"]["學生成績總表"].Enable = UserAcl.Current["ClassStudentScoreReport_P.formClassScore"].Executable;
            rbItem2["報表"]["成績相關報表"]["學生成績總表"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0)
                {
                    formClassScore fcs = new formClassScore(K12.Presentation.NLDPanels.Class.SelectedSource);
                    fcs.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選班級");
                    return;
                }

            };

            // 學生成績總表
            Catalog catalog1b = RoleAclSource.Instance["班級"]["功能按鈕"];
            catalog1b.Add(new RibbonFeature("ClassStudentScoreReport_P.formClassScore", "學生成績總表"));        
        }
    }
}
