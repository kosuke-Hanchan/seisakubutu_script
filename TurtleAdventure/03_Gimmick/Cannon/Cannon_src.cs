using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class Cannon_src : MonoBehaviour, IDamageable
{
/*------------- 概要 -------------------
大砲全般の処理を行う。
・一定間隔で砲弾を射出(オブジェクト生成)
・ヒットポイント管理処理(アニメーション、被ダメージ、撃破処理)
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] DamageNumber damageNumber;                     // 被ダメージ時のダメージ数エフェクト(アセット)
    [SerializeField] GimmickStatus_data dt_g_gimmickStatus_data;    // Enemyステータス管理用スクリプタブルオブジェクト
    [SerializeField] GameObject go_g_cannonBall_obj;                // 射出する砲弾オブジェクト
    [SerializeField] GameObject go_g_death_effect;                  // 撃破時エフェクト

/*--------------- 定数 ----------------*/
    private const float FL_G_FIRING_INTERVAL_TIME = 2.0f;   // 砲弾の射出間隔

/*------------- 代入用変数----------------*/
    private float fl_g_firing_interval_time_cnt;            // 砲弾の射出間隔時間計測用
    private int s4_g_cannon_hp;                             // 大砲のヒットポイント

    private Animator at_g_animator;                         // "Animator"コンポーネント取得用
    
/*------------ 列挙体 -------------------*/
    // 大砲の状態
    private enum CANNON_STATE
    {
        NORMALLY,   // 通常状態
        DEATH       // 撃破状態
    }
    private CANNON_STATE cannon_state;


    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        at_g_animator = GetComponent<Animator>();               // "Animator"コンポーネントを取得
        s4_g_cannon_hp = dt_g_gimmickStatus_data.MAX_HP;     // スクリプタブルオブジェクトからヒットポイントを取得
        fl_g_firing_interval_time_cnt = 0;                      // 0リセット
        cannon_state = CANNON_STATE.NORMALLY;                   // 大砲の状態を"通常状態"に設定
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        ShootCannon();  // 砲弾発射処理
    }



    /// <summary>
    /// 一定間隔での砲弾を発射(生成)処理
    /// </summary>
    private void ShootCannon()
    {
        // IF:大砲の状態が"撃破状態"の場合、早期リターン
        if(cannon_state == CANNON_STATE.DEATH)
        {
            return;
        }
        else
        {
            // NOP
        }

        // 前回発射時(初回はインスタンスロード時)からの経過時間を計測
        fl_g_firing_interval_time_cnt += Time.deltaTime;

        // IF:射出間隔時間経過したか
        if(fl_g_firing_interval_time_cnt >= FL_G_FIRING_INTERVAL_TIME)
        {
            // 砲弾オブジェクトを生成(射出)
            Instantiate(go_g_cannonBall_obj, this.transform.position, this.transform.rotation);

            // 射出間隔時間計測用変数を0リセット
            fl_g_firing_interval_time_cnt = 0;
        }
        else
        {
            // NOP
        }
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
        // IF:大砲の状態が"撃破状態"の場合、早期リターン
        if(cannon_state == CANNON_STATE.DEATH)
        {
            return;
        }
        else
        {
            // NOP
        }

        // 被ダメージ値エフェクトを生成
        damageNumber.Spawn(this.transform.position, s4_l_damage);

        // ヒットポイントから被ダメージ値を減算
        s4_g_cannon_hp -= s4_l_damage;

        // IF:ヒットポイントが0以下か
        if(s4_g_cannon_hp <= 0)
        {
            // 大砲の状態を"撃破状態"に設定
            cannon_state = CANNON_STATE.DEATH;

            // 撃破アニメーションを再生
            at_g_animator.Play("Cannon_Death", 0, 0);
        }
        else
        {
            // 被ダメージアニメーションを再生
            at_g_animator.Play("Cannon_GetDamage", 0, 0);
        }
    }



    /// <summary>
    /// アニメーションイベント
    /// 撃破アニメーション終了時に呼び出される
    /// </summary>
    /// <detail>
    /// ・撃破時エフェクトの生成
    /// ・大砲オブジェクトの削除
    /// </detail>
    public void AnimEventDeath()
    {
        // 撃破時エフェクトを生成
        Instantiate(go_g_death_effect, this.transform.position, Quaternion.identity);

        // 大砲オブジェクトを削除
        Destroy(this.gameObject);
    }
}
