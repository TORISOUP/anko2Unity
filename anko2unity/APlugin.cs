namespace anko2unity {
	public abstract class APlugin : ankoPlugin2.IPlugin {

		#region IPlugin メンバー

		public abstract string Description { get; }

		public abstract bool IsAlive { get; }

		public abstract string Name { get; }

		public abstract void Run();

		public virtual ankoPlugin2.IPluginHost host { get; set; }

		#endregion

	}
}