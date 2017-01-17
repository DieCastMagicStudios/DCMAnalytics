
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
 
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DieCastMagic.AnalyticsIntegration {
	/// <summary>
	/// A script assigned in the editor to allow telemetry calls to be posted
	/// to the GameAnalytics frontend.
	/// </summary>
	public sealed class StartAnalytics {
		
		void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	
		void Start() {
			// If we are not on the main menu and there is currently an assigned
			// IrisPlayer, initialize using default means.  Otherwise, just
			// initialize using the current win condition.
			DCMAnalytics.Init((
				IrisPlayer.current != null 
				&& SceneManager.GetActiveScene().buildIndex == 0) ?
				IrisPlayer.current.WinCondition : 1);
		}
		
	}
}
