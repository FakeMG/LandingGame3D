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
        transform.position = new Vector3(transform.position.x, transform.position.y, 30);

        MoveAlongXAxis();
        AddParralaxEffectOnYAxis();
    }

    private void MoveAlongXAxis() {
        Vector2 offset = material.mainTextureOffset;
        offset.x += speed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }

    private void AddParralaxEffectOnYAxis() {
        Vector2 offset = material.mainTextureOffset;
        offset.y = ((transform.position.y / transform.localScale.y) * material.mainTextureScale.y) / parralax;
        material.mainTextureOffset = offset;
    }
}
