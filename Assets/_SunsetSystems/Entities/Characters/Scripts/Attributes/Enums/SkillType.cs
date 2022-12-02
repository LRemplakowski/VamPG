public enum SkillType
{
    /*--PHYSICAL--*/
    Athletics = 1, Brawl = 1 << 1, Craft = 1 << 2, Drive = 1 << 3, Firearms = 1 << 4, Larceny = 1 << 5, Melee = 1 << 6, Stealth = 1 << 7, Survival = 1 << 8,
    /*--SOCIAL--*/
    AnimalKen = 1 << 9, Etiquette = 1 << 10, Insight = 1 << 11, Intimidation = 1 << 12, Leadership = 1 << 13, Performance = 1 << 14, Persuasion = 1 << 15, Streetwise = 1 << 16, Subterfuge = 1 << 17,
    /*--MENTAL--*/
    Academics = 1 << 18, Awareness = 1 << 19, Finance = 1 << 20, Investigation = 1 << 21, Medicine = 1 << 22, Occult = 1 << 23, Politics = 1 << 24, Science = 1 << 25, Technology = 1 << 26,
    Invalid = 1 << 27
}
