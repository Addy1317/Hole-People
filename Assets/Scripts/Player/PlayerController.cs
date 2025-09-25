using SlowpokeStudio.Hole;
using UnityEngine;

namespace SlowpokeStudio.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float rayDistance = 100f;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // tap/click
            {
                DetectHoleTap();
            }
        }

        private void DetectHoleTap()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                HoleColor hole = hit.collider.GetComponent<HoleColor>();
                if (hole != null)
                {
                    Debug.Log($"[PlayerController] Tapped Hole with color: {hole.Color}");
                    // 👇 Later we’ll notify GridManager here
                }
                else
                {
                    Debug.Log("[PlayerController] Tapped something that is not a hole.");
                }
            }
        }
    }
}
