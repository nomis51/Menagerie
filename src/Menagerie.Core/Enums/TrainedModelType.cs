namespace Menagerie.Core.Enums
{
    public enum TrainedModelType
    {
        CurrencyType,
        StackSize,
        ItemLinks,
        ItemSockets,
        SocketColor
    }

    public static class TrainedModelTypeConverter
    {
        public static string Convert(TrainedModelType trainedModelType)
        {
            return trainedModelType switch
            {
                TrainedModelType.CurrencyType => "currency_type",
                _ => string.Empty
            };
        }
    }
}