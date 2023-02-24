using EmissaryABIProtos;
using Google.Protobuf;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EntityABI;

public delegate byte[] WapcFunc(byte[] payload);

public class Wapc
{
  public Wapc()
  {
    interop = new Interop();
    Interop.SetInterop(interop);
  }

  private Interop interop;

    public static unsafe void testCall()
    {
        var start = new Startup
        {
            Name = $"From Emissary: {DateTime.Now.ToLongTimeString()} on {RuntimeInformation.OSArchitecture}"
        };

        var b = start.ToByteArray();

        var retVal = new RetMsg();
        Int32 retLen = 4096;

        Byte[] returnArray = new byte[retLen];

        fixed (byte* bp = b, ra = returnArray)
        {
            Interop.EntityToArenaCall(1, bp, b.Length, ra, &retLen);
            Console.WriteLine($"Arena Call Returns - length={retLen}");
            Console.WriteLine($"Return array: {Convert.ToHexString(returnArray, 0, retLen)}");
            retVal.MergeFrom(returnArray, 0, retLen);
        }

        Console.WriteLine($"Entity Gets: {retVal.Name}");


    }

    //public static unsafe byte[] HostCall(
    //  string binding,
    //  string wapcNamespace,
    //  string operation,
    //  byte[] payload)
    //{
    //  Encoding utf8 = Encoding.UTF8;

    //  byte[] bindingBuf = utf8.GetBytes(binding);
    //  Span<byte> bindingBytesSpan = bindingBuf;
    //  byte[] namespaceBuf = utf8.GetBytes(wapcNamespace);
    //  Span<byte> namespaceBytesSpan = namespaceBuf;
    //  byte[] operationBuf = utf8.GetBytes(operation);
    //  Span<byte> operationBytesSpan = operationBuf;

    //  Span<byte> payloadBytesSpan = payload;

    //  fixed (byte* bindingBytesPtr = bindingBytesSpan,
    //    namespaceBytesPtr = namespaceBytesSpan,
    //    operationBytesPtr = operationBytesSpan,
    //    payloadBytesPtr = payloadBytesSpan)
    //  {
    //    bool result = Interop.HostCall(
    //      bindingBytesPtr, bindingBuf.Length,
    //      namespaceBytesPtr, namespaceBuf.Length,
    //      operationBytesPtr, operationBuf.Length,
    //      payloadBytesPtr, payload.Length);
    //    if (!result)
    //    {
    //      int errorLen = Interop.HostErrorLen();
    //      byte[] errorBuf = new byte[errorLen];
    //      Span<byte> errorBytesSpan = errorBuf;
    //      fixed (byte* errorBytesPtr = errorBytesSpan)
    //      {
    //        Interop.HostError(errorBytesPtr);
    //      }

    //      string error = utf8.GetString(errorBuf);
    //      ConsoleLog($"Host error: {error}");
    //      byte[] ret = new byte[0];
    //      return ret;
    //    }
    //  }

    //  int responseLen = Interop.HostResponseLen();
    //  byte[] responseBuf = new byte[responseLen];
    //  Span<byte> responseBytesSpan = responseBuf;
    //  fixed (byte* responseBytesPtr = responseBytesSpan)
    //  {
    //    Interop.HostResponse(responseBytesPtr);
    //  }
    //  return responseBuf;
    //}

    //public unsafe static void ConsoleLog(string msg)
    //{
    //  Encoding utf8 = Encoding.UTF8;

    //  byte[] msgBuf = utf8.GetBytes(msg);
    //  Span<byte> msgBytesSpan = msgBuf;
    //  fixed (byte* msgBytesPtr = msgBytesSpan)
    //  {
    //    Interop.ConsoleLog(msgBytesPtr, msgBuf.Length);
    //  }
    //}

    //public void RegisterFunction(string name, WapcFunc fn)
    //{
    //  interop.RegisterFunction(name, fn);
    //}
}

