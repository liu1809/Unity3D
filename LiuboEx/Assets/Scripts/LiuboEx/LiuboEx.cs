using UnityEngine;

/// <summary>
/// <para> 功能 : 一些小功能函数
/// </para>
/// <para> 作者 : Liubo
/// </para>
/// <para> 日期 : 2014-03-13
/// </para>
/// </summary>
public class LiuboEx
{
    #region Public Fields

    /// <summary>
    /// Everything的LayerMask值
    /// </summary>
    static public int LayerMaskEverything
    {
        get { return -1; }
    }

    /// <summary>
    /// Nothing的LayerMask值
    /// </summary>
    static public int LayerMaskNothing
    {
        get { return 0; }
    }

    #endregion

    #region Public Functions
    /// <summary>
    /// 举个栗子:_min = 1f, _max = 5f，当_value = 5.123f时 返回1.123f，当_value = 0.7f时 返回4.3f，
    /// </summary>
    static public float ClampEx(float _value, float _min, float _max)
    {
        if (_min == _max)
        {
            Debug.Log("ClampEx 用法错误");
            return _min;
        }
        else if (_min > _max)
        {
            float temp = _min;
            _min = _max;
            _max = temp;
        }

        if (_value >= _min && _value <= _max)
        {
            return _value;
        }
        else if (_value < _min)
        {
            return ClampEx(_max - (_min - _value), _min, _max);
        }
        else
        {
            return ClampEx(_min + (_value - _max), _min, _max);
        }

    }

    /// <summary>
    /// LayerMask转Layer(LayerMask必须有且仅有一个值)
    /// </summary>
    static public int LayerMask2Layer(LayerMask _layerMask)
    {
        float curLayerF = Mathf.Log(_layerMask.value, 2f);

        int curLayer = (int)curLayerF;

        if (curLayer < 0 || curLayer > 31)
        {
            Debug.Log("LayerMask必须有且仅有一个值!");
            return 0;
        }
        if ((curLayerF - curLayer) != 0f)
        {
            Debug.Log("LayerMask必须有且仅有一个值!");
            return 0;
        }

        return curLayer;
    }

    /// <summary>
    /// Layer转LayerMask
    /// </summary>
    static public int Layer2LayerMask(int _layer)
    {
        if (_layer >= 0 && _layer <= 31)
        {
            return 1 << _layer;
        }
        else
        {
            Debug.Log("Layer超限");
            return 0;
        }
    }


    static public float GetGameObjectRenderSize(GameObject _go)
    {
        System.Collections.Generic.List<UnityEngine.Renderer> rendererList = new System.Collections.Generic.List<Renderer>();

        if(_go.renderer)
        {
            rendererList.Add(_go.renderer);
        }
        UnityEngine.Renderer[] childRenderers = _go.GetComponentsInChildren<Renderer>();
        if (childRenderers.Length > 0)
        {
            rendererList.AddRange(childRenderers);
        }

        Vector3 minValue = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        Vector3 maxValue = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

        foreach(UnityEngine.Renderer one in rendererList)
        {
            if (minValue.x > one.bounds.min.x)
                minValue.x = one.bounds.min.x;
            if (minValue.y > one.bounds.min.y)
                minValue.y = one.bounds.min.y;
            if (minValue.z > one.bounds.min.z)
                minValue.z = one.bounds.min.z;

            if (maxValue.x < one.bounds.max.x)
                maxValue.x = one.bounds.max.x;
            if (maxValue.y < one.bounds.max.y)
                maxValue.y = one.bounds.max.y;
            if (maxValue.z < one.bounds.max.z)
                maxValue.z = one.bounds.max.z;
        }

        return Vector3.Distance(minValue, maxValue);
    }
    #endregion
}
