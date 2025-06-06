// TugOfWarUI.cs
using UnityEngine;

public class TugOfWarUI : MonoBehaviour
{
    public RectTransform leftRed,
        leftYellow,
        green,
        rightYellow,
        rightRed;
    private float maxValue,
        greenWidth,
        greenStartOffset;
    private bool uiActive;

    public void Initialize(float sliderMaxValue, float sliderGreenWidth, float sliderGreenStart)
    {
        maxValue = sliderMaxValue;
        greenWidth = sliderGreenWidth;
        greenStartOffset = sliderGreenStart;
    }

    public void UpdateZones(float leftRedEnd, float rightRedEnd)
    {
        if (!uiActive)
            return;

        float invMax = 1f / maxValue;
        float lr = leftRedEnd * invMax;
        float gs = greenStartOffset * invMax;
        float ge = gs + greenWidth * invMax;
        float rr = rightRedEnd * invMax;

        SetAnchors(leftRed, 0f, lr);
        SetAnchors(leftYellow, lr, gs);
        SetAnchors(green, gs, ge);
        SetAnchors(rightYellow, ge, rr);
        SetAnchors(rightRed, rr, 1f);
    }

    public void SetActive(bool on)
    {
        uiActive = on;
        leftRed.gameObject.SetActive(on);
        leftYellow.gameObject.SetActive(on);
        green.gameObject.SetActive(on);
        rightYellow.gameObject.SetActive(on);
        rightRed.gameObject.SetActive(on);
    }

    private void SetAnchors(RectTransform rt, float xMin, float xMax)
    {
        rt.anchorMin = new Vector2(xMin, 0f);
        rt.anchorMax = new Vector2(xMax, 1f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
