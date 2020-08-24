using System;
using System.Collections.Generic;
using System.Text;

namespace aria
{
    class Base64
    {
        private static string part_encode(string subPlain)
        {
            List<char> base64_table = new List<char>
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
                'w', 'x', 'y', 'z', '0', '1', '2', '3',
                '4', '5', '6', '7', '8', '9', '+', '/',
            };
            int shift = 2;
            char carry = (char)0;
            bool ignore_flag = false;
            string crypted = null;

            for (uint index = 0; index < subPlain.Length; ++index)
            {
                if (ignore_flag)
                    crypted += '=';
                else
                {
                    char cur = (char)(subPlain[(int)index] >> shift | carry);
                    if (subPlain[(int)index] == 0)
                        ignore_flag = true;
                    carry = (char)((subPlain[(int)index] << (6 - shift)) & 0x3f);
                    shift += 2;
                    crypted += base64_table[cur];
                }
            }
            if (subPlain[subPlain.Length - 1] == 0)
                crypted += '=';
            else
            {
                char cur = (char)(subPlain[subPlain.Length - 1] & 0x3f);
                crypted += base64_table[cur];
            }
            return crypted;
        }

        private static string part_decode(string subCrypted)
        {
            int shift = 2;
            string plain=null;

            for (int index = 0; index < subCrypted.Length - 1; ++index)
            {
                
                if (subCrypted[index] == '=') break;
                char cur = (char)(getValue(subCrypted[index]) << shift);
                char carry = (char)(getValue(subCrypted[index+1]) >> (6 - shift));
                shift += 2;
                plain += cur | carry;
            }

            return plain;
        }

        private static char getValue(char ch)
        {
            char retch;

            if (ch >= 'A' && ch <= 'Z')
            {
                retch = (char)(ch - 'A');
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                retch = (char)(ch - 'a' + 26);
            }
            else if (ch >= '0' && ch <= '9')
            {
                retch = (char)(ch - '0' + 52);
            }
            else if (ch == '+')
            {
                retch = (char)62;
            }
            else if (ch == '/')
            {
                retch = (char)63;
            }
            else
            {
                retch = (char)0;
            }
            return retch;
        }

        public static string encode(string plain)
        {
            int remainder = plain.Length % 3;
            if (remainder != 0) remainder = 3 - remainder;
            for (int i = 0; i < remainder; ++i)
                plain += (char)0;
            string crypted = null;
            int start_pos = 0;
            for (uint index = 0; plain.Length > index; index += 3)
            {
                string subplain = plain.Substring(start_pos, 3);
                string subcrypted = part_encode(subplain);
                start_pos += 3;
                crypted += subcrypted;
            }
            return crypted;
        }

        public static string decode(string crypted)
        {
            string plain=null;
            int sIndex = 0;
            for (int index = 0; crypted.Length > (uint) index; index += 4) {
                string subCrypted = crypted.Substring(sIndex, 4);
                string subPlain = part_decode(subCrypted);
                sIndex += 4;
                plain += subPlain;
            }
            return plain;
        }
    }
}
