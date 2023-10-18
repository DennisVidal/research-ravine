using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Desktop.Input;
using Icaros.Desktop.UI;
using System;

namespace Icaros.Desktop.Player {
    public class LocalPlayer : MonoBehaviour {
        public static LocalPlayer Instance = null;

        public GameObject Camera;
        public GameObject Vehicle;

        //Current Movementspeed
        public float MoveSpeed = 0f;
        //Acceleration per Tick
        public float Acceleration = 5f;

        //Multiplies the calculation result for rolls (z-axis rotation)
        public float RollRotationFactor = 2f;
        //Multiplies the calculation result for pitch (x-axis rotation) 
        public float PitchRotationFactor = 1.5f;
        
        private IInputDevice myController = null;
        private CharacterController cc;
        private float currentZ = 0f;

        private Transform camToFollow = null;

        private void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start() {
            cc = GetComponent<CharacterController>();
        }

        public void linkController(IInputDevice device) {
            myController = device;
            device.SecondButtonPressed += speedUp;
            device.ThirdButtonPressed += slowDown;
            device.xAxisRotated += rotateX;
            device.zAxisRotated += rotateZ;
        }

        public void recalibrate() {
            if (myController == null)
                return;

            if (myController.GetDeviceTypeID() == DeviceManager.ICAROS_CONTROLLER_DEVICE_ID) {
                IcarosController con = myController as IcarosController;
                con.recalibrateFor(IcarosController.Orientation.Icaros);
            }
        }

        public void recenterCamera() {
            UnityEngine.XR.InputTracking.Recenter();
        }

        void speedUp() {
            MoveSpeed += Acceleration;
        }

        void slowDown() {
            MoveSpeed -= Acceleration;
        }

        void rotateX(float x) {
            Vector3 euler = transform.localEulerAngles;
            euler.x = Mathf.Min(Mathf.Max(x * PitchRotationFactor, -65.0f), 65.0f);
            transform.localEulerAngles = euler;
        }

        void rotateZ(float z) {
            Vehicle.transform.localEulerAngles = new Vector3(0, 0, z);
            currentZ = z;
        }
        
        void Update() {
            Vector3 euler = transform.localEulerAngles;
            
            if (MoveSpeed < 0)
                euler.y += -currentZ * -RollRotationFactor * Time.deltaTime;
            else
                euler.y += currentZ * -RollRotationFactor * Time.deltaTime;

            transform.localEulerAngles = euler;
            cc.Move(transform.forward * MoveSpeed * Time.deltaTime);

            updateBody();
        }

        void updateBody() {
            if (camToFollow == null) {
                Camera[] cams = GetComponentsInChildren<Camera>();
                foreach(Camera c in cams) {
                    if (c.CompareTag("MainCamera")) {
                        camToFollow = c.transform;
                    }
                }

                return;
            }

            try {
                Vehicle.transform.position = camToFollow.transform.position;
            }catch (Exception e) {
                camToFollow = null;
            }

        }
    }
}