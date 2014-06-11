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
public class FileDialogDemo : MonoBehaviour 
{

	
	void OnGUI () 
    {
        if (GUILayout.Button("打开"))
        {
            Win32FileDialog.FileInfo fileInfo = Win32FileDialog.OpenFileDialog();

            if (fileInfo != null)
            {
                Debug.Log("路径: " + fileInfo.MyPath + "\n 文件名: " + fileInfo.MyName);
            }
            else
            {
                Debug.Log("没打开");
            }
        }

        if(GUILayout.Button("另存"))
        {
            Win32FileDialog.FileInfo fileInfo = Win32FileDialog.SaveFileDialog("ABC", "abc");

            if (fileInfo != null)
            {
                Debug.Log("路径: " + fileInfo.MyPath);
            }
            else
            {
                Debug.Log("没打开");
            }
        }
	}

}
