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
using EmissaryABIProtos.Calls.AlterEntityInstance;
using EmissaryABIProtos.Calls.ApplyPhysicsToEntityInstance;
using EmissaryABIProtos.Calls.GenerateStateChange;
using EmissaryABIProtos.Calls.RegisterEntityHandlers;
using EmissaryABIProtos.Calls.RequestDestroyEntityInstance;
using EmissaryABIProtos.Calls.SpawnNewEntityInstance;
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

        #region ABI Calls

        // Call via interop

        protected  AlterEntityInstanceReturn AlterEntityInstance(AlterEntityInstanceCall call)
        {
            return new AlterEntityInstanceReturn();
        }

        protected  ApplyPhysicsToEntityInstanceReturn ApplyPhysicsToEntityInstance(ApplyPhysicsToEntityInstanceCall call)
        {
            return new ApplyPhysicsToEntityInstanceReturn();
        }

        protected RequestDestroyEntityInstanceReturn RequestDestroyEntityInstance(RequestDestroyEntityInstanceCall call)
        {
            return new RequestDestroyEntityInstanceReturn();
        }


        protected GenerateStateChangeReturn GenerateStateChange(GenerateStateChangeCall call)
        {
            return new GenerateStateChangeReturn();
        }

        protected  RegisterEntityHandlersReturn RegisterEntityHandlers(RegisterEntityHandlersCall call)
        {
            return new RegisterEntityHandlersReturn();
        }
        protected  SpawnNewEntityInstanceReturn SpawnNewEntityInstance(SpawnNewEntityInstanceCall call)
        {
            return new SpawnNewEntityInstanceReturn();
        }
        protected  SubscribeToChannelReturn SubscribeToChannel(SubscribeToChannelCall call)
        {
            return new SubscribeToChannelReturn();
        }



        #endregion
        #region ABI Callbacks to Entity

        // We will be called via interop and will dispatch via DispatchOffset (virtual functions)

        public virtual DestroyEmissaryReturn DestroyEmissary(DestroyEmissaryCall call) { return new(); }
        public virtual DestroyEntityHandlerReturn DestroyEntityHandler(DestroyEntityHandlerCall call) { return new(); }
        public virtual DestroyEntityInstanceReturn DestroyEntityInstance(DestroyEntityInstanceCall call) { return new(); }
        public virtual EntityInstanceUpdateReturn EntityInstanceUpdate(EntityInstanceUpdateCall call) { return new(); }
        public virtual EntityInstanceVersionUpdateReturn EntityInstanceVersionUpdate(EntityInstanceVersionUpdateCall call) { return new(); }
        public virtual InstantiateEntityInstanceReturn InstantiateEntityInstance(InstantiateEntityInstanceCall call) { return new(); }
        public virtual ReceiveInstanceStateChangeReturn ReceiveInstanceStateChange(ReceiveInstanceStateChangeCall call) { return new(); }
        public virtual ReceiveMessageFromChannelReturn ReceiveMessageFromChannel(ReceiveMessageFromChannelCall call) { return new(); }

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
                    ret.DestroyEmissaryReturn = entity.DestroyEmissary(call.DestroyEmissaryCall);
                    return ret;
                case ABI_Callbacks_Calls.CallsOneofCase.DestroyEntityHandlerCall:
                    ret.DestroyEntityHandlerReturn = entity.DestroyEntityHandler(call.DestroyEntityHandlerCall);
                    return ret;
            }

            throw new Exception($"Unknown Callback: {call.CallsCase} - entity {call.DispatchOffset}");
        }


        #endregion
    }
}
