using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
/// <summary>
/// <para> 功能 : Window下最大化
/// </para>
/// <para> 作者 : Liubo
/// </para>
/// <para> 日期 : 2014-03-13
/// </para>
/// 修复 重新载入时窗口变化的Bug__2014-05-07
/// 删除 分辨率改动事件__2014-05-07
/// </summary>
public class ScreenEx : MonoBehaviour
{
    #region Private Fields

    static private ScreenEx mInstance = null;

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    static private IntPtr mWin32WindowPtr = IntPtr.Zero;

    static private bool mIsFirstRun = true;
#endif


    private Resolution[] mResolutions;

    #endregion

    #region Public Fields

    public string m_Win32WindowName = "";

    public bool m_StartMaxWindow = false;

    public int m_Width = 800;

    public int m_Height = 600;
    static public bool IsFullScreen
    {
        set
        {
            if (value)
            {
                Screen.fullScreen = true;

                Screen.SetResolution(mInstance.mResolutions[mInstance.mResolutions.Length - 1].width, mInstance.mResolutions[mInstance.mResolutions.Length - 1].height, true);
            }
            else
            {
                Screen.SetResolution(mInstance.m_Width, mInstance.m_Height, false);
                MaxMainWindow(2);
            }
        }
    }

    #endregion

    #region Unity3D Event

    void Awake()
    {

        if (mInstance == null)
        {
            mInstance = this;
        }
        else
        {
            Debug.Log("ScreenEx Only One! " + "-1-" + mInstance.gameObject.name + " -2-" + this.gameObject.name);
        }
    }

    void Start()
    {
#if UNITY_STANDALONE_WIN
        if (mIsFirstRun)
        {
            Init();
        }
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN
        Application.CancelQuit();
        StartCoroutine(IE_Qiut());
#endif
    }

    #endregion

    #region Private Functions

#if UNITY_EDITOR
    [ContextMenu("GetWin32WindowName")]
    void GetWindowName()
    {
        m_Win32WindowName = UnityEditor.PlayerSettings.productName;
    }
#endif

    void Init()
    {
        StartCoroutine(IE_Init());
    }


    IEnumerator IE_Init()
    {
#if UNITY_STANDALONE_WIN
        mIsFirstRun = false;


        if (Screen.width != m_Width || Screen.height != m_Height)
        {
            Screen.SetResolution(m_Width, m_Height, false);
            yield return new WaitForEndOfFrame();
        }

        mResolutions = Screen.GetResolution;

        yield return StartCoroutine(IE_GetWin32Ptr());

        if (m_StartMaxWindow)
        {
            MaxMainWindow();
        }
#endif
        yield return new WaitForEndOfFrame();
    }

    static public void MaxMainWindow(int _delay = 0)
    {
#if UNITY_STANDALONE_WIN
        mInstance.StartCoroutine(mInstance.IE_MaxMainWindow(_delay));
#endif
    }

#if UNITY_STANDALONE_WIN
    IEnumerator IE_GetWin32Ptr()
    {
        mWin32WindowPtr = FindWindow(null, m_Win32WindowName);

        yield return new WaitForEndOfFrame();

        if (mWin32WindowPtr == IntPtr.Zero)
        {
            Debug.Log("Window名字不对 获取不到指针!");
        }

    }

    IEnumerator IE_Qiut()
    {
        if (Screen.width != m_Width || Screen.height != m_Height)
        {
            Screen.SetResolution(m_Width, m_Height, false);
            yield return new WaitForEndOfFrame();
        }

        mWin32WindowPtr = IntPtr.Zero;

        Application.Quit();
    }

    IEnumerator IE_MaxMainWindow(int _delay = 0)
    {
        if (_delay > 0)
        {
            for (int i = 0; i < _delay; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        if (mWin32WindowPtr != IntPtr.Zero)
        {
            SendMessage(mWin32WindowPtr, 274, 61488, 0);
        }
        else
        {
            yield return StartCoroutine(IE_GetWin32Ptr());

            if (mWin32WindowPtr != IntPtr.Zero)
            {
                SendMessage(mWin32WindowPtr, 274, 61488, 0);
            }
        }
    }
#endif

    #endregion
}
