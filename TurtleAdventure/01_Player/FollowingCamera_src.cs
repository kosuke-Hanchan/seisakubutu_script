using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera_src : MonoBehaviour
{
    private GameObject player;   
    private Vector3 offset;      

    // Use this for initialization
    void Start()
    {
        player = this.transform.parent.gameObject;
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, 6.0f * Time.deltaTime);
    }
}