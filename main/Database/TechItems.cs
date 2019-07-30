using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class TechItems : ResourceSet<TechItem>
	{
		public const string AUTOMATION_OVERLAY_ID = "AutomationOverlay";

		public TechItem automationOverlay;

		public const string SUITS_OVERLAY_ID = "SuitsOverlay";

		public TechItem suitsOverlay;

		public const string JET_SUIT_ID = "JetSuit";

		public TechItem jetSuit;

		public const string BETA_RESEARCH_POINT_ID = "BetaResearchPoint";

		public TechItem betaResearchPoint;

		public const string GAMMA_RESEARCH_POINT_ID = "GammaResearchPoint";

		public TechItem gammaResearchPoint;

		public const string CONVEYOR_OVERLAY_ID = "ConveyorOverlay";

		public TechItem conveyorOverlay;

		public TechItems(ResourceSet parent)
			: base("TechItems", parent)
		{
			automationOverlay = AddTechItem("AutomationOverlay", RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.DESC, GetSpriteFnBuilder("overlay_logic"));
			suitsOverlay = AddTechItem("SuitsOverlay", RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.DESC, GetSpriteFnBuilder("overlay_suit"));
			betaResearchPoint = AddTechItem("BetaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.DESC, GetSpriteFnBuilder("research_type_beta_icon"));
			gammaResearchPoint = AddTechItem("GammaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.DESC, GetSpriteFnBuilder("research_type_gamma_icon"));
			conveyorOverlay = AddTechItem("ConveyorOverlay", RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.DESC, GetSpriteFnBuilder("overlay_conveyor"));
			jetSuit = AddTechItem("JetSuit", RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.NAME, RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.DESC, GetSpriteFnBuilder("overlay_suit"));
		}

		private Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName)
		{
			return (string anim, bool centered) => Assets.GetSprite(spriteName);
		}

		public TechItem AddTechItem(string id, string name, string description, Func<string, bool, Sprite> getUISprite)
		{
			if (TryGet(id) != null)
			{
				DebugUtil.LogWarningArgs("Tried adding a tech item called", id, name, "but it was already added!");
				return Get(id);
			}
			Tech tech = LookupGroupForID(id);
			if (tech == null)
			{
				return null;
			}
			TechItem techItem = new TechItem(id, this, name, description, getUISprite, tech);
			Add(techItem);
			tech.unlockedItems.Add(techItem);
			return techItem;
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (TechItem resource in resources)
			{
				if (resource.Id == id)
				{
					return resource.IsComplete();
				}
			}
			return true;
		}

		public Tech LookupGroupForID(string itemID)
		{
			foreach (KeyValuePair<string, string[]> item in Techs.TECH_GROUPING)
			{
				if (Array.IndexOf(item.Value, itemID) != -1)
				{
					return Db.Get().Techs.Get(item.Key);
				}
			}
			return null;
		}
	}
}
