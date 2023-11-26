using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace txtCutter.Enums
{
    /// <summary>
    /// 分割模式
    /// </summary>
    public enum CutType
    {
        /// <summary>
        /// 基础模式
        /// 文件名+序号
        /// </summary>
        [Description("文件名+序号")]
        File_Index,

        /// <summary>
        /// 序号+文件模式
        /// </summary>
        Index_File,

        /// <summary>
        /// 文件+序号+其他
        /// </summary>
        File_Index_Else,

        /// <summary>
        /// 其他+序号
        /// </summary>
        Else_Index,

        /// <summary>
        /// 序号+其他
        /// </summary>
        Index_Else,
    }

    /// <summary>
    /// 分割模式追加功能
    /// </summary>
    public static class CutTypeAdd
    {
        /// <summary>
        /// 获取Enum项描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }


        /// <summary>
        /// 获取ToolTip提示
        /// </summary>
        /// <param name="cutType"></param>
        /// <returns></returns>
        public static string GetToolTip(this CutType cutType)
        {
            var ret = "";
            switch (cutType)
            {
                case CutType.Index_File:
                    ret = "(索引)_[文件名].txt";
                    break;
                case CutType.File_Index_Else:
                    ret = "[文件名]_(索引)_{备注内容}.txt";
                    break;
                case CutType.Else_Index:
                    ret = "{备注内容}_(索引).txt";
                    break;
                case CutType.Index_Else:
                    ret = "(索引)_{备注内容}.txt";
                    break;
                case CutType.File_Index:
                default:
                    ret = "[文件名]_(索引).txt";
                    break;
            }
            return ret;
        }
    }
}
