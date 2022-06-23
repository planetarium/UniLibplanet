using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Tx;
using UnityEngine;

namespace Libplanet.Unity
{
    /// <summary>
    /// Agent runs <see cref="Unity.Miner"/>, <see cref="Unity.SwarmRunner"/> and Action Controller
    /// You can use <c>RunOnMainThread</c>, <c>MakeTransaction</c> to manage actions.
    /// </summary>
    public class ActionWorker
    {
        private readonly ConcurrentQueue<System.Action> _actions =
            new ConcurrentQueue<System.Action>();

        /// <summary>
        /// The <see cref="Swarm{T}"/> to use for mining.
        /// </summary>
        private Swarm<PolymorphicAction<ActionBase>> _swarm;

        /// <summary>
        /// Initialize a <see cref="SwarmRunner"/> instance.
        /// </summary>
        /// <param name="swarm">The <see cref="Swarm{T}"/> to use.</param>
        /// <param name="privateKey">The <see cref="PrivateKey"/> to use.</param>
        public ActionWorker(
            Swarm<PolymorphicAction<ActionBase>> swarm,
            PrivateKey privateKey)
        {
            _swarm = swarm;
            PrivateKey = privateKey;
        }

        private PrivateKey PrivateKey { get; set; }

        /// <summary>
        /// Stage the action transactions.
        /// </summary>
        /// <param name="gameActions"><see cref="Action"/> list to be use.</param>
        public void MakeTransaction(IEnumerable<ActionBase> gameActions)
        {
            var actions = gameActions.Select(
                gameAction => (PolymorphicAction<ActionBase>)gameAction).ToList();
            Task.Run(() => MakeTransaction(actions, true));
        }

        /// <summary>
        /// Append action.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to be use.</param>
        public void RunOnMainThread(System.Action action)
        {
            _actions.Enqueue(action);
        }

        /// <summary>
        /// Append action.
        /// </summary>
        /// <returns>This can be <c>null</c>.</returns>
        public IEnumerator CoProcessActions()
        {
            while (true)
            {
                if (_actions.TryDequeue(out System.Action action))
                {
                    action();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private Transaction<PolymorphicAction<ActionBase>> MakeTransaction(
                    IEnumerable<PolymorphicAction<ActionBase>> actions, bool broadcast)
        {
            var polymorphicActions = actions.ToArray();
            Debug.LogFormat(
                "Make Transaction with Actions: `{0}`",
                string.Join(",", polymorphicActions.Select(i => i.InnerAction)));
            return _swarm.BlockChain.MakeTransaction(PrivateKey, polymorphicActions);
        }
    }
}
