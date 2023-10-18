using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a dummy class. Replace with Valve's package in case of using SteamVR. Ignore in case of using Gear/Oculus/Daydream.
namespace Valve.VR
{
    public class SteamVR_TrackedCamera
    {
        public static dummy_Source Source(bool bla)
        {
            return new dummy_Source();
        }

        public class dummy_Source
        {
            public bool hasCamera = false;
            public Texture2D texture = null;
            public dummy_Bounds frameBounds = null;

            public void Acquire()
            {

            }
            public void Release()
            {

            }
        }

        public class dummy_Bounds
        {
            public float uMax;
            public float uMin;
            public float vMax;
            public float vMin;
        }
    }
}