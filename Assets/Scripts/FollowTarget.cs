using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    public Transform hero;
    //获取hero当前位置

    private Vector3 offset;
    //位置偏移

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - hero.position;

        //位置偏移=当前位置-hero的位置
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = offset + hero.position;

        //当前位置=偏移位置+hero位置
    }
}
