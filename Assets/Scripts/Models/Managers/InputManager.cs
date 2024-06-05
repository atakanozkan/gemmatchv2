using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockShift
{
    public class InputManager : MonoBehaviour
    {
        private Camera mainCamera;
        private RaycastHit2D[] hit;
        private bool resume =true;
        private int hit_count;
        
        
        private void Awake()
        {
            mainCamera = Camera.main;
            hit = new RaycastHit2D[1];
        }

        private void Update()
        {
            GenerateInput();
        }

        // Start is called before the first frame update
        void GenerateInput()
        {
            if (!resume)
            {
                return;
            }
            
            Vector3 pos;

            if (Input.GetMouseButtonDown(0))
            {
                pos = Input.mousePosition;
                pos.z = mainCamera.transform.position.z;
                GameObject hittedObject = GetHittedObject(pos);

                if (!hittedObject)
                {
                    return;
                }

                if (hittedObject.CompareTag("Cell"))
                {
                    Cell hitCell = hittedObject.GetComponent<Cell>();
                    GameManager.instance.OnCellTouched.Invoke(hitCell);
                }
            }
        }
        private GameObject GetHittedObject(Vector3 position)
        {
            hit_count = Physics2D.RaycastNonAlloc(mainCamera.ScreenToWorldPoint(position), Vector2.zero, hit);

            if (hit_count>0 && hit[0].collider != null)
            {
                return hit[0].collider.gameObject;
            }
            return null;
        }
        private void CheckResume(GameState state)
        {
            if (state == GameState.Playing)
            {
                resume = true;
            }
            else
            {
                resume = false;
            }
        }

        private void OnEnable()
        {
            GameManager.instance.OnGameStateChanged += CheckResume;
        }

        private void OnDisable()
        {
            GameManager.instance.OnGameStateChanged -= CheckResume;
        }
    }

}
