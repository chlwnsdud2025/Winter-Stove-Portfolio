using UnityEngine;

public class JengaOutManager : MonoBehaviour
{
    public JengaManager jengaM;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jenga"))
        {
            jengaM.TriggerGameOver();
        }
    }
}
