//
// Unity用第三人称相机
// 
//  N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		public float smooth = 3f;       // 照相机动作的平滑变数
        Transform standardPos;			// the usual position for the camera, specified by a transform in the game
		Transform frontPos;			// Front Camera locater
		Transform jumpPos;          // Jump Camera locater

        // 不能顺利连接时(快速切换)的提升标志
        bool bQuickSwitch = false;	//Change Camera Position Quickly
	
	
		void Start ()
		{
            // 参照的初始化
            standardPos = GameObject.Find ("CamPos").transform;
		
			if (GameObject.Find ("FrontPos"))
				frontPos = GameObject.Find ("FrontPos").transform;

			if (GameObject.Find ("JumpPos"))
				jumpPos = GameObject.Find ("JumpPos").transform;

            //开始照相机
            transform.position = standardPos.position;	
			transform.forward = standardPos.forward;	
		}
	
		void FixedUpdate () // 该照相机的开关如果不是在FixedUpdate内，就不能正常运转。
        {
		
			if (Input.GetButton ("Fire1")) {    // left Ctlr	
                                                // 改变前置摄像头
                setCameraPositionFrontView();
			} else if (Input.GetButton ("Fire2")) { //Alt	
                                                    // 改变跳相机
                setCameraPositionJumpView();
			} else {
                // 将相机恢复到标准位置和方向
                setCameraPositionNormalView();
			}
		}

		void setCameraPositionNormalView ()
		{
			if (bQuickSwitch == false) {
                // 相机到标准位置和方向
                transform.position = Vector3.Lerp (transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
				transform.forward = Vector3.Lerp (transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
			} else {
                // 相机到标准位置和方向/快速变化
                transform.position = standardPos.position;	
				transform.forward = standardPos.forward;
				bQuickSwitch = false;
			}
		}
	
		void setCameraPositionFrontView ()
		{
            // 改变前置摄像头
            bQuickSwitch = true;
			transform.position = frontPos.position;	
			transform.forward = frontPos.forward;
		}

		void setCameraPositionJumpView ()
		{
            // 改变跳相机
            bQuickSwitch = false;
			transform.position = Vector3.Lerp (transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);	
			transform.forward = Vector3.Lerp (transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);		
		}
	}
}