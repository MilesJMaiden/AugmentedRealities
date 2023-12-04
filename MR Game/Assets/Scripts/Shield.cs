using UnityEngine;
using System.Collections;
using System.Linq;//LINQ 用于将游戏对象的子对象转换为游戏对象数组

public class Shield : MonoBehaviour
{
    private GameObject[] shieldParts;
    private int hitCount = 0;//被打多少次——————————————————这里有可能是public
    private IEnumerator currentCoroutine;
    //private IEnumerator currentCoroutine;是一个名为 的私有变量的声明currentCoroutine。该变量的类型为IEnumerator，它是 Unity 中用于创建协程的接口。



    //step1: setactive all parts 

    void Start()
    {
        // Automatically find and assign the shield parts
        shieldParts = transform.Cast<Transform>().Select(t => t.gameObject).ToArray();  // ToArray()把四个parts放进数组
        //LINQ is used to transform the children of a GameObject into an array of GameObjects. This is done using the Select method in combination with the ToArray method:

        // Initialize all parts as active
        foreach (var part in shieldParts)
        {
            part.SetActive(true);
        }
    }


    //step2:  
    public void TakeHit()
    {

        //修改这里enemy的攻击次数，所以差一个攻击后的判断，在enemy攻击后currentHitCount = hitcount+1

        hitCount++;

        // 每两下攻击减少一个part,在这里是<2时，最后要重新回到0
        if (hitCount % 2 == 0)
        {
            DisableNextShieldPart();
        }

        // Reset or start the coroutine for re-enabling shield parts
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = ReEnableShieldPart();
        StartCoroutine(currentCoroutine);
    }




    //function 1 ，--------------vfx也在加在这里！！
    private void DisableNextShieldPart()
    {
        foreach (var part in shieldParts)// 主要用在类似collection这样，对象的次序不是很重要的时候
        {
            if (part.activeSelf)
            {
                part.SetActive(false);
                break;
            }
        }
    }


    //倒计时
    private IEnumerator ReEnableShieldPart()
    {
        yield return new WaitForSeconds(5);//yield return：在这里的作用和for配合，for第一次执行完后继续倒计时五秒

        for (int i = shieldParts.Length - 1; i >= 0; i--)
        {
            if (!shieldParts[i].activeSelf)
            {
                shieldParts[i].SetActive(true);
                break;
            }
        }

        hitCount = 0; // Reset hit count after re-enabling a part，可要可不要，连招的时候可以不要
    }
}
