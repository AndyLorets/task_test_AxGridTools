using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TestSlotMechanic
{
    [System.Serializable]
    public class SlotSymbolData
    {
        public string symbolId;
        public Sprite icon;
    }

    [RequireComponent(typeof(RectTransform))]
    public class SlotItemView : MonoBehaviour
    {
        [SerializeField] private Image _symbolImage;

        private RectTransform _rect;
        private Sequence _winSequence; 
        private static readonly Color WinGlowColor = new Color(1f, 1f, 0.8f); 

        public float CurrentY => _rect.anchoredPosition.y;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void Setup(float size, SlotSymbolData initialData)
        {
            _rect.sizeDelta = new Vector2(size, size);
            SetData(initialData);
        }

        public void SetData(SlotSymbolData data)
        {
            if (_symbolImage != null && data != null)
            {
                _symbolImage.sprite = data.icon;
                gameObject.name = $"Symbol_{data.symbolId}";
            }
        }

        public void StartWinAnimationLoop()
        {
            StopWinAnimation();

            _winSequence = DOTween.Sequence();

            _winSequence.Join(transform.DORotate(new Vector3(0, 0, 5f), 0.3f).SetEase(Ease.Linear));
            _winSequence.Join(transform.DOScale(1.15f, 0.5f).SetEase(Ease.InOutSine));
            _winSequence.Join(_symbolImage.DOColor(WinGlowColor, 0.5f));

            _winSequence.SetLoops(-1, LoopType.Yoyo);
        }

        public void StopWinAnimation()
        {
            _winSequence?.Kill();
            _winSequence = null;

            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            _symbolImage.color = Color.white;
        }

        public void SetPositionY(float yPos)
        {
            _rect.anchoredPosition = new Vector2(0, yPos);
        }

        public bool MoveDown(float delta, float bottomLimit, float wrapOffset)
        {
            Vector2 pos = _rect.anchoredPosition;
            pos.y -= delta;

            bool wrapped = false;

            if (pos.y < bottomLimit)
            {
                pos.y += wrapOffset;
                wrapped = true;
            }

            _rect.anchoredPosition = pos;
            return wrapped;
        }
    }
}