using UnityEngine;

namespace Assets.PSW.Code.Sword
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Sword : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float speed = 3.5f;
        [Tooltip("화면 오른쪽 경계를 벗어난 뒤 추가로 이동할 거리입니다.")]
        [SerializeField, Min(0f)] private float despawnMargin = 1f;

        private int _damage;
        private Rigidbody2D _rigidbody;
        private bool _hasHit;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.linearDamping = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        public void StartSword(Sprite image, int damage, Vector3 pos, float screenRightX)
        {
            if (speed <= 0f || float.IsNaN(speed) || float.IsInfinity(speed))
            {
                Debug.LogError("Sword의 speed는 0보다 큰 유한한 값이어야 합니다.", this);
                Destroy(gameObject);
                return;
            }

            transform.position = pos;
            _hasHit = false;
            _damage = damage;
            spriteRenderer.sprite = image;
            _rigidbody.linearVelocity = Vector2.right * speed;

            // 고정 카메라와 일정한 오른쪽 이동을 전제로 한 수명입니다.
            // 풀링 도입 시 예약 삭제를 취소 가능한 반환 타이머로 교체해야 합니다.
            float distance = Mathf.Max(0f, screenRightX + despawnMargin - pos.x);
            Destroy(gameObject, distance / speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasHit)
            {
                return;
            }

            Dummy dummy = other.GetComponentInParent<Dummy>();
            if (dummy == null || !dummy.isActiveAndEnabled)
            {
                return;
            }

            // Destroy가 처리되기 전 추가 콜백이 들어와도 피해는 한 번만 전달합니다.
            _hasHit = true;
            dummy.TakeHit(_damage);
            Destroy(gameObject);
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
