using SlowpokeStudio.character;
using SlowpokeStudio.Grid;
using SlowpokeStudio.ManHole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float rayDistance = 100f;

        // References
        private GridManager gridManager;
        private GridPathHandler pathCheckSystem;
        private GridObjectDetection gridObjectDetection;
        private bool levelInitialized = false;
        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            //FindLevelReferences();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectHoleTap();
            }
        }

        private void DetectHoleTap()
        {
            if (gridManager == null || pathCheckSystem == null || gridObjectDetection == null)
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Hole hole = hit.collider.GetComponent<Hole>();
                if (hole != null)
                {
                    Debug.Log($"[PlayerController] Tapped Hole with color: {hole.holeColor}");

                    Vector2Int holeGridPos = gridManager.GetGridPosition(hole.transform.position);

                    // 🔹 NEW: Ask BFS for all reachable characters + paths
                    var reachable = pathCheckSystem.CollectReachableFromHole(holeGridPos, hole.holeColor);

                    int movedCount = 0;

                    foreach (var kvp in reachable)
                    {
                        CharacterManager mover = kvp.Key;
                        List<Vector2Int> path = kvp.Value;

                        if (mover != null && path != null && path.Count > 0)
                        {
                            mover.MoveAlongPath(path, hole);
                            movedCount++;
                        }
                    }

                    Debug.Log($"[PlayerController] Total characters moved this tap: {movedCount}");
                }
            }

        }

        private void FindLevelReferences()
        {
            gridManager = GridManager.Instance;
            pathCheckSystem = GridManager.Instance.pathCheckSystem;
            gridObjectDetection = GridManager.Instance.gridObjectDetection;

            if (gridManager == null || pathCheckSystem == null || gridObjectDetection == null)
            {
                Debug.LogError("[PlayerController] Missing GridManager or PathCheckSystem in level.");
            }
        }

        public void InitLevelReferences()
        {
            gridManager = GridManager.Instance;
            pathCheckSystem = GridManager.Instance.pathCheckSystem;
            gridObjectDetection = GridManager.Instance.gridObjectDetection;

            if (gridManager == null || pathCheckSystem == null || gridObjectDetection == null)
            {
                Debug.LogError("[PlayerController] Failed to initialize level references!");
                levelInitialized = false;
                return;
            }

            levelInitialized = true;
            Debug.Log("[PlayerController] Level references initialized successfully.");
        }


    }
}


