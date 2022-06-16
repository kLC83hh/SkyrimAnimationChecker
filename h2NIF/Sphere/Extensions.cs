using nifly;

namespace h2NIF.Sphere
{
    internal static class Extensions
    {
        public static (Vector3 center, float radius, string msg)? UpdateVertColor(this BSTriShape sphere)
        {
            M.D($"{sphere.name.get()} {sphere.rawColors.Count} {sphere.HasVertexColors()} {sphere.triangles.Count}");
            sphere.SetVertexColors(true);
            foreach (BSVertexData vert in sphere.vertData)
            {
                vert.colorData[0] = 255;
                vert.colorData[1] = 0;
                vert.colorData[2] = 0;
                vert.colorData[3] = 255;
            }
            //foreach (Triangle t in sphere.triangles) { }
            M.D($"{sphere.name.get()} {sphere.rawColors.Count} {sphere.HasVertexColors()} {sphere.triangles.Count}");
            return (new(), 0, "Manual update");
        }


        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NiShape sphere, Vector3 center, float radius)
        {
            sphere.transform.translation = center;
            sphere.transform.scale = radius;
            return (center, radius, "Manual update");
        }
        //public static (Vector3 center, float radius, string msg)? UpdateSphere(this NifFile nif, NiShape sphere, NiShape skin, Vector3 sensitivity, Vector3 strength)
        //{
        //    // get skin datas
        //    var skinDismember = new SkinDismember(nif, skin);
        //    if (skinDismember.Error) return (new(), 0, skinDismember.Reason);
        //    BSVertexData[] vdArray = skinDismember.Partition.vertData.ToArray();

        //    //M.D($"{vdArray[0].weightBones[0]}");
        //    //BSSkinBoneData bd = skinData.bones[(int)vdArray[0].weightBones[0]];

        //    // get skin vertices
        //    Vertex[] vertices = new Vertex[vdArray.Length];
        //    for (int i = 0; i < vdArray.Length; i++) vertices[i] = new Vertex(i, vdArray[i].vert, 1);

        //    return UpdateSphere(nif, sphere, vertices, strength, sensitivity);
        //}


    }
}
