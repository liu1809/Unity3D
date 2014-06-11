using UnityEngine;
using System.Collections;

public class InputExListener : MonoBehaviour 
{
    #region Delegate
    public delegate void OnHover(GameObject _go, bool _isHover);
    public OnHover onMouseHover;

    public delegate void OnHoverEx(GameObject _go, bool _isHover, RaycastHit _hit);
    public OnHoverEx onMouseHoverEx;

    public delegate void OnPress(GameObject _go, bool _isPress);
    public OnPress onMousePress;

    public delegate void OnClick(GameObject _go);
    public OnClick onMouseClick;

    public delegate void OnDoubleClick(GameObject _go);
    public OnDoubleClick onMouseDoubleClick;


    public delegate void OnTouchClick(GameObject _go);
    public OnTouchClick onTouchClick;

    public delegate void OnTouchDoubleClick(GameObject _go);
    public OnTouchDoubleClick onTouchDoubleClick;

    public delegate void OnTouchLongPress(GameObject _go);
    public OnTouchLongPress onTouchLongPress;

    public delegate void OnTouchPress(GameObject _go, bool _isPress);
    public OnTouchPress onTouchPress;
    #endregion

	#region Methods
	public void MouseOnHover(bool _isHover, RaycastHit _hit) 
    {
        if (onMouseHover != null) onMouseHover(gameObject, _isHover);
        if (onMouseHoverEx != null) onMouseHoverEx(gameObject, _isHover, _hit);
	}

    public void MouseOnPress(bool _isPress)
    {
        if (onMousePress != null) onMousePress(gameObject, _isPress);
    }

    public void MouseOnClick()
    {
        if (onMouseClick != null) onMouseClick(gameObject);
    }

    public void MouseOnDoubleClick()
    {
        if (onMouseDoubleClick != null) onMouseDoubleClick(gameObject);
    }

    
    public void TouchClick()
    {
        if (onTouchClick != null) onTouchClick(gameObject);
    }

    public void TouchDoubleClick()
    {
        if (onTouchDoubleClick != null) onTouchDoubleClick(gameObject);
    }

    public void TouchLongPress()
    {
        if (onTouchLongPress != null) onTouchLongPress(gameObject);
    }

    public void TouchPress(bool _isPress)
    {
        if (onTouchPress != null) onTouchPress(gameObject, _isPress);
    }
	#endregion

    static public InputExListener Get(GameObject go)
    {
        InputExListener listener = go.GetComponent<InputExListener>();

        if (listener != null)
        {
            return listener;
        }
        else
        {
            listener = go.AddComponent<InputExListener>();
            return listener;
        }
    }
}
