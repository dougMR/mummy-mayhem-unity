using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodableScript : MonoBehaviour, IDamageable
{
    public int myHP = 50;
    public GameObject explosionGO; // drag your explosion prefab here
    public GameObject preExplosionGO;
    public float delay = 0.5f;
    public float radius = 8.0f;
    public float explodePower = 200f;
    private int _hp;
    private bool _exploded = false;
    private float _lastVelocity = 0;
    private float _maxSafeSpeedChange = 5f;

    private void Start()
    {
        Init();

    }

    private void Init()
    {
        HP = myHP;
    }

    private void FixedUpdate()
    {
        // Monitor changes in velocity
        float velocity = GetComponent<Rigidbody>().velocity.magnitude;
        float velDiff = Mathf.Abs(velocity - _lastVelocity);
        if (velDiff > _maxSafeSpeedChange)
        {
            Debug.Log("Explodable velocity change: " + velDiff);
            float overage = velDiff - _maxSafeSpeedChange;
            int damage = (int)overage;
            TakeDamage(damage);
        }
        _lastVelocity = velocity;
    }
    /*
    private void OnCollisionEnter(Collision other)
    {
        // float impulse = other.impulse.magnitude;
        // float force = impulse / Time.fixedDeltaTime;
        Vector3 norm = other.GetContact(0).normal;
        float mass = other.rigidbody != null ? other.rigidbody.mass : 1;
        float impact = Vector3.Dot(norm, other.relativeVelocity) * mass;

        if (impact > 5)
        {

            int damage = Mathf.RoundToInt(impact / 2);

            Debug.Log("     ------------------");
            Debug.Log("#Contacts :: " + other.contactCount);
            Debug.Log("Explodable Dot :: " + impact);
            // Debug.Log("Explodable impulse :: " + impulse);
            // Debug.Log("Explodable force :: " + force);
            Debug.Log("Explodable relativeVelocity :: " + other.relativeVelocity.magnitude);
            Debug.Log("Explodable. damage :: " + damage);
            HP -= damage;
        }

    }
    */

    private void Detonate()
    {
        if (_exploded) return;
        // Show initial explosion animation
        GameObject expl = (GameObject)Instantiate(preExplosionGO, transform.position, Quaternion.identity);
        // Swell the barrel
        transform.localScale = transform.localScale * 1.1f;
        GameManager.Instance.DelayFunction(ExplodeMe, delay);
        _exploded = true;
    }
    private void ExplodeMe()
    {

        Debug.Log("Explodable.ExplodeMe(" + gameObject.name + ")");
        // Debug.Log("Explodeable._exploded :: " + _exploded);



        // call GameManager Explosion
        // Debug.Log("Explodable transform.position : " + transform.position);
        // Debug.Log("Explodable radius, power, go :: " + radius + ", " + explodePower + ", " + explosionGO.name);


        // destroy me
        Destroy(gameObject);


        GameManager.Instance.Explode(transform.position, radius, explodePower, explosionGO);

    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            // Debug.Log("Explodable.HP :: " + _hp);
            if (_hp <= 0)
            {
                Detonate();
            }
        }
    }


}
