namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class TutorialStepStarted : TutorialEvent
    {
        public long GameplayTime { get; }
        public TutorialStepStarted(int tutorialStep, long gameplayTime) : base(tutorialStep)
        {
            GameplayTime = gameplayTime;
        }
    }
}