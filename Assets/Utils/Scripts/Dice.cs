using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Dice
{
    public abstract class DicePool<T, U> where T : BaseStat where U : BaseStat
    {
        public T First { get; protected set; }
        public U Second { get; protected set; }

        public int GetPoolSize()
        {
            return First.GetValue() + Second.GetValue();
        }

        public int GetPoolSize(bool includeModifiers)
        {
            return First.GetValue(includeModifiers) + Second.GetValue(includeModifiers);
        }

        public int GetPoolSize(List<ModifierType> modifierTypes)
        {
            return First.GetValue(modifierTypes) + Second.GetValue(modifierTypes);
        }
    }

    public sealed class TwoAttributePool : DicePool<Attribute, Attribute>
    {
        public TwoAttributePool(Attribute first, Attribute second)
        {
            First = first;
            Second = second;
        }
    }

    public sealed class AttributeSkillPool : DicePool<Attribute, Skill>
    {
        public AttributeSkillPool(Attribute attribute, Skill skill)
        {
            First = attribute;
            Second = skill;
        }
    }

    public sealed class TwoSkillPool : DicePool<Skill, Skill>
    {
        public TwoSkillPool(Skill first, Skill second)
        {
            First = first;
            Second = second;
        }
    }

    public sealed class DisciplineAttributePool : DicePool<Discipline, Attribute>
    {
        public DisciplineAttributePool(Discipline discipline, Attribute attribute)
        {
            First = discipline;
            Second = attribute;
        }
    }

    public sealed class DisciplineSkillPool : DicePool<Discipline, Skill>
    {
        public DisciplineSkillPool(Discipline discipline, Skill skill)
        {
            First = discipline;
            Second = skill;
        }
    }

    public class Outcome
    {
        public readonly int successes;
        public readonly bool isCritical, isMessy, isBestialFailure;
        public readonly List<int> normalDice = new List<int>(), hungerDice = new List<int>();

        public Outcome(int successes, bool isCritical, bool isMessy, bool isBestialFailure, List<int> normalDice, List<int> hungerDice)
        {
            this.successes = successes;
            this.isCritical = isCritical;
            this.isMessy = isMessy;
            this.isBestialFailure = isBestialFailure;
            this.normalDice = normalDice;
            this.hungerDice = hungerDice;
        }

        public Outcome(int successes, bool isCritical, List<int> normalDice)
        {
            this.successes = successes;
            this.isCritical = isCritical;
            isMessy = false;
            isBestialFailure = false;
            this.normalDice = normalDice;
        }
    }

    public static class Roll
    {
        private static System.Random r = new System.Random();
        public static Outcome d10(int normalDice, int hungerDice)
        {
            System.Random r = new System.Random((int) DateTime.Now.ToFileTime());
            int[] normals = new int[normalDice];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = r.Next(1, 11);
            }
            int[] hunger = new int[hungerDice];
            for (int i = 0; i < hunger.Length; i++)
            {
                hunger[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(normals, n => n >= 6).Length + Array.FindAll(hunger, h => h >= 6).Length;
            int tens = Array.FindAll(normals, n => n == 10).Length;
            int hungerTens = Array.FindAll(hunger, h => h == 10).Length;
            bool isCritical = false;
            if (tens + hungerTens >= 2)
            {
                successes += (tens + hungerTens) / 2;
                isCritical = true;
            }
            bool isMessy = isCritical && hungerTens > 0;
            Debug.Log("d10:"
                + "\nnormal dice: " + DiceRollToString(normals)
                + "\nhunger dice: " + DiceRollToString(hunger)
                + "\nsuccesses: " + successes
                + "\ntens: " + tens
                + "\nhungerTens: " + hungerTens
                + "\nisCritical? " + isCritical
                + "\nisMessy?" + isMessy);
            return new Outcome(successes, isCritical, isMessy, false, new List<int>(normals), new List<int>(hunger));
        }

        public static Outcome d10(int normalDice, int hungerDice, int dc)
        {
            System.Random r = new System.Random();
            int[] normals = new int[normalDice];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = r.Next(1, 11);
            }
            int[] hunger = new int[hungerDice];
            for (int i = 0; i < hunger.Length; i++)
            {
                hunger[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(normals, n => n >= 6).Length + Array.FindAll(hunger, h => h >= 6).Length;
            int tens = Array.FindAll(normals, n => n == 10).Length;
            int hungerOnes = Array.FindAll(hunger, h => h == 1).Length;
            int hungerTens = Array.FindAll(hunger, h => h == 10).Length;
            bool isCritical = false;
            if (tens + hungerTens >= 2)
            {
                successes += (tens + hungerTens) / 2;
                isCritical = true;
            }
            bool isMessy = isCritical && hungerTens > 0;
            bool isBestial = hungerOnes > 0 && successes < dc;
            Debug.Log("d10:"
                + "\nnormal dice: " + DiceRollToString(normals)
                + "\nhunger dice: " + DiceRollToString(hunger)
                + "\nsuccesses: " + successes
                + "\nDC: " + dc
                + "\ntens: " + tens
                + "\nhungerTens: " + hungerTens
                + "\nisCritical? " + isCritical
                + "\nisMessy?" + isMessy
                + "\nisBestial? " + isBestial);
            return new Outcome(successes, isCritical, isMessy, isBestial, new List<int>(normals), new List<int>(hunger));
        }

        public static Outcome d10(int dice)
        {
            System.Random r = new System.Random();
            int[] roll = new int[dice];
            for (int i = 0; i < roll.Length; i++)
            {
                roll[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(roll, n => n >= 6).Length;
            int tens = Array.FindAll(roll, n => n == 10).Length;
            bool isCritical = false;
            if (tens >= 2)
            {
                successes += tens / 2;
                isCritical = true;
            }
            Debug.Log("d10:"
                + "\ndice: " + DiceRollToString(roll)
                + "\nsuccesses: " + successes
                + "\ntens: " + tens
                + "\nisCritical? " + isCritical);
            return new Outcome(successes, isCritical, new List<int>(roll));
        }

        private static string DiceRollToString(int[] roll)
        {
            string result = "";
            foreach (int i in roll)
            {
                result = result + i + ", ";
            }
            return result;
        }
    }
}