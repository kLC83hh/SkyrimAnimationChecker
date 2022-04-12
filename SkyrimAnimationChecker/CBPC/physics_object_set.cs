using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public partial class physics_object_set : PropertyHandler
    {
        // Property Handling

        public delegate void ValueUpdateEventHandler(physics_object o);
        public event ValueUpdateEventHandler? ValueUpdated;
        protected void Set(physics_object o, [System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            o.ValueUpdated += (o) => ValueUpdated?.Invoke(o);
            base.Set(o, name);
        }



        #region Values
        public new physics_object[] Values => GetPropertyHandleValues<physics_object>();
        #endregion

        #region Property Handling
        public physics_object GetObject(string key) => (physics_object)(GetType().GetProperty(key)?.GetValue(this) ?? new physics_object(String.Empty, key, 0, 0));
        public void SetObject(string key, physics_object data) => this.GetType().GetProperty(key)?.SetValue(this, data, null);

        public double[] GetPhysics(string key) => GetObject(key).Values;
        public bool SetPhysics(string key, params double[] data) => GetObject(key).SetValue(data);
        #endregion

    }
    public partial class physics_object_set : PropertyHandler
    {
        // Defaults
        public physics_object_set() : base(
            KeysOrder: new string[] { "stiffness", "damping", "(max|min)offset", "timetick", "linear[XYZxyz]$", "rotational", "timeStep", "linear[XYZxyz]rotation", "spreadforce", "collision" },
            RegexOrder: true
            )
        {
            Default_3b();
            Default_3b_Gravities();
            Default_3b_armor();

        }
    }
    public partial class physics_object_set : PropertyHandler
    {
        // part
        private void Default_()
        {
            //stiffness = new physics_object();
        }
        #region Declaration
        //public physics_object stiffness { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }

    public partial class physics_object_set : PropertyHandler
    {
        // 3b
        private void Default_3b()
        {
            stiffness = new physics_object();
            stiffness2 = new physics_object();
            damping = new physics_object();

            Xmaxoffset = new physics_object();
            Xminoffset = new physics_object();
            Ymaxoffset = new physics_object();
            Yminoffset = new physics_object();
            Zmaxoffset = new physics_object();
            Zminoffset = new physics_object();

            timetick = new physics_object();

            linearX = new physics_object();
            linearY = new physics_object();
            linearZ = new physics_object();

            rotationalX = new physics_object();
            rotationalY = new physics_object();
            rotationalZ = new physics_object();

            timeStep = new physics_object();

            linearZrotationY = new physics_object();
            linearXspreadforceZ = new physics_object();
            linearYspreadforceX = new physics_object();
            linearZspreadforceX = new physics_object();

            collisionFriction = new physics_object();
            collisionElastic = new physics_object();
        }

        #region Declaration
        public physics_object stiffness { get => Get<physics_object>(); set => Set(value); }
        public physics_object stiffness2 { get => Get<physics_object>(); set => Set(value); }
        public physics_object damping { get => Get<physics_object>(); set => Set(value); }

        public physics_object Xmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Xminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Ymaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Yminoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Zmaxoffset { get => Get<physics_object>(); set => Set(value); }
        public physics_object Zminoffset { get => Get<physics_object>(); set => Set(value); }

        public physics_object timetick { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZ { get => Get<physics_object>(); set => Set(value); }

        public physics_object rotationalX { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationalY { get => Get<physics_object>(); set => Set(value); }
        public physics_object rotationalZ { get => Get<physics_object>(); set => Set(value); }

        public physics_object timeStep { get => Get<physics_object>(); set => Set(value); }

        public physics_object linearZrotationY { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearXspreadforceZ { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearYspreadforceX { get => Get<physics_object>(); set => Set(value); }
        public physics_object linearZspreadforceX { get => Get<physics_object>(); set => Set(value); }

        public physics_object collisionFriction { get => Get<physics_object>(); set => Set(value); }
        public physics_object collisionElastic { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
    public partial class physics_object_set : PropertyHandler
    {
        // 3b_Gravity, 3b_MoreGravity
        private void Default_3b_Gravities()
        {

            gravityBias = new physics_object();
            gravityCorrection = new physics_object();

            gravityInvertedCorrection = new physics_object();
            gravityInvertedCorrectionStart = new physics_object();
            gravityInvertedCorrectionEnd = new physics_object();

        }
        #region Declaration
        public physics_object gravityBias { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityCorrection { get => Get<physics_object>(); set => Set(value); }

        public physics_object gravityInvertedCorrection { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityInvertedCorrectionStart { get => Get<physics_object>(); set => Set(value); }
        public physics_object gravityInvertedCorrectionEnd { get => Get<physics_object>(); set => Set(value); }
        #endregion
    }
    public partial class physics_object_set : PropertyHandler
    {
        // 3b_armor
        private void Default_3b_armor()
        {
            breastClothedPushup = new physics_object();
            breastLightArmoredPushup = new physics_object();
            breastHeavyArmoredPushup = new physics_object();
            breastClothedAmplitude = new physics_object();
            breastLightArmoredAmplitude = new physics_object();
            breastHeavyArmoredAmplitude = new physics_object();
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
