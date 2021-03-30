using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinTest : MonoBehaviour
{
    public Player[] players;
    public int style;
    Player player;
    [SerializeField]
    Animator animator;
    Coroutine timer;
    // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(players[style]);
        animator = player.GetComponent<Animator>();
    }

    IEnumerator AnimationTimer(string animation)
    {
        float time = 0f;

        while (time < 2f)
        {
            time += 1;
            yield return new WaitForSeconds(1f);
        }
        animator.SetInteger("action", 0);
        timer = null;
        yield break;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Walk");
            animator.SetInteger("action", 1);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Walk"));
        }


        if (Input.GetKey(KeyCode.I))
        {
            Debug.Log("Idle");
            animator.SetInteger("action", 0);

        }

        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("Bump");
            animator.SetInteger("action", 5);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Bump"));
        }

        if (Input.GetKey(KeyCode.C))
        {
            Debug.Log("Crash");
            animator.SetInteger("action", 4);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Crash"));
        }

        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("Drop");
            animator.SetInteger("action", 6);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Drop"));
        }

        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Stop");
            animator.SetInteger("action", 2);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Stop"));
        }

        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("Ride");
            animator.SetInteger("action", 3);
            if (timer == null)
                timer = StartCoroutine(AnimationTimer("Ride"));
        }


    }
}
