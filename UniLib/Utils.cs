using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    }
}
