﻿using System;
using System.Collections.Generic;
using System.Text;
using ScriptRuntime;
using StorySystem;

namespace GameFramework
{
    public enum AiStateLogicId : int
    {
        Invalid = 0,
        Entity_DslLogic = 1,
        Entity_General = 2,
        Entity_Leader = 3,
        Entity_Member = 4,

        MaxNum
    }
    public enum AiStateId : int
    {
        Invalid = 0,
        Idle,
        Combat,
        Escape,
        GoHome,
        Patrol,
        DslLogic,
        SkillCommand,
        MoveCommand,
        PursuitCommand,
        PatrolCommand,
        WaitCommand,
        MaxNum
    }
    public class AiStoryInstanceInfo
    {
        public StoryInstance m_StoryInstance;
        public bool m_IsUsed;

        public void Recycle()
        {
            m_StoryInstance.Reset();
            m_IsUsed = false;
        }
    }
    public class AiStateInfo
    {
        public int CurState
        {
            get
            {
                int state = (int)AiStateId.Invalid;
                if (m_StateStack.Count > 0)
                    state = m_StateStack.Peek();
                return state;
            }
        }
        public void PushState(int state)
        {
            m_StateStack.Push(state);
        }
        public int PopState()
        {
            int ret = (int)AiStateId.Invalid;
            if (m_StateStack.Count > 0)
                ret = m_StateStack.Pop();
            return ret;
        }
        public void ChangeToState(int state)
        {
            if (m_StateStack.Count > 0)
                m_StateStack.Pop();
            m_StateStack.Push(state);
        }
        public void CloneAiStates(IEnumerable<int> states)
        {
            m_StateStack = new Stack<int>(states);
        }
        public int[] CloneAiStates()
        {
            return m_StateStack.ToArray();
        }
        public void Reset()
        {
            m_StateStack.Clear();
            m_AiDatas.Clear();
            if (null != m_AiStoryInstanceInfo) {
                m_AiStoryInstanceInfo.Recycle();
                m_AiStoryInstanceInfo = null;
            }
            m_IsInited = false;

            m_AiLogic = 0;
            m_AiParam = new string[c_MaxAiParamNum];
            m_AiStoryInstanceInfo = null;
            m_Time = 0;
            m_IsInited = false;
            m_leaderID = 0;
            m_HomePos = Vector3.Zero;
            m_Target = 0;
            m_HateTarget = 0;
            m_IsExternalTarget = false;
            m_LastChangeTargetTime = 0;
        }
        public int AiLogic
        {
            get { return m_AiLogic; }
            set { m_AiLogic = value; }
        }
        public string[] AiParam
        {
            get { return m_AiParam; }
        }
        public TypedDataCollection AiDatas
        {
            get { return m_AiDatas; }
        }
        public AiStoryInstanceInfo AiStoryInstanceInfo
        {
            get { return m_AiStoryInstanceInfo; }
            set { m_AiStoryInstanceInfo = value; }
        }
        public bool IsInited
        {
            get { return m_IsInited; }
            set { m_IsInited = value; }
        }
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public int LeaderID
        {
            get { return m_leaderID; }
            set { m_leaderID = value; }
        }
        public ScriptRuntime.Vector3 HomePos
        {
            get { return m_HomePos; }
            set { m_HomePos = value; }
        }
        public int Target
        {
            get { return m_Target; }
            set
            {
                m_Target = value;
                m_IsExternalTarget = false;
            }
        }
        public int HateTarget
        {
            get { return m_HateTarget; }
            set { m_HateTarget = value; }
        }
        public bool IsExternalTarget
        {
            get { return m_IsExternalTarget; }
        }
        public long LastChangeTargetTime
        {
            get { return m_LastChangeTargetTime; }
            set { m_LastChangeTargetTime = value; }
        }
        public void SetExternalTarget(int target)
        {
            m_Target = target;
            m_IsExternalTarget = true;
        }
        
        private Stack<int> m_StateStack = new Stack<int>();
        private int m_AiLogic = 0;
        private string[] m_AiParam = new string[c_MaxAiParamNum];
        private TypedDataCollection m_AiDatas = new TypedDataCollection();
        private AiStoryInstanceInfo m_AiStoryInstanceInfo = null;
        private long m_Time = 0;
        private bool m_IsInited = false;
        private int m_leaderID = 0;
        private ScriptRuntime.Vector3 m_HomePos = Vector3.Zero;
        private int m_Target = 0;
        private int m_HateTarget = 0;
        private bool m_IsExternalTarget = false;
        private long m_LastChangeTargetTime = 0;

        public const int c_MaxAiParamNum = 8;
    }
}
