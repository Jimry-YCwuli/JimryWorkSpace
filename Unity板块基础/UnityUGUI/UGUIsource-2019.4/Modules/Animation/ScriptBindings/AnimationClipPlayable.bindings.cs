// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
    [NativeHeader("Modules/Animation/ScriptBindings/AnimationClipPlayable.bindings.h")]
    [NativeHeader("Modules/Animation/Director/AnimationClipPlayable.h")]
    [StaticAccessor("AnimationClipPlayableBindings", StaticAccessorType.DoubleColon)]
    [RequiredByNativeCode]
    public struct AnimationClipPlayable : IPlayable, IEquatable<AnimationClipPlayable>
    {
        PlayableHandle m_Handle;

        public static AnimationClipPlayable Create(PlayableGraph graph, AnimationClip clip)
        {
            var handle = CreateHandle(graph, clip);
            return new AnimationClipPlayable(handle);
        }

        private static PlayableHandle CreateHandle(PlayableGraph graph, AnimationClip clip)
        {
            PlayableHandle handle = PlayableHandle.Null;
            if (!CreateHandleInternal(graph, clip, ref handle))
                return PlayableHandle.Null;

            return handle;
        }

        internal AnimationClipPlayable(PlayableHandle handle)
        {
            if (handle.IsValid())
            {
                if (!handle.IsPlayableOfType<AnimationClipPlayable>())
                    throw new InvalidCastException("Can't set handle: the playable is not an AnimationClipPlayable.");
            }

            m_Handle = handle;
        }

        public PlayableHandle GetHandle()
        {
            return m_Handle;
        }

        public static implicit operator Playable(AnimationClipPlayable playable)
        {
            return new Playable(playable.GetHandle());
        }

        public static explicit operator AnimationClipPlayable(Playable playable)
        {
            return new AnimationClipPlayable(playable.GetHandle());
        }

        public bool Equals(AnimationClipPlayable other)
        {
            return GetHandle() == other.GetHandle();
        }

        public AnimationClip GetAnimationClip()
        {
            return GetAnimationClipInternal(ref m_Handle);
        }

        public bool GetApplyFootIK()
        {
            return GetApplyFootIKInternal(ref m_Handle);
        }

        public void SetApplyFootIK(bool value)
        {
            SetApplyFootIKInternal(ref m_Handle, value);
        }

        public bool GetApplyPlayableIK()
        {
            return GetApplyPlayableIKInternal(ref m_Handle);
        }

        public void SetApplyPlayableIK(bool value)
        {
            SetApplyPlayableIKInternal(ref m_Handle, value);
        }

        internal bool GetRemoveStartOffset()
        {
            return GetRemoveStartOffsetInternal(ref m_Handle);
        }

        internal void SetRemoveStartOffset(bool value)
        {
            SetRemoveStartOffsetInternal(ref m_Handle, value);
        }

        internal bool GetOverrideLoopTime()
        {
            return GetOverrideLoopTimeInternal(ref m_Handle);
        }

        internal void SetOverrideLoopTime(bool value)
        {
            SetOverrideLoopTimeInternal(ref m_Handle, value);
        }

        internal bool GetLoopTime()
        {
            return GetLoopTimeInternal(ref m_Handle);
        }

        internal void SetLoopTime(bool value)
        {
            SetLoopTimeInternal(ref m_Handle, value);
        }

        internal float GetSampleRate()
        {
            return GetSampleRateInternal(ref m_Handle);
        }

        internal void SetSampleRate(float value)
        {
            SetSampleRateInternal(ref m_Handle, value);
        }

        [NativeThrows]
        extern private static bool CreateHandleInternal(PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);

        [NativeThrows]
        extern private static AnimationClip GetAnimationClipInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static bool GetApplyFootIKInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetApplyFootIKInternal(ref PlayableHandle handle, bool value);

        [NativeThrows]
        extern private static bool GetApplyPlayableIKInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetApplyPlayableIKInternal(ref PlayableHandle handle, bool value);

        [NativeThrows]
        extern private static bool GetRemoveStartOffsetInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetRemoveStartOffsetInternal(ref PlayableHandle handle, bool value);

        [NativeThrows]
        extern private static bool GetOverrideLoopTimeInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetOverrideLoopTimeInternal(ref PlayableHandle handle, bool value);

        [NativeThrows]
        extern private static bool GetLoopTimeInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetLoopTimeInternal(ref PlayableHandle handle, bool value);

        [NativeThrows]
        extern private static float GetSampleRateInternal(ref PlayableHandle handle);

        [NativeThrows]
        extern private static void SetSampleRateInternal(ref PlayableHandle handle, float value);
    }
}
