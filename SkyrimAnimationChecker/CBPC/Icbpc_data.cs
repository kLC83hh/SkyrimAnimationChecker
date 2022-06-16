namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public interface Icbpc_data : Common.IPropertyHandler
    {
        public virtual string DataType => "single";

        /// <summary>
        /// Fullname
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A name that stripped bone number and side
        /// </summary>
        public string NameShort { get; }

        public Common.physics_object_set? Find(string name);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public interface Icbpc_data_mirrored : Icbpc_data
    {
        public virtual string[] DefaultMirrorKeys => new string[] {
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
        public virtual MirrorPair[] DefaultMirrorPairs => new MirrorPair[] {
            new MirrorPair("Xmaxoffset", "Xminoffset"),
            new MirrorPair("YmaxoffsetRot", "YminoffsetRot"),
            new MirrorPair("ZmaxoffsetRot", "ZminoffsetRot"),
            new MirrorPair("collisionXmaxoffset", "collisionXminoffset")
        };
        public string[] MirrorKeys { get; set; }
        public MirrorPair[] MirrorPairs { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public interface Icbpc_data_multibone : Icbpc_data_mirrored
    {
        public abstract Icbpc_data GetData(int? num = null);
        public string[] UsingKeys { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public interface Icbpc_breast_data : Icbpc_data_multibone
    {
        //public abstract Icbpc_data_mirrored GetData(int? num = null);
        //public (Common.physics_object[] Left, Common.physics_object[] Right) GetUsingValues(string name);
    }
}
