EnigmaDb
========

EnigmaDb is a document database engine for .NET used to store entire entities in an easy way.
It's designed primary as an embedded service.

It relies on the following thirdparty open source libraries
- ProtoBuf.NET
- Remotion.Linq

Right now it's only tested on Windows. In the future the following is planned to be tested and supported.
- Windows
- Linux, Mono
- Windows Phone
- Android, Monodroid
- iOS, Monotouch

What's working right now?
- The storage of data
- Truncating data with a method call
- Indexes on simple properties on top level node of the entities
- Data retrieval through LINQ
- Inmemory storage, great for unittests

What's on the roadmap?
- Indexes that works with all type of properties
- Index rebuilding
- Ability to add jobs that runs at a scheduled time
- Maintainance job that truncates database at night and rebuilds indexes
- Better lock management to improve write performance
