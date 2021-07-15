using Tensorflow;

namespace Menagerie.ML.Models
{
    public class CurrencyTypeModel : CnnModelV1
    {
        #region Constructors

        public CurrencyTypeModel() : base("./training/currency_type/currency_type.h5", "./training/currency_type/currency_type-classes.txt", new TensorShape(46, 46, 3))
        {
        }

        #endregion
    }
}