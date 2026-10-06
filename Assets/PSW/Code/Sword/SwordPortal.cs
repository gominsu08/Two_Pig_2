using UnityEngine;

namespace Assets.PSW.Code.Sword
{
    public class SwordPortal : MonoBehaviour
    {
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private string openClipName;
        [SerializeField] private string closeClipName;

        private int _openHash;
        private int _closeHash;

        private int _damage;
        private Sprite _image;

        private void Awake()
        {
            _openHash = Animator.StringToHash(openClipName);
            _closeHash = Animator.StringToHash(closeClipName);
        }

        public void OpenPortal(Sprite image, int damage)
        {
            _damage = damage;
            _image = image;
            animator.Play(_openHash, 0, 0f);
        }

        public void StartCloseClip() => animator.Play(_closeHash, 0, 0f);

        public void PopSword()
        {
            GameObject swordObject = Instantiate(swordPrefab);
            Sword sword = swordObject.GetComponent<Sword>();

            if (sword == null)
            {
                Destroy(swordObject);
                return;
            }

            sword.StartSword(_image, _damage, transform.position);
        }

        // 포탈 애니메이션의 마지막 프레임에서 호출합니다.
        public void ClosePortal()
        {
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            _image = null;
            _damage = 0;
        }

    }
}
