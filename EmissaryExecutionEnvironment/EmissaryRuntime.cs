using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace EmissaryExecutionEnvironment
{
    public class EmissaryRuntime
    {
        public void Launch(string wasmFile)
        {
            using var engine = new Engine(new Config().WithReferenceTypes(true));

            using var linker = new Linker(engine);

            var emissaryStore = new EmissaryStore();
            using var module = Module.FromFile(engine, wasmFile);
            using var store = new Store(engine, emissaryStore);

            linker.DefineWasi();

            store.SetWasiConfiguration(new WasiConfiguration().WithInheritedStandardOutput());

            emissaryStore.BindABI(store, linker);
            //emissaryStore.BindWasi(store, linker);

            var instance = linker.Instantiate(store, module);

            var run = instance.GetAction("_start");
            if (run is null)
            {
                Console.WriteLine("error: run export is missing");
                return;
            }

            run();
        }
    }
}
