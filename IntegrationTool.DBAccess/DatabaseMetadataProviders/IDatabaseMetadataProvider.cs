﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public interface IDatabaseMetadataProvider
    {
        List<DbMetadataTable> DatabaseTables { get; set; }
        void Initialize();
    }
}
