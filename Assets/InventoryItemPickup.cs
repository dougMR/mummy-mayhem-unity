using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemPickup : MonoBehaviour
{

    public string myName;
    public Sprite mySprite;
    // private InventoryItem _myItem;
    // private string _spritePath;


    // Start is called before the first frame update
    void Start()
    {

        /*
        _spritePath = AssetDatabase.GetAssetPath(mySprite);
        int indexStart = _spritePath.IndexOf("Resources/") + 10;
        int indexEnd = _spritePath.IndexOf(".");
        int length = indexEnd - indexStart;
        _spritePath = _spritePath.Substring(indexStart, length);
        */
        // _myItem = new InventoryItem(name, spritePath);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            // Debug.Log("ItemPickup.OnTriggerEnter()");
            Pickup();
        }
    }

    private void Pickup()
    {
        // Add this item to Player's inventory
        InventoryItem item = InventoryManager.Instance.AddItem(myName, mySprite);
        // Flash item's sprite on screen
        item.ShowSprite();
        // GameManager.Instance.ShowMessage("You picked up the "+myName, 0.5f);
        // Better not to announce events that can be seen?
        Destroy(gameObject);
    }
}
