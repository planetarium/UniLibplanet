# Tutorial

This tutorial will walk you through creating a basic blockchain application
called Clicker.  Users will be able to do the following:

- Run a local blockchain node.
- Create transactions containing `Click` actions.
- Have blocks mined containing transactions, if any.[^1]
- Have mined blocks added to the local blockchain.
- Update the UI counting the total number of clicks once `Click` actions
  are evaluated by the local blockchain.

## Initial Setup

<!-- This subsection should be rewritten for importing an SDK package -->
### Development Environment Setup

To ease the initial setup process, such as packaging `dll` files etc.,
we will be using [this template][template] as a starting point.  You can
also check the finished sample project [here][sample].

First and foremost, install [Unity Hub][^2] first.  Once finished,
clone [this][template] repository using [git] to any location you like.

Open installed [Unity Hub] and from `Open` :arrow_forward:
`Add project from disk`, select `Clicker` directory and select `Add project`.
Once `Clicker` project is added to the list of projects, click on the project
to open.  You will be prompted to choose a Unity Editor version.  If you don't
have Unity Editor version 2021.3.0f1 already installed, install it first and
select the version from the list and press `Open with 2021.3.0f1`.[^3]

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
under `Tools` :arrow_forward: `Libplanet`.

<!-- footnotes -->

----

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
[Bencodex.Net]: https://github.com/planetarium/bencodex.net
