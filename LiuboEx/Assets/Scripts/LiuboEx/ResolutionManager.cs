using UnityEngine;
using System.Collections;

public class ResolutionManager : MonoBehaviour 
{

    public int m_DelayFrameCount = 0;

    public delegate void OnResolutionChange();

	/// <summary>
	/// 分辨率改变事件委托
	/// </summary>
    static public OnResolutionChange onResolutionChange;


    private int lastWidth = 0;
    private int lastHeight = 0;

	void Start() 
	{
        lastWidth = Screen.width;
        lastHeight = Screen.height;
	}
	
	void LateUpdate() 
	{
		if(Screen.width != lastWidth || Screen.height != lastHeight)
        {
            if(m_DelayFrameCount <= 0)
            {
                if (onResolutionChange != null)
                {
                    onResolutionChange();
                }
            }
            else
            {
                StopCoroutine("IE_OnScreenResolutionChange");

                StartCoroutine("IE_OnScreenResolutionChange");
            }
        }
	}

    IEnumerator IE_OnScreenResolutionChange()
    {
        int curDelayCount = m_DelayFrameCount;
        while (curDelayCount > 0)
        {
            curDelayCount--;
            yield return new WaitForEndOfFrame();
        }

        if(onResolutionChange != null)
        {
            onResolutionChange();
        }
    }
}
