
using eppo_sdk.dto;
using eppo_sdk.dto.bandit;
using static NUnit.Framework.Assert;

namespace eppo_sdk_test.dto;


public class BanditFlagsTest
{
    [Test]
    public void ShouldIndicateBanditFlags()
    {
        var banditFlags = new BanditFlags()
        {
            // Typical `BanditVariationDto` values where `variation` is duplicated across the VariationValue, VariationKey and BanditKey fields.
            ["variation"] = new BanditVariation[] {new("variation", "banditFlagKey", "variation", "variation")},

            ["banditKey"] = new BanditVariation[] {new("banditKey", "flagKey", "variationKey", "variationValue")}
        };

        Multiple(() =>
        {
            That(banditFlags.IsBanditFlag("notAFlag"), Is.False);
            That(banditFlags.IsBanditFlag("banditFlagKey"), Is.True);

            That(banditFlags.IsBanditFlag("banditKey"), Is.False);
            That(banditFlags.IsBanditFlag("variationKey"), Is.False);
            That(banditFlags.IsBanditFlag("variationValue"), Is.False);
            That(banditFlags.IsBanditFlag("flagKey"), Is.True);
        });
    }

    [Test]
    public void ShouldFindBanditByFlagAndVariation()
    {
        var banditFlags = new BanditFlags()
        {
            // Typical `BanditVariationDto` values where `variation` is duplicated across the VariationValue, VariationKey and BanditKey fields.
            ["variation"] = new BanditVariation[] {new("variation", "banditFlagKey", "variation", "variation")},

            ["banditKey"] = new BanditVariation[] {new("banditKey", "flagKey", "variationKey", "variationValue")}
        };

        // Will match
        var result1 = banditFlags.TryGetBanditKey("banditFlagKey","variation", out string? bandit1);
        var result2 = banditFlags.TryGetBanditKey("flagKey","variationValue", out string? bandit2);

        // Will not match (has to match variationValue)
        var noResult = banditFlags.TryGetBanditKey("flagKey","banditKey", out string? banditNoMatch);

        Multiple(() =>
        {
            That(result1, Is.True);
            That(bandit1, Is.Not.Null);
            That(bandit1, Is.EqualTo("variation"));

            That(result2, Is.True);
            That(bandit2, Is.Not.Null);
            That(bandit2, Is.EqualTo("banditKey"));

            That(noResult, Is.False);
            That(banditNoMatch, Is.Null);
        });
    }
}
