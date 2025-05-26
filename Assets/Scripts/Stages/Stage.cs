using System;
using UnityEngine;

namespace Stages
{
    [CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Stage")]
    public class Stage : ScriptableObject
    {
        public Wave[] waves = Array.Empty<Wave>();
        public float totalTime;
    }
}