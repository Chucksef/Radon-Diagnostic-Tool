using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateScripts : MonoBehaviour {

    public GameObject AFF;
    public GameObject TA;
    public GameObject MAVT;
    public GameObject MAL;

    public void updateAirFlow()
    {
        var affText = AFF.GetComponent<InputField>().text;
        var taText = TA.GetComponent<InputField>().text;
        var output = gameObject.GetComponentInChildren<Text>();


        float aff;
        float ta;

        if(affText == "" || taText == "")
        {

        } else
        {
            aff = float.Parse(affText);
            ta = float.Parse(taText);

            float mult = ta * aff;
            output.text = mult.ToString();

        }
    }

    public void updateFlowLimit()
    {
        var mavtText = MAVT.GetComponent<InputField>().text;
        var malText = MAL.GetComponent<InputField>().text;
        var output = gameObject.GetComponentInChildren<Text>();

        float mavt;
        float mal;

        if (mavtText == "" || malText == "")
        {

        }
        else
        {
            mavt = float.Parse(mavtText);
            mal = float.Parse(malText);

            float val = mavt * (1 + (mal/100));
            output.text = val.ToString();

        }

    }

}
