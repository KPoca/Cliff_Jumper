using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] int HP = 3;

    public void SetHP(int value){
        HP = value;
    }

    public int GetHP(){
        return HP;
    }
}
