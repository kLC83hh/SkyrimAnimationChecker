﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SkyrimAnimationChecker.Common;

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
            KeysOrder: new string[] { "stiffness", "damping", "(max|min)offset", "timetick", "linear[XYZxyz]$", "rotational", "timeStep", "linear[XYZxyz]rotation", "spreadforce" },
            KeysOrder2: new string[] { "collision" }
            ) => Defaults();
        public physics_object_set(string name) : base(
            KeysIgnore: new string[] { "Name" },
            KeysOrder: new string[] { "stiffness", "damping", "(max|min)offset", "timetick", "linear[XYZxyz]$", "rotational", "timeStep", "linear[XYZxyz]rotation", "spreadforce" },
            KeysOrder2: new string[] { "collision" }
            ) => Defaults(name);
        private void Defaults(string? name = null)
        {
            if (name != null) Name = name;
            Default_Normal();
            Default_Collision();
            Default_3b();
            Default_3b_Gravities();
            Default_3b_armor();

        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Name { get => Get<string>(); set => Set(value); }
        public override string ToString() { return Name; }
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

    // normal
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_Normal()
        {
            stiffness = new physics_object();
            stiffness2 = new physics_object();
            damping = new physics_object();

            timetick = new physics_object();
            timeStep = new physics_object();

            maxoffset = new physics_object();

            forceMultipler = new physics_object();
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
    // collision
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_Collision()
        {
            collisionXmaxoffset = new physics_object();
            collisionXminoffset = new physics_object();
            collisionYmaxoffset = new physics_object();
            collisionYminoffset = new physics_object();
            collisionZmaxoffset = new physics_object();
            collisionZminoffset = new physics_object();

            collisionFriction = new physics_object();
            collisionPenetration = new physics_object();
            collisionMultipler = new physics_object();
            collisionMultiplerRot = new physics_object();
            collisionElastic = new physics_object();
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
    // 3b
    public partial class physics_object_set : PropertyHandler
    {
        private void Default_3b()
        {

            Xmaxoffset = new physics_object();
            Xminoffset = new physics_object();
            Ymaxoffset = new physics_object();
            Yminoffset = new physics_object();
            Zmaxoffset = new physics_object();
            Zminoffset = new physics_object();

            linearX = new physics_object();
            linearY = new physics_object();
            linearZ = new physics_object();

            rotationalX = new physics_object();
            rotationalY = new physics_object();
            rotationalZ = new physics_object();

            Xdefaultoffset = new physics_object();
            Ydefaultoffset = new physics_object();
            Zdefaultoffset = new physics_object();

            linearXrotationX = new physics_object();
            linearXrotationY = new physics_object();
            linearXrotationZ = new physics_object();
            linearYrotationX = new physics_object();
            linearYrotationY = new physics_object();
            linearYrotationZ = new physics_object();
            linearZrotationX = new physics_object();
            linearZrotationY = new physics_object();
            linearZrotationZ = new physics_object();

            linearXspreadforceY = new physics_object();
            linearXspreadforceZ = new physics_object();
            linearYspreadforceX = new physics_object();
            linearYspreadforceZ = new physics_object();
            linearZspreadforceX = new physics_object();
            linearZspreadforceY = new physics_object();

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
    // 3b_Gravity, 3b_MoreGravity
    public partial class physics_object_set : PropertyHandler
    {
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
    // 3b_armor
    public partial class physics_object_set : PropertyHandler
    {
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