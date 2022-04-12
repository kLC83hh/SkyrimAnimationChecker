using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SkyrimAnimationChecker.CBPC
{
    public class breast_3ba_object : Common.PropertyHandler
    {
        public breast_3ba_object() : base(KeysException: new string[] { "Mirrorable" }, AddibleKeysException: true)
        {
            L1 = new(1, CBPC_Breast.Sides.L);
            L2 = new(2, CBPC_Breast.Sides.L);
            L3 = new(3, CBPC_Breast.Sides.L);
            R1 = new(1, CBPC_Breast.Sides.R);
            R2 = new(2, CBPC_Breast.Sides.R);
            R3 = new(3, CBPC_Breast.Sides.R);

            foreach (var item in Values)
            {
                item.ValueUpdated += (o) => Mirror(o);
            }
        }

        public CBPC_Breast? Find(string name)
        {
            switch (name)
            {
                case "ExtraBreast1L": return L1;
                case "ExtraBreast2L": return L2;
                case "ExtraBreast3L": return L3;
                case "ExtraBreast1R": return R1;
                case "ExtraBreast2R": return R2;
                case "ExtraBreast3R": return R3;
            }
            return null;
        }
        public void SetTo(CBPC_Breast br, string key, double value, bool mirror = false)
        {
            Find(br.Name)?.Data.SetPhysics(key, value);
            if (mirror)
                Find(MirrorName(br.Name))?.Data.SetPhysics(key, IsMirrorable(key) ? MirrorValue(value) : value);
        }
        public void SetTo(CBPC_Breast br, string key, double[] value, bool mirror = false)
        {
            Find(br.Name)?.Data.SetPhysics(key, value);
            if (mirror)
                Find(MirrorName(br.Name))?.Data.SetPhysics(key, IsMirrorable(key) ? MirrorValue(value) : value);
        }
        public void SetTo(string name, string key, double value, bool mirror = false)
        {
            Find(name)?.Data.SetPhysics(key, value);
            if (mirror)
                Find(MirrorName(name))?.Data.SetPhysics(key, IsMirrorable(key) ? MirrorValue(value) : value);
        }
        public void SetTo(string name, string key, double[] value, bool mirror = false)
        {
            Find(name)?.Data.SetPhysics(key, value);
            if (mirror)
                Find(MirrorName(name))?.Data.SetPhysics(key, IsMirrorable(key) ? MirrorValue(value) : value);
        }
        private void Mirror(Common.physics_object o)
        {
            //M.D("Mirror");
            Find(MirrorName(o.Name))?.Data.SetPhysics(o.Key, IsMirrorable(o.Key) ? MirrorValue(o.Values) : o.Values);
        }
        private string MirrorName(string name)
        {
            string m_name = name;
            if (m_name.EndsWith("L")) m_name = m_name.Substring(0, m_name.Length - 1) + "R";
            else if (m_name.EndsWith("R")) m_name = m_name.Substring(0, m_name.Length - 1) + "L";
            return m_name;
        }
        private double MirrorValue(double value)
        {
            if (value == 0) return 0;
            else return -value;
        }
        private double[] MirrorValue(double[] value) {
            double[] buffer = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == 0) buffer[i] = 0;
                else buffer[i] = -value[i];
            }
            return buffer;
        }
        private string[] _Mirrorable = new string[] { "rotationalZ", "linearZrotationY", "linearXspreadforceZ", "linearYspreadforceX", "linearZspreadforceX" };
        public string[] Mirrorable { get => _Mirrorable; set { _Mirrorable = value; OnPropertyChanged(); } }
        private bool IsMirrorable(string key)
        {
            foreach (string s in Mirrorable)
            {
                if (s == key) return true;
            }
            return false;
        }


        public CBPC_Breast L1 { get => Get<CBPC_Breast>(); set => Set(value); }
        public CBPC_Breast L2 { get => Get<CBPC_Breast>(); set => Set(value); }
        public CBPC_Breast L3 { get => Get<CBPC_Breast>(); set => Set(value); }
        public CBPC_Breast R1 { get => Get<CBPC_Breast>(); set => Set(value); }
        public CBPC_Breast R2 { get => Get<CBPC_Breast>(); set => Set(value); }
        public CBPC_Breast R3 { get => Get<CBPC_Breast>(); set => Set(value); }


        public new CBPC_Breast[] Values => GetPropertyHandleValues<CBPC_Breast>();
        //public CBPC_Breast GetPropValue(string key) => (CBPC_Breast)(GetType().GetProperty(key)?.GetValue(this) ?? new CBPC_Breast());
        //public T GetPropValue<T>(string key) => (T)(GetType().GetProperty(key)?.GetValue(this) ?? new());
    }

}
