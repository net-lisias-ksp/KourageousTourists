using System.Collections.Generic;

namespace KourageousTourists
{
	public class ModuleManagerSupport : UnityEngine.MonoBehaviour
	{
		public static IEnumerable<string> ModuleManagerAddToModList()
		{
			string[] r = {typeof(ModuleManagerSupport).Namespace};
			return r;
		}
	}
}
