using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;

public class SceneStartManager_src : MonoBehaviour
{
    // [SerializeField] string saveFile = "SaveFile.es3";
    [SerializeField] string[] sc_g_scene_Name;
    [SerializeField] Transform[] startPosition;
    [SerializeField] GameObject go_g_playerObj;

    string previousSceneName;

    public void SceneStart()
    {
        // // 移動前のシーン名を取得
        // if (ES3.KeyExists("previousScene"))
        // {
        //     previousSceneName = ES3.Load<string>("previousScene");
        // }
        
        // for(int i = 0; i < sc_g_scene_Name.Length; i++)
        // {

        //     if(sc_g_scene_Name[i] == previousSceneName)
        //     {
        //         go_g_playerObj.transform.position = startPosition[i].position;
        //     }
        //     else
        //     {
        //         // NOP
        //     }
        // }
    }
}
