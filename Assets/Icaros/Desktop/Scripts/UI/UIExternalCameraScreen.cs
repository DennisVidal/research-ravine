using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Desktop.UI {
    public class UIExternalCameraScreen : MonoBehaviour {

        public new Camera camera;
        public SteamVRCameraTexture sct;

        void Start() {
            camera.backgroundColor = UISystem.Instance.CameraBackgroundColor;
            sct.gameObject.SetActive(true);
        }

        public void Hide() {
            sct.enabled = false;
            camera.gameObject.SetActive(false);
        }

        public void Show() {
            camera.gameObject.SetActive(true);
            sct.enabled = true;
        }
    }
}
