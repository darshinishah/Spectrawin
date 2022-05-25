using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softlock.Models;

namespace Softlock.App_Code
{
    public class EncodeLicense
    {

        #region Private

        #region Declaration
        struct IMMENSE
        {
            public long l;
            public long r;
        };

        struct GREAT
        {
            public long l;
            public long r;
            public long c;
        };

        public const int SETNEWKEY = 1;
        public const int USECURKEY = 0;
        public const int DECRYPT = 1;
        public const int ENCRYPT = 0;

        public const long DES_MAGIC1 = 0x690F0000L;
        public const long DES_MAGIC2 = 0x9A260000L;
        public const long DES_MAGIC3 = 0x88D50000L;

        public const long DES_MAGIC7 = 0x7CA11045L;
        public const long DES_MAGIC8 = 0x4A1A6E57L;

        public long[] DESBit = new long[33];
        private int initflag = 1;
        private byte[] str = new byte[9];

        private readonly byte[] ipc1 = { 0, 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };

        private readonly byte[] ipc2 = { 0, 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };

        private readonly byte[] iet = { 0, 32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1 };

        private readonly byte[] ipp = { 0, 16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25 };

        private readonly byte[] ip = { 0, 58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7 };

        private readonly byte[] ipm = { 0, 40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };

        private byte[] ibin = { 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 };

        private GREAT[] kns = new GREAT[17];

        private readonly byte[][][] isArr =
        {
    new byte[][]
    {
        new byte[] {0, 14, 15, 10, 7, 2, 12, 4, 13},
        new byte[] {0, 0, 3, 13, 13, 14, 10, 13, 1},
        new byte[] {0, 4, 0, 13, 10, 4, 9, 1, 7},
        new byte[] {0, 15, 13, 1, 3, 11, 4, 6, 2}
    },
    new byte[][]
    {
        new byte[] {0, 4, 1, 0, 13, 12, 1, 11, 2},
        new byte[] {0, 15, 13, 7, 8, 11, 15, 0, 15},
        new byte[] {0, 1, 14, 6, 6, 2, 14, 4, 11},
        new byte[] {0, 12, 8, 10, 15, 8, 3, 11, 1}
    },
    new byte[][]
    {
        new byte[] {0, 13, 8, 9, 14, 4, 10, 2, 8},
        new byte[] {0, 7, 4, 0, 11, 2, 4, 11, 13},
        new byte[] {0, 14, 7, 4, 9, 1, 15, 11, 4},
        new byte[] {0, 8, 10, 13, 0, 12, 2, 13, 14}
    },
    new byte[][]
    {
        new byte[] {0, 1, 14, 14, 3, 1, 15, 14, 4},
        new byte[] {0, 4, 7, 9, 5, 12, 2, 7, 8},
        new byte[] {0, 8, 11, 9, 0, 11, 5, 13, 1},
        new byte[] {0, 2, 1, 0, 6, 7, 12, 8, 7}
    },
    new byte[][]
    {
        new byte[] {0, 2, 6, 6, 0, 7, 9, 15, 6},
        new byte[] {0, 14, 15, 3, 6, 4, 7, 4, 10},
        new byte[] {0, 13, 10, 8, 12, 10, 2, 12, 9},
        new byte[] {0, 4, 3, 6, 10, 1, 9, 1, 4}
    },
    new byte[][]
    {
        new byte[] {0, 15, 11, 3, 6, 10, 2, 0, 15},
        new byte[] {0, 2, 2, 4, 15, 7, 12, 9, 3},
        new byte[] {0, 6, 4, 15, 11, 13, 8, 3, 12},
        new byte[] {0, 9, 15, 9, 1, 14, 5, 4, 10}
    },
    new byte[][]
    {
        new byte[] {0, 11, 3, 15, 9, 11, 6, 8, 11},
        new byte[] {0, 13, 8, 6, 0, 13, 9, 1, 7},
        new byte[] {0, 2, 13, 3, 7, 7, 12, 7, 14},
        new byte[] {0, 1, 4, 8, 13, 2, 15, 10, 8}
    },
    new byte[][]
    {
        new byte[] {0, 8, 4, 5, 10, 6, 8, 13, 1},
        new byte[] {0, 1, 14, 10, 3, 1, 5, 10, 4},
        new byte[] {0, 11, 1, 0, 13, 8, 3, 14, 2},
        new byte[] {0, 7, 2, 7, 8, 13, 10, 7, 13}
    },
    new byte[][]
    {
        new byte[] {0, 3, 9, 1, 1, 8, 0, 3, 10},
        new byte[] {0, 10, 12, 2, 4, 5, 6, 14, 12},
        new byte[] {0, 15, 5, 11, 15, 15, 7, 10, 0},
        new byte[] {0, 5, 11, 4, 9, 6, 11, 9, 15}
    },
    new byte[][]
    {
        new byte[] {0, 10, 7, 13, 2, 5, 13, 12, 9},
        new byte[] {0, 6, 0, 8, 7, 0, 1, 3, 5},
        new byte[] {0, 12, 8, 1, 1, 9, 0, 15, 6},
        new byte[] {0, 11, 6, 15, 4, 15, 14, 5, 12}
    },
    new byte[][]
    {
        new byte[] {0, 6, 2, 12, 8, 3, 3, 9, 3},
        new byte[] {0, 12, 1, 5, 2, 15, 13, 5, 6},
        new byte[] {0, 9, 12, 2, 3, 12, 4, 6, 10},
        new byte[] {0, 3, 7, 14, 5, 0, 1, 0, 9}
    },
    new byte[][]
    {
        new byte[] {0, 12, 13, 7, 5, 15, 4, 7, 14},
        new byte[] {0, 11, 10, 14, 12, 10, 14, 12, 11},
        new byte[] {0, 7, 6, 12, 14, 5, 10, 8, 13},
        new byte[] {0, 14, 12, 3, 11, 9, 7, 15, 0}
    },
    new byte[][]
    {
        new byte[] {0, 5, 12, 11, 11, 13, 14, 5, 5},
        new byte[] {0, 9, 6, 12, 1, 3, 0, 2, 0},
        new byte[] {0, 3, 9, 5, 5, 6, 1, 0, 15},
        new byte[] {0, 10, 0, 11, 12, 10, 6, 14, 3}
    },
    new byte[][]
    {
        new byte[] {0, 9, 0, 4, 12, 0, 7, 10, 0},
        new byte[] {0, 5, 9, 11, 10, 9, 11, 15, 14},
        new byte[] {0, 10, 3, 10, 2, 3, 13, 5, 3},
        new byte[] {0, 0, 5, 5, 7, 4, 0, 2, 5}
    },
    new byte[][]
    {
        new byte[] {0, 0, 5, 2, 4, 14, 5, 6, 12},
        new byte[] {0, 3, 11, 15, 14, 8, 3, 8, 9},
        new byte[] {0, 5, 2, 14, 8, 0, 11, 9, 5},
        new byte[] {0, 6, 14, 2, 2, 5, 8, 3, 6}
    },
    new byte[][]
    {
        new byte[] {0, 7, 10, 8, 15, 9, 11, 1, 7},
        new byte[] {0, 8, 5, 1, 9, 6, 8, 6, 2},
        new byte[] {0, 0, 15, 7, 4, 14, 6, 2, 8},
        new byte[] {0, 13, 9, 12, 14, 3, 13, 12, 11}
    }
};

        private enum LicenseState : int
        {
            SFTL_ERROR = -1,
            ADDOPTION = 1,
            SWRUPFRADE = 2,
            TIMEDLICENSE = 3
        }
        #endregion

        #region Functions
        private long HexToInt(char ch)
        {
            return ch >= '0' && ch <= '9' ? (long)(ch - '0') : (long)(ch - 'A' + 10);
        }

        private char IntToHex(long i)
        {
            return i <= 9 ? (char)(i + '0') : (char)(i - 10 + 'A');
        }
        private long ConvertStringToHex(string str)
        {
            int i;
            long hex = 0;

            for (i = 0; i < 8; i++)
            {
                hex = (long)((hex <<= 4) | HexToInt(str[i]));
            }

            return hex;
        }

        private string ConvertHexToString(long hex)
        {
            int i;
            char[] str = new char[8];

            for (i = 7; i >= 0; i--)
            {
                str[i] = (char)IntToHex(hex & 0xf);
                hex >>= 4;
            }

            return new string(str).Trim();
        }

        private long GetBit(IMMENSE source, int bitno, int nbits)
        {
            if (bitno <= nbits)
            {
                if ((DESBit[bitno] & source.r) != 0)
                    return 1L;
                else
                    return 0L;
            }
            else
            {
                if ((DESBit[bitno - nbits] & source.l) != 0)
                    return 1L;
                else
                    return 0L;
            }
        }
        private void KS(IMMENSE key, long[,] kn)
        {

            int it, i, j, k, l;
            IMMENSE icd_1 = new IMMENSE();
            //GREAT[] kn = new GREAT[16];
            long[] icd = new long[2];

            for (int n = 1; n <= 16; n++)
            {

                if (n == 1)
                {
                    icd[1] = icd[0] = 0L;
                    for (j = 28, k = 56; j >= 1; j--, k--)
                    {
                        icd[1] = (icd[1] <<= 1) | GetBit(key, ipc1[j], 32);
                        icd[0] = (icd[0] <<= 1) | GetBit(key, ipc1[k], 32);
                    }

                }
                if (n == 1 || n == 2 || n == 9 || n == 16)
                {
                    it = 1;

                }
                else
                {
                    it = 2;

                }
                for (i = 1; i <= it; i++)
                {
                    icd[1] = (icd[1] | ((icd[1] & 1L) << 28)) >> 1;
                    icd[0] = (icd[0] | ((icd[0] & 1L) << 28)) >> 1;
                }
                icd_1.r = icd[1];
                icd_1.l = icd[0];
                // kn[n,0] = kn[n,1] = kn[n,2] = 0;
                for (j = 16, k = 32, l = 48; j >= 1; j--, k--, l--)
                {
                    kn[n, 2] = (kn[n, 2] <<= 1) | (ushort)GetBit(icd_1, ipc2[j], 28);
                    kn[n, 1] = (kn[n, 1] <<= 1) | (ushort)GetBit(icd_1, ipc2[k], 28);
                    kn[n, 0] = (kn[n, 0] <<= 1) | (ushort)GetBit(icd_1, ipc2[l], 28);
                }
                //return kn;
            }
        }
        private void CY(long ir, long[,] k, ref long iout, int i, int isw)
        {
            GREAT ie = new GREAT();
            long itmp, ietmp1, ietmp2;
            byte[] iec = new byte[9];
            int ii, jj, irow, icol, iss, j, l, m;

            ii = (isw == DECRYPT ? 17 - i : i);

            ie.r = ie.c = ie.l = 0;
            for (j = 16, l = 32, m = 48; j >= 1; j--, l--, m--)
            {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                ie.r = (ie.r <<= 1) | ((DESBit[iet[j]] & ir) != 0 ? 1 : 0);
                ie.c = (ie.c <<= 1) | ((DESBit[iet[l]] & ir) != 0 ? 1 : 0);
                ie.l = (ie.l <<= 1) | ((DESBit[iet[m]] & ir) != 0 ? 1 : 0);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }

            ie.r ^= k[ii, 2];
            ie.c ^= k[ii, 1];
            ie.l ^= k[ii, 0];
            ietmp1 = (long)(((long)ie.c << 16) + (long)ie.r);
            ietmp2 = (long)(((long)ie.l << 8) + ((long)ie.c >> 8));
            for (j = 1, m = 5; j <= 4; j++, m++)
            {
                iec[j] = ((byte)(ietmp1 & 0x3f));
                iec[m] = ((byte)(ietmp2 & 0x3f));
                ietmp1 >>= 6;
                ietmp2 >>= 6;
            }
            itmp = 0;
            for (jj = 8; jj >= 1; jj--)
            {
                j = iec[jj];
                irow = ((j & 0x1) << 1) + ((j & 0x20) >> 5);
                icol = ((j & 0x2) << 2) + (j & 0x4) + ((j & 0x8) >> 2) + ((j & 0x10) >> 4);
                iss = isArr[icol][irow][jj];
                itmp = (long)((itmp <<= 4) | ibin[iss]);
            }
            iout = 0;
            for (j = 32; j >= 1; j--)
            {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                iout = (long)((iout <<= 1) | ((DESBit[ipp[j]] & itmp) != 0 ? 1 : 0));
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }
        }
        private IMMENSE DES(IMMENSE inp, IMMENSE key, int newkey, int isw)
        {
            int i, j, k;
            long ic = 0, shifter;
            IMMENSE itmp = new IMMENSE { l = 0, r = 0 };
            long[,] kns = new long[17, 3];

            if (initflag != 0)
            {
                initflag = 0;
                DESBit[1] = shifter = 1;
                for (j = 2; j <= 32; j++)
                {
                    DESBit[j] = (shifter <<= 1);
                }
            }
            if (newkey == SETNEWKEY)
            {
                newkey = USECURKEY;
                //for (i = 1; i <= 16; i++)
                //{
                KS(key, kns);
                //}
            }

            for (j = 32, k = 64; j >= 1; j--, k--)
            {
                itmp.r = (itmp.r <<= 1) | GetBit(inp, ip[j], 32);
                itmp.l = (itmp.l <<= 1) | GetBit(inp, ip[k], 32);
            }
            for (i = 1; i <= 16; i++)
            {
                //ii = (isw == DECRYPT ? 17 - i : i);
                CY(itmp.l, kns, ref ic, i, isw);
                ic ^= itmp.r;
                itmp.r = itmp.l;
                itmp.l = ic;
            }
            ic = itmp.r;
            itmp.r = itmp.l;
            itmp.l = ic;
            IMMENSE OutParam = new IMMENSE();
            OutParam.r = OutParam.l = 0;
            for (j = 32, k = 64; j >= 1; j--, k--)
            {
                OutParam.r = (OutParam.r <<= 1) | GetBit(itmp, ipm[j], 32);
                OutParam.l = (OutParam.l <<= 1) | GetBit(itmp, ipm[k], 32);
            }
            return OutParam;
        }

        #endregion

        #endregion

        #region public
        public string EncodeKey(LicenseDetailModel model)
        {
            string KeyPart1, KeyPart2, KeyPart3, KeyPart4, KeyPart5, KeyPart6;
            string serialNumber_temp = model.SerialNumber, serialNumber = model.SerialNumber;
            int Model = model.ModelNumber;
            string swVersion = model.Application;
            //long options = 0x1A;
            long options_temp = 0x0, options = 0x0;


            if(model.LicenseOptions != null && model.LicenseOptions.Length > 0)
            {
                foreach (string opt in model.LicenseOptions)
                {
                    options += Convert.ToInt32(opt);
                }
            }

            if (serialNumber_temp.Length > 8)
            {
                serialNumber = serialNumber_temp.Substring(1, 8);
                char serno = serialNumber_temp[0];
                options_temp = (serno - 0x30) << 16;
                options = options_temp | options;
            }

            DateTime cur = DateTime.Now.AddDays(model.ExpirationDays);
            long t = (cur.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            long ex = model.ExpirationDays * 86400;

            IMMENSE key = new IMMENSE();

            key.l = DES_MAGIC7;
            key.r = DES_MAGIC8;

            IMMENSE one = new IMMENSE();
            IMMENSE two = new IMMENSE();
            IMMENSE three = new IMMENSE();

            int state = (int)LicenseState.TIMEDLICENSE;

            switch (state)
            {
                case (int)LicenseState.ADDOPTION:
                    break;
                case (int)LicenseState.SWRUPFRADE:
                    break;
                case (int)LicenseState.TIMEDLICENSE:
                    one.l = DES_MAGIC3;
                    one.l += Model;
                    two.l = t;
                    two.r = ConvertStringToHex(serialNumber);
                    three.l = ConvertStringToHex(swVersion);
                    three.r = ex;
                    break;
            }


            IMMENSE threeOut = DES(three, key, SETNEWKEY, ENCRYPT);
            key.l = threeOut.l;
            key.r = threeOut.r;

            IMMENSE twoOut = DES(two, key, SETNEWKEY, ENCRYPT);
            key.l = twoOut.l;
            key.r = twoOut.r;

            one.r = options;

            IMMENSE oneOut = DES(one, key, SETNEWKEY, ENCRYPT);

            KeyPart1 = ConvertHexToString(oneOut.l);
            KeyPart2 = ConvertHexToString(oneOut.r);

            KeyPart3 = ConvertHexToString(twoOut.l);
            KeyPart4 = ConvertHexToString(twoOut.r);

            KeyPart5 = ConvertHexToString(threeOut.l);
            KeyPart6 = ConvertHexToString(threeOut.r);

            return KeyPart1 + "-" + KeyPart2 + "-" + KeyPart3 + "-" + KeyPart4 + "-" + KeyPart5 + "-" + KeyPart6;

        }

        #endregion
    }
}
