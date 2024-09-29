using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBattleRoom_src : MonoBehaviour
{
/*------------- 概要 -------------------
本処理のトリガーとしたいコライダー（トリガー）へアタッチする。
プレイヤーがトリガーに触れた際、下記処理を行う
・部屋の扉を閉じる
・敵を出現させる
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject[] ago_g_setDoorObj;     // 対象ドアオブジェクト
    [SerializeField] GameObject[] ago_g_enemyObj;       // 出現させる敵オブジェクト
    [SerializeField] GameObject go_g_enemyApp_eff;      // 敵の出現エフェクト

    [SerializeField] bool is_changeBGM_flg;             // 敵出現時にBGMを変更するかの設定フラグ(T：変更する、F：変更しない)
    [SerializeField] AudioClip ac_g_BGM;                // 変更するBGM

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Animator at_g_animator;             // "Animator"コンポーネント取得用
    private AudioSource as_g_audioSource_bgm;    // BGM用AudioSource格納用



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
        // BGM用AudioSourceを取得
        as_g_audioSource_bgm = GameObject.FindGameObjectWithTag("AudioSource_bgm").GetComponent<AudioSource>();

        // 初期状態として扉を開く
        foreach(GameObject go_l_setDoorObj in ago_g_setDoorObj)
        {
            // 扉の開閉アニメーション変数を設定して扉を開く
            at_g_animator = go_l_setDoorObj.transform.GetComponent<Animator>();
            at_g_animator.SetBool("State", true);
        }
        // 初期状態として敵オブジェクトを非アクティブ化する
        foreach(GameObject go_l_enemyObj in ago_g_enemyObj)
        {
            // ドアオブジェクトを非アクティブ化
            go_l_enemyObj.SetActive(false); 
        }
    }


    /// <summary>
    /// 他コライダーに接触した際に呼び出される
    /// </summary>
    /// <detail>
    /// プレイヤーがトリガーに接触した際、下記処理を行う
    /// ・扉オブジェクトのアニメーション変数を設定して扉を閉じる
    /// ・非アクティブ化していた敵オブジェクトをアクティブ化する(敵を出現させる)
    /// </detail>
    /// <param name="cl_l_hitCol">接触したコライダー</param>
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

        foreach(GameObject go_l_setDoorObj in ago_g_setDoorObj)
        {
            // 扉の開閉アニメーション変数を設定して扉を閉じる
            at_g_animator = go_l_setDoorObj.transform.GetComponent<Animator>();
            at_g_animator.SetBool("State", false);
        }
        foreach(GameObject go_l_enemyObj in ago_g_enemyObj)
        {
            // 敵オブジェクトをアクティブ化
            go_l_enemyObj.SetActive(true); 
            Instantiate(go_g_enemyApp_eff, go_l_enemyObj.transform.position, Quaternion.identity);
        }

        // IF：BGMの変更フラグがT（変更する）か
        if(is_changeBGM_flg)
        {
            // BGM用AudioSourceのAudioClipを変更
            as_g_audioSource_bgm.clip = ac_g_BGM;
            as_g_audioSource_bgm.Play();
        }
        else
        {
            // NOP
        }
        
        Destroy(this.gameObject);
    }
}
