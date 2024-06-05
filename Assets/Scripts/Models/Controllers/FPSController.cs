using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockShift
{
    public class FPSController : MonoBehaviour
    {
        public int targetFrameRate = 60; // Set the target frame rate

        void Awake()
        {
            QualitySettings.vSyncCount = 0; // Disable vSync
            Application.targetFrameRate = targetFrameRate; // Set the target frame rate
        }
    }
}
