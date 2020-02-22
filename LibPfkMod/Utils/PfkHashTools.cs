namespace PfkHashUtils
{
    using System;
    using System.Text;
    using Force.Crc32;

    public class PfkHashTools
    {
        private const string Salt = "PFK";
        private const string Alphabet = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>
        /// キーからCRC32を返す。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>CRC32</returns>
        public static long ComputeFileID(Guid key)
        {
            var result = ComputeHashInt(key.ToString());

            return result;
        }

        /// <summary>
        /// キーからReferenceIDを算出する。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>ReferenceID</returns>
        public static long ComputeReferenceID(Guid key)
        {
            var result = ComputeHashInt(key.ToString());

            return result;
        }

        /// <summary>
        /// CRC32のハッシュからからハッシュテキストを返す。
        /// </summary>
        /// <param name="hash">CRC32</param>
        /// <returns>ハッシュテキスト</returns>
        public static string ComputeHashIds(long hash)
        {
            //// hashidsでhashを算出する。
            HashidsNet.Hashids hashids = new HashidsNet.Hashids(Salt, 0, Alphabet);

            var result = hashids.EncodeLong(hash);

            return result;
        }

        /// <summary>
        /// テキストからHashを返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>Hash</returns>
        public static string ComputeHashX(string text)
        {
            string result = string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(text.Replace("-", string.Empty));
            //// テキストのCRC32を計算する。
            long crc = Crc32Algorithm.Compute(bytes);

            //// Encodeの入力は正の整数が必要なため絶対値を取る。
            crc = Math.Abs(crc);

            //// hashidsでhashを算出する。
            HashidsNet.Hashids hashids = new HashidsNet.Hashids(Salt, 0, Alphabet);

            result = hashids.EncodeLong(crc);

            return result;
        }

        private static long ComputeHashInt(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            //// テキストのCRC32を計算する。
            long crc = Crc32Algorithm.Compute(bytes);

            return crc;
        }
    }
}
