using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class readOption : MonoBehaviour {

    /// <summary>
    /// Public Variables go here!!!
    /// </summary>
    [Header("Panels")]
    public GameObject pnl_BuildInfo;
    public GameObject pnl_RiserInfo;
    public GameObject pnl_PipeInfo;
    public GameObject pnl_ResultsInfo;
    public GameObject pnl_Results;
    public GameObject pnl_Blur;

    [Header("Building Info Fields")]
    public InputField in_AirFlowFactor;
    public InputField in_TreatmentArea;
    public InputField out_AirFlow;
    public InputField in_MaxAirVelocityTarget;
    public InputField in_MaxAirLeeway;
    public InputField out_MaxAirFlowLimit;
    public InputField in_MinVacuum;

    [Header("Riser Info Fields")]
    public InputField in_PipeLength;
    public InputField in_Elbows;
    public InputField in_45s;
    public InputField in_RainCaps;

    //[Header("Pipe Size Dropdown")]
    //public Dropdown in_PipeDropdown;
    [Header("Pipe Sizes")]
    public int[] pipeSizes;

    [Header("Results Matrix Fields")]
    public InputField[] out_SystemsNeeded;
    public InputField[] out_OuterDiameter;
    public InputField[] out_RiserAirVelocity;
    public InputField[] out_CFMPerSystem;
    public InputField[] out_FanDPNeeded;



    /// <summary>
    /// Private Variables Go Here!!!
    /// </summary>
    int dropValue;                                                                  //variable to store the dropdown menu selection as an array index

    float i_AirFlowFactor;                                                          //variable for Air Flow
    float i_TreatmentArea;                                                          //variable for Treatment Area
    float o_AirFlow;                                                                //calculated variable for AirFlow
    float i_MaxAirVelocityTarget;                                                   //variable for Max Air Velocity Target
    float i_MaxAirLeeway;                                                           //variable for Max Air Leeway
    float o_MaxAirFlowLimit;                                                        //calculated variable for Max Air Flow Limit
    float i_MinVacuum;                                                              //variable for Minimum Vacuum @ Pit

    float i_PipeLength;                                                             //variable for pipe length
    float i_Elbows;                                                                 //variable for elbows
    float i_45s;                                                                    //variable for 45s
    float i_RainCaps;                                                               //variable for Rain Cap

    float[] o_OuterDiameter = new float[5];                                         //variable for Pipe Diameter - Outer
    float[] o_WallThick = new float[] { .216f, .237f, .280f, .322f, .365f };        //variable for Wall Thickness

    float[] o_InsideDiameter = new float[5];                                        //calculated variable for Pipe Diameter - Inner
    float[] o_PipeSqFt = new float[5];                                              //calculated variable for Pipe Square Footage
    float[] o_SystemsNeeded = new float[5];                                         //calculated variable for # of Systems Needed
    float[] o_RiserAirVelocity = new float[5];                                      //calculated variable for Air Velocity / Riser
    float[] o_CFMPerSystem = new float[5];                                          //calculated variable for CFM Per System
    float[] o_TEL = new float[5];                                                   //calculated variable for Total Equivalent Length
    float[] o_FanDPNeeded = new float[5];                                           //calculated variable for Minimum Differential Pressure

    InputField[] inputs = new InputField[11];

    ColorBlock badCB = new ColorBlock();
    ColorBlock tempCB = new ColorBlock();
    ColorBlock goodCB = new ColorBlock();


    // Use this for initialization
    void Start() {
        inputs[0] = in_AirFlowFactor;
        inputs[1] = in_TreatmentArea;
        inputs[2] = out_AirFlow;
        inputs[3] = in_MaxAirVelocityTarget;
        inputs[4] = in_MaxAirLeeway;
        inputs[5] = out_MaxAirFlowLimit;
        inputs[6] = in_MinVacuum;
        inputs[7] = in_PipeLength;
        inputs[8] = in_Elbows;
        inputs[9] = in_45s;
        inputs[10] = in_RainCaps;

        goodCB = in_Elbows.colors;
        badCB = in_Elbows.colors;
        badCB.normalColor = new Color(1, 0.2980392f, 0.2980392f);
        badCB.fadeDuration = 0f;
        badCB.highlightedColor = new Color(1, 0.2980392f, 0.2980392f);
        badCB.disabledColor = new Color(1, 0.2980392f, 0.2980392f);

        //The following block gets the pipe details panel's text boxes, then does shit with them...
        //Transform tempBox = pnl_ResultsInfo.GetComponent<Transform>();
        //var tempTexts = tempBox.GetComponentsInChildren<Text>();

        //for (int i = 0; i < tempTexts.Length; i++)
        //{
        //    tempTexts[i].text = i.ToString();
        //}

    }


    // Function to get the Outer Diameter and Wall Thickness based on the dropdown selection, 
    // which form the basis for the rest of the data, but can't be calculated themselves
    //
    // This function is called when the Calculate button is pressed, and when the dropdown menu selection changes.

    public void FlyInRight(GameObject flyMe) {
        Animator anim = flyMe.GetComponent<Animator>();
        anim.Play("Fly In Right");
    }

    IEnumerator NormalColor(InputField obj)
    {
        Debug.Log("Coroutine starting at "+Time.time);
        tempCB = badCB;
        obj.colors = tempCB;
        tempCB = goodCB;
        tempCB.fadeDuration = 1.5f;
        obj.colors = tempCB;
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Coroutine finished at " + Time.time);
        obj.colors = goodCB;
        //for (int i = 0; i < 1; i++)
        //{


        //    if(i == 0)
        //    {
        //        Debug.Log("Color = red");
        //        obj.colors = badCB;
        //    }
        //    else if(i == 1)
        //    {
        //        Debug.Log("Color = white");
        //        obj.colors = goodCB;
        //    }
        //yield return new WaitForSeconds(1);
        //}
    }

    public void FlyOutRight(GameObject pnl1)
    {
        pnl_Blur.SetActive(false);
        pnl1.GetComponent<Animator>().Play("Fly Out Right");
    }

    public void ResetFields()
    {
        //Resets all Main fields to default values "";
        foreach (var input in inputs)
        {
            input.text = "";
            input.colors = goodCB;
        }
    }

    public void DefaultValues()
    {
        //Resets all Main fields to default values "";
        in_AirFlowFactor.text = "0.020";
        in_TreatmentArea.text = "30000";
        in_MaxAirVelocityTarget.text = "700";
        in_MaxAirLeeway.text = "10";
        in_MinVacuum.text = "0.75";
        in_PipeLength.text = "32";
        in_Elbows.text = "10";
        in_45s.text = "1";
        in_RainCaps.text = "1";

        UpdateAirFlow();
        UpdateLimit();

    }

    public void UpdateAirFlow()
    {
        if (float.Parse(in_AirFlowFactor.text) > 0 && float.Parse(in_TreatmentArea.text) > 0)
        {
            i_AirFlowFactor = float.Parse(in_AirFlowFactor.text);
            i_TreatmentArea = float.Parse(in_TreatmentArea.text);
            o_AirFlow = i_AirFlowFactor * i_TreatmentArea;
            out_AirFlow.text = o_AirFlow.ToString();
        }
    }

    public void UpdateLimit()
    {
        if (float.Parse(in_MaxAirVelocityTarget.text) > 0 && float.Parse(in_MaxAirLeeway.text) > 0)
        {
            i_MaxAirVelocityTarget = float.Parse(in_MaxAirVelocityTarget.text);
            i_MaxAirLeeway = float.Parse(in_MaxAirLeeway.text);
            o_MaxAirFlowLimit = i_MaxAirVelocityTarget * (1 + (i_MaxAirLeeway/100));
            out_MaxAirFlowLimit.text = o_MaxAirFlowLimit.ToString();
        }
    }

    public void runData(GameObject panel)
    {
        bool formFilledOut = true;

        foreach (var x in inputs)
        {
            if (x.text == "")
            {
                formFilledOut = false;
                Debug.Log("Starting Coroutine");
                StartCoroutine(NormalColor(x));
            }
        }


        if(formFilledOut)
        {
            SetBuildingInfo();
            UpdateMatrix();
            pnl_Blur.SetActive(true);
            panel.SetActive(true);
            FlyInRight(panel);
        }

    }

    // Feeds input text into variables
    public void SetBuildingInfo()
    {
        i_AirFlowFactor = float.Parse(in_AirFlowFactor.text);
        i_MaxAirVelocityTarget = float.Parse(in_MaxAirVelocityTarget.text);
        o_MaxAirFlowLimit = float.Parse(out_MaxAirFlowLimit.text);
        i_MinVacuum = float.Parse(in_MinVacuum.text);

        i_PipeLength = float.Parse(in_PipeLength.text);
        i_Elbows = float.Parse(in_Elbows.text);
        i_45s = float.Parse(in_45s.text);
        i_RainCaps = float.Parse(in_RainCaps.text);

    }

    public void UpdateMatrix()
    {
        //Loop over the length of the SystemsNeeded Array, doing calculations and filling in each box in the column as we go...
        for (int i = 0; i < out_SystemsNeeded.Length; i++)
        {

            o_OuterDiameter[i] = float.Parse(out_OuterDiameter[i].text); //First get the value of the outer diameters for each Pipe Size
            o_InsideDiameter[i] = o_OuterDiameter[i] - (2 * o_WallThick[i]);
            o_PipeSqFt[i] = (Mathf.PI * Mathf.Pow(o_InsideDiameter[i] / 2, 2)) / 144;
            o_SystemsNeeded[i] = Mathf.Max(1f, (Mathf.Round(o_AirFlow / o_PipeSqFt[i] / i_MaxAirVelocityTarget)));
            o_RiserAirVelocity[i] = o_AirFlow / (o_PipeSqFt[i] * o_SystemsNeeded[i]);
            o_CFMPerSystem[i] = o_AirFlow / o_SystemsNeeded[i];
            o_TEL[i] = i_PipeLength + (i_Elbows * 30 * o_InsideDiameter[i] / 12) + (i_45s * 16 * o_InsideDiameter[i] / 12) + (i_RainCaps * 60 * o_InsideDiameter[i] / 12);
            o_FanDPNeeded[i] = i_MinVacuum + (0.109136f * Mathf.Pow(o_CFMPerSystem[i], 1.9f) / Mathf.Pow(o_InsideDiameter[i], 5.02f) * (o_TEL[i]) / 100);

            //Set each inputfield to its corresponding variable.
            out_OuterDiameter[i].text = o_OuterDiameter[i].ToString();
            out_SystemsNeeded[i].text = o_SystemsNeeded[i].ToString();
            out_RiserAirVelocity[i].text = Mathf.Round(o_RiserAirVelocity[i]).ToString();
            out_CFMPerSystem[i].text = (Mathf.Round(o_CFMPerSystem[i]*1000f)/1000f).ToString();
            out_FanDPNeeded[i].text = (Mathf.Round(o_FanDPNeeded[i]*1000f)/1000f).ToString();

            //Check if Riser Air Velocity is too large...
            var oldColors = out_RiserAirVelocity[i].colors;
            var goodColors = oldColors;

            //...If so, set that field's color to red
            if (o_RiserAirVelocity[i] > o_MaxAirFlowLimit)
            {
                out_SystemsNeeded[i].colors = badCB;
                out_OuterDiameter[i].colors = badCB;
                out_RiserAirVelocity[i].colors = badCB;
                out_CFMPerSystem[i].colors = badCB;
                out_FanDPNeeded[i].colors = badCB;
            }
            else
            {
                out_SystemsNeeded[i].colors = goodCB;
                out_OuterDiameter[i].colors = goodCB;
                out_RiserAirVelocity[i].colors = goodCB;
                out_CFMPerSystem[i].colors = goodCB;
                out_FanDPNeeded[i].colors = goodCB;
            }

        }

    }

}
