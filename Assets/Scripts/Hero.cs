using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hero : MonoBehaviour
{

    public NavMeshAgent agent;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                //hit.point;
                //print(hit.point);

                agent.SetDestination(hit.point);
                //鼠标左键点击位置为人物移动最终位置
            }
        }
        Cursor.visible = true;

        //保持鼠标不隐藏

        Cursor.lockState = 0;

        anim.SetFloat("speed", agent.velocity.magnitude);

    }
}
