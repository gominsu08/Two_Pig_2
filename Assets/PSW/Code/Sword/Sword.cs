using UnityEngine;

namespace Assets.PSW.Code.Sword
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Sword : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float speed = 3.5f;

        private int _damage;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.linearDamping = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        public void StartSword(Sprite image, int damage, Vector3 pos)
        {
            transform.position = pos;
            _damage = damage;
            spriteRenderer.sprite = image;
            _rigidbody.linearVelocity = Vector2.right * speed;
        }

        private void OnDisable()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
    }
}
