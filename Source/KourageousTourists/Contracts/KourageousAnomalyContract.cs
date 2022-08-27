/*
	This file is part of Kourageous Tourists /L
		© 2020-2022 LisiasT : http://lisias.net <support@lisias.net>
		© 2017-2020 Nikita Makeev (whale_2)

	Kourageous Tourists /L is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Kourageous Tourists /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Kourageous Tourists /L.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Kourageous Tourists /L.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections.Generic;
using System.Linq;

using FinePrint.Contracts.Parameters;

using KSPe;

namespace KourageousTourists.Contracts
{

	public class KourageousAnomaly
	{
		public string name { get; internal set; }
		public string anomalyDescription { get; internal set; }
		public CelestialBody body { get; internal set; }
		public string contractDescription { get; internal set; }
		public string contractSynopsis { get; internal set; }
		public string contractCompletion { get; internal set; }
		public float payoutModifier { get; internal set; }
		public bool reputationBonus { get; internal set; }
		public float anomalyDiscoveryDistance { get; internal set; }
		public string[] achievementsRequired { get; internal set; }

		public void Save(ConfigNode node) {
			node.AddValue ("anomaly", body.name + ":" + name);
		}

		public static KourageousAnomaly Load(ConfigNode node, Dictionary<String, KourageousAnomaly> anomalies) {
			string anomalyName = node.GetValue ("anomaly");
			return (KourageousAnomaly)anomalies [anomalyName].MemberwiseClone ();
		}
	}

	public class KourageousAnomalyContract : KourageousContract
	{
		public const string cfgNode = "ANOMALY";

		private static KourageousAnomalyContract instance = null;
		internal static KourageousAnomalyContract Instance = instance ?? (instance = new KourageousAnomalyContract());

		internal readonly Dictionary<String, KourageousAnomaly> anomalies = new Dictionary<String, KourageousAnomaly>();
		protected KourageousAnomaly chosenAnomaly;
		private float anomalyDiscoveryDistance = 50.0f;
		private string achievementsRequiredDef = "";

		public KourageousAnomalyContract () : base()
		{
			this.minTourists = 2;
			this.readAnomalyConfig();
		}

		public static void Reload()
		{
			instance = null;
		}

		private void reReadAnomalyConfig()
		{
			this.anomalies.Clear();
			this.readAnomalyConfig();
		}

		private void readAnomalyConfig()
		{
			ConfigNode configRoot = GameDatabase.Instance.GetConfigNodes(KourageousContract.cfgRoot).FirstOrDefault();
			if (null == configRoot) return;

			ConfigNodeWithSteroids config = ConfigNodeWithSteroids.from(configRoot);
			Util.PQS pqs = Util.PQS.Instance;

			this.anomalyDiscoveryDistance = config.GetValue<float>("anomalyDiscoveryDistance", this.anomalyDiscoveryDistance);
			this.achievementsRequiredDef = config.GetValue<string>("achievementsRequired", this.achievementsRequiredDef);

			ConfigNode[] nodes = config.GetNodes(cfgNode);
			foreach (ConfigNode configNode in nodes)
			{
				Log.dbg("cfg node: {0}", configNode);
				ConfigNodeWithSteroids node = ConfigNodeWithSteroids.from(configNode);

				KourageousAnomaly anomaly = new KourageousAnomaly ();

				{
					if (!node.HasValue("name")) continue;
					anomaly.name = node.GetValue<string>("name");
				}
				Log.dbg("anomaly name: {0}", anomaly.name);

				{ 
					if (!node.HasValue("body")) continue;
					String body = node.GetValue<string>("body");
					Log.dbg("anomaly body: {0}", body);
					if (!Util.CelestialBodies.Instance.Exists(body)) continue;
					anomaly.body = Util.CelestialBodies.Instance[body].celestialBody;
				}
				Log.dbg("anomaly body obj: {0}", anomaly.body);

				if(!pqs.Exists(anomaly.body, anomaly.name))	continue;

				{ 
					if (!node.HasValue("anomalyDescription")) continue;
					anomaly.anomalyDescription = node.GetValue<string>("anomalyDescription");
				}
				{ 
					if (!node.HasValue("contractDescription")) continue;
					anomaly.contractDescription = node.GetValue<string>("contractDescription");
				}
				{
					if (!node.HasValue("contractSynopsis")) continue;
					anomaly.contractSynopsis = node.GetValue<string>("contractSynopsis");
				}

				anomaly.payoutModifier = node.GetValue<float>("payoutModifier", 1.0f);
				Log.dbg("payout modifier: {0}", anomaly.payoutModifier);

				anomaly.reputationBonus = node.GetValue<bool>("reputationBonus", false);
				Log.dbg("anomaly reputationBonus: {0}", anomaly.reputationBonus);

				anomaly.anomalyDiscoveryDistance = node.GetValue<float>("anomalyDiscoveryDistance", anomalyDiscoveryDistance);
				Log.dbg("anomaly anomalyDiscoveryDistance: {0}", anomaly.anomalyDiscoveryDistance);

				{ 
					string achievementsRequired = node.GetValue<string>("achievementsRequired", this.achievementsRequiredDef);
					anomaly.achievementsRequired = achievementsRequired.Split(',');
				}
				Log.dbg("anomaly achievementsRequired: {0}", anomaly.achievementsRequired);

				anomalies.Add(anomaly.body + ":" + anomaly.name, anomaly);
				Log.dbg("added: {0}", anomaly.body + ":" + anomaly.name);
			}
		}

		protected KourageousAnomaly chooseAnomaly(CelestialBody body) {

			Log.dbg("entered KourageousAnomallyContract chooseAnomaly");
			reReadAnomalyConfig ();
			Log.dbg("anomalies: {0}, distance: {1}", anomalies.Count, anomalyDiscoveryDistance);

			List<KourageousAnomaly> chosen = new List<KourageousAnomaly> ();
			foreach (KeyValuePair<string, KourageousAnomaly> entry in anomalies)
				if (entry.Value.body.name.Equals (body.name))
					chosen.Add (entry.Value);

			Log.dbg("chosen: {0}, cnt: {1}", chosen, chosen.Count);
			if (chosen.Count == 0)
				return null;

			Random rnd = new Random ();
			return chosen [rnd.Next (chosen.Count)];
		}

		protected override bool ConfigureContract()
		{
			base.ConfigureContract(); // Ignore the return

			this.chosenAnomaly = this.chooseAnomaly(targetBody);
			if (null == this.chosenAnomaly) return false;

			this.achievementsRequired.UnionWith(this.chosenAnomaly.achievementsRequired);
			this.difficultyMultiplier = this.chosenAnomaly.payoutModifier;
			return true;
		}

		protected override void GenerateTourist(ProtoCrewMember tourist)
		{
			// TODO: Add support for gender for 1.3 build
			KerbalTourParameter itinerary = new KerbalTourParameter(tourist.name, tourist.gender);
			// TODO: Add difficulty multiplier
			itinerary.FundsCompletion = 25000.0;
			itinerary.ReputationCompletion = 0.0f;
			itinerary.ReputationFailure = 0.0f;
			itinerary.ScienceCompletion = 0.0f;
			this.AddParameter(itinerary);

			KerbalDestinationParameter dstParameter = new KerbalDestinationParameter(
				targetBody, FlightLog.EntryType.Land, tourist.name
			);
			dstParameter.FundsCompletion = 1000.0f;
			dstParameter.FundsFailure = 0.0f;
			dstParameter.ReputationCompletion = 0.0f;
			dstParameter.ReputationFailure = 0.0f;
			dstParameter.ScienceCompletion = 0.0f;
			/*dstParameter.NestToParent (itinerary);
			dstParameter.CreateID ();
			dstParameter.AddParameter (new Contracts.Parameters.LandOnBody (targetBody));*/
			itinerary.AddParameter(dstParameter);

			KourageousAnomalyParameter anomalyParameter = new KourageousAnomalyParameter(
				targetBody, tourist.name, chosenAnomaly.name, chosenAnomaly.anomalyDescription
			);
			anomalyParameter.FundsCompletion = 1300.0;
			anomalyParameter.FundsFailure = 0.0;
			anomalyParameter.ReputationCompletion = 1.0f;
			anomalyParameter.ReputationFailure = 1.0f;
			anomalyParameter.ScienceCompletion = 0.0f;
			itinerary.AddParameter(anomalyParameter);
		}

		protected override void GenerateContract()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			this.SetExpiry();
			this.SetScience(0.0f, targetBody);
			this.SetDeadline(targetBody);
			this.SetReputation(
				(this.chosenAnomaly.reputationBonus ? (float)(this.numTourists / 2.5) : 2)
				, (this.chosenAnomaly.reputationBonus ? this.numTourists / 2 : 5)
				, targetBody);
			this.SetFunds(
				3000,
				9000,
				21000,
				targetBody);
		}

		protected override List<CelestialBody> getSelectableBodies() => this.getCelestialBodyList(true);

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousAnomallyContract Generate body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}

		protected override string GetTitle () {
			Log.dbg("entered: KourageousAnomallyContract GetTitle anomaly={0}", chosenAnomaly);
			return String.Format("Visit {0} with {1}",
				chosenAnomaly.anomalyDescription,  getProperTouristWordLc());
		}

		protected override string GetDescription() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractDescription, getProperTouristWord(), chosenAnomaly.anomalyDiscoveryDistance,
				trainingHint(chosenAnomaly.body.bodyName, "LANDED", "EVA", true));
		}

		protected override string GetSynopsys() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractSynopsis, getProperTouristWordLc(), anomalyDiscoveryDistance);
		}

		protected override string MessageCompleted ()
		{
			return KourageousContract.tokenize (chosenAnomaly.contractCompletion,
				getProperTouristWordLc (), anomalyDiscoveryDistance);
		}

		protected override void OnSave (ConfigNode node) {
			base.OnSave (node);
			chosenAnomaly.Save(node);
		}

		protected override void OnLoad(ConfigNode node) {
			base.OnLoad (node);
			reReadAnomalyConfig ();
			chosenAnomaly = KourageousAnomaly.Load (node, anomalies);
		}
	}
}

