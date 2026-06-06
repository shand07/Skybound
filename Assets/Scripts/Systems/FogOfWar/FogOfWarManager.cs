using System.Collections.Generic;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Systems.FogOfWar
{
    public class FogOfWarManager : MonoBehaviour
    {
        public static FogOfWarManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 40;
        [SerializeField] private int gridHeight = 40;
        [SerializeField] private float tileSize = 2f;
        [SerializeField] private Vector3 gridOrigin = Vector3.zero;

        [Header("References")]
        [SerializeField] private FogTile fogTilePrefab;

        [Header("Update")]
        [SerializeField] private float updateInterval = 0.15f;

        [Header("Map Bounds")]
        [SerializeField] private Renderer mapRenderer;
        [SerializeField] private float boundsPadding = 4f;

        private readonly List<FogTile> fogTiles = new();
        private readonly List<VisionSource> visionSources = new();

        private float updateTimer;
        private bool hasGeneratedGrid;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning("Duplicate FogOfWarManager found. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundServiceRegistry.Register(this);

            SkyboundDebug.Log("FogOfWarManager initialized.", this);
        }

        private void Start()
        {
            GenerateFogGrid();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                SkyboundServiceRegistry.Unregister<FogOfWarManager>();
            }
        }

        private void Update()
        {
            if (!hasGeneratedGrid)
                return;

            updateTimer += Time.deltaTime;

            if (updateTimer < updateInterval)
                return;

            updateTimer = 0f;

            RefreshVisionSources();
            UpdateFog();
        }

        public FogState GetFogStateAtWorldPosition(Vector3 worldPosition)
        {
            FogTile closestTile = GetClosestTile(worldPosition);

            if (closestTile == null)
            {
                SkyboundDebug.Warning($"No fog tile found near world position {worldPosition}. Returning Unexplored.", this);
                return FogState.Unexplored;
            }

            return closestTile.State;
        }

        public bool IsWorldPositionVisible(Vector3 worldPosition)
        {
            FogTile closestTile = GetClosestTile(worldPosition);
            return closestTile != null && closestTile.State == FogState.Visible;
        }

        private void GenerateFogGrid()
        {
            fogTiles.Clear();
            hasGeneratedGrid = false;

            if (fogTilePrefab == null)
            {
                SkyboundDebug.MissingReference(this, nameof(fogTilePrefab), "Assign a FogTile prefab in the inspector.");
                return;
            }

            if (mapRenderer == null)
            {
                SkyboundDebug.MissingReference(this, nameof(mapRenderer), "Assign the map renderer used to calculate fog bounds.");
                return;
            }

            Bounds bounds = mapRenderer.bounds;

            float mapWidth = bounds.size.x + boundsPadding;
            float mapHeight = bounds.size.z + boundsPadding;

            gridWidth = Mathf.CeilToInt(mapWidth / tileSize);
            gridHeight = Mathf.CeilToInt(mapHeight / tileSize);

            Vector3 startPosition = new Vector3(
                bounds.min.x,
                bounds.max.y + 0.05f,
                bounds.min.z
            );

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Vector3 position = startPosition + new Vector3(
                        x * tileSize,
                        0f,
                        z * tileSize
                    );

                    FogTile tile = Instantiate(
                        fogTilePrefab,
                        position,
                        Quaternion.identity,
                        transform
                    );

                    tile.transform.localScale = new Vector3(
                        tileSize,
                        0.01f,
                        tileSize
                    );

                    tile.SetState(FogState.Unexplored);
                    fogTiles.Add(tile);
                }
            }

            hasGeneratedGrid = true;

            SkyboundDebug.Log($"Generated fog grid. Width: {gridWidth}, Height: {gridHeight}, Tiles: {fogTiles.Count}", this);
        }

        private void RefreshVisionSources()
        {
            visionSources.Clear();

            VisionSource[] sources = FindObjectsByType<VisionSource>(FindObjectsSortMode.None);

            foreach (VisionSource source in sources)
            {
                if (source == null || !source.gameObject.activeInHierarchy)
                    continue;

                if (!source.CanRevealFog())
                    continue;

                visionSources.Add(source);
            }
        }

        private void UpdateFog()
        {
            foreach (FogTile tile in fogTiles)
            {
                if (tile.State == FogState.Visible)
                    tile.SetState(FogState.Explored);
            }

            foreach (VisionSource source in visionSources)
                RevealAroundSource(source);
        }

        private void RevealAroundSource(VisionSource source)
        {
            Vector3 sourcePosition = source.transform.position;
            float visionRange = source.VisionRange;

            foreach (FogTile tile in fogTiles)
            {
                float distance = Vector3.Distance(
                    new Vector3(sourcePosition.x, 0f, sourcePosition.z),
                    new Vector3(tile.transform.position.x, 0f, tile.transform.position.z)
                );

                if (distance <= visionRange + tileSize * 0.5f)
                    tile.SetState(FogState.Visible);
            }
        }

        private FogTile GetClosestTile(Vector3 worldPosition)
        {
            FogTile closest = null;
            float closestDistance = float.MaxValue;

            foreach (FogTile tile in fogTiles)
            {
                float distance = Vector3.Distance(
                    new Vector3(worldPosition.x, 0f, worldPosition.z),
                    new Vector3(tile.transform.position.x, 0f, tile.transform.position.z)
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = tile;
                }
            }

            return closest;
        }
    }
}