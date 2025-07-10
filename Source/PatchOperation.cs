using System.Xml;

namespace PAT.MoveableMonoliths {
	public class PatchOperation : Verse.PatchOperation {
		private string name;
		private string defName;
		private string setting;

		private string minifiedDef;
		private string thingCategory;
		private bool? claimable;
		private float? mass;

		protected override bool ApplyWorker(XmlDocument xml) {
			try {
				if (!CheckSetting(setting)) return true;

				XmlNodeList targets = SelectThing(xml, name, defName);
				foreach (XmlElement target in targets) {
					ApplyPatch(xml, target);
				}

				return true;
			} catch (System.Exception e) {
				Verse.Log.Error($"Moveable Monoliths patch ({name}/{defName}@{setting}) failed: {e}");
				return false;
			}
		}

		private void ApplyPatch(XmlDocument xml, XmlElement root) {
			if (minifiedDef != null) {
				if (minifiedDef == "") {
					for (XmlNode node = root.FirstChild; node != null; node = node.NextSibling) {
						if (node.Name == "minifiedDef") {
							root.RemoveChild(node);
							break;
						}
					}
				} else {
					SetElementValue(xml, root, "minifiedDef", minifiedDef);
				}
			}

			if (thingCategory != null) {
				XmlElement thingCategories = GetOrCreateElement(xml, root, "thingCategories");
				thingCategories.AppendChild(CreateElement(xml, "li", thingCategory));
			}

			if (claimable.HasValue) {
				XmlElement building = GetOrCreateElement(xml, root, "building");
				SetElementValue(xml, building, "claimable", claimable.Value ? "true" : "false");
			}

			if (mass.HasValue) {
				XmlElement statBases = GetOrCreateElement(xml, root, "statBases");
				SetElementValue(xml, statBases, "Mass", mass.Value.ToString());
			}
		}

		private static XmlNodeList SelectThing(XmlDocument xml, string name, string defName) {
			if (!string.IsNullOrEmpty(name)) {
				return xml.SelectNodes($"Defs/ThingDef[@Name=\"{name}\"]");
			}
			if (!string.IsNullOrEmpty(defName)) {
				return xml.SelectNodes($"Defs/ThingDef[defName=\"{defName}\"]");
			}
			throw new System.ApplicationException("PatchOperation does not have \"name\" nor \"defname\"");
		}

		private static bool CheckSetting(string setting) {
			switch ((setting ?? "").Trim().ToLowerInvariant()) {
				case "obelisk": return Settings.Instance.obelisk;
				case "monolith": return Settings.Instance.monolith;
				case "harbringer": return Settings.Instance.harbringer;
				case "pitgate": return Settings.Instance.pitgate;
				default: throw new System.ApplicationException($"PatchOperation has an unknown setting specified: \"{setting}\"");
			}
		}

		private static void SetElementValue(XmlDocument doc, XmlElement root, string name, string content) {
			GetOrCreateElement(doc, root, name).InnerXml = content;
		}
		private static XmlElement GetOrCreateElement(XmlDocument doc, XmlElement root, string name) {
			XmlElement el = root[name];
			if (el == null) root.AppendChild(el = CreateElement(doc, name));
			return el;
		}
		private static XmlElement CreateElement(XmlDocument doc, string name, string content = null) {
			XmlElement el = doc.CreateElement(name);
			if (content != null) el.InnerXml = content;
			return el;
		}
	}
}
