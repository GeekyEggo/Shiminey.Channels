[![Build status badge](https://github.com/GeekyEggo/Shiminey.Channels/workflows/build/badge.svg)](https://github.com/GeekyEggo/Shiminey.Channels/actions?query=workflow%3Abuild)
![Test coverage badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/geekyeggo/ea47b909ef1163bf584b62b1f80f7496/raw/shiminey.channels-coverage.json)
[![Shiminey.Channels verion on NuGet.org](https://img.shields.io/nuget/v/Shiminey.Channels.svg)](https://www.nuget.org/packages/Shiminey.Channels)
[![Twitter icon](https://img.shields.io/badge/GeekyEggo--brightgreen?style=social&logo=twitter)](https://www.twitter.com/geekyeggo)

## 🧬 Shiminey.Channels

Shiminey.Channels is a lightweight set of classes designed to extend and complement the existing [System.Threading.Channels](https://www.nuget.org/packages/System.Threading.Channels) library, with a focus on allowing greater control of ordering elements within a channel.

## 📝 Use Case

Imagine an application that allows users to download files concurrently; the application also allows users to define a degree of parallelism, i.e. 4 downloads can happen at the same time.

The application utilizes a single channel that contains pending downloads, however, what were to happen if the user wished to change the priority of these downloads, i.e. skip or "download now"? Using the default implementation of channels we aren't able to do anything, however with Shiminey, we can solve this problem.

## 🔀 How It Works

In addition to the standard `TryWrite(T item)` offered from channel writers, Shiminey exposes a new `TryWriteOrderable(T item, out IChannelItemController controller)` method. With this `controller` it becomes possible to re-position the data within the channel:

- ⏭️ `TryMoveFirst()` - _Attempts to move the item to the front._
- ⏩ `TryMoveForward()` - _Attempts to increase the priority of the item by bring it forward one place._
- ⏮️ `TryMoveLast()` - _Attempts to move the item the end._
- ⏪ `TryMoveBackward()` - _Attempts to decrease the priority of the item by moving it backwards one place._

## 🎉 Basic Usage

```csharp
using Shiminey.Channels;

// Create a channel, and write two items to it.
var channel = Channel.CreateUnboundedOrderableChannel<string>();
channel.Writer.TryWrite("One");
channel.Writer.TryWriteOrderable("Two", out var twoController);

// The controller allows us to relocate the item within the channel.
twoController.TryMoveFirst();

string item;
item = await channel.Reader.ReadAsync(); // Two
item = await channel.Reader.ReadAsync(); // One
```