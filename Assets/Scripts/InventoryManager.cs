using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public AudioClip pickupClip;
    public int value = 10;
    public GameObject inventorySlotPrefab;
    private Canvas _UICanvas;
    private List<InventoryItem> _items = new List<InventoryItem>();
    private bool _inventoryShowing = true;
    private GameObject _inventoryPanel;
    private AudioSource _pickupSound;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _inventoryPanel = GameObject.Find("/UI_Canvas/Inventory Panel");

        _pickupSound = gameObject.AddComponent<AudioSource>();
        _pickupSound.clip = pickupClip;
        
        // ToggleInventoryDisplay(false);
        // Testing create inventory items display
    /*
        _UICanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
        Image newSlot = Instantiate(inventorySlotPrefab);
        newSlot.transform.SetParent(_UICanvas.transform);
        
        RectTransform slotRectTransform = newSlot.GetComponent<RectTransform>();
        // slotRectTransform.anchoredPosition = new Vector2(0, 0);
        slotRectTransform.localPosition = new Vector3(0, 0, 0);
        // slotRectTransform.pivot = new Vector2(0, 0);
        */
/*
        // Vector3 v3Pos = new Vector3(0.0f, 1.0f, 0.25f);
        // newSlot.transform.position = Camera.main.ViewportToWorldPoint(v3Pos);
        float xMin = _UICanvas.GetComponent<RectTransform>().rect.xMin;
        float yMax = _UICanvas.GetComponent<RectTransform>().rect.yMax;
        Debug.Log("xMin: " + xMin);
        // slotRectTransform.position = new Vector3(xMin, yMax, 25);

        Debug.Log("slotRectTransform.pivot: " + slotRectTransform.pivot);
        Debug.Log("slotRectTransform.anchoredPosition: " + slotRectTransform.anchoredPosition);
        Debug.Log("rect: " + _UICanvas.GetComponent<RectTransform>().rect);
*/
    }
    private void Update(){

        if (Input.GetKeyDown(KeyCode.I)){
            // SHow Last Message
            ToggleInventoryDisplay();
        }
    }
    private void ToggleInventoryDisplay( bool show ){
        _inventoryShowing = show;
        
        if ( !show ) {
            // Hide Inventory
            _inventoryPanel.SetActive(false);
        } else {
            // Show Inventory
            _inventoryPanel.SetActive(true);
        }
    }
    private void ToggleInventoryDisplay(){
        Debug.Log("Overload ToggleInventoryDisplay()");
        ToggleInventoryDisplay( !_inventoryShowing );
    }
    public bool HasItemByName(string name){
        bool hasItem = false;
        for( int i = 0; i < Items.Count; i++ ) {
            if(Items[i].MyName == name){
                hasItem = true;
                break;
            }
        }
        return hasItem;
    }
    public InventoryItem GetItemByName(string name){
        for( int i = 0; i < Items.Count; i++ ) {
            if(Items[i].MyName == name){
                return Items[i];
            }
        }
        return null;
    }
    public InventoryItem AddItem(string name, Sprite sprite)
    {
        // Debug.Log("InventoryManager -> AddItem("+name+", "+sprite+")");
        InventoryItem newItem = new InventoryItem(name, sprite);
        _items.Add(newItem);
        AddItemDisplay( newItem );
        _pickupSound.Play();
        return newItem;
    }
    public void RemoveItem(InventoryItem item){
        RemoveItemDisplay(item);
        _items.Remove(item);
        _pickupSound.Play();

    }
    private void AddItemDisplay( InventoryItem item) {
        
        string rowName = _items.Count < 8 ? "Row 1" : "Row 2";
        // Debug.Log("_inventoryPanel.transform.Find(rowName):: "+_inventoryPanel.transform.Find("Rows/"+rowName));
        GameObject row = _inventoryPanel.transform.Find("Rows/"+rowName).gameObject;
        GameObject newSlot = Instantiate(inventorySlotPrefab);
        newSlot.transform.SetParent(row.transform);
        newSlot.transform.localScale = new Vector3(1,1,1); // without this, scale changes to 0.88... ?
        newSlot.transform.localPosition = Vector3.zero;
        newSlot.transform.localRotation = Quaternion.identity;
        Image itemSprite = newSlot.transform.Find("ItemSprite").gameObject.GetComponent<Image>();
        itemSprite.sprite = item.MySprite;
        // Debug.Log("itemSprite :: "+itemSprite);
        // Debug.Log("itemSprite.sprite :: "+itemSprite.sprite);
        // Debug.Log("itemSprite.rotation ==> "+itemSprite.transform.rotation);
        itemSprite.transform.SetParent(newSlot.transform);
        item.MySlot = newSlot;
    }
    private void RemoveItemDisplay(InventoryItem item){
        GameObject slot = item.MySlot;
        Destroy(slot, 1.0f);
    }
    public List<InventoryItem> Items
    {
        get => _items;
    }
}


public class InventoryItem
{
    private string _name;
    private Sprite _mySprite;
    private GameObject _mySlot;


    public InventoryItem(string name, Sprite sprite)
    {
        _name = name;
        _mySprite = sprite;
    }

    public void ShowSprite()
    {
        GameManager.Instance.ShowSprite(_mySprite);
    }

    public void PickUp()
    {

    }
    public void Use()
    {

    }
    public void AddToInventory()
    {

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
    public string MyName
    {
        get => _name;
    }
    public GameObject MySlot {
        get => _mySlot;
        set {
            _mySlot = value;
        }
    }
}