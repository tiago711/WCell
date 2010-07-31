using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WCell.Constants.Achievements;
using WCell.Core;
using WCell.Core.DBC;
using WCell.Core.Initialization;
using WCell.RealmServer.Entities;

namespace WCell.RealmServer.Achievement
{
	internal delegate AchievementCriteriaEntry AchievementCriteriaEntryCreator();

	/// <summary>
	/// Global container for Achievement-related data
	/// </summary>
	public static class AchievementMgr
	{
		private static readonly AchievementCriteriaEntryCreator[] AchievementEntryCreators =
			new AchievementCriteriaEntryCreator[(int)AchievementCriteriaType.End];

		public static AchievementCriteriaEntry[] CriteriaEntries = new AchievementCriteriaEntry[13000];

		public static readonly Dictionary<AchievementEntryId, AchievementEntry> AchievementEntries = new Dictionary<AchievementEntryId, AchievementEntry>();

		public static readonly Dictionary<AchievementCategoryEntryId, AchievementCategoryEntry> AchievementCategoryEntries = new Dictionary<AchievementCategoryEntryId, AchievementCategoryEntry>();

		[Initialization(InitializationPass.Fifth)]
		public static void InitAchievements()
		{
			LoadCriteria();
		}

		public static void LoadCriteria()
		{
			SetEntryCreator(AchievementCriteriaType.KillCreature, () => new KillCreatureAchievementCriteriaEntry());                            // 0
			SetEntryCreator(AchievementCriteriaType.WinBg, () => new WinBattleGroundAchievementCriteriaEntry());                                // 1
			SetEntryCreator(AchievementCriteriaType.ReachLevel, () => new ReachLevelAchievementCriteriaEntry());                                // 5
			SetEntryCreator(AchievementCriteriaType.ReachSkillLevel, () => new ReachSkillLevelAchievementCriteriaEntry());                      // 7
			SetEntryCreator(AchievementCriteriaType.CompleteAchievement, () => new CompleteAchievementAchievementCriteriaEntry());              // 8
			SetEntryCreator(AchievementCriteriaType.CompleteQuestCount, () => new CompleteQuestCountAchievementCriteriaEntry());                // 9
			SetEntryCreator(AchievementCriteriaType.CompleteDailyQuestDaily, () => new CompleteDailyQuestDailyAchievementCriteriaEntry());      // 10
			SetEntryCreator(AchievementCriteriaType.CompleteQuestsInZone, () => new CompleteQuestsInZoneAchievementCriteriaEntry());            // 11
			SetEntryCreator(AchievementCriteriaType.CompleteDailyQuest, () => new CompleteDailyQuestAchievementCriteriaEntry());                // 14
			SetEntryCreator(AchievementCriteriaType.CompleteBattleground, () => new CompleteBattlegroundAchievementCriteriaEntry());            // 15
			SetEntryCreator(AchievementCriteriaType.DeathAtMap, () => new DeathAtMapAchievementCriteriaEntry());                                // 16
			SetEntryCreator(AchievementCriteriaType.DeathInDungeon, () => new DeathInDungeonAchievementCriteriaEntry());                        // 18
			SetEntryCreator(AchievementCriteriaType.CompleteRaid, () => new CompleteRaidAchievementCriteriaEntry());                            // 19
			SetEntryCreator(AchievementCriteriaType.KilledByCreature, () => new KilledByCreatureAchievementCriteriaEntry());                    // 20
			SetEntryCreator(AchievementCriteriaType.FallWithoutDying, () => new FallWithoutDyingAchievementCriteriaEntry());                    // 24
			SetEntryCreator(AchievementCriteriaType.DeathsFrom, () => new DeathsFromAchievementCriteriaEntry());                                // 26
			SetEntryCreator(AchievementCriteriaType.CompleteQuest, () => new CompleteQuestAchievementCriteriaEntry());                          // 27
			SetEntryCreator(AchievementCriteriaType.BeSpellTarget, () => new BeSpellTargetAchievementCriteriaEntry());                          // 28
			SetEntryCreator(AchievementCriteriaType.BeSpellTarget2, () => new BeSpellTargetAchievementCriteriaEntry());                         // 69
			SetEntryCreator(AchievementCriteriaType.CastSpell, () => new CastSpellAchievementCriteriaEntry());                                  // 29
			SetEntryCreator(AchievementCriteriaType.CastSpell2, () => new CastSpellAchievementCriteriaEntry());                                 // 110
			SetEntryCreator(AchievementCriteriaType.HonorableKillAtArea, () => new HonorableKillAtAreaAchievementCriteriaEntry());              // 31
			SetEntryCreator(AchievementCriteriaType.WinArena, () => new WinArenaAchievementCriteriaEntry());                                    // 32
			SetEntryCreator(AchievementCriteriaType.PlayArena, () => new PlayArenaAchievementCriteriaEntry());                                  // 33
			SetEntryCreator(AchievementCriteriaType.LearnSpell, () => new LearnSpellAchievementCriteriaEntry());                                // 34
			SetEntryCreator(AchievementCriteriaType.OwnItem, () => new OwnItemAchievementCriteriaEntry());                                      // 36
			SetEntryCreator(AchievementCriteriaType.WinRatedArena, () => new WinRatedArenaAchievementCriteriaEntry());                          // 37
			SetEntryCreator(AchievementCriteriaType.HighestTeamRating, () => new HighestTeamRatingAchievementCriteriaEntry());                  // 38
			SetEntryCreator(AchievementCriteriaType.ReachTeamRating, () => new ReachTeamRatingAchievementCriteriaEntry());                      // 39
			SetEntryCreator(AchievementCriteriaType.LearnSkillLevel, () => new LearnSkillLevelAchievementCriteriaEntry());                      // 40
			//TODO: Add more types.


			new DBCReader<AchievementEntryConverter>(RealmServerConfiguration.GetDBCFile(WCellDef.DBC_ACHIEVEMENTS));
			new DBCReader<AchievementCategoryEntryConverter>(RealmServerConfiguration.GetDBCFile(WCellDef.DBC_ACHIEVEMENT_CATEGORIES));
			//new DBCReader<AchievementCriteriaEntryMapper>(RealmServerConfiguration.GetDBCFile(WCellDef.DBC_ACHIEVEMENT_CRITERIAS));
		}

		internal static AchievementCriteriaEntryCreator GetCriteriaEntryCreator(AchievementCriteriaType criteria)
		{
			return AchievementEntryCreators[(int)criteria];
		}

		internal static void SetEntryCreator(AchievementCriteriaType criteria, AchievementCriteriaEntryCreator creator)
		{
			AchievementEntryCreators[(int)criteria] = creator;
		}

		public static AchievementEntry Get(AchievementEntryId achievementEntryId)
		{
			return AchievementEntries[achievementEntryId];
		}

		public static AchievementCategoryEntry Get(AchievementCategoryEntryId achievementCategoryEntryId)
		{
			return AchievementCategoryEntries[achievementCategoryEntryId];
		}

		public static void UpdateAchievementCriteria(AchievementCriteriaType criteria, uint value1)
		{
			UpdateAchievementCriteria(criteria, value1, 0, null);
		}

		public static void UpdateAchievementCriteria(AchievementCriteriaType criteria, uint value1, Unit unit)
		{
			UpdateAchievementCriteria(criteria, value1, 0, unit);
			
		}
		public static void UpdateAchievementCriteria(AchievementCriteriaType criteria, uint value1, uint value2)
		{
			UpdateAchievementCriteria(criteria, value1, value2, null);
		}

		public static void UpdateAchievementCriteria(AchievementCriteriaType criteria, uint value1, uint value2, Unit unit)
		{
			
		}
	}
}