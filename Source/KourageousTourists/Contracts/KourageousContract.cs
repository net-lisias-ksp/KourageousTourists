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

using Contracts;
using TP = KSPe.Util.TextProcessing;

namespace KourageousTourists
{
	public class KourageousContract : Contract
	{
		public const String cfgRoot = "KOURAGE";

		protected CelestialBody targetBody = null;
		protected int numTourists;
		protected List<ProtoCrewMember> tourists;
		protected string hashString;

		private float _difficultyMultiplier = 1.0f;
		protected float difficultyMultiplier {
			get => this._difficultyMultiplier;
			set => Math.Max(0.1f, Math.Min(value, 10));
		}

		protected int minTourists = 1;
		protected int maxTourists = 5;

		public KourageousContract() : base() {
			this.tourists = new List<ProtoCrewMember> ();
			this.hashString = "";
			this.numTourists = 0;
		}

		public bool hasTourist(string tourist) {
			foreach (ProtoCrewMember crew in tourists) {
				if (crew.name == tourist) {
					return true;
				}
			}
			return false;
		}

		public override bool CanBeCancelled() {
			// TODO: Let's make that if any tourist is out of Kerbin,
			// the contract can't be cancelled
			return true;
		}

		public override bool CanBeDeclined() {
			return true;
		}

		public override bool MeetRequirements ()
		{
			// Later we should offer the contract only after some other tourist contract were completed
			return true;
		}

	#region Difficulty Multiplier

		protected new void SetFunds(float advance, float completion, CelestialBody body = null)
			=> base.SetFunds(
				(float)(this.difficultyMultiplier*advance)
				, (float)(this.difficultyMultiplier*completion)
				, body
			);

		protected new void SetFunds(float advance, float completion, float failure, CelestialBody body = null)
			=> base.SetFunds(
				(float)(this.difficultyMultiplier*advance)
				, (float)(this.difficultyMultiplier*completion)
				, failure
				, body
			);

		protected new void SetScience(float completion, CelestialBody body = null)
			=> base.SetScience(
				(float)(this.difficultyMultiplier*completion)
				, body
			);

	#endregion

		protected void SetDeadline(CelestialBody targetBody)
		{
			// TODO: Calculate the Deadline using the distance from homeworld to the targetBody
			base.SetDeadlineYears(1, targetBody);
		}

		protected CelestialBody selectNextCelestialBody()
		{
			List<CelestialBody> allBodies = this.getSelectableBodies();
			if (allBodies.Count < 1) return null;
			return allBodies[UnityEngine.Random.Range(0, allBodies.Count - 1)];
		}

		protected List<CelestialBody> getCelestialBodyList(bool includeHome)
		{
			List<CelestialBody> allBodies = GetBodies_Reached(includeHome, false).ToList();
			Log.detail("celestials: {0}", String.Join(",", allBodies.Select(b => b.ToString()).ToArray()));
			return allBodies;
		}

		// Perhaps this is implemented somewhere in Contract.TextGen
		protected string getProperTouristWord() {

			string t;
			if (this.numTourists > 1000)	t = this.numTourists.ToString();
			else							t = TP.Number.To.Text(this.numTourists, true, "no");

			if (t.Length > 16) t = this.numTourists.ToString();
			return t + " tourist" + (this.numTourists > 1 ? "s" : "");
		}

		protected string getProperTouristWordLc() {
			string t = getProperTouristWord ();
			return char.ToLower (t [0]) + t.Substring (1);
		}

		protected sealed override bool Generate()
		{
			Log.dbg("entered {0} Generate", this.GetType().Name);

			targetBody = selectNextCelestialBody();
			if (targetBody == null)
				return false;

			this.numTourists = UnityEngine.Random.Range(this.minTourists, this.maxTourists);
			Log.dbg("num tourists: {0}", numTourists);

			if (!this.ConfigureContract()) return false;

			for (int i = 0; i < this.numTourists; i++)
			{
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype (ProtoCrewMember.KerbalType.Tourist);

				this.tourists.Add(tourist);
				Log.dbg("generated: {0}", tourist.name);
				this.GenerateTourist(tourist);
			}

			this.GenerateHashString();
			this.GenerateContract();
			return true;
		}

		protected virtual bool ConfigureContract() => false;
		protected virtual void GenerateTourist(ProtoCrewMember tourist) { }
		protected virtual void GenerateContract() { }
		protected virtual List<CelestialBody> getSelectableBodies() => this.getCelestialBodyList(false);

		private void GenerateHashString()
		{
			string hash = this.GetType().FullName + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
			Log.dbg("hashString = {0}", hash);
		}

		protected override string GetHashString() => this.hashString;

		protected override void OnCompleted()
		{
			Log.detail("OnCompleted");

			foreach (ProtoCrewMember tourist in tourists)
			{
				Log.detail("Setting hasToured for {0}", tourist.name);
				KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
				if (roster.Exists(tourist.name))
				{
					ProtoCrewMember t = roster[tourist.name];
					t.type = ProtoCrewMember.KerbalType.Tourist;
					t.hasToured = true;
				}
			}
			base.OnCompleted();
		}

		protected override void OnLoad (ConfigNode node)
		{
			int bodyID = int.Parse(node.GetValue ("targetBody"));
			foreach(CelestialBody body in FlightGlobals.Bodies)
			{
				if (body.flightGlobalsIndex == bodyID)
					targetBody = body;
			}
			ConfigNode touristNode = node.GetNode ("TOURISTS");
			Log.dbg("tourist node: {0}", touristNode);
			if (touristNode == null) {
				Log.warn("Can't load tourists from save file");
				return;
			}
			foreach (ConfigNode crewNode in touristNode.GetNodes()) {
				Log.dbg("tourist: {0}", crewNode);
				this.tourists.Add (
					new ProtoCrewMember (
						HighLogic.CurrentGame.Mode, crewNode, ProtoCrewMember.KerbalType.Tourist));
			}
			this.numTourists = tourists.Count;
			Log.detail("numtourists: {0}; {1}", this.numTourists, tourists.Count);
		}

		protected override void OnSave (ConfigNode node)
		{
			Log.dbg("saving {0} tourists", this.numTourists);
			Log.dbg("node: {0}", node);
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue ("targetBody", bodyID);
			ConfigNode touristNode = node.AddNode ("TOURISTS");
			foreach (ProtoCrewMember tourist in this.tourists) {
				ConfigNode crewNode = touristNode.AddNode ("TOURIST");
				tourist.Save (crewNode);
				Log.dbg("adding tourist: {0}", tourist.name);
			}
			Log.dbg("node: {0}", node);
			Log.dbg("tourist node: {0}", touristNode);
			
		}

		protected static string tokenize(params Object[] args) {
			string result = args[0].ToString();
			for(int i = 1; i < args.Length; ++i)
				result = result.Replace("Token" + (i-1), args[i].ToString());
			return result;
		}

		protected static string trainingHint(string body, string situation, string ability, bool commandSeat = false) {

			string hint = "Please note, tourists should be trained at least to level {0} to be able to disembark the" +
							" vessel on {1}. Training usually could be performed by {2}";

			int requiredLevel;
			for (requiredLevel = 0; requiredLevel < 6; ++requiredLevel)
			{
				ProtoTourist pt = TouristFactory.Instance.createForLevel(requiredLevel);
				if (null == pt) continue;
				bool r = true;
				r &= 0 == pt.celestialBodies.Count || pt.celestialBodies.Contains(body);
				r &= (null == situation || pt.situations.Contains(situation));
				r &= pt.abilities.Contains(ability);
				if(r) break;
			}

			string training;
			switch(requiredLevel)
			{
				case 0:
					training = "All tourists are eligible.";
					break;

				case 1:
					training = "the orbital flight and successful recovery.";
					break;

				default:
					training = "visiting Mun or Minmus and following safe recovery.";
					break;
			}

			return String.Format(hint, requiredLevel, body, training + (0 != requiredLevel && commandSeat?" Or they can be sent on Command Seats!":""));
		}
	}
}

