using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class TrainingDummy_src : MonoBehaviour, IDamageable
{
/*------------- 概要 -------------------
訓練用案山子の処理全般を行う

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] DamageNumber damageNumber;     // 被ダメージ時のダメージ数エフェクト(アセット)

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Animator at_g_animator;

/*------------ 列挙体 -------------------*/
    // 訓練用案山子の状態(アニメーション変数で使用)
    private enum STATE
    {
        IDLE,       // 待機状態
        GET_HIT,    // 被ダメージ状態
    }



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        at_g_animator = gameObject.GetComponent<Animator>();
    }


    /// <summary>
    /// "IDamageable"継承関数
    /// 被ダメージ処理を行う
    /// </summary>
    /// <param name="damage">
    /// 被ダメージ値
    /// </param>
    public void Damage(int s4_l_damage)
    {
        // 被ダメージ値エフェクトを生成
        damageNumber.Spawn(this.transform.position, s4_l_damage);

        // アニメーション変数を"被ダメージ状態"にセット
        at_g_animator.SetInteger("state", (int)STATE.GET_HIT);
    }



    /// <summary>
    /// アニメーションイベント（待機状態へ遷移）
    /// </summary>
    /// <detail>
    /// 被ダメージアニメーション終了時にアニメーション変数を"待機状態"に切り替える。
    /// </detail>
    public void AnimEventStateSetIdle()
    {
        // アニメーション変数を"待機状態"にセット
        at_g_animator.SetInteger("state", (int)STATE.IDLE);
    }    
}
