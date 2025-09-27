using UnityEditor;
using UnityEngine;

namespace SlowpokeStudio.character
{
    public enum ObjectColor { Red, Blue, Green, Yellow }
    public class Character : MonoBehaviour
    {
        [Header("Assigned on prefab or in Inspector")]
        public ObjectColor characterColor;

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

            ApplyColor();
        }

        private void Start()
        {
            ApplyColor();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (_renderer == null)
                    _renderer = GetComponent<Renderer>();

                ApplyColor();
                EditorApplication.QueuePlayerLoopUpdate(); 
                SceneView.RepaintAll(); 
            }
        }
#endif

        private void ApplyColor()
        {
            if (_renderer == null) return;

            switch (characterColor)
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