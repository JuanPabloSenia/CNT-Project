using UnityEngine;

public class ComboTextLogic : MonoBehaviour {

    public static float combo;

    public Vector3 startVel;
    float timer;

    public Rigidbody rb;
    public TextMesh text;

    public void StartScore (Vector3 hitPos, bool enemyHit)
    {
        Debug.Log(combo + "Combo");
        transform.position = hitPos;
        timer = 0;
        if (enemyHit)
        {
            combo++;
            text.text = "x " + combo;
            text.color = Color.Lerp(Color.yellow, Color.red, combo / 100f);
        }
        else
        {
            text.text = "Miss";
            text.color = Color.red;
        }
        transform.LookAt(Camera.main.transform);
        rb.velocity = new Vector3(Random.Range(startVel.x, -startVel.x), startVel.y, 0);
	}

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.65f)
            text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), Time.deltaTime*8);
        if (timer > 2)
        {
            gameObject.SetActive(false);
        }
    }
}
