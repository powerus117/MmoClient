# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2020.3 MMO client that connects to a server via raw TCP sockets. Uses protobuf-net for wire serialization and a shared message library (`MmoShared.dll`) that defines the protocol. The server project lives in a separate repository.

## Key Libraries & Frameworks

- **Zenject** - Dependency injection. All bindings are registered in `MonoInstaller` subclasses under `Assets/Scripts/Installers/`.
- **UniRx** - Reactive extensions. `ReactiveProperty<T>`, `ReactiveDictionary<K,V>`, `CompositeDisposable`, and `ObservableExtensions` are used throughout for data binding and UI event handling.
- **DOTween** - Animation tweening (player movement/rotation).
- **Addressables** - Async asset loading for dynamically spawned UI windows. Prefabs marked as Addressable are loaded/released via `UIService`.
- **protobuf-net** - Binary serialization of network messages (length-prefixed Base128).
- **MmoShared.dll** - Pre-compiled shared library (not source in this repo) containing `Message`, `MessageId`, `Vector2I`, `UserInfo`, and all `*Sync`/`*Notify` message types. Types from namespaces like `MmoShared.Messages.*` and `MmoServer.Core` come from this DLL.

## Architecture

### Scene Flow

`Project` scene (bootstrap) -> `Login` scene -> `Game` scene. `ProjectSceneInstaller` waits for DI to finish, then loads Login. Successful login fires `LoggedInSignal` which triggers loading the Game scene.

### Dependency Injection Hierarchy

- **ProjectInstaller** (project-scoped singleton): `ConnectionManager`, `MessageReceiver`, `MessageSender`, `SceneLoader`, `SignalManager`, `UIService`, `PlayerService`, `LoginService`. These survive scene transitions.
- **LoginInstaller** (scene-scoped): Login UI presenters bound from scene instances.
- **GameInstaller** (scene-scoped): `PlayerManager`, `PlayerController` bound from scene instances.

### Networking

TCP connection managed by `ConnectionManager`. Messages flow through:
1. **Outbound**: Service calls `IMessageSender.Send(Message)` -> `ConnectionManager.SendMessage` -> `Connection.AddMessage` (writes `ushort` message ID + protobuf payload).
2. **Inbound**: `MessageReader` runs a dedicated read thread calling `Connection.ReadMessages` (reads `ushort` ID, deserializes via `RuntimeTypeModel`). A polling loop on the main thread calls `Connection.ProcessMessages` which dispatches to `IMessageReceiver` subscribers.
3. **Message discovery**: `MessageTypeHelper` scans all assemblies at startup via reflection to build `MessageId -> Type` map.

Convention: outbound messages are `*Notify`, inbound messages are `*Sync`.

### Custom Signal Bus

`SignalManager` is a lightweight pub/sub for local (non-network) events. Signals implement `ISignal`. Subscribe/unsubscribe with `ISignalManager.Subscribe<T>/Unsubscribe<T>`. This is separate from Zenject's built-in signal system.

### MVP Pattern

UI follows Model-View-Presenter. Presenters are MonoBehaviours that receive injected services and bind to reactive models. Examples: `LoginScreenPresenter` + `LoginScreenModel`, `PlayerPresenter` + `PlayerModel`.

### UI Window System

`UIService` (project-scoped singleton) manages dynamic UI windows loaded via Unity Addressables. Windows extend `UIWindow` (or `UIWindow<TParams>` for parameterized windows) and are opened by Addressable address string via `IUIService.Open<TWindow>(address, layer)`.

- **Layers**: `UILayer` enum (`Default=100`, `Dialog=200`, `Overlay=300`, `System=400`) maps to Canvas `sortingOrder`. Each layer can hold one active window.
- **Queuing**: If a window is already active on a layer, new requests to that layer are queued and opened when the current window closes.
- **Priority interruption**: Opening a window on a higher layer hides (via `CanvasGroup`) active windows on lower layers. They are revealed when the higher-priority window closes.
- **Addressable keys**: Centralized in `UIAddresses` static class. Prefabs live in `Assets/Prefabs/UI/`.
- **Window lifecycle**: `OnOpened()`, `OnClosed()`, `OnHidden()`, `OnRevealed()`. Close via `UIWindow.Close()`.

### Grid System

`WorldGrid` converts between `Vector2Int` grid positions and `Vector3` world positions (offset by 0.5 to center in cells). `Vector2IExtensions` bridges between Unity's `Vector2Int` and the shared library's `Vector2I`.

## Configuration

Server connection settings (IP, port) are in the ScriptableObject at `Assets/Resources/Configs/Connection/ConnectionConfig.asset` (default: `localhost:7800`).

## NuGet Packages

Managed via `packages.config` at the repo root. DLLs live in `Assets/Plugins/` and `Packages/`. Key packages: `protobuf-net 3.1.22`, `Newtonsoft.Json`, `System.Text.Json`.

## Conventions

- C# namespaces mirror the folder structure under `Assets/Scripts/`.
- Services are constructor/field-injected via `[Inject]` and implement `IInitializable`/`IDisposable` for lifecycle.
- UI event subscriptions use UniRx `.OnClickAsObservable()` pattern with `CompositeDisposable` for cleanup in `OnDestroy`.
- Network message handlers are subscribed in `Initialize()` and unsubscribed in `Dispose()`.
