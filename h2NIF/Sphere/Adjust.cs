namespace h2NIF.Sphere
{
    internal class Adjust
    {
        public Adjust(nifly.NiShape s) => sphere = s;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public nifly.NiShape sphere { get; }

        /// <summary>
        /// f = <c>sphere.transform.scale</c>*<c>A</c>+<c>B</c>
        /// </summary>
        /// <param name="A">ratio to scale</param>
        /// <param name="B">raw translation</param>
        protected virtual float F(float A, float B) => sphere.transform.scale * A + B;


        /// <inheritdoc cref="F(float, float)"/>
        protected virtual float Fx(float A, float B) => F(A, B);
        /// <inheritdoc cref="Fx(float, float)"/>
        public void Right(float A, float B = 0) => sphere.transform.translation.x += Fx(A, B);
        /// <inheritdoc cref="Fx(float, float)"/>
        public void Right(double A, double B = 0) => Right((float)A, (float)B);
        /// <inheritdoc cref="Fx(float, float)"/>
        public void Left(float A, float B = 0) => sphere.transform.translation.x -= Fx(A, B);
        /// <inheritdoc cref="Fx(float, float)"/>
        public void Left(double A, double B = 0) => Left((float)A, (float)B);


        /// <inheritdoc cref="F(float, float)"/>
        protected virtual float Fy(float A, float B) => F(A, B);
        /// <inheritdoc cref="Fy(float, float)"/>
        public void Forward(float A, float B = 0) => sphere.transform.translation.y += Fy(A, B);
        /// <inheritdoc cref="Fy(float, float)"/>
        public void Forward(double A, double B = 0) => Forward((float)A, (float)B);
        /// <inheritdoc cref="Fy(float, float)"/>
        public void Backward(float A, float B = 0) => sphere.transform.translation.y -= Fy(A, B);
        /// <inheritdoc cref="Fy(float, float)"/>
        public void Backward(double A, double B = 0) => Backward((float)A, (float)B);


        /// <inheritdoc cref="F(float, float)"/>
        protected virtual float Fz(float A, float B) => F(A, B);
        /// <inheritdoc cref="Fz(float, float)"/>
        public void Raise(float A, float B = 0) => sphere.transform.translation.z += Fz(A, B);
        /// <inheritdoc cref="Fz(float, float)"/>
        public void Raise(double A, double B = 0) => Raise((float)A, (float)B);
        /// <inheritdoc cref="Fz(float, float)"/>
        public void Lower(float A, float B = 0) => sphere.transform.translation.z -= Fz(A, B);
        /// <inheritdoc cref="Fz(float, float)"/>
        public void Lower(double A, double B = 0) => Lower((float)A, (float)B);


        /// <inheritdoc cref="F(float, float)"/>
        protected virtual float Fs(float A, float B) => F(A, B);
        /// <inheritdoc cref="Fs(float, float)"/>
        public virtual void ScaleUp(float A, float B = 0) => sphere.transform.scale += Fs(A, B);
        /// <inheritdoc cref="Fs(float, float)"/>
        public virtual void ScaleUp(double A, double B = 0) => ScaleUp((float)A, (float)B);
        /// <inheritdoc cref="Fs(float, float)"/>
        public virtual void ScaleDown(float A, float B = 0) => sphere.transform.scale -= Fs(A, B);
        /// <inheritdoc cref="Fs(float, float)"/>
        public virtual void ScaleDown(double A, double B = 0) => ScaleDown((float)A, (float)B);


        /// <summary>
        /// <code>sphere.transform.scale *= value</code>
        /// </summary>
        /// <param name="value">ratio to adjust</param>
        public virtual void Scale(float value) => sphere.transform.scale *= value;

        /// <inheritdoc cref="Scale(float)"/>
        public virtual void Scale(double value) => Scale((float)value);
    }
}