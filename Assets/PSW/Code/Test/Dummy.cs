using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private string hitAnimation;
    [SerializeField] private string idleAnimation;

    private int _hitHash;
    private int _idleHash;

    private void Awake()
    {
        _hitHash = Animator.StringToHash(hitAnimation);
        _idleHash = Animator.StringToHash(idleAnimation);
    }

    public void TakeHit(int damage)
    {
        // 체력 처리는 추후 추가합니다. 연속 피격 시에도 Hit를 처음부터 재생합니다.
        animator.Play(_hitHash, 0, 0f);
    }
}
