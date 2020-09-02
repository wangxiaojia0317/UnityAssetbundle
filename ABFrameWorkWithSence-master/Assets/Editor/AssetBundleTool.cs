using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

public class AssetBundleTool : MonoBehaviour
{
   
   [MenuItem("AssetBundle/Build")]
   private static void BuildAsset()
   {
       Debug.Log("Package AssetBundle");
       string path=Application.dataPath+"/AssetBundle";
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path,BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
        Debug.Log("打包成功");
   }


    #region 设置资源的名字
    [MenuItem("AssetBundle/SetLable")]
    private static void SetLable()
    {
        allFilesPath.Clear();

        GetAllFiles(Application.dataPath + "/Prefab");
        for (int i = 0; i < allFilesPath.Count; i++)
        {
            int tmpIndex = allFilesPath[i].FullName.IndexOf("Assets");
            string strAssetFilePath = allFilesPath[i].FullName.Substring(tmpIndex);
            AssetImporter tmpImportObj = AssetImporter.GetAtPath(strAssetFilePath);
            tmpImportObj.assetBundleName =Path.GetDirectoryName(strAssetFilePath).Replace("\\","_");
            tmpImportObj.assetBundleVariant= allFilesPath[i].Extension == ".unity" ? "u3d" : "ab";
        }

        Debug.Log("资源标签设置成功");
    }

    //AB所有的资源文件
    static List<FileSystemInfo> allFilesPath = new List<FileSystemInfo>();
    private static void GetAllFiles(string dirs)
    {
        DirectoryInfo dir = new DirectoryInfo(dirs);
        FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
        foreach (FileSystemInfo fsinfo in fsinfos)
        {
            //判断是否为文件夹　　
            if (fsinfo is DirectoryInfo)
            {
                GetAllFiles(fsinfo.FullName);
            }
            else
            {
                //将得到的文件全路径放入到集合中
                if (fsinfo.Extension == ".meta")
                {
                    continue;
                }
#if UNITY_EDITOR
                //Debug.Log(fsinfo.FullName);
#endif
                allFilesPath.Add(fsinfo);

            }
        }
    }
    #endregion

    #region Manifest

    /// <summary>
    /// 获取Manifest的依赖文件
    /// </summary>
    private void Manifest()
    {
        AssetBundle manifesAB = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundle/AssetBundle");
        AssetBundleManifest manifest = manifesAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] l = manifest.GetAllAssetBundles();
        foreach (var item in l)
        {
            string[] depends = manifest.GetAllDependencies(item);
            foreach (var a in depends)
            {
                Debug.Log(a);
            }
        }
    }

    /// <summary>
    /// 网络请求ab包
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadAssets()
    {
        string url = "file:///" + Application.dataPath + "/AssetBundle/assets_prefab_common_model_model_a.ab";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        GameObject cube = bundle.LoadAsset<GameObject>("Cube");
        Instantiate(cube);

    }
    #endregion


}
