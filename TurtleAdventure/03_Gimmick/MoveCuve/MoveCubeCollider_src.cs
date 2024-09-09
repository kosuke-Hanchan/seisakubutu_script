using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCubeCollider_src : MonoBehaviour
{
    
    private enum DIRECTION{
        RIGHT,
        LEFT,
        FORWARD,
        BACK,
    }

    [SerializeField] private DIRECTION direction;
    private Player_ctrl_src script;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider Hit){
        //毎回GetComponentするのは動作が重いので、プレイヤーかどうかをまず調べる(早期リターン)
        if (!Hit.CompareTag("PlayerAttackCollider")){
            return;
        }
        script = Hit.transform.parent.transform.GetComponent<Player_ctrl_src>();

        if(script.player_state == Player_ctrl_src.PLAYER_STATE.SHOT){
            switch(direction){
                case DIRECTION.RIGHT:
                    Debug.Log(DIRECTION.RIGHT);
                    break;
                case DIRECTION.LEFT:
                    Debug.Log(DIRECTION.LEFT);
                    break;
                case DIRECTION.FORWARD:
                    Debug.Log(DIRECTION.FORWARD);
                    break;
                case DIRECTION.BACK:
                    Debug.Log(DIRECTION.BACK);
                    break;
            }
            

        }
    }
}
