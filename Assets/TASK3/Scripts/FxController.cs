using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;

namespace TestSlotMechanic
{
    public class FxController : MonoBehaviourExtBind
    {
        public ParticleSystem[] _winParticles;

        [Bind(GameConstants.EvtPlayWinParticles)]
        private void PlayFx()
        {
            if (_winParticles == null) return;

            for (int i = 0; i < _winParticles.Length; i++)
            {
                if (_winParticles[i] != null)
                    _winParticles[i].Play();
            }
        }

        [Bind(GameConstants.EvtStopWinParticles)]
        private void StopFx()
        {
            if (_winParticles == null) return;

            for (int i = 0; i < _winParticles.Length; i++)
            {
                if (_winParticles[i] != null)
                    _winParticles[i].Stop();
            }
        }
    }
}