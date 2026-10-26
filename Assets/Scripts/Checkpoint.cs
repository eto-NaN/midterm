using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool activated = false;
    [SerializeField] private AudioSource activateSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        KillPlayer player = other.GetComponent<KillPlayer>();
        if (player == null) return;

        player.SetCheckpoint(transform.position);

        if (activated) return;
        activateSound.Play();
        activated = true;
        ScoreManager.instance.AddScore(20);
    }
}
