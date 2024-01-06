using System;
using System.Reflection;

namespace EnumPrint
{
    /// <summary>
    /// Provides extension methods for creating the string representations of an <see cref="Enum"/> value.
    /// </summary>
    public static class EnumPrintExtensions
    {
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <typeparam name="T">An <see cref="Enum"/>.</typeparam>
        /// <param name="value">The value of <see cref="Enum"/>.</param>
        /// <returns>The string representation.</returns>
        public static string Print<T>(this T value) where T : unmanaged, Enum
        {
            if (EnumPrintHelper<T>.HasFlags)
            {
                // 处理位域输出。
                return EnumPrintHelper<T>.GetBitFlagsString(value);
            }
            return EnumPrintHelper<T>.GetPrintNormal(value);
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation using the specified format.
        /// </summary>
        /// <typeparam name="T">An <see cref="Enum"/>.</typeparam>
        /// <param name="value">The value of <see cref="Enum"/>.</param>
        /// <param name="formatChar">The format char.</param>
        /// <returns>The string representation</returns>
        /// <exception cref="FormatException"><paramref name="formatChar"/> is invalid.</exception>
        public static string Print<T>(this T value, char formatChar) where T : unmanaged, Enum
        {
            switch (formatChar)
            {
                case 'G':
                case 'g':
                    return Print(value); // 常规
                case 'F':
                case 'f':
                    return EnumPrintHelper<T>.GetBitFlagsString(value); // 位域输出
                case 'D':
                case 'd':
                    return EnumPrintHelper<T>.GetValueString(value); // 整数输出
                case 'X':
                case 'x':
                    return EnumPrintHelper<T>.GetHexString(value); // 十六进制输出
                default:
                    throw new FormatException("format is Invalid.");
            }
        }

        /// <summary>
        /// 缓存枚举信息，以及实现一些基础处理。
        /// </summary>
        private static class EnumPrintHelper<T> where T : unmanaged, Enum
        {
            /// <summary>
            /// 基础类型代码
            /// </summary>
            private static readonly TypeCode s_baseType;
            /// <summary>
            /// 最大所需的字符缓冲区数
            /// </summary>
            private static readonly int s_charBuffSize;
            /// <summary>
            /// 分隔符
            /// </summary>
            private static readonly string s_separator;
            /// <summary>
            /// 定义的值
            /// </summary>
            private static readonly T[] s_values;
            /// <summary>
            /// 定义的值的ulong形式
            /// </summary>
            private static readonly ulong[] s_values64;
            /// <summary>
            /// 定义的值的ulong形式
            /// </summary>
            private static readonly string[] s_names;

            /// <summary>
            /// 遍历位域时的起始索引
            /// </summary>
            private static readonly int s_flagsStartIndex;
            /// <summary>
            /// 遍历位域时的结束索引
            /// </summary>
            private static readonly int s_flagsEndIndex;
            /// <summary>
            /// 当值是0的时候，位域输出的默认值
            /// </summary>
            private static readonly string s_flagsValueWhenZero;

            /// <summary>
            /// 是否是位域
            /// </summary>
            public static readonly bool HasFlags;

            static EnumPrintHelper()
            {
                Type type = typeof(T);
                TypeInfo typeInfo = type.GetTypeInfo();
                // 获取基础类型。
                Type basetype = Enum.GetUnderlyingType(type);
                // 手动实现TypeCode
                if (basetype == typeof(byte))
                {
                    s_baseType = TypeCode.Byte;
                }
                else if (basetype == typeof(SByte))
                {
                    s_baseType = TypeCode.SByte;
                }
                else if (basetype == typeof(Int16))
                {
                    s_baseType = TypeCode.Int16;
                }
                else if (basetype == typeof(UInt16))
                {
                    s_baseType = TypeCode.UInt16;
                }
                else if (basetype == typeof(Int32))
                {
                    s_baseType = TypeCode.Int32;
                }
                else if (basetype == typeof(UInt32))
                {
                    s_baseType = TypeCode.UInt32;
                }
                else if (basetype == typeof(Int64))
                {
                    s_baseType = TypeCode.Int64;
                }
                else
                {
                    s_baseType = TypeCode.UInt64;
                }
                // 是否表示位域
                HasFlags = typeInfo.GetCustomAttribute<FlagsAttribute>() != null;
                // 获取分隔符
                EnumSeparatorAttribute spAttr = typeInfo.GetCustomAttribute<EnumSeparatorAttribute>();
                s_separator = spAttr != null ? spAttr.Separator : ", ";
                // 获取定义的值
                Array values = Enum.GetValues(type);
                int length = values.Length;
                //  PrintNameDict = new Dictionary<T, string>(length, new MyEnumCmp());
                s_values = new T[length];
                s_values64 = new ulong[length];
                s_names = new string[length];
                EnumValueFriendlyNameAttribute friendlyNameAttr;
                string output;
                s_charBuffSize = 0;
                unsafe
                {
                    fixed (T* tp = s_values)
                    fixed (ulong* lp = s_values64)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            tp[i] = (T)values.GetValue(i);
                            lp[i] = GetUInt64(tp[i]);
                            output = tp[i].ToString();
                            // 获取相应字段的FieldInfo，然后获取是否带有EnumValueFriendlyNameAttribute
                            friendlyNameAttr = typeInfo.GetDeclaredField(output).GetCustomAttribute<EnumValueFriendlyNameAttribute>();
                            if (friendlyNameAttr != null)
                            {
                                output = friendlyNameAttr.Name;
                            }
                            s_charBuffSize += output.Length + s_separator.Length;
                            s_names[i] = output;
                        }
                    }
                }
                if (s_charBuffSize > 0)
                {
                    s_flagsStartIndex = s_values.Length - 1;
                    s_flagsEndIndex = s_values.Length - 1;
                    if (s_values64[0] == 0)
                    {
                        s_flagsEndIndex = 1;
                        s_flagsValueWhenZero = s_names[0];
                    }
                    else
                    {
                        s_flagsEndIndex = 0;
                        s_flagsValueWhenZero = "0";
                    }
                }
            }

            /// <summary>
            /// 非位域输出
            /// </summary>
            public static string GetPrintNormal(T value)
            {
                ulong uv = GetUInt64(value);
                unsafe
                {
                    fixed (ulong* p = s_values64)
                    {
                        int length = s_values64.Length;
                        int idx = FindIndex(p, s_values64.Length, GetUInt64(value));
                        if (idx < 0)
                        {
                            return GetValueString(value);
                        }
                        return s_names[idx];
                    }
                }
            }



            /// <summary>
            /// 获取十进制字符串
            /// </summary>
            public static string GetValueString(T value)
            {
                unsafe
                {
                    T* p = &value;
                    switch (s_baseType)
                    {
                        case TypeCode.Byte:
                            return (*(byte*)p).ToString();
                        case TypeCode.SByte:
                            return (*(sbyte*)p).ToString();
                        case TypeCode.UInt16:
                            return (*(ushort*)p).ToString();
                        case TypeCode.Int16:
                            return (*(short*)p).ToString();
                        case TypeCode.UInt32:
                            return (*(uint*)p).ToString();
                        case TypeCode.Int32:
                            return (*(int*)p).ToString();
                        case TypeCode.Int64:
                            return (*(long*)p).ToString();
                        default: // TypeCode.UInt64:
                            return (*(ulong*)p).ToString();
                    }
                }
            }

            /// <summary>
            /// 获取十六进制字符串
            /// </summary>
            public static string GetHexString(T value)
            {
                unsafe
                {
                    T* p = &value;
                    switch (sizeof(T))
                    {
                        case 1:
                            return (*(byte*)p).ToString("X2", null);
                        case 2:
                            return (*(ushort*)p).ToString("X4", null);
                        case 4:
                            return (*(uint*)p).ToString("X8", null);
                        default: // 8
                            return (*(ulong*)p).ToString("X16", null);
                    }
                }
            }

            /// <summary>
            /// 处理位域输出
            /// </summary>
            public static string GetBitFlagsString(T value)
            {
                ulong v64 = GetUInt64(value);

                if (s_charBuffSize == 0)
                {
                    // 如果没有定义的值，不用比较，直接返回整数值。
                    return GetValueString(value);
                }

                if (v64 == 0)
                {
                    return s_flagsValueWhenZero;
                }

                // 开始遍历
                int index = s_flagsStartIndex;
                unsafe
                {
                    char* charBuffer = stackalloc char[s_charBuffSize];
                    char* buffer = charBuffer + s_charBuffSize;

                    fixed (T* tp = s_values)
                    fixed (ulong* lp = s_values64)
                    {
                        while (index >= s_flagsEndIndex)
                        {
                            if ((lp[index] & v64) == lp[index])
                            {
                                v64 -= lp[index];
                                buffer = InsertToCharBuffer(buffer, s_separator);
                                buffer = InsertToCharBuffer(buffer, s_names[index]);
                            }
                            --index;
                        }
                    }
                    if (v64 != 0)
                    {
                        // 如果最后不是0，说明是错误的位域
                        return GetValueString(value);
                    }
                    return new string(buffer, 0, s_charBuffSize - (int)(buffer - charBuffer) - s_separator.Length);
                }

            }

            /// <summary>
            /// 将字符串追加到缓冲区。从后向前。
            /// </summary>
            unsafe private static char* InsertToCharBuffer(char* buffer, string str)
            {
                int length = str.Length;
                buffer -= length;
                char* tb = buffer;
                fixed (char* pStr = str)
                {
                    char* tstr = pStr;
                    if (sizeof(IntPtr) == 8)
                    {
                        while (length >= 16)
                        {
                            *(long*)(tb) = *(long*)(tstr);
                            *(long*)(tb + 4) = *(long*)(tstr + 4);
                            *(long*)(tb + 8) = *(long*)(tstr + 8);
                            *(long*)(tb + 12) = *(long*)(tstr + 12);
                            tb += 16;
                            tstr += 16;
                            length -= 16;
                        }
                        while (length >= 4)
                        {
                            *(long*)(tb) = *(long*)(tstr);
                            tb += 4;
                            tstr += 4;
                            length -= 4;
                        }
                    }
                    else
                    {
                        while (length >= 16)
                        {
                            *(int*)(tb) = *(int*)(tstr);
                            *(int*)(tb + 2) = *(int*)(tstr + 2);
                            *(int*)(tb + 4) = *(int*)(tstr + 4);
                            *(int*)(tb + 6) = *(int*)(tstr + 6);
                            *(int*)(tb + 8) = *(int*)(tstr + 8);
                            *(int*)(tb + 10) = *(int*)(tstr + 10);
                            *(int*)(tb + 12) = *(int*)(tstr + 12);
                            *(int*)(tb + 14) = *(int*)(tstr + 14);
                            tb += 16;
                            tstr += 16;
                            length -= 16;
                        }
                        while (length >= 2)
                        {
                            *(int*)(tb) = *(int*)(tstr);
                            tb += 2;
                            tstr += 2;
                            length -= 2;
                        }
                    }

                    while (length > 0)
                    {
                        *tb = *tstr;
                        ++tb;
                        ++tstr;
                        --length;
                    }
                }




                return buffer;
            }

            /// <summary>
            /// 查找
            /// </summary>
            unsafe private static int FindIndex(ulong* p, int length, ulong v)
            {
                int i = 0;
                while (length >= 4)
                {
                    if (p[i] == v) return i;
                    if (p[i + 1] == v) return i + 1;
                    if (p[i + 2] == v) return i + 2;
                    if (p[i + 3] == v) return i + 3;
                    i += 4;
                    length -= 4;
                }
                while (length > 0)
                {
                    if (p[i] == v) return i;
                    ++i;
                    --length;
                }
                return -1;
            }

            /// <summary>
            /// 转为long
            /// </summary>
            private static ulong GetUInt64(T value)
            {
                unsafe
                {
                    T* p = &value;
                    switch (sizeof(T))
                    {
                        case 1:
                            return *(byte*)p;
                        case 2:
                            return *(ushort*)p;
                        case 4:
                            return *(uint*)p;
                        default: // 8
                            return *(ulong*)p;
                    }
                }
            }


        }


        // 导入未定义的类型。
        private enum TypeCode
        {
            Byte,
            SByte,
            Int16,
            UInt16,
            Int32,
            UInt32,
            Int64,
            UInt64
        }
    }
}

#region 导入非托管泛型约束类型
namespace System.Runtime.InteropServices
{
    internal enum UnmanagedType
    {
        Bool = 0x2,         // 4 byte boolean value (true != 0, false == 0)

        I1 = 0x3,         // 1 byte signed value

        U1 = 0x4,         // 1 byte unsigned value

        I2 = 0x5,         // 2 byte signed value

        U2 = 0x6,         // 2 byte unsigned value

        I4 = 0x7,         // 4 byte signed value

        U4 = 0x8,         // 4 byte unsigned value

        I8 = 0x9,         // 8 byte signed value

        U8 = 0xa,         // 8 byte unsigned value

        R4 = 0xb,         // 4 byte floating point

        R8 = 0xc,         // 8 byte floating point

        Currency = 0xf,         // A currency

        BStr = 0x13,        // OLE Unicode BSTR

        LPStr = 0x14,        // Ptr to SBCS string

        LPWStr = 0x15,        // Ptr to Unicode string

        LPTStr = 0x16,        // Ptr to OS preferred (SBCS/Unicode) string

        ByValTStr = 0x17,        // OS preferred (SBCS/Unicode) inline string (only valid in structs)

        IUnknown = 0x19,        // COM IUnknown pointer. 

        IDispatch = 0x1a,        // COM IDispatch pointer

        Struct = 0x1b,        // Structure

        Interface = 0x1c,        // COM interface

        SafeArray = 0x1d,        // OLE SafeArray

        ByValArray = 0x1e,        // Array of fixed size (only valid in structs)

        SysInt = 0x1f,        // Hardware natural sized signed integer

        SysUInt = 0x20,

        VBByRefStr = 0x22,

        AnsiBStr = 0x23,        // OLE BSTR containing SBCS characters

        TBStr = 0x24,        // Ptr to OS preferred (SBCS/Unicode) BSTR

        VariantBool = 0x25,        // OLE defined BOOLEAN (2 bytes, true == -1, false == 0)

        FunctionPtr = 0x26,        // Function pointer

        AsAny = 0x28,        // Paired with Object type and does runtime marshalling determination

        LPArray = 0x2a,        // C style array

        LPStruct = 0x2b,        // Pointer to a structure

        CustomMarshaler = 0x2c,

        Error = 0x2d,

        [System.Runtime.InteropServices.ComVisible(false)]
        IInspectable = 0x2e,

        [System.Runtime.InteropServices.ComVisible(false)]
        HString = 0x2f,        // Windows Runtime HSTRING

        [System.Runtime.InteropServices.ComVisible(false)]
        LPUTF8Str = 0x30,        // UTF8 string
    }
}
#endregion
