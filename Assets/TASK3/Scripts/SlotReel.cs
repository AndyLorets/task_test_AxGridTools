using AxGrid;
using AxGrid.Base;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AxGrid.Model;

namespace TestSlotMechanic
{
    public class SlotReel : MonoBehaviourExtBind
    {
        [Header("Настройки элементов")]
        [SerializeField] private SlotItemView _itemPrefab;
        [SerializeField] private SlotSymbolData[] _availableSymbols;
        [SerializeField] private int _visibleItemsCount = 5;
        [SerializeField] private float _itemHeight = 140f;
        [SerializeField] private float _spacing = 60f;

        [Header("Параметры движения")]
        [SerializeField, Min(1000)] private float _maxSpeed = 1500f;
        [SerializeField, Min(500)] private float _acceleration = 1000f;
        [SerializeField, Min(400)] private float _stopDeceleration = 800f;
        [SerializeField, Min(400)] private float _targetSnapSpeed = 400f;

        private const float SnapCaptureRange = 0.4f;
        private const float SnapDuration = 0.9f;
        private const float SnapOvershoot = 2.5f;

        private List<SlotItemView> _activeItems = new List<SlotItemView>();
        private float _currentSpeed = 0f;
        private bool _isSpinning = false;
        private bool _isStopping = false;
        private bool _isSnapping = false;
        private float _bottomLimit;
        private float _wrapOffset;
        private float _snapMovedDelta;

        private float StepSize => _itemHeight + _spacing;

        [OnStart]
        private void BuildReel()
        {
            int halfCount = _visibleItemsCount / 2;
            _bottomLimit = -(halfCount * StepSize) - (StepSize / 2f);
            _wrapOffset = _visibleItemsCount * StepSize;

            for (int i = 0; i < _visibleItemsCount; i++)
            {
                var instance = Instantiate(_itemPrefab, transform);
                float startY = (halfCount - i) * StepSize;
                instance.Setup(_itemHeight, GetRandomSymbol());
                instance.SetPositionY(startY);
                _activeItems.Add(instance);
            }
        }

        [Bind(GameConstants.EvtVisualStartSpin)]
        private void OnStartSpin()
        {
            for (int i = 0; i < _activeItems.Count; i++)
            {
                _activeItems[i].StopWinAnimation();
            }

            _isSpinning = true;
            _isStopping = false;
            _isSnapping = false;
            _currentSpeed = 0;
        }

        [Bind(GameConstants.EvtVisualStopSpin)]
        private void OnStopSpin()
        {
            _isStopping = true;
        }

        [OnUpdate]
        private void UpdateReel()
        {
            if (!_isSpinning || _isSnapping) return;

            if (!_isStopping)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
            }
            else
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSnapSpeed, _stopDeceleration * Time.deltaTime);

                if (_currentSpeed <= _targetSnapSpeed)
                {
                    for (int i = 0; i < _activeItems.Count; i++)
                    {
                        var item = _activeItems[i];
                        if (item.CurrentY > 0 && item.CurrentY < StepSize * SnapCaptureRange)
                        {
                            StartFinalSnap(item);
                            break;
                        }
                    }
                }
            }

            ProcessMovement(_currentSpeed * Time.deltaTime);
        }

        private void StartFinalSnap(SlotItemView winnerItem)
        {
            _isSnapping = true;
            float distanceToMove = winnerItem.CurrentY;
            _snapMovedDelta = 0; 

            DOTween.To(() => _snapMovedDelta, x => {
                float delta = x - _snapMovedDelta;
                _snapMovedDelta = x;
                ApplyManualMove(delta);
            }, distanceToMove, SnapDuration)
            .SetEase(Ease.OutBack, SnapOvershoot)
            .OnComplete(() => {
                _isSpinning = false;
                _isStopping = false;
                _isSnapping = false;
                _currentSpeed = 0;

                winnerItem.StartWinAnimationLoop();
                Settings.Fsm.Invoke(GameConstants.EvtVisualSpinStopped);
            });
        }

        private void ApplyManualMove(float delta)
        {
            for (int i = 0; i < _activeItems.Count; i++)
            {
                _activeItems[i].MoveDown(delta, _bottomLimit, _wrapOffset);
            }
        }

        private void ProcessMovement(float deltaY)
        {
            for (int i = 0; i < _activeItems.Count; i++)
            {
                var item = _activeItems[i];
                bool wrapped = item.MoveDown(deltaY, _bottomLimit, _wrapOffset);
                if (wrapped && !_isStopping) item.SetData(GetRandomSymbol());
            }
        }

        private SlotSymbolData GetRandomSymbol() => _availableSymbols[Random.Range(0, _availableSymbols.Length)];
    }
}