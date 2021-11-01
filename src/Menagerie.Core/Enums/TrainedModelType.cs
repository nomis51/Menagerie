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
                TrainedModelType.StackSize => "stack_size",
                TrainedModelType.ItemLinks => "item_links",
                TrainedModelType.ItemSockets => "item_sockets",
                TrainedModelType.SocketColor => "socket_color",
                _ => string.Empty
            };
        }
    }
}