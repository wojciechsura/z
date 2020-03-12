﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Models
{
    public class OpenDialogResult
    {
        public OpenDialogResult(bool result, string fileName)
        {
            Result = result;
            FileName = fileName;
        }

        public bool Result { get; }
        public string FileName { get; }
    }
}
