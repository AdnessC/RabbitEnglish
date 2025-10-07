using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraMovement : MonoBehaviour
{
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ��������� ������� ������, ����� ��������� �� �������
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = newPosition;
    }
}