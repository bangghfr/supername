using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Button _buttonSettings;
    [SerializeField] private Button _buttonExit;

    public event Action PlayButtonClicked;
    public event Action SettingsButtonClicked;
    public event Action ExitButtonClicked;

    private void Start()
    {
        _buttonPlay.onClick.AddListener(PlayClicked);
        _buttonSettings.onClick.AddListener(SettingsClicked);
        _buttonExit.onClick.AddListener(ExitClicked);
    }

    private void PlayClicked()
    {
        PlayButtonClicked?.Invoke();
    }

    private void SettingsClicked()
    {
        SettingsButtonClicked?.Invoke();
    }

    private void ExitClicked()
    {
        ExitButtonClicked?.Invoke();
    }

    private void OnDestroy()
    {
        _buttonPlay.onClick.RemoveListener(PlayClicked);
        _buttonSettings.onClick.RemoveListener(SettingsClicked);
        _buttonExit.onClick.RemoveListener(ExitClicked);
    }
}
