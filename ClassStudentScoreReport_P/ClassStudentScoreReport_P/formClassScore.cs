using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClassStudentScoreReport_P
{
    public partial class formClassScore : FISCA.Presentation.Controls.BaseForm
    {
        List<string> _ClassIDList;

        BackgroundWorker _bgLoadData;

        public formClassScore(List<string> ClassIDList)
        {
            InitializeComponent();
            _ClassIDList = ClassIDList;
            _bgLoadData = new BackgroundWorker();
            _bgLoadData.DoWork += _bgLoadData_DoWork;
            _bgLoadData.RunWorkerCompleted += _bgLoadData_RunWorkerCompleted;
            _bgLoadData.WorkerReportsProgress = true;
            _bgLoadData.ProgressChanged += _bgLoadData_ProgressChanged;
        }

        void _bgLoadData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        void _bgLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void _bgLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            // 讀取學生資料

            // 讀取成績資料
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void formClassScore_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;

            // 畫面預設値
            int sy,ss;
            if(int.TryParse(K12.Data.School.DefaultSchoolYear,out sy))
                iptSchoolYear.Value = sy;

            if (int.TryParse(K12.Data.School.DefaultSemester, out ss))
                iptSemester.Value = ss;

        }
    }
}
