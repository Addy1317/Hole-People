using SlowpokeStudio.character;
using UnityEditor;
using UnityEngine;

namespace SlowpokeStudio.ManHole
{
    public class Hole : MonoBehaviour
    {
        public Transform holeCenter;

        [Header("Assigned on prefab or in Inspector")]
        public ObjectColor holeColor;

        [Header("Material Mapping")]
        public Material redMat;
        public Material greenMat;
        public Material blueMat;
        public Material yellowMat;

        private Renderer _renderer;

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();

            if (holeCenter == null)
                Debug.LogError($"[Hole] HoleCenter is not assigned in {gameObject.name}");

            ApplyColor();
        }

        private void Start()
        {
            // Ensures color is set on scene load in play mode
            ApplyColor();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Updates color immediately in the Editor when anything is changed
            if (!Application.isPlaying)
            {
                if (_renderer == null)
                    _renderer = GetComponent<Renderer>();

                ApplyColor();
                EditorApplication.QueuePlayerLoopUpdate(); // forces update
                SceneView.RepaintAll(); // visually refresh scene
            }
        }
#endif

        private void ApplyColor()
        {
            if (_renderer == null) return;

            switch (holeColor)
            {
                case ObjectColor.Red:
                    _renderer.sharedMaterial = redMat;
                    break;
                case ObjectColor.Green:
                    _renderer.sharedMaterial = greenMat;
                    break;
                case ObjectColor.Blue:
                    _renderer.sharedMaterial = blueMat;
                    break;
                case ObjectColor.Yellow:
                    _renderer.sharedMaterial = yellowMat;
                    break;
            }
        }
    }
}

