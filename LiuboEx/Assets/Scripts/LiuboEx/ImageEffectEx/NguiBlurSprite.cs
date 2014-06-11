using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(UISprite))]
public class NguiBlurSprite : MonoBehaviour 
{
    public UISprite m_Sprite;

	void Start() 
	{
		if(m_Sprite == null)
        {
            m_Sprite = gameObject.GetComponent<UISprite>();
        }
	}

	void OnEnable() 
	{
        NguiBlur.AddBlurSprite(this);
	}
    void OnDisable()
    {
        NguiBlur.RemoveBlurSprite(this);
    }

}
