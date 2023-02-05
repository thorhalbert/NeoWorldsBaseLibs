# Emissary Container format

Uncompressed tar file (we might permit inputs to be compressed with gzip or bz2, though we must decompress it to make it seekable).

## Directories:

- **_admin/**       Buildout manifest and other setup/configuration files
- **_code/**        Binary Code
- **_data/**        Data files (graphics, materials, meshes)
- **_src/**         Source Modules (stripped from built version)

