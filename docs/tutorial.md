# Tutorial

This tutorial will walk you through creating a basic blockchain application
called Clicker.  Users will be able to do the following:

- Run a local blockchain node.
- Create transactions containing `Click` actions.
- Have blocks mined containing transactions, if any[^1].
- Have mined blocks added to the local blockchain.
- Update the UI counting the total number of clicks once `Click` actions
  are evaluated by the local blockchain.


## Initial Setup

<!-- This subsection should be rewritten for importing an SDK package -->
### Development Environment Setup

To ease the initial setup process, such as packaging `dll` files etc.,
we will be using [this template][template] as a starting point.  You can
also check the finished sample project [here][sample].

First and foremost, install [Unity Hub] first[^2].  Once finished,
clone [this][template] repository using [git] to any location you like.

Open installed [Unity Hub] and from `Open` → `Add project from disk`,
select `Clicker` directory and select `Add project`.  Once `Clicker` project
is added to the list of projects, click on the project to open.  You will be
prompted to choose a Unity Editor version.  If you don't have Unity Editor
version 2021.3.0f1 already installed, install it first and select the version
from the list and press `Open with 2021.3.0f1`[^3].

### Blockchain Node Setup

In order to run a [Libplanet] blockchain node, the following three are needed:

- A genesis block: This is the very first block of a blockchain.  Only nodes
  with the same genesis block can properly communicate with each other.
- A swarm configuration: A `json` file that determines the behavior of a node,
  This is outside the scope of this tutorial.
- A private key: This determines the identity of a node.  All transactions
  created and all blocks mined will be signed using this private key.

These three files can be easily created using Unity Editor menu.  Create each
using `Create genesis block`, `Create swarm config`, and `Create private key`
under `Tools` → `Libplanet`.


## States and Actions

Three componenets are needed to create an action for a [Libplanet] blockchain
to consume:

- Action itself.
- Arguments for an action.  This is stored on **Blockchain** store.
- Output for an action.  This is stored on **State** store.

An action is any [C#] object implementing an `IAction` interface.
Since we can't directly record a [C#] object to a blockchain,
when we say an action is recorded on a blockchain, what it technically means
is its arguments, together with its name to identify which action took place,
is written.

As a result of executing an action, i.e. being recorded on a blockchain,
changes the game state by writing the output of the action to a stored state.

In both cases, data is encoded in [Bencodex] format.

### `CountState`

In order to store the total number of clicks for a player, we essentially
wrap `long` inside a class called `CountState` that inherits `DataModel`.
Create a new file named `CountState.cs` under `Assets/Scripts/States/`
with the following content.

```csharp
using Libplanet.Store;
using Scripts.Data;

namespace Scripts.State
{
    public class CountState : DataModel
    {
        public long Count { get; private set; }

        // Used for creating a new state.
        public CountState(long count)
            : base()
        {
            Count = count;
        }

        // Used for deserializing stored state.
        public CountState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        // Used for adding `count` to the current state.
        public void AddCount(long count)
        {
            Count = Count + count;
        }
    }
}
```

As all data recorded on blockchain and the state storage in [Bencodex] format,
the `DataModel` class is there to help with all the heavy lifting of encoding
and decoding behind the scenes.

<!-- footnotes -->

---

[^1]: Blocks will be mined regardless of whether there are any transactions
      to mine or not.

[^2]: This tutorial is written with [Unity Hub] 3.1.2.

[^3]: At this moment, only Unity Editor version 2021.3.0f1 is supported.

<!-- links -->

[Unity Hub]: https://unity3d.com/get-unity/download
[template]: https://github.com/planetarium/libplanet-unity-template
[sample]: https://github.com/planetarium/planet-clicker/tree/sample
[git]: https://git-scm.com/
[Libplanet]: https://github.com/planetarium/libplanet
[C#]: https://docs.microsoft.com/en-us/dotnet/csharp
[Bencodex]: https://github.com/planetarium/bencodex
