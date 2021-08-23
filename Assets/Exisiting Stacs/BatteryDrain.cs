using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryDrain : MonoBehaviour
{
    public StacsEntity entity;
    public float batteryDrainRateLarge;
    public float batteryDrainRateSmall;

    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
    }
    // Start is called before the first frame update
    void Start()
    {
        entity.batteryState = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(entity.speed > Utils.EPSILON)
            entity.batteryState -= batteryDrainRateLarge * Time.deltaTime;
        else
            entity.batteryState -= batteryDrainRateSmall * Time.deltaTime;

        entity.batteryState = Utils.Clamp(entity.batteryState, 0, 100);
    }
}
