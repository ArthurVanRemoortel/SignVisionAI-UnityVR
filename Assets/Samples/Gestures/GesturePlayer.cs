using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class GesturePlayer : MonoBehaviour
{
    public IKTargetFollowVRRig RigFollower;
    public Animator RigAnimator;
    public AnimationClip[] Clips;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AnimateGesture(AnimationClip clip)
    {
        RigFollower.Follow = false;
        RigAnimator.Play("Waving");
    }
}

[CustomEditor(typeof(GesturePlayer))] // Replace YourScript with the name of your MonoBehaviour script
public class GesturePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GesturePlayer targetScript = (GesturePlayer)target;
        
        foreach (var clip in targetScript.Clips)
        {
            if (GUILayout.Button(clip.name))
            {
                // Define the behavior when the button is clicked
                targetScript.AnimateGesture(clip);
            }
        }
    }
}
