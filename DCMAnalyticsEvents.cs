
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
	/// A mapping of GameAnalytics states to the possible game states found
	/// in Iris Burning.
	/// </summary>
	public enum AMatchEvent : int {
		Start = 0,
		Win = 1,
		Loss = 2
	}
	
	/// <summary>
	/// Extension method for AMatchEvent enumeration.
	/// </summary>
	public static class AMatchEventExtensions {
		/// <summary>
		/// Converts this AMatchEvent to a GameAnalytics-friendly string.
		/// </summary>
		public static string ToAnalyticsString(this AMatchEvent val) {
			switch (val) {
				case AMatchEvent.Start:
					return "match_start";
				case AMatchEvent.Win:
					return "match_win";
				case AMatchEvent.Loss:
					return "match_loss";
				default:
					return "invalid_event"
			}
		}
	}
	
}
