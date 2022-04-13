using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class cc_options_object : PropertyHandler
    {
        public cc_options_object() : base(KeysIgnore: new string[] { "Use" }) { }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool Use { get => Get<bool>(); set => Set(value); }

        public string Conditions { get => Get<string>(); set => Set(value); }
        public int Priority { get => Get<int>(); set => Set(value); }
    }
}
