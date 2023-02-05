# Emissary Container format

Uncompressed tar file (we might permit inputs to be compressed with gzip or bz2, though we must decompress it to make it seekable).

## Directories:

- **_adm/** - Buildout manifest and other setup/configuration files
- **_bin/** - Binary Code
- **_dat/** - Data files (graphics, materials, meshes)
- **_src/** - Source Modules (stripped from built version)

### _adm

- EmissaryProject.yaml - setup for the project, entity server to get signatures from and upload results, and possible parent project
- Manifest.yaml - generated from the builder, including signed files and hashes
- LoadPackages.yaml - digital BOL, part of the linking process
- Linker.yaml - special instructions for wasm linking, handles micro-management on the APIs

This layer, might contain any of the private protobuf communications which would get compiled.  (This could live in _bin later or a special prot dir).

### _bin

This directory should not be checked into git (need this in the .gitignore in the solution base dir).

### _dat

Any files needed for operations in wasm.   The filenames are root relative - /_dat/file - these are extremely sandboxed.
Programs can only (for now) read this directory (and subdirectories).

### _src

This will contain the sources, in whatever languages.   There might be other setup directives for the builder.
This will not be copied into the actual final emissary.
