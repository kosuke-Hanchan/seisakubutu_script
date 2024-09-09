using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonKeyDoor_src : MonoBehaviour
{
/*------------- 概要 -------------------
施錠された扉にアタッチする。
鍵を使用して施錠された扉の開錠を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] DungeonKeyManager_src sc_g_keyManager;  // キーマネージャー"DungenKeyManager_src"用（鍵の所持数等の管理用スクリプト）
    [SerializeField] bool fg_g_bossDoor;                    // ボス用扉か(T:ボス用扉、F:通常扉)
    [SerializeField] AudioClip ac_g_doorOpen_se;          // 扉の開錠SE

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Animator at_g_animator;     // "Animator"コンポーネント取得用
    private AudioSource as_g_audioSource;   // SE用オーディオソース
   

    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
        // AudioSourceコンポーネント取得
        as_g_audioSource = this.transform.GetComponent<AudioSource>();
        // 扉の開閉アニメーション変数を設定して扉を閉じる（初期値）
        at_g_animator = this.transform.GetComponent<Animator>();
        at_g_animator.SetBool("State", false);

    }

    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// 扉の鍵を開錠する。
    /// </summary>
    /// <detail>
    /// 接触したコライダーがプレイヤーの場合、対象の鍵所持数を使用して当オブジェクトを削除(開錠)する。
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

        // IF:対象のカギを1つ以上所持しているか
        if(sc_g_keyManager.UseDoorKey(fg_g_bossDoor))
        {

            // アニメーション変数をtrueに設定（扉を開く）
            at_g_animator.SetBool("State", true);
            // 扉開錠時のSEを再生
            as_g_audioSource.PlayOneShot(ac_g_doorOpen_se);
        }
        else
        {
            // NOP
        }
    }
}
