﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Protocol
{
    public static class BooleanByteWrapper
    {
        public static byte SetFlag(byte flag, byte offset, bool value)
        {
            if (offset >= 8)
            {
                throw new ArgumentException("offset must be lesser than 8");
            }
            return value ? ((byte)((int)flag | 1 << (int)offset)) : ((byte)((int)flag & 255 - (1 << (int)offset)));
        }
        public static byte SetFlag(int flag, byte offset, bool value)
        {
            if (offset >= 8)
            {
                throw new ArgumentException("offset must be lesser than 8");
            }
            return value ? ((byte)(flag | 1 << (int)offset)) : ((byte)(flag & 255 - (1 << (int)offset)));
        }
        public static bool GetFlag(byte flag, byte offset)
        {
            if (offset >= 8)
            {
                throw new ArgumentException("offset must be lesser than 8");
            }
            return (flag & (byte)(1 << (int)offset)) != 0;
        }
    }
}
