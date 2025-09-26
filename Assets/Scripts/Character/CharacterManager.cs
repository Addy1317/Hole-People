using SlowpokeStudio.Grid;
using SlowpokeStudio.ManHole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.character
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stopDistanceToHole = 0.1f;

        private Vector2Int currentGridPos;
        private ObjectColor myColor;
        private bool isMoving = false;

        [SerializeField] private Character character;

        private void Awake()
        {
            //myColor = GetComponent<Character>().characterColor;
           // myColor = character.characterColor;
           
        }

        public ObjectColor GetColor()
        {
            return character.characterColor;
        }

        public void MoveToHole(Hole targetHole)
        {
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
    }
}
