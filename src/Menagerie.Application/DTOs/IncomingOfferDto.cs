using System.Text.RegularExpressions;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Application.DTOs;

public class IncomingOfferDto : IncomingOffer
{
    #region Constants

    private static readonly Regex _regNormalizeName = new(@"\(T[0-9]+\)|[0-9\.]+ "); 
    #endregion

    public string ItemNameNormalized => _regNormalizeName.Replace(ItemName, "");

}