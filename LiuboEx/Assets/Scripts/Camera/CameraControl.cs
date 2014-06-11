using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour
{
    #region Public Fields
    public Transform m_CameraParent;

    public Camera[] m_ChildCameras;

    public bool m_IgnoreNgui = true;

    public List<Collider> m_IgnoreNguiColliderList = new List<Collider>();

    public bool m_IsEnableZoom = true;

    public bool m_IsEnableMove = true;

    public float m_MinDistance = 1f;
    public float m_MaxDistance = 50f;

    public float m_MinOrthSize = 1f;
    public float m_MaxOrthSize = 50f;

    #region Mouse Control
    public float m_Mouse_RotateSpeed = 1f;

    public float m_Mouse_MoveSpeed_P = 1f;

    public float m_Mouse_MoveSpeed_O = 1f;

    public float m_Mouse_PushPullSpeed_P = 1f;

    public float m_Mouse_PushPullSpeed_O = 1f;

    public float m_Mouse_PushPullTime = 0.5f;
    #endregion

    #region Touch Control
    public float m_Touch_RotateSpeed = 1f;

    public float m_Touch_MoveSpeed_P = 1f;

    public float m_Touch_MoveSpeed_O = 1f;

    public float m_Touch_PushPullSpeed_P = 1f;

    public float m_Touch_PushPullSpeed_O = 1f;
    #endregion

    #endregion

    #region Privete Fields

    Camera mCamera;

    float curPushPullTime = 0f;

    bool mIsOrtho = false;

    bool mCurFrameIsHitUI = false;

    bool mMouseDownIsHitUI = false;

    bool mMousePushPulling = false;

    float mMousePushPullTargetOrthoSize = 0f;

    float mMousePushPullTargetDistance = 0f;

    bool mTouchBeganIsHitUI = false;

    bool mIsTweening = false;

    float mCurTweenTime = 0f;

    float mCurTweenRatio = 0f;

    #endregion

    void Start()
    {
        if (transform.parent != m_CameraParent)
        {
            transform.parent = m_CameraParent;
        }

        mCamera = camera;

        mIsOrtho = mCamera.isOrthoGraphic;

        mMousePushPullTargetOrthoSize = mCamera.orthographicSize;

        mMousePushPullTargetDistance = -mCamera.transform.localPosition.z;

        Debug.Log(gameObject.name + " CC 初始化完成.");
    }

    void Update()
    {
        //mCurFrameIsHitUI = IsHitUI();
        mCurFrameIsHitUI = NguiEx.IsHitUI(m_IgnoreNgui, m_IgnoreNguiColliderList);

        Debug.Log(mCurFrameIsHitUI);
        #region Mouse Control
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            mMouseDownIsHitUI = mCurFrameIsHitUI;
        }

        if (!mMouseDownIsHitUI)
        {
            if (Input.GetMouseButton(1) && !Input.GetMouseButton(0) && !Input.GetMouseButton(2))
            {
                MouseRotateCamera();
            }
            else if (m_IsEnableMove && Input.GetMouseButton(2) && !Input.GetMouseButton(0))
            {
                MouseMoveCamera();
            }
        }
        if (m_IsEnableZoom && !mCurFrameIsHitUI)
        {
            MousePushPullCamera();
        }
#endif
        #endregion

        #region Touch Control
#if !UNITY_EDITOR && UNITY_ANDROID || UNITY_IPHONE
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            mTouchBeganIsHitUI = mCurFrameIsHitUI;
        }

        if (!mTouchBeganIsHitUI)
        {
            if (Input.touchCount == 1)
            {
                TouchRotateCamera();
            }
            else if (m_IsEnableZoom && Input.touchCount == 2)
            {
                TouchPushPullCamera();
            }
            else if (m_IsEnableMove && Input.touchCount >= 3)
            {
                TouchMoveCamera();
            }
        }
#endif
        #endregion
    }

    #region Public Attribute
    public bool MyIsOrtho
    {
        get
        {
            return mIsOrtho;
        }
        set
        {
            if (mIsOrtho != value)
            {
                mIsOrtho = value;
                mCamera.isOrthoGraphic = mIsOrtho;

                if (mIsOrtho)
                {
                    float targetOrthoSize = Mathf.Sin(mCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * -transform.localPosition.z;

                    UpdateCamera(targetOrthoSize, Vector3.zero);
                }
                else
                {
                    float targetDistance = mCamera.orthographicSize / Mathf.Sin(mCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

                    UpdateCamera(0f, new Vector3(0f, 0f, -targetDistance));
                }
            }
        }
    }

    public Camera MyCamera
    {
        get { return mCamera; }
    }
    #endregion

    #region Public Method

    public void TweenStart(CCTween _ccTween, float _time)
    {
        TweenStart(new CCTween[] { _ccTween }, _time);
    }

    public void TweenStart(CCTween[] _ccTweens, float _time)
    {
        if (mIsTweening)
        {
            StopAllCoroutines();
        }
        else
        {
            mIsTweening = true;
        }
        StartCoroutine(IE_Tweening(_ccTweens, _time));
    }

    public void TweenStop()
    {
        mIsTweening = false;
    }

    public void TweenStop(float _ratio)
    {
        if (mCurTweenRatio > 0f && mCurTweenRatio < _ratio)
        {
            TweenStop();
        }
    }

    public void UpdateCamera(float _orthoSize, Vector3 _localPos, bool _isUpdatePullPullTargetValues = true)
    {
        if (mIsOrtho)
        {
            mCamera.orthographicSize = _orthoSize;

            if (_isUpdatePullPullTargetValues)
            {
                mMousePushPullTargetOrthoSize = _orthoSize;
            }
        }
        else
        {
            mCamera.transform.localPosition = _localPos;

            if (_isUpdatePullPullTargetValues)
            {
                mMousePushPullTargetDistance = -_localPos.z;
            }
        }

        UpdateChildCameras(mIsOrtho, _orthoSize, _localPos);
    }

    #endregion

    #region Private Method

    #region Mouse Control Method
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
    void MousePushPullCamera()
    {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScrollWheel != 0)
        {
            if (mCurTweenRatio > 0f && mCurTweenRatio < 0.25f)
            {
                TweenStop(0.25f);
            }
            mMousePushPulling = true;

            curPushPullTime = 0f;

            if (mIsOrtho)
            {
                float startOrthoSize = mCamera.orthographicSize;

                mMousePushPullTargetOrthoSize -= mouseScrollWheel * startOrthoSize * m_Mouse_PushPullSpeed_O;

                mMousePushPullTargetOrthoSize = Mathf.Clamp(mMousePushPullTargetOrthoSize, m_MinOrthSize, m_MaxOrthSize);
            }
            else
            {
                float startLocalZ = mCamera.transform.localPosition.z;

                mMousePushPullTargetDistance += mouseScrollWheel * startLocalZ * m_Mouse_PushPullSpeed_P;

                mMousePushPullTargetDistance = Mathf.Clamp(mMousePushPullTargetDistance, m_MinDistance, m_MaxDistance);
            }
        }

        if (mMousePushPulling)
        {
            curPushPullTime += Time.deltaTime;

            curPushPullTime = Mathf.Clamp(curPushPullTime, 0f, m_Mouse_PushPullTime);

            if (curPushPullTime >= m_Mouse_PushPullTime)
            {
                mMousePushPulling = false;
            }

            if (mIsOrtho)
            {
                float curOrthoSize = Mathf.Lerp(mCamera.orthographicSize, mMousePushPullTargetOrthoSize, Mathf.Clamp01(curPushPullTime / m_Mouse_PushPullTime));

                UpdateCamera(curOrthoSize, Vector3.zero, false);
            }
            else
            {
                float curDistance = Mathf.Lerp(-mCamera.transform.localPosition.z, mMousePushPullTargetDistance, Mathf.Clamp01(curPushPullTime / m_Mouse_PushPullTime));

                Vector3 curLocalPos = new Vector3(0f, 0f, -curDistance);

                UpdateCamera(0f, curLocalPos, false);
            }
        }
    }

    void MouseRotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0f || mouseY != 0f)
        {
            TweenStop(0.25f);

            if (mouseX != 0f)
            {
                m_CameraParent.Rotate(Vector3.up, m_Mouse_RotateSpeed * 4f * Input.GetAxis("Mouse X"), Space.World);
            }
            if (mouseY != 0f)
            {
                m_CameraParent.Rotate(Vector3.left, m_Mouse_RotateSpeed * 4f * Input.GetAxis("Mouse Y"), Space.Self);
            }
        }

    }

    void MouseMoveCamera()
    {
        TweenStop(0.25f);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float moveValue = 0f;

        if (mIsOrtho)
        {
            moveValue = mCamera.orthographicSize * -m_Mouse_MoveSpeed_O * 0.05f;
        }
        else
        {
            moveValue = mCamera.transform.localPosition.z * m_Mouse_MoveSpeed_P * 0.02f;
        }

        Vector3 moveVector = new Vector3(mouseX * moveValue, mouseY * moveValue, 0f);

        m_CameraParent.Translate(moveVector, transform);
    }
#endif
    #endregion

    #region Touch Control Method
#if !UNITY_EDITOR && UNITY_ANDROID || UNITY_IPHONE
    void TouchRotateCamera()
    {
        Vector2 touchMoveValue = Input.GetTouch(0).deltaPosition;

        if (touchMoveValue != Vector2.zero)
        {
            TweenStop(0.25f);

            m_CameraParent.Rotate(Vector3.up, touchMoveValue.x, Space.World);
            m_CameraParent.Rotate(Vector3.left, touchMoveValue.y, Space.Self);
        }
    }

    void TouchPushPullCamera()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            TweenStop(0.25f);

            float curTwoFingersDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

            float lastTwoFingersDistance = Vector2.Distance(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition, Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            float pushPullValue = lastTwoFingersDistance - curTwoFingersDistance;

            if (mIsOrtho)
            {
                float targetOrthSize = mCamera.orthographicSize + pushPullValue * m_Touch_PushPullSpeed_O * mCamera.orthographicSize * 0.004f;

                targetOrthSize = Mathf.Clamp(targetOrthSize, m_MinOrthSize, m_MaxOrthSize);

                mCamera.orthographicSize = targetOrthSize;

                UpdateChildCameras(mIsOrtho, targetOrthSize, Vector3.zero);
            }
            else
            {
                float targetLocalZ = mCamera.transform.localPosition.z + pushPullValue * m_Touch_PushPullSpeed_P * mCamera.transform.localPosition.z * 0.004f;

                targetLocalZ = Mathf.Clamp(-targetLocalZ, m_MinDistance, m_MaxDistance);

                Vector3 targetLocalPos = new Vector3(0f, 0f, -targetLocalZ);

                mCamera.transform.localPosition = targetLocalPos;

                UpdateChildCameras(mIsOrtho, 0f, targetLocalPos);
            }
        }
    }

    void TouchMoveCamera()
    {
        Vector2 moveVector = Vector2.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            moveVector += Input.GetTouch(i).deltaPosition;
        }

        if (moveVector != Vector2.zero)
        {
            TweenStop(0.25f);

            if (mIsOrtho)
            {
                moveVector *= mCamera.orthographicSize * -m_Touch_MoveSpeed_O * 0.002f;
            }
            else
            {
                moveVector *= mCamera.transform.localPosition.z * m_Touch_MoveSpeed_P * 0.002f;
            }

            m_CameraParent.Translate(moveVector, transform);
        }
    }
#endif
    #endregion

    bool IsHitUI()
    {
        if (m_IgnoreNgui && m_IgnoreNguiColliderList.Count > 0)
        {
            if (UICamera.lastHit.collider)
            {
                if (!m_IgnoreNguiColliderList.Contains(UICamera.lastHit.collider))
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

    void UpdateChildCameras(bool _isOrtho, float _orthoSize, Vector3 _localPos)
    {
        if (m_ChildCameras.Length > 0)
        {
            foreach (Camera one in m_ChildCameras)
            {
                if (_isOrtho)
                {
                    if (!one.isOrthoGraphic)
                    {
                        one.isOrthoGraphic = true;
                    }
                    one.orthographicSize = _orthoSize;
                }
                else
                {
                    if (one.isOrthoGraphic)
                    {
                        one.isOrthoGraphic = false;
                    }
                    one.transform.localPosition = _localPos;
                }

            }
        }
    }

    IEnumerator IE_Tweening(CCTween[] _ccTweens, float _time)
    {
        mCurTweenTime = 0f;

        mCurTweenRatio = 0f;

        while (mIsTweening)
        {
            mCurTweenTime += Time.deltaTime;

            mCurTweenRatio = Mathf.Clamp01(mCurTweenTime / _time);

            foreach (CCTween one in _ccTweens)
            {
                one.Tween(mCurTweenRatio);
            }

            if (mCurTweenTime >= _time)
            {
                break;
            }
            yield return null;
        }
        mIsTweening = false;
    }

    #endregion

}

#region Camera Tween Class

public class CCTween
{
    public CameraControl MyOrbitCamera { get; set; }

    public virtual void Tween(float _ratio)
    {

    }

}
public class CCTweenParentPosition : CCTween
{
    public Vector3 MyStartPos { get; set; }
    public Vector3 MyTargetPos { get; set; }
    public bool MyIsLocal { get; set; }

    public CCTweenParentPosition(CameraControl _cameraControl, Vector3 _targetPos, bool _isLocal)
    {
        MyOrbitCamera = _cameraControl;

        if (_isLocal)
        {
            MyStartPos = _cameraControl.m_CameraParent.localPosition;
        }
        else
        {
            MyStartPos = _cameraControl.m_CameraParent.position;
        }

        MyTargetPos = _targetPos;
        MyIsLocal = _isLocal;
    }

    public override void Tween(float _ratio)
    {
        if (MyStartPos != MyTargetPos)
        {
            if (MyIsLocal)
            {
                MyOrbitCamera.m_CameraParent.localPosition = Vector3.Lerp(MyStartPos, MyTargetPos, _ratio);
            }
            else
            {
                MyOrbitCamera.m_CameraParent.position = Vector3.Lerp(MyStartPos, MyTargetPos, _ratio);
            }
        }
    }
}
public class CCTweenParentRotation : CCTween
{
    public Quaternion MyStartRotation { get; set; }
    public Quaternion MyTargetRotation { get; set; }
    public bool MyIsLocal { get; set; }

    public CCTweenParentRotation(CameraControl _cameraControl, Quaternion _targetRotation, bool _isLocal)
    {
        MyOrbitCamera = _cameraControl;
        if (_isLocal)
        {
            MyStartRotation = _cameraControl.m_CameraParent.localRotation;
        }
        else
        {
            MyStartRotation = _cameraControl.m_CameraParent.rotation;
        }
        MyTargetRotation = _targetRotation;
        MyIsLocal = _isLocal;
    }

    public override void Tween(float _ratio)
    {
        if (MyStartRotation != MyTargetRotation)
        {
            if (MyIsLocal)
            {
                MyOrbitCamera.m_CameraParent.localRotation = Quaternion.Lerp(MyStartRotation, MyTargetRotation, _ratio);
            }
            else
            {
                MyOrbitCamera.m_CameraParent.rotation = Quaternion.Lerp(MyStartRotation, MyTargetRotation, _ratio);
            }
        }
    }
}
public class CCTweenDistance : CCTween
{
    public float MyStartDistance { get; set; }
    public float MyTargetDistance { get; set; }

    public CCTweenDistance(CameraControl _cameraControl, float _targetDistance)
    {
        MyOrbitCamera = _cameraControl;
        MyStartDistance = -_cameraControl.transform.localPosition.z;
        MyTargetDistance = _targetDistance;
    }
    public override void Tween(float _ratio)
    {
        if (!MyOrbitCamera.MyIsOrtho)
        {
            MyOrbitCamera.UpdateCamera(0f, new Vector3(0f, 0f, -Mathf.Clamp(Mathf.Lerp(MyStartDistance, MyTargetDistance, _ratio), MyOrbitCamera.m_MinDistance, MyOrbitCamera.m_MaxDistance)));
        }
        else
        {
            Debug.Log("相机是正交的 你得用 CCTweenOrthoSize");
        }
    }
}
public class CCTweenOrthoSize : CCTween
{
    public float MyStartOrthoSize { get; set; }
    public float MyTargetOrthoSize { get; set; }

    public CCTweenOrthoSize(CameraControl _cameraControl, float _targetOrthoSize)
    {
        MyOrbitCamera = _cameraControl;
        MyStartOrthoSize = _cameraControl.MyCamera.orthographicSize;
        MyTargetOrthoSize = _targetOrthoSize;
    }

    public override void Tween(float _ratio)
    {
        if (MyOrbitCamera.MyIsOrtho)
        {
            MyOrbitCamera.UpdateCamera(Mathf.Clamp(Mathf.Lerp(MyStartOrthoSize, MyTargetOrthoSize, _ratio), MyOrbitCamera.m_MinOrthSize, MyOrbitCamera.m_MaxOrthSize), Vector3.zero);
        }
        else
        {
            Debug.Log("相机是透视的 你得用 CCTweenDistance");
        }
    }
}

#endregion
