using UnityEngine;
using System.Collections;
using System.Linq;//LINQ ���ڽ���Ϸ������Ӷ���ת��Ϊ��Ϸ��������

public class Shield : MonoBehaviour
{
    private GameObject[] shieldParts;
    private int hitCount = 0;//������ٴΡ����������������������������������������п�����public
    private IEnumerator currentCoroutine;
    //private IEnumerator currentCoroutine;��һ����Ϊ ��˽�б���������currentCoroutine���ñ���������ΪIEnumerator������ Unity �����ڴ���Э�̵Ľӿڡ�



    //step1: setactive all parts 

    void Start()
    {
        // Automatically find and assign the shield parts
        shieldParts = transform.Cast<Transform>().Select(t => t.gameObject).ToArray();  // ToArray()���ĸ�parts�Ž�����
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

        //�޸�����enemy�Ĺ������������Բ�һ����������жϣ���enemy������currentHitCount = hitcount+1

        hitCount++;

        // ÿ���¹�������һ��part,��������<2ʱ�����Ҫ���»ص�0
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




    //function 1 ��--------------vfxҲ�ڼ��������
    private void DisableNextShieldPart()
    {
        foreach (var part in shieldParts)// ��Ҫ��������collection����������Ĵ����Ǻ���Ҫ��ʱ��
        {
            if (part.activeSelf)
            {
                part.SetActive(false);
                break;
            }
        }
    }


    //����ʱ
    private IEnumerator ReEnableShieldPart()
    {
        yield return new WaitForSeconds(5);//yield return������������ú�for��ϣ�for��һ��ִ������������ʱ����

        for (int i = shieldParts.Length - 1; i >= 0; i--)
        {
            if (!shieldParts[i].activeSelf)
            {
                shieldParts[i].SetActive(true);
                break;
            }
        }

        hitCount = 0; // Reset hit count after re-enabling a part����Ҫ�ɲ�Ҫ�����е�ʱ����Բ�Ҫ
    }
}
