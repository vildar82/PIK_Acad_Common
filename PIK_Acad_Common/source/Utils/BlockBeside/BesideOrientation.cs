using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.WPF;
using AcadLib.WPF.Converters;

namespace PIK_Acad_Common.Utils.BlockBeside
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BesideOrientation
    {
        /// <summary>
        /// Расстановка блоков в ряд - на одной отметке Y
        /// </summary>
        [Description("В ряд")]
        Ряд,
        /// <summary>
        /// Расстановка блоков в столбик - по вертикале
        /// </summary>
        [Description("В столбик")]
        Столбик
    }
}
