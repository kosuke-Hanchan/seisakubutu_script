using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_src : MonoBehaviour
{
/*------------- 概要 -------------------
プレイヤーの爆弾設置アクション時に設置する爆弾用スクリプトである。
爆弾オブジェクトにアタッチする。

下記処理を行う。
・与ダメージ処理
・オブジェクト削除処理
・爆発時間演算処理

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_explosion_effect;          // 爆発エフェクト
    [SerializeField] SphereCollider cl_g_bombCollider;          // 当たり判定用コライダー
    [SerializeField] PlayerStatus_data dt_g_playerStatus_data;  // プレイヤーステータス管理用スクリプタブルオブジェクト

/*--------------- 定数 ----------------*/
    private const float FL_G_DESTROY_TIME = 3.0f;           // オブジェクト生存時間
    private const float FL_G_DESTROY_TIME_BUFFER = 0.1f;    // オブジェクト生存時間バッファ

/*------------- 代入用変数----------------*/
    private float fl_g_destroy_time_cnt;    // 爆弾オブジェクトの生存時間計測用
    private bool bl_g_oneshot_flg;          // 処理をワンショット化するためのフラグ



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        bl_g_oneshot_flg = true;                // ワンショットフラグを有効化
        cl_g_bombCollider.enabled = false;      // 当たり判定用コライダーを無効化
        fl_g_destroy_time_cnt = 0f;             // 時間カウントを0リセット 
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        // 経過時間計測
        fl_g_destroy_time_cnt += Time.deltaTime;
        // IF:オブジェクト生存時間経過したか
        if(fl_g_destroy_time_cnt >= FL_G_DESTROY_TIME)
        {
            // IF:ワンショットフラグが有効か
            if(bl_g_oneshot_flg)
            {
                // 爆発エフェクトを生成
                Instantiate(go_g_explosion_effect, this.transform.position,Quaternion.identity); 
                // 当たり判定用コライダーを有効化
                cl_g_bombCollider.enabled = true;
                // ワンショットフラグを無効化
                bl_g_oneshot_flg = false;
            }
            else
            {
                // NOP
            }
            
            // IF:オブジェクト生存時間+バッファ時間経過したか
            if(fl_g_destroy_time_cnt >= FL_G_DESTROY_TIME + FL_G_DESTROY_TIME_BUFFER)
            {
                // 爆弾オブジェクトを削除
                Destroy(this.gameObject);
            }
            else
            {
                // NOP
            }
            
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// 爆弾の当たり判定用コライダーにアタッチしたスクリプト(BombCollider_src)で呼び出す関数
    /// 接触した敵オブジェクトの"Damage"関数を呼び出してダメージを与える。
    /// </summary>
    /// <param name="go_l_enemyObj">
    /// 接触したGameObject
    /// </param>
    public void BombDamage(GameObject go_l_enemyObj)
    {
        // ヒットしたオブジェクトの"IDamageble"を取得
        IDamageable damageHit = go_l_enemyObj.GetComponent<IDamageable>();

        // IF:ダメージ判定が実装されていない場合、早期リターン
        if (damageHit == null)
        {
            return;
        }
        else
        {
            // NOP
        }
        // ヒットしたオブジェクトの"Damage"関数を呼び出す。(引数：与ダメージ値-乱数用範囲～与ダメージ値の乱数)
        damageHit.Damage((int)Random.Range(dt_g_playerStatus_data.BOMB_ATTACK_DAMAGE_VALUE-dt_g_playerStatus_data.DAMAGE_RANDOM_RANGE,
                                            dt_g_playerStatus_data.BOMB_ATTACK_DAMAGE_VALUE));
    }
}
