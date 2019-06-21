using UnityEngine;
using System.Collections;
/// <summary>
/// 摄像机跟随角色移动  如果把摄像机设置为主角的子物体 那么 就不会有平滑过渡的形式了 显得不好看
/// </summary>
public class CameraFollow : MonoBehaviour {
    //动态控制摄像机是否跟随角色移动
    private bool startFollow = false;
    //声明摄像机的位置
    private Transform m_Transform;
    //声明角色的位置
    private Transform m_Player;

    //用于重置游戏初始化摄像机的位置
    private Vector3 normalPos;
    //封装属性
     public bool StartFollow {
        get { return startFollow; }
        set { startFollow = value; }
       }

    void Start () {
        //获得摄像机的位置
        m_Transform = gameObject.GetComponent<Transform>();
        //用于重置游戏初始化摄像机的位置
        normalPos = m_Transform.position;
        //获得角色的位置
        m_Player = GameObject.Find("cube_books").GetComponent<Transform>();
    }
	
	
	void Update () {
        if (startFollow)
        {
            //摄像机新的位置      m_Player.position.y + 1.08f角色的位置头上一点
            Vector3 nextPot = new Vector3(m_Transform.position.x, m_Player.position.y + 1.5f, m_Player.position.z);
            //插值函数平滑过渡
            m_Transform.position = Vector3.Lerp(m_Transform.position, nextPot,Time.deltaTime);
        }
	}
    /// <summary>
    /// 重置摄像机
    /// </summary>
    public void ResetCamera()
    {
        m_Transform.position = normalPos;
    }
}
