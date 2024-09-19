using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickSwitch_src : MonoBehaviour
{
/*------------- 概要 -------------------
プレイヤーの当たり判定によるスイッチのON/OFF切り替え及び
扉アニメーション変数操作による扉開閉処理を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] bool fg_g_switchState_flg;       // スイッチの状態(ON/OFF)
    [SerializeField] bool fg_g_isTimedSwitch;         // 時限スイッチ設定値(T:時限スイッチ、F:通常スイッチ)
    [SerializeField] SwitchDoor_src[] sc_g_subjectDoor;

/*--------------- 定数 ----------------*/
    [SerializeField] float FL_G_TIMED_SWITCH_LIMIT_TIME = 5f;   // スイッチがONになってからOFFに遷移するまでの制限時間

/*------------- 代入用変数----------------*/
    private Animator at_g_anmtr;            // "Animator"コンポーネント取得用
    public float fl_g_timeCounter;          // 制限時間カウンター用



    // スイッチタイプ
    public enum SWITCH_TYPE
    {
        LEVER,  // レバー型スイッチ
        FLOOR   // 床型スイッチ
    }
    [SerializeField] SWITCH_TYPE switch_type;



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    void Awake()
    {
        at_g_anmtr = this.gameObject.GetComponent<Animator>();      // "Animator"コンポーネントを取得
        at_g_anmtr.SetBool("switch_state", fg_g_switchState_flg);   // アニメーション変数を初期化
        fl_g_timeCounter = 0;                                       // カウンターを0リセット
        fg_g_switchState_flg = false;                               // スイッチ状態を"OFF"で初期化
        SetDoorAnimVar();                                           // 扉オブジェクトの状態変数をスイッチ状態に設定
    }


    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    void Update()
    {
        // IF:時限スイッチ設定か    
        if(fg_g_isTimedSwitch)
        {
            TimedSwitch();
        }
        else
        {
            // NOP
        }
    }


    /// <summary>
    /// 時限スイッチ処理
    /// </summary>
    /// <detail>
    /// 時限スイッチ設定時に呼び出される。
    /// スイッチ状態が"ON"になっている時間をカウントし、
    /// 制限時間経過するとスイッチ状態を"OFF"に設定する。
    /// </detail>
    private void TimedSwitch()
    {
        // スイッチの状態がTrue(ON)でない場合早期リターン
        if(!fg_g_switchState_flg)
        { 
            return;
        }
        else
        {
            // NOP
        }

        // 時間をカウント
        fl_g_timeCounter += Time.deltaTime;

        // IF:カウンターが制限時間以上経過したか
        if(fl_g_timeCounter >= FL_G_TIMED_SWITCH_LIMIT_TIME)
        {
            // スイッチの状態をFalse(OFF)に設定
            fg_g_switchState_flg = false;
            // アニメーション変数をスイッチの状態に設定
            at_g_anmtr.SetBool("switch_state", fg_g_switchState_flg);
            // 扉オブジェクトの状態変数をスイッチ状態に設定
            SetDoorAnimVar();
            // カウンターを0リセット
            fl_g_timeCounter = 0;
        }
        else
        {
            // NOP
        }
    }


    /// <summary>
    /// 他コライダーに接触した際に呼び出される
    /// スイッチのON/OFF処理
    /// </summary>
    /// <detail>
    /// スイッチタイプ(SWITCH_TYPE)に応じた処理を行う
    /// レバータイプ：プレイヤーの「攻撃用」当たり判定用コライダーが接触した際にON/OFFを切り替える。
    /// 床型スイッチタイプ：プレイヤーの当たり判定用コライダーが接触した際にONに切り替える。(時限式の場合のみOFF切り替え可)
    /// </detail>
    /// <param name="cl_l_hitCol"></param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        switch(switch_type)
        {
            case SWITCH_TYPE.LEVER:
                // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
                if (!go_l_hitObj.CompareTag("PlayerAttackCollider"))
                {
                    return;
                }
                else
                {
                    // NOP
                }

                // カウンターを0リセット
                fl_g_timeCounter = 0;

                // スイッチの状態(ON/OFF)を切り替える
                fg_g_switchState_flg = !fg_g_switchState_flg;
                // アニメーション変数をスイッチの状態に設定
                at_g_anmtr.SetBool("switch_state", fg_g_switchState_flg);

                // 扉オブジェクトの状態変数をスイッチ状態に設定
                SetDoorAnimVar();
                break;

            case SWITCH_TYPE.FLOOR:
                // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
                if (!go_l_hitObj.CompareTag("PlayerHitCollider"))
                {
                    return;
                }
                else
                {
                    // NOP
                }

                // スイッチの状態をONに設定
                fg_g_switchState_flg = true;

                // アニメーション変数をスイッチの状態に設定
                at_g_anmtr.SetBool("switch_state", fg_g_switchState_flg);

                // Doorオブジェクトのアニメーション変数をスイッチ状態に設定
                SetDoorAnimVar();
                break;
        }
        
    }



    /// <summary>
    /// 扉オブジェクトのアニメーション変数を設定する。
    /// DoorStateTransスクリプトの"DoorStateTrans"関数を呼び出す。
    /// </summary>
    private void SetDoorAnimVar()
    {
        foreach(SwitchDoor_src sc_l_subjectDoor in sc_g_subjectDoor)
        {
            // 扉オブジェクトにアタッチしたアニメーション変数のセッター関数を呼び出す。
            sc_l_subjectDoor.DoorStateTrans(fg_g_switchState_flg); 
        }
    }
}
