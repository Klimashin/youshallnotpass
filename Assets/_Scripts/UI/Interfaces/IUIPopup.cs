using UnityEngine.UI;


public interface IUIPopup : IUIElement 
{
	bool isPreCached { get; }
	Button[] buttonsClose { get; }
}