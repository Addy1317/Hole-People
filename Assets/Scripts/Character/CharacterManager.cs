using SlowpokeStudio.Grid;
using SlowpokeStudio.ManHole;
using SlowpokeStudio.Services;
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
        private bool isMoving = false;

        [SerializeField] private Character character;

        internal void Init(Vector2Int gridPos)
        {
            currentGridPos = gridPos;
        }

        internal bool IsMovementFinished()
        {
            return !isMoving;
        }

        internal ObjectColor GetColor()
        {
            return character.characterColor;
        }

        internal void MoveAlongPath(List<Vector2Int> path, Hole targetHole)
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

        private IEnumerator FollowPathRoutine(List<Vector2Int> path, Hole targetHole)
        {
            isMoving = true;

            foreach (var nextPos in path)
            {
                Vector3 worldTarget = GridManager.Instance.GetWorldPosition(nextPos.x, nextPos.y);

                yield return StartCoroutine(MoveToWorldPosition(worldTarget));

                GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
                GridManager.Instance.SetCell(nextPos.x, nextPos.y, CellType.Character);

                GridManager.Instance.gridObjectDetection.UpdateCharacterPosition(this, currentGridPos, nextPos);

                currentGridPos = nextPos;
            }

            Transform holeCenter = targetHole.holeCenter;
            while (Vector3.Distance(transform.position, holeCenter.position) > stopDistanceToHole)
            {
                transform.position = Vector3.MoveTowards(transform.position, holeCenter.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            GridManager.Instance.SetCell(currentGridPos.x, currentGridPos.y, CellType.Empty);
            GameService.Instance.audioManager.PlaySFX(SFXType.OnCharaceterReachedToHoleSFX);
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
