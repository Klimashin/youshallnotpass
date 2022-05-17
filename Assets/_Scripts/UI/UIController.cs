using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class UIController : MonoBehaviour, IInitializable
{
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _uiCanvas;
    [SerializeField] private MainMenuScreen _mainMenuScreen;
    [SerializeField] private GameplayScreen _gameplayScreen;
    [SerializeField] private GameOverScreen _gameOverScreen;

    private readonly Dictionary<Type, IUIElement> _uiElementsMap = new();

    public void Initialize()
    {
        FillUiDictionary();

        SetupUiCamera();
    }

    public T ShowUIElement<T>() where T : UIElement
    {
        var type = typeof(T);

        if (!_uiElementsMap.TryGetValue(type, out var uiElement))
        {
            throw new KeyNotFoundException($"UIController::ShowUIElement - Missing ui element of type {type}.");
        }

        uiElement.Show();
        return (T)uiElement;
    }

    public void HideUIElement<T>() where T : UIElement
    {
        var type = typeof(T);

        if (!_uiElementsMap.TryGetValue(type, out var uiElement))
        {
            throw new KeyNotFoundException($"UIController::HideUIElement - Missing ui element of type {type}.");
        }

        uiElement.Hide();
    }

    private void FillUiDictionary()
    {
        _uiElementsMap[_mainMenuScreen.GetType()] = _mainMenuScreen;
        _uiElementsMap[_gameplayScreen.GetType()] = _gameplayScreen;
        _uiElementsMap[_gameOverScreen.GetType()] = _gameOverScreen;
    }

    private void SetupUiCamera()
    {
        var mainCamera = Camera.main;
        var cameraData = mainCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(_uiCamera);
    }
}
