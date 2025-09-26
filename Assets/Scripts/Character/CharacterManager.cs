using SlowpokeStudio.Grid;
using SlowpokeStudio.ManHole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.character
{
    public class CharacterManager : MonoBehaviour
    {
        #region Old Code
        /*        [Header("Movement Settings")]
                [SerializeField] private float moveSpeed = 5f;
                [SerializeField] private float stopDistanceToHole = 0.1f;

                private Vector2Int currentGridPos;
                private ObjectColor myColor;
                private bool isMoving = false;

                [SerializeField] private Character character;

                public void Init(Vector2Int gridPos)
                {
                    currentGridPos = gridPos;
                }

                public bool IsMovementFinished()
                {
                    return !isMoving;
                }

                public ObjectColor GetColor()
                {
                    return character.characterColor;
                }

                public void MoveToHoleWithChain(Hole targetHole, List<CharacterManager> connectedCharacters)
                {
                    StartCoroutine(MoveChainCharactersRoutine(targetHole, connectedCharacters));
                }

                private IEnumerator MoveChainCharactersRoutine(Hole targetHole, List<CharacterManager> characters)
                {
                    foreach (var character in characters)
                    {
                        if (character == null || !character.gameObject.activeSelf)
                            continue;

                        character.MoveToHole(targetHole);
                        yield return new WaitUntil(() => character.IsMovementFinished()); // Wait until character finishes
                        yield return new WaitForSeconds(0.05f); // Small delay for cleaner visuals
                    }
                }

                public void MoveToHole(Hole targetHole)
                {
                    Debug.Log($"[CharacterManager] MoveToHole called for {gameObject.name}");
                    if (isMoving) return;

                    if (targetHole == null)
                    {
                        Debug.LogError("[CharacterMover] Target hole is null!");
                        return;
                    }

                    if (targetHole.holeCenter == null)
                    {
                        Debug.LogError($"[CharacterMover] Hole center is NOT assigned for: {targetHole.name}");
                        return;
                    }

                    Vector2Int holeGridPos = GridManager.Instance.GetGridPosition(targetHole.transform.position);
                    StartCoroutine(MoveStepByStep(holeGridPos, targetHole));
                }   

                private IEnumerator MoveStepByStep(Vector2Int targetGridPos, Hole targetHole)
                {
                    if (targetHole.holeCenter == null)
                    {
                        Debug.LogError($"[CharacterMover] HoleCenter is null for hole: {targetHole.name}");
                        yield break;
                    }

                    isMoving = true;

                    List<Vector2Int> path = GenerateStraightLinePath(currentGridPos, targetGridPos);

                    foreach (var nextPos in path)
                    {
                        Vector3 worldTarget = GridManager.Instance.GetWorldPosition(nextPos.x, nextPos.y);

                        // Move to next cell
                        yield return StartCoroutine(MoveToWorldPosition(worldTarget));

                        // Update GridManager
                        GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
                        GridManager.Instance.SetCell(nextPos.x, nextPos.y, CellType.Character);

                        GridManager.Instance.gridObjectDetection.UpdateCharacterPosition(this, currentGridPos, nextPos);

                        currentGridPos = nextPos;
                    }

                    // Final move into hole's center (or close enough)
                    Transform holeCenter = targetHole.holeCenter; // You expose this in HoleColor
                    while (Vector3.Distance(transform.position, holeCenter.position) > stopDistanceToHole)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, holeCenter.position, moveSpeed * Time.deltaTime);
                        yield return null;
                    }

                    // Deactivate and clear grid cell
                    GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
                    gameObject.SetActive(false); // or Destroy(gameObject);

                    GridManager.Instance.gridObjectDetection.RemoveCharacterAt(currentGridPos);

                    Debug.Log($"[CharacterMover] Character deactivated at hole {targetGridPos}");
                }

                private IEnumerator MoveToWorldPosition(Vector3 targetPos)
                {
                    while (Vector3.Distance(transform.position, targetPos) > 0.01f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                        yield return null;
                    }
                }

                private List<Vector2Int> GenerateStraightLinePath(Vector2Int start, Vector2Int end)
                {
                    List<Vector2Int> path = new List<Vector2Int>();

                    if (start.x == end.x)
                    {
                        // Vertical movement
                        int dir = (end.y > start.y) ? 1 : -1;
                        for (int y = start.y + dir; y != end.y + dir; y += dir)
                            path.Add(new Vector2Int(start.x, y));
                    }
                    else if (start.y == end.y)
                    {
                        // Horizontal movement
                        int dir = (end.x > start.x) ? 1 : -1;
                        for (int x = start.x + dir; x != end.x + dir; x += dir)
                            path.Add(new Vector2Int(x, start.y));
                    }

                    return path;
                }
        */
        #endregion

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stopDistanceToHole = 0.1f;

        private Vector2Int currentGridPos;
        private bool isMoving = false;

        [SerializeField] private Character character;

        public void Init(Vector2Int gridPos)
        {
            currentGridPos = gridPos;
        }

        public bool IsMovementFinished()
        {
            return !isMoving;
        }

        public ObjectColor GetColor()
        {
            return character.characterColor;
        }

        // 🔹 NEW: Move along BFS path
        public void MoveAlongPath(List<Vector2Int> path, Hole targetHole)
        {
            if (isMoving) return;

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning($"[CharacterManager] No path provided for {name}");
                return;
            }

            if (targetHole == null || targetHole.holeCenter == null)
            {
                Debug.LogError($"[CharacterManager] Invalid hole or holeCenter for {name}");
                return;
            }

            StartCoroutine(FollowPathRoutine(path, targetHole));
        }

        // 🔹 NEW: Coroutine for BFS path movement
        private IEnumerator FollowPathRoutine(List<Vector2Int> path, Hole targetHole)
        {
            isMoving = true;

            foreach (var nextPos in path)
            {
                Vector3 worldTarget = GridManager.Instance.GetWorldPosition(nextPos.x, nextPos.y);

                // Step movement
                yield return StartCoroutine(MoveToWorldPosition(worldTarget));

                // Update grid state
                GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
                GridManager.Instance.SetCell(nextPos.x, nextPos.y, CellType.Character);

                GridManager.Instance.gridObjectDetection.UpdateCharacterPosition(this, currentGridPos, nextPos);

                currentGridPos = nextPos;
            }

            // Final approach to hole center
            Transform holeCenter = targetHole.holeCenter;
            while (Vector3.Distance(transform.position, holeCenter.position) > stopDistanceToHole)
            {
                transform.position = Vector3.MoveTowards(transform.position, holeCenter.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Clean up
            GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
            GridManager.Instance.gridObjectDetection.RemoveCharacterAt(currentGridPos);

            gameObject.SetActive(false);

            Debug.Log($"[CharacterManager] {name} reached hole {targetHole.holeColor} at {targetHole.transform.position}");
            isMoving = false;
        }

        private IEnumerator MoveToWorldPosition(Vector3 targetPos)
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

    }
}
