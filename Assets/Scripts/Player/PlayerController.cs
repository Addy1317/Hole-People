using SlowpokeStudio.character;
using SlowpokeStudio.Grid;
using SlowpokeStudio.ManHole;
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

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            FindLevelReferences();
        }

        private void FindLevelReferences()
        {
            gridManager = GridManager.Instance;//FindObjectOfType<GridManager>();
            pathCheckSystem = GridManager.Instance.pathCheckSystem;//FindObjectOfType<PathCheckSystem>();
            gridObjectDetection = GridManager.Instance.gridObjectDetection;

            if (gridManager == null || pathCheckSystem == null || gridObjectDetection == null)
            {
                Debug.LogError("[PlayerController] Missing GridManager or PathCheckSystem in level.");
            }
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

                    // STEP 1: Search for all characters of same color
                    List<GridObjectData> sameColorCharacters = gridObjectDetection.GetAllCharactersOfColor(hole.holeColor);

                    HashSet<Vector2Int> charactersToMove = new HashSet<Vector2Int>();

                    // STEP 2: Check path for each one
                    foreach (var charData in sameColorCharacters)
                    {
                        // Only if it's in same row or column
                        if (charData.gridPosition.x == holeGridPos.x && pathCheckSystem.IsPathClearVertical(holeGridPos, charData.gridPosition))
                        {
                            AddConnectedChain(charData, charactersToMove);
                        }
                        else if (charData.gridPosition.y == holeGridPos.y && pathCheckSystem.IsPathClearHorizontal(holeGridPos, charData.gridPosition))
                        {
                            AddConnectedChain(charData, charactersToMove);
                        }
                    }

                    // STEP 3: Move all collected characters
                    foreach (Vector2Int gridPos in charactersToMove)
                    {
                        Vector3 worldPos = gridManager.GetWorldPosition(gridPos.x, gridPos.y);
                        Collider[] hits = Physics.OverlapSphere(worldPos, 0.1f);

                        foreach (Collider col in hits)
                        {
                            CharacterManager mover = col.GetComponent<CharacterManager>();
                            if (mover != null)
                            {
                                mover.MoveToHole(hole);
                                break;
                            }
                        }
                    }

                    Debug.Log($"[PlayerController] Total characters moved: {charactersToMove.Count}");
                }
            }
            
        }

        private void AddConnectedChain(GridObjectData charData, HashSet<Vector2Int> charactersToMove)
        {
            List<GridObjectData> connected = gridObjectDetection.GetAdjacentSameColorCharacters(charData.gridPosition, charData.color);
            foreach (var c in connected)
            {
                charactersToMove.Add(c.gridPosition);
            }
        }

    }
}


