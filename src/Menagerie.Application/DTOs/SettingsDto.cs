using Menagerie.Application.DTOs.Settings;

namespace Menagerie.Application.DTOs;

public class SettingsDto : Shared.Models.Setting.Settings
{
    public new GeneralSettingsDto General { get; set; }
    public new IncomingTradesSettingsDto IncomingTrades { get; set; }
    public new OutgoingTradesSetingsDto OutgoingTrades { get; set; }
    public new ChaosRecipeSettingsDto ChaosRecipe { get; set; }
    public new StashTabGridSettingsDto StashTabGrid { get; set; }
}