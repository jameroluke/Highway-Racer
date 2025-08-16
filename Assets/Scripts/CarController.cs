using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CarController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.12f;
    [SerializeField] private Ease ease = Ease.OutQuad;

    [Header("Collision")]
    [SerializeField] private GameObject explosionGameObject;
    [SerializeField] private GameObject fireGameObject;

    private int _lane;
    private int _laneCount;
    private float _laneSpacing;
    private int _startLane;
    private Vector2 _touchStart;
    private bool _enableControl;

    public void EnableControl(bool enable) => _enableControl = enable;

    private void Start() {
        var gameManager = GameManager.Instance;
        if (gameManager == null) {
            Debug.LogError("GameManager instance not found. Please ensure GameManager is initialized before CarController.");
            return;
        }
        _laneCount = gameManager.LaneCount;
        _laneSpacing = gameManager.LaneSpacing;
        _startLane = gameManager.StartLane;
        _lane = Mathf.Clamp(_startLane, 0, _laneCount - 1);
        SnapToLane(_lane);
    }

    private void Update() {
        if (_enableControl) {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                Move(-1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                Move(+1);
            }
        }
    }

    private void Move(int dir) {
        int targetLane = Mathf.Clamp(_lane + dir, 0, _laneCount - 1);
        if (targetLane == _lane) {
            return;
        }

        _lane = targetLane;
        Vector3 target = LanePosition(_lane);

        transform.DOKill();
        transform.DOMove(target, moveDuration).SetEase(ease);
    }

    private Vector3 LanePosition(int laneIndex) {
        float leftMostX = -((_laneCount - 1) * 0.5f) * _laneSpacing;
        float x = leftMostX + laneIndex * _laneSpacing;
        return new Vector3(x, transform.position.y, transform.position.z);
    }

    private void SnapToLane(int laneIndex) => transform.position = LanePosition(laneIndex);

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Obstacle>(out var obstacle)) {
            transform.DOKill();
            SoundManager.Instance.Play("explosion");
            GameManager.Instance.GameOver();
        }

        if (explosionGameObject != null) {
            explosionGameObject.SetActive(true);
        }
        if (fireGameObject != null) {
            fireGameObject.SetActive(true);
        }
    }
}
