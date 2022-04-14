using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class cc_extraoptions_object : PropertyHandler
    {
        public cc_extraoptions_object() : base(KeysIgnore: new string[] { "Use" }) { }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool Use { get => Get<bool>(); set => Set(value); }

        public double BellyBulge { get => Get<double>(); set => Set(value); }
        public double BellyBulgeMax { get => Get<double>(); set => Set(value); }
        public double BellyBulgePosLowest { get => Get<double>(); set => Set(value); }
        public double BellyBulgeReturnTime { get => Get<double>(); set => Set(value); }

        public double VaginaOpeningMultiplier { get => Get<double>(); set => Set(value); }
        public double VaginaOpeningLimit { get => Get<double>(); set => Set(value); }

        public double Find(string key) => PropertyHandleGetValue<double>(key);
    }
}
