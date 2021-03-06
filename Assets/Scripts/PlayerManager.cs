/*

This script is a Player's weapons manager. It should be renamed. 

Weapon class is included below.

This script is tangled with FireWeaponScript.cs

They are being combined.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    public static int CLUB_INDEX = 0;
    public static int GRENADE_INDEX = 1;
    public static int ROCKET_INDEX = 2;
    public static int PISTOL_INDEX = 3;
    public static int AK47_INDEX = 4;
    private Weapon[] _weapons = new Weapon[5];

    // public AudioClip clubWhooshClip; // <- redundant with FireWeaponScript.cs
    // private AudioSource _clubWhooshSound; // <- redundant with FireWeaponScript.cs

    // public Image grenadeReloadImage;
    // public Image rpgReloadImage;
    // public Image pistolReloadImage;
    // public AudioClip pistolReloadClip;
    // public AudioClip grenadeReloadClip;
    // public AudioClip rpgReloadClip;
    // private AudioSource _rpgReloadSound;
    // private AudioSource _pistolReloadSound;
    private Coroutine reloadCo;
    // private AudioSource _grenadeReloadSound;

    public AudioClip ammoFullClip;
    private AudioSource _ammoFullSound;

    private int _currentWeaponIndex = 0;

    private GameObject _clubGO;
    private GameObject _grenadeGunGO;
    private GameObject _rocketLauncherGO;
    private GameObject _pistolGO;
    private GameObject _AK47GO;
    private AudioSource _playerAudioSource;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _clubGO = GameObject.Find("Main Camera/ClubHolder/Club");
        _grenadeGunGO = GameObject.Find("Main Camera/GrenadeLauncher");
        _rocketLauncherGO = GameObject.Find("Main Camera/RocketLauncher");
        _pistolGO = GameObject.Find("Main Camera/Pistol");
        _AK47GO = GameObject.Find("Main Camera/AK47");
        _clubGO.SetActive(false);
        _grenadeGunGO.SetActive(false);
        _rocketLauncherGO.SetActive(false);
        _pistolGO.SetActive(false);
        _AK47GO.SetActive(false);
        CurrentWeapon = null;
        InitWeapons();

    }

    void Start()
    {

        _playerAudioSource = gameObject.AddComponent<AudioSource>();
        /*
        Color c = grenadeReloadImage.color;
        c.a = 0;
        grenadeReloadImage.color = c;
        rpgReloadImage.color = c;
        pistolReloadImage.color = c;
        */

        /*
                if (clubWhooshClip != null)
                {
                    _clubWhooshSound = gameObject.AddComponent<AudioSource>();
                    _clubWhooshSound.clip = clubWhooshClip;
                }
                if (grenadeReloadClip != null)
                {
                    _grenadeReloadSound = gameObject.AddComponent<AudioSource>();
                    _grenadeReloadSound.clip = grenadeReloadClip;
                }
                if (pistolReloadClip != null)
                {
                    _pistolReloadSound = gameObject.AddComponent<AudioSource>();
                    _pistolReloadSound.clip = pistolReloadClip;
                }
                if (rpgReloadClip != null)
                {
                    _rpgReloadSound = gameObject.AddComponent<AudioSource>();
                    _rpgReloadSound.clip = rpgReloadClip;
                }
                */
        if (ammoFullClip != null)
        {
            _ammoFullSound = gameObject.AddComponent<AudioSource>();
            _ammoFullSound.clip = ammoFullClip;
            _ammoFullSound.volume = 0.5f;
        }

    }


    private void InitWeapons()
    {
        // Create all weapons.  Make unequipped by default;
        _weapons[CLUB_INDEX] = new Weapon("Club", _clubGO, CLUB_INDEX, int.MaxValue, int.MaxValue, "Images/Club_sprite", "Images/Club_sprite", "Sounds/whoosh", "Sounds/whoosh", "Sounds/whoosh");

        _weapons[GRENADE_INDEX] = new Weapon("Grenade Launcher", _grenadeGunGO, GRENADE_INDEX, 50, 100, "Images/GrenadeGun_sprite", "Images/ammoBox_sprite", "Sounds/single-nerf-shot", "Sounds/grenade-reload", "Sounds/metalic_click");

        _weapons[ROCKET_INDEX] = new Weapon("Rocket Launcher", _rocketLauncherGO, ROCKET_INDEX, 20, 50, "Images/Rocket_sprite", "Images/rocketAmmo_sprite", "Sounds/rocket-shoot-002", "Sounds/rocket-reload", "Sounds/metalic_click");

        _weapons[PISTOL_INDEX] = new Weapon("Pistol", _pistolGO, PISTOL_INDEX, 50, 50, "Images/Pistol_sprite", "Images/pistolAmmo_sprite", "Sounds/gunshot", "Sounds/grenade-reload", "Sounds/metalic_click", 0.7f);

        _weapons[AK47_INDEX] = new Weapon("AK47", _AK47GO, AK47_INDEX, 50, 100, "Images/AK47_sprite", "Images/AKAmmo_sprite", "Sounds/gunshot", "Sounds/grenade-reload", "Sounds/metalic_click", 0.1f);

        foreach (Weapon weapon in _weapons)
        {
            weapon.Equipped = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            // Cycle Through Weapons
            CycleWeapons();
        }
    }

    private void CycleWeapons()
    {
        // Cycle through to next available weapon
        int index = _currentWeaponIndex;
        int tries = 0;
        do
        {
            index = (index + 1) % _weapons.Length;
            tries++;
            if (_weapons[index].Equipped && _weapons[index].Ammo > 0)
            {
                WeaponIndex = index;
                break;
            }
        } while ((!_weapons[index].Equipped || _weapons[index].Ammo <= 0) && tries < _weapons.Length);
    }

    // void OnGUI(){

    //     Event e = Event.current;
    //     if (e.isKey)
    //     {
    //         if(e.type == EventType.KeyUp){
    //             m.Log("KeyUp:: "+e.keyCode);
    //             if(e.keyCode == 'C'){
    //                 WeaponIndex ++;
    //             }
    //         }
    //     }
    // }

    public Weapon GetWeaponByName(string weaponName)
    {
        foreach (Weapon weapon in _weapons)
        {
            if (weapon.Name == weaponName)
            {
                return weapon;
            }
        }
        return null;
    }
    public bool AddWeaponByName(string weaponName)
    {

        return AddWeaponByIndex(GetWeaponByName(weaponName).Index);
    }
    public bool AddWeaponByIndex(int weaponInex)
    {
        // return true if we were able to equip weapon or add ammo
        Weapon weapon = _weapons[weaponInex];
        // Check if Player already has this weapon

        if (!weapon.Equipped)
        {
            // Add new weapon type to Player's inventory

            GameManager.Instance.ShowSprite(weapon.MySprite);
            Audio.PlayOneShot(weapon.LoadClip);

            weapon.Equipped = true;
            int equippedWeaponCount = 0;
            foreach (Weapon w in _weapons)
            {
                if (w.Equipped)
                    equippedWeaponCount++;
            }
            string use;
            switch (weaponInex)
            {
                case 0:
                    use = "swing";
                    break;
                case 1:
                    use = "launch";
                    break;
                case 2:
                case 3:
                case 4:
                    use = "fire";
                    break;
                default:
                    use = "use";
                    break;
            }
            // Wait a pause, then show message
            float delay = 0.4f;
            if (equippedWeaponCount == 1)
            {
                CurrentWeapon = weapon;
                GameManager.Instance.ShowMessage("You Got the " + weapon.Name + "!");
                // GameManager.Instance.ShowMessage("'G' or Left Mouse Btn to " + use + ".");
            }
            else
            {
                GameManager.Instance.ShowMessage("You Got the " + weapon.Name + "!");
                // GameManager.Instance.ShowMessage("'C' to change weapons,\n\r'G' or Left Mouse Btn to " + use + ".");
            }
            return true;
        }
        else
        {
            // Weapon already equipped. Add ammo for that weapon
            return ReloadWeapon(weapon.Name, 10);
        }
    }

    public void PlayAmmoFullSound()
    {
        if (_ammoFullSound) _ammoFullSound.Play();
    }

    public bool ReloadWeapon(string weaponName, int amount = 5)
    {
        if (weaponName == "") weaponName = PlayerManager.Instance.CurrentWeapon.Name;
        bool addedAmmo = PlayerManager.Instance.GetWeaponByName(weaponName).AddAmmo(amount);
        if (!addedAmmo)
        {
            return false;
        }

        /*
        Image reloadImage;

        AudioSource reloadSound;
        switch(weaponName) {
        
            case "Rocket Launcher":
                reloadSound = _rpgReloadSound;
                reloadImage = rpgReloadImage;
                break;
            case "Grenade Thrower":
                reloadSound = _grenadeReloadSound;
                reloadImage = grenadeReloadImage;
                break;
            case "Pistol":
                reloadSound = _pistolReloadSound;
                reloadImage = pistolReloadImage;
                break;
            default:
                reloadSound = _clubWhooshSound;
                reloadImage = grenadeReloadImage;
                break;
            
        }
        reloadSound.Play();
        
        if (reloadCo != null)
            StopCoroutine(reloadCo);
        reloadCo = StartCoroutine(FlashImage(reloadImage));
        */

        Weapon weaponToReload = GetWeaponByName(weaponName);

        GameManager.Instance.ShowSprite(weaponToReload.AmmoSprite);

        Audio.PlayOneShot(weaponToReload.LoadClip);
        return true;
    }
    IEnumerator FlashImage(Image whichImage)
    {
        // Debug.Log(" ----- CoRoutine START!!");
        whichImage.gameObject.SetActive(true);
        whichImage.color = Color.white;
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            // Debug.Log("FlashDamageImage(" + ft + ")");
            Color c = whichImage.color;
            if (ft < 0.1f)
                ft = 0;

            c.a = ft;
            whichImage.color = c;
            yield return new WaitForSeconds(.1f); ;
        }
        // Debug.Log(" ----- CoRoutine OVER!!");
        whichImage.gameObject.SetActive(false);
    }

    //  GETTERS / SETTERS

    public AudioSource Audio
    {
        get => _playerAudioSource;
    }

    public Weapon CurrentWeapon
    {
        get
        {
            return _weapons.Length > 0 ? _weapons[_currentWeaponIndex] : null;
        }
        set
        {
            // Debug.Log(value);
            if (value is Weapon && System.Array.IndexOf(_weapons, value) != -1)
            {
                GameManager.Instance.ShowSprite(value.MySprite);
                _currentWeaponIndex = System.Array.IndexOf(_weapons, value);
                foreach (Weapon w in _weapons)
                {
                    w.Active = w.Index == _currentWeaponIndex ? true : false;
                }
                value.AddAmmo(0);
            }
        }
    }

    public int WeaponIndex
    {
        get
        {
            return _currentWeaponIndex;
        }
        private set
        {
            // Set Weapon by index
            value = value % _weapons.Length;
            _currentWeaponIndex = value;
            CurrentWeapon = _weapons[value];
        }
    }

}



/*

    WEAPON CLASS

*/

public class Weapon
{
    private Image _ammoBar = GameObject.Find("UI_Canvas/AmmoBar_bg/AmmoBar_bar").GetComponent<Image>();
    private string _name;
    private int _index;
    private GameObject _myGO;
    private int _ammoCount;
    private int _maxAmmo;
    private Sprite _mySprite;
    private Sprite _ammoSprite;
    private bool _equipped = false;
    private AudioClip _loadClip;
    private AudioClip _fireClip;
    private AudioClip _emptyClip;
    private float _autoFireFrequency;


    public Weapon(string name, GameObject myGameObject, int index, int ammo, int maxAmmo, string spritePath, string ammoSpritePath, string fireClipPath, string loadClipPath, string emptyClipPath, float autoFireFrequency = 0f)
    {
        // Debug.Log("New Weapon()");
        // Debug.Log("index: "+index);
        Name = name;
        Index = index;
        _myGO = myGameObject; // The weapon's gameObject in Player gameObject
        _ammoCount = ammo;
        _maxAmmo = maxAmmo;
        MySprite = Resources.Load<Sprite>(spritePath);
        AmmoSprite = Resources.Load<Sprite>(ammoSpritePath);
        FireClip = Resources.Load<AudioClip>(fireClipPath);
        LoadClip = Resources.Load<AudioClip>(loadClipPath);
        EmptyClip = Resources.Load<AudioClip>(emptyClipPath);
        AutoFrequency = autoFireFrequency;
    }

    public bool AddAmmo(int amt)
    {
        // !!! Needs Refactoring !!!
        //
        // Debug.Log("Weapon("+Name+") AddAmmo("+amt+")");
        bool success = false;
        if ((amt > 0 && _ammoCount < _maxAmmo) || (amt < 0 && _ammoCount + amt >= 0))
        {
            // Not full and adding, or subtracting but not more than we have
            Ammo += amt;
            success = true;
        }
        else if (amt <= 0)
        {
            // Subtracting
            if (_ammoCount > 0)
            {
                // We can subtract more than we have, but Ammo will bottom out at zero
                Ammo += amt;
                success = true;
            }
            else
            {
                // Ammo is too low to subtract from
                GameManager.Instance.ShowMessage(Name + " is empty.");
                success = false;
            }
        }
        else
        {
            if (PlayerManager.Instance.GetWeaponByName(Name).Equipped)
            {
                GameManager.Instance.ShowMessage(Name + " is full.");
            }
            else
            {
                // Player doesn't have this weapon equipped
                GameManager.Instance.ShowMessage("You're carrying max \n\r" + Name + " ammo.");
            }

            PlayerManager.Instance.PlayAmmoFullSound();
            success = false;
        }
        UpdateAmmoBar();
        return success;

    }

    private void UpdateAmmoBar()
    {
        if (PlayerManager.Instance.CurrentWeapon == this)
        {
            // Debug.Log(PlayerManager.Instance.CurrentWeapon.Name + " == " + this.Name);
            float ratio = (float)Ammo / (float)MaxAmmo;
            _ammoBar.fillAmount = ratio;
        }

    }

    public Sprite MySprite
    {
        get => _mySprite;
        private set
        {
            if (value is Sprite)
            {
                _mySprite = value;
            }
        }
    }

    public Sprite AmmoSprite
    {
        get => _ammoSprite;
        private set
        {
            _ammoSprite = value;
        }
    }

    public AudioClip FireClip
    {
        get => _fireClip;
        private set
        {
            _fireClip = value;
        }
    }

    public AudioClip LoadClip
    {
        get => _loadClip;
        private set
        {
            _loadClip = value;
        }
    }

    public AudioClip EmptyClip
    {
        get => _emptyClip;
        private set
        {
            _emptyClip = value;
        }
    }

    public int Ammo
    {
        get => _ammoCount;
        private set
        {
            _ammoCount = Mathf.Clamp(value, 0, _maxAmmo);
        }
    }
    public int MaxAmmo
    {
        get => _maxAmmo;
    }

    public string Name
    {
        get => _name;
        private set
        {
            if (value is string)
            {
                _name = value;
            }
            else
            {
                _name = null;
            }
        }
    }
    public int Index
    {
        get => _index;
        private set
        {
            _index = value;
        }
    }
    public bool Active
    {
        // Player is using it
        get => _myGO.activeSelf;
        set
        {
            _myGO.SetActive(value);
        }
    }
    public bool Equipped
    {
        // Player has it
        get => _equipped;
        set
        {
            _equipped = value;
        }
    }
    public float AutoFrequency
    {
        get => _autoFireFrequency;
        set
        {
            _autoFireFrequency = value;
        }
    }
    public GameObject MyGO
    {
        get => _myGO;
        set
        {
            _myGO = value;
        }
    }
}
