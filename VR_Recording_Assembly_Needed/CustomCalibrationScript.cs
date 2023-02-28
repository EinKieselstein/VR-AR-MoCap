#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.VisualScripting;
using RootMotion.Demos;
using R1Tools.Core;

public class CustomCalibrationScript : CalibratorController
{
    [SerializeField] bool useMultipleCharsInScene;

    private SwitchCharacters switchCharScript;

    public GameObject[] allAvatars = new GameObject[0];

    [SerializeField] bool calibrateFromInspector = false;

    private bool doneCalibratingCurrentAvatar = false;

    [SerializeField] private bool recalibrateAllAvatars = false;

    [HideInInspector]
    public bool allCharsCalibrated = false;


    private bool triggerPressed;

    private int currentAvatarIndex = 0;

    private void Start()
    {
        if(useMultipleCharsInScene) switchCharScript = GetComponent<SwitchCharacters>();
    }

    void LateUpdate()
    {
        if (calibrateFromInspector && !allCharsCalibrated)
        {
            NotifyCalibration();
            calibrateFromInspector = false;
        }
        
        if (useMultipleCharsInScene)
        {
            
            if (recalibrateAllAvatars)
            {
                Debug.Log("recalibrate avatars");
                allCharsCalibrated = false;
                doneCalibratingCurrentAvatar = false;
                currentAvatarIndex = 0;
                switchCharScript.SwitchCharacter(allAvatars[currentAvatarIndex]);
                recalibrateAllAvatars = false;
            }

            if (!allCharsCalibrated && doneCalibratingCurrentAvatar)
            {
                doneCalibratingCurrentAvatar = false;

                if (currentAvatarIndex == allAvatars.Length)
                {
                    allCharsCalibrated = true;
                    Debug.Log("Done calibrating all avatars initially");
                    switchCharScript.switchToNextCharacter = true; ;
                    return;
                }
                else
                {
                    Debug.Log("done with current avatar, switching to next");
                    switchCharScript.SwitchCharacter(allAvatars[currentAvatarIndex]);
                }
            }
            
        }

        if (triggerPressed)
        {
            triggerPressed = false;
            
            if (!allCharsCalibrated)
            {
                calibrate();

                InitialCalibrations();
            }
        }
    }
    
    public new void calibrate()
        {
            if (ignoreWaist)
            {
                if (leftFootTracker != null && rightFootTracker != null && leftFootTracker.isTracked == true && rightFootTracker.isTracked == true)
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, false, null,null, leftHandTracker, rightHandTracker, null ,null,null,null,leftFootTracker.gameObject.transform, rightFootTracker.gameObject.transform);
                }
                else
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, true, null,null, leftHandTracker, rightHandTracker, null ,null,null,null,null, null);
                }
            }
            else
            {
                if (leftFootTracker != null && rightFootTracker != null && leftFootTracker.isTracked == true && rightFootTracker.isTracked == true)
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, false, bodyTracker.gameObject.transform,chestTracker.gameObject.transform, leftHandTracker, rightHandTracker, null ,null,null,null,leftFootTracker.gameObject.transform, rightFootTracker.gameObject.transform);
                }
                else
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, true, null,null, leftHandTracker, rightHandTracker,null ,null,null,null,null, null);
                }
            }

            trackerScript.onCalibrate(); 
        }

    public void NotifyCalibration()
    {
        Debug.Log("in Calibration");
        triggerPressed = true;
    }

    public void DoneCalibratingCurrent()
    {
        doneCalibratingCurrentAvatar = true;
        
        currentAvatarIndex += 1;
    }


    void InitialCalibrations()
    {
        bool componentAvailable = ik.gameObject.TryGetComponent<IndividualCalibrationData>(out IndividualCalibrationData avatarCalibScr);

        if(componentAvailable)
        {
            avatarCalibScr.avatarCalibrationData = data;
        }
        else
        {
            Debug.Log("no individual calibration data script on avatar, only one avatar usable");
        }

    }
}
#endif
