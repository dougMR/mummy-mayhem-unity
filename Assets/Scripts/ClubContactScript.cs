using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubContactScript : MonoBehaviour
{
    // public GameObject CollisionSpherePrefab;
    public GameObject explosionGO; // drag your explosion prefab here
    public AudioClip thumpClip;
    private AudioSource thumpSound;
    private GameObject _player;
    private float _clubForce = 64.0f;
    private float _radius = 0.25f;
    private bool _newSwing = true;
    // private int _clubDamage = 1;
    // Start is called before the first frame update
    void Start()
    {
        thumpSound = gameObject.AddComponent<AudioSource>();
        thumpSound.clip = thumpClip;
        // thumpSound.volume = 0.9f;
        thumpSound.playOnAwake = false;
        _player = transform.root.gameObject;
        // Debug.Log("ClubContactScript.Start()");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        // if(other.name != "Player" ){
        // if(!other.gameObject.layer == "Player"){
        // if(!other.transform.IsChildOf(_player.transform)){
        if (other.transform.root != _player.transform)
        {

            Rigidbody otherRB = other.GetComponent<Rigidbody>();
            if (otherRB != null)
            {

                if (!NewSwing)
                {
                    return;
                }
                NewSwing = false;

                // Debug.Log("ClubContactScript.OnTriggerEnter() -- "+other.name);

                thumpSound.Play();

                // float explodePower = _clubForce;

                // float upwardsModifier = 0.2f;

                // if ( otherRB.CompareTag("Mummy") && !otherRB.name.Contains("Separated"))gggg
                // {   
                //     otherRB.GetComponent<EnemyAIScript>().ApplyDamage( _clubDamage );
                // }else {

                // }
                GameManager.Instance.Explode(transform.position, _radius, _clubForce, explosionGO, "Enemies");
                // other.GetComponent<Rigidbody>().AddForce(_player.transform.forward * _clubForce);
                // Push target back...
                if (other.name.Contains("Mummy"))
                {
                    // other.transform.position += _player.transform.forward * 0.5f;
                    otherRB.AddForce(_player.transform.forward * 5f, ForceMode.VelocityChange);
                }
                else
                {
                    otherRB.AddForce(_player.transform.forward, ForceMode.Impulse);
                }
                // Debug.Log("ClubContactScript other.name:: " + otherRB.name);
                // GameManager.Instance.GamePaused = true;
                /*
                if ( otherRB.CompareTag("Mummy") && !otherRB.name.Contains("Separated"))
                {                    

                    
                    GameObject[] children = otherRB.GetComponent<SwapMummyScript>().Swap().GetComponentsInChildren<GameObject>();
    
                    foreach (GameObject child in children)
                    {
                        // Debug.Log("Foreach loop: " + child);
                        if (child != null){
                            if(child.name == "Torso" && Random.Range(0, 4) == 0){
 
                                 List<GameObject> pieces = child.GetComponent<SubdivideObjectScript>().SubdivideMe();

                                for ( int p = 0; p < pieces.Count; p++){
                                    
                                    GameObject piece = pieces[p];
                                    // Debug.Log("piece["+p+"]: "+piece);
                                    piece.GetComponent<Rigidbody>().AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                                }
                            } else {
                                child.GetComponent<Rigidbody>().AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                            }
                        }
                    }
                    
                }else if (otherRB.name == "Head" || otherRB.name == "Torso" || otherRB.name == "Legs"){
                    List<GameObject> pieces = otherRB.GetComponent<SubdivideObjectScript>().SubdivideMe();
                    // Debug.Log("pieces.Length: "+pieces.Length);
                    for ( int p = 0; p < pieces.Count; p++){
                        
                        GameObject piece = pieces[p];
                        // Debug.Log("piece["+p+"]: "+piece);
                        piece.GetComponent<Rigidbody>().AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                    }
                } else {
                    
                    otherRB.AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                }
                
                    
                }
                */



                // Vector3 forceVector = (other.transform.position - _player.transform.position).normalized; 

                // RaycastHit hit;

                // Vector3 spawnPoint = transform.position;
                // GameObject collisionSphere = Instantiate(CollisionSpherePrefab, spawnPoint, Quaternion.identity) as GameObject;
                // collisionSphere.GetComponent<Rigidbody>().AddForce(forceVector * _clubForce, ForceMode.Impulse);

            }
        }
    }

    /* private void OnCollisionEnter(Collision collision) {
         if(collision.collider.name != "Player"){

             Rigidbody otherRB = collision.collider.GetComponent<Rigidbody>();
             if( otherRB != null){
                 ContactPoint contact = collision.contacts[0];
                 Vector3 forceVector = (contact.point - _player.transform.position);
                 // Get vector parallel to ground, so we are not hitting up (which launches Mummies into the air)
                 forceVector = Vector3.ProjectOnPlane(forceVector, Vector3.up).normalized * _clubForce;
                 otherRB.AddForce(forceVector, ForceMode.Impulse);
                 thumpSound.Play();

                 Debug.Log("Club Collided: "+collision.collider.name);

                 Debug.Log("normal: "+contact.normal);
                 Debug.DrawRay(_player.transform.position, forceVector.normalized, Color.green, 4, false);
                 // GameManager.Instance.GamePaused = true;
                 Debug.Log("DIst:: "+contact.separation);

             }
         }
     }*/
    public bool NewSwing
    {
        get => _newSwing;
        set
        {
            _newSwing = value;
        }
    }
}
