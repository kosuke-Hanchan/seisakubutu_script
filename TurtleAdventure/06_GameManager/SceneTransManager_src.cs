using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ES3Internal;

public class SceneTransManager_src : MonoBehaviour
{
    private SceneFader_src sc_g_SceneFader_src;

    public string saveFile = "SaveFile.es3";
    [SerializeField] string sc_g_scene_Name;
    private Player_ctrl_src sc_g_Player_ctrl_src;


    private void Awake()
    {
        sc_g_SceneFader_src = GameObject.FindGameObjectWithTag("SceneFader").GetComponent<SceneFader_src>();
        sc_g_Player_ctrl_src = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ctrl_src>();
    }


    private void ChangeScene()
    {
        sc_g_SceneFader_src.FadeToScene(sc_g_scene_Name);
    }


    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        
        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!cl_l_hitCol.gameObject.CompareTag("PlayerHitCollider"))
        {
            return;   
        }
        else
        {
            // NOP
        }
        // 現在のシーン名を保存
        // string currentScene = SceneManager.GetActiveScene().name;
        // ES3.Save<string>("previousScene", currentScene);
        // プレイヤーの操作を禁止
        sc_g_Player_ctrl_src.StartEventMoveCtrl();
        ChangeScene();
    }
}