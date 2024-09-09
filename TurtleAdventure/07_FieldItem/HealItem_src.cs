using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem_src : MonoBehaviour
{
/*------------- 概要 -------------------
回復アイテムオブジェクトへアタッチする。
回復アイテムオブジェクトにプレイヤーが触れた際、
Player_ctrl_srcスクリプト内のHeal関数(HP回復処理)呼び出しを行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] uint u1_g_healPoint;             // アイテム取得時のHP回復量
    [SerializeField] AudioClip ac_g_getSound;         // アイテム取得時のSE

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private AudioSource as_g_audioSource_SE; // SE用オーディオソース



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
        // SE用AudioSourceを取得
        as_g_audioSource_SE = GameObject.FindGameObjectWithTag("AudioSource_se").GetComponent<AudioSource>();

    }


    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// プレイヤーのHeal関数(HP回復処理)を呼び出す。
    /// </summary>
    /// <detail>
    /// 接触したコライダーがプレイヤーの場合、プレイヤーのHeal関数(HP回復処理)を呼び出して当オブジェクトを削除する。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // IF：接触したコライダーのGameObjectがプレイヤーか(早期リターン)
        if(!cl_l_hitCol.gameObject.CompareTag("PlayerHitCollider"))
        {
            return;   
        }
        else
        {
            // NOP
        }

        // オブジェクトのPlayer_ctrl_srcを取得する
        Player_ctrl_src sc_l_Player_ctrl_src = cl_l_hitCol.gameObject.transform.parent.GetComponent<Player_ctrl_src>(); 
        // 回復処理を呼び出し。
        sc_l_Player_ctrl_src.Heal(u1_g_healPoint);
        as_g_audioSource_SE.PlayOneShot(ac_g_getSound);
        Destroy(this.gameObject);
    }
}