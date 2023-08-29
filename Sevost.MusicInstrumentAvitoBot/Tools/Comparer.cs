namespace AvitoMusicInstrumentsBot.GuitarModels.Tools
{
    public class Comparer
    {
        protected bool Compare(string[] possibleTitles, string stringToCompare)
        {
            foreach (var possibleTitle in possibleTitles)
                if (stringToCompare.Contains(possibleTitle))
                    return true;

            return false;
        }

        protected bool IsItGoodPrice(int price, int priceThreshold)
            => price <= priceThreshold;
    }
}