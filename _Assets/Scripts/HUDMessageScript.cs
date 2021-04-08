using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

public class HUDMessageScript : MonoBehaviour
{
    public Text HUDTextField;
    private Coroutine messageCo;
    // Start is called before the first frame update
    void Start()
    {
        HUDTextField.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMessage(string message)
    {
        HUDTextField.gameObject.SetActive(true);
        HUDTextField.text = message;
        if (messageCo != null)
            StopCoroutine(messageCo);
        messageCo = StartCoroutine(HideMessage());
    }
    IEnumerator HideMessage(float dur = 2f)
    {
        yield return new WaitForSeconds(dur);
        HUDTextField.gameObject.SetActive(false);
    }
}
