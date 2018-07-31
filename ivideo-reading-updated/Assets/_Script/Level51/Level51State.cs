/*
 * Static class to hold data needed throughout level
 * 
 * TODO make can be passed between levels
 * Me thinks just add FinishLevel function where currentStep resets to 0
 */

public static class Level51State {

    public delegate void OnStepChange(int newStep);
    public static OnStepChange NotifyStepChange;

    private static int currentStep = 0;

    public static int CurrentStep { get { return currentStep; } }
    
    /// <summary>
    /// Increments the current step and broadcasts it
    /// </summary>
    public static void NextStep()
    {
        currentStep++;
        NotifyStepChange(currentStep);
    }

}