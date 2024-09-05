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
using txtCutter.Properties;

using SizeType = txtCutter.Enums.SizeType;
using txtCutter.Helper;

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

            // 设置ICON
            Icon = Resources.favicon;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Item.OpenOnWindows("https://github.com/huimengli");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //读取文件
            Item.ChoiceTxtFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), textBox1);
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                return;
            }

            SetInputFile(textBox1.Text);
        }

        /// <summary>
        /// 设置输入文件
        /// </summary>
        /// <param name="input"></param>
        private void SetInputFile(string input)
        {
            textBox1.Text = input;
            var file = new FileInfo(textBox1.Text);
            fileName = textBox1.Text;
            textBox2.Text = file.DirectoryName;
            textBox3.Text = file.NameWithoutExtension();

            //设置鼠标提示
            toolTip1.SetToolTip(textBox1, textBox1.Text);
            toolTip1.SetToolTip(textBox2, textBox2.Text);
            toolTip1.SetToolTip(label4, Item.FormatFileSize(file.Length));
            toolTip1.SetToolTip(textBox3, textBox3.Text);

            //设置示例
            SetShili();

            //设置文件显示大小
            SetFileSize((SizeType)comboBox2.SelectedItem);

            //设置分割数据
            SetPart(int.Parse(textBox6.Text));
            SetNumber(int.Parse(textBox7.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Item.ChoiceFolder(textBox2, "选择输出文件夹", textBox2.Text);
            Item.ChoiceFolder(textBox2, "选择输出文件夹", textBox2.Text, true);
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

        /// <summary>
        /// 设置文件大小
        /// </summary>
        /// <param name="sizeType"></param>
        private void SetFileSize(SizeType sizeType)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                return;
            }
            var file = new FileInfo(textBox1.Text);
            if (file.Exists)
            {
                label4.Text = Item.FormatFileSize(file.Length, sizeType);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sizeType = (SizeType)Enum.Parse(typeof(SizeType), ((ComboBox)sender).SelectedItem.ToString());
            SetFileSize(sizeType);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();       //获得路径

            SetInputFile(path);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 设置总份数
        /// </summary>
        /// <param name="eachPartSize">每份大小(单位:KB)</param>
        private void SetPart(int eachPartSize)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                return;
            }
            else
            {
                var file = new FileInfo(textBox1.Text);
                var partNumber = Math.Ceiling((double)file.Length / eachPartSize/1024);
                textBox9.Text = partNumber.ToString();
            }
        }

        /// <summary>
        /// 设置每份大小
        /// </summary>
        /// <param name="partNumber">分割数量</param>
        private void SetNumber(int partNumber)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                return;
            }
            else
            {
                var file = new FileInfo(textBox1.Text);
                var partEachSize = (long)Math.Ceiling((double)file.Length / partNumber);
                textBox8.Text = Item.FormatFileSize(partEachSize);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = false;
            textBox7.ReadOnly = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = false;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                textBox6.Text = "1";
                SetPart(1);
            }
            SetPart(int.Parse(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                textBox7.Text = "1";
                SetNumber(1);
            }
            SetNumber(int.Parse(textBox7.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("没有选择文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var file = new FileInfo(textBox1.Text);
            var cutType = (CutType)Enum.Parse(typeof(CutType), comboBox1.Text);

            int partSize;
            if (radioButton1.Checked)
            {
                partSize = int.Parse(textBox6.Text) * 1024; // 每部分的大小 (字节)
            }
            else if (radioButton2.Checked)
            {
                partSize = (int)Math.Ceiling((double)file.Length / int.Parse(textBox7.Text));
            }
            else
            {
                MessageBox.Show("错误的分割模式!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var stream = File.OpenRead(textBox1.Text))
            {
                int fileIndex = 0;
                while (stream.Position < stream.Length)
                {
                    fileIndex++;
                    var content = ReadNextPart.ReadNextPart5(stream, partSize, Encoding.UTF8, out int actualBytesRead);
                    var newFileName = cutType.GetFileName(textBox3.Text, fileIndex, textBox4.Text);

                    // 只写入实际读取的字节数
                    File.WriteAllText(Path.Combine(textBox2.Text, newFileName), content, Encoding.UTF8);
                }
            }

            MessageBox.Show("分割完成!", "", MessageBoxButtons.OK);
        }
    }
}
