namespace HttpComparer
{
	public class DiffRecord
	{
		public string Op { get; set; }
		public string Path { get; set; }
		public object BaseValue { get; set; }
		public object SideValue { get; set; }
	}
}
