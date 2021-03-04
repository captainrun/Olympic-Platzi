﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Language
{
    Spanish,
    English,
    Chems
}

public enum TxtCategory
{
    H1,
    MediumText,
    SmallText
}

public enum TxtSize
{
    Small,
    Medium,
    Big
}

public delegate void LanguageEvent(Language text);

public delegate void TextSizeEvent(TxtSize txtSize);

public class GlobalSettings : MonoBehaviour
{
    public static TxtSize CurrentTextSize;
    public static Language GlobalLanguage;
    public static DifficultyScriptable CurrentDifficult;

    [SerializeField] private GlobalConfigurationScriptable currentConfiguration;
    [SerializeField] private GlobalConfigurationScriptable defaultSetting;
    [SerializeField] private List<DifficultyScriptable> difficulties = new List<DifficultyScriptable>();

    private static GlobalSettings _instance;
    public static LanguageEvent OnUpdateLanguage;
    public static TextSizeEvent OnUpdateTextSize;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        RestoreDefaultSetting();
    }

    private void UpdateLanguage(Language newLanguage)
    {
        GlobalLanguage = newLanguage;
        currentConfiguration.language = newLanguage;
        OnUpdateLanguage?.Invoke(currentConfiguration.language);
    }

    public void ChooseDifficult(int difficult)
    {
        UpdateDifficult(difficulties[difficult]);
    }

    private void UpdateDifficult(DifficultyScriptable difficult)
    {
        CurrentDifficult = difficult;
        currentConfiguration.difficultyScriptable = CurrentDifficult;
    }


    public void RestoreDefaultSetting()
    {
        UpdateLanguage(defaultSetting.language);
        ChangeTextSize(defaultSetting.txtSize);
    }


    private void ChangeTextSize(TxtSize txtSize)
    {
        CurrentTextSize = txtSize;
        currentConfiguration.txtSize = txtSize;
        OnUpdateTextSize?.Invoke(txtSize);
    }


    public void ChangeLanguageEnglish()
    {
        UpdateLanguage(Language.English);
    }

    public void ChangeLanguageSpanish()
    {
        UpdateLanguage(Language.Spanish);
    }

    public void ChangeText(int num)
    {
        switch (num)
        {
            case 1:
                ChangeTextSize(TxtSize.Small);
                break;
            case 2:
                ChangeTextSize(TxtSize.Medium);
                break;
            case 3:
                ChangeTextSize(TxtSize.Big);
                break;
            default:
                print("Language not found");
                break;
        }
    }
}