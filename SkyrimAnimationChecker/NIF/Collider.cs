using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.NIF
{
    internal class Collider : NIF
    {
        public Collider(Common.VM_GENERAL linker) : base(linker) { }
        public Collider(Common.VM linker) : base(linker) { }

        /// <summary>
        /// Replace3BA
        /// </summary>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public int Make(int weight = -1, bool? overwrite = null)
        {
            vm.NIFrunning = true;
            string[] localExample = new string[2] {
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_0.nif"),
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_1.nif")
            };
            int r1 = 1, r2 = 1;
            if (weight < 0) weight = vm.weightNumber;
            if (overwrite == null) overwrite = vm.overwriteInterNIFs;
            string[] bsfile = vm.fileNIF_bodyslide.Split(',').ForEach(x => x.Trim());
            if (bsfile.Length != 2) throw EE.New(1011);
            switch (weight)
            {
                case 2:
                    r1 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere0 : localExample[0], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[0]), vm.fileNIF_out0, overwrite);
                    r2 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere1 : localExample[1], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[1]), vm.fileNIF_out1, overwrite);
                    break;
                case 1:
                    r2 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere1 : localExample[1], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[1]), vm.fileNIF_out1, overwrite);
                    break;
                case 0:
                    r1 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere0 : localExample[0], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[0]), vm.fileNIF_out0, overwrite);
                    break;
                default:
                    r1 = 0;
                    break;
            }
            vm.NIFrunning = false;
            return r1 * (r2 > 1 ? r2 + 1 : r2);
        }
        private int Replace3BA(string sphere, string bodyslide, string output, bool? overwrite = null)
        {
            if (overwrite == null) overwrite = false;
            if (System.IO.File.Exists(output) && !(bool)overwrite) return 2;
            if (!System.IO.File.Exists(sphere) || !System.IO.File.Exists(bodyslide)) return 4;
            nifly.NifFile spNI = new nifly.NifFile(), bsNI = new nifly.NifFile();
            spNI.Load(sphere);
            bsNI.Load(bodyslide);
            nifly.vectorNiShape eShapes = spNI.GetShapes(), bsShapes = bsNI.GetShapes();
            for (int i = 0; i < eShapes.Count; i++)
            {
                if (eShapes[i].name.get() == "3BA")
                {
                    nifly.NiShape? shape = null;
                    foreach (var s in bsShapes)
                    {
                        if (s.name.get() == "3BA") { shape = s; break; }
                    }
                    if (shape != null)
                    {
                        nifly.NiAlphaProperty alpha = new();
                        spNI.DeleteShape(eShapes[i]);
                        nifly.NiShape cloned = spNI.CloneShape(shape, shape.name.get(), bsNI);
                        spNI.GetShader(cloned).SetAlpha(0.5f);
                    }
                    break;
                }
            }
            spNI.FinalizeData();
            spNI.Save(output);
            return 1;
        }

    }
}
