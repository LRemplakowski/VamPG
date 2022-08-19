using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Entities.Characters;
using SunsetSystems.Game;

public class DevSpellbook : ExposableMonobehaviour
{
    public CharacterStats devStatsAsset;
    public TMP_Dropdown dropdown;
    List<DisciplinePower> powers = new();

    private void Start()
    {
        dropdown.ClearOptions();
        List<DisciplinePower> bsPowers = devStatsAsset.GetDiscipline(DisciplineType.BloodSorcery).GetKnownPowers();
        List<DisciplinePower> fPowers = devStatsAsset.GetDiscipline(DisciplineType.Fortitude).GetKnownPowers();
        List<TMP_Dropdown.OptionData> options = new();
        bsPowers.ForEach(dp =>
        {
            options.Add(new TMP_Dropdown.OptionData(dp.name));
            powers.Add(dp);
        });
        fPowers.ForEach(dp =>
        {
            options.Add(new TMP_Dropdown.OptionData(dp.name));
            powers.Add(dp);
        });
        dropdown.options = options;
    }

    public void Use()
    {
        DisciplinePower dp = powers[dropdown.value];
        bool cast = false;
        if (dp.Target.Equals(Target.Self))
        {
            cast = Spellbook.HandleEffects(dp, GameManager.GetMainCharacter(), GameManager.GetMainCharacter());
        }
        if (dp.Target.Equals(Target.Hostile))
        {
            cast = Spellbook.HandleEffects(dp, GameManager.GetMainCharacter(), FindObjectOfType<NPC>());
        }
        Debug.Log("Did cast? " + cast);
    }
}
