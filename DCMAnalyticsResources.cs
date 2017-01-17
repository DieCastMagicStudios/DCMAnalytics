
/* 
 * Copyright 2017 Die-Cast Magic Studios, LLC.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

namespace DieCastMagic.AnalyticsIntegration {
	/// <summary>
	/// An enumeration of the possible currencies/resources found in the game.
	/// </summary>
	public enum AResource : int {
		Cash = 0,
		Iridium = 1,
		Research = 2,
		Population = 3,
		Favor = 4
	}
	
	/// <summary>
	/// Extension methods for the AResource enumeration.
	/// </summary>
	public static class AResourceExtensions {
		/// <summary>
		/// Returns a GameAnalytics-friendly string from this AResource.
		/// </summary>
		public static string ToAnalyticsString(this AResource val) {
			switch (val) {
				case AResource.Cash:
					return "cash";
				case AResource.Iridium:
					return "iridium";
				case AResource.Research:
					return "research";
				case AResource.Population:
					return "population";
				case AResource.Favor
					return "favor";
				default:
					return "invalid_resource"
			}
		}
	}
	
	/// <summary>
	/// An enumeration of the possible avenues of gainin/losing resources in
	/// Iris Burning.
	/// </summary>
	public enum AResourceType : int {
		Found = 0,
		Sold = 1,
		Gifted = 2,
		Subsidiary = 3,
		Stolen = 4,
	}
	
	/// <summary>
	/// Extension helper methods for the AResourceType enumeration.
	/// </summary>
	public static class AResourceTypeExtensions {
		/// <summary>
		/// Returns a GameAnalytics-friendly string from this AResourceType.
		/// </summary>
		public static string ToAnalyticsString(this AResourceType val) {
			switch (val) {
				case AResourceType.Found:
					return "found";
				case AResourceType.Sold:
					return "sold";
				case AResourceType.Gifted:
					return "gifted";
				case AResourceType.Subsidiary:
					return "subsidiary";
				case AResourceType.Stolen:
					return "stolen";
				default:
					return "invalid_resource_type";
			}
		}
	}
	
	/// <summary>
	/// An enumeration of WHERE a resource came from, i.e. a building or unit.
	/// </summary>
	public enum AResourceAcquisition : int {
		BuildingGenerated = 0,
		UnitGenerated = 1,
		NPCGenerated = 2
	}
	
	/// <summary>
	/// Extension helper methods for the AResourceType enumeration.
	/// </summary>
	public static class AResourceAcquisitionExtensions {
		/// <summary>
		/// Returns a GameAnalytics-friendly string from this AResourceAcquisition.
		/// </summary>
		public static string ToAnalyticsString(this AAResourceAcquisition val) {
			switch (val) {
				case AResourceAcquisition.BuildingGenerated:
					return "building_generated";
				case AResourceAcquisition.Cash:
					return "unit_generated";
				case AResourceAcquisition.Cash:
					return "npc_generated";
				default:
					return "invalid_resource_acqisition";
		}
	}
}
