using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulatorObject : MonoBehaviour
{
    public SimulatorCameraController cameraController;
    public GameObject simulatorUI;
    public GameObject successPopup;
    public Text moveCountTxt;
    public MapLoader simulatingMap;
    // Start is called before the first frame update
    public void Adapting()
    {
        Simulator.instance.cameraController = cameraController;
        Simulator.instance.successPopup = successPopup;
        Simulator.instance.simulatorUI = simulatorUI;
        Simulator.instance.MoveCountTxt = moveCountTxt;
        Simulator.instance.simulatingMap = simulatingMap;

    }

    public void Reloading()
    {

    }
}
