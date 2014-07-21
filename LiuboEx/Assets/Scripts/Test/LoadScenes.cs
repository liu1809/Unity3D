using UnityEngine;
using System.Collections;

using System.IO;
public class LoadScenes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUILayout.Button("asdf"))
        {
            StartCoroutine(IE_Load());
        }
    }

    IEnumerator IE_Load()
    {
        string terrainPath = Application.streamingAssetsPath + "/BuildScenes_Scene.assetbundle";
        if (System.IO.File.Exists(terrainPath))
        {
            byte[] buffer;

            using (FileStream stream = new FileStream(terrainPath, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[stream.Length];

                stream.Read(buffer, 0, (int)stream.Length);
            }

            AssetBundleCreateRequest assetRequest = AssetBundle.CreateFromMemory(buffer);

            yield return assetRequest;

            AssetBundle assetBundle = assetRequest.assetBundle;

            Application.LoadLevel("BuildScenes");

            //if (assetRequest.isDone)
            //{
            //    terrainGO = Instantiate(assetBundle.mainAsset) as GameObject;

            //    terrainGO.transform.parent = transform;
            //}


        }
    }
}
