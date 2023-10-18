using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Icaros.Desktop.UI {
    public class SteamVRCameraTexture : MonoBehaviour {

        public int sizeRatio = 16;

        private new Renderer renderer = null;

        private void Start() {
            renderer = GetComponent<Renderer>();
        }

        void OnEnable() {
            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            var source = SteamVR_TrackedCamera.Source(true);
            source.Acquire();

            if (!source.hasCamera)
                enabled = false;
        }

        void OnDisable() {
            if (renderer != null)
                renderer.material.mainTexture = null;

            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            var source = SteamVR_TrackedCamera.Source(true);
            source.Release();
        }

        void Update() {
            var source = SteamVR_TrackedCamera.Source(true);
            Texture2D texture = source.texture;
            if (texture == null) {
                return;
            }

            texture.filterMode = FilterMode.Trilinear;
            renderer.material.mainTexture = texture;

            var bounds = source.frameBounds;
            renderer.material.mainTextureOffset = new Vector2(bounds.uMin, bounds.vMin);

            float du = bounds.uMax - bounds.uMin;
            float dv = bounds.vMax - bounds.vMin;
            renderer.material.mainTextureScale = new Vector2(du, dv);

            float aspect = (float)texture.width / texture.height;
            aspect *= Mathf.Abs(du / dv);

            transform.localScale = new Vector3(sizeRatio, sizeRatio / aspect, 1);
        }
    }
}