﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using MicroMvvm;

namespace PIK_Acad_Common.Utils.SelectBlockByParam.UI
{
    public class PropertyViewModel : ObservableObject
    {
        public PropertyViewModel (Property property)
        {
            Property = property;
        }
        
        public Property Property { get; set; }

        public bool IsChecked { get { return isChecked; } set { isChecked = value; RaisePropertyChanged(); } }
        bool isChecked;
    }
}
