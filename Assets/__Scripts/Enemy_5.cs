using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part is another serializable data storage class just like WeaponDefinition
/// </summary>
[System.Serializable]


public class Enemy_5 : Enemy {

    [Header("Set in Inspector: Enemy_5")]
    static public Transform PROJECTILE_ANCHOR;

    [SerializeField]
    private Vector3 p0, p1; // The two points to interpolate
    private float timeStart; // Birth time for this Enemy_4
    private float duration = 4; // Duration of movement
    private float lastShotTime;
    public int numberShotsinburst = 3;
    private int numberofShotsleft;

    private Renderer collarRend;
    public GameObject collar;

    public WeaponDefinition def;
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float continuousDamage = 0; // Damage per second (Laser)
    public float delayBetweenShots = -1;
    public float velocity = 20; // Speed of projectiles


    
    

    private void Start()
    {
        
        lastShotTime = Time.time;
        // There is already an initial position chosen by Main.SpawnEnemy()
        // so add it to points as the initial p0 & p1
        p0 = p1 = pos;

        collar = transform.Find("Collar_1").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        InitMovement();

        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
       
    }

    void InitMovement()
    {
        p0 = p1; // Set p0 to the old p1
        // Assign a new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Reset the time
        timeStart = Time.time;
    }

public void Fire()
    {
        Debug.Log("Weapon Fired:" + gameObject.name);
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
         p = MakeProjectile();
            p.rigid.velocity = vel;
        if (transform.up.y < 0)
        {
        vel.y = -vel.y;
        }
    }
    public override void Move()
    {
        // This completely overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1) //start firing process
        
        { // start to move again
            InitMovement();
            u = 0;
        }
        if (Time.time - lastShotTime < fireRate){
            print("taking shot at" + Time.time);
            lastShotTime = Time.time;
            
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = ((1 - u) * p0) + (u * p1);// Simple linear interpolation
    }
    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        go.tag = "ProjectileEnemy";
        go.layer = LayerMask.NameToLayer("ProjectileEnemy");
    
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        lastShotTime = Time.time;
        return p;
    }
    
    // This changes the color of just one Part to red instead of the whole ship.
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
}
  

