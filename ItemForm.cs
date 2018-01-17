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
    public partial class ItemForm : Form
    {
        public ItemForm(CategoryDataSet dsCategory)
        {
            InitializeComponent();
            categoryDataSet.Merge(dsCategory);
        }

        public ItemForm(CategoryDataSet dsCategory, DateTime nowDate, string category, string item, int money, string remarks)
        {
            InitializeComponent();
            categoryDataSet.Merge(dsCategory);
            monCalendar.SetDate(nowDate);
            cmbCategory.Text = category;
            textItem.Text = item;
            txtRemarks.Text = remarks;
            mtxtMoney.Text = money.ToString();
        }

        private ItemForm()
        {

        }

    }
}
