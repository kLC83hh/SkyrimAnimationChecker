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
            { 101, "VM load failed" },
            { 102, "Conversion error occurred at VM loading" },
            // 1000 nif
            { 1001, $"NIF: Colliders: Combine: {Environment.NewLine}Invalid data: Length of both outputs are different." },
            { 1002, $"NIF: Colliders: Combine: {Environment.NewLine}Invalid data: Write statuses are different." },
            { 1003, $"NIF: Colliders: Combine: {Environment.NewLine}Invalid data: Group statuses are different." },
            { 1004, $"NIF: Colliders: Combine: {Environment.NewLine}Invalid data: Sphere names are different." },
            { 1011, $"NIF: Make: Incorrect bodyslide file name" },
            // 2000 cbpc physics
            { 2001, $"CBPC: Physics: Parse_Type: {Environment.NewLine}Can not retrieve CBPConfig data." },
            { 2008, $"CBPC: Physics: Parse_Type: {Environment.NewLine}Can not recognize CBPConfig data type." },
            //{ 2009, $"CBPC: Physics: Parse_Type: {Environment.NewLine}Can not recognize CBPConfig data type." },
            //{ 2011, $"CBPC: Physics: Parse_Breast: {Environment.NewLine}Can not recognize CBPConfig data type." },
            { 2101, "CBPC_Physics_Keywords: Can not find keyword name" },
            { 2201, $"CBPC: Physics: Parse3BA:" },
            { 2202, $"CBPC: Physics: Parse3BA:" },
            { 2203, $"CBPC: Physics: Parse3BA:" },
            { 2901, $"cbpc_breast: Can not recognize breast bone side" },
            { 2902, $"cbpc_breast: Can not recognize breast bone number" },
            // 5000 cbpc collision
            { 5001, $"CBPC: CollisionCBPC: Filter: Can not retrieve CBPConfig data." },
            { 5005, $"CBPC: CollisionCBPC: GetColliders: Can not retrieve CBPConfig data." },
            { 5011, $"CBPC: CollisionCBPC: MakeOrganized: Can not retrieve CBPConfig data." },
            { 5201, $"CBPC: CollisionCBPC: Option: Can not retrieve CBPConfig data." },
            { 5202, $"CBPC: CollisionCBPC: Option: Can not convert priority value to int" },
            { 5203, $"CBPC: CollisionCBPC: Option: Can not convert extraoptions to double" },
            // 11000 mainwindow nif
            // 12000 mainwindow cbpc
            { 12001, $"Invalid CBPC Physics location" }
        };
        internal static Exception New(int code, string? msg = null, System.Diagnostics.StackTrace? trace = null, [System.Runtime.CompilerServices.CallerMemberName] string? caller = null)
        {
            if (List[code] != msg)
            {
                if (!string.IsNullOrWhiteSpace(List[code]))
                    msg = $"{List[code]}{Environment.NewLine}{msg}";
            }
            else if (List[code] == null && msg == null) msg = trace?.ToString();
            else msg = List[code];
            if (caller != null) msg += $"{Environment.NewLine}Caller: {caller}";
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
