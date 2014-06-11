using UnityEngine;
using System.Collections;

/// <summary>
/// 调色板(NGUI 3.5.6)
/// </summary>
public class ColorPalette : MonoBehaviour 
{
    #region 公有字段
    public Camera m_UICamera;

    public GameObject m_RootGO;

    public UISlider m_UISlider;

    public UISprite m_DragSprite;

    public UITexture m_SliderNguiTexture;

    public UITexture m_SquareNguiTexture;

    public UISprite m_CurColorSprite;

    public Color curColor = Color.red;

    public Color m_StartSliderColor = Color.green;

    static public ColorPalette MyInstance
    {
        get 
        {
            if (mInstance != null)
            {
                return mInstance;
            }
            else
            {
                Debug.Log("调色板要拖到场景中才能用");
                return null;
            }
        }
    }

    #endregion

    #region 私有字段
    static private ColorPalette mInstance = null;

    private Texture2D mSliderTexture2D;

    private Texture2D mSquareTexture2D;

    private int mSliderTexture2DWidth;
    private int mSliderTexture2DHeight;

    //private float mSliderRatio = 1f;

    private int mSquareTexture2DWidth;
    private int mSquareTexture2DHeight;

    private float mSquareRatioX = 1f;
    private float mSquareRatioY = 1f;

    private Color mSliderColor = Color.red;

    private bool mIsInited = false;
    #endregion

    #region 委托
    public delegate void OnColorChange(Color _color);

    public OnColorChange onColorChange;
    #endregion

    #region 私有函数
    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
        }
        else
        {
            Debug.Log("这个调色板只能有一个");
            DestroyObject(gameObject);
        }
    }

	void Start() 
    {
        Init();
	}

    void Init()
    {
        StartCoroutine(IE_Init());
    }

    IEnumerator IE_Init()
    {
        yield return new WaitForEndOfFrame();

        mSliderTexture2D = new Texture2D(4, 256, TextureFormat.ARGB32, false);
        mSquareTexture2D = new Texture2D(256, 256, TextureFormat.ARGB32, false);

        mSliderTexture2DWidth = mSliderTexture2D.width;
        mSliderTexture2DHeight = mSliderTexture2D.height;

        mSquareTexture2DWidth = mSquareTexture2D.width;
        mSquareTexture2DHeight = mSquareTexture2D.height;

        DrawSliderTexture2D();

        DrawSquareTexture2D();

        m_DragSprite.transform.localPosition = new Vector3(m_SquareNguiTexture.width, m_SquareNguiTexture.height, 0f);

        m_CurColorSprite.color = GetCurColor(1f, 1f, true);

        m_SliderNguiTexture.mainTexture = mSliderTexture2D;
        m_SquareNguiTexture.mainTexture = mSquareTexture2D;

        EventDelegate.Add(m_UISlider.onChange, OnSliderChange);

        UIEventListener.Get(m_SquareNguiTexture.gameObject).onPress += OnSquarePress;

        if (m_RootGO.activeInHierarchy)
        {
            mIsInited = true;
        }
        else
        {
            mIsInited = false;
        }
    }

    Color GetCurColor(float _xRatio, float _yRatio, bool _isInited = false)
    {
        mSquareRatioX = _xRatio;
        mSquareRatioY = _yRatio;

        curColor = Color.Lerp(Color.black, Color.Lerp(Color.white, mSliderColor, _xRatio), _yRatio);

        m_CurColorSprite.color = curColor;

        if (!_isInited && onColorChange != null)
        {
            onColorChange(curColor);
        }

        return curColor;
    }

    Color GetSliderColor(float _ratio)
    {
        if ((_ratio >= 0f) && (_ratio < 0.1667f))
        {
            return new Color(1f, 0f, Mathf.Clamp01(_ratio / 0.1667f));
        }
        else if ((_ratio >= 0.1667f) && (_ratio < 0.3333f))
        {
            return new Color(1 - Mathf.Clamp01((_ratio - 0.1667f) / 0.1667f), 0f, 1f);
        }
        else if(_ratio >= 0.3333 && _ratio < 0.5f)
        {
            return new Color(0f, Mathf.Clamp01((_ratio - 0.3333f) / 0.1667f) , 1f);
        }
        else if (_ratio >= 0.5f && _ratio < 0.6667f)
        {
            return new Color(0f, 1f, 1 - Mathf.Clamp01((_ratio - 0.5f) / 0.1667f));
        }
        else if (_ratio >= 0.6667f && _ratio < 0.8333f)
        {
            return new Color(Mathf.Clamp01((_ratio - 0.6667f) / 0.1667f), 1f, 0f);
        }
        else if(_ratio >= 0.8333f && _ratio <=1f)
        {
            return new Color(1f, 1 - Mathf.Clamp01((_ratio - 0.8333f) / 0.1667f), 0f);
        }
        else
        {
            return Color.white;
        }
    }

    void DrawSliderTexture2D()
    {
        for (int i = 0; i < mSliderTexture2DHeight; i++)
        {
            for (int j = 0; j < mSliderTexture2DWidth; j++)
            {
                mSliderTexture2D.SetPixel(j, i, GetSliderColor(Mathf.Clamp01((float)i / (float)mSliderTexture2DHeight)));
            }
        }

        mSliderTexture2D.Apply();
    }

    void DrawSquareTexture2D()
    {
        for (int i = 0; i < mSquareTexture2DHeight; i++)
        {
            for (int j = 0; j < mSquareTexture2DWidth; j++)
            {
                mSquareTexture2D.SetPixel(j, i, Color.Lerp(Color.black, Color.Lerp(Color.white, mSliderColor, ((float)j / (float)mSquareTexture2DWidth)), ((float)i / (float)mSquareTexture2DHeight)));
            }
        }

        mSquareTexture2D.Apply();
    }

    void OnSliderChange()
    {
        if (mIsInited)
        {
            mSliderColor = GetSliderColor(m_UISlider.value);

            DrawSquareTexture2D();

            m_CurColorSprite.color = GetCurColor(mSquareRatioX, mSquareRatioY);
        }
        else
        {
            mIsInited = true;
        }
    }

    void OnSquarePress(GameObject _go, bool _isPress)
    {
        if(_isPress)
        {
            StartCoroutine("IE_DragSquare");
        }
        else
        {
            StopCoroutine("IE_DragSquare");
        }
    }

    IEnumerator IE_DragSquare()
    {
        float maxPosX = m_SquareNguiTexture.width;

        float maxPosY = m_SquareNguiTexture.height;

        Vector3 curMousePos = Input.mousePosition + new Vector3(1f, 0f, 0f);

        while (true)
        {
            if (curMousePos != Input.mousePosition)
            {
                curMousePos = Input.mousePosition;

                Vector3 uiPos = m_UICamera.ScreenToWorldPoint(curMousePos);

                Vector3 localPos = m_SquareNguiTexture.transform.InverseTransformPoint(uiPos);

                localPos = new Vector3(Mathf.Clamp(localPos.x, 0f, maxPosX), Mathf.Clamp(localPos.y, 0f, maxPosY), 0f);

                m_DragSprite.transform.localPosition = localPos;

                m_CurColorSprite.color = GetCurColor(Mathf.Clamp01(localPos.x / maxPosX), Mathf.Clamp01(localPos.y / maxPosY));

            }

            yield return null;
        }
    }
    #endregion

    #region 公有函数
    public void Show()
    {
        Show(null);
    }

    public void Show(OnColorChange _onColorChange)
    {
        m_RootGO.SetActive(true);

        if(_onColorChange != null)
        {
            onColorChange = _onColorChange;
        }
    }

    public void Hide()
    {
        m_RootGO.SetActive(false);
    }

    public void Toggle()
    {
        if (m_RootGO.activeInHierarchy)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    #endregion
}
