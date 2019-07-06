//
//SpringBone.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpringBone.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class SpringBone : MonoBehaviour
	{
        //下一页


        public Transform child;

        //本垒的方向
        public Vector3 boneAxis = new Vector3 (-1.0f, 0.0f, 0.0f);
		public float radius = 0.05f;

        //每个SpringBone都使用dragForce和stiffnessForce吗
        public bool isUseEachBoneForceSettings = false;

        //弹簧回力
        public float stiffnessForce = 0.01f;

        //力量的衰减力
        public float dragForce = 0.4f;
		public Vector3 springForce = new Vector3 (0.0f, -0.0001f, 0.0f);
		public SpringCollider[] colliders;
		public bool debug = true;
        //Kobayashi:我们应该开始行动了
        public float threshold = 0.01f;
		private float springLength;
		private Quaternion localRotation;
		private Transform trs;
		private Vector3 currTipPos;
		private Vector3 prevTipPos;
		//Kobayashi
		private Transform org;
        //Kobayashi:参考unitychan中的“SpringManager”组件
        private SpringManager managerRef;

		private void Awake ()
		{
			trs = transform;
			localRotation = transform.localRotation;
            //Kobayashi:参考unitychan中的“SpringManager”组件
            // GameObject.Find("unitychan_dynamic").GetComponent<SpringManager>();
            managerRef = GetParentSpringManager (transform);
		}

		private SpringManager GetParentSpringManager (Transform t)
		{
			var springManager = t.GetComponent<SpringManager> ();

			if (springManager != null)
				return springManager;

			if (t.parent != null) {
				return GetParentSpringManager (t.parent);
			}

			return null;
		}

		private void Start ()
		{
			springLength = Vector3.Distance (trs.position, child.position);
			currTipPos = child.position;
			prevTipPos = child.position;
		}

		public void UpdateSpring ()
		{
			//Kobayashi
			org = trs;
            //复位转速
            trs.localRotation = Quaternion.identity * localRotation;

			float sqrDt = Time.deltaTime * Time.deltaTime;

			//stiffness
			Vector3 force = trs.rotation * (boneAxis * stiffnessForce) / sqrDt;

			//drag
			force += (prevTipPos - currTipPos) * dragForce / sqrDt;

			force += springForce / sqrDt;

            //不要和前一帧的值一样。
            Vector3 temp = currTipPos;

			//verlet
			currTipPos = (currTipPos - prevTipPos) + currTipPos + (force * sqrDt);

            //把长度复原
            currTipPos = ((currTipPos - trs.position).normalized * springLength) + trs.position;

            //冲撞判定
            for (int i = 0; i < colliders.Length; i++) {
				if (Vector3.Distance (currTipPos, colliders [i].transform.position) <= (radius + colliders [i].radius)) {
					Vector3 normal = (currTipPos - colliders [i].transform.position).normalized;
					currTipPos = colliders [i].transform.position + (normal * (radius + colliders [i].radius));
					currTipPos = ((currTipPos - trs.position).normalized * springLength) + trs.position;
				}


			}

			prevTipPos = temp;

            //应用旋转
            Vector3 aimVector = trs.TransformDirection (boneAxis);
			Quaternion aimRotation = Quaternion.FromToRotation (aimVector, currTipPos - trs.position);
            //原始
            //trs.rotation = aimRotation * trs.rotation;
            //Kobayahsi:Lerp with mixWeight
            Quaternion secondaryRotation = aimRotation * trs.rotation;
			trs.rotation = Quaternion.Lerp (org.rotation, secondaryRotation, managerRef.dynamicRatio);
		}

		private void OnDrawGizmos ()
		{
			if (debug) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere (currTipPos, radius);
			}
		}
	}
}
