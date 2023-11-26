using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using txtCutter.Heplper;
using txtCutter.Enums;

using SizeType = txtCutter.Enums.SizeType;

namespace txtCutter
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 选择文件
        /// </summary>
        private string fileName = "";

        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = Enum.GetValues(typeof(CutType));
            comboBox2.DataSource = Enum.GetValues(typeof(SizeType));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Item.OpenOnWindows("https://github.com/huimengli");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //读取文件
            Item.ChoiceTxtFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), textBox1);
            var file = new FileInfo(textBox1.Text);
            fileName = textBox1.Text;
            textBox2.Text = file.DirectoryName;
            label4.Text = file.Length.ToString();
            textBox3.Text = file.Name;

            //设置鼠标提示
            toolTip1.SetToolTip(textBox1, textBox1.Text);
            toolTip1.SetToolTip(textBox2, textBox2.Text);
            toolTip1.SetToolTip(label4, Item.FormatFileSize(file.Length));
            toolTip1.SetToolTip(textBox3, textBox3.Text);

            //设置示例
            SetShili();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Item.ChoiceFolder(textBox2, "选择输出文件夹", textBox2.Text);
            toolTip1.SetToolTip(textBox2, textBox2.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(((ComboBox)sender).SelectedItem.ToString());
            var select = (CutType)Enum.Parse(typeof(CutType), ((ComboBox)sender).SelectedItem.ToString());
            switch (select)
            {
                default:
                case CutType.File_Index:
                case CutType.Index_File:
                    textBox4.ReadOnly = true;
                    break;
                case CutType.File_Index_Else:
                case CutType.Else_Index:
                case CutType.Index_Else:
                    textBox4.ReadOnly = false;
                    break;
            }

            //设置示例
            SetShili();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //设置示例
            SetShili();
        }

        /// <summary>
        /// 设置示例
        /// </summary>
        private void SetShili()
        {
            var shili = "";
            var shiliIndex = Item.CoveringNumber(12, 3);
            switch ((CutType)Enum.Parse(typeof(CutType), comboBox1.SelectedItem.ToString()))
            {
                case CutType.Index_File:
                    shili = $"{shiliIndex}_{textBox3.Text}.txt";
                    break;
                case CutType.File_Index_Else:
                    shili = $"{textBox3.Text}_{shiliIndex}_{textBox4.Text}.txt";
                    break;
                case CutType.Else_Index:
                    shili = $"{textBox4.Text}_{shiliIndex}.txt";
                    break;
                case CutType.Index_Else:
                    shili = $"{shiliIndex}_{textBox4.Text}.txt";
                    break;
                case CutType.File_Index:
                default:
                    shili = $"{textBox3.Text}_{shiliIndex}.txt";
                    break;
            }
            textBox5.Text = shili;
        }
    }
}
