using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.Common
{
    public class VM : PropertyHandler
    {
        public VM() => Load();
        public VM_GENERAL GENERAL { get; set; } = new();
        public VM_Vbreast Vbreast { get; set; } = new();
        public VM_VPhysics Vphysics { get; set; } = new();


        [JsonIgnore]
        private object[] VMs => Values;
        private bool NewVMs(object[]? o = null)
        {
            if (o != null)
            {
                for (int i = 0; i < o.Length; i++)
                {
                    if (o[i] is JsonElement v)
                    {
                        switch (i)
                        {
                            case 0:
                                GENERAL = JsonSerializer.Deserialize<VM_GENERAL>(v) ?? new();
                                break;
                            case 1:
                                Vbreast = JsonSerializer.Deserialize<VM_Vbreast>(v) ?? new();
                                break;
                            case 2:
                                Vphysics = JsonSerializer.Deserialize<VM_VPhysics>(v) ?? new();
                                break;
                        }
                    }
                }
                //if (o[0] is JsonElement v)
                //    GENERAL = JsonSerializer.Deserialize<VM_GENERAL>(v) ?? new();
                //if (o[1] is JsonElement vb)
                //    Vbreast = JsonSerializer.Deserialize<VM_Vbreast>(vb) ?? new();
                //if (o[2] is JsonElement vp)
                //    Vphysics = JsonSerializer.Deserialize<VM_VPhysics>(vp) ?? new();
                return true;
            }
            return false;
        }


        #region Save to file
        [JsonIgnore]
        private static string vmFilePath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyrimAnimationCheckerConfig.json");

        [JsonIgnore]
        public static int LoadCount = 0;

        public void Save()
        {
            using (System.IO.StreamWriter sw = new(vmFilePath, false))
            {
                JsonSerializer.Serialize(sw.BaseStream, VMs, VMs.GetType(), new JsonSerializerOptions() { WriteIndented = true });
                sw.Flush();
            }
        }
        public bool Load<T>(ref T o, bool force = false)
        {
            bool result = Load(force);
            o = (T)VMs.First(x => x is T);
            return result;
        }
        public bool Load(bool force = false)
        {
            if (LoadCount > 0 && !force) return true;
            LoadCount++;
            if (System.IO.File.Exists(vmFilePath))
            {
                object? buffer = null;
                using (System.IO.StreamReader sr = new(vmFilePath))
                {
                    try { buffer = JsonSerializer.Deserialize(sr.BaseStream, VMs.GetType()); }
                    catch (JsonException) { }
                }
                return NewVMs((object[]?)buffer);
            }
            return false;
        }
        #endregion

    }
}
