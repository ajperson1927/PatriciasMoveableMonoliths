using Verse;

namespace PAT.MoveableMonoliths {
	public class Settings : ModSettings {
		private static Settings instance;
		public static Settings Instance {
			get {
				if (instance == null) instance = LoadedModManager.GetMod<ModMain>().Settings;
				return instance;
			}
		}

		public bool obelisk = true;
		public bool monolith = true;
		public bool harbringer = true;
		public bool pitgate = true;

		public override void ExposeData() {
			Scribe_Values.Look(ref obelisk, "obelisk");
			Scribe_Values.Look(ref monolith, "monolith");
			Scribe_Values.Look(ref harbringer, "harbringer");
			Scribe_Values.Look(ref pitgate, "pitgate");
		}
	}
}
