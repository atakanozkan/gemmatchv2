using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BlockShift
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Camera _camera;

        private void Start()
        {
            ResizeSpriteToScreen();
        }

        public void ResizeSpriteToScreen() {
            if (_renderer == null) return;
     
            transform.localScale = new Vector3(1,1,1);
     
            var width = _renderer.sprite.bounds.size.x;
            var height = _renderer.sprite.bounds.size.y;
     
            float worldScreenHeight = _camera.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height);
        }
    }

}
