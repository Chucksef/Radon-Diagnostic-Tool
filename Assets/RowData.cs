using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowData : MonoBehaviour {

    public Text txt_Model;
    public Text txt_Manufacturer;
    public Text txt_AirFlow;
    public Text txt_DP;
    public Text txt_DuctSize;

    private Image thisImage;
    private FanModel fanModel;
    
	void Start () {
		thisImage = gameObject.GetComponent<Image>();
	}

    public void Setup(FanModel currentFan)
    {
        fanModel = currentFan;
        txt_Model.text = fanModel.Model;
        txt_Manufacturer.text = fanModel.Manufacturer;
        txt_AirFlow.text = ((fanModel.WattsMax * 100)/100).ToString();
        txt_DP.text = fanModel.MaxPressure.ToString();
        txt_DuctSize.text = fanModel.DuctSize.ToString();
    }

}
