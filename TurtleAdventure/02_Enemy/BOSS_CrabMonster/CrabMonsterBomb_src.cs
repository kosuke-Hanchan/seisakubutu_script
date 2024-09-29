using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMonsterBomb_src : MonoBehaviour
{

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_explosion_effect;          // 爆発エフェクト
    [SerializeField] SphereCollider cl_g_bombCollider;          // 攻撃判定用コライダー
    [SerializeField] SphereCollider cl_g_hitCollider;              // 当たり判定用コライダー
    [SerializeField] EnemyStatus_data dt_g_enemyStatus_data;    // プレイヤーステータス管理用スクリプタブルオブジェクト
    [SerializeField] Color colr_g_startEmissionColor;           // 爆弾生成時のEmissionカラー
    [SerializeField] Color colr_g_endEmissionColor;             // 爆弾初期移動処理後のEmissionカラー
/*--------------- 定数 ----------------*/
    private const float FL_G_DESTROY_TIME = 5.0f;           // オブジェクト生存時間
    private const float FL_G_DESTROY_TIME_BUFFER = 0.1f;    // オブジェクト生存時間バッファ(即時削除した場合当たり判定がシビアになるため)
    private float FL_G_MOVE_FORCE = 10.0f;                  // プレイヤーの攻撃により爆弾を跳ね返す際の力
    private float FL_G_INIT_MOVE_DURATION = 10.0f;          // 初期移動時の速度
    private float FL_G_INIT_MOVE_DIST = 5.0f;               // 初期移動の距離

/*------------- 代入用変数----------------*/
    private float fl_g_destroy_time_cnt;    // 爆弾オブジェクトの生存時間計測用
    private bool fg_g_oneshot_flg;          // 処理をワンショット化するためのフラグ
    private GameObject go_g_playerBody;     // プレイヤーオブジェクト取得用(プレイヤーの向きを取得する際に使用)
    private Rigidbody rb_g_rigidBody;       // 爆弾のRigitBodyコンポーネント取得用
    private Vector3 vt3_g_startPosition;    // 爆弾生成時の初期位置取得用
    private Vector3 vt3_g_targetPosition;   // 爆弾生成時の初回移動目標位置
    private bool fg_g_bombAddForce_perm_flg;// 爆弾へのAddForce（RigidBody）許可フラグ（T:許可,F:禁止）※連続でAddForceするのを防ぐ
    private bool fg_g_playerDamage_flg;     // 爆弾の攻撃対象
    private bool fg_g_isInitMove_comp;      // 初期移動処理の完了フラグ(T：完了,F：未完了)※初期移動完了時に移動処理を禁止するため
    private Material mt_g_bomb_mat;    // オブジェクトのマテリアル
    private Renderer rdr_g_objectRenderer;
    
    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    void Awake()
    {
        // Rendererを取得してマテリアルを設定
        rdr_g_objectRenderer = GetComponent<Renderer>();
        mt_g_bomb_mat = rdr_g_objectRenderer.material;
        mt_g_bomb_mat.EnableKeyword("_EMISSION");
        // Emissionカラーを変更
        mt_g_bomb_mat.SetColor("_EmissionColor", colr_g_startEmissionColor * 1);

        go_g_playerBody = GameObject.FindGameObjectWithTag("PlayerBody");   // プレイヤーオブジェクトを取得
        rb_g_rigidBody = GetComponent<Rigidbody>();     // RigidBodyコンポーネントを取得
        rb_g_rigidBody.isKinematic = true;              // RigidBodyのisKinematicをtrueに設定(物理演算を無効化)
        fg_g_oneshot_flg = true;                        // ワンショットフラグを有効化
        cl_g_bombCollider.enabled = false;              // 攻撃判定用コライダーを無効化
        fl_g_destroy_time_cnt = 0f;                     // 時間カウントを0リセット 
        fg_g_playerDamage_flg = true;                   // プレイヤーへの与ダメージを許可
        fg_g_bombAddForce_perm_flg = true;              // 爆弾へのAddForceを許可
        vt3_g_startPosition = this.transform.position;  // 爆弾生成時の初期位置を取得
        fg_g_isInitMove_comp = false;                   // 初期移動処理を未完了に設定
        // 爆弾生成時の初回移動目標位置を取得
        vt3_g_targetPosition = this.transform.position - transform.up * FL_G_INIT_MOVE_DIST;
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    void Update()
    {
        // 初期移動処理の完了フラグがFalse(未完了)か
        if(!fg_g_isInitMove_comp){
            // 爆弾生成時の初回移動処理
            BombInitialMove();
        }
        else
        {
            // NOP
        }

        // 経過時間計測
        fl_g_destroy_time_cnt += Time.deltaTime;
        // IF:オブジェクト生存時間経過したか
        if(fl_g_destroy_time_cnt >= FL_G_DESTROY_TIME)
        {
            // IF:ワンショットフラグが有効か
            if(fg_g_oneshot_flg)
            {
                // 爆発エフェクトを生成
                Instantiate(go_g_explosion_effect, this.transform.position,Quaternion.identity); 
                // 攻撃判定用コライダーを有効化
                cl_g_bombCollider.enabled = true;
                // ワンショットフラグを無効化
                fg_g_oneshot_flg = false;
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
    /// 爆弾生成時の初回移動処理
    /// </summary>
    private void BombInitialMove()
    {
        // オブジェクトを目標位置に向かって移動させる
        this.transform.position = Vector3.MoveTowards(this.transform.position, vt3_g_targetPosition, FL_G_INIT_MOVE_DURATION * Time.deltaTime);
        
        // IF：位置が初回移動目標位置に到達したか
        if(this.transform.position == vt3_g_targetPosition)
        {
            // 初期移動処理完了フラグをTrue(完了)に設定（初期移動処理を禁止）
            fg_g_isInitMove_comp = true;
        }
        else
        {
            // NOP
        }

        // 目的地までの距離に応じてEmission値を変更する
        float fl_l_distanceToTarget = Vector3.Distance(this.transform.position, vt3_g_targetPosition);
        float fl_l_maxDistance = FL_G_INIT_MOVE_DIST;  // 距離の基準となる最大値（調整可能）

        // Emissionの色を設定 (徐々に明るくなる)
        Color colr_l_emissionColor = Color.Lerp(colr_g_endEmissionColor, colr_g_startEmissionColor, fl_l_distanceToTarget / fl_l_maxDistance);
        mt_g_bomb_mat.SetColor("_EmissionColor", colr_l_emissionColor * 1);
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

        // IF：初期移動処理完了フラグがTrue（完了）か(初期移動が完了しているか)
        if(fg_g_isInitMove_comp)
        {
            // IF：接触したコライダーのタグが"PlayerAttackCollider（プレイヤーの攻撃判定用コライダー）"か
            if(go_l_hitObj.CompareTag("PlayerAttackCollider"))
            {
                if(fg_g_bombAddForce_perm_flg)
                {
                    // 物理演算を有効化
                    rb_g_rigidBody.isKinematic = false;
                    // プレイヤーの攻撃方向へ爆弾をAddForce（爆弾を飛ばす）
                    rb_g_rigidBody.AddForce(go_g_playerBody.transform.forward * FL_G_MOVE_FORCE, ForceMode.Impulse);
                    // プレイヤーへの与ダメージを禁止
                    fg_g_playerDamage_flg = false;
                    // 爆弾のAddForceを禁止(連続でAddForceされるのを防ぐため)
                    fg_g_bombAddForce_perm_flg = false;
                    // 当たり判定用コライダーを無効化
                    cl_g_hitCollider.enabled = false;
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

            // IF：接触したコライダーのタグが"Wall"または"Enemy"か
            if(go_l_hitObj.CompareTag("Wall") || go_l_hitObj.CompareTag("Enemy"))
            {
                // 即時爆発
                fl_g_destroy_time_cnt = FL_G_DESTROY_TIME;
            }
            else
            {
                // NOP
            }
        }
        else
        {
            // IF：接触したコライダーが"PlayerHitCollider（プレイヤーの当たり判定用コライダー）"か
            if(go_l_hitObj.CompareTag("PlayerHitCollider")){
                // 即時爆発してプレイヤーにダメージを与える
                fl_g_destroy_time_cnt = FL_G_DESTROY_TIME;
                // 初期移動処理完了フラグをTrue(完了)に設定（初期移動処理を中止）
                fg_g_isInitMove_comp = true;
            }
            else
            {
                // MOP
            }
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
    /// 外部スクリプトで"fg_g_playerDamage_flg"を取得するためのゲッター関数
    /// </summary>
    public bool GetPlayerDamagePermFlg()
    {
        return fg_g_playerDamage_flg;
    }
}
