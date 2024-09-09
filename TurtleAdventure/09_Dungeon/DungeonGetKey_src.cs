using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGetKey_src : MonoBehaviour
{
/*------------- 概要 -------------------
鍵オブジェクトにアタッチする。
鍵オブジェクトにプレイヤーが触れた際の鍵取得処理を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] DungeonKeyManager_src sc_g_keyManager;  // "DungenKeyManager_src"スクリプト（手持ちの鍵数管理、開錠可否判定等）設定用
    [SerializeField] bool fg_g_bossDoor;                    // ボス扉かの設定用
    [SerializeField] AudioClip ac_g_getSound;               // 鍵取得時のSE

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
    /// 鍵の取得処理
    /// </summary>
    /// <detail>
    /// 接触したコライダーがプレイヤーの場合、対象の鍵所持数を加算して当オブジェクトを削除する。
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
        // 対象のカギ所持数を加算
        sc_g_keyManager.GetDoorKey(fg_g_bossDoor);
        as_g_audioSource_SE.PlayOneShot(ac_g_getSound);
        Destroy(this.gameObject);
    }
}
