namespace StorageCore.DbHelper
{
    public class Filter
    {
        public Filter() { }

        public Filter(string name, Comparison comparison)
        {
            this.Name = name;
            this.Comparison = comparison;
        }

        public string Name { get; init; }
        public Comparison Comparison { get; init; }
        public object Value { get; set; }
    }
}