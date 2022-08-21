using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Libplanet.Net;
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
        private static InitConfig _saveInitConfig;

        private static BootstrapConfig _bootstrapConfig;
        private static BootstrapConfig _saveBootstrapConfig;

        private static string swarmConfigPath = $"{Application.streamingAssetsPath}/";
        private static string swarmConfigFilename = "test_swarm_config.json";

         /// <summary>
        /// Initialize editor window.
        /// </summary>
        [MenuItem("Tools/Libplanet/Utils/Edit swarm config file")]
        public static void Init()
        { 
            string filePath = swarmConfigPath + swarmConfigFilename;
            if(File.Exists(filePath))
            {
                string _swarmConfigJsonString = File.ReadAllText(filePath);

                SwarmConfig swarmConfig = SwarmConfig.FromJson(_swarmConfigJsonString);
                _initConfig = swarmConfig.InitConfig;                
                _saveInitConfig = new InitConfig();

                _bootstrapConfig = swarmConfig.BootstrapConfig;
                _saveBootstrapConfig = new BootstrapConfig();

                const string title = "Edit swarm config file";
                var window = EditorWindow.GetWindowWithRect(
                    typeof(SwarmConfigBuilder),
                    new Rect(0, 0, 800, 500),
                    true,
                    title);
                window.Show();
            }
            else
            {
                EditorUtility.DisplayDialog("Alert", "Swarm config file is not exist.", "Close");
            }
             
        }

        
        
        /* InitConfig */
        int maxTimeout, minTimeout;
        int routingTableNumBuckets, routingTableBucketSize;
        int blockLocatorThreshold;
        int transportType;
        string host, port;
        List<string> iceServerUrls;

        /* BootstrapConfig */
        int searchDepth;
        int dialTimeout;
        List<string> seedPeerStrs;

        bool isFirst = true;
        /// <summary>
        /// Redraw on GUI event.
        /// </summary>
        public void OnGUI()
        {
            if(isFirst)
            {
                iceServerUrls = new List<string>();
                seedPeerStrs = new List<string>();

                foreach(IceServer iceServer in _initConfig.IceServers)
                {
                    iceServerUrls.Add(iceServer.Url.ToString());
                }

                foreach(BoundPeer seedPeer in _bootstrapConfig.SeedPeers)
                {
                    // string[] strArray = seedPeer.ToString().Split(".");
                    
                    seedPeerStrs.Add($"{seedPeer.PublicKey},{seedPeer.EndPoint.Host},{seedPeer.EndPoint.Port}");
                }
                isFirst = false;
            }

            /* InitConfig */
            DrawInitConfigGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            /* BootstrapConfig */
            DrawBootstrapConfigGUI();
            
            if(GUI.changed)
            {
                /* InitConfig */
                OnInitConfigGUIChanged();

                /* BootstrapConfig */
                OnBootstrapConfigGUIChanged();
            }
            
            EditorGUILayout.Space();
 
            if (GUILayout.Button("Save swarm_config.json"))
            {
                OnClickSaveButton();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Generated bound peer string", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(_boundPeerString);
        }

        private void DrawBootstrapConfigGUI()
        {
            EditorGUILayout.LabelField("Bootstrap Config", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SeedPeers", EditorStyles.label);
            
            // Add SeedPeers 버튼을 누른 경우
            if (GUILayout.Button("Add SeedPeer", GUILayout.Width(120)))
            {
                OnClickAddSeedPeerButton();
            }
            EditorGUILayout.EndHorizontal();

            for(int i = 0; i < seedPeerStrs.Count(); i++)
            {
                seedPeerStrs[i] = EditorGUILayout.TextField("", seedPeerStrs[i]);
            }
            searchDepth = EditorGUILayout.IntField("SearchDepth", _bootstrapConfig.SearchDepth);
            minTimeout = EditorGUILayout.IntField("DialTimeout", (int)_bootstrapConfig.DialTimeout.TotalSeconds);
        }

        private void OnBootstrapConfigGUIChanged()
        {
            _saveBootstrapConfig.SearchDepth = searchDepth;

            TimeSpan dialTimeSpan = new TimeSpan(0, 0, dialTimeout);
            _bootstrapConfig.DialTimeout = dialTimeSpan;
        }

        private void OnInitConfigGUIChanged()
        {
            TimeSpan maxTimeSpan = new TimeSpan(0, 0, maxTimeout);
            _saveInitConfig.MaxTimeout = maxTimeSpan;

            TimeSpan minTimeSpan = new TimeSpan(0, 0, minTimeout);
            _saveInitConfig.MinTimeout = minTimeSpan;

            _saveInitConfig.RoutingTableNumBuckets = routingTableNumBuckets;
            _saveInitConfig.BlockLocatorIndexSampleThreshold = blockLocatorThreshold;

            _saveInitConfig.TransportType = (SwarmOptions.TransportType)transportType;

            _saveInitConfig.Host = host.ToString();

            if (port != null)
            {
                try
                {
                    _saveInitConfig.Port = Int32.Parse(port);
                }
                catch (FormatException)
                {
                    // Debug.Log($"Unable to parse Port Number '{port}'");
                }
            }

            _initConfig = _saveInitConfig;
        }

        private void DrawInitConfigGUI()
        {
            EditorGUILayout.LabelField("InitConfig", EditorStyles.boldLabel);

            maxTimeout = EditorGUILayout.IntField("MaxTimeout", (int)_initConfig.MaxTimeout.TotalSeconds);
            minTimeout = EditorGUILayout.IntField("MinTimeout", (int)_initConfig.MinTimeout.TotalSeconds);

            routingTableNumBuckets = EditorGUILayout.IntField("RoutingTableNumBuckets", _initConfig.RoutingTableNumBuckets);
            routingTableBucketSize = EditorGUILayout.IntField("RoutingTableBucketSize", _initConfig.RoutingTableBucketSize);

            blockLocatorThreshold = EditorGUILayout.IntField("BlockLocatorIndexSampleThreshold", _initConfig.BlockLocatorIndexSampleThreshold);

            // TODO: Transport Type 삭제되었다는 소식
            transportType = EditorGUILayout.IntField("TransportType", (int)_initConfig.TransportType);

            host = EditorGUILayout.TextField("Host", _initConfig.Host);
            port = EditorGUILayout.TextField("Port", _initConfig.Port.ToString()); // 기본값이 null이라서 string으로 표현해야 함.


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IceServers", EditorStyles.label);

            // Add IceServer 버튼을 누른 경우
            if (GUILayout.Button("Add IceServer", GUILayout.Width(120)))
            {
                OnClickAddIceServerButton();
            }
            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < iceServerUrls.Count(); i++)
            {
                iceServerUrls[i] = EditorGUILayout.TextField("", iceServerUrls[i]);
            }
        }
        
        public void OnClickAddIceServerButton()
        {
            iceServerUrls.Add("turn://");
            // Debug.Log("IceServer Field is Added (Count: " + iceServerUrls.Count() + ")");
        }

        public void OnClickAddSeedPeerButton()
        {
            seedPeerStrs.Add("");
        }

        public void OnClickSaveButton()
        {
            /* InitConfig */
            IEnumerable<IceServer> inputIceServers = new IceServer[]{};
            foreach(string url in iceServerUrls)
            {
                inputIceServers = inputIceServers.Concat(new[] {new IceServer(url)});
            }
            _saveInitConfig.IceServers = inputIceServers;



            /* BootstrapConfig */
            IEnumerable<BoundPeer> inputBoundPeers = new BoundPeer[]{};
            foreach(string str in seedPeerStrs)
            {
                Debug.Log("BoundPeer String = " + str);
                inputBoundPeers = inputBoundPeers.Concat(new[] { BoundPeer.ParsePeer(str) });
            }
            _saveBootstrapConfig.SeedPeers = inputBoundPeers;

            // json으로 변환을 위한 SwarmConfig 생성
            SwarmConfig saveSwarmConfing = new SwarmConfig();
            saveSwarmConfing.InitConfig = _saveInitConfig;
            saveSwarmConfing.BootstrapConfig = _saveBootstrapConfig;

            // json으로 바꿔서 저장
            string saveJsonString = saveSwarmConfing.ToJson();
            string testFilePath = swarmConfigPath + "test_swarm_config.json";
            if(File.Exists(testFilePath))
            {
                File.WriteAllText(testFilePath, saveJsonString);
            }
            else
            {
                EditorUtility.DisplayDialog("Alert", "Swarm config file is not exist.", "Close");
            }
        }
    }

  
}