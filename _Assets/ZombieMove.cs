using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System.Linq;  // to concat lists

public class ZombieMove : MonoBehaviour
{
    // public AudioClip hitMeClip;
    // public Canvas floatingTextCanvasPrefab;
    // private AudioSource _hitMeSound;
    public float _maxHP = 10f;
    private Animator _myAnimator;
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
    private float _myMoveDuration = 7.0f;
    private float _chasePlayerDuration = 2.0f;
    private float _changeDirectionTimer;
    private float _checkObstaclesTimer;
    private float _checkObstaclesFrequency = 2.0F;
    private float _lastCollidedPlayerTime = 0f;
    private float _bornTime;
    private Vector3 _startingPosition;
    private UnityEngine.AI.NavMeshAgent agent;
    private IEnumerator _turnCoroutine;
    private Vector3 _lastVelocity;


    // Start is called before the first frame update
    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _moveForce = _moveForceWander;

        _player = GameObject.Find("Player");
        _changeDirectionTimer = _myMoveDuration + Random.Range(0, _myMoveDuration);
        _checkObstaclesTimer = _checkObstaclesFrequency;
        _rb = GetComponent<Rigidbody>();
        _moveDir = transform.forward;
        Turn(_moveDir, true);

        _bornTime = Time.time;
        _startingPosition = transform.position;

        // Play Walk Animation
        _myAnimator.SetBool("Moving", true);
        AnimatorStateInfo state = _myAnimator.GetCurrentAnimatorStateInfo(0);
        // _myAnimator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
        _myAnimator.Play("Base Layer.Walking", -1, Random.Range(0f, 1f));
        SetAnimationSpeed();
    }

    void SetAnimationSpeed()
    {
        float animationSpeed = _moveForce / _moveForceChase;
        _myAnimator.speed = animationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 constantVelocity = transform.forward * _moveForce;
        // Let Y velocity be determined by rigidbody physics
        _rb.velocity = new Vector3(constantVelocity.x, _rb.velocity.y, constantVelocity.z);

        _changeDirectionTimer -= Time.deltaTime;
        _checkObstaclesTimer -= Time.deltaTime;
        if (_changeDirectionTimer <= 0)
        {
            StopChase();
            // Close enough to Player to chase Player?
            float playerDist = Vector3.Distance(_player.transform.position, transform.position);
            if (playerDist < _chaseDist)
            {
                ChasePlayer();
            }
            else
            {
                // Close enough to chase another zombie who is chasing Player?
                // See if I can follow another enemy that leads to player...
                int minSteps = int.MaxValue;
                Collider[] colliders = Physics.OverlapSphere(transform.position, _chaseDist);
                for (int i = 0; i < colliders.Length; i++)
                {
                    GameObject nextGO = colliders[i].gameObject;
                    if (nextGO == _player)
                    {
                        ChasePlayer();
                        break;
                    }
                    else if (nextGO != gameObject && nextGO.name.Contains("zombie") && nextGO.GetComponent<ZombieMove>().HasTarget())
                    {
                        int steps = nextGO.GetComponent<ZombieMove>().StepsToPlayer();
                        if (steps < minSteps)
                        {
                            // Make that our target
                            ChaseOther(nextGO);
                            minSteps = steps;
                        }
                    }
                }
            }
            if (HasTarget())
            {
                Debug.DrawLine(transform.position, _chaseTarget.transform.position, Color.green, 2);
                FaceTarget();
            }
            else
            {
                ChangeDirection();
            }
            _changeDirectionTimer = HasTarget() ? _chasePlayerDuration : _myMoveDuration;
            SetAnimationSpeed();
        }
        else if (_checkObstaclesTimer <= 0)
        {
            if (CheckObstacles())
            {
                ChangeDirection(true);
            }
            _checkObstaclesTimer = _checkObstaclesFrequency;
        }
    }

    void FixedUpdate()
    {
        _moveDir = transform.forward; // <-- in case facing changed by outside forces
    }

    public void ApplyDamage(int damage)
    {
        // Debug.Log("ApplyDamage("+damage+")");
        if (_HP <= 0)
        {
            // Debug.Log("ZombieMove.ApplyDamage() _HP == > "+_HP);
            return;
        }
        // if (floatingTextCanvasPrefab != null)
        //     ShowFloatingText(Mathf.Clamp(damage, 0, _HP).ToString());
        _HP -= damage;

        if (_HP <= 0)
        {
            Die();
        }
        else
        {
            // float distToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            // float maxDist = 30f;
            // float pct = (maxDist - distToPlayer) / maxDist;
            // float volume = Mathf.Clamp(pct, 0f, 0.65f);
            // _hitMeSound.volume = volume;
            // _hitMeSound.Play();
        }
    }

    public void Die()
    {
        // Debug.Log("EnemyAI.Die()");
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        float otherMass = collision.rigidbody ? collision.rigidbody.mass : 1;

        /*
        If you just want a measurement of how strong the hit was (like, for example for damage calculations), the dot product of collision normal and collision velocity (ie the velocity of the two bodies relative to each other), times the mass of the other collider should give you useful values.
         -- https://forum.unity.com/threads/getting-impact-force-not-just-velocity.23746/
        */

        Vector3 myCollisionNormal = collision.contacts[0].normal;

        float impact = Vector3.Dot(myCollisionNormal, collision.relativeVelocity);// * otherMass;

        if (impact > 5)
        {
            Debug.Log("Enemy Collision, Impact :: " + impact);

            int damage = Mathf.RoundToInt(impact / 2);

            // ApplyDamage(damage);
        }
        else
        {
            // Bumped into Something, Change Direction?
            if (CheckObstacles())
            {
                ChangeDirection(true);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        float attackDelta = Time.time - _lastCollidedPlayerTime;
        //Check for a match with the specified name on any GameObject that collides with your GameObject

        if (transform.name.Contains("zombie") && collision.gameObject == _player && attackDelta >= 1)
        {
            Vector3 targetDir = _player.transform.position - transform.position;
            float attackAngle = Mathf.Abs(Vector3.Angle(targetDir, transform.forward));
            if (attackAngle < 45)
            {
                // Attack Player
                // Debug.Log("zombie COLLIDED PLAYER");
                ChasePlayer();
                _lastCollidedPlayerTime = Time.time;
                // collision.gameObject.GetComponent<HPScript>().TakeDamage(_attackPoints);
            }
        }
    }

    void FaceTarget()
    {
        if (Time.time - _bornTime < 9)
        {
            return;
        }
        if (!HasTarget())
        {
            return;
        }
        // Find the vector pointing from our position to the target
        Vector3 _direction = (_chaseTarget.transform.position - transform.position).normalized;
        // Face that direction
        _moveDir = _direction;
        Turn(_moveDir);
    }

    public void FaceOther(GameObject other)
    {
        if (Time.time - _bornTime < 9)
        {
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

    void ChangeDirection(bool forceChange = false)
    {
        // Turn to new random direction
        _moveForce = _moveForceWander;
        Vector3 newDir = new Vector3();
        // if( aboutFace){
        //     newDir = transform.forward;
        // } else {
        int startRange = forceChange ? 1 : 0;
        int i = Random.Range(startRange, 3);

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
    public void ChasePlayer()
    {
        _moveForce = _moveForceChase;
        _chaseTarget = _player;
        FaceTarget();
    }
    public void ChaseOther(GameObject other)
    {
        _chaseTarget = other;
        // Debug.Log("ChaseOther:: "+other);
        FaceOther(other);
    }
    public void StopChase()
    {
        _moveForce = _moveForceWander;
        _chaseTarget = null;
    }
    public bool HasTarget()
    {
        return _chaseTarget != null;
    }
    public GameObject GetTarget()
    {
        return _chaseTarget;
    }
    public float TargetDistance()
    {
        if (HasTarget())
        {
            return Vector3.Distance(_chaseTarget.transform.position, transform.position);
        }
        return Mathf.Infinity;
    }
    public int StepsToPlayer()
    {
        // Returns # of links in the chaseTarget chain leading to Player
        if (!HasTarget())
        {
            return int.MaxValue;
        }
        else
        {
            if (_chaseTarget == _player)
            {
                return 0;
            }
            int links = 0;
            GameObject nextTarget = _chaseTarget;
            GameObject go = GameObject.Find("wibble");
            if (!nextTarget)
            {
                // Debug.Log("No game object called _chaseTarget: "+_chaseTarget);
                _chaseTarget = null;
                return int.MaxValue;
            }
            while (nextTarget != null && nextTarget.GetComponent<ZombieMove>().HasTarget())
            {
                // Debug.Log("links: "+links);
                Debug.Log("in loop, prev nextTarget: " + nextTarget);
                nextTarget = nextTarget.GetComponent<ZombieMove>().GetTarget();
                // Debug.Log("in loop, next nextTarget: "+nextTarget);
                links++;
                if (nextTarget == _player)
                {
                    // Debug.Log("Found Player in ["+links+"] links");
                    return links;
                }
                else if (nextTarget == null)
                {
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
        Vector3 lowOrigin = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        if (Physics.Raycast(lowOrigin, transform.forward, out hit))
        {

            if (hit.rigidbody != null && !(hit.rigidbody.constraints == RigidbodyConstraints.FreezePositionX | hit.rigidbody.constraints == RigidbodyConstraints.FreezePositionZ))
            {
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
        a = Mod((a + 180), 360) - 180;
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
    private float Mod(float a, float n)
    {
        // Modulus that accepts negatives, used by Turn CoRoutine
        // Learn to create and  call static class functions!
        return a - Mathf.Floor((a / n)) * n;
    }
    // void ShowFloatingText(string text)
    // {

    //     Canvas floatingCanvas = Instantiate(floatingTextCanvasPrefab, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), Quaternion.identity);
    //     floatingCanvas.enabled = true;

    //     // Face Camera
    //     Vector3 v = Camera.main.transform.position - floatingCanvas.transform.position;
    //     v.x = v.z = 0.0f;
    //     floatingCanvas.transform.LookAt(Camera.main.transform.position - v);
    //     floatingCanvas.transform.rotation = (Camera.main.transform.rotation); // Take care about camera rotation

    //     Text floatingText = floatingCanvas.transform.Find("FloatingText").GetComponent<Text>();
    //     floatingText.text = text;
    //     // RectTransform floatRect = _floatingText.GetComponent<RectTransform>();
    //     // floatRect.transform.localPosition = _floatingText.transform.localPosition;
    //     // floatRect.transform.localScale = _floatingText.transform.localScale;
    //     // floatRect.transform.localRotation = _floatingText.transform.localRotation;

    //     floatingText.GetComponent<Animator>().SetTrigger("FloatText");
    //     Destroy(floatingCanvas.gameObject, 2);
    // }
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
