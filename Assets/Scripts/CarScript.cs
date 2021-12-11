using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public AudioClip engineClip;
    public AudioClip klunkClip;
    public AudioClip brakesClip;
    private bool _playerClose = false;
    private AudioSource _engineSound;
    private AudioSource _klunk;
    private AudioSource _brakes;
    private GameObject _driver;
    private PlayerMovementScript _PMScript;
    private Rigidbody _rb;
    private float _inputX, _inputZ, _moveSpeed = 0f, _acceleration = 20.0f, _maxSpeed = 32f;
    // _rotateSpeed = 32f;
    private float _gravity = 200f;
    private GameManager GM;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _engineSound = gameObject.AddComponent<AudioSource>();
        _engineSound.clip = engineClip;
        _engineSound.loop = true;
        _engineSound.volume = 0.6f;
        _klunk = gameObject.AddComponent<AudioSource>();
        _klunk.clip = klunkClip;
        _brakes = gameObject.AddComponent<AudioSource>();
        _brakes.clip = brakesClip;
        GM = GameManager.Instance;
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
    }

    void FixedUpdate()
    {
        if (_driver != null)
        {
            _inputX = Input.GetAxis("Horizontal");
            _inputZ = Input.GetAxis("Vertical");

            // Debug.Log("BEFORE MOVE Velocity.sqrMagnitude: " + _rb.velocity.sqrMagnitude);
            // Debug.Log("Mathf.Approximately(_rb.velocity.sqrMagnitude, 0f) :: "+(Mathf.Approximately(_rb.velocity.sqrMagnitude, 0f)));

            move();
            _moveSpeed = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z;
            if (Mathf.Abs(_moveSpeed) < 0.1)
            {
                _moveSpeed = 0;
            }
            //_rb.velocity.sqrMagnitude < _maxSpeed*_maxSpeed
            _engineSound.pitch = 0.5f + 1.5f * (Mathf.Abs(_moveSpeed) / _maxSpeed);

            if (_moveSpeed != 0)
                rotate();

        }

    }

    private void move()
    {
        if (isGrounded())
        {
            if (_inputZ > 0 && Mathf.Abs(_moveSpeed) < _maxSpeed)
            {
                // Forward
                // Debug.Log("Forward " + Time.deltaTime);
                _rb.AddForce(transform.forward * _acceleration, ForceMode.Acceleration);
            }
            else if (_inputZ < 0 && Mathf.Abs(_moveSpeed) < _maxSpeed)
            {
                // Reverse
                // Debug.Log("Reverse");
                if (!_brakes.isPlaying && _moveSpeed > _maxSpeed / 2)
                {
                    _brakes.Play();
                }
                _rb.AddForce(-transform.forward * _acceleration, ForceMode.Acceleration);
            }
            else if (_rb.velocity.sqrMagnitude > Mathf.Pow(0.1f, 2f))
            {
                // Decelerate
                // Debug.Log("Decelerating");

                _rb.velocity = _rb.velocity * 0.99f;
                if (_rb.velocity.sqrMagnitude < Mathf.Pow(0.1f, 2f))
                {
                    _rb.velocity = Vector3.zero;
                    // Debug.Log(_rb.velocity.sqrMagnitude+" :: STOPPED");
                }
            }
        }
        else
        {
            // Debug.Log("PUSHING DOWN");
            _rb.AddForce((-Vector3.up) * Time.deltaTime * _gravity, ForceMode.Impulse);
        }

        // Vector3 rayDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        // _rb.velocity += rayDirection * _moveSpeed;
        // Debug.Log("Velocity.sqrMagnitude: " + _rb.velocity.sqrMagnitude);

    }
    private void rotate()
    {
        if (isGrounded())
        {

            // float relZvel = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z;
            float rotX = _inputX * (_moveSpeed < 0 ? -1 : 1); // * (_rb.velocity.sqrMagnitude / (_maxSpeed*_maxSpeed)) ;

            transform.Rotate(Vector3.up * rotX);
            // Debug.Log("_moveSpeed: "+_moveSpeed);
            _rb.AddForce(transform.right * _acceleration * rotX, ForceMode.Acceleration);
            // transform.Translate(transform.right * (-rotX));
        }

    }

    private bool isGrounded()
    {
        // RaycastHit hit;
        RaycastHit closestValidHit = new RaycastHit();
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -Vector3.up, 4f);
        foreach (RaycastHit hit in hits)
        {
            if (!hit.transform.IsChildOf(transform) && (closestValidHit.collider == null || closestValidHit.distance > hit.distance))
            {
                closestValidHit = hit;
            }
        }
        // if (Physics.Raycast(transform.position, -Vector3.up, out hit))


        if (closestValidHit.collider != null)
        {
            float dist = closestValidHit.distance;
            if (dist < 3.5)
            {
                // Debug.Log(hit);
                // Debug.Log("grounded");
                // Debug.DrawLine(transform.position, hit.point, Color.green);
                // Debug.Log("GROUNDED: " + closestValidHit.distance);
                // Debug.Log(":::" + closestValidHit.collider.name);
                return true;
            }
            else
            {
                // Debug.DrawRay(transform.position, -Vector3.up * 0.1f, Color.red);
                // Debug.Log("NOT GROUNDED: " + closestValidHit.distance);
                return false;
            }
        }
        // Debug.Log("NOT GROUNDED: ");
        return false;
    }

    void OnTriggerEnter(Collider other)
    {

        // Is Player in my Trigger?
        if (other.name == "Player")
        {
            _playerClose = true;
            GM.ShowMessage("Press E key to get in / out of Car.");
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
    void OnCollisionEnter(Collision collision)
    {
        float magnitude = (collision.impulse / Time.fixedDeltaTime).magnitude;
        float min = 1000.0f;
        if (magnitude > min)
        {
            Debug.Log("Car collision magnitude:: " + magnitude);
            Debug.Log("Car collision impulse:: " + collision.impulse);
            Debug.Log("Collision other :: " + collision.other.name);
            Debug.Log("Car Collision name:: " + gameObject.name);
            float range = 4000.0f;
            _klunk.volume = Mathf.Clamp((magnitude - min) / range, 0.1f, 1);
            _klunk.Play();
        }

    }

    void SetDriver(GameObject driver)
    {
        Debug.Log("SetDriver()");
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
        _engineSound.Play();
        GM.ShowMessage("W/A/S/D or Arrowkeys to Drive.");
    }

    void RemoveDriver()
    {
        Debug.Log("RemoveDriver()");

        Rigidbody driverRB = _driver.GetComponent<Rigidbody>();
        driverRB.isKinematic = false;
        driverRB.useGravity = true;
        _driver.transform.parent = null;
        _driver.GetComponent<CapsuleCollider>().enabled = true;

        _PMScript.StopDriving();

        // Quaternion targetRot = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
        // Stand Driver Upright
        _driver.transform.eulerAngles = new Vector3(
            0,
            _driver.transform.forward.y,
            0
        );
        _driver.transform.position = transform.position + new Vector3(0f, 2f, 0f);
        // _driver.transform.rotation.SetLookRotation(_driver.transform.forward, Vector3.up);
        _driver = null;
        _PMScript = null;
        _engineSound.Stop();
    }
}
