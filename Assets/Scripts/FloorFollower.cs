using Unity.VisualScripting;
using UnityEngine;

public class FloorFollower : MonoBehaviour
{
    [Header("References")]
    public Transform hero;
    public float tileLenght = 10f;
    public float offsetZ = 0f;

    private float lastHeroZ;
    private Vector3 startPos;

    private void Start()
    {
        if (hero == null)
        {
            var found = GameObject.FindWithTag("Player");
            if (found) hero = found.transform;
        }

        startPos = transform.position;
        lastHeroZ = hero ? hero.position.z : 0f; 
    }

    private void Update()
    {
        if (hero == null) return;
        float heroZ = hero.position.z;

        if (heroZ - lastHeroZ >= tileLenght)
        {
            MoveForward();
            lastHeroZ = heroZ;
        }
    }

    private void MoveForward()
    {
        Vector3 newPos = transform.position;
        newPos.z += tileLenght;
        newPos.z += offsetZ;
        transform.position = newPos;

    }
    public void ResetFloor()
    {
        transform.position = startPos;
        lastHeroZ = hero ? hero.position.z : 0f;
    }

}
