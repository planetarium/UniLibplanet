using System;
using System.IO;
using System.Text;
using Libplanet.Crypto;
using Libplanet.Node;
using UnityEditor;
using UnityEngine;

namespace Libplanet.Unity.Editor
{
    /// <summary>
    /// Unity editor menu item for managing swarm config builder
    /// </summary>
    public class SwarmConfigBuilder : EditorWindow
    {
        private string _privateKeyString = string.Empty;
        private string _host = string.Empty;
        private int _port = 0;
        private string _boundPeerString = string.Empty;

        private string _swarmConfigJsonString;
        private static InitConfig _initConfig;
        
        // 1. StreamingAssets에서 swarm_config 이라는 json파일을 가져온다.
        // 2. 각각의 정보를 클래스에 담는다.
        // 3. 에디터에 뿌려준다. 

         /// <summary>
        /// Initialize editor window.
        /// </summary>
        [MenuItem("Tools/Libplanet/Utils/Edit swarm config file")]
        public static void Init()
        {
            
            string swarmConfigPath = $"{Application.streamingAssetsPath}/swarm_config.json";
            string _swarmConfigJsonString = File.ReadAllText(swarmConfigPath);
            
            SwarmConfig swarmConfig = SwarmConfig.FromJson(_swarmConfigJsonString);
            _initConfig = swarmConfig.InitConfig;

            const string title = "Edit swarm config file";

            var window = EditorWindow.GetWindowWithRect(
                typeof(SwarmConfigBuilder),
                new Rect(0, 0, 800, 200),
                true,
                title);
            window.Show();
        }

        /// <summary>
        /// Redraw on GUI event.
        /// </summary>
        public void OnGUI()
        {
            EditorGUILayout.LabelField("InitConfig", EditorStyles.boldLabel);

            SwarmConfig swarmConfig = new SwarmConfig();
            
            EditorGUILayout.IntField("MaxTimeout", (int)_initConfig.MaxTimeout.TotalSeconds);
            EditorGUILayout.IntField("MinTimeout",(int)_initConfig.MinTimeout.TotalSeconds);
            EditorGUILayout.IntField("RoutingTableNumBuckets", _initConfig.RoutingTableNumBuckets);
            EditorGUILayout.IntField("RoutingTableBucketSize", _initConfig.RoutingTableBucketSize);
            EditorGUILayout.IntField("BlockLocatorIndexSampleThreshold", _initConfig.BlockLocatorIndexSampleThreshold);
            EditorGUILayout.TextField("TransportType", _initConfig.TransportType.ToString());
            EditorGUILayout.TextField("Host", _host);
            EditorGUILayout.IntField("Port", _port);

            // draft 
            
            // // Zero port is excluded.
            // if (_port < 1 || _port > 65535)
            // {
            //     _boundPeerString = "Invalid port number";
            // }
            // else if (_host.Length < 1)
            // {
            //     _boundPeerString = "Invalid host";
            // }
            // else
            // {
            //     try
            //     {
            //         PrivateKey privateKey = new PrivateKey(ByteUtil.ParseHex(_privateKeyString));
            //         string publicKeyString = ByteUtil.Hex(privateKey.PublicKey.Format(true));
            //         _boundPeerString = $"{publicKeyString},{_host},{_port}";
            //     }
            //     catch (Exception)
            //     {
            //         _boundPeerString = "Invalid private key string";
            //     }
            // }

            // EditorGUILayout.Space();

            // EditorGUILayout.LabelField("Generated bound peer string", EditorStyles.boldLabel);
            // EditorGUILayout.SelectableLabel(_boundPeerString);
        }
    }
}