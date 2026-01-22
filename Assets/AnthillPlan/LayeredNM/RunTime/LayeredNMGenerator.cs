using UnityEngine;

namespace AnthillPlan.LayeredNM {
    
    // run runtime graph 
    public class LayeredNmGenerator {
        
        public class HeightmapGeneratorSettings {
            public enum HeightmapType {
                Perlin = 0,
                Ridged = 1,
                Cellular = 2,
                Turbulence = 3,
                DiamondSquare = 4,
                FFT_Erosion = 5,
                FBM = 6,
                DomainWarp = 7,
                Simplex = 8,
                Value = 9,
                Billow = 10,
                WhiteNoise = 11,
                CurlNoise = 12,
                DomeConst = 13,
                PyramidConst = 14,
                ConeConst = 15,
                SteepPeakConst = 16,
                VerySteepPeakConst = 17,
                SteepRidgedConst = 18,
            }

            public HeightmapType heightmapType = HeightmapType.Perlin;
            public TextureFormat format = TextureFormat.RGBA32; // for use with Unity Terrain

            public float noiseScale = 100f;
            [Range(1, 8)] public int octaves = 4;
            [Range(0, 1)] public float persistence = 0.5f;
            public float lacunarity = 2f;
            [HideInInspector] public Vector2 offset;
            [Range(0, 100f)] public float heightMultiplier = 1f;

            // later used for tiling
            public Vector2 randomOffsetRange = new Vector2(-1000, 1000);
            public Vector2 randomScaleRange = new Vector2(50, 200);
        }
    }
}