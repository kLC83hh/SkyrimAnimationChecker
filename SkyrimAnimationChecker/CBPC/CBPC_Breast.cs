using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.CBPC
{
    public class CBPC_Breast : Notify.NotifyPropertyChanged
    {
        public CBPC_Breast()
        {
            _Data = new();
            _Data.ValueUpdated += (o) => ValueUpdated?.Invoke(o);
        }
        public CBPC_Breast(string name, physics_object_set? data = null)
        {
            Name = NameParser(name);
            _Data = data ?? new();
            _Data.ValueUpdated += (o) => ValueUpdated?.Invoke(o);
        }
        public CBPC_Breast(int num, Sides side, physics_object_set? data = null)
        {
            Name = NumSideParser(num, side);
            _Data = data ?? new();
            _Data.ValueUpdated += (o) => ValueUpdated?.Invoke(o);
        }
        public delegate void ValueUpdateEventHandler(Common.physics_object o);
        public event ValueUpdateEventHandler? ValueUpdated;

        private string NameParser(string name)
        {
            if (name.Contains('1')) Number = 1;
            else if (name.Contains('2')) Number = 2;
            else if (name.Contains('3')) Number = 3;
            else throw new Exception("Can not recognize breast bone number");

            if (name.EndsWith('L')) Side = Sides.L;
            else if (name.EndsWith('R')) Side = Sides.R;
            else throw new Exception("Can not recognize breast bone side");

            return name;
        }
        private string NumSideParser(int num, Sides side)
        {
            if (num == 1 || num == 2 || num == 3)
            {
                string name = $"ExtraBreast{num}";
                switch (side)
                {
                    case Sides.L: return name + "L";
                    case Sides.R: return name + "R";
                }
                throw new Exception("Can not recognize breast bone side");
            }
            throw new Exception("Can not recognize breast bone number");
        }

        public string Name { get => Get<string>(); set { Set(value); NameParser(value); } }
        public string NameShort => GetNameShort();
        private string GetNameShort()
        {
            string n = Name;
            if (n.EndsWith("L") || n.EndsWith("R")) n = n.Substring(0, n.Length - 1);
            if (n.EndsWith("1") || n.EndsWith("2") || n.EndsWith("3")) n = n.Substring(0, n.Length - 1);
            return n;
        }

        public int Number { get => Get<int>(); set => Set(value); }

        public enum Sides { L, R }
        public Sides Side { get => Get<Sides>(); set => Set(value); }

        public physics_object_set _Data;
        public physics_object_set Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged(); }
        }
    }

}
