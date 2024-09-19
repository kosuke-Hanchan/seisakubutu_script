using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoorCtrlProtect_src : MonoBehaviour
{
/*------------- 概要 -------------------
コライダーにアタッチする。
プレイヤーが当コライダーに触れた際、扉を開いた状態で固定する。
時限式スイッチを使用して開閉する扉にて、時限内に扉を通過したら扉を開いた状態で固定したい際に使用する。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] SwitchDoor_src[] asc_g_subjectDoor;     // 固定する対象の扉オブジェクト

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し

    /// <summary>
    /// 他コライダーに接触した際に呼び出される。
    /// プレイヤーに触れた際、SwitchDoor_src(扉オブジェクト)のアニメーション変数及び
    /// 扉操作の許可フラグを設定する。
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
        foreach(SwitchDoor_src sc_l_subjectDoor in asc_g_subjectDoor)
        {
            sc_l_subjectDoor.DoorStateTrans(true);
            sc_l_subjectDoor.fg_g_doorCtrl_perm_flg = false;
        }
    }
}
