using System;
using UnityEngine;

public abstract class UIElement : MonoBehaviour, IUIElement
{
    public event Action<IUIElement> OnElementHideStartedEvent;
    public event Action<IUIElement> OnElementHiddenCompletelyEvent;
    public event Action<IUIElement> OnElementStartShowEvent;
    public event Action<IUIElement> OnElementShownEvent;
    public event Action<IUIElement> OnElementDestroyedEvent;

    public bool IsActive { get; protected set; } = true;

    public virtual void Show()
    {
        OnPreShow();
        gameObject.SetActive(true);
        IsActive = true;

        OnElementStartShowEvent?.Invoke(this);

        OnPostShow();
    }

    protected virtual void OnPreShow() { }

    protected virtual void OnPostShow()
    {
        OnElementShownEvent?.Invoke(this);
    }

    public void Hide()
    {
        if (!IsActive)
        {
            return;
        }

        OnPreHide();

        OnElementHideStartedEvent?.Invoke(this);

        HideInstantly();
    }

    public virtual void HideInstantly()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        gameObject.SetActive(false);
        OnPostHide();
        OnElementHiddenCompletelyEvent?.Invoke(this);
    }

    protected virtual void OnPreHide() { }
    protected virtual void OnPostHide() { }
}
