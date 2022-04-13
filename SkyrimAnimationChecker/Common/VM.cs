using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class VM : PropertyHandler
    {
        public VM()
        {
            //VMs = new() { GENERAL, VM_V3BA };
            Load();
        }
        public VM_GENERAL GENERAL { get; set; } = new();
        public VM_Vbreast Vbreast { get; set; } = new();

        //public static ref VM GETVM<T>() { return ref _VM; }

        [System.Text.Json.Serialization.JsonIgnore]
        private object[] VMs => Values;
        private bool NewVMs(object[]? o = null)
        {
            if (o != null)
            {
                if (o[0] is System.Text.Json.JsonElement)
                    GENERAL = System.Text.Json.JsonSerializer.Deserialize<VM_GENERAL>((System.Text.Json.JsonElement)o[0]) ?? new();
                if (o[1] is System.Text.Json.JsonElement)
                    Vbreast = System.Text.Json.JsonSerializer.Deserialize<VM_Vbreast>((System.Text.Json.JsonElement)o[1]) ?? new();
                return true;
            }
            //VMs = new() { GENERAL, VM_V3BA };
            return false;
        }


        [System.Text.Json.Serialization.JsonIgnore]
        private static string vmFilePath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyrimAnimationCheckerConfig.json");

        [System.Text.Json.Serialization.JsonIgnore]
        public static int LoadCount = 0;

        public void Save()
        {
            using (System.IO.StreamWriter sw = new(vmFilePath, false))
            {
                System.Text.Json.JsonSerializer.Serialize(sw.BaseStream, VMs, VMs.GetType(), new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
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
                    try { buffer = System.Text.Json.JsonSerializer.Deserialize(sr.BaseStream, VMs.GetType()); }
                    catch (System.Text.Json.JsonException) { }
                }
                return NewVMs((object[]?)buffer);
            }
            return false;
        }

    }
}
