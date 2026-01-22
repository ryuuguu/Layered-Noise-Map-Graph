/*
 * HeightmapUtility.cs
 * Created by Arian - GameDevBox
 * YouTube Channel: https://www.youtube.com/@GameDevBox
 *
 * 🎮 Want more Unity tips, tools, and advanced systems?
 * 🧠 Learn from practical examples and well-explained logic.
 * 📦 Subscribe to GameDevBox for more game dev content!
 * 
 * Static utility class providing various noise generation algorithms for heightmap creation.
 * Used by the HeightmapGenerator ScriptableObject to produce different terrain heightmaps
 * based on configurable noise types and parameters.
 */

using UnityEngine;

namespace AnthillPlan.LayeredNM {
    public static class HeightmapUtility {
        public static float[,] GenerateNoiseMap(HeightmapGenerator settings) {
            switch (settings.heightmapType) {
                case HeightmapGenerator.HeightmapType.Perlin:
                    return GeneratePerlinNoise(settings);
                case HeightmapGenerator.HeightmapType.Ridged:
                    return GenerateRidgedNoise(settings);
                case HeightmapGenerator.HeightmapType.Cellular:
                    return GenerateCellularNoise(settings);
                case HeightmapGenerator.HeightmapType.Turbulence:
                    return GenerateTurbulenceNoise(settings);
                case HeightmapGenerator.HeightmapType.DiamondSquare:
                    return GenerateDiamondSquare(settings.width, 0.6f, settings.seed);
                case HeightmapGenerator.HeightmapType.FFT_Erosion:
                    return GenerateSimpleErosion(GeneratePerlinNoise(settings), 0.4f);
                case HeightmapGenerator.HeightmapType.FBM:
                    return GenerateFBM(settings);
                case HeightmapGenerator.HeightmapType.DomainWarp:
                    return GenerateDomainWarp(settings);
                case HeightmapGenerator.HeightmapType.Value:
                    return GenerateValueNoise(settings);
                case HeightmapGenerator.HeightmapType.Billow:
                    return GenerateBillowNoise(settings);
                case HeightmapGenerator.HeightmapType.WhiteNoise:
                    return GenerateWhiteNoise(settings);
                case HeightmapGenerator.HeightmapType.CurlNoise:
                    return GenerateCurlNoise(settings);
                case HeightmapGenerator.HeightmapType.DomeConst:
                    return GenerateDome(settings);
                case HeightmapGenerator.HeightmapType.PyramidConst:
                    return GeneratePyramid(settings);
                case HeightmapGenerator.HeightmapType.ConeConst:
                    return GenerateCone(settings);
                case HeightmapGenerator.HeightmapType.SteepPeakConst:
                    return GenerateSteepPeak(settings);
                case HeightmapGenerator.HeightmapType.VerySteepPeakConst:
                    return GenerateVerySteepPeak(settings);
                case HeightmapGenerator.HeightmapType.SteepRidgedConst:
                    return GenerateSteepRidged(settings);
                default:
                    return GeneratePerlinNoise(settings);
            }
        }

        /// <summary>
        /// base Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GeneratePerlinNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var octaveOffsets = GetOctavesOffSets(s);
            float maxNoise = float.MinValue, minNoise = float.MaxValue;

            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int i = 0; i < s.octaves; i++) {
                        float sampleX = (x - s.width / 2f) / Mathf.Max(s.noiseScale, 0.0001f) * frequency +
                                        octaveOffsets[i].x;
                        float sampleY = (y - s.height / 2f) / Mathf.Max(s.noiseScale, 0.0001f) * frequency +
                                        octaveOffsets[i].y;
                        float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                        noiseHeight += perlin * amplitude;

                        amplitude *= s.persistence;
                        frequency *= s.lacunarity;
                    }

                    maxNoise = Mathf.Max(maxNoise, noiseHeight);
                    minNoise = Mathf.Min(minNoise, noiseHeight);
                    map[x, y] = noiseHeight;
                }
            }

            return NormalizeMap(map);
        }

        private static Vector2[] GetOctavesOffSets(HeightmapGenerator s ) {
           
            System.Random prng = new System.Random(s.seed);
            var octaveOffsets = new Vector2[s.octaves];

            for (int i = 0; i < s.octaves; i++) {
                float offsetX = prng.Next(-100000, 100000) + s.offset.x;
                float offsetY = prng.Next(-100000, 100000) + s.offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            return octaveOffsets;
        }

        /// <summary>
        /// uses Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateFBM(HeightmapGenerator s) {
            return GeneratePerlinNoise(s); // Already fBM-style (Just change Values)
        }

        /// <summary>
        /// uses Perlin noise but calls twice
        /// both calls will return same map
        /// this is probably wrong
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateDomainWarp(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            float[,] warpX = GeneratePerlinNoise(s);
            float[,] warpY = GeneratePerlinNoise(s);

            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    float wx = x + (warpX[x, y] - 0.5f) * s.noiseScale * 0.5f;
                    float wy = y + (warpY[x, y] - 0.5f) * s.noiseScale * 0.5f;
                    float sample = Mathf.PerlinNoise(wx / s.noiseScale, wy / s.noiseScale);
                    map[x, y] = sample;
                }
            }
            return NormalizeMap(map);
        }


        /// <summary>
        /// uses Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateRidgedNoise(HeightmapGenerator s) {
            float[,] baseNoise = GeneratePerlinNoise(s);
            float[,] map = new float[s.width, s.height];

            for (int y = 0; y < s.height; y++)
            for (int x = 0; x < s.width; x++)
                map[x, y] = Mathf.Pow(1f - Mathf.Abs(baseNoise[x, y] * 2f - 1f), 2f);

            return NormalizeMap(map);
        }


        /// <summary>
        /// not Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateCellularNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            Vector2[] points = new Vector2[32];
            System.Random rand = new System.Random(s.seed);

            for (int i = 0; i < points.Length; i++)
                points[i] = new Vector2(rand.Next(0, s.width), rand.Next(0, s.height));

            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    float minDist = float.MaxValue;
                    foreach (var p in points) {
                        float dist = Vector2.Distance(new Vector2(x, y), p);
                        if (dist < minDist) minDist = dist;
                    }

                    map[x, y] = Mathf.InverseLerp(0, Mathf.Sqrt(s.width * s.width + s.height * s.height), minDist);
                }
            }

            return map;
        }


        /// <summary>
        /// does not change
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateTurbulenceNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var octaveOffsets = GetOctavesOffSets(s);

            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    float freq = 1f, amp = 1f, total = 0f;
                    for (int i = 0; i < s.octaves; i++) {
                        float nx = x / (s.noiseScale / freq)+octaveOffsets[i].x;
                        float ny = y / (s.noiseScale / freq)+octaveOffsets[i].y;
                        total += Mathf.Abs(Mathf.PerlinNoise(nx, ny) * 2f - 1f) * amp;
                        freq *= s.lacunarity;
                        amp *= s.persistence;
                    }

                    map[x, y] = total;
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// not Perlin noise
        /// </summary>
        /// <param name="size"></param>
        /// <param name="roughness"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static float[,] GenerateDiamondSquare(int size, float roughness, int seed) {
            int gridSize = Mathf.ClosestPowerOfTwo(size - 1) + 1;
            float[,] map = new float[gridSize, gridSize];
            System.Random rand = new System.Random(seed);

            map[0, 0] = map[0, gridSize - 1] = map[gridSize - 1, 0] = map[gridSize - 1, gridSize - 1] = 0.5f;

            int step = gridSize - 1;
            while (step > 1) {
                int half = step / 2;

                for (int y = half; y < gridSize; y += step)
                for (int x = half; x < gridSize; x += step) {
                    float avg = (map[x - half, y - half] + map[x - half, y + half] + map[x + half, y - half] +
                                 map[x + half, y + half]) / 4f;
                    map[x, y] = avg + ((float) rand.NextDouble() * 2f - 1f) * roughness;
                }

                for (int y = 0; y < gridSize; y += half)
                for (int x = (y + half) % step; x < gridSize; x += step) {
                    float sum = 0f;
                    int count = 0;
                    if (x - half >= 0) {
                        sum += map[x - half, y];
                        count++;
                    }

                    if (x + half < gridSize) {
                        sum += map[x + half, y];
                        count++;
                    }

                    if (y - half >= 0) {
                        sum += map[x, y - half];
                        count++;
                    }

                    if (y + half < gridSize) {
                        sum += map[x, y + half];
                        count++;
                    }

                    map[x, y] = sum / count + ((float) rand.NextDouble() * 2f - 1f) * roughness;
                }

                step /= 2;
                roughness *= 0.5f;
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// Map modifier
        /// </summary>
        /// <param name="map"></param>
        /// <param name="erosionFactor"></param>
        /// <returns></returns>
        public static float[,] GenerateSimpleErosion(float[,] map, float erosionFactor = 0.5f) {
            int w = map.GetLength(0);
            int h = map.GetLength(1);
            float[,] result = new float[w, h];

            for (int y = 1; y < h - 1; y++) {
                for (int x = 1; x < w - 1; x++) {
                    float avg = (
                        map[x, y] + map[x - 1, y] + map[x + 1, y] +
                        map[x, y - 1] + map[x, y + 1]
                    ) / 5f;

                    result[x, y] = Mathf.Lerp(map[x, y], avg, erosionFactor);
                }
            }

            return NormalizeMap(result);
        }

        
        /// <summary>
        /// not Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateValueNoise(HeightmapGenerator s) {
            int width = s.width;
            int height = s.height;
            float scale = Mathf.Max(s.noiseScale, 0.0001f);
            float[,] map = new float[width, height];
            System.Random prng = new System.Random(s.seed);


            int gridWidth = Mathf.CeilToInt(width / scale) + 2;
            int gridHeight = Mathf.CeilToInt(height / scale) + 2;
            float[,] gridValues = new float[gridWidth, gridHeight];

            for (int gy = 0; gy < gridHeight; gy++) {
                for (int gx = 0; gx < gridWidth; gx++) {
                    gridValues[gx, gy] = (float) prng.NextDouble();
                }
            }

            for (int y = 0; y < height; y++) {
                float sampleY = y / scale;
                int y0 = Mathf.FloorToInt(sampleY);
                int y1 = y0 + 1;
                float sy = sampleY - y0;

                for (int x = 0; x < width; x++) {
                    float sampleX = x / scale;
                    int x0 = Mathf.FloorToInt(sampleX);
                    int x1 = x0 + 1;
                    float sx = sampleX - x0;

                    float v00 = gridValues[x0, y0];
                    float v10 = gridValues[x1, y0];
                    float v01 = gridValues[x0, y1];
                    float v11 = gridValues[x1, y1];

                    float ix0 = Mathf.Lerp(v00, v10, sx);
                    float ix1 = Mathf.Lerp(v01, v11, sx);
                    map[x, y] = Mathf.Lerp(ix0, ix1, sy);
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// uses Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateBillowNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            System.Random prng = new System.Random(s.seed);
            Vector2[] octaveOffsets = new Vector2[s.octaves];

            for (int i = 0; i < s.octaves; i++) {
                float offsetX = prng.Next(-100000, 100000) + s.offset.x;
                float offsetY = prng.Next(-100000, 100000) + s.offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int i = 0; i < s.octaves; i++) {
                        float sampleX = (x - s.width / 2f) / Mathf.Max(s.noiseScale, 0.0001f) * frequency +
                                        octaveOffsets[i].x;
                        float sampleY = (y - s.height / 2f) / Mathf.Max(s.noiseScale, 0.0001f) * frequency +
                                        octaveOffsets[i].y;
                        float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                        noiseHeight += (Mathf.Abs(perlin) * 2f - 1f) * amplitude;

                        amplitude *= s.persistence;
                        frequency *= s.lacunarity;
                    }

                    map[x, y] = noiseHeight;
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// Not Perlin noise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateWhiteNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            System.Random rand = new System.Random(s.seed);

            for (int y = 0; y < s.height; y++)
            for (int x = 0; x < s.width; x++)
                map[x, y] = (float) rand.NextDouble();

            return map;
        }

        /// <summary>
        /// uses Perlin noise but calls it twice
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateCurlNoise(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            float epsilon = 0.01f;

            float[,] noiseX = GeneratePerlinNoise(s);
            float[,] noiseY = GeneratePerlinNoise(s);

            for (int y = 1; y < s.height - 1; y++) {
                for (int x = 1; x < s.width - 1; x++) {
                    float dYdx = (noiseY[x + 1, y] - noiseY[x - 1, y]) / (2 * epsilon);
                    float dXdy = (noiseX[x, y + 1] - noiseX[x, y - 1]) / (2 * epsilon);

                    map[x, y] = dYdx - dXdy;
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateDome(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (1f - (Mathf.Pow(x * xm - 0.5f, 2) + Mathf.Pow(y * ym - 0.5f, 2)));
                    //  Mathf.Pow(1f - Mathf.Abs(baseNoise[x, y] * 2f - 1f), 2f);
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateSteepRidged(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (1f - (Mathf.Pow(Mathf.Abs(x * xm - 0.5f), 0.5f) +
                                       Mathf.Pow(Mathf.Abs(y * ym - 0.5f), 0.5f)));
                    //  Mathf.Pow(1f - Mathf.Abs(baseNoise[x, y] * 2f - 1f), 2f);
                }
            }

            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateVerySteepPeak(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (
                        1f - (Mathf.Pow(Mathf.Pow(x * xm - 0.5f, 2) + Mathf.Pow(y * ym - 0.5f, 2), 0.3f)));
                }
            }
            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateSteepPeak(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (
                        1f - (Mathf.Pow(Mathf.Pow(x * xm - 0.5f, 2) + Mathf.Pow(y * ym - 0.5f, 2), 0.45f)));
                }
            }
            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GeneratePyramid(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (1f - (Mathf.Abs(x * xm - 0.5f) + Mathf.Abs(y * ym - 0.5f)));
                    //  Mathf.Pow(1f - Mathf.Abs(baseNoise[x, y] * 2f - 1f), 2f);
                }
            }
            return NormalizeMap(map);
        }

        /// <summary>
        /// const map
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float[,] GenerateCone(HeightmapGenerator s) {
            float[,] map = new float[s.width, s.height];
            var xm = 1f / s.width;
            var ym = 1f / s.height;
            for (int y = 0; y < s.height; y++) {
                for (int x = 0; x < s.width; x++) {
                    map[x, y] = (
                        1f - (Mathf.Sqrt(Mathf.Pow(x * xm - 0.5f, 2) + Mathf.Pow(y * ym - 0.5f, 2))));
                    //  Mathf.Pow(1f - Mathf.Abs(baseNoise[x, y] * 2f - 1f), 2f);
                }
            }
            return NormalizeMap(map);
        }

        /// <summary>
        /// lowers map until all edge are below 0
        /// so a water plane 0 will make the map an island
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static float[,] IslandMap(float[,] map) {
            int w = map.GetLength(0);
            int h = map.GetLength(1);
            float seaLevel = 0;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    if (y == 0 || y == h - 1 || x == 0 || x == w - 1 ||
                        y == 1 || y == h - 1 || x == 1 || x == w - 1) {
                        if (seaLevel < map[x, y]) {
                            seaLevel = map[x, y];
                        }
                    }
                }
            }
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    map[x, y] -= seaLevel;
                }
            }
            return map;
        }

        public static float[,] BeachCircleMap(float[,] map) {
            int w = map.GetLength(0);
            int h = map.GetLength(1);
            var border = 6;
            float c = (Mathf.Min(w, h) + border) / 2.0f;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    {
                        var rDist = (new Vector2(x - c + border / 2f, y - c + border / 2f)).magnitude;
                        if ((y == 0 || y == h - 1 || x == 0 || x == w - 1 ||
                             y == 1 || y == h - 1 || x == 1 || x == w - 1) ||
                            rDist > c - 10) {
                            map[x, y] -= rDist - (c - 10);
                        }
                    }
                }
            }
            return map;
        }

        public static float[,] NormalizeMap(float[,] map) {
            int w = map.GetLength(0);
            int h = map.GetLength(1);
            float min = float.MaxValue, max = float.MinValue;

            foreach (float v in map) {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            float range = max - min;
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                //map[x, y] = (map[x, y] - min) / Mathf.Max(range, 0.0001f);
                map[x, y] = Mathf.Clamp01((map[x, y] - min) / Mathf.Max(range, 0.0001f));

            return map;
        }

        public static float[,] ApplySharpness(float[,] map, int iterations, float intensity) {
            int w = map.GetLength(0), h = map.GetLength(1);
            float[,] sharpened = (float[,]) map.Clone();

            for (int iter = 0; iter < iterations; iter++) {
                float[,] copy = (float[,]) sharpened.Clone();
                for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++) {
                    float center = copy[x, y] * (1 + intensity * 4);
                    float neighbors = (copy[x - 1, y] + copy[x + 1, y] + copy[x, y - 1] + copy[x, y + 1]) / 4f;
                    sharpened[x, y] = Mathf.Clamp01(center - neighbors * intensity);
                }
            }

            return sharpened;
        }


        /// <summary>
        /// change this code to work LayeredNM
        /// </summary>
        /// <param name="map"></param>
        /// <param name="format"></param>
        /// <param name="heightMultiplier"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D CreateTextureFromMap(float[,] map, TextureFormat format, float heightMultiplier,
            string name) {
            int w = map.GetLength(0), h = map.GetLength(1);
            Texture2D tex = new Texture2D(w, h, format, false);
            tex.name = name;

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++) {
                float v = Mathf.Pow(map[x, y], heightMultiplier);
                tex.SetPixel(x, y, new Color(v, v, v));
            }

            tex.Apply();
            return tex;
        }
    }

}