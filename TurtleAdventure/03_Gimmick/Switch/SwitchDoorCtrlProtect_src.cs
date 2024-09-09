using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoorCtrlProtect_src : MonoBehaviour
{
/*------------- 概要 -------------------

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] SwitchDoor_src[] sc_g_subjectDoor;

/*--------------- 定数 ----------------*/

/*------------- 代入用変数----------------*/

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cl_l_hitCol"></param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;
        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!go_l_hitObj.CompareTag("PlayerHitCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }
        foreach(SwitchDoor_src sc_l_subjectDoor in sc_g_subjectDoor)
        {
            sc_l_subjectDoor.DoorStateTrans(true);
            sc_l_subjectDoor.fg_g_doorCtrl_perm_flg = false;
        }
    }
}
