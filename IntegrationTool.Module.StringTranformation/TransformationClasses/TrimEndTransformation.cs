﻿using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationTool.Module.StringTranformation.TransformationClasses
{
    [StringTransformationAttribute(
        TransformationType = StringTransformationType.TrimEnd,
        Param1Label = "Trimcharacter",
        Param1Visibility = Visibility.Visible,
        Param2Label = "",
        Param2Visibility = Visibility.Hidden)]
    public class TrimEndTransformation : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            if (!String.IsNullOrEmpty(stringTransformation.Parameter1))
            {
                return inputString.TrimEnd(stringTransformation.Parameter1[0]);
            }
            else
            {
                return inputString.TrimEnd();
            }
        }
    }
}
