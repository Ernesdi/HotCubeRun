using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {


    private Transform m_Transform;
    private Transform gem_Transform;
    int orientation;
    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        //获得宝石子物体的Transform
        gem_Transform = m_Transform.FindChild("gem 3").GetComponent<Transform>();
         orientation = GemRotate();
    }
	
	// Update is called once per frame
	void Update () {
        //随机旋转
        if (orientation == 1)
        {
            gem_Transform.Rotate(Vector3.up);
        }
        else if (orientation == 2) {
            gem_Transform.Rotate(Vector3.down);
        }
        else if (orientation == 3)
        {
            gem_Transform.Rotate(Vector3.left);
        }
        else if (orientation == 0)
        {
            gem_Transform.Rotate(Vector3.right);
        }
    }

    private int GemRotate() {
        int pr = Random.Range(1, 100);
        if (pr <= 25)
        {
            return 1;
        }
        else if(pr>25&&pr<=50)
        {
            return 2;
        }
        else if (pr > 50 && pr <= 75)
        {
            return 3;
        }
        return 0;
    }
}
