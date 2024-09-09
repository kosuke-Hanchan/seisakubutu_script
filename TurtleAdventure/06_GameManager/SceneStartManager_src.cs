using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;

public class SceneStartManager_src : MonoBehaviour
{
    [SerializeField] string saveFile = "SaveFile.es3";
    [SerializeField] string[] sc_g_scene_Name;
    [SerializeField] Transform[] startPosition;
    [SerializeField] GameObject go_g_playerObj;



    void Start()
    {
        // 前のシーン名を読み込む
        string previousSceneName = ES3.Load<string>("PreviousSceneName", saveFile);
        
        for(int i = 0; i < sc_g_scene_Name.Length; i++)
        {

            Debug.Log(sc_g_scene_Name[i] == previousSceneName);
            if(sc_g_scene_Name[i] == previousSceneName)
            {
                
                go_g_playerObj.transform.position = startPosition[i].position;
            }
            else
            {
                // NOP
            }
        }
    }
}
