namespace StorageCore.DbHelper
{
    public class Join
    {
        public string TableName { get; set; }
        public string JoinsToTableName { get; set; }
        public string JoinColumn { get; set; }
        public string JoinsToColumn { get; set; }
        public string[] Columns { get; set; }
        public JoinType JoinType { get; set; } = JoinType.Left;

        /// <summary>
        /// Could be null, Use if you need join one table two times
        /// </summary>
        public string JoinAs { get; set; }

        public bool JoinAsTrue => !string.IsNullOrEmpty(JoinAs);
    }
}