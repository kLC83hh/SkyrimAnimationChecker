using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public partial class cbpc_breast : Notify.NotifyPropertyChanged
    {
        public cbpc_breast() => Init();
        public cbpc_breast(string name, physics_object_set? left = null, physics_object_set? right = null) => Init(name, left, right);
        public cbpc_breast(int num, physics_object_set? left = null, physics_object_set? right = null) => Init(num, left, right);
        private void Init(object? param = null, physics_object_set? left = null, physics_object_set? right = null)
        {
            if (param != null)
            {
                if (param is string s) Name = NameParser(s);
                if (param is int n) Name = NumSideParser(n);
            }
            Left = left ?? new("L");
            Right = right ?? new("R");
            Left.ValueUpdated += (o) => Mirror(o);
            Right.ValueUpdated += (o) => Mirror(o);
        }

        private string NameParser(string name)
        {
            if (name.Contains('1')) Number = 1;
            else if (name.Contains('2')) Number = 2;
            else if (name.Contains('3')) Number = 3;
            else Number = 0;
            //else throw EE.New(2902);

            //if (name.StartsWith('L') || name.EndsWith('L')) Side = Sides.L;
            //else if (name.StartsWith('R') || name.EndsWith('R')) Side = Sides.R;
            //else throw EE.New(2901);

            return name;
        }
        private string NumSideParser(int num)
        {
            if (num == 0) return "Breast";
            if (num == 1 || num == 2 || num == 3)
            {
                return $"ExtraBreast{num}";
            }
            throw EE.New(2902);
        }

        /// <summary>
        /// Fullname
        /// </summary>
        public string Name { get => Get<string>(); set { Set(value); NameParser(value); } }
        /// <summary>
        /// A name that stripped bone number and side
        /// </summary>
        public string NameShort => GetNameShort();
        /// <summary>
        /// Get a name that stripped bone number and side
        /// </summary>
        /// <param name="name">Fullname</param>
        /// <returns></returns>
        public string GetNameShort(string? name = null)
        {
            string n = name ?? Name;
            if (n.StartsWith("L") || n.StartsWith("R")) n = n.Substring(1);
            if (n.EndsWith("L") || n.EndsWith("R")) n = n.Substring(0, n.Length - 1);
            if (n.EndsWith("1") || n.EndsWith("2") || n.EndsWith("3")) n = n.Substring(0, n.Length - 1);
            if (n.EndsWith("L") || n.EndsWith("R")) n = n.Substring(0, n.Length - 1);
            return n;
        }
        /// <summary>
        /// Bone number for 3ba
        /// </summary>
        public int Number { get => Get<int>(); set => Set(value); }


        public physics_object_set Left { get=>Get<physics_object_set>(); set => Set(value); }
        public physics_object_set Right { get => Get<physics_object_set>(); set => Set(value); }



    }
    public partial class cbpc_breast
    {
        private void Mirror(physics_object o) => Find(MirrorName(o.Name))?.SetPhysics(o.Key, CanMirror(o.Key) ? MirrorValue(o.Values) : o.Values);
        public physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "LBreast": return Left;
                case "ExtraBreast1L": return Left;
                case "ExtraBreast2L": return Left;
                case "ExtraBreast3L": return Left;
                case "RBreast": return Right;
                case "ExtraBreast1R": return Right;
                case "ExtraBreast2R": return Right;
                case "ExtraBreast3R": return Right;
            }
            return null;
        }
        private string MirrorName(string name)
        {
            string m_name = name;
            if (m_name.StartsWith("L")) m_name = "R" + m_name.Substring(1);
            else if (m_name.StartsWith("R")) m_name = "L" + m_name.Substring(1);
            else if (m_name.EndsWith("L")) m_name = m_name.Substring(0, m_name.Length - 1) + "R";
            else if (m_name.EndsWith("R")) m_name = m_name.Substring(0, m_name.Length - 1) + "L";
            return m_name;
        }
        private double MirrorValue(double value)
        {
            if (value == 0) return 0;
            else return -value;
        }
        private double[] MirrorValue(double[] value)
        {
            double[] buffer = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == 0) buffer[i] = 0;
                else buffer[i] = -value[i];
            }
            return buffer;
        }
        private string[] _MirrorKeys = new string[] { "rotationalZ", "linearZrotationY", "linearXspreadforceZ", "linearYspreadforceX", "linearZspreadforceX" };
        public string[] MirrorKeys { get => _MirrorKeys; set { _MirrorKeys = value; OnPropertyChanged(); } }
        private bool CanMirror(string key)
        {
            foreach (string s in MirrorKeys)
            {
                if (s == key) return true;
            }
            return false;
        }

    }
}
