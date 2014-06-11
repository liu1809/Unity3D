using UnityEngine;
using System.Collections;

public class CameraControlTest : MonoBehaviour 
{
	public CameraControl m_cameraControl;

    public UIToggle m_Toggle;

    public UIButton m_Button;
	void Awake()
	{
		
	}

	void Start() 
	{
        EventDelegate.Add(m_Toggle.onChange, delegate
        {
            m_cameraControl.MyIsOrtho = m_Toggle.value;
        });

        EventDelegate.Add(m_Button.onClick, delegate
        {
            m_cameraControl.TweenStart(new CCTween[]
            {
                new CCTweenParentRotation(m_cameraControl, Quaternion.identity, false),
                new CCTweenParentPosition(m_cameraControl, Vector3.zero, false),
                new CCTweenDistance(m_cameraControl, 5f)
            }, 1f);
        });
	}
	
	void Update() 
	{
		
	}
}
