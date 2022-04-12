using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal static class EF
    {
        internal static Exception NewException(int code, string? msg)
        {
            var e = new Exception(msg);
            e.Data.Add("Code", code);
            return e;
        }
        internal static (int code, string msg) ParseException(Exception e)
        {
            return ((int)(e.Data["Code"] ?? -1), e.Message);
        }
    }
}
