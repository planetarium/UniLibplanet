#nullable disable
using System;
using System.Collections.Generic;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Renderers;
using Libplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Node;
using Libplanet.Store;
using NetMQ;

namespace Libplanet.Unity
{
    /// <summary>
    /// Agent configure and manage <see cref="Miner"/>, <see cref="SwarmRunner"/> and <see cref="ActionWorker"/>
    /// </summary>
    public class Agent
    {
        private static readonly Lazy<Agent> Lazy =
                new Lazy<Agent>(() => new Agent());

        private Swarm<PolymorphicAction<ActionBase>> _swarm;

        private BlockChain<PolymorphicAction<ActionBase>> _blockChain;

        private Agent()
        {
        }

        /// <summary>
        /// The singleton instance of <see cref="Agent"/>.
        /// </summary>
        public static Agent Instance
        {
            get
            {
                return Lazy.Value;
            }
        }

        /// <summary>
        /// Configured <see cref="Miner"/> It can be use coroutine.
        /// </summary>
        public Miner Miner { get; private set; }

        /// <summary>
        /// Configured <see cref="SwarmRunner"/> It can be use coroutine.
        /// </summary>
        public SwarmRunner SwarmRunner { get; private set; }

        /// <summary>
        /// Configured <see cref="ActionWorker"/> It can be use coroutine.
        /// </summary>
        public ActionWorker ActionWorker { get; private set; }

        /// <summary>
        /// Address of the <see cref="PrivateKey"/>.
        /// </summary>
        public Address Address { get; private set; }

        private PrivateKey PrivateKey { get; set; }

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
        /// Dispose <see cref="Swarm{T}"/> and <see cref="NetMQConfig"/> clean up.
        /// </summary>
        public void Cleanup()
        {
            NetMQConfig.Cleanup(false);
            _swarm?.Dispose();
        }

        /// <summary>
        /// Initialize Agent.
        /// </summary>
        /// <param name="renderers">Listener to check status.</param>
        public void Initialize(
            IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> renderers)
        {
            ConfigureKeys();
            ConfigureNode(renderers);
            ConfigureMiner();
            ConfigureActionWorker();
        }

        private void ConfigureKeys()
        {
            PrivateKey = Utils.LoadPrivateKey(Paths.PrivateKeyPath);
            Address = PrivateKey.PublicKey.ToAddress();
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
            SwarmRunner = new SwarmRunner(_swarm, PrivateKey);

            _blockChain = _swarm.BlockChain;
        }

        private void ConfigureMiner()
        {
            Miner = new Miner(
                _swarm,
                PrivateKey);
        }

        private void ConfigureActionWorker()
        {
            ActionWorker = new ActionWorker(
                _swarm,
                PrivateKey);
        }
    }
}
