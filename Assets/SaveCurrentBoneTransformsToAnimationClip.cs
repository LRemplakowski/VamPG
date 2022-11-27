using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

namespace SunsetSystems
{
    public class SaveCurrentBoneTransformsToAnimationClip : MonoBehaviour
    {
        [SerializeField]
        private Transform _root;

        [Button]
        public void DoSave()
        {
            Transform[] bones = _root.GetComponentsInChildren<Transform>();
            AnimationClip clip = new();
            foreach (Transform bone in bones)
            {
                Keyframe[] positionXKeys = new Keyframe[2];
                Keyframe[] positionYKeys = new Keyframe[2];
                Keyframe[] positionZKeys = new Keyframe[2];
                Keyframe[] rotationXKeys = new Keyframe[2];
                Keyframe[] rotationYKeys = new Keyframe[2];
                Keyframe[] rotationZKeys = new Keyframe[2];
                Keyframe[] scaleXKeys = new Keyframe[2];
                Keyframe[] scaleYKeys = new Keyframe[2];
                Keyframe[] scaleZKeys = new Keyframe[2];

                positionXKeys[0] = new Keyframe(0f, bone.localPosition.x);
                positionXKeys[1] = new Keyframe(1f, bone.localPosition.x);

                positionYKeys[0] = new Keyframe(0f, bone.localPosition.y);
                positionYKeys[1] = new Keyframe(1f, bone.localPosition.y);

                positionZKeys[0] = new Keyframe(0f, bone.localPosition.z);
                positionZKeys[1] = new Keyframe(1f, bone.localPosition.z);

                rotationXKeys[0] = new Keyframe(0f, bone.localEulerAngles.x);
                rotationXKeys[1] = new Keyframe(1f, bone.localEulerAngles.x);

                rotationYKeys[0] = new Keyframe(0f, bone.localEulerAngles.y);
                rotationYKeys[1] = new Keyframe(1f, bone.localEulerAngles.y);

                rotationZKeys[0] = new Keyframe(0f, bone.localEulerAngles.z);
                rotationZKeys[1] = new Keyframe(1f, bone.localEulerAngles.z);

                scaleXKeys[0] = new Keyframe(0f, bone.localScale.x);
                scaleXKeys[1] = new Keyframe(1f, bone.localScale.x);

                scaleYKeys[0] = new Keyframe(0f, bone.localScale.y);
                scaleYKeys[1] = new Keyframe(1f, bone.localScale.y);

                scaleZKeys[0] = new Keyframe(0f, bone.localScale.z);
                scaleZKeys[1] = new Keyframe(1f, bone.localScale.z);

                AnimationCurve posXCurve = new(positionXKeys);
                AnimationCurve posYCurve = new(positionYKeys);
                AnimationCurve posZCurve = new(positionZKeys);
                AnimationCurve rotXCurve = new(rotationXKeys);
                AnimationCurve rotYCurve = new(rotationYKeys);
                AnimationCurve rotZCurve = new(rotationZKeys);
                AnimationCurve scaleXCurve = new(scaleXKeys);
                AnimationCurve scaleYCurve = new(scaleYKeys);
                AnimationCurve scaleZCurve = new(scaleZKeys);

                string relativeBonePath = AnimationUtility.CalculateTransformPath(bone, _root);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localPosition.x", posXCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localPosition.y", posYCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localPosition.z", posZCurve);

                clip.SetCurve(relativeBonePath, typeof(Transform), "localEulerAngles.x", rotXCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localEulerAngles.y", rotYCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localEulerAngles.z", rotZCurve);

                clip.SetCurve(relativeBonePath, typeof(Transform), "localScale.x", scaleXCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localScale.y", scaleYCurve);
                clip.SetCurve(relativeBonePath, typeof(Transform), "localScale.z", scaleZCurve);
            }
            AssetDatabase.CreateAsset(clip, "Assets/generated clip.anim");
        }
    }
}