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
    public GameObject bulletPrefab;
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

    private GameObject _club;
    private AudioSource _clubWhooshSound;
    private float _lastSwingTime = 0f;
    // private GameObject _gun;
    private ParticleSystem _muzzleFlash;
    private GameObject _crosshairs;
    private GameObject _pistol;
    private GameObject _AK47;
    private float _lastFiredTime = 0;
    private bool _triggerReleased = true;

    void Start()
    {
        _club = GameObject.Find("Player/Main Camera/ClubHolder/Club");
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
        _AK47 = GameManager.Instance.FindChildByName(myCamera, "AK47");
        _crosshairs = GameManager.Instance.FindChildByName(myCamera, "Crosshairs");

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
        // Debug.Log("FireWeaponScript.FirePistol()");
        Weapon weapon = PlayerManager.Instance.GetWeaponByName("Pistol");
        // if (_pistolSound != null)
        //     _pistolSound.PlayOneShot(pistolClip, 1f);        
        PlayerManager.Instance.Audio.PlayOneShot(weapon.FireClip);

        // 1. muzzle flash
        // !!! Move muzzleflash when we change weapon
        _muzzleFlash.transform.position = weapon.MyGO.transform.position;
        _muzzleFlash.Play();
        _pistol.GetComponent<Animator>().Play("Pistol Kick", -1, 0f);

        //   1b. hide crosshair
        _crosshairs.SetActive(false);
        GameManager.Instance.DelayFunction(() =>
        {
            _crosshairs.SetActive(true);
        }, 1f);

        // 2. cast forward-facing ray
        ShootBullet();
        //   2b. get distance of ray hit
        //   2c. calculate time for bullet to travel that distance
        //   2d. wait that duration
        //   2e. cast same ray, get ray hit
        //   2f. spawn bullet impact at that point
        //     2f1. impact smoke puff particle emitter
        //     2f2. tiny explosion at that point
        // 

    }

    void FireAK47()
    {
        // Debug.Log("FireWeaponScript.FireAK47()");

        Weapon weapon = PlayerManager.Instance.GetWeaponByName("AK47");

        PlayerManager.Instance.Audio.PlayOneShot(weapon.FireClip);

        // 1. muzzle flash
        _muzzleFlash.transform.position = weapon.MyGO.transform.position;
        _muzzleFlash.Play();
        _AK47.GetComponent<Animator>().Play("AKkick", -1, 0f);

        //   1b. hide crosshair
        _crosshairs.SetActive(false);
        GameManager.Instance.DelayFunction(() =>
        {
            _crosshairs.SetActive(true);
        }, 1f);

        // 2. cast forward-facing ray
        ShootBullet(10f, 0.3f);
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

    void LaunchGrenade()
    {

        if (PlayerManager.Instance.CurrentWeapon.Ammo <= 0)
        {
            emptySound.Play();
            return;
        }
        Weapon weapon = PlayerManager.Instance.GetWeaponByName("Grenade Launcher");
        weapon.MyGO.GetComponent<Animator>().Play("Kick", -1, 0f);
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

    void SwingClub()
    {
        if (Time.time > _lastSwingTime + 0.4f)
        {
            _lastSwingTime = Time.time;
            _club.GetComponent<Animator>().Play("Swing");
            if (_clubWhooshSound != null)
                _clubWhooshSound.Play();
            // v Assumes ClubContactScript is on a child of the object to which this script is attached
            gameObject.GetComponentInChildren<ClubContactScript>().NewSwing = true;
        }
    }


    void ShootBullet(float impactPower = 8f, float impactRadius = 0.2f)
    {

        // Debug.Log("     ------  FireWeaponScript.ShootBullet()");
        Transform cameraTransform = Camera.main.transform;
        Vector3 pistolPos = cameraTransform.position;
        Vector3 shotDirection = cameraTransform.forward;

        // 2. cast forward-facing ray
        // don't hit Player
        int layerInt = LayerMask.NameToLayer("Player"); // -1 if not found
        // Debug.Log("layerInt: " + layerInt);
        int notPlayer = layerInt == -1 ? ~0 : ~layerInt; // ~ is bitwise NOT, so ~0 means All Layers
        RaycastHit hit;
        if (Physics.Raycast(pistolPos, shotDirection, out hit, Mathf.Infinity, notPlayer, QueryTriggerInteraction.Ignore))
        {
            // ^ .Ignore means don't hit triggers
            // Debug.DrawRay(pistolPos, shotDirection * hit.distance, Color.yellow, 10);
            // Debug.Log("Did Hit :: "+hit.collider.name+" , dist:: "+hit.distance);

            GameObject bulletImpact = Instantiate(bulletImpactPrefab, hit.point, cameraTransform.rotation) as GameObject;

            if (hit.rigidbody != null)
            {
                // Debug.Log("FireWeapon.Shoot hit [" + hit.rigidbody.name + "]");
                hit.rigidbody.AddForceAtPosition(shotDirection * impactPower * 2, hit.point);
            }

            GameManager.Instance.Explode(hit.point, impactRadius, impactPower, null);

            // GameObject bullet = Instantiate(bulletPrefab, hit.point, cameraTransform.rotation) as GameObject;

            // bullet.GetComponent<Rigidbody>().AddForce(cameraTransform.forward * 800f, ForceMode.Impulse);
            // bullet.GetComponent<Rigidbody>().velocity = cameraTransform.forward * 800f;

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


    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }
        if (Input.GetKeyUp(KeyCode.G) || Input.GetMouseButtonUp(0))
        {
            _triggerReleased = true;
        }
        else if (Input.GetKey(KeyCode.G) || Input.GetMouseButton(0))
        {
            float autoFrequency = PlayerManager.Instance.CurrentWeapon.AutoFrequency;
            bool canfire = _triggerReleased ||
                    (autoFrequency > 0 && Time.time - _lastFiredTime > autoFrequency);

            if (!canfire)
            {
                // Not long enough since last shot, abort
                return;
            }
            _lastFiredTime = Time.time;
            _triggerReleased = false;

            string weaponName = PlayerManager.Instance.CurrentWeapon.Equipped ? PlayerManager.Instance.CurrentWeapon.Name : "None";
            bool weaponEmpty = weaponName != "None" && PlayerManager.Instance.CurrentWeapon.Ammo <= 0;

            // Debug.Log("FireWeaponScript.currentWeapon: " + PlayerManager.Instance.CurrentWeapon.Name);

            if (weaponEmpty)
            {
                GameManager.Instance.ShowMessage("You Need Ammo", 0.1f);
            }
            else if (weaponName == "Grenade Launcher")
            {
                LaunchGrenade();
                GameManager.Instance.CauseCommotion(transform.position, 8f, 3f);
            }
            else if (weaponName == "Rocket Launcher")
            {
                FireRocket();
                GameManager.Instance.CauseCommotion(transform.position, 20f, 3f);
            }
            else if (weaponName == "Club")
            {
                SwingClub();
                GameManager.Instance.CauseCommotion(transform.position, 5f, 3f);
            }
            else if (weaponName == "Pistol")
            {
                FirePistol();
                GameManager.Instance.CauseCommotion(transform.position, 10f, 3f);
            }
            else if (weaponName == "AK47")
            {
                FireAK47();
                GameManager.Instance.CauseCommotion(transform.position, 10f, 3f);
            }
            else if (weaponName == "None")
            {
                GameManager.Instance.ShowMessage("You Need a Weapon.", 0.1f);
            }
            if (PlayerManager.Instance.CurrentWeapon.Equipped)
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
