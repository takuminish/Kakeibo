using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kakeibo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            categoryDataSet1.DataTable1.AddDataTable1Row("給料", "入金");
            categoryDataSet1.DataTable1.AddDataTable1Row("食費", "出金");
            categoryDataSet1.DataTable1.AddDataTable1Row("雑費", "出金");
            categoryDataSet1.DataTable1.AddDataTable1Row("住居", "出金");
            MoneyRange();

        }

        private void AddData()
        {
            ItemForm frmItem = new ItemForm(categoryDataSet1);
            DialogResult drRet = frmItem.ShowDialog();

            if (drRet == DialogResult.OK)
            {
                moneyDataSet.DataTable1.AddDataTable1Row(
                    frmItem.monCalendar.SelectionRange.Start,
                    frmItem.cmbCategory.Text,
                    frmItem.txtmiddlecategory.Text,
                    frmItem.textItem.Text,
                    int.Parse(frmItem.mtxtMoney.Text),
                    frmItem.txtRemarks.Text
                    );
            }

        }

        private void 追加AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveData()
        {
            string path = "MoneyData.csv";
            string strData = "";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                path,
                false,
                System.Text.Encoding.Default
            );

            foreach (MoneyDataSet.DataTable1Row drMoney in moneyDataSet.DataTable1)
            {
                strData = drMoney.日付.ToShortDateString() + ","
                    + drMoney.大分類 + ","
                    + drMoney.中分類 + ","
                    + drMoney.品名 + ","
                    + drMoney.金額.ToString() + ","
                    +drMoney.備考 + ",";
                sw.WriteLine(strData);
            }
            sw.Close();

        }

        private void S(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void LoadData()
        {
            string path = "MoneyData.csv";
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] strData;
            string strLine;
            bool fileExists = System.IO.File.Exists(path);

            if (fileExists)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    path,
                    System.Text.Encoding.Default
                );

                while (sr.Peek() >= 0)
                {
                    strLine = sr.ReadLine();
                    strData = strLine.Split(delimiter);
                    moneyDataSet.DataTable1.AddDataTable1Row(
                        DateTime.Parse(strData[0]),
                        strData[1],
                        strData[2],
                        strData[3],
                        int.Parse(strData[4]),
                        strData[5]
                     );
                }
                sr.Close();
            }
        }

        private void UpdateData()
        {
            int nowRow = dgv.CurrentRow.Index;
            DateTime oldDate = DateTime.Parse(dgv.Rows[nowRow].Cells[0].Value.ToString());
            string oldBigcategory = dgv.Rows[nowRow].Cells[1].Value.ToString();
            string oldMuddlecategory = dgv.Rows[nowRow].Cells[2].Value.ToString();
            string olditem = dgv.Rows[nowRow].Cells[3].Value.ToString();
            int oldMoney = int.Parse(dgv.Rows[nowRow].Cells[4].Value.ToString());
            string oldremarks = dgv.Rows[nowRow].Cells[5].Value.ToString();

            ItemForm frmItem = new ItemForm(categoryDataSet1, oldDate, oldBigcategory, oldMuddlecategory, olditem, oldMoney, oldremarks);
            DialogResult drRet = frmItem.ShowDialog();

            if (drRet == DialogResult.OK)
            {
                dgv.Rows[nowRow].Cells[0].Value = frmItem.monCalendar.SelectionRange.Start;
                dgv.Rows[nowRow].Cells[1].Value = frmItem.cmbCategory.Text;
                dgv.Rows[nowRow].Cells[2].Value = frmItem.textItem.Text;
                dgv.Rows[nowRow].Cells[3].Value = int.Parse(frmItem.mtxtMoney.Text);
                dgv.Rows[nowRow].Cells[4].Value = frmItem.txtRemarks.Text;
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void 変更CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void DeleteData()
        {
            int nowRow = dgv.CurrentRow.Index;
            dgv.Rows.RemoveAt(nowRow);
        }

        private void 削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void ChartSet(int[] yValues,DateTime day)
        {
            foreach (MoneyDataSet.DataTable1Row drMoney in moneyDataSet.DataTable1)
            {
                if (drMoney.日付 == day)
                {
                    switch (drMoney.大分類)
                    {
                        case "食費": yValues[0] += drMoney.金額; break;
                        case "雑費": yValues[1] += drMoney.金額; break;
                        case "住居": yValues[2] += drMoney.金額; break;
                    }
                }
            }
        }

        private void DaySelect()
        {
            DateTime day = GetSelectDate();
            if (day != DateTime.MaxValue)
            {
                chart1.Series.Clear();
                chart1.Series.Add("大分類別");

                chart1.Series["大分類別"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                string[] xValues = new string[] { "食費", "雑費", "住居" };
                int[] yValues = new int[] { 0, 0, 0 };

                ChartSet(yValues, day);

                for (int i = 0; i < xValues.Length; i++)
                {
                    //グラフに追加するデータクラスを生成
                    System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                    dp.SetValueXY(xValues[i], yValues[i]);  //XとYの値を設定
                    dp.IsValueShownAsLabel = true;  //グラフに値を表示するように指定
                    chart1.Series["大分類別"].Points.Add(dp);   //グラフにデータ追加
                }
            }
        }

        private void dayselect_Click(object sender, EventArgs e)
        {
            DaySelect();
            
        }

        private DateTime GetSelectDate()
        {
            DaySelectForm frmDay = new DaySelectForm();
            DialogResult drRet = frmDay.ShowDialog();

            if (drRet == DialogResult.OK)
            {
                return frmDay.monCalendar.SelectionRange.Start;

            }

            return DateTime.MaxValue;

        }

        private void MoneyRange()
        {
            DateTime dtNow = DateTime.Now;
            int iMonth = dtNow.Month;

            chart1.Series.Clear();
            chart1.Series.Add("日付別");

            chart1.Series["日付別"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            string[] xValues = new string[DateTime.DaysInMonth(dtNow.Year, dtNow.Month)];
            int[] yValues = new int[DateTime.DaysInMonth(dtNow.Year, dtNow.Month)];

            Chartinit(xValues, yValues,dtNow.Year, dtNow.Month);
            ChartSet2(xValues, yValues, dtNow.Year, dtNow.Month);
            for (int i = 0; i < xValues.Length; i++)
            {
                //グラフに追加するデータクラスを生成
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                dp.SetValueXY(xValues[i], yValues[i]);  //XとYの値を設定
                dp.IsValueShownAsLabel = true;  //グラフに値を表示するように指定
                chart1.Series["日付別"].Points.Add(dp);   //グラフにデータ追加
            }
        }

        private void DayRangeButton_Click(object sender, EventArgs e)
        {
            MoneyRange();


        }

        private void Chartinit(string[] xValues, int[] yValues, int year, int month)
        {
            for (int i = 0; i < yValues.Length; i++) {
                xValues[i] = year + "." + month + "." + (i+1);
                yValues[i] = 0;
            }
        }

        private void ChartSet2(string[] xValues, int[] yValues, int year, int month)
        {
            foreach (MoneyDataSet.DataTable1Row drMoney in moneyDataSet.DataTable1)
            {
                if (drMoney.日付.Year == year && drMoney.日付.Month == month )
                {
                  yValues[(drMoney.日付.Day)-1] += drMoney.金額; 
                }
            }
        }


    }
}
