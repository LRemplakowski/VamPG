using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Party
{
    public interface IPartyManager
    {
        List<Creature> ActiveParty { get; }
        Creature MainCharacter { get; }
        List<Creature> Companions { get; }
        List<CreatureData> AllCoterieMembers { get; }

        Creature GetPartyMemberByID(string key);
        CreatureData GetPartyMemberDataByID(string key);
        bool IsRecruitedMember(string key);
        void RecruitCharacter(CreatureData creatureData);
        void RecruitMainCharacter(CreatureData mainCharacterData);
        void AddCreatureAsActivePartyMember(Creature creature);
        void InitializePartyAtPosition(Vector3 position);
    }
}
