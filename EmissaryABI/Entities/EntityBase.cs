using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using EmissaryABIProtos;
using EmissaryABIProtos.Callbacks;
using EmissaryABIProtos.Callbacks.DestroyEmissary;
using EmissaryABIProtos.Callbacks.DestroyEntityHandler;
using EmissaryABIProtos.Callbacks.DestroyEntityInstance;
using EmissaryABIProtos.Callbacks.EntityInstanceUpdate;
using EmissaryABIProtos.Callbacks.EntityInstanceVersionUpdate;
using EmissaryABIProtos.Callbacks.InstantiateEntityInstance;
using EmissaryABIProtos.Callbacks.ReceiveInstanceStateChange;
using EmissaryABIProtos.Callbacks.ReceiveMessageFromChannel;
using EmissaryABIProtos.Calls;
using EmissaryABIProtos.Calls.AlterEntityInstance;
using EmissaryABIProtos.Calls.ApplyPhysicsToEntityInstance;
using EmissaryABIProtos.Calls.GenerateStateChange;
using EmissaryABIProtos.Calls.RegisterEntityHandlers;
using EmissaryABIProtos.Calls.RequestDestroyEntityInstance;
using EmissaryABIProtos.Calls.SpawnNewEntityInstance;
using EmissaryABIProtos.Messages.EntityIdentity;
using EmissaryABIProtos.Messages.SubscribeToChannel;

namespace EntityABI.Entities
{
    /// <summary>
    /// Each entity needs to implement a class with this as a base.   This will automatically handle
    /// callbacks as virtual functions.   You can have multiple implementations for a single entity if
    /// you have different versions that you want to handle.
    /// 
    /// The host/arena to emissary calls are not really grpc, they're just method calls within the interop
    /// from the wasm processor with a protobuf call and a protobuf return.   There's only one method call
    /// in and one out and are dispatched here.
    /// </summary>
    public abstract class EntityBase
    {
        // Calls from the wasm processor will tell us the dispatch ref to call within the list below
        private static EntityBase[] EntityRegistry = Array.Empty<EntityBase>();

        protected int DispatchOffset { get; private set; }      // We assign this, not the derrived class

        protected EntityIdentity Identity { get; private set; }

        protected EntityBase(EntityIdentity entityIdentity)
        {
            Identity = entityIdentity;

            // Register this

            foreach (var r in EntityRegistry)
                if (r.Identity == Identity) throw new ArgumentException("Entity Already Exists");

            DispatchOffset = EntityRegistry.Length;  // This is where new will appear on the array

            // Append to the array
            var registry = EntityRegistry.ToList();
            registry.Add(this);
            EntityRegistry = registry.ToArray();   // Array for speed

            // call interop to register the entity handler
            var ret = EntityABI_RegisterEntityHandlers(new RegisterEntityHandlersCall
            {
                NewDispatchOffset = DispatchOffset
            });

            // Register will eventually return other stuff for us which we can save for the entity implementation
        }

        #region ABI Calls

        // Call via interop

        private ABI_Call_Returns PerformABICall(ABI_Call_Calls abiCall)
        {
            abiCall.DispatchOffset = DispatchOffset;

            // Call happens here - dummy until we get this 

            return new ABI_Call_Returns();
        }

        protected AlterEntityInstanceReturn EntityABI_AlterEntityInstance(AlterEntityInstanceCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                AlterEntityInstanceCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.AlterEntityInstanceReturn; 
        }

        protected ApplyPhysicsToEntityInstanceReturn EntityABI_ApplyPhysicsToEntityInstance(ApplyPhysicsToEntityInstanceCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                ApplyPhysicsToEntityInstanceCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.ApplyPhysicsToEntityInstanceReturn;
        }

        protected RequestDestroyEntityInstanceReturn EntityABI_RequestDestroyEntityInstance(RequestDestroyEntityInstanceCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                RequestDestroyEntityInstanceCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.RequestDestroyEntityInstanceReturn;
        }

        protected GenerateStateChangeReturn EntityABI_GenerateStateChange(GenerateStateChangeCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                GenerateStateChangeCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.GenerateStateChangeReturn;
        }

        protected RegisterEntityHandlersReturn EntityABI_RegisterEntityHandlers(RegisterEntityHandlersCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                RegisterEntityHandlersCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.RegisterEntityHandlersReturn;
        }

        protected SpawnNewEntityInstanceReturn EntityABI_SpawnNewEntityInstance(SpawnNewEntityInstanceCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                SpawnNewEntityInstanceCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.SpawnNewEntityInstanceReturn;
        }

        protected SubscribeToChannelReturn EntityABI_SubscribeToChannel(SubscribeToChannelCall call)
        {
            var abiCall = new ABI_Call_Calls
            {
                SubscribeToChannelCall = call
            };

            var ret = PerformABICall(abiCall);
            return ret.SubscribeToChannelReturn;
        }



        #endregion
        #region ABI Callbacks to Entity

        // We will be called via interop and will dispatch via DispatchOffset (virtual functions)

        public virtual DestroyEmissaryReturn ABICallback_DestroyEmissary(DestroyEmissaryCall call)
        {
            return new();
        }

        public virtual DestroyEntityHandlerReturn ABICallback_DestroyEntityHandler(DestroyEntityHandlerCall call)
        {
            return new();
        }

        public virtual DestroyEntityInstanceReturn ABICallback_DestroyEntityInstance(DestroyEntityInstanceCall call)
        {
            return new();
        }

        public virtual EntityInstanceUpdateReturn ABICallback_EntityInstanceUpdate(EntityInstanceUpdateCall call)
        {
            return new();
        }

        public virtual EntityInstanceVersionUpdateReturn ABICallback_EntityInstanceVersionUpdate(EntityInstanceVersionUpdateCall call)
        {
            return new();
        }

        public virtual InstantiateEntityInstanceReturn ABICallback_InstantiateEntityInstance(InstantiateEntityInstanceCall call)
        {
            return new();
        }

        public virtual ReceiveInstanceStateChangeReturn ABICallback_ReceiveInstanceStateChange(ReceiveInstanceStateChangeCall call)
        {
            return new();
        }

        public virtual ReceiveMessageFromChannelReturn ABICallback_ReceiveMessageFromChannel(ReceiveMessageFromChannelCall call)
        {
            return new();
        }

        /// <summary>
        /// Unwrap the callback and call the callback on the proper enty and return the Value
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ABI_Callbacks_Returns ABI_RunCallBack(ABI_Callbacks_Calls call)
        {
            var entity = EntityRegistry[call.DispatchOffset];  // Find the entity
            var ret = new ABI_Callbacks_Returns
            {
                DispatchOffset = call.DispatchOffset
            };

            switch (call.CallsCase)
            {
                case ABI_Callbacks_Calls.CallsOneofCase.DestroyEmissaryCall:
                    ret.DestroyEmissaryReturn = entity.ABICallback_DestroyEmissary(call.DestroyEmissaryCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.DestroyEntityHandlerCall:
                    ret.DestroyEntityHandlerReturn = entity.ABICallback_DestroyEntityHandler(call.DestroyEntityHandlerCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.DestroyEntityInstanceCall:
                    ret.DestroyEntityInstanceReturn = entity.ABICallback_DestroyEntityInstance(call.DestroyEntityInstanceCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.EntityInstanceUpdateCall:
                    ret.EntityInstanceUpdateReturn = entity.ABICallback_EntityInstanceUpdate(call.EntityInstanceUpdateCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.EntityInstanceVersionUpdateCall:
                    ret.EntityInstanceVersionUpdateReturn = entity.ABICallback_EntityInstanceVersionUpdate(call.EntityInstanceVersionUpdateCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.InstantiateEntityInstanceCall:
                    ret.InstantiateEntityInstanceReturn = entity.ABICallback_InstantiateEntityInstance(call.InstantiateEntityInstanceCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.ReceiveInstanceStateChangeCall:
                    ret.ReceiveInstanceStateChangeReturn = entity.ABICallback_ReceiveInstanceStateChange(call.ReceiveInstanceStateChangeCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.ReceiveMessageFromChannelCall:
                    ret.ReceiveMessageFromChannelReturn = entity.ABICallback_ReceiveMessageFromChannel(call.ReceiveMessageFromChannelCall);
                    return ret;
            }

            throw new Exception($"Unknown Callback: {call.CallsCase} - entity {call.DispatchOffset}");
        }


        #endregion
    }
}
