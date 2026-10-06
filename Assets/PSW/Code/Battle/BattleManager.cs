using Assets.PSW.Code.Datas;
using Assets.PSW.Code.Sword;
using System.Collections.Generic;
using UnityEngine;

namespace PSW.Code.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private List<TestBattleData> testDataList = new List<TestBattleData>();
        [SerializeField] private SwordPortal swordPortalPrefab;
        [SerializeField] private Camera battleCamera;
        [SerializeField] private float spawnZ;
        [Header("생성 범위 (화면 비율 0~1)")]
        [Tooltip("생성 영역의 왼쪽 아래. (0, 0)은 화면 왼쪽 아래입니다.")]
        [SerializeField] private Vector2 viewportMin = new Vector2(0f, 0.7f);
        [Tooltip("생성 영역의 오른쪽 위. (1, 1)은 화면 오른쪽 위입니다.")]
        [SerializeField] private Vector2 viewportMax = new Vector2(0.1f, 0.8f);

        private float[] _elapsedTimes;

        private void Start()
        {
            if (battleCamera == null)
            {
                battleCamera = Camera.main;
            }

            if (battleCamera == null || swordPortalPrefab == null)
            {
                Debug.LogError("BattleManager에 카메라와 SwordPortal 프리팹을 연결하세요.", this);
                enabled = false;
                return;
            }

            _elapsedTimes = new float[testDataList.Count];
        }

        private void Update()
        {
            if (_elapsedTimes.Length != testDataList.Count)
            {
                System.Array.Resize(ref _elapsedTimes, testDataList.Count);
            }

            for (int i = 0; i < testDataList.Count; i++)
            {
                TestBattleData data = testDataList[i];
                if (data.foolTime <= 0f || float.IsNaN(data.foolTime) || float.IsInfinity(data.foolTime))
                {
                    _elapsedTimes[i] = 0f;
                    continue;
                }

                _elapsedTimes[i] += Time.deltaTime;
                if (_elapsedTimes[i] < data.foolTime)
                {
                    continue;
                }

                // 프레임 지연 시 한꺼번에 여러 포탈을 생성하지 않습니다.
                _elapsedTimes[i] %= data.foolTime;
                SpawnPortal(data);
            }
        }

        private void SpawnPortal(TestBattleData data)
        {
            Vector2 viewportPosition = new Vector2(
                Random.Range(viewportMin.x, viewportMax.x),
                Random.Range(viewportMin.y, viewportMax.y));
            if (!TryGetSpawnPosition(battleCamera, viewportPosition, out Vector3 position))
            {
                return;
            }

            if (!TryGetSpawnPosition(battleCamera, new Vector2(1f, viewportPosition.y), out Vector3 rightEdge))
            {
                return;
            }

            SwordPortal portal = Instantiate(swordPortalPrefab, position, swordPortalPrefab.transform.rotation);
            portal.OpenPortal(data.swordImage, data.damage, rightEdge.x);
        }

        private bool TryGetSpawnPosition(Camera camera, Vector2 viewportPosition, out Vector3 position)
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(viewportPosition.x, viewportPosition.y, 0f));
            Plane spawnPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, spawnZ));
            if (spawnPlane.Raycast(ray, out float distance))
            {
                position = ray.GetPoint(distance);
                return true;
            }

            position = default;
            return false;
        }

        private void OnValidate()
        {
            Vector2 min = new Vector2(Mathf.Clamp01(viewportMin.x), Mathf.Clamp01(viewportMin.y));
            Vector2 max = new Vector2(Mathf.Clamp01(viewportMax.x), Mathf.Clamp01(viewportMax.y));
            viewportMin = Vector2.Min(min, max);
            viewportMax = Vector2.Max(min, max);
        }

        private void OnDrawGizmosSelected()
        {
            Camera camera = battleCamera != null ? battleCamera : Camera.main;
            if (camera == null)
            {
                return;
            }

            if (!TryGetSpawnPosition(camera, viewportMin, out Vector3 bottomLeft)
                || !TryGetSpawnPosition(camera, new Vector2(viewportMax.x, viewportMin.y), out Vector3 bottomRight)
                || !TryGetSpawnPosition(camera, viewportMax, out Vector3 topRight)
                || !TryGetSpawnPosition(camera, new Vector2(viewportMin.x, viewportMax.y), out Vector3 topLeft))
            {
                return;
            }

            Color previousColor = Gizmos.color;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.color = previousColor;
        }
    }
}
