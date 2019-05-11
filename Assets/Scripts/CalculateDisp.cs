using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateDisp : MonoBehaviour {

    public GameObject velo;
    public GameObject angle;
    public GameObject grav;

    public void CalculateDisplacement ()
    {
        var g = grav.GetComponent<InputField>().text;
        var v = velo.GetComponent<InputField>().text;
        var a = angle.GetComponent<InputField>().text;

        float gin;
        float vin;
        float ain;

        if (g == "")
        {
            gin = 9.8f;
        } else
        {
            gin = float.Parse(g);
            gin = Mathf.Abs(gin);
        }

        if (v == "")
        {
            vin = 0.0f;
        } else
        {
            vin = float.Parse(v);
        }

        if (a == "")
        {
            ain = 0.0f;
        }
        else
        {
            ain = float.Parse(a);
        }

        Vector2 vin2 = new Vector2(vin * Mathf.Cos(ain * (Mathf.PI / 180f)), vin * Mathf.Sin(ain * (Mathf.PI / 180f)));

        Debug.Log("Gravity : " + gin);
        Debug.Log("Velocity : " + vin);
        Debug.Log("Angle : " + ain);
        Debug.Log("X Velocity: " + vin2.x);
        Debug.Log("Y Velocity: " + vin2.y);

        float time = (2 * vin2.y) / gin;

        Debug.Log("Time airborn: " + time);

        float dist = time * vin2.x;

        Debug.Log("Distance Travelled: " + dist);
        //var calcResult = gin + " " + ain + " " + vin;
        var txt = gameObject.GetComponent<Text>();

        txt.text = Mathf.Round(dist).ToString();

    }
}
