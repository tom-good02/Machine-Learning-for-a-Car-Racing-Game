using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadModelMapDifficultyManager : MonoBehaviour
{
    [SerializeField] private Image easyHighlight;
    [SerializeField] private Image mediumHighlight;
    [SerializeField] private Image hardHighlight;
    [SerializeField] private Image silverstoneHighlight;
    [SerializeField] private GameObject newEasyHighlight;
    [SerializeField] private GameObject newMediumHighlight;
    [SerializeField] private GameObject newHardHighlight;
    [SerializeField] private GameObject newSilverstoneHighlight;

    public void Awake()
    {
        var trackDifficulty = PlayerPrefs.GetInt(PlayerPrefKeys.TrackDifficulty, 0);
        switch (trackDifficulty)
        {
            case 0:
                SetEasy();
                break;
            case 1:
                SetMedium();
                break;
            case 2:
                SetHard();
                break;
            case 3:
                SetSilverstone();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetEasy()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 0);
        easyHighlight.enabled = true;
        newEasyHighlight.SetActive(true);
        mediumHighlight.enabled = false;
        newMediumHighlight.SetActive(false);
        hardHighlight.enabled = false;
        newHardHighlight.SetActive(false);
        silverstoneHighlight.enabled = false;
        newSilverstoneHighlight.SetActive(false);
    }
    
    public void SetMedium()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 1);
        easyHighlight.enabled = false;
        newEasyHighlight.SetActive(false);
        mediumHighlight.enabled = true;
        newMediumHighlight.SetActive(true);
        hardHighlight.enabled = false;
        newHardHighlight.SetActive(false);
        silverstoneHighlight.enabled = false;
        newSilverstoneHighlight.SetActive(false);
    }
    
    public void SetHard()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 2);
        easyHighlight.enabled = false;
        newEasyHighlight.SetActive(false);
        mediumHighlight.enabled = false;
        newMediumHighlight.SetActive(false);
        hardHighlight.enabled = true;
        newHardHighlight.SetActive(true);
        silverstoneHighlight.enabled = false;
        newSilverstoneHighlight.SetActive(false);
    }
    
    public void SetSilverstone()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 3);
        easyHighlight.enabled = false;
        newEasyHighlight.SetActive(false);
        mediumHighlight.enabled = false;
        newMediumHighlight.SetActive(false);
        hardHighlight.enabled = false;
        newHardHighlight.SetActive(false);
        silverstoneHighlight.enabled = true;
        newSilverstoneHighlight.SetActive(true);
    }
}
