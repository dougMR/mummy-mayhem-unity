// From: https://docs.unity3d.com/Manual/WheelColliderTutorial.html

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque = 50; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle = 35; // maximum steer angle the wheel can have

    // CUstom

    public float brakeTorque = 200;
    public GameObject ornament;
    public AudioClip engineClip;
    public AudioClip klunkClip;
    public AudioClip brakesClip;
    private AudioSource _engineSound;
    private AudioSource _klunk;
    private AudioSource _brakes;
    private bool _playerClose = false;
    private GameObject _driver;
    private PlayerMovementScript _PMScript;
    private Rigidbody _rb;
    private GameManager GM;
    private float _mph = 0;
    private float _maxMph = 50;
    private bool _braking = false;
    private float _inputZ;
    private float _minSpeedometerAngle = -140f;
    private float _maxSpeedometerAngle = 140f;
    private GameObject _needle;
    private float _minEngineVol = 0.05f; // based on engine sound I'm using at the time
    private float _maxEngineVol = 0.2f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        GM = GameManager.Instance;
        _needle = GameObject.Find("needle");
        Debug.Log("Needle: "+_needle);

        _engineSound = gameObject.AddComponent<AudioSource>();
        _engineSound.clip = engineClip;
        _engineSound.loop = true;
        _engineSound.volume = _minEngineVol;
        _klunk = gameObject.AddComponent<AudioSource>();
        _klunk.clip = klunkClip;
        _brakes = gameObject.AddComponent<AudioSource>();
        _brakes.clip = brakesClip;
        _brakes.volume = 0.5f;
        
        // float speedThreshold = 60f;
        // int stepsBelowThreshold = 1;
        // int stepsAboveThreshold = 1;
        // foreach (AxleInfo axleInfo in axleInfos)
        // {
        //     axleInfo.leftWheel.ConfigureVehicleSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        //     axleInfo.rightWheel.ConfigureVehicleSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        // }
    }

    void UpdateSpeedometer(){
        float pct = _mph / _maxMph;
        float degreeRange = _maxSpeedometerAngle - _minSpeedometerAngle;
        float needleAngle = _minSpeedometerAngle + degreeRange * pct;
        _needle.transform.localRotation = Quaternion.AngleAxis(needleAngle, Vector3.forward);
    }

    void OnCollisionEnter(Collision collision)
    {
        float magnitude = (collision.impulse / Time.fixedDeltaTime).magnitude;
        float min = 1000.0f;
        if (magnitude > min ) {
            float range = 4000.0f;
            _klunk.volume = Mathf.Clamp((magnitude - min)/range, 0.1f, 0.8f);
            _klunk.Play();
            Debug.Log("Car Collision, magnitude :: "+magnitude);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Is User pressing E key?
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (_playerClose && _driver == null)
            {
                SetDriver(GameObject.Find("Player"));
            }
            else if (_driver != null)
            {
                RemoveDriver();
            }
        }
        if (_driver != null)
        {
            float carSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            // 1 m/s = 3.6 km/h. 1km/h = 0,621371 Mph
            float KMpH = 3.6f * carSpeed;
            _mph = KMpH * 0.621371f;
            // Debug.Log("Car MPH ==> " + Mathf.Round(_mph * 10) / 10);

            // Engine sound
            float speedPct = ( Mathf.Abs(_mph) / _maxMph );
            int numGears = 4;
            float pctPerGear = 1f / numGears;
            float gear = Mathf.Clamp(Mathf.Ceil(speedPct / pctPerGear), 1, 4);
            float gearPct = (speedPct - (gear - 1) * pctPerGear) / pctPerGear;
            float vol = _minEngineVol + (_maxEngineVol-_minEngineVol) * speedPct;
            _engineSound.pitch = 0.2f + (1f * pctPerGear * gear) + 0.6f * gearPct;
            _engineSound.volume = vol;

            UpdateSpeedometer();

            if ( Input.GetKey(KeyCode.Space) )
            {
                Debug.Log("Brake Engaged");
                _braking = true;

            } else {

                float rpm = (axleInfos[0].leftWheel.rpm + axleInfos[0].rightWheel.rpm) * 0.5f;
                rpm = Mathf.Round(rpm * 10) / 10;

                // If rpm != 0 and wheels are turning opposite direction from _inputZ, apply brake
                _braking = Mathf.Abs(rpm) > 5 && _inputZ != 0 && Mathf.Sign(rpm) != Mathf.Sign(_inputZ);
            }
        }
    }


    public void FixedUpdate()
    {
        if (_driver != null)
        {
            float motor = 0;
            _inputZ = Input.GetAxis("Vertical");
                // Debug.Log("_inputZ :: " + _inputZ);
            if(_mph < _maxMph) {
                motor = maxMotorTorque * _inputZ;
            }
            
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

            if (_braking && _mph > 30f)
            {
                if(!_brakes.isPlaying) {
                    // start skidding
                    _brakes.Play();
                } else {
                    // continue skidding
                    if (_brakes.time > _brakes.clip.length * 0.65 ){
                        _brakes.time = _brakes.clip.length * 0.3f;
                    }
                }
            }
            // Set color of hood ornament to show _braking / gas
            Renderer renderer = ornament.GetComponent<Renderer>();
            Color speedColor = _braking ? Color.red : _inputZ == 0 ? Color.grey : Color.green;
            renderer.material.SetColor("_Color", speedColor);
            
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    // if(_braking){
                    //     axleInfo.leftWheel.steerAngle = steering * 1.7f;
                    //     axleInfo.rightWheel.steerAngle = steering * 1.7f;
                    // } else {
                        axleInfo.leftWheel.steerAngle = steering;
                        axleInfo.rightWheel.steerAngle = steering;
                    // }
                }
                if (axleInfo.motor)
                {
                    // Slip when _braking
                    WheelFrictionCurve sidewaysWfc;
                    WheelFrictionCurve forwardWfc;
                    sidewaysWfc = axleInfo.leftWheel.sidewaysFriction;
                    forwardWfc = axleInfo.leftWheel.forwardFriction;
                    // sidewaysWfc.extremumValue = _braking ? 0.5f : 1.0f;
                    // Debug.Log("sidewaysWfc.extremumValue :: "+sidewaysWfc.extremumValue);
                    sidewaysWfc.extremumSlip = _braking ? 0.5f : 0.2f;
                    sidewaysWfc.asymptoteSlip = _braking ? 0.7f : 0.5f;
                    axleInfo.leftWheel.sidewaysFriction = axleInfo.rightWheel.sidewaysFriction = sidewaysWfc;
                    forwardWfc.extremumSlip = _braking ? 1.0f : 0.4f;
                    forwardWfc.asymptoteSlip = _braking ? 2.0f : 0.8f;
                    axleInfo.leftWheel.forwardFriction = axleInfo.rightWheel.forwardFriction = forwardWfc;

                    if (_braking)
                    {
                        axleInfo.leftWheel.brakeTorque = axleInfo.rightWheel.brakeTorque = brakeTorque;
                        axleInfo.leftWheel.motorTorque = 0;
                        axleInfo.rightWheel.motorTorque = 0;
                    }
                    else
                    {
                        axleInfo.leftWheel.brakeTorque = axleInfo.rightWheel.brakeTorque = 0;

                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                        
                    }
                    
                }
                // Align Wheel Mesh with Wheel Collider
                
                Quaternion quat;
                Vector3 position;
                axleInfo.leftWheel.GetWorldPose(out position, out quat);
                // subtract car's rotation from wheel's world rotation
                quat = Quaternion.Inverse(transform.rotation) * quat;
                // axleInfo.leftWheelGO.transform.position = position;
                axleInfo.leftWheelGO.transform.localRotation = quat;
                
                axleInfo.rightWheel.GetWorldPose(out position, out quat);
                quat = Quaternion.Inverse(transform.rotation) * quat;
                // axleInfo.rightWheelGO.transform.position = position;
                axleInfo.rightWheelGO.transform.localRotation = quat;
                
                
            }
        }
    }

    void Go(){

    }

    void OnTriggerEnter(Collider other)
    {

        // Is Player in my Trigger?
        if (other.name == "Player")
        {
            _playerClose = true;
            GM.ShowMessage("Press 'E' to get in / out of Car.");
        }
    }
    void OnTriggerExit(Collider other)
    {

        // Is Player in my Trigger?
        if (other.name == "Player")
        {
            _playerClose = false;
        }
    }

    void SetDriver(GameObject driver)
    {
        Debug.Log("SetDriver()");
        _engineSound.Play();
        _driver = driver;
        Rigidbody driverRB = _driver.GetComponent<Rigidbody>();
        _PMScript = _driver.GetComponent<PlayerMovementScript>();

        _PMScript.StartDriving();
        driverRB.isKinematic = true;
        driverRB.useGravity = false;
        _driver.transform.parent = transform;
        _driver.transform.position = transform.position;
        _driver.GetComponent<CapsuleCollider>().enabled = false;
        _driver.transform.rotation = transform.rotation;
        //_engineSound.Play();
        GM.ShowMessage("W/A/S/D or Arrowkeys to Drive.");
        // _braking = false;
    }

    void RemoveDriver()
    {
        Debug.Log("RemoveDriver()");

        _engineSound.Stop();
        Rigidbody driverRB = _driver.GetComponent<Rigidbody>();
        driverRB.isKinematic = false;
        driverRB.useGravity = true;
        _driver.transform.parent = null;
        _driver.GetComponent<CapsuleCollider>().enabled = true;

        _PMScript.StopDriving();

        // Stand Driver Upright
        _driver.transform.eulerAngles = new Vector3(
            0,
            _driver.transform.forward.y,
            0
        );
        _driver.transform.position = transform.position + new Vector3(0f, 2f, 0f);
        _driver = null;
        _PMScript = null;
        // _engineSound.Stop();
        foreach (AxleInfo axleInfo in axleInfos){
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.brakeTorque = axleInfo.rightWheel.brakeTorque = brakeTorque;
                axleInfo.leftWheel.motorTorque = 0;
                axleInfo.rightWheel.motorTorque = 0;
            }
        }
                        
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public GameObject leftWheelGO;
    public GameObject rightWheelGO;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}