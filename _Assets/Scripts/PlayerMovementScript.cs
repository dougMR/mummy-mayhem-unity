using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{

    public AudioClip oofClip;
    private AudioSource oofSound;
            
    private bool _isDriving;

    float inputX, inputZ, mouseXmove, moveSpeed = 6f, rotateSpeed = 24f;
    // public Text messageText;
    private Rigidbody _rb;
    private float jumpForce = 10f;
    RaycastHit hit;
    private Vector3 _groundNormal = Vector3.up;
    // private bool _isOnSlope = false;
    private float _slopeLimit = 10f;
    private float _groundFriction = 1;
    private Vector3 _groundSlope = Vector3.zero;
    private GameManager GM;
    private bool _climbing = false;
    private float _climbMaxSpeedPercent = 0.7f;
    private float _climbStepDistance = 2.0f;
    private float _startClimbHeight = 0f;
    private float _strideHeight = 0.06f;

    void Start()
    {

        _rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        // Periodically check slope
        // InvokeRepeating("GetGroundSlope", 0f, 1f);
        GM = GameManager.Instance;
        if (oofClip != null)
        {
            oofSound = gameObject.AddComponent<AudioSource>();
            oofSound.clip = oofClip;
        }
    }

    // Update is called once per frame
    void Update()
    {

        

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        // set motion to zero
        if(inputX != 0 || inputZ != 0)
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);

        if (inputZ != 0)
            move();
        
        if (inputX != 0)
            strafe();

        if (Input.GetAxis("Mouse X") != 0)
        {
            mouseXmove = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;
            rotate();
        }

        
        // GM.Output(transform.forward.ToString());
        
        if (inputZ != 0){
            // Debug.Log("inputZ: "+inputZ);
            // GM.Output("inputZ: "+inputZ, true);
        }
        
        // _rb.velocity += outsideForce;

        // HandleSlopes();
        // GM.Output("_groundSlope:: "+(Vector3.Angle (_groundSlope, Vector3.forward).ToString()), true);

        // Jump   

        // messageText.text = "grounded: " + isGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
        }

        // Prevent from spinning
        if(GetComponent<Rigidbody>().angularVelocity != Vector3.zero) GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // Debug.Log("AVEL "+GetComponent<Rigidbody>().angularVelocity);
    }

/*
    private void OnTriggerEnter(Collider other) {
        // 
        if (other.tag == "ClimbingSurface")
        {
            _climbing = true;
            _rb.useGravity = false;
        }
    }
    private void OnTriggerExit(Collider other) {
        // 
        if (other.tag == "ClimbingSurface")
        {
            _climbing = false;
            _rb.useGravity = true;
        }
    }
    */

    private bool FacingClimbingSurface( Collider coll ){

        Ray ray = new Ray(transform.position, transform.forward);
        return coll.Raycast(ray, out hit, 2.0f);
        // LayerMask mask = LayerMask.GetMask("ClimbingSurface");
        // return Physics.Raycast(transform.position, transform.forward, 2f, mask);
    }

    private void OnCollisionEnter(Collision collision){
        // Debug.Log("Player Collided with "+collision.collider.name);
        if ( !_climbing && collision.gameObject.tag == "ClimbingSurface" && FacingClimbingSurface(collision.collider))
        {
            _climbing = true;
            _startClimbHeight = transform.position.y;
            _rb.useGravity = false;
            Debug.Log("PlayerMovement Start Climbing");
        }

        // Fall / Collision Damage

        Vector3 myCollisionNormal = collision.contacts[0].normal;
        float otherMass = collision.rigidbody ? collision.rigidbody.mass : 1;
        float thisMass = GetComponent<Rigidbody>().mass;
        float collisionForce = Vector3.Dot(myCollisionNormal, collision.relativeVelocity); // * otherMass / thisMass;
        // float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        // Debug.Log("impact : "+impact);

        float forceThreshold = 14f;
        if ( collisionForce > forceThreshold )
        {
            Debug.Log("IMPACT TO PLAYER ==> "+collisionForce);
            // int damage = (int)Mathf.Floor( collisionForce / 10 );
            int damage = (int)Mathf.Floor( (collisionForce-forceThreshold) * 5 );
            gameObject.GetComponent<HPScript>().TakeDamage(damage);
            float maxVolumeDamage = 32f;
            float vol = 0.4f + 0.6f * ((float)damage / maxVolumeDamage);
            oofSound.volume = Mathf.Clamp( vol, 0, 1);
            Debug.Log("oofSound.volume :: "+damage+" / "+maxVolumeDamage+" = "+vol+" -- " +oofSound.volume);
            oofSound.Play();
        }
    

        // Debug.Log("-----------------COLLISION #contacts: "+collision.contacts.Length);
        // GetGroundSlope();
        // _groundSlope = GetGroundSlope();
        /*
        // Average all contact vectors
        bool isBelowMe = false;
        Vector3 averageNormal = Vector3.zero;
        // foreach (ContactPoint contact in collision.contacts)
        // {
        //     if(contact.point.y < transform.position.y){
        //         // Is below us
        //         averageNormal = (averageNormal + contact.normal).normalized;
        //         Debug.Log("----- "+contact.otherCollider.name);
        //         isBelowMe = true;
        //         Debug.DrawRay(contact.point,contact.normal*3, Color.red, 5f);
        //     } 
        // }
        if(isBelowMe){
            _groundNormal = averageNormal;
            
            _isOnSlope = Vector3.Angle (Vector3.up, _groundNormal) >= _slopeLimit;
            _groundFriction = collision.collider.material.staticFriction;
            Debug.Log("COLLIDED");
            Debug.Log("_groundFriction: "+_groundFriction);
            Debug.Log("_groundNormal: "+_groundNormal);

            Vector3 groundParallel = Vector3.Cross ( Vector3.up, _groundNormal);
            // Crossing the vector we made before with the initial normal gives us a vector that is parallel to the slope and always pointing down
            Vector3 slopeParallel = Vector3.Cross ( groundParallel, _groundNormal);
            Debug.Log("groundParallel: "+groundParallel);
            Debug.Log("slopeParallel: "+slopeParallel);
            Debug.DrawRay (transform.position, slopeParallel * 10, Color.green, 5f);
            // Debug.DrawRay(transform.position,_groundNormal*3, Color.red, 5f);
        }
        */
        
    }
    private void OnCollisionExit(Collision collision){

        if (collision.gameObject.tag == "ClimbingSurface")
        {
            _climbing = false;
            _rb.useGravity = true;
            Debug.Log("PlayerMovement Stop Climbing");
        }
    }

    private bool isGrounded()
    {
        Vector3 lowOrigin = transform.position + new Vector3(0,-1,0);
        // v For when we check four raycasts and average the slopes hit
        
        float halfWidth = 0.5f;
        Vector3 p1 = lowOrigin + new Vector3(-halfWidth, 0, -halfWidth);
        Vector3 p2 = lowOrigin + new Vector3(-halfWidth, 0, halfWidth);
        Vector3 p3 = lowOrigin + new Vector3(halfWidth, 0, -halfWidth);
        Vector3 p4 = lowOrigin + new Vector3(halfWidth, 0, halfWidth);
        List<Vector3> points = new List<Vector3>(new Vector3[]{p1,p2,p3,p4});

        foreach( Vector3 p in points ) {
            RaycastHit hit;
            if (Physics.Raycast(p, -Vector3.up, out hit))
            {
                float dist = hit.distance;
                if (dist < 2.5)
                {
                    // Debug.Log(hit);
                    // Debug.Log("grounded");
                    // Debug.DrawLine(transform.position, hit.point, Color.green);
                    // messageText.text = "hit: " + hit.distance;
                    return true;
                }
            }
        }
        return false;
    }


    private void rotate()
    {
        // messageText.text = "inputX: " + inputX;
        // transform.Rotate(new Vector3(0f, inputX * Time.deltaTime * rotateSpeed, 0f));
        // messageText.text = "mouseXmove: " + mouseXmove + "\nmouseYmove: " + Input.GetAxis("Mouse Y");
        // transform.Rotate(new Vector3(0f, mouseXmove, 0f));
        transform.Rotate(transform.up * mouseXmove);
    }

    private void strafe()
    {
        if(_isDriving)
            return;
        // Vector3 direction = Vector3.right * inputX;
        // _rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        // transform.position += transform.right * inputX * Time.deltaTime * moveSpeed;
        _rb.velocity += transform.right * inputX * moveSpeed;
    }

    private void move()
    {
        if(_isDriving)
            return;
        // Debug.Log("Player.move()");
        // transform.position += transform.forward * inputZ * Time.deltaTime * moveSpeed;
        // _rb.MovePosition(transform.position + transform.forward * inputZ * Time.fixedDeltaTime * moveSpeed);
        // if( isGrounded() )
        if( _climbing && !(inputZ < 0 && isGrounded())){
            if (Input.GetKeyDown(KeyCode.Space) && inputZ < 0){
                // Move backwards off the climbing surface
                _rb.velocity += transform.forward * moveSpeed * -1f;
            } else {
                // Climb up / down
                _rb.velocity = transform.up * inputZ * GetClimbSpeed();
                
            }
            
        } else {
            _rb.velocity += transform.forward * inputZ * moveSpeed;
            
            if ( isGrounded() ) {
                // oscillate walking
                // float camY = 0.9f + Mathf.PingPong(Time.time, _strideHeight);
                float frequency = 12f;
                float camY = 0.9f + _strideHeight + Mathf.Sin(Time.time * frequency) * _strideHeight;
                Camera.main.transform.localPosition = 
                new Vector3(
                    Camera.main.transform.localPosition.x, 
                    camY, 
                    Camera.main.transform.localPosition.z);
            }
            
        }
        
        // GM.Output(_rb.velocity.ToString(), true);
        // _rb.AddForce(transform.forward * inputZ * Time.fixedDeltaTime * moveSpeed);
        // _rb.AddRelativeForce(transform.forward * thrust);
        
    }

    private float GetClimbSpeed(){
        // Oscillate climbing speed

        float climbDist = transform.position.y - _startClimbHeight;
        float steps = Mathf.Floor( climbDist / _climbStepDistance );
        float currentStepDist = climbDist - steps * _climbStepDistance;
        float halfClimbStepDist = _climbStepDistance*0.5f;
        float halfStepPercent = 1f - Mathf.Abs( halfClimbStepDist - currentStepDist ) / halfClimbStepDist;
        float maxClimbSpeed = moveSpeed * _climbMaxSpeedPercent;
        float climbSpeed = maxClimbSpeed - halfStepPercent * maxClimbSpeed * 0.75f;

        return climbSpeed;

        // Debug.Log("PlayerMovement climgDist ==> "+climbDist);
        // Debug.Log("steps ==> "+steps);
        // Debug.Log("currentStepDist ==> "+currentStepDist);
        // Debug.Log("halfClimbStepDist ==> "+halfClimbStepDist);
        // Debug.Log("halfStepPercent ==> "+halfStepPercent);
        // Debug.Log("climbSpeed ==> "+climbSpeed);

    }

    public void StartDriving(){
        _isDriving = true;
    }
    public void StopDriving(){
        _isDriving = false;
    }

    // v Functions Callable from Host Page's Javascript v
    public void callRotate(float xSpeed)
    {
        // messageText.text = "xSpeed: " + xSpeed;
        transform.Rotate(new Vector3(0f, xSpeed * Time.deltaTime * rotateSpeed, 0f));
    }
    public void callRotateY(float yRotation)
    {
        transform.eulerAngles = new Vector3(0, yRotation, 0);
    }

    public void callRotateXYZ(string xyzString)
    {
        // expects string "x,y,z"

        // messageText.text = "xyzString: " + xyzString;
        string[] xyzArray = xyzString.Split(',');
        float Xrot = float.Parse(xyzArray[0]);
        float Yrot = float.Parse(xyzArray[1]);
        var Zrot = float.Parse(xyzArray[2]);

        // messageText.text = "X: " + Xrot + "\nY: " + Yrot + "\nZ: " + Zrot;

        transform.eulerAngles = new Vector3(Xrot, Yrot, Zrot);
    }

    private void HandleSlopes(){
        if (_groundSlope != Vector3.zero)
        {
    
            //Character sliding of surfaces        
            _rb.velocity += _groundSlope * moveSpeed * (1 - _groundFriction) ;
        }
       
    }
/*
    private Vector3 SlopeDirection(){
        // Returns normalized vector pointing downhill
        // !! This should be in a globally accessible script!

        // Thanks To:
        // https://forum.unity.com/threads/making-a-player-slide-down-a-slope.469988/

        // // Raycast with infinite distance to check the slope directly under the player no matter where they are
        // Physics.Raycast (this.transform.position, Vector3.down, out hit, Mathf.Infinity);
 
        // // Saving the normal
        // Vector3 n = hit.normal;
       
        // Crossing my normal with the player's up vector (if your player rotates I guess you can just use Vector3.up to create a vector parallel to the ground
        Vector3 groundParallel = Vector3.Cross ( Vector3.up, _groundNormal);
 
        // Crossing the vector we made before with the initial normal gives us a vector that is parallel to the slope and always pointing down
        Vector3 slopeParallel = Vector3.Cross ( groundParallel, _groundNormal);
        
 
        // // Just the current angle we're standing on
        // float currentSlope = Mathf.Round (Vector3.Angle (hit.normal, transform.up));
        // // Debug.Log (currentSlope);

        return slopeParallel.normalized;
 
        // // If the slope is on a slope too steep and the player is Grounded the player is pushed down the slope.
        // if (currentSlope >= 45f && MaintainingGround()) {
        //     isSliding = true;
        //     transform.position += slopeParallel.normalized / 2;
        // }
 
        // // If the player is standing on a slope that isn't too steep, is grounded, as is not sliding anymore we start a function to count time
        // else if (currentSlope < 45 && MaintainingGround() && isSliding) {
        //     TimePassed ();
               
        //     // If enough time has passed the sliding stops. There's no need for these last two if statements, the thing works already, but it's nicer to have the player slide for a little bit more once they get back on the ground
        //     if (currentSlope < 45 && MaintainingGround() && isSliding && timePassed > 1f) {
        //         isSliding = false;
        //     }
        // }
        // }
    }
    */
    private void GetGroundSlope()
    {
        // Debug.Log("GetGroundSlope()");
        Vector3 lowOrigin = transform.position + new Vector3(0,-1,0);
        // v For when we check four raycasts and average the slopes hit
        
        float halfWidth = 0.25f;
        Vector3 p1 = lowOrigin + new Vector3(-halfWidth, 0, -halfWidth);
        Vector3 p2 = lowOrigin + new Vector3(-halfWidth, 0, halfWidth);
        Vector3 p3 = lowOrigin + new Vector3(halfWidth, 0, -halfWidth);
        Vector3 p4 = lowOrigin + new Vector3(halfWidth, 0, halfWidth);
        List<Vector3> points = new List<Vector3>(new Vector3[]{p1,p2,p3,p4});
        List<float> frictions = new List<float>();
        List<Vector3> slopes = new List<Vector3>();

        foreach( Vector3 p in points ) {
            RaycastHit hit;
            if (Physics.Raycast(p, -Vector3.up, out hit))
            {
                // Debug.Log("Raycast Hit "+hit.collider.name);
                float dist = hit.distance;
                bool belowMe = hit.point.y < lowOrigin.y && dist < 3;
                // Debug.Log("hit.point.y ("+hit.point.y+") vs lowOrigin.y ("+lowOrigin.y+")");
                // Debug.Log("dist: "+dist);
                // Debug.DrawLine(hit.point,p, Color.yellow, 10f);
                if (belowMe)
                {
                    // Debug.Log("Below Me.");
                    Vector3 n = hit.normal;
                    bool steepEnough = Vector3.Angle (Vector3.up, n) >= _slopeLimit;
                    if(steepEnough){
                        Debug.Log("Steep Enough.");
                        Debug.Log("SLOPE:: "+Vector3.Angle (Vector3.up, n));
                        Vector3 groundParallel = Vector3.Cross ( Vector3.up, n);
                        Vector3 slopeParallel = Vector3.Cross ( groundParallel, n);
                        // Debug.Log("VECTOR# SP = " +slopeParallel);
                        slopes.Add(slopeParallel);
                        frictions.Add(hit.collider.material.staticFriction);
                        // Debug.DrawRay(hit.point,hit.normal*3, Color.red, 10f);
                        // Debug.DrawRay(hit.point,slopeParallel*4, Color.green, 10f);
                    }
                }
            }
        }
        
        
        if(slopes.Count > 0){
            _groundSlope = Vector3.zero;
            foreach (Vector3 s in slopes){
                // Debug.Log("VECTOR# S = " +s);
                _groundSlope += s;
            }
            _groundSlope = _groundSlope.normalized;

            // Debug.DrawRay(lowOrigin,_groundSlope*4, Color.blue, 10f);

            _groundFriction = 0;
            foreach (float f in frictions){
                _groundFriction += f;
            }
            _groundFriction /= frictions.Count;
        } else {
            _groundFriction = 0.6f;
            _groundSlope = Vector3.zero;
        }
    }
}
