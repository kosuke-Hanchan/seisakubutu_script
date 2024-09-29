using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class TargetDestFlgMng_src : MonoBehaviour
{
/*------------- 概要 -------------------
特定の敵を指定の数撃破した際のフィードバック処理を行う。

フィードバック処理
・アイテムの表示/非表示（鍵の出現等に使用）
・扉の開閉（敵を倒さなくては通り抜けることができない部屋等に使用）
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject[] ago_g_feedbackObj;            // ターゲット撃破時のフィードバック対象となるオブジェクトを設定する。
    [SerializeField] bool fg_g_booleanType_Select;              // タイプをインスペクターから選択する（扉の開/閉、アイテムの表示/非表示を選択）
    [SerializeField] uint u1_g_targetEnemies;                   // ターゲットとする敵の数を設定する。
    [SerializeField] GameObject go_g_setItem_eff;               // アイテム出現時のエフェクト
    [SerializeField] GameObject go_g_doorOpCl_eff;              // 扉開閉時のエフェクト
    [SerializeField] CinemachineVirtualCamera cm_g_chinemachine_Vcam;    // CinemachineVirtualCamera取得用

    [SerializeField] bool is_changeBGM_flg;                     // 条件達成時にBGMを変更するかの設定フラグ(T：変更する、F：変更しない)
    [SerializeField] AudioClip ac_g_BGM;                        // 変更するBGM


    [System.Serializable]
    public class MyFeedbackObject
    {
        public GameObject feedbackObj;          // フィードバック対象オブジェクト
        public FEEDBACK_TYPE feedbackType;      // フィードバックタイプ（アイテム表示/非表示、扉開閉など）
    }
    public List<MyFeedbackObject> feedbackObjects = new List<MyFeedbackObject>();   // 複数のオブジェクトをリストとして管理

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Transform tf_g_playerObj_trfm;      // プレイヤーオブジェクトのTransformコンポ取得用
    private uint u1_g_targetEnemies_count;      // ターゲットの撃破数カウント用
    private Animator at_g_animator;             // "Animator"コンポーネント取得用
    private AudioSource as_g_audioSource_bgm;   // BGM用AudioSource格納用

/*------------- 列挙体 ---------------*/
    public enum FEEDBACK_TYPE
    {
        SET_ITEM,
        OPEN_DOOR    
    }        



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
        // BGM用AudioSourceを取得
        as_g_audioSource_bgm = GameObject.FindGameObjectWithTag("AudioSource_bgm").GetComponent<AudioSource>();
        // プレイヤーオブジェクトのTransformコンポ取得用
        tf_g_playerObj_trfm = GameObject.FindGameObjectWithTag("Player").transform;
    }


    /// <summary>
    /// ターゲット対象となる撃破可能オブジェクトにて呼び出す。
    /// ターゲットの撃破数を更新し、閾値を超えた際にフィードバック処理を呼び出す。
    /// </summary>
    public void OnEnemyDefeated()
    {
        u1_g_targetEnemies_count++;
        if(u1_g_targetEnemies_count >= u1_g_targetEnemies)
        {
            StartCoroutine(TargetDestroyReaction());
        }
        else
        {
            // NOP
        }
    }


    /// <summary>
    /// フィードバック処理用コルーチン
    /// </summary>
    /// <detail>
    /// ・フィードバック処理関数を呼び出す
    /// ・フィードバック対象オブジェクトへ一時的に変更する（強調する）
    /// </detail>
    private IEnumerator TargetDestroyReaction()
    {
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

        yield return new WaitForSeconds(1f);  // 待機

        // フィードバック処理
        foreach(MyFeedbackObject feedbackObject in feedbackObjects)
        {
            // フィードバックタイプに基づく処理
            FeedBackTypejudg(feedbackObject);

            // カメラの追従対象を一時的に変更（変化を強調するため）
            cm_g_chinemachine_Vcam.Follow = feedbackObject.feedbackObj.transform;
            yield return new WaitForSeconds(1f);  // 待機
        }

        // カメラの追従対象をプレイヤーへ戻す
        cm_g_chinemachine_Vcam.Follow = tf_g_playerObj_trfm;
        yield break;
    }



    /// <summary>
    /// フィードバックタイプの判定を行い、結果に応じて各タイプ処理関数を呼び出す。
    /// </summary>
    private void FeedBackTypejudg(MyFeedbackObject feedbackObject)
    {
        switch(feedbackObject.feedbackType)
        {
            // アイテム表示/非表示タイプ
            case FEEDBACK_TYPE.SET_ITEM:
                TypeSetItem(feedbackObject.feedbackObj);
                break;
            
            // 扉開閉タイプ
            case FEEDBACK_TYPE.OPEN_DOOR:
                TypeDoorOpen(feedbackObject.feedbackObj);
                break;
            
            // それ以外
            default:
                // NOP
                break;
        }
    }


    /// <summary>
    /// フィードバック(アイテム表示/非表示タイプ)処理
    /// </summary>
    /// <detail>
    /// ターゲットオブジェクトとして設定したアイテムの表示設定を変更する。
    /// </detail>
    private void TypeSetItem(GameObject go_l_feedbackObj)
    {
        // アイテムの表示/非表示を行う
        go_l_feedbackObj.SetActive(fg_g_booleanType_Select);

        if(go_g_setItem_eff != null)
        {
            // エフェクト生成
            Instantiate(go_g_setItem_eff, go_l_feedbackObj.transform.position, Quaternion.identity);
        }
        else
        {
            // NOP
        }
    }


    /// <summary>
    /// フィードバック(扉開閉タイプ)処理
    /// </summary>
    /// <detail>
    /// ターゲットオブジェクトとして設定した扉のアニメーション変数を設定する。
    /// </detail>
    private void TypeDoorOpen(GameObject go_l_feedbackObj)
    {
        // 扉の開閉アニメーション変数を設定する。
        at_g_animator = go_l_feedbackObj.transform.GetComponent<Animator>();
        at_g_animator.SetBool("State", fg_g_booleanType_Select);
        if(go_g_doorOpCl_eff != null)
        {
            // エフェクト生成
            Instantiate(go_g_doorOpCl_eff, go_l_feedbackObj.transform.position, Quaternion.identity);
        }
        else
        {
            // NOP
        }
    }        
}