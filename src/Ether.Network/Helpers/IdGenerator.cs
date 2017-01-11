namespace Ether.Network.Helpers
{
    /// <summary>
    /// Contains usefull methods.
    /// </summary>
    public static partial class Helper
    {
        private static int id = 0;

        /// <summary>
        /// Generates an unique Id.
        /// </summary>
        /// <returns></returns>
        public static int GenerateUniqueId()
        {
            return ++id;
        }
    }
}
