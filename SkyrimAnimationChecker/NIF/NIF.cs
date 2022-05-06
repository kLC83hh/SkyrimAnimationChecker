using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.NIF
{
    internal class NIF
    {
        protected VM_GENERAL vm;
        public NIF(VM_GENERAL linker) => vm = linker;
        public NIF(VM linker) => vm = linker.GENERAL;


    }
    public static class NIFExtensions
    {
        public static nifly.NiNode? GetNode(this nifly.NifFile nif, string name)
        {
            foreach (var node in nif.GetNodes())
            {
                if (node.name.get() == name) return node;
            }
            return null;
        }
        public static nifly.NiShape? GetShape(this nifly.NifFile nif, string name)
        {
            if (!nif.GetShapes().Any(x => x.name.get() == name)) return null;
            else return nif.GetShapes().First(x => x.name.get() == name);
        }

        public static void SetTransform(this nifly.NifFile nif, string name, nifly.MatTransform transform)
        {
            if (nif.GetShapeNames().Any(x => x == name))
            {
                nifly.NiShape? shape = nif.GetShape(name);
                if (shape != null) shape.transform = transform;
            }
            else
            {
                nifly.NiNode? node = nif.GetNode(name);
                if (node != null) (node).transform = transform;
            }
        }
        public static void SetTransform(this nifly.NifFile nif, string name, string transform)
        {
            if (nif.GetShapeNames().Any(x => x == name))
            {
                nifly.NiShape? shape = nif.GetShape(name);
                if (shape != null)
                {
                    var val = GetVals(transform);
                    shape.transform.scale = val.s;
                    shape.transform.translation.x = val.x;
                    shape.transform.translation.y = val.y;
                    shape.transform.translation.z = val.z;
                }
            }
            else
            {
                nifly.NiNode? node = nif.GetNode(name);
                if (node != null)
                {
                    var val = GetVals(transform);
                    node.transform.scale = val.s;
                    node.transform.translation.x = val.x;
                    node.transform.translation.y = val.y;
                    node.transform.translation.z = val.z;
                }
            }
        }
        private static (float x, float y, float z, float s) GetVals(string data)
        {
            string[] split = data.Split(',');
            if (split.Length != 4) return (0, 0, 0, 1);

            try
            {
                float x, y, z, s;
                x = float.Parse(split[0]);
                y = float.Parse(split[1]);
                z = float.Parse(split[2]);
                s = float.Parse(split[3]);
                return (x, y, z, s);
            }
            catch { }

            return (0, 0, 0, 1);
        }

        private static List<nifly.NiShape> GetChildShapes(this nifly.NifFile nif, nifly.NiNode parent, bool all = false)
        {
            var shapes = nif.GetShapes();
            List<nifly.NiShape> children = new();
            foreach (nifly.NiShape shape in shapes)
            {
                if (nif.GetParentNode(shape).name.get() == parent.name.get() && (shape.transform.scale > 0 || all)) children.Add(shape);
            }
            return children;
        }
        public static void UpdateSphere(this nifly.NifFile nif, string name, string transform)
        {
            if (name.Contains("Finger"))
            {
                var shape = nif.GetShape(name + " Sphere");
                if (shape != null)
                {
                    nif.SetTransform(shape.name.get(), transform);
                }
            }
            else
            {
                var node = nif.GetNode(name + " Location");
                if (node != null)
                {
                    //nifly.vectoruint32 indices = new();
                    //node.childRefs.GetIndices(indices);
                    //if (indices.Count == 1) nif.SetTransform(name + " Sphere", transform);
                    List<nifly.NiShape> children = nif.GetChildShapes(node);
                    if (children.Count == 1) nif.SetTransform(children.First().name.get(), transform);
                }
            }
        }
        public static void UpdateCapsule(this nifly.NifFile nif, string name, string transformI, string transformF)
        {
            var node = nif.GetNode(name + " Location");
            if (node != null)
            {
                List<nifly.NiShape> children = nif.GetChildShapes(node, true);
                if (children.Count > 1)
                {
                    children = children.OrderBy(x => x.name.get()).ToList();
                    if (children.First().transform.scale > 0 && children.Last().transform.scale > 0)
                    {
                        nif.SetTransform(children.First().name.get(), transformI);
                        nif.SetTransform(children.Last().name.get(), transformF);
                    }
                }
            }
        }



    }

}
