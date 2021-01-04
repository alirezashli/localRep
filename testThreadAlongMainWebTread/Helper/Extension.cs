using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once IdentifierTypo
namespace testThreadAlongMainWebTread.Util
{
    public static class ValidationExtension
    {
        private static readonly string InvalidEntry =
            "(^1{1,}$)|(^2{1,10}$)|(^3{1,}$)|(^4{1,}$)|(^5{1,}$)|(^6{1,}$)|(^7{1,}$)|(^8{1,}$)|(^9{1,}$)|(^0{1,}$)";

        private static readonly int[] PanValidationCoefficient =
            {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2};

        public static bool IsPaymentId(this string paymentId)
            => !string.IsNullOrWhiteSpace(paymentId) && Regex.IsMatch(input: paymentId, pattern: "^[0-9]{1,18}$");
        public static bool IsPan(this string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || !Regex.IsMatch(input: pan, pattern: "^[1-9]{1}[0-9]{15,18}$"))
                return false;
            var summary = 0;
            for (var index = 0; index < pan.Length; index++)
            {
                int coefficientResult;
                coefficientResult =
                    (coefficientResult = int.Parse(pan[index].ToString()) * PanValidationCoefficient[index]) > 9
                        ? coefficientResult - 9
                        : coefficientResult;
                summary += coefficientResult;
            }

            return summary % 10 == 0; return summary % 10 == 0;
        }

        public static bool IsIpAddress(this string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return false;
            ipAddress = ipAddress.Trim();
            //IPV4 Validation
            var ipv4Validation = Regex.IsMatch(ipAddress,
                @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");
            if (ipv4Validation) return true;

            if (ipAddress.Contains("%"))
                ipAddress = ipAddress.Substring(startIndex: 0, length: ipAddress.IndexOf('%'));

            if (IPAddress.TryParse(ipAddress, out var ipv6))
                return ipv6.AddressFamily == AddressFamily.InterNetworkV6;

            return false;
        }

        public static bool IsCvv2(this string str)
            => !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(input: str, pattern: "^[0-9]{3,4}$");

        public static bool IsToken(this string str)
            => !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(input: str, pattern: "^[0-9A-Za-z]{36}$");

        public static bool IsPin2(this string str)
            => !string.IsNullOrWhiteSpace(str) &&
               Regex.IsMatch(input: str, pattern: "^[0-9]{5,12}$");

        public static bool IsExpireDate(this string str)
        {
            if (
                string.IsNullOrWhiteSpace(str) ||
                !Regex.IsMatch(input: str, pattern: "^[0-9]{4}$"))
                return false;

            str = str.StartsWith("00") ? $"14{str}" : $"13{str}";

            return !(int.Parse(str) < int.Parse(DateTime.Now.CurrentPersianYearAndMonth_TwoDigit()));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string CurrentPersianYearAndMonth_TwoDigit(this DateTime dt)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(dt).ToString().Substring(2):00}{pc.GetMonth(dt):00}";
        }

        public static bool IsTerminalId(this string terminalId)
            => !string.IsNullOrWhiteSpace(terminalId) && Regex.IsMatch(input: terminalId, pattern: "^[0-9]{8}$");

        public static bool IsAcceptorId(this string acceptorId)
            => !string.IsNullOrWhiteSpace(acceptorId) && Regex.IsMatch(input: acceptorId, pattern: "^[0-9]{15}$");

        public static bool IsNumeric(this string str, int minLen, int maxLen)
            => !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(input: str, pattern: $"^[0-9]{{{minLen},{maxLen}}}$");

        public static bool IsNationalId(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) ||
                Regex.IsMatch(input: str, pattern: InvalidEntry) ||
                !Regex.IsMatch(input: str, pattern: "^[0-9]{10}$")) return false;
            var a = Convert.ToInt32(str[9].ToString());
            long b = 0;
            var coefficient = 10;
            foreach (var value in str.Substring(0, 9))
            {
                b += Convert.ToInt32(value.ToString()) * coefficient;
                coefficient -= 1;
            }

            var c = Convert.ToInt32(b % 11);
            return ((c == 0 & a == c) | (c == 1 & a == 1) | (c > 1 & a == (11 - c)));
        }

        public static bool IsAmount(this string amount)
            => Regex.IsMatch(amount, "^[0-9]{4,12}$");

        public static bool IsAmount(this long amount)
        {
            return amount >= 1000 && amount < 999999999999;
        }


        public static bool IsAmount(this decimal amount)
        {
            return amount >= 1000 && amount < 999999999999;
        }
        public static bool IsMobileNumber(this string mobile)
            => !string.IsNullOrWhiteSpace(mobile) && Regex.IsMatch(input: mobile, pattern: "^989[0-9]{2}[0-9]{7}$");

        public static bool IsIban(this string iban)
            => !string.IsNullOrWhiteSpace(iban) &&
               Regex.IsMatch(input: iban, pattern: "^IR[0-9]{24}$") &&
               decimal.Parse($"{iban.Substring(4)}1827{iban.Substring(startIndex: 2, length: 2)}") % 97 == 1;



        public static bool IsUri(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            try
            {
                var uri = new Uri(str);
                return true;
            }
            catch (Exception)
            {
                return false;
                //Ignored
            }
            //   Regex.IsMatch(input: str.ToLower(),
            //       pattern: @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        }


        public static bool IsMailAddress(this string str)
            => !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(input: str,
                   pattern:
                   @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                   options: RegexOptions.IgnoreCase);

        public static bool IsMerchantId(this string str)
        {
            return !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(str, "^[0-9|A-Z]{4}$");
        }
        public static bool IsRrn(this string str)
        {
            return !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(str, "^[0-9]{7,12}$");
        }

        public static bool IsStan(this long stan)
            => IsStan(stan.ToString());
        public static bool IsStan(this int stan)
            => IsStan(stan.ToString());
        public static bool IsStan(this string stan)
            => !string.IsNullOrWhiteSpace(stan) && Regex.IsMatch(stan, "^[0-9]{1,6}$");


    }
    public static class Extension
    {


        private static readonly byte[] CipherKey =
        {
            0x13, 0x29, 0x00, 0x8F, 0xD6, 0xDD, 0x20, 0x15,
            0xF0, 0xC0, 0x86, 0x2F, 0x88, 0x1D, 0x91, 0x1C,
            0x9F, 0xDA, 0x7B, 0x93, 0x33, 0xC5, 0xD9, 0x00
        };
        public static string GetBin(this string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length < 6)
                throw new Exception("Invliad Pan Pattern");
            return pan.Substring(0, 6);
        }

        public static string ToPersianDateTime(this DateTime dt)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(dt):0000}/{pc.GetMonth(dt):00}/{pc.GetDayOfMonth(dt):00} {dt:HH:mm:ss}";
        }


        //public static T MapTo<T>(this object source)
        //    where T : class, new()
        //{
        //    var result = Activator.CreateInstance<T>();
        //    foreach (var propsInfo in from s in source.GetType().GetProperties()
        //                              let mapAttribute = s.GetCustomAttribute<SoapAttribute>()
        //                              //join t in typeof(T).GetProperties() on (mapAttribute?.TargetPropertyName ?? s.Name).ToLower() equals
        //                              join t in typeof(T).GetProperties() on mapAttribute?.TargetPropertyName.ToLower() equals t.Name.ToLower()
        //                              where s.CanRead && t.CanWrite && mapAttribute != null
        //                              select new
        //                              {
        //                                  Source = s,
        //                                  Target = t
        //                              })
        //        propsInfo.Target.SetValue(result, propsInfo.Source.GetValue(source, null), null);
        //    return result;
        //}
        public static string AsMaskedPan(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str) || str.Length < 16) return str;
                return $"{str.Substring(0, 6)}{string.Empty.PadLeft(str.Length - 10, '*')}{str.Substring(str.Length - 4)}";
            }
            catch (Exception)
            {
                return null;
            }

        }
        public static long ToDateAsLong(this DateTime dateTime)
            => long.Parse(dateTime.ToString(format: "yyyyMMdd"));

        public static int ToTimeAsInt(this DateTime dateTime)
            // ReSharper disable once StringLiteralTypo
            => int.Parse(dateTime.ToString(format: "HHmmss"));

        public static long ToEpochTimestamp(this DateTime dateTime)
            => (long)(dateTime.ToUniversalTime() - new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0,
                           second: 0, kind: DateTimeKind.Utc)).TotalSeconds;

        private static readonly DateTime Epoch = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);

        public static DateTime FromEpoch(this long unixTime)
            => Epoch.AddSeconds(unixTime).ToLocalTime();



        // ReSharper disable once MemberCanBePrivate.Global
        public static byte[] HexStringToByteArray(this string hexString)
            => Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(value: hexString.Substring(startIndex: x, length: 2), fromBase: 16))
                .ToArray();

        public static string ByteArrayToHexString(this byte[] bytes)
            => bytes.Select(t => t.ToString(format: "X2")).Aggregate((a, b) => $"{a}{b}");

        //public static string ReversePinBlock(this byte[] pinBlock, string pan)
        //{
        //    var l2 = $"0000{pan.Substring(startIndex: pan.Length - (1 + 12), length: 12)}".HexStringToByteArray();
        //    var result = new byte[8];
        //    for (var i = 0; i < pinBlock.Length; i++)
        //        result[i] = (byte)(pinBlock[i] ^ l2[i]);

        //    return result.ByteArrayToHexString().Replace(oldValue: "F", newValue: string.Empty).Substring(2);
        //}


        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        public static string EncCBCPKCS5(this string plainText, byte[] cipherKey = null)
            => plainText.EncCBCPKCS5(cipherKey, vector: null);

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        public static string EncCBCPKCS5(this string plainText, byte[] cipherKey, byte[] vector)
        {
            try
            {
                byte[] result;

                var provider = new TripleDESCryptoServiceProvider
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.None,
                    Key = cipherKey ?? CipherKey,
                    IV = vector ?? new byte[8]
                };
                var encryptProvider = provider.CreateEncryptor();

                var plainBytes = Encoding.UTF8.GetBytes(plainText);

                // PKCS5 Manual Padding
                var blockSize = provider.BlockSize / 8;
                var pl = blockSize - plainBytes.Length % blockSize;
                var paddingValue = Convert.ToByte(value: $"{pl:00}", fromBase: 16);

                var buffer = new byte[plainBytes.Length + pl];
                Array.Copy(sourceArray: plainBytes, sourceIndex: 0, destinationArray: buffer, destinationIndex: 0, length: plainBytes.Length);
                for (var idx = plainBytes.Length; idx < buffer.Length; idx++)
                    buffer[idx] = paddingValue;


                using (var ms = new MemoryStream(buffer))
                {
                    using (var cs = new CryptoStream(ms, encryptProvider, CryptoStreamMode.Write))
                    {
                        cs.Write(buffer, offset: 0, count: buffer.Length);
                    }
                    result = ms.ToArray();
                }

                return result.ByteArrayToHexString();
            }
            catch (Exception)
            {
                return null;
            }
        }       

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        public static string DcrCBCPKCS5FromHexString(this string encryptedHexString, byte[] cipherKey = null)
            => encryptedHexString.HexStringToByteArray().DcrCBCPKCS5(cipherKey);

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        public static string DcrCBCPKCS5(this byte[] encryptedTextArray, byte[] cipherKey = null)
        {
            var cryptoServiceProvider = new TripleDESCryptoServiceProvider
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                Key = cipherKey ?? CipherKey,
                IV = new byte[8]
            };

            var descriptor = cryptoServiceProvider.CreateDecryptor();

            var byteArray = encryptedTextArray;
            var buffer = new char[byteArray.Length];

            using (var memoryStream = new MemoryStream(byteArray))
            {
                using (var cryptoStream =
                    new CryptoStream((Stream)memoryStream, descriptor, CryptoStreamMode.Read))
                using (var streamReader = new StreamReader((Stream)cryptoStream, Encoding.UTF8))
                    streamReader.ReadBlock(buffer, index: 0, count: buffer.Length);
            }

            var num = int.Parse(((byte)buffer[buffer.Length - 1]).ToString(format: "X2"));
            var chArray = new char[buffer.Length - num];
            Array.Copy(sourceArray: buffer, sourceIndex: 0, destinationArray: chArray, destinationIndex: 0, length: chArray.Length);
            return ((IEnumerable<char>)chArray).Select(t => t.ToString()).Aggregate((a, b) => a + b);
        }

        public static byte[] EncryptWithTripleDes(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[] result;
            using (var alg = new TripleDESCryptoServiceProvider
            {
                Key = key,
                IV = iv,
                Mode = mode,
                Padding = padding
            })
            {
                MemoryStream mStream;

                using (var cryptStream = new CryptoStream(mStream = new MemoryStream(), transform: alg.CreateEncryptor(),
                    mode: CryptoStreamMode.Write))
                {
                    cryptStream.Write(data, offset: 0, count: data.Length);
                    result = mStream.ToArray();
                }
            }

            return result;
        }

        public static byte[] EncryptWithDes(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[] result;
            using (var alg = new DESCryptoServiceProvider
            {
                Key = key,
                IV = iv,
                Mode = mode,
                Padding = padding
            })
            {
                MemoryStream mStream;

                using (var cryptoStream = new CryptoStream((mStream = new MemoryStream()), transform: alg.CreateEncryptor(),
                    mode: CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, offset: 0, count: data.Length);
                    result = mStream.ToArray();
                }
            }

            return result;
        }

        public static byte[] DecryptWithTripleDes(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[] result;
            using (var alg = new TripleDESCryptoServiceProvider
            {
                Key = key,
                IV = iv,
                Mode = mode,
                Padding = padding
            })
            {
                MemoryStream mStream;

                using (var cryptStream = new CryptoStream(mStream = new MemoryStream(), transform: alg.CreateDecryptor(),
                    mode: CryptoStreamMode.Write))
                {
                    cryptStream.Write(data, offset: 0, count: data.Length);
                    cryptStream.FlushFinalBlock();
                    result = mStream.ToArray();
                }
            }

            return result;
        }

        public static byte[] DecryptWithDes(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[] result;
            using (var alg = new DESCryptoServiceProvider()
            {
                Key = key,
                IV = iv,
                Mode = mode,
                Padding = padding
            })
            {
                MemoryStream mStream;

                using (var cryptStream = new CryptoStream(mStream = new MemoryStream(), transform: alg.CreateDecryptor(),
                    mode: CryptoStreamMode.Write))
                {
                    cryptStream.Write(data, offset: 0, count: data.Length);
                    cryptStream.FlushFinalBlock();
                    result = mStream.ToArray();
                }
            }

            return result;
        }


        public static string GetDataMemberName(this object obj)
        {
            var type = obj.GetType();
            return type.GetCustomAttribute<DataMemberAttribute>()?.Name;
        }

        public static byte[] EncryptWithTripleDes2(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[][] keys;
            switch (key.Length)
            {
                case 8:
                    keys = new[]
                    {
                        key,
                        key,
                        key
                    };
                    break;
                case 16:
                    {
                        var kPart1 = new byte[8];
                        var kPart2 = new byte[8];

                        Array.Copy(sourceArray: key, sourceIndex: 0, destinationArray: kPart1, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 8, destinationArray: kPart2, destinationIndex: 0, length: 8);

                        keys = new[]
                        {
                        kPart1,
                        kPart2,
                        kPart1
                    };
                    }
                    break;
                case 24:
                    {
                        var kPart1 = new byte[8];
                        var kPart2 = new byte[8];
                        var kPart3 = new byte[8];

                        Array.Copy(sourceArray: key, sourceIndex: 0, destinationArray: kPart1, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 8, destinationArray: kPart2, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 16, destinationArray: kPart3, destinationIndex: 0, length: 8);

                        keys = new[]
                        {
                        kPart1,
                        kPart2,
                        kPart3
                    };
                    }
                    break;
                default:
                    throw new ArgumentException(message: nameof(key), innerException: new Exception(message: "Invalid Key Length"));
            }


            return
                EncryptWithDes(
                    data: DecryptWithDes(
                        data: EncryptWithDes(
                            data, key: keys[0], iv: iv, mode: mode, padding: padding),
                        key: keys[1], iv: iv, mode: mode, padding: padding),
                    key: keys[2], iv: iv, mode: mode, padding: padding);
        }

        public static byte[] DecryptWithTripleDes2(this byte[] data, byte[] key, byte[] iv, CipherMode mode,
            PaddingMode padding)
        {
            byte[][] keys;
            switch (key.Length)
            {
                case 8:
                    keys = new[]
                    {
                        key,
                        key,
                        key
                    };
                    break;
                case 16:
                    {
                        var kPart1 = new byte[8];
                        var kPart2 = new byte[8];

                        Array.Copy(sourceArray: key, sourceIndex: 0, destinationArray: kPart1, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 8, destinationArray: kPart2, destinationIndex: 0, length: 8);

                        keys = new[]
                        {
                        kPart1,
                        kPart2,
                        kPart1
                    };
                    }
                    break;
                case 24:
                    {
                        var kPart1 = new byte[8];
                        var kPart2 = new byte[8];
                        var kPart3 = new byte[8];

                        Array.Copy(sourceArray: key, sourceIndex: 0, destinationArray: kPart1, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 8, destinationArray: kPart2, destinationIndex: 0, length: 8);
                        Array.Copy(sourceArray: key, sourceIndex: 16, destinationArray: kPart3, destinationIndex: 0, length: 8);

                        keys = new[]
                        {
                        kPart1,
                        kPart2,
                        kPart3
                    };
                    }
                    break;
                default:
                    throw new ArgumentException(message: nameof(key), innerException: new Exception(message: "Invalid Key Length"));
            }


            return
                DecryptWithDes(
                    data: EncryptWithDes(
                        data: DecryptWithDes(
                            data, key: keys[0], iv: iv, mode: mode, padding: padding),
                        key: keys[1], iv: iv, mode: mode, padding: padding),
                    key: keys[2], iv: iv, mode: mode, padding: padding);
        }

        //public static byte[] GeneratePinBlock(this string pin, string pan)
        //{
        //    var l1 = $"0{pin.Length:X}{pin}".PadRight(totalWidth: 16, paddingChar: 'F').HexStringToByteArray();
        //    var l2 = $"0000{pan.Substring(startIndex: pan.Length - (1 + 12), length: 12)}".HexStringToByteArray();
        //    var result = new byte[8];
        //    for (var i = 0; i < l1.Length; i++)
        //        result[i] = (byte)(l1[i] ^ l2[i]);
        //    return result;
        //}

        public static string CalculateSha256(this string pan)
        {
            var buffer = pan.HexStringToByteArray();
            return SHA256.Create().ComputeHash(buffer).ByteArrayToHexString();
        }

        public static string ToJsonString<T>(this T obj)
            where T : class
            => obj == null ? null : Encoding.UTF8.GetString(bytes: obj.ToJson());

        public static byte[] ToJson<T>(this T obj)
            where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static T FromJsonString<T>(this string jsonString) where T : class
            => Encoding.UTF8.GetBytes(jsonString).FromJson<T>();

        public static T FromJson<T>(this byte[] jsonBuffer) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream(jsonBuffer))
                return serializer.ReadObject(memoryStream) as T;
        }


        public static string PreparePhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

            if (phoneNumber.Length == 11 && phoneNumber.StartsWith("09"))
                phoneNumber = $"98{phoneNumber.Substring(1)}";
            if (phoneNumber.Length == 10 && phoneNumber.StartsWith("9"))
                phoneNumber = $"98{phoneNumber}";

            if (!phoneNumber.IsMobileNumber())
                phoneNumber = null;

            return phoneNumber;
        }


        public static string ExceptionToString(this Exception e)
        {
            if (e == null) return null;
            return $"\r\n{e.Message}\r\n\r\nStack Trace : {e.StackTrace}{(e.InnerException?.ExceptionToString() ?? string.Empty)}";
        }

    }
}