using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using UnityEngine;

namespace TestSlotMechanic
{
    public class GameMain : MonoBehaviourExtBind
    {
        [OnAwake]
        private void AwakeThis()
        {
            Log.Debug("GameMain Awake");
        }

        [OnStart]
        private void StartThis()
        {
            Settings.Fsm = new FSM();
            Settings.Fsm.Add(new SlotInitState());
            Settings.Fsm.Add(new SlotReadyState());
            Settings.Fsm.Add(new SlotSpinningState());
            Settings.Fsm.Add(new SlotStoppingState());

            Settings.Fsm.Start("SlotInit");
        }

        [OnUpdate]
        private void UpdateThis()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }
    }
}