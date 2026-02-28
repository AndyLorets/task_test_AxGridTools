namespace TestSlotMechanic
{
    public static class GameConstants
    {
        public const string StateInit = "SlotInit";
        public const string StateReady = "SlotReady";
        public const string StateSpinning = "SlotSpinning";
        public const string StateStopping = "SlotStopping";

        public const string EvtVisualStartSpin = "VisualStartSpin";
        public const string EvtVisualStopSpin = "VisualStopSpin";
        public const string EvtVisualSpinStopped = "VisualSpinStopped";
        public const string EvtPlayWinParticles = "PlayWinParticles";
        public const string EvtStopWinParticles = "StopWinParticles";

        public const string ModelBtnStart = "BtnStartEnable";
        public const string ModelBtnStop = "BtnStopEnable";
    }
}