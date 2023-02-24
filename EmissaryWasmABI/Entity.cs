using EmissaryABIProtos;
using EntityInterop;
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
        public delegate byte[] EntityFunc(int abi, int abitask, byte[] payload);

        public Entity()
        {
            interop = new Interop();
            //Interop.SetInterop(interop);
        }

        private Interop interop;

        public Startup EnterStartup()
        {
      

            //var ba = start.ToByteArray();

            //var ba = _entity_get_startup();

            //var ret = new Startup();
            //ret.MergeFrom(ba);

            //var wormhole = _get_entity_test(1);

            var start = new Startup { Name = $"Hello From Arena Side- {RuntimeInformation.OSArchitecture}" };
            var ret = new RetMsg();

            var rv = CallEntityRPC(0, 0, start, ret);

            return start;
        }

        private int CallEntityRPC(int v1, int v2, Startup start, RetMsg ret)
        {
            var ba = new byte[8192];

            int error = interop.CallEntityRPC(v1, v2, start, ref ba);

            return error;
        }

        //[DllImport("EntityInterop")]
        //public static extern byte[]? _entity_get_startup();
        //public static extern Int32 _get_entity_test(Int32 input);
    }
}
