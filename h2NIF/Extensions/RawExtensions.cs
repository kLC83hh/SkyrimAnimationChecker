using h2NIF.DataStructure;
using nifly;

namespace h2NIF.Extensions
{
    internal static class RawExtensions
    {
        //public static NiObject GetBlock(this NifFile nif, NiRef refer) => nif.GetHeader().GetBlockById(refer.index);
        public static T? GetBlock<T>(this NifFile nif, NiRef refer) => nif.GetBlock<T>(refer.index);
        public static T? GetBlock<T>(this NifFile nif, uint refer)
        {
            if (nif.GetHeader().GetBlockById(refer) is T output) return output;
            //else throw new InvalidCastException($"NiObject can not be converted to {typeof(T)}");
            else return default;
        }
        public static NiNode? GetNode(this NifFile nif, string name)
        {
            foreach (var node in nif.GetNodes())
            {
                if (node.name.get() == name) return node;
            }
            return null;
        }
        //public static NiNode? GetNode(this Nif nif, string name)
        //{
        //    nifly.NiNode? node = ((NifFile)nif)?.GetNode(name);
        //    NiNode? outnode = null;
        //    if (node != null) outnode = (NiNode)node;
        //    return outnode;
        //}

        //public static BSTriShape GetTriShape(this NifFile nif, string name)
        //{
        //    BSTriShape ts = new();
        //    NiShape shape = new();
        //    ((NiShape)ts).name.get();
        //    ((BSTriShape)shape).name.get();
        //    //nif.GetHeader().GetBlockID
        //}
        public static NiShape? GetShape(this NifFile nif, string name)
        {
            if (!nif.GetShapes().Any(x => x.name.get() == name)) return null;
            else return nif.GetShapes().First(x => x.name.get() == name);
        }
        public static NiShape[] GetNamedShapes(this NifFile nif, string name, string? nameEnd = null)
        {
            if (!nif.GetShapes().Any(x => x.name.get().StartsWith(name))) return Array.Empty<NiShape>();
            else
            {
                if (string.IsNullOrEmpty(nameEnd)) return nif.GetShapes().Where(x => x.name.get().StartsWith(name)).ToArray();
                else
                {
                    if (!nif.GetShapes().Any(x => x.name.get().EndsWith(nameEnd))) return Array.Empty<NiShape>();
                    else return nif.GetShapes().Where(x => x.name.get().StartsWith(name)).Where(x => x.name.get().EndsWith(nameEnd)).ToArray();
                }
            }
        }

        public static List<NiShape> GetChildShapes(this NifFile nif, NiNode parent, bool all = false)
        {
            List<NiShape> children = new();
            foreach (var shape in nif.GetShapes())
            {
                if (nif.GetParentNode(shape).name.get() == parent.name.get() && (shape.transform.scale > 0 || all)) children.Add(shape);
            }
            //System.Collections.Concurrent.ConcurrentBag<NiShape> children = new();
            //Parallel.ForEach(nif.GetShapes(), shape =>
            //{
            //    if (nif.GetParentNode(shape).name.get() == parent.name.get() && (shape.transform.scale > 0 || all)) children.Add(shape);
            //});
            return children.ToList();
        }
        //public static List<NiShape> GetChildShapes(this NifFile nif, NiNode parent, bool all = false) => nif.GetChildShapes((nifly.NiNode)parent, all);
        //public static List<NiShape> GetChildShapes(this Nif nif, NiNode parent, bool all = false) => ((NifFile)nif).GetChildShapes((nifly.NiNode)parent, all);
        //public static List<NiShape> GetChildShapes(this Nif nif, nifly.NiNode parent, bool all = false) => ((NifFile)nif).GetChildShapes(parent, all);

        public static BSTriShape? GetTriShape(this NifFile nif, string name)
        {
            if (!nif.GetTriShapes().Any(x => x.name.get() == name)) return null;
            else return nif.GetTriShapes().First(x => x.name.get() == name);
        }
        public static BSTriShape[] GetNamedTriShapes(this NifFile nif, string name, string? nameEnd = null)
        {
            if (!nif.GetTriShapes().Any(x => x.name.get().StartsWith(name))) return Array.Empty<BSTriShape>();
            else
            {
                if (string.IsNullOrEmpty(nameEnd)) return nif.GetTriShapes().Where(x => x.name.get().StartsWith(name)).ToArray();
                else
                {
                    System.Text.RegularExpressions.Regex regexEnd = new($@"{nameEnd}\d*$");
                    if (!nif.GetTriShapes().Any(x => regexEnd.IsMatch(x.name.get()))) return Array.Empty<BSTriShape>();
                    else return nif.GetTriShapes().Where(x => x.name.get().StartsWith(name)).Where(x => regexEnd.IsMatch(x.name.get())).ToArray();
                }
            }
        }
        public static BSTriShape[] GetTriShapes(this NifFile nif)
        {
            List<BSTriShape> bSTriShapes = new();
            foreach (var shape in nif.GetShapes())
            {
                var trishape = nif.GetBlock<BSTriShape>(nif.GetBlockID(shape));
                if (trishape != null) bSTriShapes.Add(trishape);
            }
            //System.Collections.Concurrent.ConcurrentBag<BSTriShape> bSTriShapes = new();
            //Parallel.ForEach(nif.GetShapes(), shape =>
            //{
            //    var trishape = nif.GetBlock<BSTriShape>(nif.GetBlockID(shape));
            //    if (trishape != null) bSTriShapes.Add(trishape);
            //});
            return bSTriShapes.ToArray();
        }

        //public static NiShape? GetSkinned(this NifFile nif)
        //{
        //    NiShape? skin = null;
        //    Parallel.ForEach(nif.GetShapes(), shape =>
        //    {
        //        if (shape.IsSkinned()) skin = shape;
        //    });
        //    return skin;
        //}
        //public static NiShape? GetSkinned(this Nif nif) => ((NifFile)nif).GetSkinned();
        public static NiShape[] GetAllSkinned(this NifFile nif)
        {
            List<NiShape> skin = new();
            foreach (var shape in nif.GetShapes())
            {
                if (shape.IsSkinned()) skin.Add(shape);
            }
            //System.Collections.Concurrent.ConcurrentBag<NiShape> skin = new();
            //Parallel.ForEach(nif.GetShapes(), shape =>
            //{
            //    if (shape.IsSkinned()) skin.Add(shape);
            //});
            return skin.ToArray();
        }
        //public static NiShape[] GetAllSkinned(this Nif nif) => ((NifFile)nif).GetAllSkinned();

        //Parellelized
        public static int GetFilteredBones(this NifFile nif, out Bone[] bones, NiShape skin, string[] filter)
        {
            nif.GetShapeBoneList(skin, out string[] list);
            nif.GetShapeBoneIDList(skin, out int[] ids);

            if (list.Length != ids.Length)
            {
                bones = Array.Empty<Bone>();
                return -Math.Abs(list.Length - ids.Length);
            }

            //List<Bone> result = new();
            //for (int i = 0; i < list.Length; i++)
            //{
            //    foreach (string f in filter)
            //    {
            //        if (list[i].StartsWith(f)) result.Add(new Bone(nif, skin, i, (uint)ids[i], list[i]));
            //    }
            //}
            System.Collections.Concurrent.ConcurrentBag<Bone> result = new();
            Parallel.For(0, list.Length, i =>
            {
                foreach (string f in filter)
                {
                    if (list[i].StartsWith(f)) result.Add(new Bone(nif, skin, i, (uint)ids[i], list[i]));
                }
            });
            bones = result.OrderBy(x => x.Name).ToArray();
            return list.Length;
        }


        public static uint GetShapeBoneList(this NifFile nif, NiShape shape, out vectorstring list)
        {
            list = new();
            return nif.GetShapeBoneList(shape, list);
        }
        public static uint GetShapeBoneList(this NifFile nif, NiShape shape, out string[] list)
        {
            vectorstring buffer = new();
            uint result = nif.GetShapeBoneList(shape, buffer);
            list = buffer.ToArray();
            return result;
        }
        public static uint GetShapeBoneIDList(this NifFile nif, NiShape shape, out vectorint list)
        {
            list = new();
            return nif.GetShapeBoneIDList(shape, list);
        }
        public static uint GetShapeBoneIDList(this NifFile nif, NiShape shape, out int[] list)
        {
            vectorint buffer = new();
            uint result = nif.GetShapeBoneIDList(shape, buffer);
            list = buffer.ToArray();
            return result;
        }
        public static bool GetShapeBoneTransform(this NifFile nif, NiShape shape, string boneName, out MatTransform transform)
        {
            transform = new();
            return nif.GetShapeBoneTransform(shape, boneName, transform);
        }
        public static bool GetShapeTransformSkinToBone(this NifFile nif, NiShape shape, string boneName, out MatTransform transform)
        {
            transform = new();
            return nif.GetShapeTransformSkinToBone(shape, boneName, transform);
        }
        public static bool GetShapeBoneBounds(this NifFile nif, NiShape shape, uint id, out BoundingSphere bs)
        {
            bs = new();
            return nif.GetShapeBoneBounds(shape, id, bs);
        }



    }
}
