using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressGraphProto : MonoBehaviour
{
    public LineRenderer SampleLR;
    public DynamicLineProto SampleLine;
    // Start is called before the first frame update
    void Start()
    {
        SampleLine = new DynamicLineProto(SampleLR, new Vector3(-50, -50, 0));

        //SampleLine.Add(Vector3.zero);
    }

    // Update is called once per frame
    float x = 0;
    float y = 0;
    int count = 0;
    void Update()
    {
        /*        
        if(Time.frameCount % 10 == 0) {
            SampleLine.Add(new Vector3(x, y, 0));
            SampleLine.Redraw();
            x += 5;
            y = Mathf.Sin(Mathf.Deg2Rad * (x%360));
        }
        */
        if(GATest.inst.reports.Count > count) {
            for(int i = count; i < GATest.inst.reports.Count; i++) {
                GAReport gar = GATest.inst.reports[i];
                SampleLine.Add(new Vector3(gar.gen, gar.bestObjective, 0));
            }
            count = GATest.inst.reports.Count;
        }
        SampleLine.Redraw();

    }

}
