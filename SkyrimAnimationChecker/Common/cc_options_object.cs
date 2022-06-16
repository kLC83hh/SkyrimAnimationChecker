namespace SkyrimAnimationChecker.Common
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cc_options_object : PropertyHandler
    {
        public cc_options_object() : base(KeysIgnore: new string[] { "Use" }) { }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool Use { get => Get<bool>(); set => Set(value); }

        public string Conditions { get => Get<string>(); set => Set(value); }
        public int Priority { get => Get<int>(); set => Set(value); }
    }
}
