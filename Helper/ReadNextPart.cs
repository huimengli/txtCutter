using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace txtCutter.Helper
{
    /// <summary>
    /// 从文件流中读取指定大小的部分
    /// 这个代码一直有问题,我打算用这块来保存这个
    /// </summary>
    public static class ReadNextPart
    {
        ///// <summary>
        ///// 判断给定的字节是否为字符的开始。此方法主要用于UTF-8编码的情况。
        ///// 在UTF-8中，一个字符可能由多个字节组成。ASCII字符（0-127）以及多字节序列的第一个字节都不会以二进制 10 开头。
        ///// 该方法用于确定是否可以在当前字节处分割字符串，而不破坏字符的完整性。
        ///// </summary>
        ///// <param name="b">要检查的字节。</param>
        ///// <returns>如果字节是字符的起始部分，则为true；否则为false。</returns>
        //private static bool IsCharStart(byte b)
        //{
        //    // 对于UTF-8，所有ASCII字符和多字节序列的开始字节都会满足此条件
        //    return (b & 0xC0) != 0x80;
        //}

        /// <summary>
        /// 判断给定的字节是否为字符的开始。此方法主要用于UTF-8编码的情况。
        /// 在UTF-8中，一个字符可能由多个字节组成。ASCII字符（0-127）以及多字节序列的第一个字节都不会以二进制 10 开头。
        /// 该方法用于确定是否可以在当前字节处分割字符串，而不破坏字符的完整性。
        /// </summary>
        /// <param name="b">要检查的字节。</param>
        /// <returns>如果字节是字符的起始部分，则为true；否则为false。</returns>
        private static bool IsCharStart(byte b)
        {
            // 对于UTF-8，所有ASCII字符和多字节序列的开始字节都会满足此条件
            // return (b & 0xC0) != 0x80;
            var ret = b < 128 || b > 191;
            return ret;
        }

#if DEBUG
        /// <summary>
        /// 从文件流中读取指定大小的数据，同时尽量保证不在字符的中间断开。
        /// 此方法读取大约 partSize 字节的数据。如果最后一个字节是多字节字符的一部分，
        /// 它会回退到这个字符的开始，以保证字符的完整性。这对于UTF-8编码的文本尤为重要，
        /// 因为UTF-8的字符可能由多个字节组成。
        /// </summary>
        /// <param name="stream">文件流对象，用于从文件中读取数据。</param>
        /// <param name="partSize">希望读取的数据大小（字节）。</param>
        /// <param name="encoding">文件的编码方式，用于确定如何解释字节数据。</param>
        /// <param name="actualBytesRead">输出参数，返回从文件中实际读取的字节总数。这有助于处理最后一个文件块，其大小可能小于 partSize。</param>
        /// <returns>字符串，包含从文件中读取的数据。</returns>
        public static string ReadNextPart1(FileStream stream, int partSize, Encoding encoding, out int actualBytesRead)
        {
            byte[] buffer = new byte[partSize];
            actualBytesRead = stream.Read(buffer, 0, buffer.Length);

            // 如果读取的字节数小于缓冲区大小，直接返回读取的内容
            if (actualBytesRead < buffer.Length)
            {
                return encoding.GetString(buffer, 0, actualBytesRead);
            }

            // 为多字节字符边界处理提供一些额外空间
            int lastByteIndex = actualBytesRead;
            if (!encoding.IsSingleByte)
            {
                while (lastByteIndex > 0 && !IsCharStart(buffer[lastByteIndex - 1]))
                {
                    lastByteIndex--;
                }

                // 确保不在字符中间断开
                if (lastByteIndex == 0)
                {
                    lastByteIndex = actualBytesRead; // 如果找不到安全的断点，保留整个缓冲区
                }
            }

            // 重新定位流位置
            stream.Position -= (actualBytesRead - lastByteIndex);

            return encoding.GetString(buffer, 0, lastByteIndex);
        }
        /// <summary>
        /// 从文件流中读取指定大小的数据，同时尽量保证不在字符的中间断开。
        /// 此方法读取大约 partSize 字节的数据。如果最后一个字节是多字节字符的一部分，
        /// 它会回退到这个字符的开始，以保证字符的完整性。这对于UTF-8编码的文本尤为重要，
        /// 因为UTF-8的字符可能由多个字节组成。
        /// </summary>
        /// <param name="stream">文件流对象，用于从文件中读取数据。</param>
        /// <param name="partSize">希望读取的数据大小（字节）。</param>
        /// <param name="encoding">文件的编码方式，用于确定如何解释字节数据。</param>
        /// <param name="actualBytesRead">输出参数，返回从文件中实际读取的字节总数。这有助于处理最后一个文件块，其大小可能小于 partSize。</param>
        /// <returns>字符串，包含从文件中读取的数据。</returns>
        public static string ReadNextPart2(FileStream stream, int partSize, Encoding encoding, out int actualBytesRead)
        {
            byte[] buffer = new byte[partSize];
            actualBytesRead = stream.Read(buffer, 0, buffer.Length);

            // 如果读取的字节数小于缓冲区大小，直接返回读取的内容
            if (actualBytesRead < buffer.Length)
            {
                return encoding.GetString(buffer, 0, actualBytesRead);
            }

            // 检查最后一个完整字符的边界
            int lastValidIndex = actualBytesRead;

            if (!encoding.IsSingleByte)
            {
                Decoder decoder = encoding.GetDecoder();
                char[] chars = new char[encoding.GetMaxCharCount(buffer.Length)];
                bool foundValidChar = false;
                int charCount = 0;

                // 尝试解码直到找到一个有效的字符边界
                for (int i = actualBytesRead; i > 0; i--)
                {
                    try
                    {
                        // 尝试解码当前缓冲区到 i 的长度
                        charCount = decoder.GetChars(buffer, 0, i, chars, 0);

                        // 如果解码成功且没有异常，我们找到了一个有效的字符边界
                        foundValidChar = true;
                        lastValidIndex = i;
                        break;
                    }
                    catch
                    {
                        // 捕获解码错误，继续向前寻找一个有效边界
                        continue;
                    }
                }

                // 如果没有找到一个有效的字符边界，返回到最初设定的 actualBytesRead
                if (!foundValidChar)
                {
                    lastValidIndex = actualBytesRead;
                }
            }

            // 重新定位流的位置，避免下一个部分重复读取多余的部分
            stream.Position -= (actualBytesRead - lastValidIndex);

            return encoding.GetString(buffer, 0, lastValidIndex);
        }
        /// <summary>
        /// 从文件流中读取指定大小的数据，同时尽量保证不在字符的中间断开。
        /// 此方法读取大约 partSize 字节的数据。如果最后一个字节是多字节字符的一部分，
        /// 它会回退到这个字符的开始，以保证字符的完整性。这对于UTF-8编码的文本尤为重要，
        /// 因为UTF-8的字符可能由多个字节组成。
        /// </summary>
        /// <param name="stream">文件流对象，用于从文件中读取数据。</param>
        /// <param name="partSize">希望读取的数据大小（字节）。</param>
        /// <param name="encoding">文件的编码方式，用于确定如何解释字节数据。</param>
        /// <param name="actualBytesRead">输出参数，返回从文件中实际读取的字节总数。这有助于处理最后一个文件块，其大小可能小于 partSize。</param>
        /// <returns>字符串，包含从文件中读取的数据。</returns>
        public static string ReadNextPart3(FileStream stream, int partSize, Encoding encoding, out int actualBytesRead)
        {
            byte[] buffer = new byte[partSize];
            actualBytesRead = stream.Read(buffer, 0, buffer.Length);

            // 如果读取的字节数小于缓冲区大小，直接返回读取的内容
            if (actualBytesRead < buffer.Length)
            {
                return encoding.GetString(buffer, 0, actualBytesRead);
            }

            // 使用正则表达式寻找合适的分割点（这里以换行符作为分割点，可以根据需要调整）
            string text = encoding.GetString(buffer, 0, actualBytesRead);

            // 定义正则表达式来匹配有效的分割位置，例如换行符或者特定的字符分割点
            string pattern = @"[\r\n]+"; // 示例：匹配换行符
            MatchCollection matches = Regex.Matches(text, pattern);

            int lastValidIndex = actualBytesRead;

            if (matches.Count > 0)
            {
                // 找到最后一个匹配的位置作为有效分割点
                lastValidIndex = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
            }

            // 如果没有找到合适的分割点，可以根据实际情况决定如何处理
            // 例如，可以使用部分读取或者返回实际读取的内容
            if (lastValidIndex == 0)
            {
                lastValidIndex = actualBytesRead;
            }

            // 重新定位流的位置，避免下一个部分重复读取多余的部分
            stream.Position -= (actualBytesRead - lastValidIndex);

            return text.Substring(0, lastValidIndex);
        }
        /// <summary>
        /// 从文件流中读取指定大小的数据，同时尽量保证不在字符的中间断开。
        /// 此方法读取大约 partSize 字节的数据。如果最后一个字节是多字节字符的一部分，
        /// 它会回退到这个字符的开始，以保证字符的完整性。这对于UTF-8编码的文本尤为重要，
        /// 因为UTF-8的字符可能由多个字节组成。
        /// </summary>
        /// <param name="stream">文件流对象，用于从文件中读取数据。</param>
        /// <param name="partSize">希望读取的数据大小（字节）。</param>
        /// <param name="encoding">文件的编码方式，用于确定如何解释字节数据。</param>
        /// <param name="actualBytesRead">输出参数，返回从文件中实际读取的字节总数。这有助于处理最后一个文件块，其大小可能小于 partSize。</param>
        /// <returns>字符串，包含从文件中读取的数据。</returns>
        public static string ReadNextPart4(FileStream stream, int partSize, Encoding encoding, out int actualBytesRead)
        {
            byte[] buffer = new byte[partSize];
            actualBytesRead = stream.Read(buffer, 0, buffer.Length);

            // 如果读取的字节数小于缓冲区大小，直接返回读取的内容
            if (actualBytesRead < buffer.Length)
            {
                return encoding.GetString(buffer, 0, actualBytesRead);
            }

            // 使用正则表达式寻找合适的分割点（这里以换行符作为分割点，可以根据需要调整）
            string text = encoding.GetString(buffer, 0, actualBytesRead);

            // 定义正则表达式来匹配有效的分割位置，例如换行符或者特定的字符分割点
            string pattern = "[\r\n]+"; // 示例：匹配换行符
            Match lastMatch = null;

            foreach (Match match in Regex.Matches(text, pattern))
            {
                lastMatch = match;
            }

            int lastValidIndex = actualBytesRead;

            if (lastMatch != null)
            {
                // 找到最后一个匹配的位置作为有效分割点
                lastValidIndex = lastMatch.Index + lastMatch.Length;
            }

            // 如果没有找到合适的分割点，可以根据实际情况决定如何处理
            if (lastValidIndex == 0)
            {
                lastValidIndex = actualBytesRead; // 如果找不到匹配的分割点，保留整个缓冲区
            }

            // 重新定位流的位置，避免下一个部分重复读取多余的部分
            stream.Position -= (actualBytesRead - lastValidIndex);

            // 确保我们只返回有效分割点之前的数据
            return text.Substring(0, lastValidIndex);
        } 
#endif

        /// <summary>
        /// 从文件流中读取指定大小的数据，同时尽量保证不在字符的中间断开。
        /// 此方法读取大约 partSize 字节的数据。如果最后一个字节是多字节字符的一部分，
        /// 它会回退到这个字符的开始，以保证字符的完整性。这对于UTF-8编码的文本尤为重要，
        /// 因为UTF-8的字符可能由多个字节组成。
        /// </summary>
        /// <param name="stream">文件流对象，用于从文件中读取数据。</param>
        /// <param name="partSize">希望读取的数据大小（字节）。</param>
        /// <param name="encoding">文件的编码方式，用于确定如何解释字节数据。</param>
        /// <param name="actualBytesRead">输出参数，返回从文件中实际读取的字节总数。这有助于处理最后一个文件块，其大小可能小于 partSize。</param>
        /// <returns>字符串，包含从文件中读取的数据。</returns>
        public static string ReadNextPart5(FileStream stream, int partSize, Encoding encoding, out int actualBytesRead)
        {
            byte[] buffer = new byte[partSize];
            actualBytesRead = stream.Read(buffer, 0, buffer.Length);

            // 如果读取字节小于缓冲区大小,直接返回读取的内容
            if (actualBytesRead < buffer.Length)
            {
                return encoding.GetString(buffer, 0, actualBytesRead);
            }

            // 为多字节字符边界处理提供一些额外的空间
            int lastByteIndex = actualBytesRead;
            if (!encoding.IsSingleByte)
            {
                byte b = buffer[lastByteIndex - 1];
                while (lastByteIndex > 0 && !IsCharStart(b))
                {
                    lastByteIndex--; 
                    b = buffer[lastByteIndex - 1];
                }
                lastByteIndex--;

                // 确保不在字符中间断开
                if (lastByteIndex <= 0)
                {
                    lastByteIndex = actualBytesRead; // 如果找不到安全的断点，保留整个缓冲区
                }
            }

            // 重新定位流位置
            stream.Position -= (actualBytesRead - lastByteIndex);

            return encoding.GetString(buffer, 0, lastByteIndex);
        }
    }
}
