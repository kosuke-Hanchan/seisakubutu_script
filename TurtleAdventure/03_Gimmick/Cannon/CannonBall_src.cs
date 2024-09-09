using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall_src : MonoBehaviour
{
/*------------- 概要 -------------------
大砲ギミックが射出する砲弾用スクリプトである。
砲弾オブジェクトにアタッチする。

下記処理を行う。
・移動処理
・与ダメージ処理
・オブジェクト削除処理

*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GimmickStatus_data dt_g_gimmickStatus_data;  // Enemyステータス管理用スクリプタブルオブジェクト
    [SerializeField] GameObject go_g_explosion_effect;          // 爆発エフェクト

/*--------------- 定数 ----------------*/
    private const float FL_G_MOVE_SPEED = 15.0f;            // 砲弾の速度
    private const float FL_G_MAX_FLIGHT_DISTANCE = 10.0f;   // 最大飛距離

/*------------- 代入用変数----------------*/
    private Vector3 vt3_g_cannonBall_prePos;                // 砲弾オブジェクト生成時のPosition格納用
    private Vector3 vt3_g_cannonBall_pos;                   // 砲弾オブジェクトの現在Position格納用



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        // 砲弾生成時の位置を記憶
        vt3_g_cannonBall_prePos = this.transform.position;
    }



    /// <summary>
    /// MonoBehaviour有効時に毎フレーム呼び出される
    /// </summary>
    private void Update()
    {
        // 現在位置を記憶
        vt3_g_cannonBall_pos = this.transform.position;
        
        //移動処理
        this.transform.Translate(Vector3.forward * Time.deltaTime * FL_G_MOVE_SPEED);
        
        // IF: 砲弾が最大移動距離移動したか
        if((vt3_g_cannonBall_pos - vt3_g_cannonBall_prePos).magnitude  >= FL_G_MAX_FLIGHT_DISTANCE)
        {
            // 爆発エフェクトを生成
            Instantiate(go_g_explosion_effect, this.transform.position, Quaternion.identity);
            
            // 砲弾オブジェクトを削除
            Destroy(this.gameObject);
        }
    }



    /// <summary>
    /// 砲弾のコライダーが他コライダーに接触した際に呼び出される処理。
    /// 接触したコライダーが敵オブジェクトの場合"Damage"関数を呼び出してダメージを与える。
    /// </summary>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!go_l_hitObj.CompareTag("PlayerHitCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }

        // ヒットしたオブジェクトのIDamageableを取得する
        IDamageable damageHit = go_l_hitObj.transform.parent.GetComponent<IDamageable>(); 

        // ダメージ判定が実装されていなければ、ダメージ判例を行わない(早期リターン)
        if (damageHit == null)
        { 
            return;
        }
        else
        {
            // NOP
        }

        // 与ダメージ
        damageHit.Damage(dt_g_gimmickStatus_data.DAMAGE_VALUE);

        // 爆発エフェクトを生成
        Instantiate(go_g_explosion_effect, this.transform.position, Quaternion.identity);

        // 砲弾オブジェクトを削除
        Destroy(this.gameObject);
    }
}
