using EmissaryABIProtos;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoWorlds.EmissaryWasmABI
{
    public class Entity
    {
        public Startup EnterStartup()
        {
      

            //var ba = start.ToByteArray();

            //var ba = _entity_get_startup();

            //var ret = new Startup();
            //ret.MergeFrom(ba);

            var wormhole = _get_entity_test(1);

            var start = new Startup { Name = $"Hello From Arena Side- {RuntimeInformation.OSArchitecture} wormhole={wormhole}" };

            return start;
        }

        [DllImport("EntityInterop")]
        //public static extern byte[]? _entity_get_startup();
        public static extern Int32 _get_entity_test(Int32 input);
    }
}
