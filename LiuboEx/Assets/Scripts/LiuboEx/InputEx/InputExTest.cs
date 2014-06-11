using UnityEngine;
using System.Collections;

public class InputExTest : MonoBehaviour 
{
    public GameObject m_TestGO;

    public GameObject m_TestGO2;

    public InputEx inputEx;

    void OnGUI()
    {
        GUILayout.Box("Click Count:" + clickCount + "\n" + "Double Click Count:" + doubleClickCount + "\n" + "Long Press Count:" + longPressCount + "\n" + "Press T Count:" + pressTCount + "\n" + "Press F Count:" + pressFCount);


        GUILayout.Box("Click Name  " + clickName);
    }

    string clickName = " ";

    int clickCount = 0;

    int doubleClickCount = 0;

    int longPressCount = 0;

    int pressTCount = 0;

    int pressFCount = 0;

	void Start() 
	{
        InputExListener.Get(m_TestGO).onMouseClick += delegate(GameObject _go)
        {
            print("CC " + _go.name);
        };

        InputExListener.Get(m_TestGO).onMouseDoubleClick += delegate(GameObject _go)
        {
            print("DC " + _go.name);
        };

        InputExListener.Get(m_TestGO).onMousePress += delegate(GameObject _go, bool _isPress)
        {
            print("PP" + _go.name + "  " + _isPress.ToString());
        };

        InputExListener.Get(m_TestGO).onMouseHover += delegate(GameObject _go, bool _isPress)
        {
            print("Hover" + _go.name + "  " + _isPress.ToString());
        };

        InputExListener.Get(m_TestGO).onTouchClick += delegate(GameObject _go)
        {
            clickCount++;
            clickName = _go.name;
        };

        InputExListener.Get(m_TestGO).onTouchDoubleClick += delegate(GameObject _go)
        {
            doubleClickCount++;
            clickName = _go.name;
        };

        InputExListener.Get(m_TestGO).onTouchPress += delegate(GameObject _go, bool _isPress)
        {
            if (_isPress)
            {
                pressTCount++;
                clickName = _go.name;
            }
            else
            {
                pressFCount++;
                clickName = _go.name;
            }
        };

        InputExListener.Get(m_TestGO).onTouchLongPress += delegate(GameObject _go)
        {
            longPressCount++;
            clickName = _go.name;
        };


        ///

        InputExListener.Get(m_TestGO2).onTouchClick += delegate(GameObject _go)
        {
            clickCount++;
            clickName = _go.name;
        };

        InputExListener.Get(m_TestGO2).onTouchDoubleClick += delegate(GameObject _go)
        {
            doubleClickCount++;
            clickName = _go.name;
        };

        InputExListener.Get(m_TestGO2).onTouchPress += delegate(GameObject _go, bool _isPress)
        {
            if (_isPress)
            {
                pressTCount++;
                clickName = _go.name;
            }
            else
            {
                pressFCount++;
                clickName = _go.name;
            }
        };

        InputExListener.Get(m_TestGO2).onTouchLongPress += delegate(GameObject _go)
        {
            longPressCount++;
            clickName = _go.name;
        };

	}
	
	void Update() 
	{
		
	}


}
