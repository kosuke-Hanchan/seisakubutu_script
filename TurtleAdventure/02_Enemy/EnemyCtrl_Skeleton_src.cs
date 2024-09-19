using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using DamageNumbersPro;
using UnityEngine.Events;

public class EnemyCtrl_Skeleton_src : MonoBehaviour, IDamageable
{
/*------------- 概要 -------------------
敵「スケルトン」の制御全般を行うスクリプトである。
・状態遷移
・状態に応じた行動
・アニメーション変数セット
・与ダメージ
・被ダメージ
物理攻撃による当たり判定、与ダメージ処理は、
「EnemyPhysicAttackDamage_src」スクリプトにて行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] bool fg_g_manegeFlg;                       // ターゲット撃破フラグ管理化設定用(T:ON、F:OFF)--「倒したら扉が開く」等に使用するか
    [SerializeField] TargetDestFlgMng_src sc_g_TargetDestFlgMng_src;  // "TargetDestFlgMng_src"スクリプト取得用
    [SerializeField] EnemyStatus_data dt_g_enemyStatus_data;    // Enemyステータス管理用スクリプタブルオブジェクト
    [SerializeField] GameObject go_g_death_effect;              // 撃破時エフェクト
    [SerializeField] DamageNumber damageNumber;                 // 被ダメージ時のダメージ数エフェクト(アセット)
    [SerializeField] SphereCollider cl_g_attackCollider;        // 攻撃当たり判定用コライダー

/*--------------- 定数 ----------------*/
    [SerializeField] float FL_G_CHASE_RANGE = 10.0f;            // 追跡可能範囲
    [SerializeField] float FL_G_ATTACK_RANGE = 2.0f;            // 攻撃アクションへ遷移するプレイヤーとの距離
    [SerializeField] float FL_G_MAX_DIST_FROM_INIT = 20.0f;     // 初期位置からの最大距離
    [SerializeField] float FL_G_VIEWING_ANGLE = 90.0f;          // 視野角

/*------------- 代入用変数----------------*/
    private Transform tf_g_player_trfm;         // プレイヤーオブジェクトの"Transform"コンポーネント取得用
    private NavMeshAgent nva_g_navMeshAgent;    // "NavMeshAgent"コンポーネント取得用
    private Vector3 vt3_g_initPos;              // 初期位置
    private Animator at_g_animator;             // "Animator"コンポーネント取得用

    private int s1_g_currentHealth;             // 現在HP
    private float fl_g_distanceToPlayer;        // プレイヤーとの距離
    private float fl_g_distanceToInitPos;       // 初期位置との距離
    
    private bool fg_playerIn_viewAng_flg;       // プレイヤーが視野角内に存在するか(T:存在する、F:存在しない)

    

/*------------ 列挙体 -------------------*/
    // Enemy状態
    public enum ENEMY_STATE
    {
        IDLE,               // 待機状態
        CHASING,            // 追跡状態
        ATTACK_AFTER,       // 攻撃後状態
        ATTACK,             // 攻撃状態
        RETURN_INIT_POS,    // 初期位置帰還状態
        PATROL,             // 巡回状態
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

        // "NavMeshAgent"コンポーネントを取得
        nva_g_navMeshAgent = this.GetComponent<NavMeshAgent>();
        
        // "Animator"コンポーネントを取得
        at_g_animator = this.GetComponent<Animator>();

        // 初期位置を取得
        vt3_g_initPos = this.transform.position;

        // 状態を"待機状態"に設定
        enemy_state = ENEMY_STATE.IDLE;

        // HPを取得
        s1_g_currentHealth = dt_g_enemyStatus_data.MAX_HP;

        // 攻撃当たり判定用コライダーを無効に設定
        cl_g_attackCollider.enabled = false;
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        // プレイヤーとEnemy(当オブジェクト)の距離を取得
        fl_g_distanceToPlayer = DistCalculation(tf_g_player_trfm.position, transform.position);
        // 初期位置とEnemy(当オブジェクト)の距離を取得
        fl_g_distanceToInitPos = DistCalculation(vt3_g_initPos, this.transform.position);
        // プレイヤーが視野角内に存在するかを取得
        fg_playerIn_viewAng_flg = IsPlayerInViewAngle();   
        // 状態遷移判定を行う
        StateTransition();
        // 状態に応じた処理を行う(引数：Enemy状態)
        ActionForState();
    }



    /// <summary>
    /// 状態遷移処理
    /// </summary>
    private void StateTransition()
    {
        // 状態遷移
        switch (enemy_state)
        {   
            // 待機状態
            case ENEMY_STATE.IDLE:
                // IF:プレイヤーが追跡範囲内および視野角内にいる場合、状態を"追跡状態"へ遷移
                if (fl_g_distanceToPlayer < FL_G_CHASE_RANGE && fg_playerIn_viewAng_flg)
                {
                    // 状態を"追跡状態"へ遷移
                    SetState(ENEMY_STATE.CHASING);  
                }
                else
                {
                    // NOP
                }
                break;

            // 追跡状態
            case ENEMY_STATE.CHASING:   
                // IF:プレイヤーが攻撃範囲内に侵入した場合、状態を"攻撃状態"へ遷移
                if (fl_g_distanceToPlayer < FL_G_ATTACK_RANGE && fg_playerIn_viewAng_flg)
                {
                    // 状態を"攻撃状態"へ遷移
                    SetState(ENEMY_STATE.ATTACK);
                }
                else
                {
                    // NOP
                }

                // IF:プレイヤーから一定距離、または初期位置から一定距離離れているか
                if (fl_g_distanceToPlayer > FL_G_CHASE_RANGE || fl_g_distanceToInitPos > FL_G_MAX_DIST_FROM_INIT)
                {   
                    // 状態を"初期位置帰還状態"へ遷移
                    SetState(ENEMY_STATE.RETURN_INIT_POS);  
                }
                else
                {
                    // NOP
                }
                break;
                
            // 攻撃後状態
            case ENEMY_STATE.ATTACK_AFTER:
                // IF:プレイヤーが攻撃範囲内かつ視野角内か
                if (fl_g_distanceToPlayer <= FL_G_ATTACK_RANGE && fg_playerIn_viewAng_flg)
                {
                    // 状態を"攻撃状態"へ遷移
                    SetState(ENEMY_STATE.ATTACK);   
                }
                else
                {
                    // 状態を"追跡状態"へ遷移
                    SetState(ENEMY_STATE.CHASING);
                }
                break;

            // 攻撃状態
            case ENEMY_STATE.ATTACK:    
                // NOP
                break;

            // 初期位置帰還状態
            case ENEMY_STATE.RETURN_INIT_POS:   
                // IF:初期位置との距離が1.0以下か
                if (DistCalculation(vt3_g_initPos, this.transform.position) <= 1.0f)
                {
                    // 状態を"待機状態"へ遷移
                    SetState(ENEMY_STATE.IDLE);      
                }
                else
                {
                    // NOP
                }

                // IF:プレイヤーが追跡範囲内、かつ初期位置とプレイヤーの距離が範囲内、かつプレイヤーが視野角内に存在するか
                if (fl_g_distanceToPlayer < FL_G_CHASE_RANGE && DistCalculation(tf_g_player_trfm.position, vt3_g_initPos) <= FL_G_MAX_DIST_FROM_INIT
                 && fg_playerIn_viewAng_flg)
                {
                    // 状態を"追跡状態"へ遷移
                    SetState(ENEMY_STATE.CHASING);
                }
                break;

            // 被ダメージ状態
            case ENEMY_STATE.GET_HIT:   
                // 現状"被ダメージ状態"は未使用
                break;

            // 撃破状態
            case ENEMY_STATE.DEATH:     
                // NOP
                break;
            
            default:
                // NOP
                break;
        }
    }



    /// <summary>
    /// 各状態に応じた行動を行う
    /// </summary>
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
                // アニメーション変数を設定   
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.CHASING);
                nva_g_navMeshAgent.SetDestination(tf_g_player_trfm.position);
                break;

            // 攻撃後状態
             case ENEMY_STATE.ATTACK_AFTER:    
                // NOP
                break;

            // 攻撃状態
            case ENEMY_STATE.ATTACK:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.ATTACK);
                break;

            // 初期位置帰還状態
            case ENEMY_STATE.RETURN_INIT_POS:
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.RETURN_INIT_POS);
                // 初期位置へ移動
                nva_g_navMeshAgent.SetDestination(vt3_g_initPos);
                break;

            // 被ダメージ状態
            case ENEMY_STATE.GET_HIT:
                // 現状"被ダメージ状態"は未使用
                break;

            // 撃破状態
            case ENEMY_STATE.DEATH:     
                // アニメーション変数を設定
                at_g_animator.SetInteger("State", (int)ENEMY_STATE.DEATH);
                // 移動を無効化
                nva_g_navMeshAgent.isStopped = true;
                // 攻撃判定用コライダーを無効化（攻撃アニメーション中に撃破された際に残り続けてしまうのを防ぐ）
                cl_g_attackCollider.enabled = false;
                break;

            default:
                // NOP
                break;
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
        // IF:状態が"撃破状態"の場合、早期リターン
        if(enemy_state == ENEMY_STATE.DEATH)
        {
            return;
        }
        else
        {
            // NOP
        }

        // HPから被ダメージ値を減算
        s1_g_currentHealth -= s1_l_damage;
        // 被ダメージ値エフェクトを生成
        damageNumber.Spawn(this.transform.position, s1_l_damage);

        // IF:状態が"待機状態"かつプレイヤーとの距離が追跡範囲内か（待機中にダメージを受けた際、強制的に追跡状態にして一方的に撃破されるのを防ぐ）
        if(enemy_state == ENEMY_STATE.IDLE && fl_g_distanceToPlayer <= FL_G_CHASE_RANGE)
        {
            SetState(ENEMY_STATE.CHASING);
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



/*------------- アニメーションイベント関数 -----------------*/

    /// <summary>
    /// アニメーションイベント
    /// 攻撃アニメーション後に状態を"攻撃後状態"へ遷移する
    /// </summary>
    public void AnimEventAttack()
    {
        // 状態を"攻撃後状態"へ遷移
        SetState(ENEMY_STATE.ATTACK_AFTER);
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
    /// アニメーションイベント
    /// 呼び出してから1秒後に"DestroyObject()"関数を呼び出す。
    /// </summary>
    public void AnimEventDeath()
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
        Instantiate(go_g_death_effect, this.transform.position, Quaternion.identity);

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
