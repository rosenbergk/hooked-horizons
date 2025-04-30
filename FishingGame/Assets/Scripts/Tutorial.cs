using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialUI;

    private bool tutorialActive = true;    
    void Start()
    {
        if (tutorialUI != null){
            tutorialUI.SetActive(true);
        }
        
    }

    public void OnFirstFishCaught()
    {
        if (tutorialActive)
        {
            tutorialActive = false;
            if (tutorialUI != null)
                tutorialUI.SetActive(false);
        }
    }
}
