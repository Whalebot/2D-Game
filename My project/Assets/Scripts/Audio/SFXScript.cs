using UnityEngine;

public class SFXScript : MonoBehaviour
{
    [SerializeField]
    public SFX sfx;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlaySFX(sfx, transform.position);
    }
}
