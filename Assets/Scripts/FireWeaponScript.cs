        /*

Wrote this to get weapons firing.

This script is tangled with PlayerManager.cs 

They should be combined.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class FireWeaponScript : MonoBehaviour
{
    public GameObject grenadePrefab;
    public GameObject rocketPrefab;
    public GameObject bulletImpactPrefab;
    public AudioClip gunClip;
    public AudioClip emptyClip;
    public AudioClip pistolClip;
    // public Image ammoBar;

    public AudioClip clubWhooshClip;

    // private string _currentWeapon = "None";
    private float speed = 32;
    
    private AudioSource gunSound;
    private AudioSource emptySound;
    private AudioSource _pistolSound;
    
    // private int maxAmmo = 100;
    // private int currentAmmo;
    
    private CauseCommotionScript _commotionScript;
    private GameObject _club;
    private AudioSource _clubWhooshSound;
    private float _lastSwingTime = 0f;
    // private GameObject _gun;
    private ParticleSystem _muzzleFlash;
    private GameObject _pistol;
    private GameObject _pistolCrosshair;

    void Start()
    {
        _club = GameObject.Find("Player/Main Camera/ClubHolder/Club");
        // _gun = GameObject.Find("Main Camera/Gun");
        _commotionScript = GetComponent<CauseCommotionScript>();
        // CurrentWeapon = "Club";
        if (gunClip != null)
        {
            gunSound = gameObject.AddComponent<AudioSource>();
            gunSound.clip = gunClip; //Resources.Load(name) as AudioClip;
            gunSound.volume = 0.3f;
        }

        if (clubWhooshClip != null)
        {
            _clubWhooshSound = gameObject.AddComponent<AudioSource>();
            _clubWhooshSound.clip = clubWhooshClip; //Resources.Load(name) as AudioClip;
            // _clubWhooshSound.volume = 0.3f;
        }
        if (emptyClip != null)
        {
            emptySound = gameObject.AddComponent<AudioSource>();
            emptySound.clip = emptyClip; //Resources.Load(name) as AudioClip;
        }
        
        if (pistolClip != null)
        {
            _pistolSound = gameObject.AddComponent<AudioSource>();
            _pistolSound.clip = pistolClip;
            // pistolSound.volume = 0.5f;
        }
        // v Locating muzzleFlash should live in an init function (Start, Awake, etc)

        GameObject myCamera = GameObject.Find("Main Camera");
        _pistol = GameManager.Instance.FindChildByName(myCamera, "Pistol");
        _pistolCrosshair = GameManager.Instance.FindChildByName(myCamera, "CrosshairCanvas");

        _muzzleFlash = GameManager.Instance.FindChildByName(myCamera, "MuzzleFlash").GetComponent<ParticleSystem>();
        // currentAmmo = maxAmmo;
        
    }

    // void UpdateAmmoBar()
    // {
    //     float ratio = (float)PlayerManager.Instance.CurrentWeapon.Ammo / (float)PlayerManager.Instance.CurrentWeapon.MaxAmmo;
    //     ammoBar.fillAmount = ratio;
    // }

    void FirePistol() 
    {
        Debug.Log("FireWeaponScript.FirePistol()");
        Transform cameraTransform = Camera.main.transform;
        Vector3 pistolPos = cameraTransform.position; //_pistol.transform.position;
        Vector3 shotDirection = cameraTransform.forward;
        
        
        if (_pistolSound != null)
            _pistolSound.PlayOneShot(pistolClip, 1f);
        // 1. muzzle flash
        _muzzleFlash.Play();

        //   1b. hide crosshair
        _pistolCrosshair.SetActive(false);
        GameManager.Instance.DelayFunction( ()=>{
            _pistolCrosshair.SetActive(true);
        }, 1f );
        
        // 2. cast forward-facing ray
        RaycastHit hit;
        if (Physics.Raycast(pistolPos, shotDirection, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(pistolPos, shotDirection * hit.distance, Color.yellow, 10);
            // Debug.Log("Did Hit :: "+hit.collider.name+" , dist:: "+hit.distance);
            // Quaternion quatRot = Quaternion.Inverse(cameraTransform.rotation);
            // Quaternion quatRot = Quaternion.Euler(-cameraTransform.rotation.eulerAngles);
            GameObject bulletImpact = Instantiate(bulletImpactPrefab, hit.point, cameraTransform.rotation) as GameObject;

            GameManager.Instance.Explode( hit.point, 0.1f, 64f, null, "Enemies");
            if( hit.rigidbody != null )
            {
                hit.rigidbody.AddForceAtPosition( shotDirection*100, hit.point );
            }
            
        }
        //   2b. get distance of ray hit
        //   2c. calculate time for bullet to travel that distance
        //   2d. wait that duration
        //   2e. cast same ray, get ray hit
        //   2f. spawn bullet impact at that point
        //     2f1. impact smoke puff particle emitter
        //     2f2. tiny explosion at that point
        // 

    }

    void FireRocket()
    {
        Vector3 playerPos = Camera.main.transform.position;
        playerPos.y -= 0.5f;
        // playerPos.y -= yOffset;
        Vector3 playerDirection = transform.forward;
        Quaternion playerRotation = transform.rotation;
        float cameraXrot = Camera.main.transform.eulerAngles.x;
        Quaternion cameraRotation = Camera.main.transform.rotation;

        Vector3 launchDir = Quaternion.AngleAxis(cameraXrot, transform.right) * playerDirection;

        float spawnDistance = 2f;

        Vector3 spawnPos = playerPos + launchDir * spawnDistance;

        // Instantiate Rocket
        GameObject myRocket = Instantiate(rocketPrefab, spawnPos, cameraRotation) as GameObject;

    }

    void ThrowGrenade()
    {
        if (PlayerManager.Instance.CurrentWeapon.Ammo <= 0)
        {
            emptySound.Play();
            return;
        }
        // currentAmmo -= 1;
        
        if (gunSound != null)
            gunSound.Play();
        // float yOffset = 0.5f;
        Vector3 playerPos = Camera.main.transform.position;
        playerPos.y -= 0.2f;
        // playerPos.y -= yOffset;
        Vector3 playerDirection = transform.forward;
        Quaternion playerRotation = transform.rotation;
        float cameraXrot = Camera.main.transform.eulerAngles.x;
        // Vector3 launchDir = Quaternion.Euler(-1 * cameraXrot, 0, 0) * playerDirection;

        Vector3 launchDir = Quaternion.AngleAxis(cameraXrot, transform.right) * playerDirection;

        /*
        Debug.Log("cameraRot: " + Camera.main.transform.rotation);
        Debug.Log("cameraXrot: " + cameraXrot);
        Debug.Log("launchDir: (" + launchDir.x + ", " + launchDir.y + ", " + launchDir.z + ")");
        */

        float spawnDistance = 1f;

        Vector3 spawnPos = playerPos + launchDir * spawnDistance;

        // Instantiate Grenade
        GameObject myGrenade = Instantiate(grenadePrefab, spawnPos, playerRotation) as GameObject;
        // myGrenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity) as GameObject;

        // Attach CollideGrenade script to Grenade
        //myGrenade.AddComponent<GrenadeCollideScript>();

        // Apply Throw Force
        Vector3 grenadeVector = launchDir * speed;
        grenadeVector.y += 2;
        // grenadeVector.y = 4 + cameraXrot *4;
        myGrenade.GetComponent<Rigidbody>().velocity = grenadeVector + gameObject.GetComponent<Rigidbody>().velocity;
    }

    void SwingClub(){
        if ( Time.time > _lastSwingTime + 0.4f ){
            _lastSwingTime = Time.time;
            _club.GetComponent<Animator>().Play("Swing");
            if(_clubWhooshSound != null)
                _clubWhooshSound.Play();
            // v Assumes ClubContactScript is on a child of the object to which this script is attached
            gameObject.GetComponentInChildren<ClubContactScript>().NewSwing = true;
        }
    }


    void Update()
    {
        if (!GameManager.Instance.GamePaused && (Input.GetKeyUp(KeyCode.G) || Input.GetMouseButtonDown(0)))
        {
            // Debug.Log("G key was released.");
            
            string weaponName = PlayerManager.Instance.CurrentWeapon.Equipped ? PlayerManager.Instance.CurrentWeapon.Name : "None";
            if (weaponName != "None" && PlayerManager.Instance.CurrentWeapon.Ammo <= 0 ){
                weaponName = "Empty";
            }

            Debug.Log("FireWeaponScript.currentWeapon: " + PlayerManager.Instance.CurrentWeapon.Name);

            if (weaponName == "Grenade Thrower")
            {
                ThrowGrenade();
                _commotionScript.CauseCommotion(10f, 3f);
            }
            else if (weaponName == "Rocket Launcher")
            {
                FireRocket();
                _commotionScript.CauseCommotion(20f, 3f);
            }
            else if (weaponName == "Club"){
                SwingClub();
                _commotionScript.CauseCommotion(5f, 3f);
            } else if (weaponName == "Pistol"){
                FirePistol();
                _commotionScript.CauseCommotion(10f, 3f);
            } else if (weaponName == "None") {
                GameManager.Instance.ShowMessage("You Need a Weapon.", 0.1f);
            } else if (weaponName == "Empty"){
                GameManager.Instance.ShowMessage("You Need Ammo", 0.1f);
            }
            if(PlayerManager.Instance.CurrentWeapon.Equipped)
                PlayerManager.Instance.CurrentWeapon.AddAmmo(-1);
            // UpdateAmmoBar();
        }
    }

    

    /*
    // Moved CurrentWeapon to PlayerManager
    public string CurrentWeapon
    {
        get { return PlayerManager.Instance.CurrentWeapon.Name; }
        set
        {
            Debug.Log("Set Weapon..."+value);
            bool newWeapon = true;
            if(value != _currentWeapon){
                switch (value)
                {
                    case "Club":
                    Debug.Log("case Club");
                        _club.SetActive(true);
                        _gun.SetActive(false);
                        break;
                    case "Grenade":
                    case "Rocket":
                    Debug.Log("case Rocket or Grenade");
                        _club.SetActive(false);
                        _gun.SetActive(true);
                        break;
                    default:
                        newWeapon = false;
                        break;
                }
                if(newWeapon)
                    _currentWeapon = value;
            }
        }
    }
    */
}
