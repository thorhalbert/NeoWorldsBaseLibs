using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using EmissaryABIProtos;
using EmissaryABIProtos.Calls.AlterEntityInstance;

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

        // AlterEntityInstance

        protected virtual AlterEntityInstanceReturn AlterEntityInstance(AlterEntityInstanceCall call)
        {
            return new AlterEntityInstanceReturn();
        }
        // ApplyPhysicsToEntityInstance
        // DestroyEntityInstance
        // GenerateStateChange
        // RegisterEntityHandlers
        // SpawnNewEntityInstance
        // SubscribeToChannel

        #endregion
        #region ABI Callbacks to Entity

        // We will be called via interop and will dispatch via DispatchOffset (virtual functions)

        // DestroyEmissary
        // DestroyEntityHandler
        // DestroyEntityInstance
        // EntityInstanceUpdate
        // EntityInstanceVersionUpdate
        // InstantiateEntityInstance
        // ReceiveInstanceStateChange
        // ReceiveMessageFromChannel

        #endregion
    }
}
