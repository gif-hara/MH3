using HK;

namespace MH3
{
    public static partial class Extensions
    {
        public static UIViewListElementBuilder CreateListElementBuilder(this HKUIDocument elementDocument)
        {
            return new UIViewListElementBuilder(elementDocument);
        }
    }
}
