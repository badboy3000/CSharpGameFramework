﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomServer
{
    internal sealed partial class RoomServer
    {
        private void LoadData()
        {
            try {
                TableConfig.LevelProvider.Instance.LoadForServer();
                TableConfig.LevelMonsterProvider.Instance.LoadForServer();
                TableConfig.LevelMonsterProvider.Instance.BuildGroupedLevelMonsters();
                TableConfig.ActorProvider.Instance.LoadForServer();
                TableConfig.SkillProvider.Instance.LoadForServer();
                TableConfig.SkillDslProvider.Instance.LoadForServer();
                TableConfig.SkillResourcesProvider.Instance.LoadForServer();
                JoinSkillDslResource();
                TableConfig.FormationProvider.Instance.LoadForServer();
                BuildFormationInfo();
            } catch (Exception ex) {
                LogSys.Log(LOG_TYPE.ERROR, "LoadData Exception {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void JoinSkillDslResource()
        {
            foreach (var pair in TableConfig.SkillProvider.Instance.SkillMgr.GetData()) {
                TableConfig.Skill skill = pair.Value as TableConfig.Skill;
                TableConfig.SkillDsl skillDsl = TableConfig.SkillDslProvider.Instance.GetSkillDsl(skill.dslSkillId);
                skill.dslFile = skillDsl.dslFile;
                skill.damageData.Damage = skill.damage;
                skill.damageData.HpRecover = skill.addhp;
                skill.damageData.MpRecover = skill.addmp;
                skill.damageData.AddAttack = skill.addattack;
                skill.damageData.AddDefence = skill.adddefence;
                skill.damageData.AddRps = skill.addrps;
                skill.damageData.AddCritical = skill.addcritical;
                skill.damageData.AddCriticalPow = skill.addcriticalpow;
                skill.damageData.AddSpeed = skill.addspeed;
                skill.damageData.AddShield = skill.addshield;
            }
            var resources = TableConfig.SkillResourcesProvider.Instance.SkillResourcesMgr.GetData();
            foreach (var resource in resources) {
                int skillId = resource.skillId;
                string key = resource.key;
                string res = resource.resource;
                TableConfig.Skill skill = TableConfig.SkillProvider.Instance.GetSkill(skillId);
                if (null != skill) {
                    if (skill.resources.ContainsKey(key)) {
                        //repeat
                    } else {
                        skill.resources.Add(key, res);
                    }
                }
            }
        }

        private void BuildFormationInfo()
        {
            foreach (var pair in TableConfig.FormationProvider.Instance.FormationMgr.GetData()) {
                var record = pair.Value as TableConfig.Formation;
                if (null != record) {
                    record.BuildFormationInfo();
                }
            }
        }
    }
}
