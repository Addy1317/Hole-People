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

                    // Step 1: Get ALL characters (including those newly unblocked) who now have path to hole
                    List<GridObjectData> validPathCharacters = pathCheckSystem.GetMovableCharacters(holeGridPos, hole.holeColor);

                    // Step 2: BFS from each valid character to collect all adjacent same-colored ones
                    HashSet<Vector2Int> finalCharactersToMove = new HashSet<Vector2Int>();

                    foreach (var charData in validPathCharacters)
                    {
                        List<GridObjectData> connected = gridObjectDetection.GetAdjacentSameColorCharacters(charData.gridPosition, charData.color);

                        foreach (var data in connected)
                            finalCharactersToMove.Add(data.gridPosition);
                    }

                    // Step 3: Move All Collected Characters
                    foreach (Vector2Int gridPos in finalCharactersToMove)
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

                    Debug.Log($"[PlayerController] Total characters moved on tap: {finalCharactersToMove.Count}");
                }
            }
            
        }
    }
}


