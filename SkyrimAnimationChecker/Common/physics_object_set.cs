namespace SkyrimAnimationChecker.Common
{
    public partial class physics_object_set : PropertyHandler
    {
        // Property Handling

        public delegate void ValueUpdateEventHandler(physics_object o);
        public event ValueUpdateEventHandler? ValueUpdated;
        protected void Set(physics_object o, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            o.ValueUpdated += (o) => ValueUpdated?.Invoke(o);
            base.Set(o, name);
        }



        #region Values
        public new physics_object[] Values => PropertyHandleGetValues<physics_object>();
        #endregion

        #region Property Handling
        public physics_object GetObject(string key) => (physics_object)(GetType().GetProperty(key)?.GetValue(this) ?? new physics_object(string.Empty, key, 0, 0));
        public void SetObject(string key, physics_object data) => this.GetType().GetProperty(key)?.SetValue(this, data, null);

        public double[] GetPhysics(string key) => GetObject(key).Values;
        public bool SetPhysics(string key, params double[] data) => GetObject(key).SetValue(data);
        #endregion

    }
    public partial class physics_object_set : PropertyHandler
    {
        // Defaults
        public physics_object_set() : base(
            KeysIgnore: new string[] { "Name" },
            KeysOrder: new PropertyOrder[] {
                new PropertyOrder(true, "collision" ),
                new PropertyOrder(false, "stiffness$", "stiffness[XYZxyz]", "stiffness2$", "stiffness2[XYZxyz]", "damping$", "damping[XYZxyz]", "(max|min)offset", "timetick", "timeStep", "linear[XYZxyz]$", "rotational", "linear[XYZxyz]rotation", "spreadforce", "Clothed", "LightArmored", "HeavyArmored" ),
                new PropertyOrder(true, "start", "end" )
            }
            ) => Defaults();
        public physics_object_set(string name) : base(
            KeysIgnore: new string[] { "Name" },
            KeysOrder: new PropertyOrder[] {
                new PropertyOrder(true, "collision" ),
                new PropertyOrder(false, "stiffness$", "stiffness[XYZxyz]", "stiffness2$", "stiffness2[XYZxyz]", "damping$", "damping[XYZxyz]", "(max|min)offset", "timetick", "timeStep", "linear[XYZxyz]$", "rotational", "linear[XYZxyz]rotation", "spreadforce", "Clothed", "LightArmored", "HeavyArmored" ),
                new PropertyOrder(true, "start", "end" )
            }
            ) => Defaults(name);
        private void Defaults(string? name = null)
        {
            if (name != null) Name = name;
            Default_Collision();
            Default_Normal();
            Default_3b();
            Default_n3b_add();
            Default_3b_Gravities();
            Default_3b_armor();

        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Name { get => Get<string>(); set => Set(value); }
        public override string ToString() { return Name; }
    }
    
    // part
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_()
        {
            //stiffness = new physics_object();
        }
        #region Declaration
        //public physics_object stiffness { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }

    // collision
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_Collision()
        {
            collisionXmaxoffset = new physics_object("collisionXmaxoffset");
            collisionXminoffset = new physics_object("collisionXminoffset");
            collisionYmaxoffset = new physics_object("collisionYmaxoffset");
            collisionYminoffset = new physics_object("collisionYminoffset");
            collisionZmaxoffset = new physics_object("collisionZmaxoffset");
            collisionZminoffset = new physics_object("collisionZminoffset");

            collisionFriction = new physics_object("collisionFriction");
            collisionPenetration = new physics_object("collisionPenetration");
            collisionMultipler = new physics_object("collisionMultipler");
            collisionMultiplerRot = new physics_object("collisionMultiplerRot");
            collisionElastic = new physics_object("collisionElastic");
        }
        #region Declaration
        public physics_object collisionXmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionXminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionYmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionYminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionZmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionZminoffset { get => Get<physics_object>(); set => Set(value); }

        public physics_object collisionFriction { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionPenetration { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionMultipler { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionMultiplerRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionElastic { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
    // normal
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_Normal()
        {
            stiffness = new physics_object("stiffness");
            stiffness2 = new physics_object("stiffness2");
            damping = new physics_object("damping");

            timetick = new physics_object("timetick");
            timeStep = new physics_object("timeStep");

            maxoffset = new physics_object("maxoffset");

            forceMultipler = new physics_object("forceMultipler");
        }
        #region Declaration
        public physics_object stiffness { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2 { get => Get<physics_object>(); set => Set(value); }
        public physics_object damping { get => Get<physics_object>(); set => Set(value); }

        public physics_object timetick { get => Get<physics_object>(); set => Set(value); }
        public physics_object timeStep { get => Get<physics_object>(); set => Set(value); }

        public physics_object maxoffset { get => Get<physics_object>(); set => Set(value); }

        public physics_object forceMultipler { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
    // 3b
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_3b()
        {

            Xmaxoffset = new physics_object("Xmaxoffset");
            Xminoffset = new physics_object("Xminoffset");
            Ymaxoffset = new physics_object("Ymaxoffset");
            Yminoffset = new physics_object("Yminoffset");
            Zmaxoffset = new physics_object("Zmaxoffset");
            Zminoffset = new physics_object("Zminoffset");

            linearX = new physics_object("linearX");
            linearY = new physics_object("linearY");
            linearZ = new physics_object("linearZ");

            rotationalX = new physics_object("rotationalX");
            rotationalY = new physics_object("rotationalY");
            rotationalZ = new physics_object("rotationalZ");

            Xdefaultoffset = new physics_object("Xdefaultoffset");
            Ydefaultoffset = new physics_object("Ydefaultoffset");
            Zdefaultoffset = new physics_object("Zdefaultoffset");

            linearXrotationX = new physics_object("linearXrotationX");
            linearXrotationY = new physics_object("linearXrotationY");
            linearXrotationZ = new physics_object("linearXrotationZ");
            linearYrotationX = new physics_object("linearYrotationX");
            linearYrotationY = new physics_object("linearYrotationY");
            linearYrotationZ = new physics_object("linearYrotationZ");
            linearZrotationX = new physics_object("linearZrotationX");
            linearZrotationY = new physics_object("linearZrotationY");
            linearZrotationZ = new physics_object("linearZrotationZ");

            linearXspreadforceY = new physics_object("linearXspreadforceY");
            linearXspreadforceZ = new physics_object("linearXspreadforceZ");
            linearYspreadforceX = new physics_object("linearYspreadforceX");
            linearYspreadforceZ = new physics_object("linearYspreadforceZ");
            linearZspreadforceX = new physics_object("linearZspreadforceX");
            linearZspreadforceY = new physics_object("linearZspreadforceY");

        }

        #region Declaration
        public physics_object Xmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Xminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Ymaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Yminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Zmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Zminoffset { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZ { get => Get<physics_object>(); set => Set(value); }

        public physics_object rotationalX { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationalY { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationalZ { get => Get<physics_object>(); set => Set(value); }

        public physics_object Xdefaultoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Ydefaultoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Zdefaultoffset { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearXrotationX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearXrotationY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearXrotationZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYrotationX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYrotationY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYrotationZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZrotationX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZrotationY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZrotationZ { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearXspreadforceY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearXspreadforceZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYspreadforceX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYspreadforceZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZspreadforceX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZspreadforceY { get => Get<physics_object>(); set => Set(value); }

        #endregion
    }
    // normal-3b additional
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_n3b_add()
        {

            stiffnessX = new physics_object("stiffnessX");
            stiffnessY = new physics_object("stiffnessY");
            stiffnessZ = new physics_object("stiffnessZ");
            stiffnessXRot = new physics_object("stiffnessXRot");
            stiffnessYRot = new physics_object("stiffnessYRot");
            stiffnessZRot = new physics_object("stiffnessZRot");

            stiffness2X = new physics_object("stiffness2X");
            stiffness2Y = new physics_object("stiffness2Y");
            stiffness2Z = new physics_object("stiffness2Z");
            stiffness2XRot = new physics_object("stiffness2XRot");
            stiffness2YRot = new physics_object("stiffness2YRot");
            stiffness2ZRot = new physics_object("stiffness2ZRot");

            dampingX = new physics_object("dampingX");
            dampingY = new physics_object("dampingY");
            dampingZ = new physics_object("dampingZ");
            dampingXRot = new physics_object("dampingXRot");
            dampingYRot = new physics_object("dampingYRot");
            dampingZRot = new physics_object("dampingZRot");

            XmaxoffsetRot = new physics_object("XmaxoffsetRot");
            XminoffsetRot = new physics_object("XminoffsetRot");
            YmaxoffsetRot = new physics_object("YmaxoffsetRot");
            YminoffsetRot = new physics_object("YminoffsetRot");
            ZmaxoffsetRot = new physics_object("ZmaxoffsetRot");
            ZminoffsetRot = new physics_object("ZminoffsetRot");

            timetickRot = new physics_object("timetickRot");
            timeStepRot = new physics_object("timeStepRot");

            linearXspreadforceYRot = new physics_object("linearXspreadforceYRot");// Deprecated
            linearXspreadforceZRot = new physics_object("linearXspreadforceZRot");// Deprecated
            linearYspreadforceXRot = new physics_object("linearYspreadforceXRot");// Deprecated
            linearYspreadforceZRot = new physics_object("linearYspreadforceZRot");// Deprecated
            linearZspreadforceXRot = new physics_object("linearZspreadforceXRot");// Deprecated
            linearZspreadforceYRot = new physics_object("linearZspreadforceYRot");// Deprecated

            rotationXspreadforceY = new physics_object("rotationXspreadforceY");
            rotationXspreadforceZ = new physics_object("rotationXspreadforceZ");
            rotationYspreadforceX = new physics_object("rotationYspreadforceX");
            rotationYspreadforceZ = new physics_object("rotationYspreadforceZ");
            rotationZspreadforceX = new physics_object("rotationZspreadforceX");
            rotationZspreadforceY = new physics_object("rotationZspreadforceY");
        }

        #region Declaration
        // 1.5.x beta2+
        public physics_object stiffnessX { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffnessY { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffnessZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffnessXRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffnessYRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffnessZRot { get => Get<physics_object>(); set => Set(value); }

        public physics_object stiffness2X { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2Y { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2Z { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2XRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2YRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2ZRot { get => Get<physics_object>(); set => Set(value); }

        public physics_object dampingX { get => Get<physics_object>(); set => Set(value); }
        public physics_object dampingY { get => Get<physics_object>(); set => Set(value); }
        public physics_object dampingZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object dampingXRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object dampingYRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object dampingZRot { get => Get<physics_object>(); set => Set(value); }

        public physics_object XmaxoffsetRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object XminoffsetRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object YmaxoffsetRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object YminoffsetRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object ZmaxoffsetRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object ZminoffsetRot { get => Get<physics_object>(); set => Set(value); }

        public physics_object timetickRot { get => Get<physics_object>(); set => Set(value); }
        public physics_object timeStepRot { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearXspreadforceYRot { get => Get<physics_object>(); set => Set(value); }// Deprecated
        public physics_object linearXspreadforceZRot { get => Get<physics_object>(); set => Set(value); }// Deprecated
        public physics_object linearYspreadforceXRot { get => Get<physics_object>(); set => Set(value); }// Deprecated
        public physics_object linearYspreadforceZRot { get => Get<physics_object>(); set => Set(value); }// Deprecated
        public physics_object linearZspreadforceXRot { get => Get<physics_object>(); set => Set(value); }// Deprecated
        public physics_object linearZspreadforceYRot { get => Get<physics_object>(); set => Set(value); }// Deprecated

        public physics_object rotationXspreadforceY { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationXspreadforceZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationYspreadforceX { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationYspreadforceZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationZspreadforceX { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationZspreadforceY { get => Get<physics_object>(); set => Set(value); }

        #endregion
    }
    // 3b_Gravities
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_3b_Gravities()
        {

            gravityBias = new physics_object("gravityBias");
            gravityCorrection = new physics_object("gravityCorrection");

            gravityInvertedCorrection = new physics_object("gravityInvertedCorrection");
            gravityInvertedCorrectionStart = new physics_object("gravityInvertedCorrectionStart");
            gravityInvertedCorrectionEnd = new physics_object("gravityInvertedCorrectionEnd");

        }
        #region Declaration
        public physics_object gravityBias { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityCorrection { get => Get<physics_object>(); set => Set(value); }

        public physics_object gravityInvertedCorrection { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityInvertedCorrectionStart { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityInvertedCorrectionEnd { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
    // 3b_armor
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_3b_armor()
        {
            breastClothedPushup = new physics_object("breastClothedPushup");
            breastLightArmoredPushup = new physics_object("breastLightArmoredPushup");
            breastHeavyArmoredPushup = new physics_object("breastHeavyArmoredPushup");
            breastClothedAmplitude = new physics_object("breastClothedAmplitude");
            breastLightArmoredAmplitude = new physics_object("breastLightArmoredAmplitude");
            breastHeavyArmoredAmplitude = new physics_object("breastHeavyArmoredAmplitude");
        }
        #region Declaration
        public physics_object breastClothedPushup { get => Get<physics_object>(); set => Set(value); }
        public physics_object breastLightArmoredPushup { get => Get<physics_object>(); set => Set(value); }
        public physics_object breastHeavyArmoredPushup { get => Get<physics_object>(); set => Set(value); }
        public physics_object breastClothedAmplitude { get => Get<physics_object>(); set => Set(value); }
        public physics_object breastLightArmoredAmplitude { get => Get<physics_object>(); set => Set(value); }
        public physics_object breastHeavyArmoredAmplitude { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
}
