#if UNITY_EDITOR
using R1Tools.Core;
using UnityEngine;
using RootMotion.FinalIK;

public class SwitchCharacters : MonoBehaviour
{
    [SerializeField] private CustomCalibrationScript calibrationControl;

    private TrackerAnimTool trackerTool;

    [HideInInspector]
    public bool switchToNextCharacter;

    [SerializeField] private bool useSwitchList;

    Calibrator.CalibrationData currentData = new Calibrator.CalibrationData();

    [SerializeField] GameObject nextAvatar;

    [SerializeField] private GameObject[] switchList = new GameObject[0];
    private int switchListIndex;
    
    
    void Start()
    {
        switchListIndex = switchList.Length - 1;//to start with the first avatar on the list when done with calibrations
        calibrationControl = gameObject.GetComponent<CustomCalibrationScript>();
        trackerTool = gameObject.GetComponent<TrackerAnimTool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (switchToNextCharacter)
        {
            Debug.Log("switch to next character in game");
            SwitchAndCalibrate();
            switchToNextCharacter = false;
        }
    }

    public void ActivateSwitchCharacter()
    {
        if (!calibrationControl.allCharsCalibrated)
        {
            Debug.Log("Can't switch through chars before all calibrations are done");
        }
        else
        {
            switchToNextCharacter = true;
        }

    }

    void SwitchAndCalibrate()
    {
        if (useSwitchList)
        {
            if (switchListIndex == switchList.Length - 1)
            {
                switchListIndex = 0;
                SwitchCharacter(switchList[switchListIndex]);
                CalibrateNewCharacter(switchList[switchListIndex]);  
            }
            else
            {
                switchListIndex += 1;
                SwitchCharacter(switchList[switchListIndex]);
                CalibrateNewCharacter(switchList[switchListIndex]);  
            }
        }
        else
        {
            SwitchCharacter(nextAvatar);
            CalibrateNewCharacter(nextAvatar);
        }
    }

    public void SwitchCharacter(GameObject newAvatar)
    {
        foreach(var avatar in calibrationControl.allAvatars)
        {
            avatar.SetActive(false);
        }
        
        newAvatar.SetActive(true);
        calibrationControl.ik = newAvatar.GetComponent<VRIK>();
        
        trackerTool.vrikScript = newAvatar.GetComponent<VRIK>();
        trackerTool.characterRoot = newAvatar.transform;
    }

    void CalibrateNewCharacter(GameObject newAvatar)
    {
        currentData = newAvatar.GetComponent<IndividualCalibrationData>().avatarCalibrationData;

        if (calibrationControl.leftFootTracker != null && calibrationControl.rightFootTracker != null && calibrationControl.leftFootTracker.isTracked == true && calibrationControl.rightFootTracker.isTracked == true)
        {
            Calibrator.Calibrate(newAvatar.GetComponent<VRIK>(), currentData, calibrationControl.headTracker, calibrationControl.bodyTracker.gameObject.transform,calibrationControl.chestTracker.gameObject.transform, calibrationControl.leftHandTracker, calibrationControl.rightHandTracker, null ,null,null,null,calibrationControl.leftFootTracker.gameObject.transform, calibrationControl.rightFootTracker.gameObject.transform);
        }
        else
        {
            Calibrator.Calibrate(newAvatar.GetComponent<VRIK>(), currentData, calibrationControl.headTracker, null,null, calibrationControl.leftHandTracker, calibrationControl.rightHandTracker,null ,null,null,null,null, null);
        }
    }
}
#endif
