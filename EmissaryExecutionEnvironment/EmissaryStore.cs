using EmissaryABIProtos;
using Google.Protobuf;
using System.Collections;
using System.Runtime.InteropServices;
using Wasmtime;
using Function = Wasmtime.Function;

namespace EmissaryExecutionEnvironment
{

    internal class EmissaryStore
    {
        const string WASI = "wasi_snapshot_preview1";
        const string ABI = "entityabi";

        internal unsafe void BindABI(Store store, Linker linker)
        {
            //linker.Define(ABI, "__entity_to_arena_call", Function.FromCallback(store, __entity_to_arena_call,
            //    new List<ValueKind> { ValueKind.Int32, ValueKind.Int32},
            //    new List<ValueKind> { ValueKind.Int32 }));

          

            CallerAction<int, int, int, int, int> procEntity= __entity_to_arena_call;

            linker.Define(ABI, "__entity_to_arena_call", Function.FromCallback(store, procEntity));
     
        }

     

        private unsafe void __entity_to_arena_call(Caller caller, int operation, int proto, int protoLen, int returnBuf, int returnlen)
        {
            // proto, returnbuf is byte* and returnlen is int*

            var mem = caller.GetMemory("memory");

            var ret = mem.GetSpan(proto, protoLen);

            var callPack = new Startup();
            callPack.MergeFrom(new CodedInputStream(ret.ToArray()));

            //Console.WriteLine($"Arena Gets: {callPack.Name}");

            var msg = new RetMsg { Name = $"Return From Arena: {DateTime.Now.ToLongTimeString()} on {RuntimeInformation.OSArchitecture}" };
            var msgb = msg.ToByteArray();
            var msgSpan = new Span<byte>(msgb);

            //Console.WriteLine($"Send Response Array: {Convert.ToHexString(msgb)}");

            var retBufLen = mem.ReadInt32(returnlen);  // How big is return buffer

            // Return the return length
            var outlen = msgb.Length;
            if (outlen > retBufLen) throw new Exception("Exceeded return buffer");
            mem.WriteInt32(returnlen, outlen);   // Set how much we're actually returning

            var retSpan = mem.GetSpan<byte>(returnBuf, outlen);

            msgSpan.CopyTo(retSpan);
        }

        private void __entity_to_arena_callX(Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results)
        {
            //   public static unsafe extern int HostRPC(int abi, int abijob, byte* sendPayload, int sendSize, byte* recvPayload, int maxRecvSize);

            var memPtr = arguments[1].AsInt32;
            var operation = arguments[0].AsInt32;

            //var bptr = caller.GetData(memPtr);
        }


        internal void BindWasi(Store store, Linker linker)
        {
            linker.Define(WASI, "environ_get", Function.FromCallback(store, _wasi_environ_get,
           new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
           new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "environ_sizes_get", Function.FromCallback(store, _wasi_environ_sizes_get,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "clock_res_get", Function.FromCallback(store, _wasi_unimplemented,
              new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
              new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "clock_time_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_advise", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int64, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_close", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_fdstat_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_fdstat_set_flags", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_filestat_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_filestat_set_size", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int64 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_filestat_set_times", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int64, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_pread", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_prestat_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_prestat_dir_name", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_pwrite", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_read", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_readdir", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_seek", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int64, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_sync", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));
                      
            linker.Define(WASI, "fd_tell", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "fd_write", Function.FromCallback(store, _wasi_unimplemented,
             new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
             new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_create_directory", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_filestat_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_filestat_set_times", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int64, ValueKind.Int64, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_link", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_open", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int64, ValueKind.Int64, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_readlink", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_remove_directory", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_rename", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_symlink", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "path_unlink_file", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "poll_oneoff", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32, ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            linker.Define(WASI, "proc_exit", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32 },
               new List<ValueKind> {  }));

            linker.Define(WASI, "random_get", Function.FromCallback(store, _wasi_unimplemented,
               new List<ValueKind> { ValueKind.Int32, ValueKind.Int32 },
               new List<ValueKind> { ValueKind.Int32 }));

            //using var store2 = new Store(engine);




        }


        private void _wasi_environ_sizes_get(Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results)
        {


        }
        private void _wasi_environ_get(Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results)
        {

        }


        private void _wasi_unimplemented(Caller caller, ReadOnlySpan<ValueBox> arguments, Span<ValueBox> results)
        {
            // var arg = caller.GetMemory("mem").ReadString(address, len);

            
        }



    }
}
