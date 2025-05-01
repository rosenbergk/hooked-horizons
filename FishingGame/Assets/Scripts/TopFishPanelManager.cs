// TopFishPanelController.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopFishPanelController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject panel; // your TopFishPanel
    public TextMeshProUGUI[] entries; // size = 10

    void Awake()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Call from your menu button’s OnClick()
    /// </summary>
    public void TogglePanel()
    {
        bool nowOn = !panel.activeSelf;
        panel.SetActive(nowOn);
        if (nowOn)
            RefreshList();
    }

    private void RefreshList()
    {
        List<(string name, float weight)> top = TopFishTracker.Instance.GetTopFish();
        for (int i = 0; i < entries.Length; i++)
        {
            if (i < top.Count)
            {
                var fish = top[i];
                entries[i].text = $"{i + 1}. {fish.name} - {fish.weight:F2} lbs.";
            }
            else
            {
                entries[i].text = $"{i + 1}. Catch more fish";
            }
        }
    }
}
