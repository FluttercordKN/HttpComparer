namespace HttpComparer
{
	public class PatchOperation
	{
		public string Op { get; set; }
		public string Path { get; set; }
		public object Value { get; set; }
	}
}
