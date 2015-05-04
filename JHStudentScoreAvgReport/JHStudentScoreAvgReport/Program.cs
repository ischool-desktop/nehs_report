using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace JHStudentScoreAvgReport
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            RibbonBarItem rbItem2 = MotherForm.RibbonBarItems["班級", "資料統計"];
            rbItem2["報表"]["成績相關報表"]["各育成績總表"].Enable = UserAcl.Current["JHClassStudentScoreAvgReport"].Executable;
            rbItem2["報表"]["成績相關報表"]["各育成績總表"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0)
                {
                    Forms.MainForm mf = new Forms.MainForm(K12.Presentation.NLDPanels.Class.SelectedSource);
                    mf.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選班級");
                    return;
                }

            };

            // 各育成績總表
            Catalog catalog1b = RoleAclSource.Instance["班級"]["功能按鈕"];
            catalog1b.Add(new RibbonFeature("JHClassStudentScoreAvgReport", "各育成績總表"));
        }
    }
}
