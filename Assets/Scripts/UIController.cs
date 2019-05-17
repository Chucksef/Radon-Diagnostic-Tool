using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public QRCodeDecodeController cont;
    public Text field;
    public GameObject Flash;
    public Image ttf;

    public Color redish;
    public Color greenish;

    IEnumerator Wait1sec()
    {
        var t = Time.time+10f;
        ttf.color = redish;
        Flash.SetActive(true);
        yield return new WaitForSeconds(.25f);
        Flash.SetActive(false);
        yield return new WaitForSeconds (9.75f);
        if(Time.time >= t)
        {
            field.text = "Searching...";
            cont.StartWork();
            ttf.color = greenish;
        }
    }

	// Use this for initialization
	void Start () {
        cont.onQRScanFinished += GetResult;
        field.text = "Searching...";
	}

    void GetResult(string resultStr)
    {
        field.text = resultStr;
        StartCoroutine(Wait1sec());
    }
}
