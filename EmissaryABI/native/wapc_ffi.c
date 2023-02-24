#include <mono-wasi/driver.h>
#include <assert.h>
#include <stdlib.h>
#include <stdbool.h>

// Only two EntityABI Calls - one from the Entity Emissary to the Arena Browser and back
// The payload and the response are the appropriate serialized protobufs for the given operation

// One of these should manifest an import and one an export into the wasm that we bind in wasmtime

__attribute__((__import_module__("entityabi"), __import_name__("__entity_to_arena_call"))) extern void
__entity_to_arena_call(int operation, const char* payload, int payloadLen, char *returnArray, int *returnlen);

MonoMethod* method_HandleArenaCall;
MonoObject* interop_instance = 0;

__attribute__((export_name("__arena_to_entity_call"))) char * __arena_to_entity_call(int operation, char *payload)
{
    if (!method_HandleArenaCall)
    {
        method_HandleArenaCall = lookup_dotnet_method("WapcGuest.dll", "EntityABI", "Interop", "ArenaToEntityCall", -1);
        assert(method_HandleArenaCall);
    }

    void* method_params[] = { interop_instance, &operation, payload };
    MonoObject* exception;
    MonoObject* result = mono_wasm_invoke_method(method_HandleArenaCall, NULL, method_params, &exception);
    assert(!exception);

    char *_result = *(char **)mono_object_unbox(result);
    return _result;
}


// wapc entries - which we'll disable as we go until we get the ABI functions to actually work (and bind properly)

//
//__attribute__((__import_module__("entityabi"), __import_name__("__guest_request"))) extern void
//__guest_request(char *operation, char *payload);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__guest_response"))) extern void
//__guest_response(const char *payload, size_t len);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__guest_error"))) extern void
//__guest_error(const char *payload, size_t len);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__host_call"))) extern bool
//__host_call(
//    const char *binding_payload, size_t binding_len,
//    const char *namespace_payload, size_t namespace_len,
//    const char *operation_payload, size_t operation_len,
//    const char *payload, size_t payload_len);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__host_response_len"))) extern size_t
//__host_response_len(void);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__host_response"))) extern void
//__host_response(char *payload);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__host_error_len"))) extern size_t
//__host_error_len(void);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__host_error"))) extern void
//__host_error(char *payload);
//
//__attribute__((__import_module__("entityabi"), __import_name__("__console_log"))) extern void
//__console_log(const char *payload, size_t payload_len);
//
//MonoMethod *method_HandleGuestCall;
//
//
//__attribute__((export_name("__guest_call"))) bool __guest_call(size_t operation_len, size_t payload_len)
//{
//  if (!method_HandleGuestCall)
//  {
//    method_HandleGuestCall = lookup_dotnet_method("WapcGuest.dll", "WapcGuest", "Interop", "HandleGuestCall", -1);
//    assert(method_HandleGuestCall);
//  }
//
//  void *method_params[] = {interop_instance, &operation_len, &payload_len};
//  MonoObject *exception;
//  MonoObject *result = mono_wasm_invoke_method(method_HandleGuestCall, NULL, method_params, &exception);
//  assert(!exception);
//
//  bool bool_result = *(bool *)mono_object_unbox(result);
//  return bool_result;
//}

void set_interop(MonoObject *interop)
{
  interop_instance = interop;
}

// Attach the imports - don't see an equivalent for the exports
void attach_internal_calls()
{
  mono_add_internal_call("EntityABI.Interop::EntityToArenaCall", __entity_to_arena_call);

  //mono_add_internal_call("WapcGuest.Interop::GuestRequest", __guest_request);
  //mono_add_internal_call("WapcGuest.Interop::GuestResponse", __guest_response);
  //mono_add_internal_call("WapcGuest.Interop::GuestError", __guest_error);
  //mono_add_internal_call("WapcGuest.Interop::HostCall", __host_call);
  //mono_add_internal_call("WapcGuest.Interop::HostResponseLen", __host_response_len);
  //mono_add_internal_call("WapcGuest.Interop::HostResponse", __host_response);
  //mono_add_internal_call("WapcGuest.Interop::HostErrorLen", __host_error_len);
  //mono_add_internal_call("WapcGuest.Interop::HostError", __host_error);
  //mono_add_internal_call("WapcGuest.Interop::ConsoleLog", __console_log);

  mono_add_internal_call("EntityABI.Interop::SetInterop", set_interop);
}
