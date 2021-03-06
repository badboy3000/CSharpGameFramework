using System;
using System.Collections.Generic;
using ScriptRuntime;
using SkillSystem;

namespace GameFramework.Skill.Trigers
{
    public class TargetManager
    {
        public HashSet<int> Targets
        {
            get { return m_Targets; }
        }
        public void Add(int targetId)
        {
            Targets.Add(targetId);
        }
        public bool Exist(int targetId)
        {
            return Targets.Contains(targetId);
        }
        public void Clear()
        {
            Targets.Clear();
        }

        private HashSet<int> m_Targets = new HashSet<int>();
    }
    /// <summary>
    /// selecttarget(type[, start_time]);
    /// </summary>
    public class SelectTargetTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            SelectTargetTrigger copy = new SelectTargetTrigger();
            copy.m_Type = m_Type;
            
                        return copy;
        }

        public override void Reset()
        {
            
        }

        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Type = callData.GetParamId(0);
            }
            if (num > 1) {
                StartTime = long.Parse(callData.GetParamId(1));
            } else {
                StartTime = 0;
            }
            
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (obj == null) {
                return false;
            }
            if (curSectionTime < StartTime) {
                return true;
            }
            Vector3 pos = obj.GetMovementStateInfo().GetPosition3D();
            TargetManager mgr = instance.CustomDatas.GetData<TargetManager>();
            if (null == mgr) {
                mgr = new TargetManager();
                instance.CustomDatas.AddData(mgr);
            }
            int targetId = scene.EntityController.SelectTargetForSkill(m_Type, senderObj.ActorId, senderObj.ConfigData, senderObj.Seq, mgr.Targets);
            if (targetId > 0) {
                mgr.Add(targetId);
                EntityInfo target = scene.EntityController.GetGameObject(targetId);
                senderObj.TargetActorId = targetId;
                senderObj.TargetGfxObj = target;
            }
            return false;
        }

        private string m_Type = "minhp";
        
    }
    /// <summary>
    /// facetotarget(starttime,remaintime[,rotate,selecttype]);
    /// </summary>
    public class FaceToTargetTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            FaceToTargetTrigger copy = new FaceToTargetTrigger();
            
            copy.m_RemainTime = m_RemainTime;
            copy.m_IsHaveRotateSpeed = m_IsHaveRotateSpeed;
            copy.m_RotateSpeed = m_RotateSpeed;
            copy.m_SelectTargetType = m_SelectTargetType;
                        copy.m_RealSelectTargetType = m_RealSelectTargetType;
            return copy;
        }

        public override void Reset()
        {
            m_IsExecuted = false;
            
            m_RealSelectTargetType = m_SelectTargetType;
        }

        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                StartTime = long.Parse(callData.GetParamId(0));
                m_RemainTime = long.Parse(callData.GetParamId(1));
            }
            if (num >= 3) {
                m_IsHaveRotateSpeed = true;
                m_RotateSpeed = Vector3.Zero;
                m_RotateSpeed.Y = (float)(float.Parse(callData.GetParamId(2)) * Math.PI / 180.0f);
            }
            if (num >= 4) {
                m_SelectTargetType = callData.GetParamId(3);
            }
            
            m_RealSelectTargetType = m_SelectTargetType;
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (null == obj) {
                return false;
            }
            if (curSectionTime < StartTime) {
                return true;
            }
            if (m_IsExecuted && curSectionTime > (StartTime + m_RemainTime)) {
                return false;
            }

            Vector3 pos = obj.GetMovementStateInfo().GetPosition3D();
            EntityInfo target = senderObj.TargetGfxObj;
            if (!m_IsExecuted && (null == target || !string.IsNullOrEmpty(m_SelectTargetType))) {
                if (string.IsNullOrEmpty(m_SelectTargetType)) {
                    int targetType = scene.EntityController.GetTargetType(senderObj.ActorId, senderObj.ConfigData, senderObj.Seq);
                    if (targetType == (int)SkillTargetType.Friend || targetType == (int)SkillTargetType.RandFriend)
                        m_RealSelectTargetType = "randfriend";
                    else
                        m_RealSelectTargetType = "randenemy";
                }
                TargetManager mgr = instance.CustomDatas.GetData<TargetManager>();
                if (null == mgr) {
                    mgr = new TargetManager();
                    instance.CustomDatas.AddData(mgr);
                }
                int targetId = scene.EntityController.SelectTargetForSkill(m_RealSelectTargetType, senderObj.ActorId, senderObj.ConfigData, senderObj.Seq, mgr.Targets);
                if (targetId > 0) {
                    mgr.Add(targetId);
                    target = scene.EntityController.GetGameObject(targetId);
                    senderObj.TargetActorId = targetId;
                    senderObj.TargetGfxObj = target;
                }
            }
            if (null != target) {
                m_IsExecuted = true;
                if (!m_IsHaveRotateSpeed || m_RotateSpeed.Y == 0) {
                    TriggerUtil.Lookat(scene, obj, target.GetMovementStateInfo().GetPosition3D());
                } else {
                    float maxRotateDelta = m_RotateSpeed.Y * TriggerUtil.ConvertToSecond(delta);
                    TriggerUtil.Lookat(scene, obj, target.GetMovementStateInfo().GetPosition3D(), maxRotateDelta);
                }
            }
            return true;
        }

        private long m_RemainTime;
        private bool m_IsHaveRotateSpeed = false;
        private Vector3 m_RotateSpeed;
        private string m_SelectTargetType;
        private bool m_IsExecuted = false;

        
        private string m_RealSelectTargetType = string.Empty;
    }
    /// <summary>
    /// cleartargets(starttime);
    /// </summary>
    public class ClearTargetsTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            ClearTargetsTrigger copy = new ClearTargetsTrigger();
            
                        return copy;
        }

        public override void Reset()
        {
            
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            EntityInfo obj = senderObj.GfxObj;
            if (obj == null) {
                return false;
            }
            if (curSectionTime < StartTime) {
                return true;
            }
            TargetManager mgr = instance.CustomDatas.GetData<TargetManager>();
            if (null != mgr) {
                mgr.Clear();
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                StartTime = long.Parse(callData.GetParamId(0));
            } else {
                StartTime = 0;
            }
            
        }

        
    }
}
