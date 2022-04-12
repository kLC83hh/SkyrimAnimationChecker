using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class physics_object : Notify.NotifyPropertyChanged
    {
        public physics_object() { }
        public physics_object(string name, string key, params double[] value)
        {
            Use = true;
            Name = name;
            Key = key;
            _ = SetValue(value);
        }

        public delegate void ValueUpdateEventHandler(physics_object o);
        public event ValueUpdateEventHandler? ValueUpdated;
        protected void Set(double value, [System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (Get<double>(name) != value)
            {
                base.Set(value, name);
                ValueUpdated?.Invoke(this);
            }
        }

        public bool Use { get => Get<bool>(); set => Set(value); }
        public string Name { get => Get<string>(); set => Set(value); }
        public string Key { get => Get<string>(); set => Set(value); }
        public double Value0 { get => Get<double>(); set => Set(value); }
        public double Value1 { get => Get<double>(); set => Set(value); }

        public double[] Values => new double[] { Value0, Value1 };
        public bool SetValue(params double[] v)
        {
            if (v.Length == 1) { Value0 = v[0]; Value1 = v[0]; }
            else if (v.Length == 2) { Value0 = v[0]; Value1 = v[1]; }
            else return true;
            return false;
        }
    }
}
