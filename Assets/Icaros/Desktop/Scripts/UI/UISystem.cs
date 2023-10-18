using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Desktop.Input;
using Icaros.Desktop.Localization;
using Icaros.Desktop.Player;

namespace Icaros.Desktop.UI {
    public class UISystem : MonoBehaviour {

        public static UISystem Instance = null;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public event System.Action<string> MenuItemSelected = delegate { };
        public event System.Action PlayPressed = delegate { };
        public event System.Action CameraPositionCalibrated = delegate { };

        public Camera MenuCamera;
        public Camera VRCamera;

        public GameObject CompanyLogoTemplate;
        public float CompanyLogoDisplayTime = 3.0f;

        public Color CameraBackgroundColor;
        public LayerMask VRCameraCullingMask;
        public LayerMask MenuCameraCullingMask;
        public float viewDistance = 10f;
        
        public Sprite DefaultButton;
        public Sprite HoverButton;

        internal bool DeviceSubmitButtonPressed = false;
        internal bool DeviceSubmitButtonReleased = false;

        private CameraClearFlags outsideOfMenuFlagsForMenuCam;
        private Color outsideOfMenuColorForMenuCam;
        private int outsideOfMenuCullingMaskForMenuCam;
        private CameraClearFlags outsideOfMenuFlagsForVRCam;
        private Color outsideOfMenuColorForVRCam;
        private int outsideOfMenuCullingMaskForVRCam;
        
        private UIWarningScreen WarningScreen;
        private UIControllerSelect ControllerSelect;
        private UICompanyLogo CompanyLogoScreen;
        private UIExternalCameraScreen ExternalCameraScreen;
        private UIWaitForController WaitForController;
        private UICalibrationScreen CalibrationScreen;
        private UIReconnectController ReconnectControllerScreen;
        private UINoBluetooth NoBluetoothScreen;
        private UIControllerLost ControllerLostScreen;
        private UILookAtMonitor LookAtMonitorView;

        private bool cameraTargetingActive = false;
        private bool firstStart = true;
        private bool userSeated = false;
        private bool userMoving = true;
        private bool submitAsAny = true;

        void Start() {
            initializeComponents();
            
            initializeMenuStructure();
            DeviceManager.Instance.DeviceUsed += DeviceRunning;
            DeviceManager.Instance.DeviceReconnecting += ControllerReconnecting;
            DeviceManager.Instance.DeviceLost += ControllerLost;

            restart();
        }
        
        #region easy access
        public static void RegisterOnPlayFunction(System.Action OnPlay) {
            if(Instance != null)
            Instance.PlayPressed += OnPlay;
        }

        public static void RegisterOnMenuItemSelectedFunction(System.Action<string> OnMenuItemSelected) {
            Instance.MenuItemSelected += OnMenuItemSelected;
        }

        public static void Restart() {
            Instance.restart();
        }

        public static void BackToMainMenu() {
            Instance.restart();
            Instance.backToMainMenu();
        }

        public static void BackToExternalCameraView() {
            Instance.restart();
            Instance.backToExternalCameraView();
        }

        public static void AddLanguageToOptions(string languageID) {
            UIManager.Instance.RegisterMenuItem("ica_lang_" + languageID, "LANG_" + languageID, "ica_languages", true);
        }

        public static void RegisterMenuItem(string Id, string Title, string Parent) {
            UIManager.Instance.RegisterMenuItem(Id, Title, Parent);
        }
        public static void RegisterPlayMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterMenuItem(Id, Title, "ica_play");
        }
        public static void RegisterOptionsMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterMenuItem(Id, Title, "ica_options", true);
        }
        public static void RegisterUnlistedMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterUnlistedMenuItem(Id, Title);
        }

        public static void OpenMenu(string Id) {
            UIManager.Instance.OpenMenu(Id);
        }
        public static void OpenMainMenu() {
            UIManager.Instance.OpenMenu();
        }
        public static void OpenOptionsMenu() {
            UIManager.Instance.OpenMenu("ica_options");
        }

        public static void CloseUI() {
            Instance.closeUI();
        }
        #endregion

        internal void ControllerLost(IInputDevice device) {
            showControllerLostScreen();
        }

        internal void ControllerReconnecting(DeviceManager.reconnectingDeviceState state){
            if (state == DeviceManager.reconnectingDeviceState.ReconnectStarted) ReconnectControllerScreen.Show();
            else ReconnectControllerScreen.Hide();
        }

        internal void OnControllerLostScreenClosed() {
            ControllerSelect.Show();
        }

        internal void OnWarningScreenClosed() {
            ControllerSelect.clear();
            DeviceManager.Instance.Rescan();
            CompanyLogoScreen.Show();
        }

        internal void OnCompanyLogoDisplayFinished() {
            ControllerSelect.Show();
        }

        internal void OnControllerSelected(IInputDevice device) {
            DeviceManager.Instance.UseDevice(device);
            WaitForController.Show();
        }

        internal void OnUserSeated() {
            if (userSeated)
                return;
            
            submitAsAny = false;

            CalibrationScreen.Hide();
            OpenMainMenu();
            userSeated = true;

            CameraPositionCalibrated();
        }

        internal void DeviceRunning(IInputDevice device) {
            device.FirstButtonPressed += OnUISubmitButtonPressed;
            device.FirstButtonReleased += OnUISubmitButtonReleased;

            device.SecondButtonPressed += OnUIPreviousButtonPressed;
            device.ThirdButtonPressed += OnUINextButtonPressed;

            device.FirstButtonPressed += OnUIAnyButtonPressed;
            device.SecondButtonPressed += OnUIAnyButtonPressed;
            device.ThirdButtonPressed += OnUIAnyButtonPressed;
            device.FourthButtonPressed += OnUIAnyButtonPressed;

            WaitForController.Hide();
            showExternalCameraScreen();
        }

        internal void OnUIAnyButtonPressed() {
            if (!submitAsAny)
                return;

            if (userMoving) {
                CalibrationScreen.Show(1);
                userMoving = false;
                return;
            }

            OnUserSeated();
        }

        internal void OnUIPreviousButtonPressed() {
            UIManager.Instance.selectPreviousButton();
        }
        
        internal void OnUINextButtonPressed() {
            UIManager.Instance.selectNextButton();
        }

        internal void OnUISubmitButtonPressed() {
            if (submitAsAny)
                return;

            DeviceSubmitButtonPressed = true;
            DeviceSubmitButtonReleased = false;

            UIManager.Instance.submitButtonPressed();
        }

        internal void OnUISubmitButtonReleased() {
            if (submitAsAny)
                return;

            DeviceSubmitButtonReleased = true;
            DeviceSubmitButtonPressed = false;
        }

        internal void OnUIMenuButtonClicked(string id) {
            MenuItemSelected(id);

            switch (id) {
                case "backToRoot":
                    OpenMainMenu();
                    break;
                case "backToOptions":
                    OpenOptionsMenu();
                    break;
                case "ica_play":
                    PlayPressed();
                    break;
                default:
                    if (id.StartsWith("ica_lang_")) {
                        string lang = id.Substring(9, 2);
                        LocalizationManager.SetLanguage(lang);
                        OpenOptionsMenu();
                    }
                    break;
            }
        }

        private void closeUI() {
            UIManager.Instance.CloseMenu();
            ExternalCameraScreen.Hide();
            
            if (cameraTargetingActive)
                revertCamera();
            Instance.cameraTargetingActive = false;
        }

        private void showExternalCameraScreen() {
            setCameraToRWT();
            submitAsAny = true;
            userSeated = false;
            userMoving = true;
            ExternalCameraScreen.Show();
            LookAtMonitorView.Hide();
            CalibrationScreen.Show(0);
        }

        private void enableCameraTargetting() {
            if (!cameraTargetingActive)
                tintCamera();
            cameraTargetingActive = true;
        }

        private void restart() {
            enableCameraTargetting();
            
            WarningScreen.Show();
            LookAtMonitorView.Show();
            CompanyLogoScreen.Hide();
            ControllerSelect.Hide();
            ExternalCameraScreen.Hide();
            CalibrationScreen.Hide();
            WaitForController.Hide(true);
            ReconnectControllerScreen.Hide();
            ControllerLostScreen.Hide();
            NoBluetoothScreen.Hide();
            UIManager.Instance.CloseMenu();
        }

        private void showControllerLostScreen() {
            restart();
            WarningScreen.Hide();
            LookAtMonitorView.Hide();
            ControllerSelect.clear();
            DeviceManager.Instance.Rescan();
            ControllerLostScreen.Show();
        }

        internal void showNoBluetoothScreen() {
            restart();
            WarningScreen.Hide();
            LookAtMonitorView.Hide();
            NoBluetoothScreen.Show();
        }

        private void backToExternalCameraView() {
            WarningScreen.Hide();
            LookAtMonitorView.Hide();
            showExternalCameraScreen();
        }

        private void backToMainMenu() {
            backToExternalCameraView();
            OnUserSeated();
        }

        private void tintCamera() {
            outsideOfMenuFlagsForMenuCam = MenuCamera.clearFlags;
            outsideOfMenuColorForMenuCam = MenuCamera.backgroundColor;
            outsideOfMenuCullingMaskForMenuCam = MenuCamera.cullingMask;
            MenuCamera.clearFlags = CameraClearFlags.SolidColor;
            MenuCamera.backgroundColor = CameraBackgroundColor;
            MenuCamera.cullingMask = MenuCameraCullingMask;

            outsideOfMenuFlagsForVRCam = VRCamera.clearFlags;
            outsideOfMenuColorForVRCam = VRCamera.backgroundColor;
            outsideOfMenuCullingMaskForVRCam = VRCamera.cullingMask;
            VRCamera.clearFlags = CameraClearFlags.SolidColor;
            VRCamera.backgroundColor = CameraBackgroundColor;
            VRCamera.cullingMask = VRCameraCullingMask;
        }

        private void setCameraToRWT() {
            MenuCamera.clearFlags = CameraClearFlags.Depth;
            VRCamera.clearFlags = CameraClearFlags.Depth;
        }
        private void undoRWT() {
            MenuCamera.clearFlags = CameraClearFlags.SolidColor;
            VRCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void revertCamera() {
            MenuCamera.clearFlags = outsideOfMenuFlagsForMenuCam;
            MenuCamera.backgroundColor = outsideOfMenuColorForMenuCam;
            MenuCamera.cullingMask = outsideOfMenuCullingMaskForMenuCam;
            VRCamera.clearFlags = outsideOfMenuFlagsForVRCam;
            VRCamera.backgroundColor = outsideOfMenuColorForVRCam;
            VRCamera.cullingMask = outsideOfMenuCullingMaskForVRCam;
        }

        private void initializeComponents() {
            WarningScreen = GetComponentInChildren<UIWarningScreen>();
            ControllerSelect = GetComponentInChildren<UIControllerSelect>();
            CompanyLogoScreen = GetComponentInChildren<UICompanyLogo>();
            ExternalCameraScreen = GetComponentInChildren<UIExternalCameraScreen>();
            WaitForController = GetComponentInChildren<UIWaitForController>();
            CalibrationScreen = GetComponentInChildren<UICalibrationScreen>();
            ReconnectControllerScreen = GetComponentInChildren<UIReconnectController>();
            ControllerLostScreen = GetComponentInChildren<UIControllerLost>();
            NoBluetoothScreen = GetComponentInChildren<UINoBluetooth>();
            LookAtMonitorView = GetComponentInChildren<UILookAtMonitor>();
        }

        private void initializeMenuStructure() {
            UIManager.Instance.RegisterMenuItem("ica_play", "PLAY");

            UIManager.Instance.RegisterMenuItem("ica_options", "OPTIONS");
            UIManager.Instance.RegisterMenuItem("ica_languages", "LANGUAGES", "ica_options");
            UIManager.Instance.RegisterMenuItem("backToRoot", "BACK", "ica_options");
            UIManager.Instance.RegisterMenuItem("backToOptions", "BACK", "ica_languages");

            AddLanguageToOptions("EN");
        }
    }
}
