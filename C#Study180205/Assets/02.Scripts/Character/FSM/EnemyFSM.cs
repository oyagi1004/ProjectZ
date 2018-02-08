using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour {

    // 플레이어 트랜스폼.
    protected Transform playerTransform;

    // 다음 목표 지점.(순찰 시)
    protected Vector3 destPos;

    // 정찰 지점 목록.
    [SerializeField]
    protected GameObject[] pointList;

    //정찰 포인트 인덱스.
    protected int wanderIndex;

    //델타 타임.
    protected float elapsedTime;

    //시야 각
    [SerializeField]
    protected float detectionAngle;

    //회전 속도.
    [SerializeField]
    protected float rotSpeed;

    [SerializeField]
    protected float NextPointOffset;

    [SerializeField]
    protected float ReturnChaseDist;

    [SerializeField]
    protected float AttackDetectionDist;

    [SerializeField]
    protected float ChaseDetectionDist;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

	void Start () {
        Initialize();
	}
	
	void Update () {
        FSMUpdate();
	}

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
