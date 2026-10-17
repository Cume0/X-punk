using System.Collections;
using UnityEngine;

public class WorldSpaceUIBar : MonoBehaviour
{
    [field: SerializeField] public int MaxValue { get; private set; }
    [field: SerializeField] public int CurrentValue { get; private set; }
    [SerializeField] private float animationSpeed = 1f;

    private RectTransform barBottom;
    private RectTransform barTop;
    private float fullWidth;
    private float targetWidth => (float)CurrentValue / MaxValue * fullWidth;
    private Coroutine changeValueCoroutine;

    public void Initialize(int maxValue, int currentValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;

        barBottom = transform.GetChild(0).GetComponent<RectTransform>();
        barTop = transform.GetChild(1).GetComponent<RectTransform>();

        fullWidth = barBottom.sizeDelta.x;
    }
    
    public void ChangeValue(int amount)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);

        if (changeValueCoroutine != null)
        {
            StopCoroutine(changeValueCoroutine);
        }

        StartCoroutine(ChangeValueOverTime(amount));
    }

    public IEnumerator ChangeValueOverTime(int amount)
    {
        var suddenChangeBar = amount >= 0 ? barBottom : barTop;
        var slowChangeBar = amount >= 0 ? barTop : barBottom;

        SetWidth(suddenChangeBar, targetWidth);
        while (Mathf.Abs(slowChangeBar.rect.width - suddenChangeBar.rect.width) > 1f)
        {
            SetWidth(slowChangeBar, Mathf.Lerp(slowChangeBar.rect.width, targetWidth, animationSpeed * Time.deltaTime));
            yield return null;
        }
        SetWidth(slowChangeBar, targetWidth);
    }

    void SetWidth(RectTransform bar, float width)
    {
        bar.sizeDelta = new Vector2(width, bar.sizeDelta.y);
    }
}
