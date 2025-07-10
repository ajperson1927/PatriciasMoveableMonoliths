using UnityEngine;
using Verse;

namespace PAT.MoveableMonoliths {
	public class ModMain : Mod {
		public Settings Settings => GetSettings<Settings>();

		public ModMain(ModContentPack content) : base(content) {}

		public override string SettingsCategory() {
			return Content.Name;
		}

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard() {
			};
			listing.Begin(inRect);

			Settings s = Settings;
			listing.Label("PAT.ApplyPatch".Translate());
			listing.CheckboxLabeled("PAT.Obelisk".Translate(), ref s.obelisk, "PAT.RequiresRestart".Translate());
			listing.CheckboxLabeled("PAT.Monolith".Translate(), ref s.monolith, "PAT.RequiresRestart".Translate());
			listing.CheckboxLabeled("PAT.Harbinger".Translate(), ref s.harbringer, "PAT.RequiresRestart".Translate());
			listing.CheckboxLabeled("PAT.Pitgate".Translate(), ref s.pitgate, "PAT.RequiresRestart".Translate());

			listing.End();
		}
	}
}
