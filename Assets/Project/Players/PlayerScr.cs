using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;

public class PlayerScr : PhotonView, IPunObservable {

    Vector3 NetPos;
    Quaternion NetRot;

    ButtonControl Bcontrol;

    public GameObject DeadBody;

    Vector3 NetVel;
    public Rigidbody MyPhys;
    public Camera MyCam;
    public Animator MyAnim;
    public Canvas MyName;
    
    private bool AttackAvailable { get; set; }
    public float AttackPower;
    float PrevX { get; set; }
    public static float Sensitivity { get; set;}

    Vector3 PushForce { get; set; }

    void Start()
    {
        Bcontrol = GameObject.FindGameObjectWithTag("Manage").GetComponent<ButtonControl>();
        MyAnim = GetComponent<Animator>();
        if (!PhotonNetwork.IsMasterClient)
        {
            GameManager.RoundGoingOn = true;
            GameManager.Awaiting = false;
        }
        Debug.Log(PhotonNetwork.IsMasterClient + " " + GameManager.RoundGoingOn + " " + GameManager.Awaiting);
        GameManager.PlayersAlive++;
        AttackAvailable = true;

        //настройки свой-чужой объект(нацелить ники/отрубить камеры/отрубить свой ник)
        if (this.IsMine)
        {
            GameManager.ActiveCam = MyCam;
            MyName.enabled = false;
        } else
        {
            MyName.GetComponentInChildren<Text>().text = this.Owner.NickName;
            MyCam.enabled = false;
            MyPhys.isKinematic = true;
        }
    }

    void FixedUpdate () {
        //MyPhys.rotation = Quaternion.Lerp(MyPhys.rotation,Quaternion.identity, 0.01f*Time.deltaTime);
        if (this.IsMine)
        {
            int horizontal = 0;
            int forward = 0;
            if (Input.GetKey(KeyCode.W))
            {
                forward += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                forward -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontal -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontal += 1;
            }

            //по религиозным причинам я кубы двигаю через rigidbody.velocity т.к. его еще нужно нормально настроить,
            //а я люблю отзывчивое управление, без всяких медленных торможений и вялого разгона.
            Vector3 direction = transform.forward * forward;
            direction = (direction.normalized + ((transform.right) * horizontal).normalized).normalized * 7f;
            MyPhys.velocity = new Vector3(direction.x,MyPhys.velocity.y,direction.z) + PushForce;

            //ввел что бы не хреначило как юлу, от столкновений
            MyPhys.angularVelocity = Vector3.Lerp( MyPhys.angularVelocity, Vector3.zero, 20f*Time.deltaTime);
            
            //поворот перса
            transform.Rotate(0, Input.GetAxis("Mouse X") * Sensitivity, 0);
            
            //сторонние силы, что действовали на меня
            PushForce = Vector3.Lerp(PushForce, Vector3.zero, 5f * Time.deltaTime);


            //атака
            if (Input.GetMouseButton(0) && AttackAvailable)
            {
                StartCoroutine(Attack());
            }
        } else
        {
            //сглаживание на врагах
            MyCam.enabled = false;
            transform.position = Vector3.Lerp(transform.position, NetPos, 15f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, NetRot, 20f * Time.deltaTime);
            MyPhys.velocity = Vector3.Lerp(MyPhys.velocity, NetVel, 20f * Time.deltaTime);
        }
        MyAnim.SetFloat("Speed", MyPhys.velocity.magnitude * 9);
    }

    //Произошла синхронизация
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 vel = MyPhys.velocity;
        stream.Serialize(ref pos);
        stream.Serialize(ref rot);
        stream.Serialize(ref vel);
        if (stream.IsReading)
        {
            NetPos = pos;
            NetRot = rot;
            NetVel = vel;
        }
    }

    //промежуток между атаками
    IEnumerator Attack()
    {
        AttackAvailable = false;
        this.RPC("Pushing", RpcTarget.All);
        yield return new WaitForSeconds(1f);
        AttackAvailable = true;
    }
    //Это атака, она выполнена в виде анимации по включению отключению триггера. Сам скрипт атаки Weapon
    [PunRPC]
    public void Pushing()
    {
        MyAnim.Play("Attack");
    }

    [PunRPC]
    public void KillMe()
    {
        if (GameManager.PlayersAlive < 2)
        {
            Bcontrol.SetWinner(this.Owner.NickName);
            Bcontrol.WinAnnounce.SetActive(true);
        }
        Instantiate(DeadBody,new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), transform.rotation);
        GameManager.ImDead = true;
        GameManager.PlayersAlive--;
        Debug.Log("Умер");
        Destroy(gameObject);
    }

    //замена rigidbody.addforce т.к. управляю персонажем напрямую через velocity
    public void SetPush(Vector3 force)
    {
        PushForce = PushForce + force;
    }



    void OnEnable()
    {
        ManagerAssist.FinalizeRound += KillMe;
    }

    void OnDisable()
    {
        ManagerAssist.FinalizeRound -= KillMe;
    }
}
