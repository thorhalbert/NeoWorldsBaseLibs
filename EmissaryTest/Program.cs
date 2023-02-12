using EmissaryExecutionEnvironment;

namespace EmissaryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var engine = new TestWasm();
            engine.Launch(@"C:\Projects\HelloWorldEmissary\BouncySphere\bin\Debug\net7.0\BouncySphere.wasm");

        }
    }
}