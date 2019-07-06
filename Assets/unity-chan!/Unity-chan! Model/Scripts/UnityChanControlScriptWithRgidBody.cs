//
// Mecanim动画数据当它没有在原点移动时带Rigidbody的控制器
// 样本
// N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
    // 必要组件的列记
    [RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{

		public float animSpeed = 1.5f;              // 动画播放速度设定
        public float lookSmoother = 3.0f;           // 相机运动的平滑设置
        public bool useCurves = true;               // 在Mecanim中设置或调整曲线。
                                                    // 如果没有这个开关，转弯就不被使用。
        public float useCurvesHeight = 0.5f;        // 修正曲线的有效高度(在容易脱离地面时增大)

        // 以下是用于角色控制器的参数
        // 前进速度
        public float forwardSpeed = 7.0f;
		// 后退速度
		public float backwardSpeed = 2.0f;
		// 旋回速度
		public float rotateSpeed = 2.0f;
        //跳跃威力
        public float jumpPower = 3.0f;
        // 角色控制器的参考
        private CapsuleCollider col;
		private Rigidbody rb;
        //角色控制器(卡塞尔协)的移动量


        private Vector3 velocity;
        //CapsuleCollider所设定的coelider的Heiht, Center的初始值的变量
        private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;                          // 对被角色附接的动画制作者的参照。
        private AnimatorStateInfo currentBaseState;         // 基本layer中使用的动画计的当前状态的参照

        private GameObject cameraObject;    // 主照相机的参照

        // 对各状态的参照
        static int idleState = Animator.StringToHash ("Base Layer.Idle");
		static int locoState = Animator.StringToHash ("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash ("Base Layer.Jump");
		static int restState = Animator.StringToHash ("Base Layer.Rest");

		// 初期化
		void Start ()
		{
            // 获得动物部件
            anim = GetComponent<Animator> ();
            // CapsuleCollider取得CapsuleCollider组件(胶囊型韩国)
            col = GetComponent<CapsuleCollider> ();
			rb = GetComponent<Rigidbody> ();
            //取得主摄像头
            cameraObject = GameObject.FindWithTag ("MainCamera");
            //CapsuleCollider组件之夜，保存Center的初始值
            orgColHight = col.height;
			orgVectColCenter = col.center;
		}


        // 下面，由于主处理与延迟主体相关联，所以在FixedUpdate内进行处理。
        void FixedUpdate ()
		{
			float h = Input.GetAxis ("Horizontal");             // 输入装置的水平轴由h定义
            float v = Input.GetAxis ("Vertical");               // 用v定义输入装置的垂直轴
            anim.SetFloat ("Speed", v);                         // 将v传递给Animator方面设置的“Speed”参数
            anim.SetFloat ("Direction", h);                         //交给Animator方面设定的“Direction”参数h
            anim.speed = animSpeed;                             // Animator的运动再生速度设定animSpeed
            currentBaseState = anim.GetCurrentAnimatorStateInfo (0);    // 基本Layer(0)的当前状态被设置为参考状态变量
            rb.useGravity = true;//它们在跳跃过程中就会对重力产生影响。



            // 以下，对人物的移动处理。
            velocity = new Vector3 (0, 0, v);       // 从上下键输入中获取Z轴方向的移动量。
                                                    // 把角色转换成本地空间，
            velocity = transform.TransformDirection (velocity);
            //下面的v的阈值与Mecanim侧上的转换一起调整。
            if (v > 0.1) {
				velocity *= forwardSpeed;       // 运用移动速度
            } else if (v < -0.1) {
				velocity *= backwardSpeed;  // 运用移动速度
            }
		
			if (Input.GetButtonDown ("Jump"))
            {   // 如果你输入空格键

                //动画片的状态只有在Locomotion中间才能跳跃
                if (currentBaseState.nameHash == locoState) {
                    //如果没有状态转移，就可以跳跃。
                    if (!anim.IsInTransition (0)) {
						rb.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool ("Jump", true);        // 向Animator发送一个切换到跳跃的标志
                    }
				}
			}


            // 通过上下键输入来移动角色
            transform.localPosition += velocity * Time.fixedDeltaTime;

            // 通过左右键输入使字符在Y轴上旋转
            transform.Rotate (0, h * rotateSpeed, 0);


            // 以下，在Animator的各状态中的处理
            // Locomotion中
            // 现在的基层是locoState的时候
            if (currentBaseState.nameHash == locoState) {
                //在曲线上调整色块的时候，以防万一进行复位
                if (useCurves) {
					resetCollider ();
				}
			}
            // JUMP中的处理
            // 现在基层jumpState的时候，
            else if (currentBaseState.nameHash == jumpState) {
				cameraObject.SendMessage ("setCameraPositionJumpView"); // 改变成跳跃时的照相机。
                                                                        // 没有转状态的情况
                if (!anim.IsInTransition (0)) {

                    // 以下，进行曲线调整时的处理
                    if (useCurves) {
                        // 以下JUMP00动画上的曲线JumpHeight与GravityControl(玩火控制)
                        // JumpHeight:JUMP00上的跳跃高度(0 ~ 1)
                        // GravityControl:1 -跳跃中(重力无效)，0 -重力有效
                        float jumpHeight = anim.GetFloat ("JumpHeight");
						float gravityControl = anim.GetFloat ("GravityControl"); 
						if (gravityControl > 0)
							rb.useGravity = false;  //摆脱跳跃中的重力影响。

                        // 把角色从角色中心撤下
                        Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit ();
                        // 只有在高达到useCurvesHeight以上时，才将滑翔机的高度和中心调整为JUMP00动画片上的曲线。
                        if (Physics.Raycast (ray, out hitInfo)) {
							if (hitInfo.distance > useCurvesHeight) {
								col.height = orgColHight - jumpHeight;          // 调整后的滑翔机的高度
                                float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3 (0, adjCenterY, 0);    // 被调整过的滑翔机中心
                            } else {
                                // 返回初始值，以低于阈值。
                                resetCollider();
							}
						}
					}
                    // Jump bool值复位(不循环)
                    anim.SetBool ("Jump", false);
				}
			}
            // IDLE中的处理
            // 当基本层都在idleState的时候
            else if (currentBaseState.nameHash == idleState) {
                //在曲线上调整色块的时候，以防万一进行复位
                if (useCurves) {
					resetCollider ();
				}
                // 输入空格键后进入Rest状态
                if (Input.GetButtonDown ("Jump")) {
					anim.SetBool ("Rest", true);
				}
			}
            // REST中的处理
            // 现在的基层在restState的时候
            else if (currentBaseState.nameHash == restState) {
                //cameraObject.SendMessage("setCameraPositionFrontView");		// 把照相机切换到正面
                // 如果状态没有被转移，则重置(以使不循环)Rest bool值。
                if (!anim.IsInTransition (0)) {
					anim.SetBool ("Rest", false);
				}
			}
		}

		void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Interaction");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
			GUI.Label (new Rect (Screen.width - 245, 90, 250, 30), "Hit Spase key while Stopping : Rest");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), "Alt : LookAt Camera");
		}


        // 角色重置函数
        void resetCollider ()
		{
            // 恢复组件的Height、Center的初始值
            col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}