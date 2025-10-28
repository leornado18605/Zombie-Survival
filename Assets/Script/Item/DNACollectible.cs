using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DNACollectible : MonoBehaviour
{
    [Header("DNA Settings")]
    [SerializeField] private int dnaValue = 1;          // giá trị DNA nhận được
    [SerializeField] private AudioClip pickupSound;     // âm thanh khi nhặt
    [SerializeField] private ParticleSystem pickupEffect; // hiệu ứng khi nhặt

    private bool isCollected = false; // tránh bị nhặt 2 lần

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return; // đã nhặt rồi thì bỏ qua

        // Kiểm tra xem đối tượng có script PlayerEvolution không
        PlayerEvolution player = other.GetComponent<PlayerEvolution>();
        if (player != null)
        {
            isCollected = true;

            // Cộng DNA cho người chơi
            player.AddDNA(dnaValue);

            // Hiệu ứng âm thanh & particle
            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Xóa vật phẩm
            Destroy(gameObject);
        }
    }
}