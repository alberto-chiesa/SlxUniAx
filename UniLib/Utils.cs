using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Gianos.UniLib
{
    static internal class Utils
    {
        /// <summary>
        /// Copied and Pasted from Sage.Platform.VirtualFileSystem.VFSQuery
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        internal static MemoryStream UnpackItemData(byte[] itemData, bool compressed)
        {
            if (itemData == null)
            {
                return null;
            }
            if (!compressed)
            {
                return new MemoryStream(itemData);
            }
            MemoryStream stream = new MemoryStream();
            using (MemoryStream stream2 = new MemoryStream(itemData))
            {
                using (System.IO.Compression.DeflateStream stream3 = new System.IO.Compression.DeflateStream(stream2, System.IO.Compression.CompressionMode.Decompress))
                {
                    int num;
                    byte[] buffer = new byte[0x100];
                    while ((num = stream3.Read(buffer, 0, 0x100)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                }
            }
            return stream;
        }

        /// <summary>
        /// Copied and Pasted from Sage.Platform.VirtualFileSystem.VFSQuery
        /// </summary>
        /// <param name="itemData">Data to pack</param>
        /// <param name="smart">Smart compresses</param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        internal static byte[] PackItemData(MemoryStream itemData, bool smart, ref bool compressed)
        {
            if (itemData == null)
            {
                return null;
            }
            byte[] buffer = itemData.ToArray();
            if (smart || compressed)
            {
                byte[] buffer2;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (DeflateStream stream2 = new DeflateStream(stream, CompressionMode.Compress))
                    {
                        stream2.Write(buffer, 0, buffer.Length);
                    }
                    buffer2 = stream.ToArray();
                }
                compressed = !smart || (buffer2.Length < buffer.Length);
                if (compressed)
                {
                    return buffer2;
                }
            }
            return buffer;
        }
    }
}
