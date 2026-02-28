using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;

namespace TestSlotMechanic
{
    [State(GameConstants.StateInit)]
    public class SlotInitState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Log.Debug("SlotInitState ENTER");
            Parent.Change(GameConstants.StateReady);
        }
    }

    [State(GameConstants.StateReady)]
    public class SlotReadyState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Log.Debug("SlotReadyState ENTER");

            Settings.Model.Set(GameConstants.ModelBtnStart, true);
            Settings.Model.Set(GameConstants.ModelBtnStop, false);
        }

        [Bind("OnBtn")]
        private void OnBtnClick(string btnName)
        {
            if (btnName == "Start")
                Parent.Change(GameConstants.StateSpinning);
        }
    }

    [State(GameConstants.StateSpinning)]
    public class SlotSpinningState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Log.Debug("SlotSpinningState ENTER");

            Settings.Model.Set(GameConstants.ModelBtnStart, false);
            Settings.Model.Set(GameConstants.ModelBtnStop, false);
            Settings.Model.EventManager.Invoke(GameConstants.EvtStopWinParticles);
            Settings.Model.EventManager.Invoke(GameConstants.EvtVisualStartSpin);
        }

        [One(3f)]
        private void AllowStop()
        {
            Log.Debug("Stop is allowed");
            Settings.Model.Set(GameConstants.ModelBtnStop, true);
        }

        [Bind("OnBtn")]
        private void OnBtnClick(string btnName)
        {
            if (btnName == "Stop")
                Parent.Change(GameConstants.StateStopping);
        }
    }

    [State(GameConstants.StateStopping)]
    public class SlotStoppingState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Log.Debug("SlotStoppingState ENTER");
            Settings.Model.Set(GameConstants.ModelBtnStop, false);

            Settings.Model.EventManager.Invoke(GameConstants.EvtVisualStopSpin);
        }

        [Bind(GameConstants.EvtVisualSpinStopped)]
        private void OnVisualStopped()
        {
            Settings.Model.EventManager.Invoke(GameConstants.EvtPlayWinParticles);
            Parent.Change(GameConstants.StateReady);
        }
    }
}