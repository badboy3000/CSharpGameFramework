﻿using System;
using System.Collections.Generic;
using ScriptRuntime;
using System.Text;

namespace GameFramework
{
    public interface IAiStateLogic
    {
        void Execute(EntityInfo entity, long deltaTime);
    }
    public delegate void AiPursueDelegation(EntityInfo entity, ScriptRuntime.Vector3 target);
    public delegate void AiStopPursueDelegation(EntityInfo entity);
    public delegate void AiSelectSkill(EntityInfo entity, SkillInfo skill);
    public delegate void AiDeadDelegation(EntityInfo entity);
    public delegate void AiTargetDelegation(EntityInfo entity, EntityInfo target);
    public delegate void AiFaceDelegation(EntityInfo entity);
    public delegate void AiSkillDelegation(EntityInfo entity, int skillId);
    public delegate void AiStopSkillDelegation(EntityInfo entity);
    public delegate void AiAddImpactDelegation(EntityInfo user, int impactId);
    public delegate void AiRemoveImpactDelegation(EntityInfo user, int impactId);
    public delegate void AiMeetEnemy(EntityInfo entity);
    public delegate void AiInitDslLogic(EntityInfo entity);
    public delegate void AiSendStoryMessageDelegation(EntityInfo entity, string msgId, object[] args);
    public delegate void AiStateHandler(EntityInfo entity, long deltaTime);
    public abstract class AbstractAiStateLogic : IAiStateLogic
    {
        public static AiPursueDelegation OnAiPursue;
        public static AiStopPursueDelegation OnAiStopPursue;
        public static AiSelectSkill OnAiSelectSkill;
        public static AiDeadDelegation OnAiDead;
        public static AiTargetDelegation OnAiTarget;
        public static AiFaceDelegation OnAiFace;
        public static AiSkillDelegation OnAiSkill;
        public static AiStopSkillDelegation OnAiStopSkill;
        public static AiAddImpactDelegation OnAiAddImpact;
        public static AiRemoveImpactDelegation OnAiRemoveImpact;
        public static AiMeetEnemy OnAiMeetEnemy;

        public static AiInitDslLogic OnAiInitDslLogic;
        public static AiSendStoryMessageDelegation OnAiSendStoryMessage;
        public AbstractAiStateLogic()
        {
            OnInitStateHandlers();
        }
        public void Execute(EntityInfo entity, long deltaTime)
        {
            if (entity.IsUnderControl()) {
                return;
            }
            if (entity.GetAIEnable()) {
                AiStateInfo npcAi = entity.GetAiStateInfo();
                if (!npcAi.IsInited) {
                    OnStateLogicInit(entity, deltaTime);
                    npcAi.IsInited = true;
                }
                int curState = npcAi.CurState;
                if (curState > (int)AiStateId.Invalid && curState < (int)AiStateId.MaxNum) {
                    AiStateHandler handler;
                    if (m_Handlers.TryGetValue(curState, out handler)) {
                        if (OnStateLogicCheck(entity, deltaTime) && null != handler) {
                            handler(entity, deltaTime);
                        }
                    } else {
                        LogSystem.Error("Illegal ai state: " + curState + " entity:" + entity.GetId());
                    }
                } else {
                    ChangeToState(entity, (int)AiStateId.Idle);
                }
            }
        }
        public void ChangeToState(EntityInfo entity, int state)
        {
            entity.GetAiStateInfo().ChangeToState(state);
        }
        public void PushState(EntityInfo entity, int state)
        {
            entity.GetAiStateInfo().PushState(state);
        }
        public void PopState(EntityInfo entity)
        {
            entity.GetAiStateInfo().PopState();
        }
        public void NotifyAiPursue(EntityInfo entity, ScriptRuntime.Vector3 target)
        {
            if (null != OnAiPursue)
                OnAiPursue(entity, target);
        }
        public void NotifyAiStopPursue(EntityInfo entity)
        {
            if (null != OnAiStopPursue)
                OnAiStopPursue(entity);
        }
        public void NotifyAiSelectSkill(EntityInfo entity, SkillInfo skill)
        {
            if (null != OnAiSelectSkill)
                OnAiSelectSkill(entity, skill);
        }
        public void NotifyAiDead(EntityInfo entity)
        {
            if (null != OnAiDead)
                OnAiDead(entity);
        }
        public void NotifyAiTarget(EntityInfo entity, EntityInfo target)
        {
            if (null != OnAiTarget)
                OnAiTarget(entity, target);
        }
        public void NotifyAiFace(EntityInfo entity)
        {
            if (null != OnAiFace)
                OnAiFace(entity);
        }
        public void NotifyAiSkill(EntityInfo entity, int skillId)
        {
            if (null != OnAiSkill)
                OnAiSkill(entity, skillId);
        }
        public void NotifyAiStopSkill(EntityInfo entity)
        {
            if (null != OnAiStopSkill)
                OnAiStopSkill(entity);
        }
        public void NotifyAiAddImpact(EntityInfo entity, int impactId)
        {
            if (null != OnAiAddImpact)
                OnAiAddImpact(entity, impactId);
        }
        public void NotifyAiRemoveImpact(EntityInfo entity, int impactId)
        {
            if (null != OnAiRemoveImpact)
                OnAiRemoveImpact(entity, impactId);
        }
        public void NotifyAiMeetEnemy(EntityInfo entity)
        {
            if (null != OnAiMeetEnemy) {
                OnAiMeetEnemy(entity);
            }
            AiSendStoryMessage(entity, "obj_meet_enemy", entity.GetId());
            AiSendStoryMessage(entity, "npc_meet_enemy:" + entity.GetUnitId(), entity.GetId());
        }

        public void NotifyAiInitDslLogic(EntityInfo entity)
        {
            if (null != OnAiInitDslLogic) {
                OnAiInitDslLogic(entity);
            }
        }
        public void AiSendStoryMessage(EntityInfo entity, string msgId, params object[] args)
        {
            if (null != OnAiSendStoryMessage) {
                OnAiSendStoryMessage(entity, msgId, args);
            }
        }
        protected void SetStateHandler(int state, AiStateHandler handler)
        {
            if (state > (int)AiStateId.Invalid && state < (int)AiStateId.MaxNum) {
                if (null != handler) {
                    if (m_Handlers.ContainsKey(state))
                        m_Handlers[state] = handler;
                    else
                        m_Handlers.Add(state, handler);
                } else {
                    m_Handlers.Remove(state);
                }
            }
        }
        protected abstract void OnInitStateHandlers();
        protected virtual void OnStateLogicInit(EntityInfo entity, long deltaTime)
        { }
        protected virtual bool OnStateLogicCheck(EntityInfo entity, long deltaTime)
        {
            return true;
        }

        private Dictionary<int, AiStateHandler> m_Handlers = new Dictionary<int, AiStateHandler>();
    }
}
