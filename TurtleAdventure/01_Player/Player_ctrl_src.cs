using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Player_ctrl_src : MonoBehaviour,IDamageable
{
/*------------- 概要 -------------------
プレイヤー関連オブジェクトをまとめた親オブジェクトにアタッチする。
プレイヤー操作の全般を担うスクリプトである。

状態遷移による下記処理を行う。
・移動処理
・アクションタイプ(選択式)に応じたアクション処理
　-ショット攻撃
　-飛び道具攻撃
　-防御
　-爆弾
　-ダッシュ
・アニメーション変数のセット処理

外的要因で呼び出される下記関数を記述。
・被ダメージ処理
・アニメーションイベント
・奈落落下時の復帰処理及び復帰位置の記憶処理
*/



/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] PlayerHeartManager_src sc_g_PlayerHeartManager_src;    // "PlayerHeartManager_src"スクリプト(ハートアイコン制御用スクリプト)
    [SerializeField] SelectTypeMenu_src sc_g_SelectTypeMenu_src;            // "TypeSelectMenu_src"スクリプト
    [SerializeField] SceneStartManager_src sc_g_SceneStartManager_src;      // "SceneStartManager_src"スクリプト
    [SerializeField] PlayerStatus_data dt_g_playerStatus_data;              // プレイヤーステータス管理用スクリプタブルオブジェクト
    [SerializeField] FloatingJoystick inputJoyStick;                        // ジョイスティック(アセット)オブジェクト
    [SerializeField] GameObject go_g_playerMdlObj;                          // プレイヤーモデルオブジェクト
    [SerializeField] SphereCollider cl_g_shotAttackCollider;                // ショット攻撃の当たり判定用コライダー
    [SerializeField] Texture[] att_g_actionType_Texture;                    // 各アクションタイプで切り替えるプレイヤーオブジェクトテクスチャ
    [SerializeField] SkinnedMeshRenderer sm_playerModel_smr;                // プレイヤーモデルオブジェクトのスキンメッシュレンダラー
    [SerializeField] Animator at_g_playerMat_anmtr;                         // プレイヤーモデルオブジェクトのマテリアル用アニメーター

    [SerializeField] GameObject go_g_gunBullet;                             // 弾オブジェクト 
    [SerializeField] GameObject go_g_bomb;                                  // 爆弾オブジェクト

    [SerializeField] GameObject go_g_death_effect;                          // 撃破時エフェクト
    [SerializeField] GameObject go_g_shot_effect;                           // ショット攻撃エフェクトオブジェクト
    [SerializeField] GameObject go_g_gun_effect;                            // 弾発射エフェクト

    [SerializeField] AudioSource as_g_audioSource_FootStep;                 // 足音用オーディオソース
    [SerializeField] AudioSource as_g_audioSource_etc;                      // その他サウンドエフェクト用オーディオソース
    [SerializeField] AudioClip ac_g_shotAttack_se;                          // ショット攻撃SE
    [SerializeField] AudioClip ac_g_bombSet_se;                             // 爆弾設置時SE
    [SerializeField] AudioClip ac_g_guard_se;                               // 防御時SE
    [SerializeField] AudioClip ac_g_gunAttack_se;                           // 銃攻撃時SE
    [SerializeField] AudioClip ac_g_getDamage_se;
    [SerializeField] AudioClip[] aac_g_footstepSound;                       // 足音SE


/*--------------- 定数 ----------------*/
    private const float FL_G_TRANS_ACTION_STT_TIME = 0.2f;      // ジョイスティックがアクションポイント範囲内の時、アクション状態へ遷移するまでの時間
    private const float FL_G_STICK_MAX_DIST = 0.2f;             // ジョイスティックのアクションポイント範囲
    private const float FL_G_WALK_SPEED = 8.0f;                 // 移動速度
    private const float FL_G_DASH_SPEED = 15.0f;                // ダッシュ時の移動速度
    private const float FL_G_AIR_MOVE_SPEED = 6.0f;             // 空中移動速度
    private const float FL_G_GRAVITY = 30f;                     // 落下力
    private const float FL_G_SHOT_ATTACK_SPEED = 25f;           // ショット攻撃時の移動速度
    private const float FL_G_SHOT_ATTACK_CONT_TIME = 0.2f;      // ショット攻撃の継続時間（攻撃距離に直結）

/*------------- 代入用変数----------------*/
    private Transform tf_g_playerMdlObj_trfm;           // プレイヤーモデルオブジェクトの"Transform"コンポーネント取得用変数
    private CharacterController cc_g_player_charaCon;   // "CharacterController"コンポーネント取得用変数
    private Animator at_g_player_anmtr;                 // "Animator"コンポーネント取得用
    private Vector3 vt3_g_fall_velocity;                // 落下速度
    private GameObject go_g_bomb_clone;                 // 設置した爆弾オブジェクトの格納用
    private Vector3 lastSafePosition;                   // 最後に居た安全エリアの格納用（奈落への落下時の復帰位置取得用）

    private float fl_g_shot_time_cnt;                   // ショット攻撃の継続時間の計測用
    private float fl_g_action_time_cnt;                 // アクション状態遷移時間の計測用
    private float fl_g_getDamagePermflg_rtn_count;      // 被ダメージ処理許可フラグ復帰時間の計測用

    private bool fg_g_guardSE_oneshot_flg;              // 防御アクション時のSE再生をワンショット化するためのフラグ（T:有効、F:無効）
    private bool fg_g_toStandby_perm_flg;               // アクション待機状態遷移処理許可フラグ（T:許可、F:禁止）
    private bool fg_g_click_state_flg;                  // 画面押下状態フラグ（T:押下中、F:非押下中）
    private bool fg_g_stickPoint_inWaitStt;             // アクション待機状態中のジョイスティックがアクションポイント範囲内か（T:範囲内、F:範囲外）
    private bool fg_g_getDamage_perm_flg;               // 被ダメージ処理許可フラグ（T:許可、F:禁止）-- 連続的に被ダメージ処理を行うことを防ぐ
    private bool fg_g_canCtrl_flg;                      // プレイヤー操作の許可フラグ(T:許可, F:禁止) -- 強制移動イベント時に使用

    public int s4_g_playerCurrent_HP;                   // プレイヤーの現在HP



/*------------ 列挙体 -------------------*/
    // プレイヤー状態
    public enum PLAYER_STATE
    {
        IDLE,           // 待機状態
        MOVE,           // 通常移動状態
        ACTION_STANDBY, // アクション待機状態
        ACTION_BEGIN,   // アクション開始状態
        GET_HIT,        // 被ダメージ状態
        SHOT,           // ショット攻撃状態
        GUN,            // 弾発射状態
        GUARD,          // 防御状態
        SET_BOMB,       // 爆弾設置状態
        DASH,           // ダッシュ状態
        DEATH           // 撃破状態
    }
    public PLAYER_STATE player_state;
    
    
    // プレイヤーのアクションタイプ
    public enum ACTION_TYPE
    {
        SHOT,       // ショット攻撃
        GUN,        // 飛び道具攻撃
        GUARD,      // 防御
        BOMB,       // 爆弾
        DASH,       // ダッシュ
    }
    public ACTION_TYPE action_type;

    // アクションタイプとその解放状態を管理する辞書
    private Dictionary<int, bool> ActionTypeStatus;

/*--------------- デバッグ用---------------*/
    [SerializeField] TextMeshProUGUI Debug_Text1;   // デバッグ用テキスト
    [SerializeField] TextMeshProUGUI Debug_Text2;   // デバッグ用テキスト

        

    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
       // 初期化・コンポーネント取得
        tf_g_playerMdlObj_trfm = go_g_playerMdlObj.transform;
        cc_g_player_charaCon = GetComponent<CharacterController>();
        at_g_player_anmtr =  go_g_playerMdlObj.GetComponent<Animator>();
        
        player_state = PLAYER_STATE.IDLE;           // プレイヤー状態を"待機状態"に設定
        action_type = ACTION_TYPE.SHOT;             // アクションタイプを"ショット攻撃"に設定
        cl_g_shotAttackCollider.enabled = false;    // ショット攻撃の当たり判定用コライダーを無効化
        fl_g_shot_time_cnt = 0;                     // 0リセット
        fl_g_action_time_cnt = 0;                   // 0リセット
        fl_g_getDamagePermflg_rtn_count = 0;        // 0リセット
        fg_g_guardSE_oneshot_flg = true;            // 防御アクション時のSE再生ワンショットフラグをtrue(有効)に設定
        fg_g_toStandby_perm_flg = true;             // アクション待機状態遷移処理許可フラグをfalse(禁止)に設定
        fg_g_click_state_flg = false;               // 画面押下状態フラグをfalse(非押下中)に設定
        fg_g_canCtrl_flg = true;                    // プレイヤー操作の許可フラグをTrue(許可)に設定

        
        // 現在HPの初期化(現段階ではプレイヤー最大HPに設定)
        s4_g_playerCurrent_HP = dt_g_playerStatus_data.MAX_HP;

        // HPゲージマネージャーの初期化
        sc_g_PlayerHeartManager_src.InitializeHeartManager(dt_g_playerStatus_data.MAX_HP, s4_g_playerCurrent_HP);

        // 各アクションタイプの初期解放状態を設定（デモプレイ用として爆弾タイプのみ未開放設定とする）
        ActionTypeStatus = new Dictionary<int, bool>
        {
            { (int)ACTION_TYPE.SHOT, true },
            { (int)ACTION_TYPE.GUN, true },
            { (int)ACTION_TYPE.GUARD, true },
            { (int)ACTION_TYPE.BOMB, false },
            { (int)ACTION_TYPE.DASH, true }
        };
        
        // アクションタイプ選択メニューの初期化
        sc_g_SelectTypeMenu_src.SetupRadialButtons(ActionTypeStatus);
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    { 
        Debug_Text1.text = "TYPE : " + (ACTION_TYPE)action_type;
        Debug_Text2.text = "STATE : " + (PLAYER_STATE)player_state;

        // IF:プレイヤーの状態が"撃破状態"か（死亡状態の場合は早期リターンを行い、操作等の処理を行わない）
        if(player_state == PLAYER_STATE.DEATH)
        {
            return;
        }
        else
        {
            // NOP
        }

        // IF:プレイヤー操作が許可されているか(強制移動イベント中ではないか)
        if(fg_g_canCtrl_flg)
        { 
            SetActionState();           // ジョイスティック操作による状態遷移処理
            PlayerMove(player_state);   // プレイヤーオブジェクト移動処理
            PlayerRotate(player_state); // プレイヤーオブジェクト回転処理
        }
        else
        {
            // NOP
        }
        ActionType(action_type);    // アクションタイプに応じた処理
        SetAnimCtrl(player_state);  // アニメーション変数のセット処理
        GetDamagePermFlgReset();    // 被ダメージ処理許可フラグを一定間隔でリセット
        GetLastSafePosition();      // 奈落に落下した際の復帰地点記憶処理
    }


    /// <summary>
    /// アクションタイプの判定を行い、結果に応じてアクション関数を呼び出す。
    /// </summary>
    /// <param name="action_type">現在のアクションタイプ</param>
    private void ActionType(ACTION_TYPE type)
    {
        switch(type)
        {
            case ACTION_TYPE.SHOT:      // ショット攻撃
                ShotMode();
                break;
            case ACTION_TYPE.GUN:       // 銃攻撃
                GunMode();
                break;
            case ACTION_TYPE.GUARD:     // 防御
                GuardMode();
                break;
            case ACTION_TYPE.BOMB:      // ボム攻撃
                BombMode();
                break;
            case ACTION_TYPE.DASH:      // ダッシュ
                DashMode();
                break;
            default:
                // NOP
                break;
        }
    }



    /// <summary>
    /// プレイヤーの状態判定を行い、結果に応じてアニメーター変数の値を変更する。
    /// </summary>
    /// <param name="state">プレイヤー状態</param>
    private void SetAnimCtrl(PLAYER_STATE state)
    {
        switch(state)
        {
            case PLAYER_STATE.IDLE:         // 待機状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.IDLE);
                break;
            case PLAYER_STATE.MOVE:         // 通常移動状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.MOVE);
                break;
            case PLAYER_STATE.ACTION_STANDBY:  // アクション待機状態
                at_g_player_anmtr. SetInteger("state", (int)PLAYER_STATE.ACTION_STANDBY);
                break;
            case PLAYER_STATE.GET_HIT:      // 被ダメージ状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.GET_HIT);
                break;
            case PLAYER_STATE.SHOT:         // ショット攻撃状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.SHOT);
                break;
            case PLAYER_STATE.GUN:          // 銃攻撃状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.GUN);
                break;
            case PLAYER_STATE.GUARD:        // 防御状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.GUARD);
                break;
            case PLAYER_STATE.SET_BOMB:     // 爆弾設置状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.SET_BOMB);
                break;
            case PLAYER_STATE.DASH:         // ダッシュ状態
                at_g_player_anmtr.SetInteger("state", (int)PLAYER_STATE.DASH);
                break;
            case PLAYER_STATE.DEATH:
                // 撃破時のアニメーション変数設定処理はDamage関数内で行う。
                break;
            default:
                // NOP
                break;
        }
    }



    /// <summary>
    /// ジョイスティックでのプレイヤーオブジェクト移動を行う
    /// </summary>
    /// <detail>
    /// 移動、落下処理を行う。
    /// ・移動処理：プレイヤー状態に応じて"通常移動速度"または"ダッシュ速度"で移動を行う。
    /// ・落下処理：プレイヤーオブジェクトが接地していない時に落下を行う。
    /// </detail>
    /// <param name="state">プレイヤー状態</param>
    private void PlayerMove(PLAYER_STATE state){
        // 移動処理
        // IF:プレイヤー状態が"通常移動状態"or"待機状態"or"ダッシュ状態"か
        if(state == PLAYER_STATE.MOVE || state == PLAYER_STATE.IDLE || state == PLAYER_STATE.DASH){
            if(!GetJoyStickPoint())
            {
                float fl_l_moveSpeed;

                switch(state)
                {
                    case PLAYER_STATE.MOVE:                 // 通常移動状態のとき = 通常移動速度
                        fl_l_moveSpeed = FL_G_WALK_SPEED;
                        break;
                    case PLAYER_STATE.IDLE:                 // 待機状態のとき = 通常移動速度
                        fl_l_moveSpeed = FL_G_WALK_SPEED;
                        SetPlayerState(PLAYER_STATE.MOVE);  // 通常移動状態へ遷移する
                        break;
                    case PLAYER_STATE.DASH:                 // ダッシュ状態のとき = ダッシュ移動速度
                        fl_l_moveSpeed = FL_G_DASH_SPEED;
                        break;
                    default:                                // それ以外 = 0
                        fl_l_moveSpeed = 0;
                        break;
                }

                Vector3 vt3_l_move_velocity = new Vector3(inputJoyStick.Horizontal, 0, inputJoyStick.Vertical);
                cc_g_player_charaCon.Move(vt3_l_move_velocity.normalized * fl_l_moveSpeed * Time.deltaTime);
            }
            else
            {
                SetPlayerState(PLAYER_STATE.IDLE);
            }
        }
        
        // 落下処理
        // IF:プレイヤーオブジェクトが地面に接触(接地)しているか。
        if(cc_g_player_charaCon.isGrounded)
        { 
            vt3_g_fall_velocity = Vector3.zero; // 0リセット
        }
        // ELSE:"FL_G_GRAVITY"を使用した演算値で落下処理を行う。
        else
        {
            vt3_g_fall_velocity.y -= FL_G_GRAVITY * Time.deltaTime;             // 地面から離れている時、重力を加算
            cc_g_player_charaCon.Move(vt3_g_fall_velocity * Time.deltaTime);    // 落下処理を実行
        }
    }



    /// <summary>
    /// プレイヤーモデルオブジェクトの回転処理を行う。
    /// </summary>
    /// <param name="state">プレイヤーの状態</param>
    private void PlayerRotate(PLAYER_STATE state)
    {
        // IF:ジョイスティックがアクションポイント範囲外か
        if(!GetJoyStickPoint()) // 移動するまでにバッファを持たせる
        { 
            int s4_l_rotate_dir;

            // プレイヤー状態によって回転方向を反転する(アクション待機状態の時反転)
            switch(state)
            {
                case PLAYER_STATE.MOVE:             // 通常移動状態のとき
                    s4_l_rotate_dir = 1;
                    break;
                case PLAYER_STATE.DASH:             // 待機状態のとき
                    s4_l_rotate_dir = 1;
                    break;
                case PLAYER_STATE.ACTION_STANDBY:   // ダッシュ状態のとき
                    s4_l_rotate_dir = -1;
                    break;
                default:                            // それ以外
                    s4_l_rotate_dir = 0;
                    break;
            }
            
            // IF:ジョイスティックが中心か(中心の場合は早期リターン)
            if(new Vector3(inputJoyStick.Horizontal * s4_l_rotate_dir, 0, inputJoyStick.Vertical * s4_l_rotate_dir).magnitude == 0)
            {
                return;
            }
            else
            {
                // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
                Quaternion qt_l_lookRotation = Quaternion.LookRotation(new Vector3(inputJoyStick.Horizontal * s4_l_rotate_dir, 0,
                                                        inputJoyStick.Vertical * s4_l_rotate_dir), Vector3.up);
                tf_g_playerMdlObj_trfm.rotation = qt_l_lookRotation; // オブジェクトの回転に反映     
            }
        }
        else
        {
            // NOP
        }   
    }

    

    /// <summary>
    /// ショット攻撃アクション処理
    /// </summary>
    /// <param name="_shotPos">ショット攻撃開始時の座標</param>
    private void ShotMode()
    {
        GetJoyStickPoint_for_ActionWait();      // アクション待機状態時(アクション直前)のジョイスティック位置を取得
        // IF:アクション開始状態もしくはショット攻撃状態か
        if(player_state == PLAYER_STATE.ACTION_BEGIN || player_state == PLAYER_STATE.SHOT)
        {
            // IF:画面を離した時、ジョイスティックがアクションポイント範囲外であったの場合
            if(!fg_g_stickPoint_inWaitStt)
            {
                // IF:ショット攻撃の最大継続時間内か
                if(fl_g_shot_time_cnt < FL_G_SHOT_ATTACK_CONT_TIME)
                {
                    SetPlayerState(PLAYER_STATE.SHOT);                  // ショット攻撃状態に遷移
                    cl_g_shotAttackCollider.enabled = true;             // 当たり判定用コライダーを有効
                    cc_g_player_charaCon.Move(tf_g_playerMdlObj_trfm.forward * FL_G_SHOT_ATTACK_SPEED * Time.deltaTime);   // 進行方向へ直進(体当たり)
                    fl_g_shot_time_cnt += Time.deltaTime;                   // ショット攻撃の継続時間をカウント
                }
                // ELSE: ショット攻撃修了時、当たり判定用コライダーを無効化
                else
                {
                    cl_g_shotAttackCollider.enabled = false;            // 当たり判定用コライダーを無効
                }
            }
            // ELSE:画面を離した時、ジョイスティックがアクションポイント範囲内の場合待機状態に戻す(アクションキャンセル)
            else
            {
                SetPlayerState(PLAYER_STATE.IDLE);  // プレイヤー状態を"待機状態"へ遷移
            }
        }
        // ELSE:攻撃時以外の場合、時間を0リセット
        else
        {
            fl_g_shot_time_cnt = 0;     // 0リセット
        }
    }



    /// <summary>
    /// 銃攻撃アクション処理
    /// </summary>
    private void GunMode()
    {
        GetJoyStickPoint_for_ActionWait();      // アクション待機状態時(アクション直前)のジョイスティック位置を取得
        // IF:アクション開始状態もしくはショット攻撃状態か
        if(player_state == PLAYER_STATE.ACTION_BEGIN || player_state == PLAYER_STATE.GUN)
        {
            // IF:画面を離した時、ジョイスティックがアクションポイント範囲外であったの場合
            if(!fg_g_stickPoint_inWaitStt)
            {
                SetPlayerState(PLAYER_STATE.GUN);   // 銃攻撃状態に遷移
            }
            // ELSE:画面を離した時、ジョイスティックがアクションポイント範囲内の場合待機状態に戻す(アクションキャンセル)
            else
            {
                SetPlayerState(PLAYER_STATE.IDLE);
            }
        }
    }



    /// <summary>
    /// 防御アクション処理
    /// </summary>
    private void GuardMode()
    {
        // IF:アクション待機状態もしくは防御状態か
        if(player_state == PLAYER_STATE.ACTION_STANDBY || player_state == PLAYER_STATE.GUARD)
        {
            SetPlayerState(PLAYER_STATE.GUARD);     // 防御状態に遷移   
            // IF:画面を離した時、ジョイスティックがアクションポイント範囲外であったの場合
            if(!GetClickState())
            {
                SetPlayerState(PLAYER_STATE.IDLE);
            }
            else
            {
                // NOP
            }

            // IF:防御SEのワンショットフラグが有効か
            if(fg_g_guardSE_oneshot_flg)
            {
                as_g_audioSource_etc.PlayOneShot(ac_g_guard_se);  // 防御SEを再生

                // 防御SEのワンショットフラグを無効化（防御中にSEが繰り返し再生されないようワンショット化）
                fg_g_guardSE_oneshot_flg = false;
            }
            else
            {
                // NOP
            }
        
        }
        else
        {
            // 防御SEのワンショットフラグを有効化
            fg_g_guardSE_oneshot_flg = true;
        }
    }



    /// <summary>
    /// 爆弾設置アクション処理
    /// </summary>
    /// <detail>
    /// 爆弾を同時に設置できるのは1つまで
    /// </detail>
    private void BombMode()
    {
        GetJoyStickPoint_for_ActionWait();  // アクション待機状態時(アクション直前)のジョイスティック位置を取得
        // IF:アクション開始状態もしくは爆弾設置状態か
        if(player_state == PLAYER_STATE.ACTION_BEGIN || player_state == PLAYER_STATE.SET_BOMB)
        {
            // IF:爆弾が設置されていないか(フィールドに爆弾が存在しない時のみ設置可能)
            if(!go_g_bomb_clone)
            {
                // IF:画面を離した時、ジョイスティックがアクションポイント範囲外の場合アクションを実行
                if(!fg_g_stickPoint_inWaitStt)
                {
                    SetPlayerState(PLAYER_STATE.SET_BOMB);  // 爆弾設置中状態に遷移
                }
                // ELSE:画面を離した時、ジョイスティックがアクションポイント範囲内の場合待機状態に戻す(アクションキャンセル)
                else
                {
                    SetPlayerState(PLAYER_STATE.IDLE);
                }
            }
            // フィールドに既に爆弾が存在している場合
            else
            {
                SetPlayerState(PLAYER_STATE.IDLE);      // 爆弾設置処理を行わず"待機状態"へ遷移
            }
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// ダッシュアクション処理
    /// </summary>
    private void DashMode()
    {
        // IF:アクション待機状態もしくはダッシュ状態か
        if(player_state == PLAYER_STATE.ACTION_STANDBY || player_state == PLAYER_STATE.DASH)
        {
            SetPlayerState(PLAYER_STATE.DASH);  // ダッシュ状態へ遷移     
        }
        else
        {
            /* NOP */
        }
    }



    /// <summary>
    /// プレイヤーの状態をセットする
    /// </summary>
    /// <param name="state">
    /// セットする状態（列挙型：PLAYER_STATE）
    /// </param>
    /// <detail>
    /// 引数の状態にplayer_stateをセットする
    /// </detail>
    private void SetPlayerState(PLAYER_STATE state)
    {
        player_state = state;
    }



    /// <summary>
    /// 画面押下の有無を取得
    /// </summary>
    /// <returns>
    /// true：画面押下中
    /// false：画面非押下中
    /// </returns>
    private bool GetClickState()
    {
        // 待機状態、移動状態、アクション待機状態、防御状態でのみ画面タッチ判定を行う
        if(player_state == PLAYER_STATE.MOVE || player_state == PLAYER_STATE.IDLE || 
        player_state == PLAYER_STATE.ACTION_STANDBY || player_state == PLAYER_STATE.GUARD || player_state == PLAYER_STATE.DASH)
        {
            // IF:ジョイスティックが有効のとき、trueを返却
            if(inputJoyStick.transform.GetChild(0).gameObject.activeSelf)
            {
                fg_g_click_state_flg = true;
            }
            // ELSE:ジョイスティックが無効のとき、falseを返却
            else
            {
                fg_g_click_state_flg = false;
            }
        }
        else
        {
            /* NOP */
        }
        return fg_g_click_state_flg;
    }



    /// <summary>
    /// ジョイスティックがアクションポイント範囲内かチェック
    /// </summary>
    /// <details>
    /// アクションポイント：有効のジョイスティックが一定時間留まっていると"アクション待機状態"へ遷移するポイント(範囲)
    /// </details>
    /// <returns>
    /// true:ジョイスティックがアクションポイント範囲内
    /// false:ジョイスティックがアクションポイント範囲外
    /// </returns>
    private bool GetJoyStickPoint()
    {
        bool fg_l_in_actionPoint_flg = false;
        // ジョイスティックの位置を取得
        float fl_l_joystick_point = new Vector2(inputJoyStick.Horizontal, inputJoyStick.Vertical).magnitude;

        // IF:ジョイスティック中心から閾値分離れたときfg_l_in_actionPoint_flgをtrueにセットする
        if(fl_l_joystick_point <= FL_G_STICK_MAX_DIST)
        {     
            fg_l_in_actionPoint_flg = true;
        }
        else
        {
            // NOP
        }
        return fg_l_in_actionPoint_flg; 
    }



    /// <summary>
    /// アクション待機状態中のジョイスティック位置を取得
    /// </summary>
    /// <detail>
    /// アクション直前（アクション待機状態時に画面を離す瞬間）のジョイスティックポイントを取得し、アクション開始時の振る舞い分けに使用する。
    /// </detail>
    private void GetJoyStickPoint_for_ActionWait()
    {
        // IF:アクションタイプ状態か
        if(player_state == PLAYER_STATE.ACTION_STANDBY)
        {        
            // アクション待機状態時のみfg_g_stickPoint_inWaitSttを更新
            fg_g_stickPoint_inWaitStt = GetJoyStickPoint();
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// ジョイスティック操作による状態遷移（アクション状態、アクション開始状態）遷移を行う
    /// </summary>
    /// <detail>
    /// ジョイスティックがアクティブ(画面を押下中)かつアクションポイント範囲内である時間をカウントし、閾値に達したとき状態を"アクション待機状態"へ遷移する。
    /// 閾値到達前に一度でもアクションポイント範囲外に出た場合は状態遷移(アクション待機状態)を行わない。
    /// アクション待機状態時、アクションポイント範囲外で画面を離すと"アクション開始状態"へ遷移する
    /// </detail>
    private void SetActionState()
    {
        // IF:画面を押下している(ジョイスティックが有効)か
        if(GetClickState())
        {
            // IF:アクション待機状態遷移処理が許可されているか(禁止時は早期リターン)
            if(!fg_g_toStandby_perm_flg)
            {
                return;
            }
            else
            {
                // NOP
            }

            // IF:ジョイスティックがアクションポイント範囲内の時
            if(GetJoyStickPoint())
            {         
                fl_g_action_time_cnt += Time.deltaTime;     // ジョイスティック位置がアクションポイント範囲内に留まっている時間を計測する

                // IF:アクションポイント範囲内で"FL_G_TRANS_ACTION_STT_TIME"時間経過したか
                if(fl_g_action_time_cnt >= FL_G_TRANS_ACTION_STT_TIME)
                {
                    SetPlayerState(PLAYER_STATE.ACTION_STANDBY);   // 状態を"アクション待機状態"へ遷移する
                }
                else
                {
                    /* NOP */
                }
            }
            // ELSE:ジョイスティックがアクションポイント範囲外になった場合
            else
            {
                fl_g_action_time_cnt = 0;            // 0リセット
                fg_g_toStandby_perm_flg = false;     // アクション待機状態遷移処理許可フラグをfalse(禁止)にセット
            }
        }
        // ELSE:画面を押下していない場合(ジョイスティックが無効)
        else
        {
            fl_g_action_time_cnt = 0;           // 0リセット
            fg_g_toStandby_perm_flg = true;     // アクション待機状態遷移処理許可フラグをtrue(許可)にセット

            // IF:アクション待機状態か(アクション待機状態時に画面を離したとき、アクション実行)
            if(player_state == PLAYER_STATE.ACTION_STANDBY)
            {
                SetPlayerState(PLAYER_STATE.ACTION_BEGIN);  // プレイヤー状態を"アクション開始状態"に設定
            }
            else
            {
                // NOP
            }
        }      
    }



    /// <summary>
    /// 被ダメージ処理許可フラグ(fg_g_getDamage_perm_flg)の復帰処理
    /// </summary>
    /// <detail>
    /// 被ダメージ処理許可フラグがfalse(禁止)に設定されてからの時間を計測し、
    /// 復帰時間が経過したらフラグをtrue(許可)へ復帰する処理である。
    /// </detail>
    private void GetDamagePermFlgReset()
    {
        // IF:被ダメージ処理許可フラグがture(許可)の場合は早期リターン
        if(fg_g_getDamage_perm_flg)
        {
            return;
        }
        else
        {
            // NOP
        }
        
        // 被ダメージ処理許可フラグがfalse(禁止)に設定されてからの時間を測定
        fl_g_getDamagePermflg_rtn_count += Time.deltaTime;

        // IF:被ダメージ処理許可フラグの復帰時間が経過したか
        if(fl_g_getDamagePermflg_rtn_count >= dt_g_playerStatus_data.GET_DAMAGE_INTERVAL_TIME)
        {
            fg_g_getDamage_perm_flg = true;         // 被ダメージ処理許可フラグをtrue(許可)に設定 
            fl_g_getDamagePermflg_rtn_count = 0;    // 0リセット
        }
        else
        {
            // NOP
        } 
    }



/*------------- 外部呼出し関数 -----------------*/
    /// <summary>
    /// ショット攻撃の当たり判定用コライダーにアタッチしたスクリプト(ShotAttackDamage_src)で呼び出す関数。
    /// ショット攻撃による敵への与ダメージ処理を行う
    /// </summary>
    /// <detail>
    /// プレイヤー状態が"ショット状態"かを判定し、
    /// trueの場合、接触した敵オブジェクトの"Damage"関数を呼び出してダメージを与える。
    /// </detail>
    /// <param name="go_l_enemyObj">
    /// 攻撃の当たり判定用コライダーが接触したGameObject
    /// </param>
    public void ShotAttackDamage(GameObject go_l_enemyObj)
    {
        // プレイヤー状態が"ショット攻撃状態"か
        if(player_state == PLAYER_STATE.SHOT)
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
            damageHit.Damage((int)Random.Range(dt_g_playerStatus_data.SHOT_ATTACK_DAMAGE_VALUE - dt_g_playerStatus_data.DAMAGE_RANDOM_RANGE,
                                                dt_g_playerStatus_data.SHOT_ATTACK_DAMAGE_VALUE));
        }
        else
        {
            /* NOP */
        }
    }



    /// <summary>
    /// "IDamageable"継承関数
    /// 被ダメージ処理を行う(拡張予定)
    /// </summary>
    /// <detail>
    /// 以下、追加される可能性のある処理内容
    /// ・プレイヤー状態に応じたダメージ量増減
    /// ・プレイヤー状態に応じたノックバックの有無
    /// ・プレイヤー状態に応じたSEの再生
    /// など
    /// </detail>
    /// <param name="s4_l_damagePoint">
    /// 被ダメージ値
    /// </param>
    public void Damage(int s4_l_damagePoint)
    {   
        // IF:プレイヤー状態が"防御状態"か
        if(player_state == PLAYER_STATE.GUARD)
        {
            Debug.Log("ダメージ無効");
            // 防御時SEを再生し、HP減算処理は行わない。
            as_g_audioSource_etc.PlayOneShot(ac_g_guard_se);
            return;
        }
        else
        {
            // NOP
        }

        // IF:被ダメージ処理が許可されているか(連続的に被ダメージ処理を行うことを防ぐ)
        if(fg_g_getDamage_perm_flg)
        {
            // プレイヤー状態に応じたダメージ処理を行う
            switch(player_state)
            {
                case PLAYER_STATE.IDLE:             // 待機状態
                    Debug.Log("被ダメージ");
                    break;
                case PLAYER_STATE.MOVE:             // 通常移動状態
                    Debug.Log("被ダメージ");
                    break;
                case PLAYER_STATE.ACTION_STANDBY:   // アクション待機状態
                    Debug.Log("被ダメージ");
                    break;
                case PLAYER_STATE.GUN:              // 銃攻撃状態
                    Debug.Log("被ダメージ");
                    break;
                case PLAYER_STATE.SET_BOMB:         // 爆弾設置状態
                    Debug.Log("被ダメージ");
                    break;
                case PLAYER_STATE.DASH:             // ダッシュ状態
                    Debug.Log("被ダメージ");
                    break;
                default:
                    // NOP
                    break;
            }
            // 被ダメージSEを再生
            as_g_audioSource_etc.PlayOneShot(ac_g_getDamage_se);

            // 被ダメージ許可フラグを禁止に設定
            fg_g_getDamage_perm_flg = false;

            // HPから被ダメージ値を減算
            s4_g_playerCurrent_HP -= s4_l_damagePoint;
            
            // HPゲージ(ハートアイコン)の減少処理関数を呼び出し
            sc_g_PlayerHeartManager_src.GetDamage(s4_g_playerCurrent_HP);
            // 現在のHPを0から最大HPの範囲に収める
            s4_g_playerCurrent_HP = Mathf.Clamp(s4_g_playerCurrent_HP,0,dt_g_playerStatus_data.MAX_HP);
            // IF:ヒットポイント0か
            if(s4_g_playerCurrent_HP == 0)
            {
                // 撃破時の処理関数を呼び出し
                Death();
            }
            else
            {
                // 被ダメージアニメーションを再生
                at_g_playerMat_anmtr.Play("PlayerMat_GetDamage", 0, 0);
            }
        }
        else
        {
            // NOP
        }
    }



    /// <summary>
    /// 回復処理を行う
    /// </summary>
    /// <param name="u1_l_healPoint"></param>
    public void Heal(uint u1_l_healPoint)
    {
        // HPから被ダメージ値を減算
        s4_g_playerCurrent_HP += (int)u1_l_healPoint;
        // 現在のHPを0から最大HPの範囲に収める
        s4_g_playerCurrent_HP = Mathf.Clamp(s4_g_playerCurrent_HP,0,dt_g_playerStatus_data.MAX_HP);
        // HPゲージ(ハートアイコン)の減少処理関数を呼び出し
        sc_g_PlayerHeartManager_src.Heal(s4_g_playerCurrent_HP);
    }



    /// <summary>
    /// プレイヤーが撃破された際の処理
    /// </summary>
    /// <detail>
    /// ・状態遷移
    /// ・アニメーションの再生
    /// ・メニューの表示など(設計中)
    /// </detail>
    private void Death()
    {
        // プレイヤー状態を"撃破状態"に設定
        SetPlayerState(PLAYER_STATE.DEATH);
        // 撃破時エフェクト生成
        Instantiate(go_g_death_effect, this.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
        // 撃破時アニメーションを再生
        at_g_player_anmtr.Play("Death", 0, 0);

        Invoke("GameOver",3);
    }
    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    /// <summary>
    /// アクションタイプの切り替え処理
    /// </summary>
    /// <detail>
    /// "RadialMenu_src"にて生成されるUIボタンによって呼び出される関数である。
    /// 各ボタン押下によるショット攻撃～ダッシュのアクションタイプ変更を行う。
    /// アクションタイプの変更はプレイヤー状態が"待機状態"のときのみ行う(バグ回避のため)。
    /// </detail>
    /// <param name="u4_l_mode">変更先のアクションタイプ種類</param>
    public void Button_SetPlayerMode(uint u4_l_type)
    {
        // アクションタイプを変更
        action_type = (ACTION_TYPE)u4_l_type;
        // プレイヤーモデルのテクスチャを、アクションタイプに応じて変更。
        sm_playerModel_smr.material.SetTexture("_AlbedoMap", att_g_actionType_Texture[(int)action_type]);
    }


    /// <summary>
    /// 強制移動イベント開始処理
    /// </summary>
    /// <detail>
    /// プレイヤー操作の許可判定フラグをFalse(操作禁止)に設定する。
    /// 操作許可判定はUpdate関数内で行っている。
    /// </detail>
    public void StartEventMoveCtrl()
    {
        fg_g_canCtrl_flg = false;
    }

    /// <summary>
    /// 強制移動イベント開始処理
    /// </summary>
    /// <detail>
    /// プレイヤー操作の許可判定フラグをTrue(操作許可)に設定する。
    /// 操作許可判定はUpdate関数内で行っている。
    /// </detail>
    public void EndEventMoveCtrl()
    {
        fg_g_canCtrl_flg = true;
    }

    /// <summary>
    /// 強制移動イベント用処理
    /// </summary>
    /// <detail>
    /// 他スクリプトにて座標(移動目標)を指定して当コルーチンを開始する。
    /// 下記順の処理内容となっている。
    /// 1.プレイヤーの操作を禁止
    /// 2.指定された座標(移動目標)まで強制的に移動
    /// 3.プレイヤーの操作を許可
    /// </detail>
    /// <param name="tf_l_target_trfm">移動先Transform</param>
    public IEnumerator EventMoveCtrl(Transform tf_l_target_trfm)
    {
        Vector3 direction = (tf_l_target_trfm.position - this.transform.position).normalized;   // 移動先の方向を計算
        Quaternion targetRotation = Quaternion.LookRotation(direction);                         // 移動先の方向を向くための回転を計算
        
        // プレイヤー状態を"待機状態"へ遷移(攻撃中等時のバグ防止)
        SetPlayerState(PLAYER_STATE.IDLE);
        
        // プレイヤーの操作を禁止
        StartEventMoveCtrl();

        // プレイヤーを移動方向を正面となるよう回転する
        while (Quaternion.Angle(go_g_playerMdlObj.transform.rotation, targetRotation) > 0.1f)
        {
            go_g_playerMdlObj.transform.rotation = Quaternion.Slerp(go_g_playerMdlObj.transform.rotation, targetRotation, 20.0f * Time.deltaTime);
            yield return null;
        }

        // プレイヤーを移動目標まで移動する
        while (Vector3.Distance(this.transform.position, tf_l_target_trfm.position) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, tf_l_target_trfm.position, 5.0f * Time.deltaTime);
            yield return null;
        }

        // プレイヤー操作を許可
        EndEventMoveCtrl();
    }
    

    /// <summary>
    /// 最終安全位置のセット処理
    /// </summary>
    /// <detail>
    /// SetLastSafePosition(プレイヤー位置の最終安全地帯への復帰処理）で使用する「最終安全地帯」を取得する
    /// </detail>
    /// <param name="go_l_object">接触したオブジェクト</param>
    public void GetLastSafePosition()
    {
        // Raycastによる接地判定
        RaycastHit hit;
        Debug.DrawRay(go_g_playerMdlObj.transform.position, Vector3.down * 0.3f, Color.red);
        if(Physics.Raycast(go_g_playerMdlObj.transform.position, Vector3.down, out hit, 0.3f))
        {
            // IF：タグが"Ground"か
            if(hit.collider.CompareTag("Ground"))
            {
                // 最終安全位置を触れているオブジェクトに設定
                lastSafePosition = hit.transform.position;
                // Y軸位置調整(地面に埋まらないように)
                lastSafePosition.y += 0.8f;
                Debug.Log("安全地点記憶");
            }
            else
            {
                // NOP
                Debug.Log("安全地点非記憶");
            }
        }
        else
        {
            // NOP
            Debug.Log("安全地点非記憶");
        }
    }



    /// <summary>
    /// 奈落へ落下時にプレイヤーの位置を最終安全地帯へ復帰する。
    /// </summary>
    /// <detail>
    /// FallAbyss_src(落下判定)スクリプトのOnTriggerEnter関数で当コルーチンを開始する。
    /// </detail>
    /// <returns></returns>
    public IEnumerator SetLastSafePosition()
    {
        // プレイヤー状態を"待機状態"に遷移
        player_state = PLAYER_STATE.IDLE;
        // プレイヤーの操作を禁止
        fg_g_canCtrl_flg = false;
        // プレイヤーのポジションを最終安全位置に移動
        this.gameObject.transform.position = lastSafePosition;
        yield return new WaitForSeconds(0.5f);  // 0.5秒待機
        // プレイヤーの操作を許可
        fg_g_canCtrl_flg = true;
        yield break;
    }



    /// <summary>
    /// アクションタイプの解放処理
    /// </summary>
    /// <param name="unlock_action"></param>
    public void UnlockAbility(int unlock_action)
    {
        if(ActionTypeStatus.ContainsKey(unlock_action) && !ActionTypeStatus[unlock_action])
        {
            ActionTypeStatus[unlock_action] = true;
        }
        else
        {
            // NOP
        }
        sc_g_SelectTypeMenu_src.SetupRadialButtons(ActionTypeStatus);
    }

/*------------- アニメーションイベント関数(アニメーションの任意のタイミングで呼び出す) -----------------*/
    /// <summary>
    /// 移動アニメーションイベント(足音)
    /// </summary>
    public void AnimEventStateFootStepSound()
    {
        as_g_audioSource_FootStep.PlayOneShot(aac_g_footstepSound[Random.Range(0, aac_g_footstepSound.Length)]);
    }

    /// <summary>
    /// 銃攻撃アニメーションイベント（弾発射、エフェクト)
    /// </summary>
    public void AnimEventGunEffect()
    {
        GameObject go_g_gunBullet_clone = Instantiate(go_g_gunBullet, this.transform.position + tf_g_playerMdlObj_trfm.forward, tf_g_playerMdlObj_trfm.rotation); //弾生成
        Instantiate(go_g_gun_effect, this.transform.position + tf_g_playerMdlObj_trfm.forward * 1f, tf_g_playerMdlObj_trfm.rotation, this.gameObject.transform); //エフェクト
    }

    /// <summary>
    /// 銃攻撃アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventGunSE()
    {
        as_g_audioSource_etc.PlayOneShot(ac_g_gunAttack_se);
    }

    /// <summary>
    /// ショット攻撃アニメーションイベント（エフェクト）
    /// </summary>
    public void AnimEventShotEffect()
    {
        Instantiate(go_g_shot_effect, this.transform.position, Quaternion.Euler(-90, 0, 0), this.gameObject.transform);
    }

    /// <summary>
    /// ショット攻撃アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventShotSE()
    {
        as_g_audioSource_etc.PlayOneShot(ac_g_shotAttack_se);
    }

    /// <summary>
    /// 爆弾設置アニメーションイベント（爆弾設置）
    /// </summary>
    public void AnimEventSetBomb()
    {
        //オブジェクト生成（爆弾）
        go_g_bomb_clone = Instantiate(go_g_bomb, this.transform.position + tf_g_playerMdlObj_trfm.forward + new Vector3 (0, 0.5f, 0), go_g_bomb.transform.rotation);
    }

    /// <summary>
    /// 爆弾設置アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventBombSetSE()
    {
        as_g_audioSource_etc.PlayOneShot(ac_g_bombSet_se);
    }

    /// <summary>
    /// アニメーションイベント（待機状態へ遷移）
    /// </summary>
    public void AnimEventStateSetIdle()
    {
        SetPlayerState(PLAYER_STATE.IDLE);
    }
}