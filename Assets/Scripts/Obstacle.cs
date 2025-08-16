using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour {
    private float _speed;

    public void Init(float speed) {
        _speed = speed;
    }

    private void Update() {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }
}
