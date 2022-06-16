using nifly;

namespace h2NIF.Sphere
{
    internal class SphereAdjust : AdvancedAdjust
    {
        public SphereAdjust(NiShape sphere, Vector3? origin = null) : base(sphere, origin) { }

        //NiShape _sphere;
        //NiShape IAdjust.sphere => _sphere;
        //char? _side;
        //char? IAdvancedAdjust.side
        //{
        //    get
        //    {
        //        if (_side == null) _side = _sphere.GetSide();
        //        return _side;
        //    }
        //}
        //Vector3 _origin = new();
        //Vector3 IAdvancedAdjust.origin => _origin;


        ///// <summary>
        ///// f = <c>sphere.transform.scale</c>*<c>A</c>+<c>B</c>
        ///// </summary>
        ///// <param name="A">ratio to scale</param>
        ///// <param name="B">raw translation</param>
        //override float F(float A, float B) => _sphere.transform.scale * A + B;


        ///// <inheritdoc cref="F(float, float)"/>
        //float IBasicAdjust.Fx(float A, float B) => F(A, B);
        ///// <inheritdoc cref="F(float, float)"/>
        //float IBasicAdjust.Fy(float A, float B) => F(A, B);
        ///// <inheritdoc cref="F(float, float)"/>
        //float IBasicAdjust.Fz(float A, float B) => F(A, B);
        ///// <inheritdoc cref="F(float, float)"/>
        //float IScaleAdjust.Fs(float A, float B) => F(A, B);

    }
}
