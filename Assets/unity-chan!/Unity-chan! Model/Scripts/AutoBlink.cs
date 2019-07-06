//
//AutoBlink.cs
//自动手枪脚本
//N.Kobayashi
//
using UnityEngine;
using System.Collections;
using System.Security.Policy;

namespace UnityChan
{
	public class AutoBlink : MonoBehaviour
	{

		public bool isActive = true;                //自动补丁有效
        public SkinnedMeshRenderer ref_SMR_EYE_DEF; //对eye_def的参考
        public SkinnedMeshRenderer ref_SMR_EL_DEF;  //EL _def参考
        public float ratio_Close = 85.0f;           //闭眼混合比例
        public float ratio_HalfClose = 20.0f;       //半闭眼混合比例
        [HideInInspector]
		public float
			ratio_Open = 0.0f;
		private bool timerStarted = false;          //定时启动管理用
        private bool isBlink = false;               //管理眼板用

        public float timeBlink = 0.4f;              //目痴的时间
        private float timeRemining = 0.0f;          //计时器剩余时间

        public float threshold = 0.3f;              // 随机判定的阈值
        public float interval = 3.0f;               // 随机判定的区间



        enum Status
		{
			Close,
			HalfClose,
            Open    //目贴状态
        }


		private Status eyeStatus;   //现在的目袋状态

        void Awake ()
		{
			//ref_SMR_EYE_DEF = GameObject.Find("EYE_DEF").GetComponent<SkinnedMeshRenderer>();
			//ref_SMR_EL_DEF = GameObject.Find("EL_DEF").GetComponent<SkinnedMeshRenderer>();
		}



        // 将其用于初始化
        void Start ()
		{
			ResetTimer ();
            // 启动随机判定函数
            StartCoroutine("RandomChange");
		}

        //复位计时器
        void ResetTimer ()
		{
			timeRemining = timeBlink;
			timerStarted = false;
		}

        //每帧调用一次Update
        void Update ()
		{
			if (!timerStarted) {
				eyeStatus = Status.Close;
				timerStarted = true;
			}
			if (timerStarted) {
				timeRemining -= Time.deltaTime;
				if (timeRemining <= 0.0f) {
					eyeStatus = Status.Open;
					ResetTimer ();
				} else if (timeRemining <= timeBlink * 0.3f) {
					eyeStatus = Status.HalfClose;
				}
			}
		}

		void LateUpdate ()
		{
			if (isActive) {
				if (isBlink) {
					switch (eyeStatus) {
					case Status.Close:
						SetCloseEyes ();
						break;
					case Status.HalfClose:
						SetHalfCloseEyes ();
						break;
					case Status.Open:
						SetOpenEyes ();
						isBlink = false;
						break;
					}
					//Debug.Log(eyeStatus);
				}
			}
		}

		void SetCloseEyes ()
		{
			ref_SMR_EYE_DEF.SetBlendShapeWeight (6, ratio_Close);
			ref_SMR_EL_DEF.SetBlendShapeWeight (6, ratio_Close);
		}

		void SetHalfCloseEyes ()
		{
			ref_SMR_EYE_DEF.SetBlendShapeWeight (6, ratio_HalfClose);
			ref_SMR_EL_DEF.SetBlendShapeWeight (6, ratio_HalfClose);
		}

		void SetOpenEyes ()
		{
			ref_SMR_EYE_DEF.SetBlendShapeWeight (6, ratio_Open);
			ref_SMR_EL_DEF.SetBlendShapeWeight (6, ratio_Open);
		}

        // 随机判定函数
        IEnumerator RandomChange ()
		{
            // 开始无限循环
            while (true) {
                //随机判定种子
                float _seed = Random.Range (0.0f, 1.0f);
				if (!isBlink) {
					if (_seed > threshold) {
						isBlink = true;
					}
				}
                // 设定区间直到下一个判定为止
                yield return new WaitForSeconds (interval);
			}
		}
	}
}