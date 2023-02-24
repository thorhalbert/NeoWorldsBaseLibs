using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text;
using static NeoWorlds.EmissaryWasmABI.Entity;
using Google.Protobuf;

namespace EntityInterop;

internal class Interop
{
    private Dictionary<string, EntityFunc> Functions;

    public Interop()
    {
        Functions = new Dictionary<string, EntityFunc>();
    }

    public unsafe int CallEntityRPC(int abi, int abijob, IMessage inmsg, ref Byte[] outmsg)
    {
        var inBuf = inmsg.ToByteArray();
        Span<byte> inSpan = inBuf;

        Span<byte> outBuf = outmsg;

        int retVal = 0;

        fixed (byte* inSpanPtr = inSpan, outSpanPtr = outBuf)
        {
            retVal = HostRPC(abi, abijob, inSpanPtr, inSpan.Length, outSpanPtr, outBuf.Length);
        }

        return retVal;
    }

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static extern void SetInterop(Interop owner);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public static unsafe extern int HostRPC(int abi, int abijob, byte* sendPayload, int sendSize, byte* recvPayload, int maxRecvSize);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void GuestRequest(byte* operation, byte* payload);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void GuestResponse(byte* payload, int len);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void GuestError(byte* payload, int len);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern bool HostCall(
    //  byte* bindingPayload, int bindingLen,
    //  byte* namespacePayload, int namespaceLen,
    //  byte* operationPayload, int operationLen,
    //  byte* payload, int payloadLen);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern int HostResponseLen();

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void HostResponse(byte* payload);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern int HostErrorLen();

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void HostError(byte* payload);

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public static unsafe extern void ConsoleLog(byte* payload, int len);
}