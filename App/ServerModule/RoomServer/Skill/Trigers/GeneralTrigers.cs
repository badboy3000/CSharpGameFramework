﻿using System;
using System.Collections.Generic;
using ScriptRuntime;
using SkillSystem;
using GameFramework;
using GameFramework.Story;

namespace GameFramework.Skill.Trigers
{
    /// <summary>
    /// bornfinish(start_time);
    /// </summary>
    internal class BornFinishTriger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            BornFinishTriger triger = new BornFinishTriger();
            
                        return triger;
        }

        public override void Reset()
        {
            
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (null == obj) return false;
            if (curSectionTime >= StartTime) {
                scene.EntityController.BornFinish(senderObj.ActorId);
                return false;
            } else {
                return true;
            }
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
    /// <summary>
    /// deadfinish(start_time);
    /// </summary>
    internal class DeadFinishTriger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            DeadFinishTriger triger = new DeadFinishTriger();
            
                        return triger;
        }

        public override void Reset()
        {
            
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (null == obj) return false;
            if (curSectionTime >= StartTime) {
                scene.EntityController.DeadFinish(senderObj.ActorId);
                return false;
            } else {
                return true;
            }
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
    /// <summary>
    /// sendstorymessage(start_time,msg,arg1,arg2,arg3,...);
    /// </summary>
    public class SendStoryMessageTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            SendStoryMessageTrigger copy = new SendStoryMessageTrigger();
            
            copy.m_Msg = m_Msg;
            copy.m_Args.AddRange(m_Args);
                        return copy;
        }

        public override void Reset()
        {
            
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (null == obj) return false;
            if (curSectionTime < StartTime) {
                return true;
            }
            List<object> args = new List<object>();
            args.Add(senderObj);
            for (int i = 0; i < m_Args.Count; ++i) {
                args.Add(m_Args[i]);
            }
            scene.StorySystem.SendMessage(m_Msg, args.ToArray());
            return false;
        }

        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                StartTime = int.Parse(callData.GetParamId(0));
                m_Msg = callData.GetParamId(1);
            }
            for (int i = 2; i < num; ++i) {
                m_Args.Add(callData.GetParamId(i));
            }
            
        }

        private string m_Msg = string.Empty;
        private List<string> m_Args = new List<string>();

        
    }
    /// <summary>
    /// params([startTime])
    /// {
    ///     int(name,value);
    ///     long(name,value);
    ///     float(name,value);
    ///     double(name,value);
    ///     string(name,value);
    ///     ...
    /// };
    /// </summary>
    internal class ParamsTriger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            ParamsTriger triger = new ParamsTriger();
            triger.m_Params = new Dictionary<string, object>(m_Params);
            return triger;
        }
        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (null == obj) return false;
            if (curSectionTime < StartTime)
                return true;
            foreach (var pair in m_Params) {
                instance.SetVariable(pair.Key, pair.Value);
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            m_Params = new Dictionary<string, object>();
            int num = callData.GetParamNum();
            if (num > 0) {
                StartTime = long.Parse(callData.GetParamId(0));
            } else {
                StartTime = 0;
            }
        }

        protected override void Load(Dsl.FunctionData funcData, SkillInstance instance)
        {
            Dsl.CallData callData = funcData.Call;
            if (null != callData) {
                Load(callData, instance);

                for (int i = 0; i < funcData.Statements.Count; ++i) {
                    Dsl.ISyntaxComponent statement = funcData.Statements[i];
                    Dsl.CallData stCall = statement as Dsl.CallData;
                    if (null != stCall) {
                        string id = stCall.GetId();
                        string key = stCall.GetParamId(0);
                        object val = string.Empty;
                        if (id == "int") {
                            val = int.Parse(stCall.GetParamId(1));
                        } else if (id == "long") {
                            val = long.Parse(stCall.GetParamId(1));
                        } else if (id == "float") {
                            val = float.Parse(stCall.GetParamId(1));
                        } else if (id == "double") {
                            val = double.Parse(stCall.GetParamId(1));
                        } else if (id == "string") {
                            val = stCall.GetParamId(1);
                        }
                        m_Params.Add(key, val);
                    }
                }
            }
        }

        private Dictionary<string, object> m_Params = null;
    }
    /// <summary>
    /// keeptarget([starttime[,remaintime]]);
    /// </summary>
    public class KeepTargetTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            KeepTargetTrigger copy = new KeepTargetTrigger();
            copy.m_RemainTime = m_RemainTime;
            return copy;
        }
        public override void Reset()
        {
        }
        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num >= 1) {
                StartTime = long.Parse(callData.GetParamId(0));
            }
            if (num >= 2) {
                m_RemainTime = long.Parse(callData.GetParamId(1));
            }
        }
        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            Scene scene = senderObj.Scene;
            EntityInfo obj = senderObj.GfxObj;
            if (curSectionTime < StartTime) {
                return true;
            }
            if (m_RemainTime > 0 && curSectionTime > (StartTime + m_RemainTime)) {
                return false;
            }
            if (senderObj.ConfigData.type == (int)SkillOrImpactType.Skill) {
                scene.EntityController.KeepTarget(senderObj.TargetActorId);
            } else {
                scene.EntityController.KeepTarget(senderObj.ActorId);
            }
            return true;
        }

        private long m_RemainTime = 0;
    }
    /// <summary>
    /// useimpact(impactid,[starttime[,is_external_impact]])[if(type)];
    /// </summary>
    public class UseImpactTrigger : AbstractSkillTriger
    {
        protected override ISkillTriger OnClone()
        {
            UseImpactTrigger copy = new UseImpactTrigger();
            copy.m_Impact = m_Impact;
            copy.m_IsExternalImpact = m_IsExternalImpact;
            copy.m_Type = m_Type;
            return copy;
        }
        public override void Reset()
        {
        }
        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GfxSkillSenderInfo senderObj = sender as GfxSkillSenderInfo;
            if (null == senderObj) return false;
            if (curSectionTime < StartTime) {
                return true;
            }
            bool needSetImpact = false;
            if (string.IsNullOrEmpty(m_Type)) {
                needSetImpact = true;
            } else {
                if (m_Type == "block" && instance.Variables.ContainsKey("impact_block")) {
                    needSetImpact = true;
                }
            }
            if (needSetImpact) {
                int impact = m_Impact;
                if (!m_IsExternalImpact) {
                    impact = SkillInstance.GenInnerHitSkillId(m_Impact <= 0 ? 1 : m_Impact);
                }
                instance.SetVariable("impact", impact);
            }
            return false;
        }
        protected override void Load(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Impact = int.Parse(callData.GetParamId(0));
            }
            if (num > 1) {
                StartTime = long.Parse(callData.GetParamId(1));
            }
            if (num > 2) {
                m_IsExternalImpact = callData.GetParamId(2) == "true";
            }
        }
        protected override void Load(Dsl.StatementData statementData, SkillInstance instance)
        {
            Dsl.FunctionData func1 = statementData.First;
            Dsl.FunctionData func2 = statementData.Second;
            if (null != func1 && null != func2) {
                Load(func1.Call, instance);
                LoadIf(func2.Call, instance);
            }
        }
        private void LoadIf(Dsl.CallData callData, SkillInstance instance)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Type = callData.GetParamId(0);
            }
        }

        private int m_Impact = 0;
        private bool m_IsExternalImpact = false;
        private string m_Type = string.Empty;
    }
}
