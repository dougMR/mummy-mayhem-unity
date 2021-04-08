/*
PlayerManager should be reconceived.
It is currently more of a player's weapons manager
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
    private int _currentWeaponIndex = 0;
    private Weapon[] _weapons = new Weapon[4];
    // string[] Ar = new string[10];
    private GameObject _clubGO;
    private GameObject _grenadeGunGO;
    private GameObject _rocketLauncherGO;
    private GameObject _pistolGO;
    
    
    // private GameObject _player;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        // Init();
        // _player = GameObject.Find("Player");
        _clubGO = GameObject.Find("Main Camera/ClubHolder/Club");
        _grenadeGunGO = GameObject.Find("Main Camera/Gun");
        _rocketLauncherGO = GameObject.Find("Main Camera/RocketLauncher");
        _pistolGO = GameObject.Find("Main Camera/Pistol");
        _clubGO.SetActive(false);
        _grenadeGunGO.SetActive(false);
        _rocketLauncherGO.SetActive(false);
        _pistolGO.SetActive(false);
        CurrentWeapon = null;
        InitWeapons();
        
        // AddWeaponByName("Pistol");
    }


    private void InitWeapons()
    {
        // Create all weapons.  Make unequipped by default;
        _weapons[CLUB_INDEX] = new Weapon("Club", _clubGO, CLUB_INDEX, int.MaxValue, int.MaxValue, "Images/Club_sprite");
        _weapons[GRENADE_INDEX] = new Weapon("Grenade Thrower", _grenadeGunGO, GRENADE_INDEX, 50, 100, "Images/GrenadeGun_sprite");
        _weapons[ROCKET_INDEX] = new Weapon("Rocket Launcher", _rocketLauncherGO, ROCKET_INDEX, 20, 50, "Images/Rocket_sprite");
        _weapons[PISTOL_INDEX] = new Weapon("Pistol", _pistolGO, PISTOL_INDEX, 50, 50, "Images/Pistol_sprite");
        foreach(Weapon weapon in _weapons){
            weapon.Equipped = false;
        }        
    }

    private void Update(){
        if (Input.GetKeyUp(KeyCode.C))
        {
            // Cycle Through Weapons
            CycleWeapons();
        }
    }

    private void CycleWeapons(){
        // Cycle through to next available weapon
        int index = _currentWeaponIndex;
        int tries = 0;
        do
        {
            index = (index+1) % _weapons.Length;
            tries ++;
            if(_weapons[index].Equipped){
                WeaponIndex = index;
                break;
            }
        }while(!_weapons[index].Equipped && tries < _weapons.Length);
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
    public Weapon GetWeaponByName( string weaponName) {
        foreach( Weapon weapon in _weapons){
            if(weapon.Name == weaponName){
                return weapon;
            }
        }
        return null;
    }
    public bool AddWeaponByName( string weaponName ){

        return AddWeaponByIndex( GetWeaponByName(weaponName).Index );
    }
    public bool AddWeaponByIndex( int weaponInex ){
        // return true if we were able to equip weapon or add ammo
        Weapon weapon = _weapons[weaponInex];
        // Check if Player already has this weapon

        if ( !weapon.Equipped ){
            // Add new weapon type to Player's inventory

            GameManager.Instance.ShowSprite( weapon.MySprite );
            weapon.Equipped = true;
            int equippedWeaponCount = 0;
            foreach (Weapon w in _weapons){
                if(w.Equipped)
                    equippedWeaponCount ++;
            }
            string use;
            switch (weaponInex)
            {
                case 0:
                    use = "swing";
                    break;
                case 1:
                    use = "throw";
                    break;
                case 2:
                case 3:
                    use = "fire";
                    break;
                default:
                    use = "use";
                    break;
            }
            // Wait a pause, then show message
            float delay = 0.4f;
            if(equippedWeaponCount == 1) {
                CurrentWeapon = weapon;
                GameManager.Instance.ShowMessage("You Got the "+weapon.Name+"!");
                GameManager.Instance.ShowMessage("'G' or Left Mouse Btn to "+use+".");
            } else {
                GameManager.Instance.ShowMessage("You Got the "+weapon.Name+"!");
                GameManager.Instance.ShowMessage("'C' to change weapons,\n\r'G' or Left Mouse Btn to "+use+".");
            }
            return true;
        } else {
            // Weapon already equipped. Add ammo for that weapon
            return weapon.AddAmmo(10);
        }
    }

    public Weapon CurrentWeapon {
        get {
            return _weapons.Length > 0 ? _weapons[_currentWeaponIndex] : null;
        }
        set {
            // Debug.Log(value);
            if(value is Weapon && System.Array.IndexOf(_weapons, value) != -1){
                GameManager.Instance.ShowSprite( value.MySprite );
                _currentWeaponIndex = System.Array.IndexOf(_weapons, value);
                foreach (Weapon w in _weapons)
                {
                    w.Active = w.Index == _currentWeaponIndex ? true : false;
                }
                value.AddAmmo(0);
            }
        }
    }

    public int WeaponIndex {
        get 
        {
            return _currentWeaponIndex;
        }
        private set 
        {
            // Set Weapon by index
            // if ( value is int){
                value = value % _weapons.Length;
                _currentWeaponIndex = value;
                CurrentWeapon = _weapons[value];
            // }
        }
    }
}

public class Weapon
{
    private Image _ammoBar = GameObject.Find("UI_Canvas/AmmoBar_bg/AmmoBar_bar").GetComponent<Image>();
    private string _name;
    private int _index;
    private GameObject _myGO;
    private int _ammoCount;
    private int _maxAmmo;
    private Sprite _mySprite;
    private bool _equipped = false;
    

    public Weapon(string name, GameObject myGameObject, int index, int ammo, int maxAmmo, string spritePath)
    {
        // Debug.Log("New Weapon()");
        // Debug.Log("index: "+index);
        Name = name;
        Index = index;
        _myGO = myGameObject; // The weapon's gameObject in Player gameObject
        _ammoCount = ammo;
        _maxAmmo = maxAmmo;
        MySprite = Resources.Load <Sprite>(spritePath);
    }

    public bool AddAmmo( int amt ){
        if((amt > 0 && _ammoCount < _maxAmmo) || (amt < 0 && _ammoCount + amt >= 0)){
            Ammo+= amt;
            amt = Mathf.Clamp(amt, 0, _maxAmmo);
            UpdateAmmoBar();
            return true;
        }
        UpdateAmmoBar();
        return false;
    }

    private void UpdateAmmoBar()
    {
        float ratio = (float)Ammo / (float)MaxAmmo;
        _ammoBar.fillAmount = ratio;
    }

    public Sprite MySprite
    {
        get => _mySprite;
        private set {
            if(value is Sprite){
                _mySprite = value;
            }
        }
    }

    public int Ammo
    {
        get => _ammoCount;
        private set {
            // if ( value is int ){
                _ammoCount = value;
            // }
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
            } else {
                _name = null;
            }
        }
    }
    public int Index {
        get => _index;
        private set 
        {
            // if(value is int){
                _index = value;
            // }
        }
    }
    public bool Active
    {
        // Player is using it
        get => _myGO.activeSelf;
        set 
        {
            // if (value is bool) {
                _myGO.SetActive(value);
            // }
        }
    }
    public bool Equipped
    {
        // Player has it
        get => _equipped;
        set 
        {
            // if (value is bool) {
                _equipped = value;
            // }
        }
    }
}
