using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Item_Button<T> : MonoBehaviour
{
    [SerializeField]
    protected Image m_Img = null;
    [SerializeField]
    protected Image m_ImgBorder = null;
    [SerializeField]
    protected float m_HoverScale = 1.05f;

    protected T m_Data = default;
    public T Data { get {  return m_Data; } }

    public abstract void Initialize(T data);

    public virtual void ButtonPressed()
    {
        OnSelect();
    }

    public virtual void OnHoverBegin()
    {
        m_Img.transform.localScale = new Vector3(m_HoverScale, m_HoverScale);
    }

    public virtual void OnHoverEnd()
    {
        m_Img.transform.localScale = new Vector3(1, 1);
    }

    public virtual void OnSelect()
    {
        m_ImgBorder.gameObject.SetActive(true);
    }

    public virtual void OnDeselect()
    {
        m_ImgBorder.gameObject.SetActive(false);
    }
}
