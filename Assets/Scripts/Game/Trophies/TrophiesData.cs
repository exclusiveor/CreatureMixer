using UnityEngine;
using System.Collections;

public enum ETrophyType
{
	None = 0,
    AchieveXpoints_0_IOG,
    AchieveXpoints_1_IOG,
    AchieveXpoints_2_IOG,
    AchieveXpoints_3_IOG,
    AchieveXpoints_4_IOG,
    ReachXpoints_0_COM,
    ReachXpoints_1_COM,
    ReachXpoints_2_COM,
    ReachXpoints_3_COM,
    ReachXpoints_4_COM,
    MadeXmoves_0_IOG,
    MadeXmoves_1_IOG,
    MadeXmoves_2_IOG,
    MadeXmoves_3_IOG,
    MadeXmoves_4_IOG,
    DestroyXpipesWithChain_0_IOG,
    DestroyXpipesWithChain_1_IOG,
    DestroyXpipesWithChain_2_IOG,
    DestroyXpipesWithChain_3_IOG,
    DestroyXpipesWithChain_4_IOG,
    DestroyXpipesWithChain_0_COM,
    DestroyXpipesWithChain_1_COM,
    DestroyXpipesWithChain_2_COM,
    DestroyXpipesWithChain_3_COM,
    DestroyXpipesWithChain_4_COM,
    CreateTileWithLevelX_0,
    CreateTileWithLevelX_1,
    CreateTileWithLevelX_2,
    CreateTileWithLevelX_3,
    CreateTileWithLevelX_4,
    HaveXblockersAtLevel_0,
    HaveXblockersAtLevel_1,
    HaveXblockersAtLevel_2,
    HaveXblockersAtLevel_3,
    HaveXblockersAtLevel_4,
    GetAllOtherTrophies
}

[System.Serializable]
public class TrophyData 
{
	public int								Reward;
	public int								Param;
	public bool								IsSingle;
}

[System.Serializable]
public class TrophyCompleteData 
{
	public int								Param;				// progress or other
	public bool								Completed;
}