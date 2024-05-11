using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private UserGameManager _userGameManager;

    private void Start()
    {
        _userGameManager = FindObjectOfType<UserGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fin"))
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        _userGameManager.IncrementScore();
    }
}
