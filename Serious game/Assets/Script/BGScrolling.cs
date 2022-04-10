using UnityEngine;

public class BGScrolling : MonoBehaviour {
    [SerializeField] private float speed = .5f;
    private Material material;

    // Start is called before the first frame update
    void Start() {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        Vector2 offset = material.mainTextureOffset;
        offset.x += speed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }
}
