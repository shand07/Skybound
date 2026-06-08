using System.Collections.Generic;
using Skybound.Core.Diagnostics;
using Skybound.Data;
using UnityEngine;

namespace Skybound.Characters
{
    public class TalentController : MonoBehaviour
    {
        [Header("Talent Trees")]
        [SerializeField] private TalentTreeData[] availableTalentTrees;

        [Header("Runtime Progression")]
        [SerializeField] private int availableTalentPoints;

        private readonly Dictionary<TalentData, int> talentRanks = new();

        public int AvailableTalentPoints => availableTalentPoints;
        public TalentTreeData[] AvailableTalentTrees => availableTalentTrees;

        private void Awake()
        {
            ValidateTalentTrees();
            InitializeTalentRanks();
        }

        public int GetTalentRank(TalentData talent)
        {
            if (talent == null)
                return 0;

            return talentRanks.TryGetValue(talent, out int rank)
                ? rank
                : 0;
        }

        public bool CanUnlockTalent(TalentData talent)
        {
            if (talent == null)
                return false;

            if (!talent.IsValid(out string errorMessage))
            {
                SkyboundDebug.Warning(
                    $"{name} cannot unlock invalid talent '{talent.name}': {errorMessage}",
                    this
                );

                return false;
            }

            if (availableTalentPoints <= 0)
                return false;

            int currentRank = GetTalentRank(talent);

            if (currentRank >= talent.MaxRank)
                return false;

            if (talent.RequiredCharacterLevel > 1)
            {
                SkyboundDebug.Warning(
                    $"{name} cannot unlock '{talent.TalentName}' yet. Character level checks are not implemented.",
                    this
                );

                return false;
            }

            foreach (TalentData prerequisite in talent.PrerequisiteTalents)
            {
                if (prerequisite == null)
                    return false;

                if (GetTalentRank(prerequisite) <= 0)
                    return false;
            }

            return true;
        }

        public bool TryUnlockTalent(TalentData talent)
        {
            if (!CanUnlockTalent(talent))
            {
                SkyboundDebug.Warning(
                    $"{name} failed to unlock talent '{talent?.TalentName}'.",
                    this
                );

                return false;
            }

            int currentRank = GetTalentRank(talent);

            talentRanks[talent] = currentRank + 1;
            availableTalentPoints--;

            SkyboundDebug.Log(
                $"{name} unlocked/upgraded talent '{talent.TalentName}' to rank {talentRanks[talent]}. Talent points left: {availableTalentPoints}",
                this
            );

            return true;
        }

        public void AddTalentPoints(int amount)
        {
            if (amount <= 0)
            {
                SkyboundDebug.Warning(
                    $"{name} tried to add invalid talent point amount: {amount}",
                    this
                );

                return;
            }

            availableTalentPoints += amount;

            SkyboundDebug.Log(
                $"{name} gained {amount} talent point(s). Total: {availableTalentPoints}",
                this
            );
        }

        private void ValidateTalentTrees()
        {
            if (availableTalentTrees == null || availableTalentTrees.Length == 0)
            {
                SkyboundDebug.Warning(
                    $"{name} has no available talent trees assigned.",
                    this
                );

                return;
            }

            for (int i = 0; i < availableTalentTrees.Length; i++)
            {
                TalentTreeData tree = availableTalentTrees[i];

                if (tree == null)
                {
                    SkyboundDebug.Warning(
                        $"{name} has null talent tree at index {i}.",
                        this
                    );

                    continue;
                }

                if (!tree.IsValid(out string errorMessage))
                {
                    SkyboundDebug.Warning(
                        $"{name} has invalid talent tree '{tree.name}': {errorMessage}",
                        this
                    );
                }
            }
        }

        private void InitializeTalentRanks()
        {
            talentRanks.Clear();

            if (availableTalentTrees == null)
                return;

            foreach (TalentTreeData tree in availableTalentTrees)
            {
                if (tree == null || tree.Talents == null)
                    continue;

                foreach (TalentData talent in tree.Talents)
                {
                    if (talent == null)
                        continue;

                    if (!talentRanks.ContainsKey(talent))
                        talentRanks.Add(talent, 0);
                }
            }

            SkyboundDebug.Log(
                $"{name} initialized {talentRanks.Count} talent(s).",
                this
            );
        }
    }
}