using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NguiEx
{
    /// <summary>
    /// 判断当前是否点击到了UI
    /// </summary>
    /// <param name="_ignoreNgui">是否忽略Ngui</param>
    /// <param name="_ignoreNguiColliderList">不算UI的Collider</param>
    /// <returns></returns>
    static public bool IsHitUI(bool _ignoreNgui, List<Collider> _ignoreNguiColliderList)
    {
        if (_ignoreNgui && _ignoreNguiColliderList.Count > 0)
        {
            if (UICamera.lastHit.collider)
            {
                if (!_ignoreNguiColliderList.Contains(UICamera.lastHit.collider))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 用一个Sprite做参考 设置Camera的Rect
    /// </summary>
    static public void SetViewportCamera(Camera _targetCamera, Camera _nguiCamera, Transform _topLeft, Transform _bottomRight)
    {
        if (_topLeft != null && _bottomRight != null)
        {
            Vector3 tl = _nguiCamera.WorldToScreenPoint(_topLeft.position);
            Vector3 br = _nguiCamera.WorldToScreenPoint(_bottomRight.position);

            Rect rect = new Rect(tl.x / Screen.width, br.y / Screen.height,
                (br.x - tl.x) / Screen.width, (tl.y - br.y) / Screen.height);

            if (rect != _targetCamera.rect)
            {
                _targetCamera.rect = rect;
            }

        }
    }

}
