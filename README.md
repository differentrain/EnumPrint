# EnumPrint

EnumPrint is a high performance library for convert the enum value to its string representation.

This library solved these problem:
- Output enum value with its friendly name.
- Improved the performance of enum-to-string convertion in .NET Framework. 

## Getting Started

### Installation

`NuGet\Install-Package EnumPrint`

***EnumPrint requires .NET Standard 1.0 . But if you copy the source code to you project directly, it may supports more runtime version with [little modification](#how-to-make-this-libaray-supports-net-framework-20).***

### Convert Enum Value To String

EnumPrint provides two extension methods:
- `Print<T>(this T) where T: unmanaged, Enum`
- `Print<T>(this T, char) where T: unmanaged, Enum`

The behaviors of these methods are same as [`Enum.ToString()`](https://learn.microsoft.com/en-us/dotnet/api/system.enum.tostring?view=netstandard-1.0#system-enum-tostring) and [`Enum.ToString(string)`](https://learn.microsoft.com/en-us/dotnet/api/system.enum.tostring?view=netstandard-1.0#system-enum-tostring(system-string)).

So you can use it with zero Learning cost.
```
using EnumPrint;

// s = "Sunday"
string s = DayOfWeek.Sunday.Print(); 

// s= "Hidden, Directory"
s = (FileAttributes.Hidden | FileAttributes.Directory).Print('F'); 
```

### Customizing Output Content

`EnumSeparatorAttribute` to specify the separator, and `EnumValueFriendlyNameAttribute`  to specify the friendly name.

```
[EnumSeparator(" and ")]
[Flags]
enum Num
{
    [EnumValueFriendlyName("No.1")]
    One,
    [EnumValueFriendlyName("No.2")]
    Two,
    Three
}

// s = "No.1"
string s = Num.One.Print();

// s = "Three"
s = Num.Three.Print();

// s = "No.1 and No.2"
s =(Num.One | Num.Two).Print();
```


## Benchmark

- **Always** using `EnumPrint` in .NET Framework.
- **Avoid** using `EnumPrint`  to convert enum value in .NET(Core) if the enum type has no `EnumSeparatorAttribute` or `EnumValueFriendlyNameAttribute` .

| Method        | Job                  | Runtime              | Mean        | Error     | StdDev    | Median      |
|-------------- |--------------------- |--------------------- |------------:|----------:|----------:|------------:|
| TestPrintG    | .NET 8.0             | .NET 8.0             |  1,364.4 ns |  24.89 ns |  23.28 ns |  1,364.6 ns |
| TestToStringG | .NET 8.0             | .NET 8.0             |    881.8 ns |  14.54 ns |  18.39 ns |    875.4 ns |
| TestPrintF    | .NET 8.0             | .NET 8.0             |  1,364.9 ns |  17.10 ns |  16.00 ns |  1,358.5 ns |
| TestToStringF | .NET 8.0             | .NET 8.0             |    892.0 ns |  17.81 ns |  19.80 ns |    882.1 ns |
| TestPrintD    | .NET 8.0             | .NET 8.0             |    172.5 ns |   2.66 ns |   2.22 ns |    172.1 ns |
| TestToStringD | .NET 8.0             | .NET 8.0             |    379.0 ns |   7.16 ns |  12.34 ns |    374.0 ns |
| TestPrintX    | .NET 8.0             | .NET 8.0             |    488.4 ns |   9.38 ns |  21.18 ns |    480.8 ns |
| TestToStringX | .NET 8.0             | .NET 8.0             |    535.8 ns |  10.64 ns |  13.84 ns |    530.4 ns |
| TestPrintG    | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  2,159.0 ns |  26.93 ns |  22.49 ns |  2,157.1 ns |
| TestToStringG | .NET Framework 4.6.1 | .NET Framework 4.6.1 | 53,461.4 ns | 348.56 ns | 326.05 ns | 53,474.7 ns |
| TestPrintF    | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  2,513.0 ns |  50.12 ns |  89.09 ns |  2,500.1 ns |
| TestToStringF | .NET Framework 4.6.1 | .NET Framework 4.6.1 | 11,662.2 ns | 231.01 ns | 283.70 ns | 11,556.0 ns |
| TestPrintD    | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  1,553.0 ns |  29.29 ns |  27.40 ns |  1,541.7 ns |
| TestToStringD | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  2,215.8 ns |  18.36 ns |  16.28 ns |  2,215.0 ns |
| TestPrintX    | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  1,370.9 ns |  27.24 ns |  56.26 ns |  1,347.9 ns |
| TestToStringX | .NET Framework 4.6.1 | .NET Framework 4.6.1 |  2,519.5 ns |  38.69 ns |  32.31 ns |  2,512.9 ns |
| TestPrintG    | .NET Framework 4.8   | .NET Framework 4.8   |  2,493.4 ns |  34.99 ns |  32.73 ns |  2,483.8 ns |
| TestToStringG | .NET Framework 4.8   | .NET Framework 4.8   | 54,529.9 ns | 404.74 ns | 378.60 ns | 54,539.1 ns |
| TestPrintF    | .NET Framework 4.8   | .NET Framework 4.8   |  2,519.5 ns |  46.25 ns |  38.62 ns |  2,501.3 ns |
| TestToStringF | .NET Framework 4.8   | .NET Framework 4.8   | 11,592.7 ns | 114.69 ns | 101.67 ns | 11,574.3 ns |
| TestPrintD    | .NET Framework 4.8   | .NET Framework 4.8   |  1,415.4 ns |  16.38 ns |  14.52 ns |  1,416.6 ns |
| TestToStringD | .NET Framework 4.8   | .NET Framework 4.8   |  2,258.5 ns |  43.82 ns |  53.82 ns |  2,235.6 ns |
| TestPrintX    | .NET Framework 4.8   | .NET Framework 4.8   |  1,332.3 ns |  19.40 ns |  17.19 ns |  1,331.1 ns |
| TestToStringX | .NET Framework 4.8   | .NET Framework 4.8   |  2,484.2 ns |  46.90 ns |  39.16 ns |  2,481.6 ns |

## How To Make This Libaray Supports .NET Framework 2.0

### Step.1

.NET Framework 2.0 does not support extension-methods, so the first step is add related type(s):

```
// internal is enough, do not need to expose this type.

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    internal sealed class ExtensionAttribute : Attribute { }
}
```
### Step.2

Since .NET Framework 2.0 does not import `TypeInfo` class, then we should use [GetCustomAttributes(Boolean)](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo.getcustomattributes?view=net-8.0#system-reflection-memberinfo-getcustomattributes(system-boolean)) method to get custom attributes.

Original code:

```
 static EnumPrintHelper()
 {
     Type type = typeof(T);
     TypeInfo typeInfo =type.GetTypeInfo();
     ……
     ……
     HasFlags = typeInfo.GetCustomAttribute<FlagsAttribute>() != null;
     EnumSeparatorAttribute spAttr = typeInfo.GetCustomAttribute<EnumSeparatorAttribute>();
     ……
     ……
     friendlyNameAttr = typeInfo.GetDeclaredField(output).GetCustomAttribute<EnumValueFriendlyNameAttribute>();
     ……
     ……
 }
```
New Code:

```
 static EnumPrintHelper()
 {
     Type type = typeof(T);
     TypeInfo typeInfo =type.GetTypeInfo();
     HasFlags = false;
     s_separator = ", ";
     foreach (object item in attributes)
     {
        if (item is FlagsAttribute)
        {
            HasFlags = true;
        }
        else if (item is EnumSeparatorAttribute enumSeparator)
        {
            s_separator = enumSeparator.Separator;
        }
     }
     ……
     ……
     friendlyNameAttr = type.GetField(output)
                            .GetCustomAttributes(true)
                            .FirstOrDefault(c=>c is EnumValueFriendlyNameAttribute) as EnumValueFriendlyNameAttribute;
     ……
     ……
 }
```
The **full code** is :

```
static EnumPrintHelper()
{
    Type type = typeof(T);
    object[] attributes = type.GetCustomAttributes(false);
    HasFlags = false;
    s_separator = ", ";
    foreach (object item in attributes)
    {
        if (item is FlagsAttribute)
        {
            HasFlags = true;
        }
        else if (item is EnumSeparatorAttribute enumSeparator)
        {
            s_separator = enumSeparator.Separator;
        }
    }
    Type basetype = Enum.GetUnderlyingType(type);
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
    Array values = Enum.GetValues(type);
    int length = values.Length;
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
                friendlyNameAttr = type
                    .GetField(output)
                    .GetCustomAttributes(true)
                    .FirstOrDefault(c => c is EnumValueFriendlyNameAttribute) as EnumValueFriendlyNameAttribute;
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
```

### Step.3

In `EnumValueFriendlyNameAttribute` class, we used `string.IsNullOrWhiteSpace` for arguments verification. 

Remove it!

Or verify arguments by another way.
