# Emissary Container format

Uncompressed tar file (we might permit inputs to be compressed with gzip or bz2, though we must decompress it to make it seekable).

## Directories:

- **_adm/** - Buildout manifest and other setup/configuration files
- **_bin/** - Binary Code
- **_dat/** - Data files (graphics, materials, meshes)
- **_src/** - Source Modules (stripped from built version)
- **_pro/** - Protobuf definitions for private communications

The outer tar file cannot be compressed, but we might permit inner files to be compressed as needed.
We may permit inner files to be compressed, but do note that this makes it reasonably impossible to random access inside such a file.

### _adm

This contains files data to validate and load the binary files.  

- EmissaryProject.yaml - setup for the project, entity server to get signatures from and upload results, and possible parent project
- Manifest.yaml - generated from the builder, including signed files and hashes - this is the one file that is not signed.   If a file is not validly 
signed in this file then it will not be used, visible or available.
- LoadPackages.yaml - digital BOL, part of the linking process.   It can be used to load in other emissaries as packages.
- Linker.yaml - special instructions for wasm linking, handles micro-management on the APIs

### _bin

This directory should not be checked into git (need this in the .gitignore in the solution base dir).  This will generally contain the webassembly
binary WAT files.   These are loaded via the various yaml files in _adm.

### _dat

Any data files needed for operations in wasm (pictures, meshes, textures, raw data, whatever your code needs).   The filenames are root 
relative - /_dat/file - these are extremely sandboxed.
Programs can only (for now) read this directory (and subdirectories).
These can also be used as external packages if there is common data between different emissaries (efficiency, reuse, and data reduction).

### _src

This will contain the sources, in whatever languages.   There might be other setup directives for the builder.
This will not be copied into the actual final emissary.

There are actually a couple of different kinds of emissary roles for an entity:

- Arena - The code that actually gets called by the graphics engine.  These will likely mirror the methods that the callbacks that are required.
GoDot has different kinds of callbacks (unlike Unity) -- so we need to handle that polymorphism.
- Helper - This is code that runs on the local Arena cluster, and not inside the engine itself -- usually communicates via local grpc.
- Server - Code that runs on the actual entity server -- these should be extremely limited in usage since they could be very remote.

So, the loading yaml will determine all this.   It is possible that the local Arena emissaries and the Server emissaries are separate (it is permitted
for Server code to stay private to the implemenation of the Entities).   Builder might build two different emissary modules (also to minimize what emissary
download size/time).  Though it is also possible
for Server code to link in non-wasm, non-sandboxed code--this is up to implementers.   The builder will have to figure this out.

### _pro

Contains private protobuf definitions - essentially another type of source, once we can figure out a wasm, language independant way of linking.
This is likely to be used as some kind of external package (in LoadPackages).  Of course, the canonical definitions are also loaded, though these might 
simply linked in at the wasm level.

