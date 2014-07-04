using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputEx : MonoBehaviour
{
    #region Public Fields
    public LayerMask m_LayerMask;

    public bool m_IgnoreNgui = true;

    public List<Collider> m_IgnoreNguiColliderList = new List<Collider>();

    public bool m_MouseHoverEnableOnlyMove = false;

    public float m_MouseDoubleClickTime = 0.3f;

    public float m_TouchDoubleClickTime = 0.3f;

    public float m_TouchLongPressTime = 0.6f;
    #endregion

    #region Private Fields
    private Camera m_Camera;

    private Ray mRay;

    private RaycastHit mHit;

    bool mCurFrameIsHitUI = false;

    bool isHit = false;
    #endregion

    #region Private Mouse Fields

    Vector3 mMouseCurMousePos;

    Vector3 mMouseOldMousePos;

    InputExListener mMouseCurHitListener;

    InputExListener mMouseCurClickHitListener;

    InputExListener mMouseCurDoubleClickHitListener;

    InputExListener mMouseCurHitHoverListener;

    InputExListener mMouseOldHitHoverListener;

    InputExListener mMouseCurHitPressListener;

    InputExListener mMouseOldHitPressListener;

    bool mMousePressing = false;

    bool mMouseIsTestDoubleClick = false;

    bool mMouseIsTestEndDoubleClick = false;

    float mMouseTestDoubleClickTime = 0f;

    #endregion

    #region Private Touch Fields
    int mCurTouchCount = 0;

    float mCurTouchDoubleClickTestTime = 0f;

    bool mTouchDoubleClickTesting = false;

    float mCurTouchLongPressTime = 0f;

    bool mTouchLongPressTesting = true;

    InputExListener mTouchCurHitListener;

    InputExListener mTouchOldPressListener;
    #endregion

    #region Delegate
    public delegate void OnMouse_DoubleClickNull();
    public OnMouse_DoubleClickNull onMouse_DoubleClickNull;


    public delegate void OnTouch_DoubleClickNull();
    public OnTouch_DoubleClickNull onTouch_DoubleClickNull;

    #endregion

    #region Unity3D Event
    void Awake()
    {
        if (!m_Camera) m_Camera = camera;
    }

    void LateUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MouseMethod();
#endif

#if UNITY_ANDROID || UNITY_IPHONE
        TouchMethod();
#endif
    }
    #endregion

    #region Private Methods
#if UNITY_EDITOR || UNITY_STANDALONE
    void MouseMethod()
    {
        GetMouseHit(Input.mousePosition);

        #region Mouse Press
        if (Input.GetMouseButtonDown(0))
        {
            if (isHit)
            {
                mMouseCurHitPressListener = mMouseCurHitListener;

                if (mMouseCurHitPressListener != null)
                {
                    mMouseCurHitPressListener.MouseOnPress(true);
                    mMousePressing = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            if (mMouseCurHitPressListener != null)
            {
                mMouseCurHitPressListener.MouseOnPress(false);
                mMousePressing = false;
                mMouseCurHitPressListener = null;
            }
        }
        #endregion

        #region Mouse Hover
        if (!mMousePressing && isHit)
        {
            
            mMouseCurHitHoverListener = mMouseCurHitListener;

            if (mMouseOldHitHoverListener == null)
            {
                mMouseCurHitHoverListener.MouseOnHover(true, mHit);
                mMouseOldHitHoverListener = mMouseCurHitHoverListener;
            }
            else
            {
                if (mMouseCurHitHoverListener != mMouseOldHitHoverListener)
                {
                    mMouseCurHitHoverListener.MouseOnHover(true, mHit);
                    mMouseOldHitHoverListener.MouseOnHover(false, mHit);
                    mMouseOldHitHoverListener = mMouseCurHitHoverListener;
                }
            }
        }
        else
        {
            
            if (mMouseOldHitHoverListener != null)
            {
                mMouseOldHitHoverListener.MouseOnHover(false, mHit);
                mMouseOldHitHoverListener = null;
            }
        }
        #endregion

        #region Mouse Click  Double Click
        if (mMouseIsTestDoubleClick)
        {
            mMouseTestDoubleClickTime += Time.deltaTime;
            if (mMouseTestDoubleClickTime <= m_MouseDoubleClickTime)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (isHit)
                    {
                        mMouseCurDoubleClickHitListener.MouseOnDoubleClick();
                    }
                    else
                    {
                        if (onMouse_DoubleClickNull != null)
                        {
                            onMouse_DoubleClickNull();
                        }
                    }
                }
            }
            else
            {
                mMouseIsTestDoubleClick = false;
                mMouseIsTestEndDoubleClick = true;
            }
        }

        if (!mMouseIsTestEndDoubleClick)
        {
            if (!mMouseIsTestDoubleClick && Input.GetMouseButtonDown(0))
            {
                if (isHit)
                {
                    mMouseCurClickHitListener = mMouseCurHitListener;

                    mMouseCurClickHitListener.MouseOnClick();
                }


                mMouseCurDoubleClickHitListener = mMouseCurHitListener;

                mMouseIsTestDoubleClick = true;
                mMouseTestDoubleClickTime = 0f;
            }
        }
        else
        {
            mMouseIsTestEndDoubleClick = false;
        }
        #endregion
    }

    void GetMouseHit(Vector3 _mousePos)
    {
        mMouseCurMousePos = _mousePos;

        if (m_MouseHoverEnableOnlyMove ? (mMouseCurMousePos != mMouseOldMousePos) : true)
        {
            mCurFrameIsHitUI = NguiEx.IsHitUI(m_IgnoreNgui, m_IgnoreNguiColliderList);

            if (!mCurFrameIsHitUI)
            {
                mRay = m_Camera.ScreenPointToRay(mMouseCurMousePos);

                if (Physics.Raycast(mRay, out mHit, Mathf.Infinity, m_LayerMask))
                {
                    mMouseCurHitListener = mHit.collider.gameObject.GetComponent<InputExListener>();

                    if (mMouseCurHitListener != null)
                    {
                        isHit = true;
                    }
                    else
                    {
                        isHit = false;
                    }
                }
                else
                {
                    isHit = false;
                }
            }
            else
            {
                isHit = false;
            }

            if (m_MouseHoverEnableOnlyMove) mMouseOldMousePos = mMouseCurMousePos;
        }
    }
#endif

#if UNITY_ANDROID || UNITY_IPHONE
    void TouchMethod()
    {
        mCurTouchCount = Input.touchCount;

        if (mTouchDoubleClickTesting)
        {
            if (mCurTouchDoubleClickTestTime <= m_TouchDoubleClickTime)
            {
                mCurTouchDoubleClickTestTime += Time.deltaTime;
            }
            else
            {
                mTouchDoubleClickTesting = false;
            }
        }

        if (mCurTouchCount >= 1)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    #region Begin

                    GetTouchHit(Input.GetTouch(0).position);

                    if (!mTouchDoubleClickTesting)
                    {
                        mCurTouchDoubleClickTestTime = 0f;
                        mTouchDoubleClickTesting = true;

                        if (isHit)
                        {
                            //Click
                            mTouchCurHitListener.TouchClick();

                            //Press Start
                            if (mTouchOldPressListener != null)
                            {
                                mTouchOldPressListener.TouchPress(false);
                            }

                            mTouchCurHitListener.TouchPress(true);

                            mTouchOldPressListener = mTouchCurHitListener;
                        }
                    }
                    else
                    {
                        if (mCurTouchDoubleClickTestTime <= m_TouchDoubleClickTime)
                        {
                            mTouchDoubleClickTesting = false;

                            GetTouchHit(Input.GetTouch(0).position);

                            if (isHit)
                            {
                                // Double Click
                                mTouchCurHitListener.TouchDoubleClick();
                            }
                            else
                            {
                                if (onTouch_DoubleClickNull != null)
                                {
                                    onTouch_DoubleClickNull();
                                }
                            }
                        }
                    }

                    break;
                    #endregion
                case TouchPhase.Stationary:
                    #region Stationary

                    if (mTouchLongPressTesting)
                    {
                        mCurTouchLongPressTime += Time.deltaTime;

                        if (mCurTouchLongPressTime >= m_TouchLongPressTime)
                        {
                            mTouchLongPressTesting = false;

                            if (isHit)
                            {
                                //Long Press
                                mTouchCurHitListener.TouchLongPress();
                                mCurTouchLongPressTime = 0f;
                            }
                        }
                    }
                    else
                    {
                        mTouchLongPressTesting = true;
                        mCurTouchLongPressTime = 0f;
                    }
                    break;
                    #endregion

                case TouchPhase.Moved:
                    if (mTouchLongPressTesting)
                    {
                        mTouchLongPressTesting = false;
                    }
                    break;
                case TouchPhase.Ended:
                    #region Ended

                    if (mTouchOldPressListener != null)
                    {
                        mTouchOldPressListener.TouchPress(false);
                        mTouchOldPressListener = null;
                    }
                    break;
                    #endregion
            }
        }
        else
        {

            if (mTouchOldPressListener != null)
            {
                mTouchOldPressListener.TouchPress(false);

                mTouchOldPressListener = null;
            }
        }
    }

    void GetTouchHit(Vector3 _mousePos)
    {
        mCurFrameIsHitUI = NguiEx.IsHitUI(m_IgnoreNgui, m_IgnoreNguiColliderList);

        if (!mCurFrameIsHitUI)
        {
            mRay = m_Camera.ScreenPointToRay(_mousePos);

            if (Physics.Raycast(mRay, out mHit, Mathf.Infinity, m_LayerMask))
            {
                mTouchCurHitListener = mHit.collider.gameObject.GetComponent<InputExListener>();

                if (mTouchCurHitListener != null)
                {
                    isHit = true;
                }
                else
                {
                    isHit = false;
                }
            }
            else
            {
                isHit = false;
            }
        }
        else
        {
            isHit = false;
        }
    }
#endif
    #endregion
}
