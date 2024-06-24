using eppo_sdk.dto.bandit;

namespace eppo_sdk.dto;

public interface IBanditFlags : IDictionary<string, BanditVariation[]>
{
    public bool IsBanditFlag(string FlagKey);
    public bool TryGetBanditKey(string flagKey, string assignment, out string? banditKey);
}

public class BanditFlags : Dictionary<string, BanditVariation[]>, IBanditFlags
{
    public bool IsBanditFlag(string flagKey) => this.Any(kvp => kvp.Value.Any(bv => bv.FlagKey == flagKey));

    public bool TryGetBanditKey(string flagKey, string assignment, out string? banditKey)
    {
        foreach (var kvp in this)
        {
            var bandit = kvp.Value.FirstOrDefault(item => item?.FlagKey == flagKey, null);
            if (bandit != null)
            {
                if (bandit.VariationValue == assignment)
                {
                    banditKey = bandit.Key;
                    return true;
                }
            }
        }
        banditKey = null;
        return false;
    }
}
