﻿using Wasmtime;
using Function = Wasmtime.Function;

namespace EmissaryExecutionEnvironment
{

    internal class EmissaryStore
    {
        const string WASI = "wasi_snapshot_preview1";

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