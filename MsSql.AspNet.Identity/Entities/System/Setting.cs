namespace MsSql.AspNet.Identity
{
    public class Setting
    {
        public string Name { get; set; }
        /// <summary>
        /// The type is the name of each class which inherits the SettingsBase class
        /// </summary>
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
