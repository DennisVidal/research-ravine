using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Desktop.Input;
using UnityEngine.EventSystems;

public class InputManager : BaseInput
{
    public static InputManager Instance;

    public Camera eventCamera = null;

    bool[] isButtonPressed;
    float[] buttonPressedDurations;
    Vector3 axisRotation;
    Quaternion rotationQuaternion;

    public event System.Action<int> onButtonPressed;
    public event System.Action<int, float> onButtonKeepPressed;
    public event System.Action<int, float> onButtonReleased;
    public event System.Action<int, float, float> onAxisRotated;
    public event System.Action<Quaternion> onAxisRotatedQuaternion;


    protected override void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isButtonPressed = new bool[4];
        buttonPressedDurations = new float[4];
        axisRotation = new Vector3();

        DeviceManager.Instance.NewDeviceRegistered += OnDeviceFound;
        DeviceManager.Instance.DeviceLost += OnDeviceLost;
        DeviceManager.Instance.DeviceUsed += OnDeviceUsed;

        DeviceManager.Instance.init();
        DeviceManager.Instance.initialized = true;
        DeviceManager.Instance.Rescan();

        GetComponent<BaseInputModule>().inputOverride = this;
    }

    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            if(isButtonPressed[i])
            {
                buttonPressedDurations[i] += Time.deltaTime;
                OnButtonKeepPressed(i);
            }
        }
    }


    public override bool GetMouseButton(int button)
    {
        return GetIcarosButton(button);
    }
    public override bool GetMouseButtonDown(int button)
    {
        return GetIcarosButtonDown(button);
    }
    public override bool GetMouseButtonUp(int button)
    {
        return GetIcarosButtonUp(button);
    }
    public override Vector2 mousePosition
    {
        get
        {
            Camera cam = GetEventCamera();
            return new Vector2(cam.pixelWidth, cam.pixelHeight) * 0.5f;
        }
    }


    public Camera GetEventCamera()
    {
        if(eventCamera == null)
        {
            eventCamera = Camera.main;
        }
        return eventCamera;
    }



    void OnDeviceFound(IInputDevice device)
    {
        if (device.GetDeviceTypeID() == DeviceManager.KEYBOARD_DEVICE_ID ||
            device.GetDeviceTypeID() == DeviceManager.ICAROS_CONTROLLER_DEVICE_ID)
        {
            DeviceManager.Instance.UseDevice(device);
        }
    }

    void OnDeviceLost(IInputDevice device)
    {
    }

    void OnDeviceUsed(IInputDevice device)
    {
        device.FirstButtonPressed += OnFirstButtonPressed;
        device.SecondButtonPressed += OnSecondButtonPressed;
        device.ThirdButtonPressed += OnThirdButtonPressed;
        device.FourthButtonPressed += OnFourthButtonPressed;

        device.FirstButtonReleased += OnFirstButtonReleased;
        device.SecondButtonReleased += OnSecondButtonReleased;
        device.ThirdButtonReleased += OnThirdButtonReleased;
        device.FourthButtonReleased += OnFourthButtonReleased;

        device.xAxisRotated += OnXAxisRotated;
        device.yAxisRotated += OnYAxisRotated;
        device.zAxisRotated += OnZAxisRotated;
        device.RotationChanged += OnRotationChanged;

        if (device.GetDeviceTypeID() == DeviceManager.KEYBOARD_DEVICE_ID) //keyboard input of second button is inverted for some reason
        {
            device.SecondButtonPressed -= OnSecondButtonPressed;
            device.SecondButtonReleased -= OnSecondButtonReleased;

            device.SecondButtonPressed += OnSecondButtonReleased;
            device.SecondButtonReleased += OnSecondButtonPressed;
        }

    }

    void OnButtonPressed(int button)
    {
        if (onButtonPressed != null)
        {
            onButtonPressed.Invoke(button);
        }
    }
    void OnButtonKeepPressed(int button)
    {
        if (onButtonKeepPressed != null)
        {
            onButtonKeepPressed.Invoke(button, buttonPressedDurations[button]);
        }
    }
    void OnButtonReleased(int button)
    {
        if (onButtonReleased != null)
        {
            onButtonReleased.Invoke(button, buttonPressedDurations[button]);
        }
        buttonPressedDurations[button] = 0.0f;
    }
    void OnAxisRotated(int axis, float angle, float deltaAngle)
    {
        if (onAxisRotated != null)
        {
            onAxisRotated.Invoke(axis, angle, deltaAngle);
        }
    }






    void OnFirstButtonPressed()
    {
        isButtonPressed[0] = true;
        OnButtonPressed(0);
    }
    void OnSecondButtonPressed()
    {
        isButtonPressed[1] = true;
        OnButtonPressed(1);
    }
    void OnThirdButtonPressed()
    {
        isButtonPressed[2] = true;
        OnButtonPressed(2);
    }
    void OnFourthButtonPressed()
    {
        isButtonPressed[3] = true;
        OnButtonPressed(3);
    }




    void OnFirstButtonReleased()
    {
        isButtonPressed[0] = false; 
        OnButtonReleased(0);
    }
    void OnSecondButtonReleased()
    {
        isButtonPressed[1] = false;
        OnButtonReleased(1);
    }
    void OnThirdButtonReleased()
    {
        isButtonPressed[2] = false;
        OnButtonReleased(2);
    }
    void OnFourthButtonReleased()
    {
        isButtonPressed[3] = false;
        OnButtonReleased(3);
    }




    void OnXAxisRotated(float x)
    {
        float deltaAngle = x - axisRotation.x;
        axisRotation.x = x;
        OnAxisRotated(0, x, deltaAngle);
    }
    void OnYAxisRotated(float y)
    {
        float deltaAngle = y - axisRotation.y;
        axisRotation.y = y;
        OnAxisRotated(1, y, deltaAngle);
    }
    void OnZAxisRotated(float z)
    {
        float deltaAngle = z - axisRotation.z;
        axisRotation.z = z;
        OnAxisRotated(2, z, deltaAngle);
    }
    void OnRotationChanged(Quaternion q)
    {
        rotationQuaternion = q;
        if (onAxisRotatedQuaternion != null)
        {
            onAxisRotatedQuaternion.Invoke(rotationQuaternion);
        }
    }



    public bool GetIcarosButton(int button)
    {
        return IsButtonPressed(button);
        //return Input.GetButton("ICAROS_Button" + (button + 1));
    }
    public bool GetIcarosButtonDown(int button)
    {
        return IsButtonPressed(button);
        //return Input.GetButtonDown("ICAROS_Button" + (button + 1));
    }
    public bool GetIcarosButtonUp(int button)
    {
        return !IsButtonPressed(button);
        //return Input.GetButtonUp("ICAROS_Button" + (button + 1));
    }


    public bool IsButtonPressed(int b)
    {
        return isButtonPressed[b];
    }
    public float GetRotation(int axis)
    {
        return axisRotation[axis];
    }
    public Vector3 GetRotation()
    {
        return axisRotation;
    }
    public Quaternion GetRotationQuaternion()
    {
        return rotationQuaternion;
    }

}
