using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private UserGameManager _userGameManager;
    public SpriteRenderer spriteRenderer;
    public GameObject bubblesObject;
    public AudioSource audioSource;
    public AudioClip captureSound; 
    private Animator animator;

    private void Start()
    {
        _userGameManager = FindObjectOfType<UserGameManager>();
        animator = bubblesObject.GetComponent<Animator>();
        bubblesObject.SetActive(false);
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
        spriteRenderer.enabled = false; // Desactivar el SpriteRenderer
        bubblesObject.SetActive(true); // Activar burbujas
        animator.Play("BurbujasAnim"); // Iniciar la animación de las burbujas
        PlayCaptureSound();
    }

    private void PlayCaptureSound()
    {
        if (audioSource != null && captureSound != null)
        {
            audioSource.PlayOneShot(captureSound);
        }
    }
}
