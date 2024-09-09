using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAttackBullet_src : MonoBehaviour
{
/*------------- 概要 -------------------
プレイヤーの銃攻撃アクション時に発射する弾丸用スクリプトである。
弾丸オブジェクトにアタッチする。

下記処理を行う。
・移動処理
・与ダメージ処理
・オブジェクト削除処理

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] PlayerStatus_data dt_g_playerStatus_data;  // プレイヤーステータス管理用スクリプタブルオブジェクト


/*--------------- 定数 ----------------*/
    private const float FL_G_MOVE_SPEED = 15.0f;                // 銃弾の移動速度
    private const float FL_G_MAX_FLIGHT_DISTANCE = 10.0f;       // 銃弾の最大飛距離

/*------------- 代入用変数----------------*/
    private Vector3 vt3_g_bullet_prePos;    // 弾丸オブジェクト生成時のPosition格納用
    private Vector3 vt3_g_bullet_pos;       // 弾丸オブジェクトの現在Position格納用



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        // 生成時の位置を記憶
        vt3_g_bullet_prePos = this.transform.position;
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        // 現在位置を記憶
        vt3_g_bullet_pos = this.transform.position;
        
        // 移動処理
        this.transform.Translate(Vector3.forward * Time.deltaTime * FL_G_MOVE_SPEED);

        // IF: 砲弾が最大移動距離移動したか
        if((vt3_g_bullet_pos - vt3_g_bullet_prePos).magnitude >= FL_G_MAX_FLIGHT_DISTANCE){
            Destroy(this.gameObject);   // 弾丸オブジェクトを削除
        }
    }



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// </summary>
    /// <detail>
    /// 接触したコライダーが敵オブジェクトの場合、"Damage"関数を呼び出してダメージを与える。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        // 毎回GetComponentするのは動作が重いので、敵かどうかをまず調べる(早期リターン)
        if (!go_l_hitObj.CompareTag("Enemy"))
        {
            return;
        }

        // ヒットしたオブジェクトの"IDamageable"コンポーネントを取得
        IDamageable damageHit = go_l_hitObj.GetComponent<IDamageable>(); 

        // ダメージ判定が実装されていなければ、ダメージ処理を行わない(早期リターン)
        if (damageHit == null)
        {
            return;
        }

        // ヒットしたオブジェクトの"Damage"関数を呼び出す。(引数：与ダメージ値-乱数用範囲～与ダメージ値の乱数)
        damageHit.Damage((int)Random.Range(dt_g_playerStatus_data.GUN_ATTACK_DAMAGE_VALUE - dt_g_playerStatus_data.DAMAGE_RANDOM_RANGE,
                                            dt_g_playerStatus_data.GUN_ATTACK_DAMAGE_VALUE));
    }
}
