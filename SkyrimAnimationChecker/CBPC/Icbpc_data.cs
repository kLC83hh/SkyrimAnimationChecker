using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.CBPC
{
    public interface Icbpc_data : Common.IPropertyHandler
    {
        public string DataType { get; }
        public string[] MirrorKeys { get; set; }
        public Common.physics_object_set? Find(string name);
    }
    public interface Icbpc_breast_data : Icbpc_data
    {
        public cbpc_breast GetData(int? num = null);
    }
}
