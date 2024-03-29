using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class mainController : MonoBehaviour {

    //ALL PUBLIC VARIABLES HERE
    [Range(.1f, 1.25f)]
    public float animTime = .25f;
    [Range(.1f, 2.5f)]
    public float warnTime = 1.25f;

    [Header("Canvas")]
    public Canvas cv;

    [Header("Panels")]
    public GameObject pnl_BuildInfo;
    public GameObject pnl_RiserInfo;
    public GameObject pnl_Results;
    public GameObject pnl_Blur;
    public GameObject pnl_Main;
    public GameObject pnl_FanOptions;

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

    [Header("Fan Options Fields")]
    public Text fanPipeSize;
    public Text fanAirFlow;
    public Text fanMinDP;

    [Header("Pipe Sizes")]
    public int[] pipeSizes;

    [Header("Results Matrix Fields")]
    public InputField[] out_SystemsNeeded;
    public InputField[] out_OuterDiameter;
    public InputField[] out_RiserAirVelocity;
    public InputField[] out_CFMPerSystem;
    public InputField[] out_FanDPNeeded;
    public Toggle[] in_SizeToggles;

    [Header("Animation Curves")]
    public AnimationCurve easeAccel = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public AnimationCurve easeDecel = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    //ALL PRIVATE VARIABLES HERE
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

    InputField[] Inputs = new InputField[11];

    int selectedCol = 0;
    float selectedPipeSize;
    float selectedAirFlow;
    float selectedMinDP;

    Color warnColor = new Color(1, 0.2980392f, 0.2980392f);
    Color selectedColor = new Color(1f, 1f, 1f);
    ColorBlock warnCB = new ColorBlock();
    ColorBlock goodCB = new ColorBlock();
    ColorBlock selectedCB = new ColorBlock();


    //runs at start
    void Start() {
        Screen.fullScreen = false;

        //Place the linked inputs into one array of all inputs
        Inputs[0] = in_AirFlowFactor;
        Inputs[1] = in_TreatmentArea;
        Inputs[2] = out_AirFlow;
        Inputs[3] = in_MaxAirVelocityTarget;
        Inputs[4] = in_MaxAirLeeway;
        Inputs[5] = out_MaxAirFlowLimit;
        Inputs[6] = in_MinVacuum;
        Inputs[7] = in_PipeLength;
        Inputs[8] = in_Elbows;
        Inputs[9] = in_45s;
        Inputs[10] = in_RainCaps;

        //These lines store new colorblock data into goodCB and warnCB
        goodCB = in_Elbows.colors; //both based on existing colorblock...

        selectedCB = in_Elbows.colors;
        selectedCB.normalColor = selectedColor;
        selectedCB.fadeDuration = 0f;
        selectedCB.highlightedColor = selectedColor;
        selectedCB.disabledColor = selectedColor;

        warnCB = in_Elbows.colors;
        warnCB.normalColor = warnColor;
        warnCB.fadeDuration = 0f;
        warnCB.highlightedColor = warnColor;
        warnCB.disabledColor = warnColor;

        StartCoroutine(DelayedPositionOnLaunch());
    }


    IEnumerator DelayedPositionOnLaunch()
    {
        yield return new WaitForSeconds(.2f);
        StartPosition(pnl_Results);
        StartPosition(pnl_FanOptions);
    }

    ///<summary>
    ///Animates RectTransform from its current Position to specified coordinates (endPos)
    ///</summary>
    IEnumerator FlyIn(RectTransform rt, Vector2 endPos, float delay){
        Vector2 startPos = rt.localPosition;
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);                                     //delays based on float
        }
        for (float i = 0f; i < animTime; i = i+Time.deltaTime)
        {
            var timePCT = i / animTime;
            Vector2 currentPos = new Vector2(Mathf.Lerp(startPos.x,endPos.x, easeDecel.Evaluate(timePCT)), Mathf.Lerp(startPos.y, endPos.y, easeDecel.Evaluate(timePCT)));
            rt.localPosition = new Vector2(currentPos.x ,currentPos.y);
            yield return null;
        }
        rt.localPosition = endPos;
    }

    ///<summary>
    ///Animates RectTransform from its current position to out of screen in whichever direction specified.
    ///</summary>
    IEnumerator FlyOut(RectTransform rt, string dir)
    {
        //RectTransform objects to store the transforms of the main panel
        var rt_main = pnl_Main.GetComponent<RectTransform>();

        var rect_this = RectTransformUtility.PixelAdjustRect(rt, cv);                           //Rect object to store rt's height and width
        var rect_main = RectTransformUtility.PixelAdjustRect(rt_main, cv);                      //Rect object to store main panel's height and width

        Vector2 startPos = rt.localPosition;                                                    //Start Position is defined as where the panel starts
        Vector2 endPos;                                                                         //declares variable to set later

        dir = dir.ToLower();            

        //Some math to figure out the rightPos (offscreen)
        switch (dir)
        {
            case "up":
            case "top":
            case "north":
                endPos.x = 0f;
                endPos.y = rect_main.height + (rect_this.height - rect_main.height) / 2;
                break;

            case "right":
            case "east":
                endPos.x = rect_main.width + (rect_this.width - rect_main.width) / 2;
                endPos.y = 0f;
                break;

            case "down":
            case "bottom":
            case "south":
                endPos.x = 0f;
                endPos.y = -(rect_main.height + (rect_this.height - rect_main.height) / 2);
                break;

            case "left":
            case "west":
                endPos.x = -(rect_main.height + (rect_this.height - rect_main.height) / 2);
                endPos.y = 0f;
                break;

            default:
                endPos.x = 5000f;
                endPos.y = 5000f;
                Debug.Log("FlyOut(): No Valid Direction Declared");
                break;
        }

        for (float i = 0f; i < animTime; i = i + Time.deltaTime)
        {
            var timePCT = i / animTime;
            Vector2 currentPos = new Vector2(Mathf.Lerp(startPos.x, endPos.x, easeAccel.Evaluate(timePCT)), Mathf.Lerp(startPos.y, endPos.y, easeAccel.Evaluate(timePCT)));
            rt.localPosition = new Vector2(currentPos.x, currentPos.y);
            yield return null;
        }

        rt.localPosition = endPos;
    }


    IEnumerator FlashWarningColor(GameObject thisInput)
    {
        var sel = thisInput.GetComponent<Selectable>();
        var lastCalcTime = Time.time;
        sel.colors = warnCB; //sets sel's colorblock to be equivalent to the warnCB
        var tempCB = goodCB;
        tempCB.fadeDuration = warnTime;
        sel.colors = tempCB;
        yield return new WaitForSeconds(warnTime);
        if(lastCalcTime+warnTime >= Time.time)
        {
            sel.colors = goodCB;
        }
    }

    private void StartPosition(GameObject sss)
    {
        //The following code places the passed game-object panel at the right-outside edge of the frame, centered vertically.
        //RectTransform objects to store the transforms of the 2 panels
        var rt_sss = sss.GetComponent<RectTransform>();
        var rt_main = pnl_Main.GetComponent<RectTransform>();

        //Rect objects to store the width/height of the 2 panels
        var rect_results = RectTransformUtility.PixelAdjustRect(rt_sss, cv);
        var rect_main = RectTransformUtility.PixelAdjustRect(rt_main, cv);

        //Some math to figure out the rightPos (offscreen)
        var rightPos = rect_main.width + (rect_results.width - rect_main.width) / 2;
        rt_sss.localPosition = new Vector3(rightPos, 0, 0);
    }

    //returns to the main menu
    public void MainMenu(GameObject thisPnl)
    {
        var rt_pnl = thisPnl.GetComponent<RectTransform>();
        pnl_Blur.SetActive(false);
        StartCoroutine(FlyOut(rt_pnl, "right"));
    }

    //runs when the Reset button is pushed
    public void ResetFields()
    {
        //Resets all Main fields to default values "";
        foreach (var input in Inputs)
        {
            input.text = "";
            input.colors = goodCB;
        }
    }

    //runs when the Default button is pushed
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

    //runs when the values in 1st and 2nd input boxes change
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

    //runs when the values in the 4th and 5th input boxes change
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

    //runs when the Calculate button is pushed
    public void runData(GameObject thisPnl)
    {
        //reset Results in_SizeToggles[i] Toggles
        in_SizeToggles[0].isOn = true;

        selectedPipeSize = 0;
        selectedCol = 0;

        bool formFilledOut = true; //saves variable to check if form is filled out...
        foreach (var x in Inputs) //check all Inputs fields in the Inputs[] array...
        {
            if (x.text == "") // if this input field has no text, that input field is bad, we can't proceed
            {
                var y = x.gameObject;
                formFilledOut = false;
                StartCoroutine(FlashWarningColor(y));
            }
        }

        if(formFilledOut)
        {
            var rt_pnl = thisPnl.GetComponent<RectTransform>();
            SetBuildingInfo();
            UpdateMatrix();
            pnl_Blur.SetActive(true);
            thisPnl.SetActive(true);
            StartCoroutine(FlyIn(rt_pnl, new Vector2(0,0), 0f));
        }
    }

    //feeds input text into variables
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

    //updates the results matrix based on the inputs from the main interface
    private void UpdateMatrix()
    {
        //Loop over the length of the SystemsNeeded Array of input boxes (for outputs), doing calculations and filling in each box in the column as we go...
        for (int i = 0; i < out_SystemsNeeded.Length; i++)
        {
            //Do the actual calculations for each variable....
            o_OuterDiameter[i] = float.Parse(out_OuterDiameter[i].text); //Get Outer Diameter into a variable
            o_InsideDiameter[i] = o_OuterDiameter[i] - (2 * o_WallThick[i]);
            o_PipeSqFt[i] = (Mathf.PI * Mathf.Pow(o_InsideDiameter[i] / 2, 2)) / 144;
            o_SystemsNeeded[i] = Mathf.Max(1f, (Mathf.Round(o_AirFlow / o_PipeSqFt[i] / i_MaxAirVelocityTarget)));
            o_RiserAirVelocity[i] = o_AirFlow / (o_PipeSqFt[i] * o_SystemsNeeded[i]);

            //Check if the Riser Velocity is over the max limit...
            while (o_RiserAirVelocity[i] > o_MaxAirFlowLimit)
            {
                //while it is, increment the system count by 1 and recalculate.
                o_SystemsNeeded[i]++;
                o_RiserAirVelocity[i] = o_AirFlow / (o_PipeSqFt[i] * o_SystemsNeeded[i]);
            }

            o_CFMPerSystem[i] = o_AirFlow / o_SystemsNeeded[i];
            o_TEL[i] = i_PipeLength + (i_Elbows * 30 * o_InsideDiameter[i] / 12) + (i_45s * 16 * o_InsideDiameter[i] / 12) + (i_RainCaps * 60 * o_InsideDiameter[i] / 12);
            o_FanDPNeeded[i] = i_MinVacuum + (0.109136f * Mathf.Pow(o_CFMPerSystem[i], 1.9f) / Mathf.Pow(o_InsideDiameter[i], 5.02f) * (o_TEL[i]) / 100);

            //Set each inputfield to its corresponding variable.
            out_OuterDiameter[i].text = o_OuterDiameter[i].ToString();
            out_SystemsNeeded[i].text = o_SystemsNeeded[i].ToString();
            out_RiserAirVelocity[i].text = Mathf.Round(o_RiserAirVelocity[i]).ToString();
            out_CFMPerSystem[i].text = (Mathf.Round(o_CFMPerSystem[i] * 1000f) / 1000f).ToString();
            out_FanDPNeeded[i].text = (Mathf.Round(o_FanDPNeeded[i] * 1000f) / 1000f).ToString();
        }
    }

    //runs when a column toggle is pressed
    public void SubmitPipeSize(GameObject fanPnl)
    {
        bool formFilledOut = false; //Saves variable to check if form is filled out.
        if (selectedCol >= 0) // if a column is selected...
        {
            formFilledOut = true; //...the form IS filled out.
        }

        if (formFilledOut) //if the form is filled out...
        {
            var rt_fanPnl = fanPnl.GetComponent<RectTransform>();
            SetFanSpecs();
            fanPnl.SetActive(true);
            StartCoroutine(FlyOut(pnl_Results.GetComponent<RectTransform>(), "right")); //...calls FlyOut() on the panel passed into this function...
            StartCoroutine(FlyIn(rt_fanPnl, new Vector2(0, 0), .2f)); //...and calls FlyIn() on the fan Panel.
        }
        else //do this if the form is NOT filled out
        {
            for (int i = 0; i < 5; i++) //for each toggle, run FlashWarningColor().
            {
                StartCoroutine(FlashWarningColor(in_SizeToggles[i].gameObject));
            }
        }
    }

    private void SetFanSpecs()
    {
        fanPipeSize.text = selectedPipeSize.ToString();
        fanAirFlow.text = (Mathf.Round(selectedAirFlow*10f)/10f).ToString();
        fanMinDP.text = (Mathf.Round(selectedMinDP*100f)/100f).ToString();
    }

    public void BackToPipeSelection(GameObject thisPnl)
    {
        StartCoroutine(FlyOut(thisPnl.GetComponent<RectTransform>(), "right"));
        StartCoroutine(FlyIn(pnl_Results.GetComponent<RectTransform>(), new Vector2(0f, 0f), .2f));
    }

    public void SelectColumn(Toggle tog)
    {
        //return all of the previously selected column to normal colors...
        if (selectedCol > -1)
        {
            out_SystemsNeeded[selectedCol].colors = goodCB;
            out_OuterDiameter[selectedCol].colors = goodCB;
            out_RiserAirVelocity[selectedCol].colors = goodCB;
            out_CFMPerSystem[selectedCol].colors = goodCB;
            out_FanDPNeeded[selectedCol].colors = goodCB;
        }

        //set the int variable to the selected Toggle's index...
        selectedCol = System.Array.IndexOf(in_SizeToggles, tog);
        selectedPipeSize = pipeSizes[selectedCol];
        selectedAirFlow = o_CFMPerSystem[selectedCol];
        selectedMinDP = o_FanDPNeeded[selectedCol];

        //set each of the columns corresponding to that index to the selected hilight color.
        out_SystemsNeeded[selectedCol].colors = selectedCB;
        out_OuterDiameter[selectedCol].colors = selectedCB;
        out_RiserAirVelocity[selectedCol].colors = selectedCB;
        out_CFMPerSystem[selectedCol].colors = selectedCB;
        out_FanDPNeeded[selectedCol].colors = selectedCB;
    } 

}
