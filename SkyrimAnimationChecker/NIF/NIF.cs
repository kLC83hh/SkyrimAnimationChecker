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
    //public class CBPC_collider_object_nameSelector : Notify.NotifyPropertyChanged
    //{
    //    public CBPC_collider_object_nameSelector()
    //    {
    //        Left = new System.Collections.ObjectModel.ObservableCollection<string>() { "NPC L", "L Breast" };
    //        Right = new System.Collections.ObjectModel.ObservableCollection<string>() { "NPC R", "R Breast" };
    //    }

    //    public System.Collections.ObjectModel.ObservableCollection<string> Left
    //    {
    //        get => Get<System.Collections.ObjectModel.ObservableCollection<string>>();
    //        set => Set(value);
    //    }
    //    public System.Collections.ObjectModel.ObservableCollection<string> Right
    //    {
    //        get => Get<System.Collections.ObjectModel.ObservableCollection<string>>();
    //        set => Set(value);
    //    }
    //}

}
