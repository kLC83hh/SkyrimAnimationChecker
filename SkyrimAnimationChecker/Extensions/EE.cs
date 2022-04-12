using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal static class EE
    {
        internal static Dictionary<int, string> List = new()
        {
            { 0, "OK" },
            { 1001, "Combine: Invalid data: Length of both outputs are different." },
            { 1002, "Combine: Invalid data: Write statuses are different." },
            { 1003, "Combine: Invalid data: Group statuses are different." },
            { 1004, "Combine: Invalid data: Sphere names are different." },
            { 2001, "Can not retrieve CBPC Config data." },
            { 12001, "Invalid CBPC Physics location" }
        };
        internal static Exception New(int code, string? msg = null, System.Diagnostics.StackTrace? trace = null)
        {
            if (List[code] != msg) msg = $"{List[code]}{Environment.NewLine}{msg}";
            else if (List[code] == null && msg == null) msg = trace?.ToString();
            else msg = List[code];
            var e = new Exception($"[{code}]{Environment.NewLine}{msg}");
            e.Data.Add("Code", code);
            return e;
        }
        internal static (int code, string msg) Parse(Exception e)
        {
            return ((int)(e.Data["Code"] ?? -1), e.Message);
        }
    }
}
