using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class cbpc_data_mirrored : PropertyHandler, Icbpc_data_mirrored
    {
        public cbpc_data_mirrored()
            : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "MirrorKeys", "MirrorPairs", "DataType", "DefaultName", "UsingKeys", "IsMirrored", "DefaultMirrorKeys", "DefaultMirrorPairs" }) => Init();
        public cbpc_data_mirrored(string name, physics_object_set? left = null, physics_object_set? right = null)
             : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "MirrorKeys", "MirrorPairs", "DataType", "DefaultName", "UsingKeys", "IsMirrored", "DefaultMirrorKeys", "DefaultMirrorPairs" }) => Init(name, left, right);
        public cbpc_data_mirrored(int num, physics_object_set? left = null, physics_object_set? right = null)
             : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "MirrorKeys", "MirrorPairs", "DataType", "DefaultName", "UsingKeys", "IsMirrored", "DefaultMirrorKeys", "DefaultMirrorPairs" }) => Init(num, left, right);
        private void Init(object? param = null, physics_object_set? left = null, physics_object_set? right = null)
        {
            Name = DefaultName;
            if (param != null)
            {
                if (param is string s) Name = NameParser(s);
                if (param is int n) Name = NumSideParser(n);
            }
            Left = left ?? new("L");
            Right = right ?? new("R");
            IsMirrored = true;
            Left.ValueUpdated += (o) => { if (IsMirrored) Mirror(o); };
            Right.ValueUpdated += (o) => { if (IsMirrored) Mirror(o); };
        }

        public virtual string DataType => "mirrored";
        protected virtual string DefaultName => "MirroredData";

        protected virtual string NameParser(string name) => name;
        protected virtual string NumSideParser(int num) => DefaultName;


        public string Name { get => Get<string>(); set { Set(value); NameParser(value); } }
        public string NameShort => GetNameShort();
        /// <summary>
        /// Get a name that stripped bone number and side
        /// </summary>
        /// <param name="name">Fullname</param>
        /// <returns></returns>
        public virtual string GetNameShort(string? name = null)
        {
            string n = name ?? Name;
            if (n.StartsWith("L") || n.StartsWith("R")) n = n.Substring(1);
            if (n.EndsWith("L") || n.EndsWith("R")) n = n.Substring(0, n.Length - 1);
            if (n.EndsWith("1") || n.EndsWith("2") || n.EndsWith("3")) n = n.Substring(0, n.Length - 1);
            if (n.EndsWith("L") || n.EndsWith("R")) n = n.Substring(0, n.Length - 1);
            return n;
        }

        /// <summary>
        /// Bone number for multiple bone
        /// </summary>
        public int Number { get => Get<int>(); set => Set(value); }



        public physics_object_set Left { get => Get<physics_object_set>(); set => Set(value); }
        public physics_object_set Right { get => Get<physics_object_set>(); set => Set(value); }



        public virtual physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "ExtraBreast1L": return Left;
                case "ExtraBreast2L": return Left;
                case "ExtraBreast3L": return Left;
                case "ExtraBreast1R": return Right;
                case "ExtraBreast2R": return Right;
                case "ExtraBreast3R": return Right;
                case "LBreast": return Left;
                case "RBreast": return Right;
                case "LButt": return Left;
                case "RButt": return Right;
                case "LFrontThigh": return Left;
                case "LRearThigh": return Left;
                case "LRearCalf": return Left;
                case "RFrontThigh": return Right;
                case "RRearThigh": return Right;
                case "RRearCalf": return Right;
                case "LLabia": return Left;
                case "RLabia": return Right;
            }
            return null;
        }


        private string[] _UsingKeys = Array.Empty<string>();
        public string[] UsingKeys
        {
            get
            {
                if (_UsingKeys.Length > 0) return _UsingKeys;
                else { GetUsingKeys(); return _UsingKeys; }
            }
        }
        public void GetUsingKeys()
        {
            List<string> keys = new();
            foreach (var item in Left.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            foreach (var item in Right.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            _UsingKeys = (from propName in keys
                          orderby KeysOrderComparer(propName, Left.KeysOrder), propName
                          select propName).ToArray();
        }


        public bool IsMirrored { get => Get<bool>(); set => Set(value); }
        protected void Mirror(physics_object o) => Find(MirrorName(o.Name))?.SetPhysics(GetPair(o.Key), CanMirror(o.Key) ? MirrorValue(o.Values) : o.Values);

        protected string MirrorName(string name)
        {
            string m_name = name;
            if (m_name.StartsWith("L")) m_name = "R" + m_name.Substring(1);
            else if (m_name.StartsWith("R")) m_name = "L" + m_name.Substring(1);
            else if (m_name.EndsWith("L")) m_name = m_name.Substring(0, m_name.Length - 1) + "R";
            else if (m_name.EndsWith("R")) m_name = m_name.Substring(0, m_name.Length - 1) + "L";
            return m_name;
        }
        protected double MirrorValue(double value)
        {
            if (value == 0) return 0;
            else return -value;
        }
        protected double[] MirrorValue(double[] value)
        {
            double[] buffer = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == 0) buffer[i] = 0;
                else buffer[i] = -value[i];
            }
            return buffer;
        }

        public string[] DefaultMirrorKeys => new string[] {
            // linear
            "Xmaxoffset", "Xminoffset",
            "linearXspreadforceY", "linearXspreadforceZ", "linearYspreadforceX", "linearZspreadforceX",
            // rotaion "linearZrotationY", "rotationXspreadforceZ",
            "YmaxoffsetRot", "YminoffsetRot", "ZmaxoffsetRot", "ZminoffsetRot",
            "rotationalZ", "linearYrotationZ",
            "rotationXspreadforceY", "rotationYspreadforceZ", "rotationZspreadforceX", "rotationZspreadforceY",
            "linearXspreadforceYRot", "linearYspreadforceZRot", "linearZspreadforceXRot", "linearZspreadforceYRot",// deprecated, backward compatable
            // collision
            "collisionXmaxoffset", "collisionXminoffset"
        };
        protected string[]? _MirrorKeys;
        public string[] MirrorKeys
        {
            get { if (_MirrorKeys == null) { _MirrorKeys = DefaultMirrorKeys; } return _MirrorKeys; }
            set { _MirrorKeys = value; OnPropertyChanged(); }
        }
        protected bool CanMirror(string key)
        {
            foreach (string s in MirrorKeys)
            {
                if (key == s)
                {
                    if (key == "linearYrotationZ")
                    {
                        if (Left.rotationalZ.Use || Right.rotationalZ.Use) return false;
                        else return true;
                    }
                    return true;
                }
            }
            return false;
        }

        public MirrorPair[] DefaultMirrorPairs => new MirrorPair[] {
            new MirrorPair("Xmaxoffset", "Xminoffset"),
            new MirrorPair("YmaxoffsetRot", "YminoffsetRot"),
            new MirrorPair("ZmaxoffsetRot", "ZminoffsetRot"),
            new MirrorPair("collisionXmaxoffset", "collisionXminoffset")
        };
        protected MirrorPair[]? _MirrorPairs;
        public MirrorPair[] MirrorPairs
        {
            get { if (_MirrorPairs == null) { _MirrorPairs = DefaultMirrorPairs; } return _MirrorPairs; }
            set { _MirrorPairs = value; OnPropertyChanged(); }
        }
        protected string GetPair(string key)
        {
            foreach (MirrorPair s in MirrorPairs)
            {
                if (s.In(key)) return s.Get(key);
            }
            return key;
        }

    }


    public class MirrorPair : PropertyHandler, IPropertyHandler
    {
        public MirrorPair(string key0, string key1) : base()
        {
            Key0 = key0;
            Key1 = key1;
        }

        public string Get(string key)
        {
            if (key == Key0) return Key1;
            else if (key == Key1) return Key0;
            else return key;
        }
        public bool In(string key)
        {
            if (key == Key0 || key == Key1) return true;
            else return false;
        }



        protected string Key0 { get => Get<string>(); set => Set(value); }
        protected string Key1 { get => Get<string>(); set => Set(value); }

        /// <summary>
        /// return <c>true</c> when failed to parse
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out MirrorPair? p)
        {
            if (s.StartsWith('{') && s.EndsWith('}'))
            {
                string[] vs = s.Split(',');
                if (vs.Length == 2)
                {
                    p = new MirrorPair(vs[0], vs[1]);
                    return false;
                }
            }
            p = null;
            return true;
        }
        public override string ToString() => $"{{{Key0},{Key1}}}";
        public static explicit operator string(MirrorPair obj) => obj.ToString();
    }
}
