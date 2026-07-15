using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ActionCommandCircle : MonoBehaviour
{
    private Transform shrinkCircle;

    private bool buttonHit = false;

    private void Start()
    {
        shrinkCircle = transform.GetChild(0);

        StartCoroutine(StartActionCommand());
    }

    private IEnumerator StartActionCommand() {

        yield return new WaitForSeconds(0.05f);

        while (!buttonHit)
        {

            shrinkCircle.localScale -= Vector3.one * 3.5f * Time.deltaTime;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {

                buttonHit = true;

            }
            else if (shrinkCircle.transform.localScale.x < 0.5)
            {
                buttonHit = true;
            }

            yield return null;

        }

        float hit = shrinkCircle.transform.localScale.x;

        if (hit > 0.85 && hit < 1.15)
        {

            Debug.Log("Perfect!");
            EventHolder.OnActionCommandCompletion?.Invoke("Perfect");

        }
        else if (hit > 0.7 && hit < 1.3)
        {

            Debug.Log("Great!");
            EventHolder.OnActionCommandCompletion?.Invoke("Great");

        }
        else if (hit > 0.5 && hit < 1.5)
        {

            Debug.Log("Good!");
            EventHolder.OnActionCommandCompletion?.Invoke("Good");

        }
        else
        {
            EventHolder.OnActionCommandCompletion?.Invoke("Miss");
        }

        Destroy(gameObject);
    }
}
