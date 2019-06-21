using UnityEngine;
using System.Collections;
/// <summary>
/// 地面陷阱
/// </summary>
public class Spikes : MonoBehaviour {
    //声明地面陷阱父物体的Transform
    private Transform m_Transform;
    //声明地面陷阱子物体钉子的Transform
    private Transform son_Transform;
    //声明钉子默认的位置
    private Vector3 normalPos = new Vector3();
    //声明钉子偏移的位置
    private Vector3 targetPos = new Vector3();
    void Start () {
        //获得地面陷阱父物体的Transform
        m_Transform = gameObject.GetComponent<Transform>();
        //获得地面陷阱子物体钉子的Transform
        son_Transform = m_Transform.FindChild("moving_spikes_b").GetComponent<Transform>();
        //把初始化出来物体的位置设定为默认位置
        normalPos = m_Transform.position;
        //目标位置是钉子的原来位置加上一个向上的偏移量
        targetPos = son_Transform.position + new Vector3(0, 0.15f, 0);
        //开始调用携程 （开始播放动画？）
        StartCoroutine("UpAndDown");
    }



    private IEnumerator UpAndDown() {
        while (true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);         
        }
    }
    /// <summary>
    /// 钉子向上的协程（Lerp）
    /// </summary>
    /// <returns></returns>
    private IEnumerator Up()
    {
        //死循环
        while (true)
        {
            //因为是向上，开始的位置是默认初始化出来的位置，目标位置是有一个偏移量，使用插值函数平滑过渡
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetPos,Time.deltaTime*25);
            //暂停一帧
            yield return null;
        }
        
    }

    /// <summary>
    /// 钉子向下的协程（Lerp）
    /// </summary>
    /// <returns></returns>
    private IEnumerator Down()
    {
        while (true)
        {
            //因为是向下，开始的位置是向上之后到达的位置，目标位置是初始化生成物体的位置，使用插值函数平滑过渡
            son_Transform.position = Vector3.Lerp(son_Transform.position, normalPos, Time.deltaTime * 25);
            //暂停一帧
            yield return null;
        }

    }
}
