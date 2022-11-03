using System;
using System.Collections.Generic;

namespace NonMonoBehaviourPlayerLoop
{
	public enum UpdateType
	{
		EarlyUpdate = 2,
		FixedUpdate = 3,
		PreUpdate = 4,
		Update = 5,
		PreLateUpdate = 6,
		PostLateUpdate = 7
	}

	internal static class UpdateTypeExtension
	{
		private static readonly Dictionary<UpdateType, Type> _updates = new Dictionary<UpdateType, Type> {
				{UpdateType.EarlyUpdate, typeof(UnityEngine.PlayerLoop.EarlyUpdate)}, 
				{UpdateType.FixedUpdate, typeof(UnityEngine.PlayerLoop.FixedUpdate)},
				{UpdateType.PreUpdate, typeof(UnityEngine.PlayerLoop.PreUpdate)},
				{UpdateType.Update, typeof(UnityEngine.PlayerLoop.Update)},
				{UpdateType.PreLateUpdate, typeof(UnityEngine.PlayerLoop.PreLateUpdate)},
				{UpdateType.PostLateUpdate, typeof(UnityEngine.PlayerLoop.PostLateUpdate)}
		};

		public static Type ToType(this UpdateType playerUpdateType) => _updates[playerUpdateType];
		public static int ToIndex(this UpdateType playerUpdateType) => (int)playerUpdateType;
		public static UpdateType FromIndex(int index) => (UpdateType)index;
	}
}