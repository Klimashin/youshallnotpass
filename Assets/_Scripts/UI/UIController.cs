using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIController : MonoBehaviour 
{
	[SerializeField] private Camera _uiCamera;
	[SerializeField] private RectTransform _uiCanvas;
	[SerializeField] private MainMenuScreen _mainMenuScreen;

	private readonly Dictionary<Type, IUIElement> _createdUIElementsMap = new();
	private readonly Dictionary<Type, UIElement> _uiPrefabsDict = new();
	
	public T ShowUIElement<T>() where T : UIElement
	{
		var type = typeof(T);

		if (_createdUIElementsMap.TryGetValue(type, out var uiElement))
		{
			uiElement.Show();
			return (T) uiElement;
		}

		throw new KeyNotFoundException($"UIController::ShowUIElement - Missing ui element of type {type}.");
	}

	private void Awake()
	{
		FillUiDictionary();
		
		SetupUiCamera();

		BuildUI();
	}

	private void FillUiDictionary()
	{
		_uiPrefabsDict[typeof(MainMenuScreen)] = _mainMenuScreen;
	}

	private void SetupUiCamera()
	{
		var mainCamera = Camera.main;
		var cameraData = mainCamera.GetUniversalAdditionalCameraData();
		cameraData.cameraStack.Add(_uiCamera);
	}

	private void BuildUI() 
	{
		foreach (var (uiElementType, uiElementPrefab) in _uiPrefabsDict)
		{
			var uiElement = Instantiate(uiElementPrefab, _uiCanvas);
			_createdUIElementsMap[uiElementType] = uiElement;
			uiElement.gameObject.SetActive(false);
		}
	}
}
