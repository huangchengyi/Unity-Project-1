using UnityEngine;
using System.Collections;

namespace UnityChan
{
    //
    // ↑↓切换循环动画的脚本(随机切换)Ver.3
    //  N.Kobayashi
    //

    // 使用此脚本时需要这些组件
    [RequireComponent(typeof(Animator))]



	public class IdleChanger : MonoBehaviour
	{
	
		private Animator anim;                      // 参照Animator
        private AnimatorStateInfo currentState;     // 保存现在的状态的参照
        private AnimatorStateInfo previousState;    // 参见上一个状态保存
        public bool _random = false;                // 随机判定开始开关
        public float _threshold = 0.5f;             // 随机确定的阈值
        public float _interval = 10f;               // 随机判定的间隔
                                                    //private float _seed = 0.0f;					// 随机确定种子





        // Use this for initialization
        void Start ()
		{
            // 初始化引用
            anim = GetComponent<Animator> ();
			currentState = anim.GetCurrentAnimatorStateInfo (0);
			previousState = currentState;
            // 启动随机确定函数
            StartCoroutine("RandomChange");
		}
	
		// Update is called once per frame
		void  Update ()
		{
            // ↑按下键/空格后，下一步发送状态的处理
            if (Input.GetKeyDown ("up") || Input.GetButton ("Jump")) {
                // 把布利安Next变成true
                anim.SetBool ("Next", true);
			}

            // ↓按下键后，将状态返回前方的处理
            if (Input.GetKeyDown ("down")) {
                // 把布利安Back变成true
                anim.SetBool ("Back", true);
			}

            //"Next"标志为true时的处理
            if (anim.GetBool ("Next")) {
                // 检查当前状态，如果状态名不对，请将布利安返回到false
                currentState = anim.GetCurrentAnimatorStateInfo (0);
				if (previousState.nameHash != currentState.nameHash) {
					anim.SetBool ("Next", false);
					previousState = currentState;				
				}
			}

            // "Back"标志为true时的处理
            if (anim.GetBool ("Back")) {
                // 检查当前状态，如果状态名不对，请将布利安返回到false
                currentState = anim.GetCurrentAnimatorStateInfo (0);
				if (previousState.nameHash != currentState.nameHash) {
					anim.SetBool ("Back", false);
					previousState = currentState;
				}
			}
		}

		void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 110, 10, 100, 90), "Change Motion");
			if (GUI.Button (new Rect (Screen.width - 100, 40, 80, 20), "Next"))
				anim.SetBool ("Next", true);
			if (GUI.Button (new Rect (Screen.width - 100, 70, 80, 20), "Back"))
				anim.SetBool ("Back", true);
		}


        // 随机确定函数
        IEnumerator RandomChange ()
		{
            // 无限循环开始
            while (true) {
                //在随机判定开关开启的情况下
                if (_random) {
                    // 取出随机种子，根据其大小设置标记
                    float _seed = Random.Range (0.0f, 1.0f);
					if (_seed < _threshold) {
						anim.SetBool ("Back", true);
					} else if (_seed >= _threshold) {
						anim.SetBool ("Next", true);
					}
				}
                // 将间隔放到下一个判定
                yield return new WaitForSeconds (_interval);
			}

		}

	}
}
