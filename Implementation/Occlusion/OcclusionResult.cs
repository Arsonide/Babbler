using System.Runtime.CompilerServices;
using UnityEngine;

namespace Babbler.Implementation.Occlusion;

public struct OcclusionResult
{
    public OcclusionState State;
    public bool AlternativePosition;
    public Vector3 Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OcclusionResult CreateFullOcclusion()
    {
        return new OcclusionResult
        {
            State = OcclusionState.FullOcclusion, AlternativePosition = false, Position = Vector3.zero,
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OcclusionResult CreateNoOcclusion()
    {
        return new OcclusionResult
        {
            State = OcclusionState.NoOcclusion, AlternativePosition = false, Position = Vector3.zero,
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OcclusionResult CreateDistantOcclusion()
    {
        return new OcclusionResult
        {
            State = OcclusionState.DistantOcclusion, AlternativePosition = false, Position = Vector3.zero,
        };
    }
}