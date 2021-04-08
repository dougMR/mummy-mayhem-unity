using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System.Linq;  // to concat lists

public class EnemyAIScript : MonoBehaviour
{
    public AudioClip hitMeClip;
    public AudioClip attackGrowl;
    public AudioClip attackClip;
    public Canvas floatingTextCanvasPrefab;
    
    private Animator _myAnimator;
    private AudioSource _hitMeSound;
    // public bool chasingPlayer = false;
    public float _maxHP = 10f;
    private float _HP = 10f;
    private float _moveForceWander = 0.5f;
    private float _moveForceChase = 1.2f;
    private float _moveForce = 0.5f;
    private GameObject _chaseTarget = null;
    private float _chaseDist = 20f;
    private int _attackPoints = 1;
    private Rigidbody _rb;
    private Vector3 _moveDir;
    private GameObject _player;
    // public LayerMask whatIsWall;
    // public float maxDistFromWall = 0f;
    private float _myMoveDuration = 7.0f;
    private float _chasePlayerDuration = 2.0f;
    private float _changeDirectionTimer;
    private float _checkObstaclesTimer;
    private float _checkObstaclesFrequency = 2.0F;
    private float _lastCollidedPlayerTime = 0f;
    
    private AudioSource _attackSound;
    private AudioSource _attackGrowl;
    private float _bornTime;
    private Vector3 _startingPosition;
    // private UnityEngine.AI.NavMeshAgent _agent;
    private IEnumerator _turnCoroutine;
    private Vector3 _lastVelocity;
    // private Text _floatingText;
    // private Canvas _floatingCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        if(_myAnimator != null) {
            // Play Walk Animation
            _myAnimator.SetTrigger("Walk");
            AnimatorStateInfo state = _myAnimator.GetCurrentAnimatorStateInfo(0);
            // _myAnimator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            _myAnimator.Play("Base Layer.Walking", -1, UnityEngine.Random.Range(0f, 1f));
            SetAnimationSpeed();
        }
        if(transform.name.Contains("Green") ){
            _moveForceWander = 1f;
            _moveForceChase = 2.5f;
            _attackPoints = 3;
        }
        _moveForce = _moveForceWander;
 
        _player = GameObject.Find("Player");
        _changeDirectionTimer = _myMoveDuration + UnityEngine.Random.Range(0, _myMoveDuration);
        _checkObstaclesTimer = _checkObstaclesFrequency;
        _rb = GetComponent<Rigidbody>();
        _moveDir = transform.forward;
        Turn(_moveDir,true);
        
        if(attackClip != null) {
            _attackSound = gameObject.AddComponent<AudioSource>();
            _attackSound.clip = attackClip;
            _attackSound.volume = 0.7f;
        }
        if(hitMeClip != null ) {
            _hitMeSound = gameObject.AddComponent<AudioSource>();
            _hitMeSound.clip = hitMeClip;
        }
        if(attackGrowl != null ) {
            _attackGrowl = gameObject.AddComponent<AudioSource>();
            _attackGrowl.clip = attackGrowl;
            _attackGrowl.volume = 0.7f;
        }

        _bornTime = Time.time;
        _startingPosition = transform.position;

        // Transform textTransform = transform.Find("FloatingTextCanvas/FloatingText");
        // if (textTransform != null){
        //     _floatingText = textTransform.GetComponent<Text>();
        //     _floatingCanvas = transform.Find("FloatingTextCanvas").GetComponent<Canvas>();
        //     _floatingCanvas.enabled = false;
        //     // InitFloatingText(_HP.ToString());
        // }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 constantVelocity = transform.forward * _moveForce;
        _rb.velocity = new Vector3(constantVelocity.x, _rb.velocity.y, constantVelocity.z);

        _changeDirectionTimer -= Time.deltaTime;
        _checkObstaclesTimer -= Time.deltaTime;
        if (_changeDirectionTimer <= 0)
        {
            StopChase();
            // Close enough to Player to chase Player?
            float playerDist = Vector3.Distance(_player.transform.position, transform.position);
            if( playerDist < _chaseDist){
                ChasePlayer();
            } else {
                CheckChaseChain();
            }
            if(HasTarget()){
                Debug.DrawLine(transform.position, _chaseTarget.transform.position, Color.green, 2);
                FaceTarget();
            }else{
                ChangeDirection();
            }
            _changeDirectionTimer = HasTarget() ? _chasePlayerDuration : _myMoveDuration;
            SetAnimationSpeed();
        } else if (_checkObstaclesTimer <= 0)
        {
            if(CheckObstacles()){
                ChangeDirection(true);
            }
            _checkObstaclesTimer = _checkObstaclesFrequency;
        }
        /*
        if (Physics.Raycast(transform.position, transform.forward, maxDistFromWall, whatIsWall))
        {
            _moveDir = ChangeDirection();
            transform.rotation = Quaternion.LookRotation(_moveDir);
        }
        */
        // FaceFloatingText();
    }

    void FixedUpdate()
    {
        _moveDir = transform.forward; // <-- in case facing changed by outside forces
        // Vector3 velocityDiff = _rb.velocity - _lastVelocity;
        // if(velocityDiff.magnitude > 2){

        // }
        // _lastVelocity = _rb.velocity;
    }

    void SetAnimationSpeed()
    {
        if(_myAnimator == null){
            return;
        }
        // For now, it just happens that _moveForceChase propels zombie at speed that syncs with Walk animation played full speed
        float animationSpeed = _moveForce / _moveForceChase;
        _myAnimator.speed = animationSpeed;
    }

    public void ApplyDamage(int damage) {
        // Debug.Log("ApplyDamage("+damage+")");
        if (_HP <= 0 ) {
            // Debug.Log("EnemyAIScript.ApplyDamage() _HP == > "+_HP);
            return;
        }
        if (floatingTextCanvasPrefab != null)
                ShowFloatingText(Mathf.Clamp(damage,0,_HP).ToString());
        _HP -= damage;
        
        // GameObject[] children = new GameObject[0];
        if (_HP <= 0){
            Die();
        } else {
            float distToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            float maxDist = 30f;
            float pct = (maxDist - distToPlayer) / maxDist;
            float volume = Mathf.Clamp(pct, 0f, 0.65f);
            _hitMeSound.volume = volume;
            _hitMeSound.Play();
            // Display floating text animation
        }
        
        // return children;
    }

    public void Die(){
        Debug.Log("EnemyAI.Die()");
        GameObject swappedMummy = gameObject.GetComponent<SwapMummyScript>().Swap();
        Debug.Log("EnemyAI.Die swappedMummy ==> "+swappedMummy.name);
        Rigidbody[] RBs = swappedMummy.GetComponentsInChildren<Rigidbody>();
        // List<GameObject> listOfGOs = new List<GameObject>();
        foreach ( Rigidbody rb in RBs ){
            // listOfGOs.Add(rb.gameObject);

            if(rb.name.Contains("Torso") && UnityEngine.Random.Range(0, 4) == 0){
                // Break apart Torso
                rb.GetComponent<SubdivideObjectScript>().SubdivideMe();

                // List<GameObject> pieces = rb.GetComponent<SubdivideObjectScript>().SubdivideMe();              
                // if (pieces.Count > 0){
                //     listOfGOs.Concat(pieces).ToList();
                // }
            }
        }
        GameManager.Instance.Score += (int)_maxHP;
        
        // return listOfGOs.ToArray();
        // GameObject[] arrayOfGameObjects = listOfGameObjects.ToArray();
    }

    private void OnCollisionEnter (Collision collision) {

        
        // Vector3.Dot(Vector3 lhs, Vector3 rhs);
        
        // Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        // collisionForce.y *= 0.25f; // Allow to sustain greater y-impact

        float otherMass = collision.rigidbody ? collision.rigidbody.mass : 1;

        /*
        If you just want a measurement of how strong the hit was (like, for example for damage calculations), the dot product of collision normal and collision velocity (ie the velocity of the two bodies relative to each other), times the mass of the other collider should give you useful values.
         -- https://forum.unity.com/threads/getting-impact-force-not-just-velocity.23746/
        */

        Vector3 myCollisionNormal = collision.contacts[0].normal;

        float impact = Vector3.Dot(myCollisionNormal, collision.relativeVelocity);// * otherMass;

        /*
        float massDiff = otherMass / GetComponent<Rigidbody>().mass;
        float impact = collisionForce.magnitude * massDiff;
        if(otherMass >= 100 ){
            Debug.Log("EnemyAI collision.impulse: "+collision.impulse);
            Debug.Log("EnemyAI collision otherMass ==> "+otherMass);
            Debug.Log("EnemyAI collision massDiff ==> "+massDiff);
            Debug.Log("EnemyAI collision collisionForce.magnitude ==> "+collisionForce.magnitude);
            Debug.Log("EnemyAI collision Impact ==> "+impact);
        }
        */
        
        // if (collisionForce.magnitude > 200.0F ) {
            
        if (impact > 5){
            // Debug.Log("Enemy Collision, Impact :: "+impact);
            // int damage = Mathf.RoundToInt( collisionForce.magnitude / 25 );

            int damage = Mathf.RoundToInt( impact / 2 );

            // Debug.Log("EnemyAI.CollisionDamage("+damage+")");
            ApplyDamage(damage);
            /*
            GameObject[] children = gameObject.GetComponent<SwapMummyScript>().Swap();
            // Debug.Log("children.Length: "+children.Length);
            foreach (GameObject child in children)
            {
                if (child != null){               
                    if(child.name.Contains("Legs")){
                        // Hack to reduce bounciness
                        child.GetComponent<Rigidbody>().AddForce( collisionForce );
                    } else if(child.name == "Torso" && Random.Range(0, 4) == 0){
                        Debug.Log("collision - Torso");
                        GameObject[] pieces = child.GetComponent<SubdivideObjectScript>().SubdivideMe();
                        // Debug.Log("pieces.Length: "+pieces.Length);
                        for ( int p = 0; p < pieces.Length; p++){
                            
                            GameObject piece = pieces[p];
                            // Debug.Log("piece["+p+"]: "+piece);
                            piece.GetComponent<Rigidbody>().AddForce( collisionForce );
                        }
                    }
                }
            }
            */
    
        } else {
            // Bumped into Something, Change Direction?
            if(CheckObstacles()){
                ChangeDirection(true);
            }
        }
        /*else if (collisionForce.magnitude > 10.0F ){
            // Bumped into Something, Change Direction
            Debug.Log("COLLISION FORCE ON MUMMY ("+gameObject.name+") = "+collisionForce);
            // Debug.Log("renderer.enabled: "+transform.Find("Head").GetComponent<Renderer>().enabled);
            // Destroy(gameObject);
            // transform.Find("Head").GetComponent<Renderer>().material.color = Color.blue;
            if ( _chaseTarget != null ) {
                ChangeDirection(true);
            }
            
        }*/
    }

    void OnCollisionStay(Collision collision)
    {
        float attackDelta = Time.time - _lastCollidedPlayerTime;
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        
        if (transform.name.Contains("Mummy") && collision.gameObject == _player && attackDelta >= 1.6)
        {
            Vector3 targetDir = _player.transform.position - transform.position;
            float attackAngle = Mathf.Abs(Vector3.Angle(targetDir, transform.forward));
            if(attackAngle < 45){
                // Attack Player
                Attack(collision.gameObject);
                
                ChasePlayer();
                _lastCollidedPlayerTime = Time.time;
            }
        }
    }

    private void Attack(GameObject target)
    {
        // Attack
        GameManager.Instance.DelayFunction( () =>
        {
            if(_attackSound != null)
                _attackSound.Play();
            target.GetComponent<HPScript>().TakeDamage(_attackPoints);

        }, 0.25f);
        
        if(_attackGrowl != null)
            _attackGrowl.Play();
        if(_myAnimator != null)
            _myAnimator.SetTrigger("Attack"); 

        _chaseTarget = target;
        FaceTarget();
    }

    private void OnCollisionExit(Collision other) {
        if(_myAnimator != null)
            _myAnimator.SetTrigger("Walk");
    }

    void FaceTarget(){
        if(Time.time - _bornTime < 9){
            return;
        }
        if(!HasTarget()){
            return;
        }
        // Find the vector pointing from our position to the target
        Vector3 _direction = (_chaseTarget.transform.position - transform.position).normalized;
        // Face that direction
        _moveDir = _direction;
        Turn(_moveDir);
        // transform.rotation = Quaternion.LookRotation( new Vector3(_moveDir.x, 0, _moveDir.z), Vector3.up );
    }

    public void FaceOther( GameObject other ){
        if(Time.time - _bornTime < 9){
            return;
        }
        StopChase();
        _moveForce = _moveForceChase;
        // Find the vector pointing from our position to the target
        Vector3 _direction = (other.transform.position - transform.position).normalized;
        // Face that direction
        _moveDir = _direction;
        Turn(_moveDir);
    }

    void ChangeDirection( bool forceChange = false )
    {
        // Turn to new random direction
        _moveForce = _moveForceWander;
        Vector3 newDir = new Vector3();
        // if( aboutFace){
        //     newDir = transform.forward;
        // } else {
            int startRange = forceChange ? 1 : 0;
            int i = UnityEngine.Random.Range(startRange, 3);
            
            if (i == 0)
            {
                newDir = transform.forward;
            }
            else if (i == 1)
            {
                newDir = transform.right;
            }
            else if (i == 2)
            {
                newDir = -transform.right;
            }
        // }
        
        _moveDir = newDir;
        Turn(_moveDir);
        //transform.rotation = Quaternion.LookRotation( new Vector3(_moveDir.x, 0, _moveDir.z), Vector3.up );
    }
    public void ChasePlayer(){
        _moveForce = _moveForceChase;
        _chaseTarget = _player;
        FaceTarget();
    }
    public void ChaseOther(GameObject other){
        _chaseTarget = other;
        // Debug.Log("ChaseOther:: "+other);
        FaceOther(other);
    }
    public void StopChasePlayer(){
        _chaseTarget = null;
        _moveForce = _moveForceWander;
    }
    public void StopChase(){
        _moveForce = _moveForceWander;
        _chaseTarget = null;
    }
    public bool HasTarget(){
        return _chaseTarget != null;
    }
    public GameObject GetTarget(){
        return _chaseTarget;
    }
    public float TargetDistance(){
        if(HasTarget()){
            return Vector3.Distance(_chaseTarget.transform.position, transform.position);
        }
        return Mathf.Infinity;
    }
    void CheckChaseChain(){
        // Close enough to chase another Mummy who is chasing Player?
        // See if I can follow another enemy that leads to player...
        int minSteps = int.MaxValue;
        Collider[] colliders = Physics.OverlapSphere(transform.position, _chaseDist);
        for (int i = 0; i < colliders.Length; i++) 
        {
            GameObject nextGO = colliders[i].gameObject;
            if(nextGO == _player){
                ChasePlayer();
                break;
            } else if(nextGO != gameObject && nextGO.name.Contains("Mummy") && !nextGO.name.Contains("Separated") && nextGO.GetComponent<EnemyAIScript>().HasTarget()){

                int steps = nextGO.GetComponent<EnemyAIScript>().StepsToPlayer();
                if ( steps < minSteps ){
                    // Make that our target
                    ChaseOther(nextGO);
                    minSteps = steps;
                }
            }
        }
    }
    public int StepsToPlayer(){
        // Returns # of links in the chaseTarget chain leading to Player
        if(!HasTarget()){
            return int.MaxValue;
        }else{
            if( _chaseTarget == _player ){
                return 0;
            }
            int links = 0;
            GameObject nextTarget = _chaseTarget;
            GameObject go = GameObject.Find("wibble");
            if (!nextTarget) {
                // Debug.Log("No game object called _chaseTarget: "+_chaseTarget);
                _chaseTarget = null;
                return int.MaxValue;
            }
            while(nextTarget != null && nextTarget.GetComponent<EnemyAIScript>().HasTarget()){
                // Debug.Log("links: "+links);
                Debug.Log("in loop, prev nextTarget: "+nextTarget);
                nextTarget = nextTarget.GetComponent<EnemyAIScript>().GetTarget();
                // Debug.Log("in loop, next nextTarget: "+nextTarget);
                links ++;
                if(nextTarget == _player){
                    // Debug.Log("Found Player in ["+links+"] links");
                    return links;
                }else if(nextTarget == null){
                    StopChase();
                    return int.MaxValue;
                }
            }
            // If we get to here, we never found Player
            StopChase();
            return int.MaxValue;
        }
    }
    private bool CheckObstacles()
    {
        RaycastHit hit;
        Vector3 lowOrigin = new Vector3(transform.position.x,transform.position.y-1,transform.position.z);
        if (Physics.Raycast(lowOrigin, transform.forward, out hit))
        {
           
            if (hit.rigidbody != null && !(hit.rigidbody.constraints == RigidbodyConstraints.FreezePositionX | hit.rigidbody.constraints == RigidbodyConstraints.FreezePositionZ)) {
                // Not locked on X or Z movement
                //  Debug.Log("Constraints: "+ hit.rigidbody.constraints);
                // Object can be moved
                return false;
            }
            // Debug.DrawRay(lowOrigin, transform.forward * dist, Color.yellow, 3.0f);
            float dist = hit.distance;
            if (dist < 1.5)
            {

                return true;
            }
            else
            {

                return false;
            }
        }
    
        return false;
    }
    void Turn(Vector3 targetRot, bool immediately = false)
    {
        // Debug.Log("Turn()");
        // Debug.Log("V3 target: " + targetRot);
        // Debug.Log("Euler target: " + Quaternion.Euler(targetRot));
        if (_turnCoroutine != null)
            StopCoroutine(_turnCoroutine);
        if (immediately)
        {
            // transform.rotation = Quaternion.Euler(targetRot);
            transform.rotation = Quaternion.LookRotation(new Vector3(targetRot.x, 0, targetRot.z), Vector3.up);

        }
        else
        {
            _turnCoroutine = TurnCoroutine(targetRot);
            StartCoroutine(_turnCoroutine);
        }
    }

    IEnumerator TurnCoroutine(Vector3 targetRot)
    {
        
            // Debug.Log("TurnCoroutine: " + transform.rotation);
            // Debug.Log("TargetRot: " + Quaternion.Euler(targetRot));
            // Debug.Log("yROt: " + transform.rotation.y);

            float duration = 1f;
            float counter = 0;
            Vector3 startRot = transform.rotation.eulerAngles;
            float startYrot = startRot.y;
            float targetYrot = Quaternion.LookRotation(targetRot, Vector3.up).eulerAngles.y;

            // Constrain -180 to 180
            float a = targetYrot - startYrot;
            // a = (a + 180) % 360 - 180;
            a = Mod((a+180),360) - 180;
            targetYrot = startYrot + a;

            float currentYrot = startYrot;
            // if(Mathf.Abs(targetYrot - startYrot) > 180f ){

            //     Debug.Log("startYrot:  " + startYrot);
            //     Debug.Log("targetYrot:  " + targetYrot);
            //     Debug.Log(" DIFF:  "+(targetYrot - startYrot));
            // }
           

            while (counter < duration)
            {
                counter += Time.deltaTime;
                currentYrot = Mathf.Lerp(startYrot, targetYrot, counter / duration);

                // if(Mathf.Abs(targetYrot - startYrot) > 180 ){
                    // Debug.Log(" -- counter / duration:: " + (counter / duration));
                    // Debug.Log("currentYrot:: " + currentYrot);
                // }

                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentYrot, transform.eulerAngles.z);
                
                
                Debug.DrawRay(transform.position, targetRot * 5f, Color.yellow, 0.2f);
                Debug.DrawRay(transform.position, transform.forward * 5f, Color.red, 0.1f);

                yield return null;
            }

            // Debug.Log(" ---------- End TurnCoroutine, rotation: " + transform.rotation);
            // Debug.Log("yROt: " + transform.rotation.y);
            // Debug.Log("targetYrot: " + targetYrot);

        
    }
    private float Mod (float a, float n) {
        // Modulus that accepts negatives, used by Turn CoRoutine
        // Learn to create and  call static class functions!
        return a - Mathf.Floor((a/n)) * n;
    }
    void ShowFloatingText(string text){
        
        Canvas floatingCanvas = Instantiate(floatingTextCanvasPrefab, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), Quaternion.identity);
        floatingCanvas.enabled = true;
        
        // Face Camera
        Vector3 v = Camera.main.transform.position - floatingCanvas.transform.position;
        v.x = v.z = 0.0f;
        floatingCanvas.transform.LookAt( Camera.main.transform.position - v );
        floatingCanvas.transform.rotation =(Camera.main.transform.rotation); // Take care about camera rotation

        Text floatingText = floatingCanvas.transform.Find("FloatingText").GetComponent<Text>();
        floatingText.text = text;
        // RectTransform floatRect = _floatingText.GetComponent<RectTransform>();
        // floatRect.transform.localPosition = _floatingText.transform.localPosition;
        // floatRect.transform.localScale = _floatingText.transform.localScale;
        // floatRect.transform.localRotation = _floatingText.transform.localRotation;

        floatingText.GetComponent<Animator>().SetTrigger("FloatText");
        Destroy(floatingCanvas.gameObject, 2);
    }
    // void FaceFloatingText(){
    //     if(_floatingText != null && _floatingCanvas.enabled){
    //         Transform floatCanvas = _floatingText.transform.parent;
    //         Vector3 v = Camera.main.transform.position - floatCanvas.transform.position;
    //         v.x = v.z = 0.0f;
    //         floatCanvas.transform.LookAt( Camera.main.transform.position - v );
    //         floatCanvas.transform.rotation =(Camera.main.transform.rotation); // Take care about camera rotation
    //     }
    // }
}
