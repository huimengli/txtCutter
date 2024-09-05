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
        /// 提示字典
        /// </summary>
        private static readonly Dictionary<CutType, string> ToolTipDict = new Dictionary<CutType, string>()
        {
            { CutType.Index_File, "(索引)_[文件名].txt" },
            { CutType.File_Index_Else, "[文件名]_(索引)_{备注内容}.txt" },
            { CutType.Else_Index, "{备注内容}_(索引).txt" },
            { CutType.Index_Else, "(索引)_{备注内容}.txt" },
            { CutType.File_Index, "[文件名]_(索引).txt" }
        };

        /// <summary>
        /// 获取Enum项描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns>描述字符串</returns>
        public static string GetEnumDescription<TEnum>(this TEnum value) where TEnum : Enum
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
            return ToolTipDict.TryGetValue(cutType, out var ret) ? ret : "[文件名]_(索引).txt";
        }

        /// <summary>
        /// 根据分割模式和输入内容来获取示例名
        /// </summary>
        /// <param name="cutType"></param>
        /// <param name="fileName">无后缀名的文件名</param>
        /// <param name="index">索引</param>
        /// <param name="elseValue">其他内容</param>
        /// <param name="civering">补位,假设补位为4,index为12,则自动补位成0012</param>
        /// <returns></returns>
        public static string GetFileName(this CutType cutType, string fileName, int index, string elseValue = "", int civering = 0)
        {
            var indexStr = civering > 0 ? index.ToString().PadLeft(civering, '0') : index.ToString();
            var sb = new StringBuilder();

            switch (cutType)
            {
                case CutType.Index_File:
                    sb.AppendFormat("{0}_{1}.txt", indexStr, fileName);
                    break;
                case CutType.File_Index_Else:
                    sb.AppendFormat("{0}_{1}_{2}.txt", fileName, indexStr, elseValue);
                    break;
                case CutType.Else_Index:
                    sb.AppendFormat("{0}_{1}.txt", elseValue, indexStr);
                    break;
                case CutType.Index_Else:
                    sb.AppendFormat("{0}_{1}.txt", indexStr, elseValue);
                    break;
                case CutType.File_Index:
                default:
                    sb.AppendFormat("{0}_{1}.txt", fileName, indexStr);
                    break;
            }

            return sb.ToString();
        }
    }
}