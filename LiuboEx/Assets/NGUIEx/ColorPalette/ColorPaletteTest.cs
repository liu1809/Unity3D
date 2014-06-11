using UnityEngine;
using System.Collections;

/// <summary>
/// <para> 功能 :
/// </para>
/// <para> 作者 : 
/// </para>
/// <para> 日期 : 
/// </para>
/// </summary>
public class ColorPaletteTest : MonoBehaviour
{
    void Start()
    {
        ColorPalette.MyInstance.onColorChange += delegate(Color _color) { renderer.material.color = _color; };
    }
}
