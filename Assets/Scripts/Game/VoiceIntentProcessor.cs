using UnityEngine;
using System.Collections;
using System;

public class VoiceIntentProcessor : MonoBehaviour, ISpeechToTextListener
{
    [Header("STT Settings")]
    [SerializeField] private bool voiceControlEnabled = true;
    [SerializeField] private string preferredLanguage = "en-US";
    [SerializeField] private bool useOfflineRecognition = false;
    [SerializeField] private float activationDelay = 0.05f;
    
    private RobotController robot;
    private bool isActivated = false;
    private bool isInitialized = false;
    private string lastTriggeredIntent = "";

    private void Start()
    {
        if (!voiceControlEnabled) return;

        // Initialize the STT service
        try 
        {
            Debug.Log("STT: Initializing...");
            if (SpeechToText.Initialize(preferredLanguage))
            {
                isInitialized = true;
                Debug.Log("STT: Successfully initialized.");
            }
            else
            {
                Debug.LogError("STT: Failed to initialize. Check if the platform is supported.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"STT: Initialization exception: {e.Message}");
        }
    }

    public void ActivateVoiceControl(RobotController robotController)
    {
        if (!voiceControlEnabled)
        {
            Debug.Log("STT: Voice control is disabled in the inspector.");
            return;
        }

        if (robotController == null)
        {
            Debug.LogError("STT: Received null RobotController in ActivateVoiceControl!");
            return;
        }

        robot = robotController;
        isActivated = true;
        
        Debug.Log($"STT: Activating voice control for {robot.name}...");
        StopAllCoroutines();
        StartCoroutine(DelayedActivation());
    }

    private IEnumerator DelayedActivation()
    {
        if (activationDelay > 0)
            yield return new WaitForSeconds(activationDelay);

        if (!isInitialized)
        {
            Debug.LogWarning("STT: Cannot activate because initialization failed.");
            yield break;
        }

        Debug.Log("STT: Requesting permissions...");
        try
        {
            SpeechToText.RequestPermissionAsync((permission) => {
                Debug.Log($"STT: Permission result = {permission}");
                if (permission == SpeechToText.Permission.Granted)
                {
                    StartListening();
                }
                else
                {
                    Debug.LogError($"STT: Microphone permission {permission} denied.");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"STT: Permission request exception: {e.Message}");
        }
    }

    public void DeactivateVoiceControl()
    {
        isActivated = false;
        StopAllCoroutines();
        try
        {
            SpeechToText.Cancel();
        }
        catch (Exception e) { Debug.LogWarning($"STT: Cancel error: {e.Message}"); }
        Debug.Log("STT: Deactivated");
    }

    private void StartListening()
    {
        if (!isActivated) return;

        try
        {
            if (SpeechToText.IsBusy())
            {
                // If it's busy, try again very soon
                Invoke(nameof(StartListening), 0.1f);
                return;
            }

            lastTriggeredIntent = "";
            Debug.Log("STT: Calling SpeechToText.Start()...");
            SpeechToText.Start(this, useOfflineRecognition);
        }
        catch (Exception e)
        {
            Debug.LogError($"STT: StartListening exception: {e.Message}");
        }
    }

    #region ISpeechToTextListener Implementation

    public void OnReadyForSpeech()
    {
        Debug.Log("STT: System ready for speech");
    }

    public void OnBeginningOfSpeech()
    {
        Debug.Log("STT: User started speaking");
    }

    public void OnVoiceLevelChanged(float level) { }

    public void OnPartialResultReceived(string text) 
    {
        if (!string.IsNullOrEmpty(text))
        {
            ProcessText(text.ToLower());
        }
    }

    public void OnResultReceived(string text, int? errorCode)
    {
        if (errorCode.HasValue)
        {
            Debug.LogError($"STT: Received error code = {errorCode.Value}");
            
            if (isActivated)
            {
                Invoke(nameof(StartListening), 0.5f);
            }
            return;
        }

        if (!string.IsNullOrEmpty(text))
        {
            Debug.Log($"STT: Final Result: <color=cyan>{text}</color>");
            ProcessText(text.ToLower());
        }

        if (isActivated)
        {
            // Use a very small delay to ensure the engine has fully stopped before restarting
            Invoke(nameof(StartListening), 0.05f);
        }
    }

    #endregion

    private void ProcessText(string text)
    {
        if (robot == null) return;

        string intent = "";
        if (text.Contains("jab") || text.Contains("one")) intent = "Jab";
        else if (text.Contains("hook") || text.Contains("three")) intent = "Hook";
        else if (text.Contains("uppercut") || text.Contains("two") || text.Contains("four")) intent = "Uppercut";
        else if (text.Contains("cross")) intent = "Cross";
        else if (text.Contains("block")) intent = "Block";

        if (!string.IsNullOrEmpty(intent))
        {
            // If we already triggered this intent in the current listening session, skip
            if (intent == lastTriggeredIntent) return;

            bool success = false;
            switch (intent)
            {
                case "Jab": success = robot.TriggerJab(); break;
                case "Hook": success = robot.TriggerHook(); break;
                case "Uppercut": success = robot.TriggerUppercut(); break;
                case "Cross": success = robot.TriggerCross(); break;
                case "Block": success = robot.TriggerBlock(); break;
            }

            if (success)
            {
                lastTriggeredIntent = intent;
                Debug.Log($"STT: Action -> {intent} (Triggered)");
            }
        }
    }

    private void OnDestroy()
    {
        DeactivateVoiceControl();
    }
}
