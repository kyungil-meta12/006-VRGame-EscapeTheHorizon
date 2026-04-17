using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefabs;
    public Transform[] spawnPoints;
    public Transform trackTarget;
    private float currentTime;

    void Update()
    {
        if(SG_GameMan.Inst.pauseState)
        {
            return;
        }

        currentTime += Time.deltaTime;
        if(currentTime >= SG_GameMan.Inst.currentSpawnInterval)
        {
            var randomZombieIndex = Random.Range(0, zombiePrefabs.Length);
            var randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            var newZombie = SG_ObjectPool.Inst.GetInstance(zombiePrefabs[randomZombieIndex]);
            newZombie.transform.position = spawnPoints[randomSpawnIndex].position;
            var comp = newZombie.GetComponent<Zombie>();
            comp.ResetState();
            comp.trackTarget = trackTarget;
            currentTime -= SG_GameMan.Inst.currentSpawnInterval;
        }
    }
}
