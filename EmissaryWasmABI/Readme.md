### Emissary ABI

Trying to come up with a way to encapsulate the interface between the emissary and the arena through
the wasm interface.  I thought this dll would cause an import reference in wasm and generate an import
that I'd have to satisfy.  But that's not how wasmtime/wasm compiling works.  Surprisingly enough this
dll ended up compiled into the final wasm code and ran just fine, including some protobuf marshal
and calling it's serializer.

So, changing my strategy, this dll will become a wrapper for an emissary-side 'rabbit-hole' which will basically be 
a de-facto rpc.   All I really need to accomplish is managing to get a new 'import' into the wasm.
I think maybe we can even manage to just have one universal call...

https://github.com/SteveSandersonMS/dotnet-wasi-sdk/issues/30

The call would have something like three arguments, the ABI type, the ABI task and the in/out buffer for the protobuf bytes.
Eventually it would be nice to not have to serialize/deserialize the records, but I suspect this is really
already very efficient, and going through the process handles versioning, which is vital.   I won't get
to directly implement the GoDot libraries through the wasmtime rabbit-hole but will have to wrap it in one 
of these protobuf ABIs.  Wasm apperently has plans for this, with a system called WIT, but for me, a gRPC like
interface (even if it's just protobuf) will be a giant win.   We'll have to get to the point where we can 
compute performance benchmarks once wasm for c# is more than experimental.

So now I just need to write a little bit of C to handle Sanderson's trick to generate a wasm import.

Also, noted that I can completely implement the WASI myself, so I might take a stab at that--I already reverse engineered
all of the Module.Define signatures.   Doing WasmTime binding
against the 'Store' is complex and I don't completely understand it, but would be a nice way to handle the virtualized
disk space of an emissary (this is not as complex as writing a fuse filesystem for C#, which I've done a couple of times).   
Directly seeking into the proper tar file and seeking around is easy and fast -- in some 
ways it may be faster than a regular disk.   The index, even for an extremely complex multi-emissary filesystem will
be stored in a read-only sqlite file (for now), so it will be very fast.   For this, just need to build out the caching
system which the arena browser and the entity server will use.  The builder will be later -- for now these will be 
manually generated--this can conceivably be done as a visual studio plugin and a project template.  You can build a big
solution containing all of your emissaries/entities as projects.   These get pushed up to the entity server which acts
somewhat like a private docker registry - and get their digital signatures in the process.

Here's a fine template - mentioned in the link above:

https://github.com/flavio/wapc-guest-dotnet

We'll bend the project to look like this.   Curious how it builds the C portion (with mono includes in it).
