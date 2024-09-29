using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionTypeUnlockItem_src : MonoBehaviour
{
/*------------- 概要 -------------------

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] AudioClip ac_g_getSound;                       // アイテム取得時のSE
    [SerializeField] Player_ctrl_src.ACTION_TYPE unlockActionType;  // 解放するアクションタイプ

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Player_ctrl_src sc_g_Player_Ctrl_src;    // sc_g_Player_Ctrl_srcスクリプト格納用
    private int[] au1_g_playerType;
    private AudioSource as_g_audioSource_SE;         // SE用オーディオソース


    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    void Awake()
    {
        // "Player"タグを探し、対象オブジェクトの"Transform"を取得
        sc_g_Player_Ctrl_src = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ctrl_src>();
        // SE用AudioSourceを取得
        as_g_audioSource_SE = GameObject.FindGameObjectWithTag("AudioSource_se").GetComponent<AudioSource>();
    }



    /// <summary>
    /// 他コライダーに接触した際に呼び出される関数
    /// プレイヤー接触時、指定のアクションタイプを解放する。
    /// </summary>
    /// <param name="cl_l_hitCol">接触したコライダー</param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        // IF：接触したコライダーのタグが"PlayerAttackCollider（プレイヤーの攻撃判定用コライダー）"か
        if(!go_l_hitObj.CompareTag("PlayerHitCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }
        sc_g_Player_Ctrl_src.UnlockAbility((int)unlockActionType);
        as_g_audioSource_SE.PlayOneShot(ac_g_getSound);
        Destroy(this.gameObject);
    }
}
