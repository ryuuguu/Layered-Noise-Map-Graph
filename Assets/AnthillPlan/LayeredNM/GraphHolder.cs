using EditorAttributes;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
   public class GraphHolder : MonoBehaviour {
      public SOLayeredNoiseMap soLayeredNoiseMap;
      public Transform mapHolder;
      public GameObject islandPrefab;

      [Button("run graph")]
      public void Run() {
         soLayeredNoiseMap.mapHolder = mapHolder;
         soLayeredNoiseMap.islandPrefab = islandPrefab;
         soLayeredNoiseMap.RunGraph();
      }
   }
}
