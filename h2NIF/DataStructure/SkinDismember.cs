using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.Extensions;

namespace h2NIF.DataStructure
{
    public class SkinDismember
    {
        public SkinDismember(NifFile nif, NiShape skin)
        {
            Nif = nif;
            Skin = skin;
        }
        public NifFile Nif { get; set; }
        public NiShape Skin { get; set; }


        private readonly Dictionary<byte, string> reasons = new()
        {
            { 1, "Error: can not retrieve BSDismemberSkinInstance" },
            { 2, "Error: can not retrieve NiSkinPartition" },
            { 4, "Error: can not retrieve NiSkinData" }
        };
        public string Reason
        {
            get
            {
                foreach (var key in reasons.Keys)
                {
                    if ((_Error & key) == key) return reasons[(byte)(_Error & key)];
                }
                return string.Empty;
            }
        }
        private byte _Error = 0;
        public bool Error => _Error != 0;


        private BSDismemberSkinInstance? _Instance = null;
        public BSDismemberSkinInstance Instance
        {
            get
            {
                if (_Instance == null) _Instance = Nif.GetBlock<BSDismemberSkinInstance>(Skin.SkinInstanceRef());
                if (_Instance == null) { _Instance = new(); _Error |= 1; }
                return _Instance;
            }
        }

        private NiSkinPartition? _Partition = null;
        public NiSkinPartition Partition
        {
            get
            {
                if (_Partition == null) _Partition = Nif.GetBlock<NiSkinPartition>(Instance.skinPartitionRef);
                if (_Partition == null) { _Partition = new(); _Error |= 2; }
                return _Partition;
            }
        }

        private NiSkinData? _Data = null;
        public NiSkinData Data
        {
            get
            {
                if (_Data == null) _Data = Nif.GetBlock<NiSkinData>(Instance.dataRef);
                if (_Data == null) { _Data = new(); _Error |= 4; }
                return _Data;
            }
        }

    }
}
