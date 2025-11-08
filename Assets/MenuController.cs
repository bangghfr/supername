using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuView _menuView;
    [SerializeField] private SettingsView _settingsView;
    [SerializeField] private Button _toggleMenuButton;
    

    private bool _isMenuActive;

    private void Start()
    {
        _menuView.PlayButtonClicked += ToggleMenu;
        _menuView.SettingsButtonClicked += OpenSettings;
        _menuView.ExitButtonClicked += Exit;
        _toggleMenuButton.onClick.AddListener(ToggleMenu);

    }

    private void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Time.timeScale = 1f;
    }

    private void OpenSettings()
    {
        throw new NotImplementedException();
    }

    private void ToggleMenu()
    {
        _isMenuActive = !_isMenuActive;
        _menuView.gameObject.SetActive(_isMenuActive);
        Time.timeScale = _isMenuActive ? 0f : 1f;
    }
}
