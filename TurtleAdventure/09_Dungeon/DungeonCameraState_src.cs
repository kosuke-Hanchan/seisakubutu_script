using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DungeonCameraState_src : MonoBehaviour
{
/*------------- 概要 -------------------

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_cameraTrackingArea;                     // カメラトラッキングエリアオブジェクト
    [SerializeField] CinemachineVirtualCamera cm_g_src_chinemachine_Vcam;    // 部屋移動前CinemachineVirtualCamera取得用
    [SerializeField] CinemachineVirtualCamera cm_g_dest_chinemachine_Vcam;   // 部屋移動後CinemachineVirtualCamera取得用

    [SerializeField] GameObject[] ago_g_toDisable_Object;
    [SerializeField] GameObject[] ago_g_toEnable_Object;

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し


    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if(!cl_l_hitCol.gameObject.CompareTag("PlayerHitCollider"))
        {
            return;   
        }
        else
        {
            // NOP
        }

        // バーチャルカメラの切り替え
        cm_g_src_chinemachine_Vcam.gameObject.SetActive(true);
        cm_g_dest_chinemachine_Vcam.gameObject.SetActive(false);
        
        foreach(GameObject go_l_toDisable_Object in ago_g_toDisable_Object)
        {
            if(!(go_l_toDisable_Object == null))
            {
                go_l_toDisable_Object.gameObject.SetActive(false);
            }
            else
            {
                // NOP
            }
        }

        foreach(GameObject go_l_toEnable_Object in ago_g_toEnable_Object)
        {
            if(!(go_l_toEnable_Object == null))
            {
                go_l_toEnable_Object.gameObject.SetActive(true);
            }
            else
            {
                // NOP
            }
        }
    }
}
