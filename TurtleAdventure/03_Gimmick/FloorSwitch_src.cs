using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSwitch_src : MonoBehaviour
{
	// //　ボックスの初期位置
	// private Vector3 defaultPosition;
	// //　ボックスの移動速度
	// private Vector3 velocity;
	// //　飛ばすレイの距離
	// [SerializeField]
	// private float rayDistance = 1f;
	// //　自身のコライダ
	// private Collider myCollider;
	// //　接触を無視するコライダ
	// [SerializeField]
	// private Collider floorCollider;
	// //　戻るスピード
	// [SerializeField]
	// private float returnSpeed = 0.1f;
	// //　沈むスピード
	// [SerializeField]
	// private float sinkingSpeed = 2f;
	// //　キャラクターがブロックに乗っているかどうか
	// private bool characterIsOnBoard;
	// [SerializeField]
	// private Vector3 blockSize = Vector3.one;
 
	// // Start is called before the first frame update
	// void Start() {
	// 	// rigidBody = GetComponent<Rigidbody>();
	// 	// defaultPosition = rigidBody.position;
	// 	myCollider = GetComponent<Collider>();
	// 	//　自身のコライダとフロアのコライダの接触をしないようにする
	// 	Physics.IgnoreCollision(myCollider, floorCollider, true);
	// }
 
	// // Update is called once per frame
	// void Update() {
	// 	//　BoxとPlayerレイヤーを持つコライダと接触するか確認し、プレイヤーが乗っていれば下向き、
	// 	if (!characterIsOnBoard) {
	// 		if (Physics.CheckBox(transform.position + Vector3.up * rayDistance, blockSize * 0.52f, Quaternion.identity, LayerMask.GetMask("Player"))) {
	// 		// 	velocity = Vector3.down * sinkingSpeed;
	// 		// 	characterIsOnBoard = true;
    //             Debug.Log("active");
    //         }
    //         //  else {
	// 		// 	velocity = Vector3.up * returnSpeed;
	// 		// }
	// 		//　キャラクターが乗っている時
	// 	}
    //     else {
	// 		// キャラクターが乗っているとされている時に、ボックスとキャラクターが接触しているか確認し、していなければ乗っていないに変更
	// 		if (!Physics.CheckBox(transform.position + Vector3.up * rayDistance, blockSize * 0.52f, Quaternion.identity, LayerMask.GetMask("Player"))) {
	// 			Debug.Log("false");
	// 		}
	// 	}
	// }
}