#include <mono-wasi/driver.h>
#include <assert.h>
#include <stdlib.h>
#include <stdbool.h>

__attribute__((__import_module__("entityabi"), __import_name__("__entity_rpc"))) extern int
__entity_rpc(int abi, int abitask, const char* protoIn, int protoInSize, char* protoOut, int protoOutSize);

//MonoMethod* method_HandleGuestCall;
MonoObject* interop_instance = 0;
//
//__attribute__((export_name("__guest_call"))) bool __guest_call(size_t operation_len, size_t payload_len)
//{
//    if (!method_HandleGuestCall)
//    {
//        method_HandleGuestCall = lookup_dotnet_method("WapcGuest.dll", "WapcGuest", "Interop", "HandleGuestCall", -1);
//        assert(method_HandleGuestCall);
//    }
//
//    void* method_params[] = { interop_instance, &operation_len, &payload_len };
//    MonoObject* exception;
//    MonoObject* result = mono_wasm_invoke_method(method_HandleGuestCall, NULL, method_params, &exception);
//    assert(!exception);
//
//    bool bool_result = *(bool*)mono_object_unbox(result);
//    return bool_result;
//}

void set_interop(MonoObject* interop)
{
    interop_instance = interop;
}

void attach_internal_calls()
{
    mono_add_internal_call("EntityInterop.Interop::HostRPC", __entity_rpc);

    //mono_add_internal_call("WapcGuest.Interop::SetInterop", set_interop);
}
