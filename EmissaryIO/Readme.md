### Emissary side of IO Implementation

Implement as an interface, at least in the c# version

This will likely have to implement the actual implementation of the interface elsewhere.
This gets linked into the WASMTime load so it may be called by emissary code.

For now, the only emissary directory which is readable is the _dat directory.  The others are not exposed to the filesystem.

Also, for now, all files are read-only and immutable.   We'll decide if we want some sort of read/write file model later.  For now, entities and 
instances should be doing permanent persistence on the world-state of the entity object or delegating this to the entity server (though one
should always consider the entity server to be remote and very slow and handle the possibily that it is unreachable).

Also, these are provided since we can't see yet that we can override/virtualize the WASI filesystems in WASMTime.   So, likely WASI file
access will not be enabled.  It would be nice if WasmTime would expose the different WASI abstractions as interfaces which could be 
implemented any way we want.  Not sure how easy it would be to do this in C# from the low level implementation in Rust.  A true filesystem
abstraction, you could just use all of the normal file operations.

- File open (binary and text) - will return a stream.
- Directory lookup
- Current directory

There is another possibily, of simply hijacking the StreamReader class and reimplementing it.   I'll have to see if this is practical in
WasmTime.  Taking a quick look at StreamReader, FileStream implementations in Mono -- this might not be impossible.  We could even theoretically
support async i/o.

Have to come up with a tool which binds a class implementation into the linker with reflection.   

This will have to happen with the implementation of the GoDot libraries.   We would like it if we can instantiate and make callbacks for GoDot
objects inside an emissary and have them act the same way as they would as if they were inside (and hopefully without much of a performance degradation).
This would also allow you do this in other languages, though intially we want to do this with c# (though python would be great).
