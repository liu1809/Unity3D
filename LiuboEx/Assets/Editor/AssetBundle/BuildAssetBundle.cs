using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    [MenuItem("LiuboEx/Build AssetBundle OneByOne Windows")]
    static void CreateAssetBunldesOneByOneWindows()
    {
        Caching.CleanCache();
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //string[] selectPaths = AssetDatabase.GetAssetPath(Selection.gameObjects)
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            //string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

            //string targetPath = sourcePath.Substring(0, sourcePath.IndexOf('.')) + ".assetBundle";
            string targetPath = sourcePath.Replace('.', '_') + ".assetBundle_Windows";

            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    [MenuItem("LiuboEx/Build AssetBundle OneByOne Android")]
    static void CreateAssetBunldesOneByOneAndroid()
    {
        Caching.CleanCache();
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //string[] selectPaths = AssetDatabase.GetAssetPath(Selection.gameObjects)
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            //string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

            //string targetPath = sourcePath.Substring(0, sourcePath.IndexOf('.')) + ".assetBundle";
            string targetPath = sourcePath.Replace('.', '_') + ".assetBundle_Android";

            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }


    [MenuItem("LiuboEx/Build AssetBundle ALL")]
    static void CreateAssetBunldesALL()
    {
        Caching.CleanCache();

        string Path = Application.dataPath + "/StreamingAssets/ALL.assetbundle";

        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in SelectedAsset)
        {
            Debug.Log("Create AssetBunldes name :" + obj);
        }

        //这里注意第二个参数就行
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies))
        {
            AssetDatabase.Refresh();
        }
        else
        {

        }
    }
}
