
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

#if UNITY_EDITOR
    using UnityEditor;
#endif

using Random = UnityEngine.Random;


namespace AnthillPlan.LayeredNM {
    /// <summary>
    /// This is used to hold and interface with LayeredNoiseMap Graphs.
    /// </summary>
    public class MBLayeredNoiseMap : MonoBehaviour {


        [Header("Graph")] public LayeredNmRuntimeGraph runtimeGraph;
        [HideInInspector] public List<RuntimeVariable> blackboardVariables = new List<RuntimeVariable>();
        [HideInInspector] public bool displayBbv = true;
        [HideInInspector] public List<RuntimeVariable> runtimeGraphBlackboardVariables;
        [HideInInspector] public ErrorMessage errorMessage = new(); // is used in custom inspector

        [Header("Settings")] public Vector2Int size = new(4, 4);
        public float scale = 1;
        public float xzScale = 1;
        public float yScale = 10;
        public int noiseSeed = 0;
        public bool newRandomSeed = true;

        [Header("output map")] public float[,] map;
        public Transform mapHolder;
        public GameObject islandPrefab;
        public bool unshare;

        public void RunGraph() {
            OnValidate();
            if (runtimeGraph == null) {
                Debug.Log($"Run Time Graph is null ");
                return;
            }

            if (newRandomSeed) {
                noiseSeed = Random.Range(1, int.MaxValue);
            }

            runtimeGraph.ctx = SettingCtx();
            map = runtimeGraph.ExecuteGraph(runtimeGraph.ctx);
            
            for (int i = 0; i < map.GetLength(0); i++) {
                for (int j = 0; j < map.GetLength(1); j++) {
                    map[i, j] *= yScale;
                }
            }

            DestroyAllChildren(mapHolder.transform);
            var island = Instantiate(islandPrefab, mapHolder);
            var offset = -scale * xzScale / 2f;
            island.transform.position = new Vector3(offset * size.x, 0, offset * size.y);

            HeightMapToMesh.Height2Tris(map, island, scale, xzScale, unshare);
        }
        
        private void OnValidate() {
            if (runtimeGraph != null) {
                errorMessage = runtimeGraph.errorMessage;
                runtimeGraphBlackboardVariables = runtimeGraph.blackboardVariables;
            }
        }

        public void UpdateBlackboard() {
            if (runtimeGraph != null) {
                if (blackboardVariables.Count > 0) {
                    Debug.Log($"A UpdateBlackboard blackboardVariables[0] {blackboardVariables[0]}");
                }
                else {
                    Debug.Log($"B UpdateBlackboard blackboardVariables Count ZERO +++++++++++++++++++++++++++++"); 
                }
                Debug.Log($"C UpdateBlackboard blackboardVariables Count {blackboardVariables.Count}");
                displayBbv = true;
                // remove extra variables
                var extraVariables = new List<RuntimeVariable>();
                foreach (var rtv in blackboardVariables) {
                    var oldRtv =
                        runtimeGraph.blackboardVariables.Find(v => v.name == rtv.name && v.dataType == rtv.dataType);
                    if (oldRtv == null) {
                        extraVariables.Add(rtv);
                    }
                }
                //Debug.Log($"f UpdateBlackboard extraVariables Count {extraVariables.Count}");
                foreach (var rtv in extraVariables) {
                    blackboardVariables.Remove(rtv);
                    Debug.Log($"G UpdateBlackboard Removed {rtv.name}");
                }

                // add missing variables
                foreach (var rtv in runtimeGraph.blackboardVariables) {
                    var oldRtv = blackboardVariables.
                        Find(v => v.name == rtv.name && v.dataType == rtv.dataType);
                    if (oldRtv == null) {
                        blackboardVariables.Add(new RuntimeVariable() {
                            name = rtv.name,
                            dataType = rtv.dataType,
                            intVal = 0,
                            floatVal = 0f,
                            map = new float[1, 1],
                        });
                        Debug.Log($"K UpdateBlackboard Add {rtv.name}");
                    }
                }
                if (blackboardVariables.Count > 0) {
                    Debug.Log($"X UpdateBlackboard blackboardVariables[0] {blackboardVariables[0]}");
                }
            }
            else {
                Debug.Log($" runtimeGraph is null ");
                displayBbv = false;
            }
        }

        private LayeredNmRuntimeGraph.GraphContext SettingCtx() {
            var ctx = new LayeredNmRuntimeGraph.GraphContext();
            ctx.size = size;
            ctx.scale = scale;
            ctx.randomSeed = noiseSeed;
            ctx.blackboardVariables = blackboardVariables;
            return ctx;
        }

        public void DestroyAllChildren(Transform parentTransform) {
            var childCount = parentTransform.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--) {
                #if UNITY_EDITOR
                    if (Application.isPlaying) {
                        Destroy(parentTransform.transform.GetChild(i).gameObject);
                    }
                    else {
                        DestroyImmediate(parentTransform.transform.GetChild(i).gameObject);
                    }
                #else
                    Destroy(parentTransform.transform.GetChild(i).gameObject);
                #endif
            }
        }
       
    }
}