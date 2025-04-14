using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PhaseManager : MonoBehaviour
{
    public enum Phase { Day, Voting, Night }
    public Phase currentPhase;

    public int dayDuration = 5;
    public int votingDuration = 45;
    public int explanationDuration = 15; // Duration for explanation period
    private int explanationTimeRemaining;

    private int votesNeededCounter = 0;
    private int dayCounter = 0;
    private int nightCounter = 0;
    public int timeRemaining;
    public bool isCountingDown = false;
    private bool nightInitialized = false;
    private bool isPaused = false; // Flag to pause the countdown

    public TMP_Text phaseTitleTXT;
    public TMP_Text timerTXT;
    public TMP_Text dayNightCounterTXT;
    public TMP_Text explanationTimerTXT; // UI for explanation timer
    public TMP_Text alivePlayerCounterTXT;
    public TMP_Text deadPlayerCounterTXT;
    public TMP_Text votesNeededTXT;

    public GameObject alivePlayerScrollView;
    public GameObject graveyardScrollView;
    public GameObject alivePlayerScrollViewContent;
    public GameObject graveyardScrollViewContent;
    public GameObject startDayTimeBtn; //Start day time BTN
    public GameObject dayTimeBtn; //Go To Next Day BTN
    public GameObject pauseVotingTimeBtn;
    public GameObject resumeVotingTimeBtn; //Innocent
    public GameObject goToNightBtn; //Prosecuted
    public GameObject gameTableUI; // Reference to the GameTableUI

    private SettingsManager settingsManager;

    void Start()
    {
        settingsManager = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();
        // Check if the ClassicTableTemplateUI is active before initializing
        if (gameTableUI.activeSelf)
        {
            InitializeGame();
        }
    }

    void Update()
    {
        // Only run the game logic if ClassicTableTemplateUI is active
        if (gameTableUI.activeSelf)
        {
            switch (currentPhase)
            {
                case Phase.Day:
                    break;
                case Phase.Voting:
                    if (!isPaused)
                    {
                        UpdateVotingPhase();
                    }
                    break;
                case Phase.Night:
                    if (!nightInitialized)
                    {
                        SetNightPhase();
                        nightInitialized = true;
                    }
                    break;
            }
        }
        else
        {
            // Stop any ongoing countdowns if the UI is not active
            CancelInvoke("_tick");
            isCountingDown = false;
        }
    }

    public void SetNightPhase()
    {
        timerTXT.gameObject.SetActive(false);
        dayTimeBtn.SetActive(true);
        nightCounter++;
        dayNightCounterTXT.text = "Night " + nightCounter;
        dayNightCounterTXT.color = Color.gray;
        Debug.Log("Set Night Phase");
    }

    public void TransitionToDay()
    {
        startDayTimeBtn.SetActive(true);
        timeRemaining = dayDuration;
        phaseTitleTXT.text = "Day Phase";
        dayTimeBtn.SetActive(false);
        currentPhase = Phase.Day;
        nightInitialized = false; // Reset for the next night phase
        timerTXT.gameObject.SetActive(true);
        timerTXT.text = dayDuration.ToString() + " seconds";
        dayNightCounterTXT.text = "Day " + ++dayCounter;
        dayNightCounterTXT.color = Color.yellow;
        Debug.Log("Transitioned to Day");
    }

    public void TransitionToVoting()
    {
        phaseTitleTXT.text = "Voting Phase";
        currentPhase = Phase.Voting;
        pauseVotingTimeBtn.SetActive(true);
        CalculateVotesNeeded();
        votesNeededTXT.gameObject.SetActive(true);  
        Debug.Log("Transitioned to Voting");
    }

    public void TransitionToNight()
    {
        votesNeededTXT.gameObject.SetActive(false);
        phaseTitleTXT.text = "Night Phase";
        currentPhase = Phase.Night;
        pauseVotingTimeBtn.SetActive(false);
        Debug.Log("Transitioned to Night");
    }

    public void UpdateDayPhase()
    {
        if (!isCountingDown)
        {
            startDayTimeBtn.SetActive(false);
            Debug.Log("Started day " + dayCounter + "!");
            isCountingDown = true;
            timeRemaining = dayDuration;
            timerTXT.text = timeRemaining.ToString() + " seconds";
            Invoke("_tick", 1f);
        }
    }

    public void UpdateVotingPhase()
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            timeRemaining = votingDuration;
            timerTXT.text = timeRemaining.ToString() + " seconds";
            Invoke("_tick", 1f);
        }
    }

    // Button to pause the voting phase and start the explanation period
    public void PauseVotingPhase()
    {
        pauseVotingTimeBtn.SetActive(false);
        isPaused = true;
        votesNeededTXT.gameObject.SetActive(false);
        CancelInvoke("_tick");

        timerTXT.gameObject.SetActive(false);

        StartExplanationTimer();
        Debug.Log("Voting phase paused");
    }

    // Start the timer for the explanation period
    public void StartExplanationTimer()
    {
        phaseTitleTXT.text = "Explanation Phase";

        explanationTimeRemaining = explanationDuration;

        explanationTimerTXT.gameObject.SetActive(true);
        explanationTimerTXT.text = explanationTimeRemaining.ToString() + " seconds";
        Invoke("_explanationTick", 1f);
    }

    // Button to resume the voting phase (victim survived)
    public void ResumeVotingPhase()
    {
        phaseTitleTXT.text = "Voting Phase";
        isPaused = false;
        explanationTimerTXT.gameObject.SetActive(false); // Hide explanation timer
        timerTXT.gameObject.SetActive(true);
        pauseVotingTimeBtn.SetActive(true);
        resumeVotingTimeBtn.SetActive(false);
        goToNightBtn.SetActive(false);
        ExtendVotingTime();
        if (currentPhase == Phase.Voting && timeRemaining > 0)
        {
            isCountingDown = true;
            Invoke("_tick", 1f);
            Debug.Log("Voting phase resumed");
        }
    }

    // Button to go to night phase (victim prosecuted)
    public void GoToNightPhase()
    {
        isPaused = false;
        isCountingDown = false;
        explanationTimerTXT.gameObject.SetActive(false); // Hide explanation timer
        resumeVotingTimeBtn.SetActive(false);
        goToNightBtn.SetActive(false);
        CancelInvoke("_tick"); // Stop any remaining countdowns
        TransitionToNight();
    }

    // Button to extend voting time by 3 seconds
    private void ExtendVotingTime(int extraTime = 3)
    {
        timeRemaining += extraTime;
        timerTXT.text = timeRemaining.ToString() + " seconds";
        Debug.Log("Voting time extended by " + extraTime + " seconds");
    }

    //Tick as in seconds
    private void _tick()
    {
        if (isPaused)
        {
            return; // Stop the countdown if paused
        }

        timeRemaining--;
        if (timeRemaining > 0)
        {
            Invoke("_tick", 1f);
            timerTXT.text = timeRemaining.ToString() + " seconds";
        }
        else if (timeRemaining == 0 && currentPhase == Phase.Day)
        {
            isCountingDown = false;
            TransitionToVoting();
        }
        else if (timeRemaining == 0 && currentPhase == Phase.Voting)
        {
            isCountingDown = false;
            TransitionToNight();
        }
    }

    // Tick method for the explanation period
    private void _explanationTick()
    {
        explanationTimeRemaining--;
        if (explanationTimeRemaining > 0)
        {
            Invoke("_explanationTick", 1f);
            explanationTimerTXT.text = explanationTimeRemaining.ToString() + " seconds";
        }
        else
        {
            explanationTimerTXT.text = "Choose destiny";
            resumeVotingTimeBtn.gameObject.SetActive(true);
            goToNightBtn.gameObject.SetActive(true);
            Debug.Log("Explanation period ended");
        }
    }

    // Method to reset the entire state
    public void ResetGame()
    {
        // Stop any ongoing Invoke calls
        CancelInvoke("_tick");
        CancelInvoke("_explanationTick");

        // Reset counters and flags
        dayCounter = 0;
        nightCounter = 0;
        timeRemaining = 0;
        isCountingDown = false;
        nightInitialized = false;
        isPaused = false;

        // Reset UI
        timerTXT.text = dayDuration.ToString() + " seconds";
        timerTXT.gameObject.SetActive(true);
        dayNightCounterTXT.text = "Day " + (dayCounter + 1); // Starts at Day 1
        dayNightCounterTXT.color = Color.yellow;
        dayTimeBtn.SetActive(false);
        pauseVotingTimeBtn.SetActive(false);
        explanationTimerTXT.gameObject.SetActive(false); // Hide explanation timer

        // Reset to initial phase
        currentPhase = Phase.Day;
        Debug.Log("Game has been reset");
    }

    // Method to initialize the game (similar to what you do in Start)
    public void InitializeGame()
    {
        Transform content = alivePlayerScrollViewContent.transform;

        dayDuration = settingsManager.dayDuration;
        votingDuration = settingsManager.votingDuration;
        explanationDuration = settingsManager.explanationDuration;
        dayCounter = 1; // Start at Day 1
        alivePlayerCounterTXT.text = "Alive: " + content.childCount;
        deadPlayerCounterTXT.text = "Dead: 0";
        currentPhase = Phase.Day;
        phaseTitleTXT.text = "Day Phase";
        dayNightCounterTXT.text = "Day " + dayCounter;
        dayNightCounterTXT.color = Color.yellow;
        isCountingDown = false;
        timerTXT.text = dayDuration.ToString() + " seconds";
        startDayTimeBtn.SetActive(true);
        dayTimeBtn.SetActive(false);
        Debug.Log("Game initialized");
    }

    public void CalculateVotesNeeded()
    {

        Transform content = alivePlayerScrollViewContent.transform;

        votesNeededCounter = Mathf.RoundToInt(content.childCount / 2);
        votesNeededTXT.text = "Votes needed: " + votesNeededCounter.ToString();
    }

    public void UpdateAliveDeadPlayerCount()
    {

        Transform aliveContent = alivePlayerScrollViewContent.transform;
        Transform deadContent = graveyardScrollViewContent.transform;

        alivePlayerCounterTXT.text = "Alive: " + aliveContent.childCount;
        deadPlayerCounterTXT.text = "Dead: " + deadContent.childCount;
    }

}
