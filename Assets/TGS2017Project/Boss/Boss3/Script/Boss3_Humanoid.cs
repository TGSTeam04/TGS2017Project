using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[SelectionBase]
public class Boss3_Humanoid : MonoBehaviour
{
    //分裂時と合体時のコントロールをするクラスの参照
    private Boss3_Controller m_BossController;
    public RocketBattery m_Battery;

    [SerializeField] float m_MaxHP = 100;
    private float m_HP;
    //移動
    [SerializeField] float m_Speed;
    //ロケットパンチ
    [SerializeField] float m_RocketSpeed;
    [SerializeField] float m_RocketRange;
    [SerializeField] float m_RocketInterval;
    [SerializeField] float m_NeedMoveDistance;    

    //各コンポーネント    
    private NavMeshAgent m_NavAgent;
    private Rigidbody m_Rb;
    private Animator m_Anim;    
    private Damageable m_DamageComp;

    //ビヘイビアと付随する値
    private BehaviorTree m_BT;
    private BBoard m_BB;

    //エフェクト
    [SerializeField] GameObject m_Explosion;
    [SerializeField] GameObject m_Efect_Numbness;
    [SerializeField] float m_NumbnessInterval = 30.0f;
    private List<GameObject> m_Numbness = new List<GameObject>();

    float m_RemainingNI;

    public GameObject tempTarget;

    private void Awake()
    {
        m_BossController = GetComponentInParent<Boss3_Controller>();
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rb = GetComponent<Rigidbody>();
        m_Anim = GetComponentInChildren<Animator>();
        m_DamageComp = GetComponent<Damageable>();
        m_DamageComp.Del_ReciveDamage = Damaged;

        m_BT = GetComponent<BehaviorTree>();
        m_BB = GetComponent<BBoard>();
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(UpdateTarget());
        //ビヘイビアツリーのセットアップ
        SetUpBT();
    }
    private IEnumerator UpdateTarget()
    {
        while (true)
        {
			GameObject target = PlayerManager.Instance.NearPlayer(transform.position).gameObject;
            //Debug.Log("Target Update" + target.ToString());
            //GameObject target = tempTarget;
            m_BB.GObjValues["target"] = target; //tempTarget;
            yield return new WaitForSeconds(3.0f);
        }
    }

    private void OnEnable()
    {
        m_HP = m_MaxHP;
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.0f);
        m_BT.IsStop = false;
    }
    // Update is called once per frame
    public void BossUpdate()
    {
        m_BT.BUpdate();

        //アニメーションの値をセット
        Vector3 vel = transform.rotation * -m_NavAgent.velocity;
        m_Anim.SetFloat("Forward", vel.z / m_NavAgent.speed);
        m_Anim.SetFloat("Right", vel.x / m_NavAgent.speed);
    }
    private void Damaged(float damage, MonoBehaviour src)
    {
        m_HP = Mathf.Max(0, m_HP - damage);
        m_RemainingNI -= damage;
        if (m_RemainingNI < 0)
        {
            GameObject shockEff = Instantiate(m_Efect_Numbness, transform);
            Vector3 modiy = new Vector3(0, 1.7f, 0)
                + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 1f), Random.Range(-0.2f, 0.2f));            
            shockEff.transform.position = transform.position + modiy;
            m_Numbness.Add(shockEff);

            m_RemainingNI = m_NumbnessInterval;
        }
        if (m_HP <= 0)
        {
            foreach (var numbness in m_Numbness)
                Destroy(numbness);
            Release();
            return;
        }
        m_Anim.SetTrigger("Damage");
    }

    public void Dead()
    {
        Instantiate(m_Explosion, transform.position, transform.rotation);
    }

    public void Release()
    {
        m_BT.BTReset();
        m_BossController.ReleaseStart();
    }

    //ビヘイビアの設定
    private void SetUpBT()
    {
        m_BB = GetComponent<BBoard>();
        m_BT = GetComponent<BehaviorTree>();
        m_BT.Init();
        m_BT.SetBoard(m_BB);

        BParallel par_Look_Other = new BParallel();
        //LookAtPlayer
        BT_LookTarget lookTarget = new BT_LookTarget("target", 5.0f);
        BSelector sl_Rocket_Other = new BSelector();
        m_BT.SetRootNode(par_Look_Other);

        /*ロケットパンチ*/
        BT_FireRocket BTT_Rocket = new BT_FireRocket(m_Battery);
        BDecorator dec_RocektRnage = new BD_CloserThen("target", m_RocketRange);
        BTT_Rocket.AddDecorator(dec_RocektRnage);
        BTT_Rocket.AddDecorator(new BD_CoolTime(m_RocketInterval));

        //接近
        BSequence seq = new BSequence();
        BT_MoveTo moveToRange = new BT_MoveTo("target", m_Speed);
        moveToRange.m_StopDistance = m_RocketRange;
        moveToRange.m_IsCanCancelMove = false;
        //moveToRange.AddDecorator(new BD_CloserThen("target", m_NeedMoveDistance).Invert());

        seq.AddNode(moveToRange);

        sl_Rocket_Other.AddNode(BTT_Rocket);
        sl_Rocket_Other.AddNode(seq);
        par_Look_Other.AddNode(lookTarget);
        par_Look_Other.AddNode(sl_Rocket_Other);
    }
}
