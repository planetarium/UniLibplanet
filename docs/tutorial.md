# Tutorial

This tutorial will walk you through creating a basic blockchain application
called Clicker.  Users will be able to do the following:

- Run a local blockchain node.
- Create transactions containing `ClickAction` actions.
- Have blocks mined containing transactions, if any[^1].
- Have mined blocks added to the local blockchain.
- Update the UI counting the total number of clicks once `ClickAction` actions
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


## Actions and States

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

### State `CountState`

In order to store the total number of clicks for a player, we essentially
wrap `long` inside a class called `CountState` that inherits `DataModel`.
Create a new file named `CountState.cs` under `Assets/Scripts/States/`
with the following content.

```csharp
using Libplanet.Store;
using Scripts.Data;

namespace Scripts.States
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

        // Used for deserializing a stored state.
        // This must be declared as base constructor cannot be inherited.
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

### Plain value `ClickActionPlainValue`

In order to store the number of clicks for an action, its arguments should also
be encoded in [Bencodex] format.  For this purpose, similar to `CountState`,
we need to wrap `long` inside a class called `ClickActionPlainValue`.
Create a new file named `ClickActionPlainValue.cs` under
`Assets/Scripts/Actions/` with the following content.

```csharp
using Libplanet.Store;

namespace Scripts.Actions
{
    public class ClickActionPlainValue : DataModel
    {
        public long Count { get; private set; }

        public AddCountPlainValue(long count)
            : base()
        {
            Count = count;
        }

        // Used for deserializing stored action.
        public AddCountPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}
```

Although there doesn't seem to be much of a difference between `CountState`
and `ClickActionPlainValue` classes, the two have completely different
uses and it is **highly advisable** to have these to be separate.

### Action `ClickAction`

Now create a file named `ClickAction.cs` under `Assets/Scripts/Actions`
with the following content for `ClickAction` action.

```csharp
using System;
using Libplanet.Action;
using Libplanet.Unity;
using Scripts.State;
using UnityEngine;

namespace Scripts.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("click_action")]
    public class ClickAction : ActionBase
    {
        private ClickActionPlainValue _plainValue;

        // Used for reflection when deserializing a stored action.
        public ClickAction()
        {
        }

        // Used for creating a new action.
        public ClickAction(long count)
        {
            _plainValue = new ClickActionPlainValue(count);
        }

        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (plainValue is Bencodex.Types.Dictionary bdict)
            {
                _plainValue = new AddCountPlainValue(bdict);
            }
            else
            {
                throw new ArgumentException($"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
            }
        }

        // Executes an action.
        // This is what gets called when a block containing an action is mined or appended to a blockchain.
        public override IAccountStateDelta Execute(IActionContext context)
        {
            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            CountState countState = states.GetState(context.Signer) is Bencodex.Types.Dictionary countStateEncoded
                ? new CountState(countStateEncoded)
                : new CountState(0L);

            // Mutates the loaded state, logs the result, and stores the resulting state.
            long prevCount = countState.Count;
            countState.AddCount(_plainValue.Count);
            long nextCount = countState.Count;
            Debug.Log($"click_action: PrevCount: {prevCount}, NextCount: {nextCount}");
            return states.SetState(context.Signer, countState.Encode());
        }
    }
}
```

As can be seen in the comments above, most of the code above is to automate
serialization and deserialization.  Pay special attention to the `Execute()`
method where the main logic of the action resides.

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
