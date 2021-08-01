using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public Text textScore;
    [SerializeField] private int score;

    private static int prePlatformID;
    private int skipOverMultiply = 2;


    private void OnCollisionEnter(Collision collision)
    {
        CenterPlayer temp = collision.transform.GetComponent<CenterPlayer>();
        if (temp && gameObject)
        {
            if(temp.id - prePlatformID == 1)
            {
                prePlatformID = temp.id;
                score++;
                skipOverMultiply = 2;
            }
            if(temp.id - prePlatformID > 1)
            {
                score += (temp.id - prePlatformID) * skipOverMultiply;
                prePlatformID = temp.id;
                skipOverMultiply += 2;
            }

            textScore.text = score.ToString();
        }
    }
}
