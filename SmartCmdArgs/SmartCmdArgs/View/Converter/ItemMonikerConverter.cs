﻿using System;
using System.Globalization;
using Microsoft.VisualStudio.Imaging;
using SmartCmdArgs.Helper;
using SmartCmdArgs.ViewModel;

namespace SmartCmdArgs.View.Converter
{
	class ItemMonikerConverter : MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var item = values[0];
            var isExpanded = (bool) values[1];
            if (item is CmdGroup grp)
            {
                if (isExpanded)
                    return KnownMonikers.FolderOpened;
                else
                    return KnownMonikers.FolderClosed;
            }
            if (item is CmdProject prj)
            {
                if (prj.Kind == ProjectKinds.CS) return KnownMonikers.CSProjectNode;
                if (prj.Kind == ProjectKinds.CSCore) return KnownMonikers.CSProjectNode;
                if (prj.Kind == ProjectKinds.CPP) return KnownMonikers.CPPProjectNode;
                if (prj.Kind == ProjectKinds.VB) return KnownMonikers.VBProjectNode;
                if (prj.Kind == ProjectKinds.FS) return KnownMonikers.FSProjectNode;
                if (prj.Kind == ProjectKinds.Node) return KnownMonikers.JSProjectNode;
                if (prj.Kind == ProjectKinds.Py) return KnownMonikers.PYProjectNode;
                return KnownMonikers.CSProjectNode;
            }
            return null;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
