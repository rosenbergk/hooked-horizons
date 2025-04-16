// FishWeightDisplay.cs
using TMPro;
using UnityEngine;

public class FishWeightDisplay : MonoBehaviour
{
    public TextMeshProUGUI fishWeightText;

    public void Update()
    {
        if (FishWeightManager.Instance != null && fishWeightText != null)
        {
            fishWeightText.text =
                $"Fish caught: {FishWeightManager.Instance.TotalFishPounds:F2} lbs";
        }
    }
}
