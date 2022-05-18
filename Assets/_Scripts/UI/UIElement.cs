using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    public virtual void Show()
    {
        OnPreShow();

        gameObject.SetActive(true);

        OnPostShow();
    }

    protected virtual void OnPreShow() { }

    protected virtual void OnPostShow() { }

    public void Hide()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        OnPreHide();

        HideInstantly();
    }

    public virtual void HideInstantly()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        gameObject.SetActive(false);
        OnPostHide();
    }

    protected virtual void OnPreHide() { }
    protected virtual void OnPostHide() { }
}
