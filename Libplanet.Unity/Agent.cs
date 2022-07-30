#nullable disable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Renderers;
using Libplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Node;
using Libplanet.Store;
using Libplanet.Tx;
using NetMQ;
using UnityEngine;

namespace Libplanet.Unity
{
    /// <summary>
    /// Agent runs <see cref="Miner"/>, <see cref="SwarmRunner"/> and <see cref="ActionWorker"/>
    /// You can use <c>RunOnMainThread</c>, <c>MakeTransaction</c> to manage actions.
    /// </summary>
    [SuppressMessage(
        "SonarLint.Bug",
        "S3453",
        Justification = "It's only instantiated by GameObject.AddComponent<T>() method.")]
    public class Agent : MonoBehaviour
    {
        private Miner _miner;
        private Coroutine _minerCo;

        private Swarm<PolymorphicAction<ActionBase>> _swarm;

        private SwarmRunner _swarmRunner;
        private Coroutine _swarmRunnerCo;

        private ActionWorker _actionWorker;
        private Coroutine _processActionsCo;

        private BlockChain<PolymorphicAction<ActionBase>> _blockChain;

        /// <summary>
        /// Use <see cref="AddComponentTo"/> instead.
        /// </summary>
        private Agent()
        {
        }

        /// <summary>
        /// Address of the <see cref="PrivateKey"/>.
        /// </summary>
        public Address Address { get; private set; }

        private PrivateKey PrivateKey { get; set; }

        /// <summary>
        /// Instantiates <see cref="Agent"/> and add it to the specified
        /// <paramref name="gameObject"/> as its component.
        /// </summary>
        /// <param name="gameObject">A <see cref="GameObject"/> to add a new <see cref="Agent"/> as
        /// its component.</param>
        /// <param name="renderers">Renderers that listen to chain state updates.</param>
        /// <returns>A new <see cref="Agent"/> instance.</returns>
        public static Agent AddComponentTo(
            GameObject gameObject,
            IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> renderers
        )
        {
            Agent self = gameObject.AddComponent<Agent>();
            PrivateKey privateKey = Utils.LoadPrivateKey(Paths.PrivateKeyPath);
            self.PrivateKey = privateKey;
            self.Address = privateKey.ToAddress();

            self.ConfigureNode(renderers);
            self._miner = new Miner(self._swarm, self.PrivateKey);
            self._actionWorker = new ActionWorker(self._swarm, self.PrivateKey);
            return self;
        }

        /// <summary>
        /// Returns the state of <paramref name="address"/> for the current
        /// <see cref="BlockChain{T}.Tip"/>.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> to look up.</param>
        /// <returns>The state of <paramref name="address"/> at <see cref="BlockChain{T}.Tip"/>.
        /// This can be <see langword="null"/> if <paramref name="address"/>
        /// has no value.</returns>
        public IValue GetState(Address address) => GetState(address, _blockChain.Tip.Hash);

        /// <summary>
        /// Returns the state of <paramref name="address"/> for <paramref name="blockHash"/>
        /// <see cref="Block{T}"/>.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> to look up.</param>
        /// <param name="blockHash">The <see cref="BlockHash"/> of the <see cref="Block{T}"/>
        /// to look up.</param>
        /// <returns>The state of <paramref name="address"/> at <paramref name="blockHash"/>.
        /// This can be <see langword="null"/> if <paramref name="address"/>
        /// has no value.</returns>
        public IValue GetState(Address address, BlockHash blockHash)
        {
            return _blockChain.GetState(address, blockHash);
        }

        /// <summary>
        /// Creates a <see cref="Transaction{T}"/> with <paramref name="actions"/>
        /// Just use <see cref="ActionWorker"/> MakeTransaction.
        /// </summary>
        /// <param name="actions">The list of <see cref="PolymorphicAction{ActionBase}"/>.</param>
        public void MakeTransaction(IEnumerable<PolymorphicAction<ActionBase>> actions)
        {
            _actionWorker.MakeTransaction(actions);
        }

        /// <summary>
        /// Append action in <see cref="ActionWorker"/>.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to be use.</param>
        public void RunOnMainThread(System.Action action)
        {
            _actionWorker.Append(action);
        }

        /// <summary>
        /// Dispose <see cref="Swarm{T}"/> and <see cref="NetMQConfig"/> clean up.
        /// </summary>
        protected void OnDestroy()
        {
            NetMQConfig.Cleanup(false);
            _swarm?.Dispose();
        }

        private void Start()
        {
            _swarmRunner.Preloaded += StartMiner;
            _swarmRunnerCo = StartCoroutine(_swarmRunner.CoSwarmRunner());
            _processActionsCo = StartCoroutine(_actionWorker.CoProcessActions());
        }

        /// <summary>
        /// Function for start miner when Preloaded completed.
        /// </summary>
        private void StartMiner()
        {
            Debug.Log("Triggered IsPreloadedChanged : Start Miner");
            _minerCo = StartCoroutine(_miner.CoStart());
        }

        private void OnApplicationQuit()
        {
            StopCoroutine(_processActionsCo);
            _processActionsCo = null;
            StopCoroutine(_minerCo);
            _minerCo = null;
            StopCoroutine(_swarmRunnerCo);
            _swarmRunnerCo = null;
        }

        private void ConfigureNode(
            IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> renderers)
        {
            SwarmConfig swarmConfig = Utils.LoadSwarmConfig(Paths.SwarmConfigPath);
            Block<PolymorphicAction<ActionBase>> genesis = Utils.LoadGenesisBlock(
                Paths.GenesisBlockPath);
            (IStore store, IStateStore stateStore) = Utils.LoadStore(Paths.StorePath);

            var nodeConfig = new NodeConfig<PolymorphicAction<ActionBase>>(
                PrivateKey,
                new NetworkConfig<PolymorphicAction<ActionBase>>(
                    NodeUtils<PolymorphicAction<ActionBase>>.DefaultBlockPolicy,
                    NodeUtils<PolymorphicAction<ActionBase>>.DefaultStagePolicy,
                    genesis),
                swarmConfig,
                store,
                stateStore,
                renderers);
            _swarm = nodeConfig.GetSwarm();
            _swarmRunner = new SwarmRunner(_swarm, PrivateKey);

            _blockChain = _swarm.BlockChain;
        }
    }
}
