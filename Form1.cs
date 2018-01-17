﻿using System;
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
                    + drMoney.分類 + ","
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
                        int.Parse(strData[3]),
                        strData[4]
                     );
                }
                sr.Close();
            }
        }

        private void UpdateData()
        {
            int nowRow = dgv.CurrentRow.Index;
            DateTime oldDate = DateTime.Parse(dgv.Rows[nowRow].Cells[0].Value.ToString());
            string oldcategory = dgv.Rows[nowRow].Cells[1].Value.ToString();
            string olditem = dgv.Rows[nowRow].Cells[2].Value.ToString();
            int oldMoney = int.Parse(dgv.Rows[nowRow].Cells[3].Value.ToString());
            string oldremarks = dgv.Rows[nowRow].Cells[4].Value.ToString();

            ItemForm frmItem = new ItemForm(categoryDataSet1, oldDate, oldcategory, olditem, oldMoney, oldremarks);
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

    }
}
