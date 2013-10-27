Version 0.1.2
=============
Indexes are now stable to use with the compare algorithm on the top level for filtering.

- Fixed a couple of issues when indexes was loading that could corrupt the index
- Rework in the index architecture
- Minor improvement to locks in the binary store
- Index rebuild if an index file is removed or if the index was created after the database
- Relies a bit more on the OS file buffering to improve stability

Version 0.1.1
=============
- Updated Remotion.Linq and ProtoBuf.NET to the latest versions available

Version 0.1.0
=============
- First release, entered GitHub and Nuget as a package
