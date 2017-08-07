# Changelog

## [Unreleased]

## [2.0.1] - 2017-08-07

### Added

- Add the `DisconnectClient(Guid clientId)` method to `NetServer`.

### Changed

- Changed `ConcurrentBag` to `ConcurrentDictionary` for client handling.

## [Released]

## [2.0.0] - 2017-08-01

### Added

- Add netstandard1.3 support
- Add NetServerConfiguration class. Provide properties to configuration a NetServer.

### Changed

- NetServer socket management. Now using [SocketAsyncEventArgs](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs(v=vs.110).aspx) for scalability and performance.
- NetClient socket management. Now using [SocketAsyncEventArgs](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs(v=vs.110).aspx) for scalability and performance.

### Removed

- Remove .NET Framework 4.6 support
- Remove .NET Framework 4.6.1 support
- Remove NetDelayer system

## [1.1.7] - 2017-01-11

### Changed

- Fix `NetClient` critical issues

## [1.1.6] - 2017-01-05

### Added

- Add support for .NET Framework 4.5
- Add support for .NET Framework 4.5.1
- Add support for .NET Framework 4.6
- Add support for .NET Framework 4.6.1

## [1.1.5] - 2016-12-14

### Changed

- `NetClient` connect method returns a `bool`
- `NetServer<T>` client list is a readonly list
- `NetServer<T>` `OnClientConnected` and `OnClientDisconnected` methods takes `T` as parameter.

## [1.1.0] - 2016-10-10

### Added


### Changed


## [1.0.0] - 2016-11-03

### Added

- `NetServer<T>` fully managed server handling `T` connection where `T` is a `NetConnection`
- `NetClient` fully managed client.
- `NetPacket` packet management.
