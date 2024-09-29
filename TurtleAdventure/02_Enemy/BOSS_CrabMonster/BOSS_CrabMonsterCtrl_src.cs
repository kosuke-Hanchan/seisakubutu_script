using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DamageNumbersPro;
using UnityEngine.Events;

public class BOSS_CrabMonsterCtrl_src : MonoBehaviour, IDamageable
{
/*------------- 概要 -------------------
Enemyの制御全般を行うスクリプトである。
・状態遷移
・状態に応じた行動
・アニメーション変数セット
・与ダメージ
・被ダメージ

攻撃による当たり判定、与ダメージ処理は、
「EnemyPhysicAttackDamage_src」スクリプトにて行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] bool fg_g_manegeFlg;                       // ターゲット撃破フラグ管理化設定用(T:ON、F:OFF)--「倒したら扉が開く」等に使用するか
    [SerializeField] TargetDestFlgMng_src sc_g_TargetDestFlgMng_src;  // "TargetDestFlgMng_src"スクリプト取得用
    [SerializeField] EnemyStatus_data dt_g_enemyStatus_data;    // Enemyステータス管理用スクリプタブルオブジェクト
    [SerializeField] GameObject go_g_death_effect;              // 撃破時エフェクト
    [SerializeField] DamageNumber damageNumber;                 // 被ダメージ時のダメージ数エフェクト(アセット)
    [SerializeField] CapsuleCollider cl_g_attackCollider;       // 攻撃当たり判定用コライダー
    [SerializeField] GameObject go_g_bomb;                      // 射出する爆弾オブジェクト
    [SerializeField] Transform tf_g_wr_max;                     // 右移動限界位置
    [SerializeField] Transform tf_g_wl_max;                     // 左移動限界位置

/*--------------- 定数 ----------------*/
    private const float FL_G_VIEWING_ANGLE = 15.0f;          // 視野角
    private const float FL_G_ANIM_SPEED_NORMAL = 1.2f;       // アニメーションスピード(通常)
    private const float FL_G_ANIM_SPEED_HIGH = 1.6f;         // アニメーションスピード(高速)
    private const float FL_G_ROTATE_SPEED_NORMAL = 3.0f;     // 回転速度(通常)
    private const float FL_G_ROTATE_SPEED_HIGH = 5.0f;       // 回転速度(高速)

/*------------- 代入用変数----------------*/
    private Transform tf_g_player_trfm;         // プレイヤーオブジェクトの"Transform"コンポーネント取得用
    private Vector3 vt3_g_initPos;              // 初期位置
    private Animator at_g_animator;             // "Animator"コンポーネント取得用
    private GameObject go_g_bomb_clone;         // 設置した爆弾オブジェクトの格納用
    private int s1_g_currentHealth;             // 現在HP
    private bool fg_playerIn_viewAng_flg;       // プレイヤーが視野角内に存在するか(T:存在する、F:存在しない)    
    private float fl_g_rotateSpeed;             // 移動速度
    private float fl_g_IntervalTime_atk;        // 攻撃間のクールタイム時間(乱数を代入)
    private float fl_g_IntervalTimeCount_atk;   // クールタイム計測用タイマー

/*------------ 列挙体 -------------------*/
    // 状態管理用列挙体
    public enum ENEMY_STATE
    {
        IDLE,               // 待機状態
        CHASING,
        PUNCH_ATTACK,       // 近距離攻撃状態
        BOMB_ATTACK,        // 遠距離攻撃状態
        ATTACK_AFTER,       // 攻撃後状態
        DIZZY,              // 巡回状態
        GET_HIT,            // 被ダメージ状態
        DEATH,              // 撃破状態
    }
    public ENEMY_STATE enemy_state; // Enemyの状態



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    void Awake(){
        // 初期化処理
        // "Player"タグを探し、対象オブジェクトの"Transform"を取得
        tf_g_player_trfm = GameObject.FindGameObjectWithTag("Player").transform;

        // "Animator"コンポーネントを取得
        at_g_animator = this.GetComponent<Animator>();

        // 状態を"追跡状態"に設定
        enemy_state = ENEMY_STATE.CHASING;

        // HPを取得
        s1_g_currentHealth = dt_g_enemyStatus_data.MAX_HP;

        // 攻撃間クールタイムを乱数でリセット
        fl_g_IntervalTime_atk = Random.Range(2,6);

        // アニメーションスピードを1(通常)に設定
        at_g_animator.SetFloat("Speed", FL_G_ANIM_SPEED_NORMAL);

        // 移動速度を定数で初期化
        fl_g_rotateSpeed = FL_G_ROTATE_SPEED_NORMAL;

        // 攻撃当たり判定用コライダーを無効に設定
        cl_g_attackCollider.enabled = false;
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        // プレイヤーが視野角内に存在するかを取得
        fg_playerIn_viewAng_flg = IsPlayerInViewAngle();   
        // 状態遷移判定を行う
        StateTransition();
        // 状態に応じた処理を行う      
        ActionForState();
    }



    /// <summary>
    /// 状態遷移処理
    /// </summary>
    /// <detail>
    /// 各状態時の状態遷移判定/遷移処理を行う
    /// </detail>
    private void StateTransition()
    {
        // 状態遷移
        switch (enemy_state)
        {   
            // 待機状態
            case ENEMY_STATE.IDLE:
                // IF:プレイヤーが視界外か
                if(!fg_playerIn_viewAng_flg)
                {
                    // 状態を"追跡状態"へ遷移
                    SetState(ENEMY_STATE.CHASING);                
                }
                else
                {
                    // NOP
                }
                // 攻撃状態遷移処理
                AttackTypeRandJudg();
                // NOP
                break;
            
            // 追跡状態
            case ENEMY_STATE.CHASING:
                // IF:プレイヤーが視界内か
                if(fg_playerIn_viewAng_flg)
                {
                    // 状態を"待機状態"へ遷移
                    SetState(ENEMY_STATE.IDLE);
                }
                else
                {
                    // NOP
                }
                // 攻撃状態遷移処理
                AttackTypeRandJudg();
                break;

            // 近距離攻撃状態
            case ENEMY_STATE.PUNCH_ATTACK:
                // NOP
                break;

            // 遠距離攻撃状態
            case ENEMY_STATE.BOMB_ATTACK:    
                // NOP
                break;

            // 攻撃後状態
            case ENEMY_STATE.ATTACK_AFTER:    
                // NOP
                break;

            // 混乱ダメージ状態
            case ENEMY_STATE.DIZZY:   
                // NOP
                break;

            // 被ダメージ状態
            case ENEMY_STATE.GET_HIT:   
                // NOP
                break;

            // 撃破状態
            case ENEMY_STATE.DEATH:     
                // NOP
                break;

            // それ以外
            default:
                // NOP
                break;
        }
    }



    /// <summary>
    /// 各状態に応じた行動を行う
    /// </summary>
    /// <detail>
    /// 状態に応じて下記処理を行う
    /// ・アニメーション変数の設定
    /// ・移動処理
    /// </detail>
    private void ActionForState()
    {
        // 各状態ごとの処理
        switch (enemy_state)
        {
            // 待機状態
            case ENEMY_STATE.IDLE:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.IDLE);
                break;
                
            // 追跡状態
            case ENEMY_STATE.CHASING:
                // プレイヤーの方向を向くよう回転
                RotateToTarget();
                // アニメーション変数を設定   
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.CHASING);
                break;

            // 近距離攻撃攻撃状態
            case ENEMY_STATE.PUNCH_ATTACK:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.PUNCH_ATTACK);
                break;

            // 遠距離攻撃状態
            case ENEMY_STATE.BOMB_ATTACK:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.BOMB_ATTACK);
                break;

            // 攻撃後状態
             case ENEMY_STATE.ATTACK_AFTER:
                // NOP
                break;

            // 混乱ダメージ状態
            case ENEMY_STATE.DIZZY:   
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.DIZZY);
                break;

            // 被ダメージ状態
            case ENEMY_STATE.GET_HIT:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.GET_HIT);
                break;

            // 撃破状態
            case ENEMY_STATE.DEATH:     
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.DEATH);
                // 攻撃判定用コライダーを無効化（攻撃アニメーション中に撃破された際に当たり判定が残り続けてしまうのを防ぐ）
                cl_g_attackCollider.enabled = false;
                break;

            // それ以外
            default:
                // NOP
                break;
        }
    }



    /// <summary>
    /// 正面がプレイヤーの方向に向くよう回転する処理
    /// </summary>
    private void RotateToTarget()
    {
        // ターゲットまでの方向を計算
        Vector3 direction = tf_g_player_trfm.position - transform.position;
        // ターゲットの方向を向くための回転を計算
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 現在の回転からターゲットの方向への回転をスムーズに補間
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, fl_g_rotateSpeed * Time.deltaTime);
    }



    /// <summary>
    /// 攻撃制御処理
    /// </summary>
    /// <detail>
    /// 下記処理を行う
    /// ・攻撃間クールタイムの計算/リセット(リセットは攻撃が毎度同じタイミングにならないよう乱数を使用)
    /// ・乱数による実行する攻撃タイプ判定
    /// ・各攻撃タイプへの状態遷移
    /// </detail>
    private void AttackTypeRandJudg()
    {
        // クールタイム計測用タイマーをカウント
        fl_g_IntervalTimeCount_atk += Time.deltaTime;

        // IF:クールタイム経過したか
        if(fl_g_IntervalTimeCount_atk >= fl_g_IntervalTime_atk)
        {
            // クールタイム計測用タイマーリセット
            fl_g_IntervalTimeCount_atk = 0;
            // クールタイムを再設定
            fl_g_IntervalTime_atk = Random.Range(2,5);

            // 攻撃タイプ(爆弾/物理)を乱数にて判定（タイプごとに確率を設定する場合は乱数のRange、及びcase数を変更する）
            switch(Random.Range(0,2))
            {
                // 0の場合：状態を"物理攻撃状態"に遷移
                case 0:
                    SetState(ENEMY_STATE.PUNCH_ATTACK);
                    break;

                // 1の場合：状態を"爆弾攻撃状態"に遷移(既に爆弾が設置されている場合は"物理攻撃状態に遷移")
                case 1:
                    // IF:爆弾が設置されていないか
                    if(!go_g_bomb_clone)
                    {
                        // 状態を"爆弾攻撃状態"へ遷移
                        SetState(ENEMY_STATE.BOMB_ATTACK);
                    }
                    else
                    {
                        // 状態を"近距離攻撃"へ遷移
                        SetState(ENEMY_STATE.PUNCH_ATTACK);
                    }
                    break;

                default:
                    // NOP
                    break;
            }
        }
    }



    /// <summary>
    /// プレイヤーが視野角内に存在するかを判定し、結果を返却する
    /// </summary>
    /// <returns>
    /// プレイヤーが視野角内に存在するか
    /// T：存在する
    /// F：存在しない
    /// </returns>
    private bool IsPlayerInViewAngle()
    {
        // プレイヤーの方向ベクトルを取得
        Vector3 directionToPlayer = (tf_g_player_trfm.position - transform.position).normalized;
        // Enemy(当オブジェクト)の前方ベクトルを取得
        Vector3 forwardDirection = transform.forward;
        // プレイヤーとEnemy(当オブジェクト)の角度を取得
        float angleToPlayer = Vector3.Angle(forwardDirection, directionToPlayer);
        // プレイヤーが視野角内に存在するかを判定し、結果を返却
        return angleToPlayer < FL_G_VIEWING_ANGLE * 0.5f;
    }

    

    /// <summary>
    /// 状態をセットする
    /// </summary>
    /// <param name="enemy_state">
    /// セットする状態（列挙型：PLAYER_ENEMY_STATE）
    /// </param>
    /// <detail>
    /// 引数の状態にenemy_stateをセットする
    /// </detail>
    private void SetState(ENEMY_STATE state)
    {
        enemy_state = state;
    }

    
    
    /// <summary>
    /// 2点間の距離を計算し、結果を返却する。
    /// </summary>
    /// <returns>
    /// 2点間の距離
    /// </returns>
    private float DistCalculation(Vector3 vt3_l_source_pos, Vector3 vt3_l_dest_pos)
    {
        return(vt3_l_source_pos - vt3_l_dest_pos).magnitude;
    }



/*------------- 外部呼出し関数 -----------------*/

    /// <summary>
    /// "IDamageable"継承関数
    /// 被ダメージ処理を行う(設計中)
    /// </summary>
    /// <detail>
    /// ・プレイヤーからの被ダメージ処理
    /// ・待機状態中にダメージを受けた場合、状態を"追跡状態"へ遷移する（一方的に撃破されるのを防ぐ）
    /// ・HPが0以下になった場合、状態を"撃破状態"へ遷移する。
    /// </detail>
    /// <param name="s1_l_damage">
    /// 被ダメージ値
    /// </param>
    public void Damage(int s1_l_damage)
    {
        // IF:状態が"撃破状態"である、または状態が"混乱状態でない場合は早期リターン
        if(enemy_state == ENEMY_STATE.DEATH || enemy_state != ENEMY_STATE.DIZZY)
        {
            return;
        }
        else
        {
            // NOP
        }

        // 状態を"被ダメージ状態"へ遷移
        SetState(ENEMY_STATE.GET_HIT);
        // HPから被ダメージ値を減算
        s1_g_currentHealth -= s1_l_damage;
        // 被ダメージ値エフェクトを生成
        damageNumber.Spawn(this.transform.position, s1_l_damage);

        // IF:HPが50以下か(50以下の場合アニメーション/移動速度を変化)
        if (s1_g_currentHealth <= 50)
        {
            // 移動速度を「高速」に切り替え
            fl_g_rotateSpeed = FL_G_ROTATE_SPEED_HIGH;
            // アニメーション速度を「高速」に切り替え
            at_g_animator.SetFloat("Speed", FL_G_ANIM_SPEED_HIGH);
        }
        else
        {
            // NOP
        }

        // IF:HPが0以下か
        if (s1_g_currentHealth <= 0)
        {
            // 状態を"撃破状態"に設定
            SetState(ENEMY_STATE.DEATH);
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// </summary>
    /// <detail>
    /// 接触したコライダーが爆弾オブジェクトである場合、状態を"混乱状態"へ遷移する
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // 接触したコライダーを持つGameObjectを取得
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        // IF：接触したオブジェクトが爆弾オブジェクトか
        if(go_l_hitObj == go_g_bomb_clone)
        {
            // IF：爆弾の攻撃対象がプレイヤーか(プレイヤーの場合は早期リターン)
            if(go_g_bomb_clone.GetComponent<CrabMonsterBomb_src>().GetPlayerDamagePermFlg())
            {
                return;
            }
            else
            {
                // NOP
            }

            // IF：状態が"待機状態"または"追跡状態"か
            if(enemy_state == ENEMY_STATE.IDLE || enemy_state == ENEMY_STATE.CHASING)
            {
                // 状態を"混乱状態"へ遷移
                SetState(ENEMY_STATE.DIZZY);
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



/*------------- アニメーションイベント関数 -----------------*/
    /// <summary>
    /// アニメーションイベント
    /// 攻撃アニメーション後に状態を"攻撃後状態"へ遷移する
    /// </summary>
    public void AnimEventStateSetMove()
    {
        // 状態を"攻撃後状態"へ遷移
        SetState(ENEMY_STATE.CHASING);
    }


    /// <summary>
    /// アニメーションイベント
    /// 攻撃の当たり判定用コライダーを有効化する
    /// </summary>
    public void AnimEventAttackCollEnable()
    {
        // コライダーを有効化
        cl_g_attackCollider.enabled = true;
    }


    /// <summary>
    /// アニメーションイベント
    /// 攻撃の当たり判定用コライダーを無効化する
    /// </summary>
    public void AnimEventAttackCollDisable()
    {
        // コライダーを無効化
        cl_g_attackCollider.enabled = false;
    }

    /// <summary>
    /// 爆弾オブジェクトの生成処理
    /// </summary>
    public void AnimEventShootBomb()
    {
        Quaternion rotation = Quaternion.LookRotation(this.transform.forward);
        // 爆弾オブジェクトを生成
        go_g_bomb_clone = Instantiate(go_g_bomb, this.transform.position + this.transform.forward + new Vector3(0, 0.7f, 0),
                                        this.transform.rotation * Quaternion.Euler(-90, 0, 0));
    }


    /// <summary>
    /// アニメーションイベント
    /// 呼び出してから1秒後に"DestroyObject()"関数を呼び出す。
    /// </summary>
    public void AnimEventStateSetDeath()
    {
        // 1秒後に"DestroyObject()"関数を呼び出す
        Invoke("DestroyObject",1);
    }


    /// <summary>
    /// Enemyオブジェクト(当オブジェクト)を削除し、撃破時エフェクトを生成する
    /// </summary>
    private void DestroyObject()
    {
        // 当オブジェクトを削除
        Destroy(this.gameObject);
        // 撃破時エフェクトを生成
        Instantiate(go_g_death_effect, this.transform.position + new Vector3(0,0,1), Quaternion.identity);

        // IF：ターゲット撃破フラグ化が有効か
        if(fg_g_manegeFlg)
        {
            sc_g_TargetDestFlgMng_src.OnEnemyDefeated(); // ターゲット撃破数を1加算する。
        }
        else
        {
            // NOP
        }
    }
}
