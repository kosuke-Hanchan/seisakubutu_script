using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoor_src : MonoBehaviour
{
/*------------- 概要 -------------------
アニメーション変数のセッタースクリプト
外部から、当スクリプトをアタッチした扉オブジェクトの
アニメーション変数を操作を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
// 無し

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    public bool fg_g_doorCtrl_perm_flg = true;  // 扉操作の許可フラグ(T:許可, F:禁止) -- タイマー式スイッチ使用時等、一度通ったドアを閉じないようにする
    private Animator at_g_animator;             // "Animator"コンポーネント取得用



    /// <summary>
    /// アニメーション変数を引数"fg_l_doorState_flg"に設定する。
    /// </summary>
    /// <param name="fg_l_doorState_flg">扉の開閉状態</param>
    public void DoorStateTrans(bool fg_l_doorState_flg)
    {
        if(!fg_g_doorCtrl_perm_flg)
        {
            return;
        }
        else
        {
            // NOP
        }
        at_g_animator = this.transform.GetComponent<Animator>();
        at_g_animator.SetBool("State", fg_l_doorState_flg);
    }
}
