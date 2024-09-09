using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMonsterBomb_src : MonoBehaviour
{

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_explosion_effect;          // 爆発エフェクト
    [SerializeField] SphereCollider cl_g_bombCollider;          // 当たり判定用コライダー
    [SerializeField] EnemyStatus_data dt_g_enemyStatus_data;    // プレイヤーステータス管理用スクリプタブルオブジェクト
    [SerializeField] bool fg_g_playerDamage_flg;

/*--------------- 定数 ----------------*/
    private const float FL_G_DESTROY_TIME = 5.0f;           // オブジェクト生存時間
    private const float FL_G_DESTROY_TIME_BUFFER = 0.1f;    // オブジェクト生存時間バッファ
    [SerializeField] float FL_G_MOVE_FORCE;

/*------------- 代入用変数----------------*/
    private float fl_g_destroy_time_cnt;    // 爆弾オブジェクトの生存時間計測用
    private bool bl_g_oneshot_flg;          // 処理をワンショット化するためのフラグ
    private GameObject go_g_playerBody;
    private Rigidbody rb_g_rigidBody;


    // Start is called before the first frame update
    void Awake()
    {
        bl_g_oneshot_flg = true;                // ワンショットフラグを有効化
        cl_g_bombCollider.enabled = false;      // 当たり判定用コライダーを無効化
        fl_g_destroy_time_cnt = 0f;             // 時間カウントを0リセット 
        fg_g_playerDamage_flg = true;           // プレイヤーへの与ダメージを許可

        // プレイヤーオブジェクトのTransformコンポ取得用
        go_g_playerBody = GameObject.FindGameObjectWithTag("PlayerBody");
        rb_g_rigidBody = GetComponent<Rigidbody>();
        rb_g_rigidBody.AddForce(new Vector3(0, 2, -2), ForceMode.Impulse);
    }



    void Update()
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
    /// 他コライダーに接触した際に呼び出される関数。
    /// </summary>
    /// <detail>
    /// ■接触したコライダーが"PlayerAttackCollider(プレイヤーの攻撃コライダー)"の場合
    /// 　- 攻撃の方向に合わせて爆弾を移動する(飛ばす)
    /// ■接触したコライダーが"Wall(壁コライダー)"または"Enemy(敵コライダー)"の場合
    /// 　- 爆弾オブジェクトの生存時間(爆発までの時間)を爆発時間に設定(即爆発)
    ///   - 爆弾オブジェクトの移動を停止
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        if(go_l_hitObj.CompareTag("Wall") || go_l_hitObj.CompareTag("Enemy"))
        {
            fl_g_destroy_time_cnt = FL_G_DESTROY_TIME;
            rb_g_rigidBody.velocity = Vector3.zero;
        }
        else
        {
            // NOP
        }

        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if(go_l_hitObj.CompareTag("PlayerAttackCollider"))
        {
            Move(go_g_playerBody.transform.forward);
            fg_g_playerDamage_flg = false;           // プレイヤーへの与ダメージを許可
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// プレイヤーのダメージ処理を呼び出す。
    /// </summary>
    /// <detail>
    /// プレイヤーへの与ダメージが許可されている際、
    /// プレイヤーの"Damage"関数(ダメージ処理)を呼び出す。
    /// </detail>
    /// <param name="go_l_hitObj">プレイヤーオブジェクト</param>
    public void BombPlayerDamage(GameObject go_l_hitObj)
    {
        // IF：プレイヤーへの与ダメージが許可されているか(禁止されている場合は早期リターン)
        if(!fg_g_playerDamage_flg)
        {
            return;
        }
        else
        {
            // NOP
        }
        
        // プレイヤーオブジェクトのIDamageableを取得する
        IDamageable damageHit = go_l_hitObj.transform.parent.GetComponent<IDamageable>(); 

        // IF:ダメージ判定が実装されていない場合、早期リターン
        if (damageHit == null)
        {
            return;
        }
        else
        {
            // NOP
        }
        // 与ダメージ
        damageHit.Damage(dt_g_enemyStatus_data.ATTACK_DAMAGE_VALUE);
    }


    /// <summary>
    /// 爆弾(当オブジェクト)の移動処理
    /// </summary>
    /// <param name="vt3_l_moveDirection">移動方向</param>
    private void Move(Vector3 vt3_l_moveDirection)
    {
        rb_g_rigidBody.AddForce(vt3_l_moveDirection * FL_G_MOVE_FORCE, ForceMode.Impulse);
    }
}
