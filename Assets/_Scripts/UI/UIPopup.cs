using UnityEngine;
using UnityEngine.UI;

public abstract class UIPopup : UIElement, IUIPopup 
{
    [SerializeField] protected bool _isPreCached;
    [Space] [SerializeField] protected Button[] _buttonsClose;
    
    public bool isPreCached => _isPreCached;
    public Button[] buttonsClose => _buttonsClose;

    public sealed override void Show() 
    {
        base.Show();

        SubscribeOnCloseEvents();

        if (isPreCached) 
        {
            transform.SetAsLastSibling();
        }
    }

    private void SubscribeOnCloseEvents() 
    {
        foreach (var button in buttonsClose)
        {
            button.onClick.AddListener(OnCloseButtonClick);
        }
    }

    public override void HideInstantly() 
    {
        if (!IsActive)
        {
            return;
        }

        UnsubscribeFromCloseEvents();

        if (isPreCached)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

        base.HideInstantly();
    }
    
    private void UnsubscribeFromCloseEvents() 
    {
        foreach (var button in buttonsClose)
        {
            button.onClick.RemoveListener(OnCloseButtonClick);
        }
    }

    private void OnCloseButtonClick() 
    {
        Hide();
    }

    public virtual void OnCreate() { }
    public virtual void OnInitialize() { }
    public virtual void OnStart() { }
}