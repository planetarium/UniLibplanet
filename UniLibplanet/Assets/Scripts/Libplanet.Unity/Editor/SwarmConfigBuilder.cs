using System;
using System.IO;
using System.Text;
using Libplanet.Crypto;
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
        private static string swarmConfigPath = $"{Application.streamingAssetsPath}/";
        private static string swarmConfigFilename = "swarm_config.json";

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

        /// <summary>
        /// Redraw on GUI event.
        /// </summary>
        int maxTimeout;
        int minTimeout;
        int routingTableNumBuckets;
        int routingTableBucketSize;
        int blockLocatorThreshold;
        int transportType;
        string host;
        string port;
        public void OnGUI()
        {
            EditorGUILayout.LabelField("InitConfig", EditorStyles.boldLabel);

            maxTimeout = EditorGUILayout.IntField("MaxTimeout", (int)_initConfig.MaxTimeout.TotalSeconds);
            minTimeout = EditorGUILayout.IntField("MinTimeout",(int)_initConfig.MinTimeout.TotalSeconds);

            routingTableNumBuckets = EditorGUILayout.IntField("RoutingTableNumBuckets", _initConfig.RoutingTableNumBuckets);
            routingTableBucketSize = EditorGUILayout.IntField("RoutingTableBucketSize", _initConfig.RoutingTableBucketSize);

            blockLocatorThreshold = EditorGUILayout.IntField("BlockLocatorIndexSampleThreshold", _initConfig.BlockLocatorIndexSampleThreshold);

            transportType = EditorGUILayout.IntField("TransportType", (int)_initConfig.TransportType);

            _saveInitConfig.Host = EditorGUILayout.TextField("Host", _initConfig.Host);
            port = EditorGUILayout.TextField("Port", _initConfig.Port.ToString()); // 기본값이 null이라서 string으로 표현해야 함.
        
            if(GUI.changed)
            {
                TimeSpan maxTimeSpan = new TimeSpan(0, 0, maxTimeout);
                _saveInitConfig.MaxTimeout = maxTimeSpan;

                TimeSpan minTimeSpan = new TimeSpan(0, 0, minTimeout);
                _saveInitConfig.MinTimeout = minTimeSpan;

                _saveInitConfig.RoutingTableNumBuckets = routingTableNumBuckets;
                _saveInitConfig.BlockLocatorIndexSampleThreshold = blockLocatorThreshold;

                _saveInitConfig.TransportType = (SwarmOptions.TransportType)transportType;
                _initConfig = _saveInitConfig;

                _saveInitConfig.Host = host.ToString();

            }
            
            EditorGUILayout.Space();
 
            if (GUILayout.Button("Save swarm_config.json"))
            {
                OnClickSaveButton();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            // EditorGUILayout.LabelField("Generated bound peer string", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(_boundPeerString);
        }

        public void OnClickSaveButton()
        {
            Debug.Log("MaxTimeout = " + _saveInitConfig.MaxTimeout.Seconds);    
            Debug.Log("MinTimeout = " + _saveInitConfig.MinTimeout.Seconds);   
            Debug.Log("RoutingTableNumBuckets = " + _saveInitConfig.RoutingTableNumBuckets);
            Debug.Log("RoutingTableBucketSize = " + _saveInitConfig.RoutingTableBucketSize);
            Debug.Log("BlockLocatorIndexSampleThreshold = " + _saveInitConfig.BlockLocatorIndexSampleThreshold);
            Debug.Log("SwarmOptions.TransportType TransportType = " + (int)_saveInitConfig.TransportType);
            Debug.Log("Host = " + _saveInitConfig.Host);
            Debug.Log("Port = " + _saveInitConfig.Port);

            SwarmConfig saveSwarmConfing = new SwarmConfig();
            saveSwarmConfing.InitConfig = _saveInitConfig;
            string saveJsonString = saveSwarmConfing.ToJson();

            string testFilePath = swarmConfigPath + "test_swarm_config.json";
            if(File.Exists(testFilePath))
            {
                // string _swarmConfigJsonString = File.ReadAllText(testFilePath);
                File.WriteAllText(testFilePath, saveJsonString);
            }
            else
            {
                EditorUtility.DisplayDialog("Alert", "Swarm config file is not exist.", "Close");
            }
        }
    }

  
}