using UnityEngine;

public class BGScrolling : MonoBehaviour {
    [SerializeField] private float parralax = 2f;
    [SerializeField] private float speed = 2f;
    private Material material;

    // Start is called before the first frame update
    void Start() {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {

        AddParralaxEffectOnAxis(true, false);
    }

    private void MoveAlongXAxis() {
        Vector2 offset = material.mainTextureOffset;
        offset.x += speed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }

    private void AddParralaxEffectOnAxis(bool x, bool y) {
        Vector2 offset = material.mainTextureOffset;
        if (x)
            offset.x = ((transform.position.x / transform.localScale.x) * material.mainTextureScale.x) / parralax;
        if (y)
            offset.y = ((transform.position.y / transform.localScale.y) * material.mainTextureScale.y) / parralax;
        material.mainTextureOffset = offset;
    }
}
