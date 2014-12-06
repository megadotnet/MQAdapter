using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messag.Utility.Transfer
{
    public static class ConvertExtension
    {
        private static Dictionary<Type, Func<object, object>> dict = new Dictionary<Type, Func<object, object>>();

        static ConvertExtension()
        {
            dict.Add(typeof(sbyte), WrapValueConvert(Convert.ToSByte));
            dict.Add(typeof(byte), WrapValueConvert(Convert.ToByte));
            dict.Add(typeof(short), WrapValueConvert(Convert.ToInt16));
            dict.Add(typeof(int), WrapValueConvert(Convert.ToInt32));
            dict.Add(typeof(long), WrapValueConvert(Convert.ToInt64));
            dict.Add(typeof(ushort), WrapValueConvert(Convert.ToUInt16));
            dict.Add(typeof(uint), WrapValueConvert(Convert.ToUInt32));
            dict.Add(typeof(ulong), WrapValueConvert(Convert.ToUInt64));
            dict.Add(typeof(double), WrapValueConvert(Convert.ToDouble));
            dict.Add(typeof(float), WrapValueConvert(Convert.ToSingle));
            dict.Add(typeof(decimal), WrapValueConvert(Convert.ToDecimal));
            dict.Add(typeof(Guid), f => new Guid(f.ToString()));

            dict.Add(typeof(sbyte?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToSByte)(o); });
            dict.Add(typeof(byte?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToByte)(o); });
            dict.Add(typeof(short?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToInt16)(o); });
            dict.Add(typeof(int?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToInt32)(o); });
            dict.Add(typeof(long?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToInt64)(o); });
            dict.Add(typeof(ushort?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToUInt16)(o); });
            dict.Add(typeof(uint?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToUInt32)(o); });
            dict.Add(typeof(ulong?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToUInt64)(o); });
            dict.Add(typeof(double?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToDouble)(o); });
            dict.Add(typeof(float?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToSingle)(o); });
            dict.Add(typeof(decimal?), (o) => { return o == null ? null : WrapValueConvert(Convert.ToDecimal)(o); });
            dict.Add(typeof(Guid?), (o) => { if (o == null) { return null; } return new Guid(o.ToString()); });

            dict.Add(typeof(string), Convert.ToString);
        }

        

        private static Func<object, object> WrapValueConvert<T>(Func<object, T> input) where T : struct
        {
            return i =>
            {
                if (i == null || i is DBNull) { return null; }
                return input(i);
            };
        }

        public static bool CanConvertTo(this object obj, Type targetType)
        {
            return dict.ContainsKey(targetType);
        }

        public static T To<T>(this object obj)
        {
            return (T)To(obj, typeof(T));
        }

        public static T To<T>(this object obj, T defaultValue)
        {
            try
            {
                if (obj == null) { return defaultValue; }
                return (T)To(obj, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool TryConvertTo(this object obj, Type targetType, out object value)
        {
            value = null;
            if (obj != null)
            {
                try
                {
                    if (obj.GetType() == targetType || targetType.IsAssignableFrom(obj.GetType()))
                    {
                        value = obj;
                    }
                    else if (dict.ContainsKey(targetType))
                    {
                        value = dict[targetType](obj);
                    }
                    else if (targetType.IsEnum)
                    {
                        value = Enum.Parse(targetType, obj.ToString(), true);
                    }
                    else
                    {
                        value = Convert.ChangeType(obj, targetType);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (!targetType.IsValueType)
                {
                    value = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public static object To(this object obj, Type targetType)
        {
            if (obj != null)
            {
                if (obj.GetType() == targetType || targetType.IsAssignableFrom(obj.GetType()))
                {
                    return obj;
                }
                else if (dict.ContainsKey(targetType))
                {
                    return dict[targetType](obj);
                }
                else
                {
                    try
                    {
                        return Convert.ChangeType(obj, targetType);
                    }
                    catch
                    {
                        throw new NotImplementedException("未实现到" + targetType.Name + "的转换");
                    }
                }
            }
            else
            {
                if ((targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>)) || !targetType.IsValueType)
                {
                    return null;
                }
                else
                {
                    throw new ArgumentNullException("obj", "不能将null转换为" + targetType.Name);
                }
            }
        }
    }
}
