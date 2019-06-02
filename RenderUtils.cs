using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderUtils
{
    public static void FillBoneMap(Transform i_root, ref Dictionary<string, Transform> o_boneMap)
    {
        Debug.Assert(!o_boneMap.ContainsKey(i_root.gameObject.name), "Bone " + i_root.gameObject.name  + " already in map!");
        o_boneMap.Add(i_root.gameObject.name, i_root);
        foreach (Transform child in i_root)
        {
            FillBoneMap(child, ref o_boneMap);
        }
    }

    public static void Combine(SkinnedMeshRenderer renderer, Dictionary<string, Transform> boneMap)
    {
        Transform[] newBones = new Transform[renderer.bones.Length];
        for (int i = 0; i < renderer.bones.Length; ++i)
        {
            Transform boneTransform = renderer.bones[i];
            if (boneTransform != null)
            {
                GameObject bone = boneTransform.gameObject;
                if (!boneMap.TryGetValue(bone.name, out newBones[i]))
                {
                    Debug.Assert(false, "No puedorrrrr");
                    break;
                }
            }
        }
        renderer.bones = newBones;
        renderer.rootBone = boneMap[renderer.rootBone.gameObject.name];
    }

    public static void Combine(SkinnedMeshRenderer renderer, Transform structure)
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        FillBoneMap(structure, ref boneMap);
        Combine(renderer, boneMap);
    }
}
